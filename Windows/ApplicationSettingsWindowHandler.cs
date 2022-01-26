using LeagueChores.Windows;
using System.Windows.Forms;

namespace LeagueChores
{
	internal class ApplicationSettingsWindowHandler : ISettingsWindowHandler
	{
		public bool AreGeneralSettings()
		{
			return true;
		}

		public string GetSettingsName()
		{
			return "Application Settings";
		}

		public void OnSave()
		{
			// Ensure the boot key knows about the setting discrepency
			Program.SyncBootKey();
		}

		public void OnWindowClose() { }

		void ApplyCheckboxBehaviour(CheckBox checkbox, string settingName, SettingsWindow window)
		{
			checkbox.Checked = (bool)Settings.File.data[settingName];
			checkbox.CheckedChanged += (s, e) =>
			{
				Settings.File.data[settingName] = checkbox.Checked;
				window.OnSettingsChanged();
			};
		}

		public void OnWindowOpen(SettingsWindow window, Control.ControlCollection collection, Settings.SummonerData summonerData, Settings.SummonerData baseSummonerData)
		{
			var ui = new ImmediateGuiHelper(collection.Owner);

			var startWithWindows = ui.AddCheckbox("startWithWindows", "Start with Windows");
			ApplyCheckboxBehaviour(startWithWindows, "startWithWindows", window);

			var openSettingsMenuOnStart = ui.AddCheckbox("openSettingsMenuOnStart", "Open settings menu on start");
			ApplyCheckboxBehaviour(openSettingsMenuOnStart, "openSettingsMenuOnStart", window);

			var showConnectedNotification = ui.AddCheckbox("showConnectedNotification", "Show connection notifications");
			ApplyCheckboxBehaviour(showConnectedNotification, "showConnectedNotification", window);
		}
	}
}
