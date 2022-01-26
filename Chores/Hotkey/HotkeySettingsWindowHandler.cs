using LeagueChores.Settings;
using LeagueChores.Windows;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LeagueChores.Chores
{
	internal class HotkeySettingsWindowHandler : ISettingsWindowHandler
	{
		public bool AreGeneralSettings()
		{
			return false;
		}

		public string GetSettingsName()
		{
			return "Hotkey Settings";
		}

		public void OnSave() { Program.disenchantChore.CleanInventory(false); }
		public void OnWindowClose() { }

		private void ApplyCheckboxBehaviour(CheckBox box, string name, SettingsWindow window, Settings.SummonerData summonerData, bool defaultValue)
		{
			bool? value = summonerData.hotkey != null ? (bool?)summonerData.hotkey[name] : defaultValue;
			box.Checked = value != null ? (bool)value : defaultValue;
			box.CheckedChanged += (s, e) =>
			{
				if (summonerData.hotkey == null)
					summonerData.hotkey = new Settings.HotkeyData();

				summonerData.hotkey[name] = box.Checked;
				window.OnSettingsChanged();
			};
		}

		public void InitHotkeyHelper(TextBox textbox, int i, List<Keys> hotkeySettings, SettingsWindow window, ImmediateGuiHelper imgui, Control.ControlCollection collection, Settings.SummonerData summonerData, Settings.SummonerData baseSummonerData)
		{
			textbox.Text = hotkeySettings[i].ToString();
			textbox.TextChanged += (s, e) =>
			{
				Keys key;
				if (Enum.TryParse(textbox.Text, out key))
				{
					hotkeySettings[i] = key;
					window.OnSettingsChanged();
				}
			};

			imgui.SameLine();
			imgui.SetOffset(-5, -5);
			var button = imgui.AddButton("Remove", "Remove");
			button.Click += (s, e) =>
			{
				hotkeySettings.RemoveAt(i);
				window.OnSettingsChanged();
				OnWindowOpen(window, collection, summonerData, baseSummonerData); // Refresh
			};
		}

		public void OnWindowOpen(SettingsWindow window, Control.ControlCollection collection, Settings.SummonerData summonerData, Settings.SummonerData baseSummonerData)
		{
			collection.Clear();
			collection.Owner.SuspendLayout();

			ImmediateGuiHelper imgui = new ImmediateGuiHelper(collection.Owner);
			bool isBase = ReferenceEquals(baseSummonerData, summonerData);

			if (summonerData.hotkey == null) // Create defaults
				summonerData.hotkey = HotkeyData.Combine(baseSummonerData.hotkey, summonerData.hotkey);

			var canAcceptQueueWithHotkey = imgui.AddCheckbox("canAcceptQueueWithHotkey", "Accept queue with hotkey");
			ApplyCheckboxBehaviour(canAcceptQueueWithHotkey, "canAcceptQueueWithHotkey", window, summonerData, HotkeyDataDefaults.canAcceptQueueWithHotkey);
			imgui.NewLine(5);

			for (int i = 0; i < summonerData.hotkey.acceptQueueHotkeys.Count; i++)
			{
				var textbox = imgui.AddHotkeyTextbox($"acceptQueueHotkey{i}", "Hotkey: ", 200);
				InitHotkeyHelper(textbox, i, summonerData.hotkey.acceptQueueHotkeys, window, imgui, collection, summonerData, baseSummonerData);
			}

			var addHotkeyButton = imgui.AddButton("addAcceptHotkeyButton", "Add new");
			addHotkeyButton.Click += (s, e) =>
			{
				summonerData.hotkey.acceptQueueHotkeys.Add(Keys.None);
				OnWindowOpen(window, collection, summonerData, baseSummonerData); // Refresh
			};

			imgui.NewLine();

			var canDeclineQueueWithHotkey = imgui.AddCheckbox("canDeclineQueueWithHotkey", "Decline queue with hotkey");
			ApplyCheckboxBehaviour(canDeclineQueueWithHotkey, "canDeclineQueueWithHotkey", window, summonerData, HotkeyDataDefaults.canDeclineQueueWithHotkey);
			imgui.NewLine(5);

			for (int i = 0; i < summonerData.hotkey.declineQueueHotkeys.Count; i++)
			{
				var textbox = imgui.AddHotkeyTextbox($"declineQueueHotkey{i}", "Hotkey: ", 200);
				InitHotkeyHelper(textbox, i, summonerData.hotkey.declineQueueHotkeys, window, imgui, collection, summonerData, baseSummonerData);
			}

			addHotkeyButton = imgui.AddButton("addAcceptHotkeyButton", "Add new");
			addHotkeyButton.Click += (s, e) =>
			{
				summonerData.hotkey.declineQueueHotkeys.Add(Keys.None);
				OnWindowOpen(window, collection, summonerData, baseSummonerData); // Refresh
			};

			collection.Owner.PerformLayout();
			collection.Owner.ResumeLayout();
		}
	}
}
