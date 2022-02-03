using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueChores.GameFlow
{
	public enum Phase
	{
		None,
		Lobby,
		Matchmaking,
		CheckedIntoTournament,
		ReadyCheck,
		ChampSelect,
		GameStart,
		FailedToLaunch,
		InProgress,
		Reconnect,
		WaitingForStats,
		PreEndOfGame,
		EndOfGame,
		TerminatedInError
	}

	public class GameClient
	{
		public string observerServerIp { get; set; }
		public int observerServerPort { get; set; }
		public bool running { get; set; }
		public string serverIp { get; set; }
		public int serverPort { get; set; }
		public bool visible { get; set; }
	}

	public class GameTypeConfig
	{
		public bool advancedLearningQuests { get; set; }
		public bool allowTrades { get; set; }
		public string banMode { get; set; }
		public int banTimerDuration { get; set; }
		public bool battleBoost { get; set; }
		public bool crossTeamChampionPool { get; set; }
		public bool deathMatch { get; set; }
		public bool doNotRemove { get; set; }
		public bool duplicatePick { get; set; }
		public bool exclusivePick { get; set; }
		public UInt64 id { get; set; }
		public bool learningQuests { get; set; }
		public int mainPickTimerDuration { get; set; }
		public int maxAllowableBans { get; set; }
		public string name { get; set; }
		public bool onboardCoopBeginner { get; set; }
		public string pickMode { get; set; }
		public int postPickTimerDuration { get; set; }
		public bool reroll { get; set; }
		public bool teamChampionPool { get; set; }
	}

	public class QueueRewards
	{
		public bool isChampionPointsEnabled { get; set; }
		public bool isIpEnabled { get; set; }
		public bool isXpEnabled { get; set; }
		public List<object> partySizeIpRewards { get; set; }
	}

	public class Queue
	{
		public List<object> allowablePremadeSizes { get; set; }
		public bool areFreeChampionsAllowed { get; set; }
		public string assetMutator { get; set; }
		public string category { get; set; }
		public int championsRequiredToPlay { get; set; }
		public string description { get; set; }
		public string detailedDescription { get; set; }
		public string gameMode { get; set; }
		public GameTypeConfig gameTypeConfig { get; set; }
		public int id { get; set; }
		public bool isRanked { get; set; }
		public bool isTeamBuilderManaged { get; set; }
		public bool isTeamOnly { get; set; }
		public UInt64 lastToggledOffTime { get; set; }
		public UInt64 lastToggledOnTime { get; set; }
		public int mapId { get; set; }
		public int maxLevel { get; set; }
		public int maxSummonerLevelForFirstWinOfTheDay { get; set; }
		public int maximumParticipantListSize { get; set; }
		public int minLevel { get; set; }
		public int minimumParticipantListSize { get; set; }
		public string name { get; set; }
		public int numPlayersPerTeam { get; set; }
		public string queueAvailability { get; set; }
		public QueueRewards queueRewards { get; set; }
		public bool removalFromGameAllowed { get; set; }
		public int removalFromGameDelayMinutes { get; set; }
		public string shortName { get; set; }
		public bool showPositionSelector { get; set; }
		public bool spectatorEnabled { get; set; }
		public string type { get; set; }
	}

	public class GameData
	{
		public UInt64 gameId { get; set; }
		public string gameName { get; set; }
		public bool isCustomGame { get; set; }
		public string password { get; set; }
		public List<object> playerChampionSelections { get; set; }
		public Queue queue { get; set; }
		public bool spectatorsAllowed { get; set; }
		public List<object> teamOne { get; set; }
		public List<object> teamTwo { get; set; }
	}

	public class GameDodge
	{
		public List<object> dodgeIds { get; set; }
		public string phase { get; set; }
		public string state { get; set; }
	}

	public class Assets
	{
		[JsonProperty("champ-select-background-sound")]
		public string ChampSelectBackgroundSound { get; set; }

		[JsonProperty("champ-select-flyout-background")]
		public string ChampSelectFlyoutBackground { get; set; }

		[JsonProperty("champ-select-planning-intro")]
		public string ChampSelectPlanningIntro { get; set; }

		[JsonProperty("game-select-icon-active")]
		public string GameSelectIconActive { get; set; }

		[JsonProperty("game-select-icon-active-video")]
		public string GameSelectIconActiveVideo { get; set; }

		[JsonProperty("game-select-icon-default")]
		public string GameSelectIconDefault { get; set; }

		[JsonProperty("game-select-icon-disabled")]
		public string GameSelectIconDisabled { get; set; }

		[JsonProperty("game-select-icon-hover")]
		public string GameSelectIconHover { get; set; }

		[JsonProperty("game-select-icon-intro-video")]
		public string GameSelectIconIntroVideo { get; set; }

		[JsonProperty("gameflow-background")]
		public string GameflowBackground { get; set; }

		[JsonProperty("gameselect-button-hover-sound")]
		public string GameselectButtonHoverSound { get; set; }

		[JsonProperty("icon-defeat")]
		public string IconDefeat { get; set; }

		[JsonProperty("icon-defeat-video")]
		public string IconDefeatVideo { get; set; }

		[JsonProperty("icon-empty")]
		public string IconEmpty { get; set; }

		[JsonProperty("icon-hover")]
		public string IconHover { get; set; }

		[JsonProperty("icon-leaver")]
		public string IconLeaver { get; set; }

		[JsonProperty("icon-victory")]
		public string IconVictory { get; set; }

		[JsonProperty("icon-victory-video")]
		public string IconVictoryVideo { get; set; }

		[JsonProperty("map-north")]
		public string MapNorth { get; set; }

		[JsonProperty("map-south")]
		public string MapSouth { get; set; }

		[JsonProperty("music-inqueue-loop-sound")]
		public string MusicInqueueLoopSound { get; set; }

		[JsonProperty("parties-background")]
		public string PartiesBackground { get; set; }

		[JsonProperty("postgame-ambience-loop-sound")]
		public string PostgameAmbienceLoopSound { get; set; }

		[JsonProperty("ready-check-background")]
		public string ReadyCheckBackground { get; set; }

		[JsonProperty("ready-check-background-sound")]
		public string ReadyCheckBackgroundSound { get; set; }

		[JsonProperty("sfx-ambience-pregame-loop-sound")]
		public string SfxAmbiencePregameLoopSound { get; set; }

		[JsonProperty("social-icon-leaver")]
		public string SocialIconLeaver { get; set; }

		[JsonProperty("social-icon-victory")]
		public string SocialIconVictory { get; set; }
	}

	public class CategorizedContentBundles
	{
	}

	public class PerPositionDisallowedSummonerSpells
	{
	}

	public class PerPositionRequiredSummonerSpells
	{
	}

	public class GameFlowMapProperties
	{
		public bool suppressRunesMasteriesPerks { get; set; }
	}

	public class Map
	{
		public Assets assets { get; set; }
		public CategorizedContentBundles categorizedContentBundles { get; set; }
		public string description { get; set; }
		public string gameMode { get; set; }
		public string gameModeName { get; set; }
		public string gameModeShortName { get; set; }
		public string gameMutator { get; set; }
		public int id { get; set; }
		public bool isRGM { get; set; }
		public string mapStringId { get; set; }
		public string name { get; set; }
		public PerPositionDisallowedSummonerSpells perPositionDisallowedSummonerSpells { get; set; }
		public PerPositionRequiredSummonerSpells perPositionRequiredSummonerSpells { get; set; }
		public string platformId { get; set; }
		public string platformName { get; set; }
		public GameFlowMapProperties properties { get; set; }
	}

	public class Session
	{
		public GameClient gameClient { get; set; }
		public GameData gameData { get; set; }
		public GameDodge gameDodge { get; set; }
		public Map map { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public Phase phase { get; set; }
	}
}
