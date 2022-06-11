using LeagueChores.Settings;
using LeagueChores.Windows;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LeagueChores.Chores.ChampSelect
{
	internal class ChampSelectSettingsWindowHandler : ISettingsWindowHandler
	{
		public bool AreGeneralSettings()
		{
			return false;
		}

		public string GetSettingsName()
		{
			return "Champ Select Settings";
		}

		public void OnSave() { }
		public void OnWindowClose() { }

		private void ApplyCheckboxBehaviour(CheckBox box, string name, SettingsWindow window, Settings.SummonerData summonerData, bool defaultValue)
		{
			bool? value = summonerData.champSelect != null ? (bool?)summonerData.champSelect[name] : defaultValue;
			box.Checked = value != null ? (bool)value : defaultValue;
			box.CheckedChanged += (s, e) =>
			{
				if (summonerData.hotkey == null)
					summonerData.hotkey = new Settings.HotkeyData();

				summonerData.champSelect[name] = box.Checked;
				window.OnSettingsChanged();
			};
		}

		public void OnWindowOpen(SettingsWindow window, Control.ControlCollection collection, Settings.SummonerData summonerData, Settings.SummonerData baseSummonerData)
		{
			collection.Clear();
			collection.Owner.SuspendLayout();

			ImmediateGuiHelper imgui = new ImmediateGuiHelper(collection.Owner);
			bool isBase = ReferenceEquals(baseSummonerData, summonerData);

			if (summonerData.champSelect == null) // Create defaults
				summonerData.champSelect = ChampSelectData.Combine(baseSummonerData.champSelect, summonerData.champSelect);

			var selectLatestSkin = imgui.AddCheckbox("selectLatestSkin", "Select latest skin in champ select, if no skin was selected.");
			ApplyCheckboxBehaviour(selectLatestSkin, "selectLatestSkin", window, summonerData, ChampSelectDataDefaults.selectLatestSkin);

			var selectAnyChroma = imgui.AddCheckbox("selectAnyChroma", "If a skin is automatically selected, select any chroma.");
			ApplyCheckboxBehaviour(selectAnyChroma, "selectAnyChroma", window, summonerData, ChampSelectDataDefaults.selectAnyChroma);

			imgui.NewLine();

			var enableUrfEffects = imgui.AddCheckbox("enableUrfEffects", "Enable random kill effect in URF, if available.");
			ApplyCheckboxBehaviour(enableUrfEffects, "enableUrfEffects", window, summonerData, ChampSelectDataDefaults.enableUrfEffects);

			collection.Owner.PerformLayout();
			collection.Owner.ResumeLayout();
		}
	}
}
