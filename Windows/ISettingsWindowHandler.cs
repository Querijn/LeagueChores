using System.Windows.Forms;

namespace LeagueChores.Windows
{
	internal interface ISettingsWindowHandler
	{
		public bool AreGeneralSettings();
		public string GetSettingsName();
		public void OnWindowOpen(SettingsWindow window, Control.ControlCollection collection, Settings.SummonerData summonerData, Settings.SummonerData baseSummonerData);
		public void OnWindowClose();
		public void OnSave();
	}
}
