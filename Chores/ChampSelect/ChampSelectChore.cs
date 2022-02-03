using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueChores
{
	enum SkinSelectionState
	{
		NoneSelected,
		IsSelecting,
		HasSelected
	}

	internal class ChampSelectChore
	{
		static readonly string skinUpdateUri = "/lol-champ-select/v1/skin-carousel-skins";
		static readonly string currentSkinUpdateUri = "/lol-champ-select/v1/skin-selector-info";
		static readonly int[] urfIcons = new int[] { 782, 783, 784 };
		static readonly int urfResetDelaySec = 25;

		// URF icons
		InventoryItem[] m_icons = null;
		int m_currentIcon = 0;
		ExecutionPlan m_turnOffIconPlan = null;
		bool m_isInUrf = false;
		bool m_shouldDelayURFIconReset = false;

		// Skin setting
		CurrentChampionSelection m_currentChampion = null;
		SkinCarouselChampion[] m_skins = null;
		SkinSelectionState m_skinSelectionState = SkinSelectionState.NoneSelected;

		GameFlow.Phase m_gameflowPhase = GameFlow.Phase.None;
		ChampSelect.Phase m_champSelectPhase = ChampSelect.Phase.NONE;

		public ChampSelectChore()
		{
			LCU.onMessage += async (s, e) =>
			{
				if (e.eventName == "Update" || e.eventName == "Create")
				{
					if (e.uri == "/lol-gameflow/v1/session")
					{
						var session = e.msg.ToObject<GameFlow.Session>();
						OnGameFlowUpdateMessage(session);
					}

					if (e.uri == "/lol-summoner/v1/current-summoner")
						OnSummonerUpdateMessage(s, e);

					if (e.uri == "/lol-champ-select/v1/session")
					{
						var session = e.msg.ToObject<ChampSelect.Session>();
						OnChampSelectTimerUpdate(session.timer);
					}

					if (e.uri.StartsWith("/lol-inventory/"))
						m_icons = await InventoryItem.Get("SUMMONER_ICON");

					UpdateSelectedSkin(s, e);
				}
				else if (e.eventName == "Delete")
				{
					if (e.uri == "/lol-gameflow/v1/session")
						OnGameFlowUpdateMessage(new GameFlow.Session());
					if (e.uri == "/lol-champ-select/v1/session")
						OnChampSelectTimerUpdate(new ChampSelect.Timer());
				}
			};

			LCU.onValid += OnConnected;
			if (LCU.isValid)
				OnConnected(this, EventArgs.Empty);
		}

		async void OnConnected(object sender, EventArgs e)
		{
			var gameFlowRequest = await LCU.GetAs<GameFlow.Session>("/lol-gameflow/v1/session");
			if (gameFlowRequest.ok)
				OnGameFlowUpdateMessage(gameFlowRequest.body);

			var timerRequest = await LCU.GetAs<ChampSelect.Timer>("/lol-gameflow/v1/session/timer");
			if (timerRequest.ok)
				OnChampSelectTimerUpdate(timerRequest.body);

			var summonerRequest = await Summoner.GetCurrent();
			if (summonerRequest.ok)
				OnSummonerUpdate(summonerRequest.body, true);

			m_icons = await InventoryItem.Get("SUMMONER_ICON");
		}

		void OnSummonerUpdate(Summoner currentSummoner, bool isInitialSetup)
		{
			m_currentIcon = currentSummoner.profileIconId;
			Log.Information($"Current profile icon: {currentSummoner.profileIconId}");

			var settings = Settings.File.data.summonerSettings[LCU.currentSummonerId];
			if (settings.champSelect == null)
			{
				settings.champSelect = new Settings.ChampSelectData();
				Settings.File.Save();
			}

			if (settings.champSelect.defaultIcon != currentSummoner.profileIconId)
			{
				// Save default icon settings if possible
				if (isWearingUrfIcon == false || settings.champSelect.defaultIcon == 0)
				{
					Log.Information($"Set default profile icon to {currentSummoner.profileIconId} (was: {settings.champSelect.defaultIcon})");
					settings.champSelect.defaultIcon = currentSummoner.profileIconId;
					Settings.File.Save();
				}

				// If we just started the app, noticed we are wearing an URF icon when we shouldn't be, set it back
				if (isInitialSetup && m_isInUrf == false && isWearingUrfIcon && settings.champSelect.defaultIcon != 0)
				{
					Log.Information($"Noticed an URF icon was set, resetting to default ({settings.champSelect.defaultIcon})");
					SelectIcon(settings.champSelect.defaultIcon);
				}
			}
		}

		void OnSummonerUpdateMessage(object sender, LCUMessageEventArgs e)
		{
			Summoner current = e.msg.ToObject<Summoner>();
			if (current != null)
				OnSummonerUpdate(current, false);
		}

		void OnGameFlowUpdateMessage(GameFlow.Session session)
		{
			bool isPlayingUrf = session.map.gameMode == "URF";
			if (m_gameflowPhase == session.phase && isPlayingUrf == m_isInUrf)
				return;

			m_isInUrf = isPlayingUrf;
			m_gameflowPhase = session.phase;
			Log.Information($"GameFlow Phase has changed: {m_gameflowPhase} ({(m_isInUrf ? "Playing URF" : "Not playing URF")})");
			OnPhasesChanged();
		}

		void OnChampSelectTimerUpdate(ChampSelect.Timer timer)
		{
			if (m_champSelectPhase == timer.phase)
				return;

			m_champSelectPhase = timer.phase;
			Log.Information($"ChampSelect Phase has changed: {m_champSelectPhase}");
			OnPhasesChanged();
		}

		async void OnPhasesChanged()
		{
			bool shouldBeWearingUrfIcon = true;
			var validatedSettings = LCU.validatedSummonerSettings.champSelect;

			switch (m_gameflowPhase)
			{
				case GameFlow.Phase.None:
					shouldBeWearingUrfIcon = false;
					m_skinSelectionState = SkinSelectionState.NoneSelected;
					break;

				case GameFlow.Phase.Lobby:
					goto case GameFlow.Phase.ReadyCheck;
				case GameFlow.Phase.ReadyCheck:
					m_skinSelectionState = SkinSelectionState.NoneSelected;
					m_shouldDelayURFIconReset = false; // Initial state
					shouldBeWearingUrfIcon = true;
					if (isWearingUrfIcon == false)
					{
						// Set Urf icon
						if (validatedSettings.enableUrfEffects == false)
							break;

						if (m_icons == null || m_icons.Length == 0)
							m_icons = await InventoryItem.Get("SUMMONER_ICON");

						var availableIcons = urfIcons.Where(u => m_icons.Any(i => i.itemId == u)).ToArray();
						if (availableIcons == null || availableIcons.Length == 0)
						{
							Log.Warning($"User has setup URF effects, is in the stage to setup URF effects, but has no available URF icons (0/{availableIcons.Length} icons)");
							break;
						}

						var settingsRef = Settings.File.data.summonerSettings[LCU.currentSummonerId];
						if (settingsRef.champSelect == null)
							settingsRef.champSelect = new Settings.ChampSelectData();

						var id = availableIcons[settingsRef.champSelect.urfIconIndex % availableIcons.Length];
						settingsRef.champSelect.urfIconIndex = (settingsRef.champSelect.urfIconIndex + 1) % availableIcons.Length;
						Settings.File.Save();
						await SelectIcon(id);
					}
					break;
			}

			switch (m_champSelectPhase)
			{
				case ChampSelect.Phase.GAME_STARTING:
					m_shouldDelayURFIconReset = true;
					shouldBeWearingUrfIcon = false;
					m_skinSelectionState = SkinSelectionState.NoneSelected;
					break;
			}

			if (shouldBeWearingUrfIcon == false && isWearingUrfIcon && m_turnOffIconPlan == null)
			{
				Log.Information($"Should be turning off urf profile icon, {(m_shouldDelayURFIconReset ? $"doing so after {urfResetDelaySec} seconds" : "reverting immediately")} (Current: {m_currentIcon}, Default: {validatedSettings.defaultIcon}, ChampSelect phase: {m_champSelectPhase}, GameFlow phase: {m_gameflowPhase}, {(m_turnOffIconPlan != null ? "has plan" : "has no plan")})");
				if (m_shouldDelayURFIconReset)
				{
					m_turnOffIconPlan = ExecutionPlan.Delay(urfResetDelaySec * 1000, async () =>
					{
						await SelectIcon(validatedSettings.defaultIcon);
						m_shouldDelayURFIconReset = false;
					});
				}
				else
				{
					await SelectIcon(validatedSettings.defaultIcon);
				}
			}
		}

		async void UpdateSelectedSkin(object sender, LCUMessageEventArgs e)
		{
			var settings = LCU.validatedSummonerSettings.champSelect;
			if (settings.selectLatestSkin.Value == false)
				return;

			// Get required data..
			if (e.msg != null)
			{
				bool isActive = e.eventName == "Update" || e.eventName == "Create";
				if (!isActive)
					return;

				// Current skin selected
				if (e.uri == currentSkinUpdateUri)
				{
					var currentChamp = e.msg.ToObject<CurrentChampionSelection>();
					if (currentChamp == null)
					{
						Log.Warning($"Could not decypher current champ selection. JSON: '{e.msg}'");
						return;
					}

					m_currentChampion = currentChamp;
				}

				// Get the available skins
				if (e.uri == skinUpdateUri)
				{
					var skins = e.msg.ToObject<SkinCarouselChampion[]>();
					if (skins == null)
					{
						Log.Warning($"Could not decypher skin carousel. JSON: '{e.msg}'");
						return;
					}

					m_skins = skins;
				}
			}

			if (m_skins == null || m_currentChampion == null) // Dont have all info yet
				return;

			if (m_currentChampion.skinSelectionDisabled || m_skinSelectionState != SkinSelectionState.NoneSelected) // Can't select skin
				return;

			m_skinSelectionState = SkinSelectionState.IsSelecting;
			var skinIndex = m_currentChampion.selectedSkinId % 100;
			if (skinIndex != 0)
			{
				m_skinSelectionState = SkinSelectionState.NoneSelected;
				return;
			}

			var bestSkin = m_skins.Where(s => s.ownership.owned).OrderByDescending(s => s.id).FirstOrDefault();
			if (bestSkin == null)
			{
				m_skinSelectionState = SkinSelectionState.NoneSelected;
				var anySkin = m_skins.FirstOrDefault();
				if (anySkin != null)
					Log.Information($"Unable to filter skins by owned for champion {anySkin.id}");
				return;
			}

			// Check if we have any chroma available (and selected in settings)
			Log.Information($"Current Skin Index = {skinIndex}");
			var anyChroma = bestSkin.childSkins.Where(s => s.ownership.owned).OrderBy(qu => Guid.NewGuid()).FirstOrDefault();
			if (settings.selectAnyChroma.Value && anyChroma != null)
			{
				Log.Information($"Chose chroma {anyChroma.id} (skin: '{bestSkin.name}', chroma: '{anyChroma.name}')");
				await SelectSkin(anyChroma.id);
			}
			else
			{
				Log.Information($"Chose skin {bestSkin.id} ({bestSkin.name}, no chroma)");
				await SelectSkin(bestSkin.id);
			}

			m_skins = null;
			m_currentChampion = null;
			m_skinSelectionState = SkinSelectionState.HasSelected;
		}

		async Task SelectSkin(int id)
		{
			await LCU.Patch("/lol-champ-select/v1/session/my-selection", "{\"selectedSkinId\":" + id.ToString() + "}");
			Log.Information($"Selected skin with id {id}");
		}

		async Task SelectIcon(int id)
		{
			if (id <= 1)
			{
				Log.Warning($"Tried to set the profile icon to an invalid id: {id}");
				return;
			}

			await LCU.Put("/lol-summoner/v1/current-summoner/icon", "{\"profileIconId\": " + id.ToString() + "}");
			Log.Information($"Set profile icon to {id}.");
		}

		bool isWearingUrfIcon => urfIcons.Any(i => i == m_currentIcon);
	}
}
