using LeagueChores.Settings;
using LeagueChores.Windows;
using System;
using System.Windows.Forms;
using LeagueChores.Chores.Loot;

namespace LeagueChores.Chores.Loot
{
	internal class LootSettingsWindowHandler : ISettingsWindowHandler
	{
		public bool AreGeneralSettings()
		{
			return false;
		}

		public string GetSettingsName()
		{
			return "Loot Settings";
		}

		public void OnSave() { Program.disenchantChore.CleanInventory(false); }
		public void OnWindowClose() { }

		private void ApplyCheckboxBehaviour(CheckBox box, string name, SettingsWindow window, Settings.SummonerData baseSummonerData, Settings.SummonerData summonerData, bool defaultValue)
		{
			box.Checked = CombineHelper.Combine((bool?)baseSummonerData.loot[name], (bool?)summonerData.loot[name], defaultValue);
			box.CheckedChanged += (s, e) =>
			{
				if (summonerData.loot == null)
					summonerData.loot = new LootData();

				summonerData.loot[name] = box.Checked;
				window.OnSettingsChanged();
			};
		}

		private void ApplyTextboxBehaviour(TextBox textBox, string name, SettingsWindow window, Settings.SummonerData baseSummonerData, Settings.SummonerData summonerData, int defaultValue)
		{
			int value = CombineHelper.Combine((int?)baseSummonerData.loot[name], (int?)summonerData.loot[name], defaultValue);
			textBox.Text = value.ToString();
			textBox.TextChanged += (s, e) =>
			{
				if (string.IsNullOrWhiteSpace(textBox.Text))
				{
					textBox.Text = "";
					return;
				}

				if (summonerData.loot == null)
					summonerData.loot = new Settings.LootData();

				summonerData.loot[name] = int.Parse(textBox.Text);
				window.OnSettingsChanged();
			};
		}

		public void OnWindowOpen(SettingsWindow window, Control.ControlCollection collection, Settings.SummonerData summonerData, Settings.SummonerData baseSummonerData)
		{
			ImmediateGuiHelper imgui = new ImmediateGuiHelper(collection.Owner);
			bool isBase = ReferenceEquals(baseSummonerData, summonerData);

			var noChampKeepCountQuestion = "How many Champion Tokens do you want to keep when you don\'t have the champion?";
			var lowLevelKeepCountQuestion = "How many Champion Tokens do you want to keep when you\'re level 0~4 on the champion?";
			var level5KeepCountQuestion = "How many Champion Tokens do you want to keep when you\'re level 5 on the champion?";
			var level6KeepCountQuestion = "How many Champion Tokens do you want to keep when you\'re level 6 on the champion?";

			var disenchantAutomatically = isBase == false ? imgui.AddCheckbox("disenchantAutomatically", "Run loot clean-up automatically") : null;
			var showDisenchantNotification = imgui.AddCheckbox("showDisenchantNotification", "Show notification when changes are made to the inventory");

			imgui.NewLine();

			var combineKeys =					imgui.AddCheckbox("combineKeys",					"Combine all key fragments");
			var openCapsules =					imgui.AddCheckbox("openCapsules",					"Open all eternals and champion capsules");
			// var openChests =					imgui.AddCheckbox("openChests",						"Open all chests");
			var disenchantEternals =			imgui.AddCheckbox("disenchantEternals",				"Disenchant all eternals");
			var disenchantDuplicateSkins =		imgui.AddCheckbox("disenchantDuplicateSkins",		"Disenchant all duplicate skins");
			var rerollAllOwnedSkins =			imgui.AddCheckbox("rerollAllOwnedSkins",			"Reroll all owned skins");

			imgui.NewLine();

			var disenchantChampions =			imgui.AddCheckbox("disenchantChampions",			"Disenchant all champions");
			TextBox noChampKeepCount;
			TextBox lowLevelKeepCount;
			TextBox level5KeepCount;
			TextBox level6KeepCount;
			using (var groupHelper = imgui.AddGroupbox(collection.Owner, 200))
			{
				noChampKeepCount =	groupHelper.AddTextbox("noChampKeepCount",	noChampKeepCountQuestion,	50);
				lowLevelKeepCount =	groupHelper.AddTextbox("lowLevelKeepCount",	lowLevelKeepCountQuestion,	50);
				level5KeepCount =	groupHelper.AddTextbox("level5KeepCount",	level5KeepCountQuestion,	50);
				level6KeepCount =	groupHelper.AddTextbox("level6KeepCount",	level6KeepCountQuestion,	50);
			}

			var disenchantButton = isBase == false ? imgui.AddButton("disenchantButton", "Disenchant now") : null;

			ApplyCheckboxBehaviour(combineKeys,					"combineKeys",					window, baseSummonerData, summonerData, LootDataDefaults.combineKeys);
			ApplyCheckboxBehaviour(openCapsules,				"openCapsules",					window, baseSummonerData, summonerData, LootDataDefaults.openCapsules);
			// ApplyCheckboxBehaviour(openChests,				"openChests",					window, baseSummonerData, summonerData, LootDataDefaults.openChests);
			ApplyCheckboxBehaviour(disenchantChampions,			"disenchantChampions",			window, baseSummonerData, summonerData, LootDataDefaults.disenchantChampions);
			ApplyCheckboxBehaviour(disenchantEternals,			"disenchantEternals",			window, baseSummonerData, summonerData, LootDataDefaults.disenchantEternals);
			ApplyCheckboxBehaviour(disenchantDuplicateSkins,	"disenchantDuplicateSkins",		window, baseSummonerData, summonerData, LootDataDefaults.disenchantDuplicateSkins);
			ApplyCheckboxBehaviour(rerollAllOwnedSkins,			"rerollAllOwnedSkins",			window, baseSummonerData, summonerData, LootDataDefaults.rerollAllOwnedSkins);
			ApplyCheckboxBehaviour(showDisenchantNotification, "showDisenchantNotification",	window, baseSummonerData, summonerData, LootDataDefaults.showDisenchantNotification);

			ApplyTextboxBehaviour(noChampKeepCount,				"noChampKeepCount",				window, baseSummonerData, summonerData, LootDataDefaults.noChampKeepCount);
			ApplyTextboxBehaviour(lowLevelKeepCount,			"lowLevelKeepCount",			window, baseSummonerData, summonerData, LootDataDefaults.lowLevelKeepCount);
			ApplyTextboxBehaviour(level5KeepCount,				"level5KeepCount",				window, baseSummonerData, summonerData, LootDataDefaults.level5KeepCount);
			ApplyTextboxBehaviour(level6KeepCount,				"level6KeepCount",				window, baseSummonerData, summonerData, LootDataDefaults.level6KeepCount);

			if (disenchantAutomatically != null)
			{
				// onlyRunManually has special behaviour
				bool? opt = summonerData.loot?.onlyRunManually;
				disenchantAutomatically.Checked = !(opt.HasValue ? opt.Value : LootDataDefaults.onlyRunManually);
				disenchantAutomatically.CheckedChanged += (s, e) =>
				{
					if (disenchantAutomatically.Checked)
					{
						MessageBoxButtons buttons = MessageBoxButtons.YesNo;
						DialogResult result = MessageBox.Show("This will automatically remove unwanted items that you've selected in the settings here, whenever they appear in your inventory.\n\nThese automatic actions can not be undone. Are you absolutely sure?", "Warning", buttons, MessageBoxIcon.Exclamation);
						// TODO: List everything that will be done automatically

						if (result == DialogResult.Yes)
						{
							if (summonerData.loot == null)
								summonerData.loot = new LootData();
							summonerData.loot.onlyRunManually = false;
							window.OnSettingsChanged();
						}
						else
						{
							disenchantAutomatically.Checked = false;
						}
					}
					else
					{
						if (summonerData.loot == null)
							summonerData.loot = new LootData();
						summonerData.loot.onlyRunManually = true;
						window.OnSettingsChanged();
					};
				};
			}

			if (disenchantButton != null)
			{
				disenchantButton.Click += (object sender, EventArgs e) =>
				{
					if (window.hasUnsavedChanges)
						MessageBox.Show("Please save your settings before disenchanting.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					else
						Program.disenchantChore.CleanInventory(true);
				};
			}
		}
	}
}
