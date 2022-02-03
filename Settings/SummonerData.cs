using LeagueChores.Util;

namespace LeagueChores.Settings
{
	internal class SummonerData : SettingsExtensions
	{
		public string? userName;
		public LootData? loot;
		public HotkeyData? hotkey;
		public ChampSelectData? champSelect;

		public static SummonerData Combine(SummonerData baseSettings, SummonerData offSettings)
		{
			SummonerData result = new SummonerData();

			result.userName = CombineHelper.Combine(baseSettings?.userName, offSettings?.userName, "Default Settings");
			result.loot = LootData.Combine(baseSettings?.loot, offSettings?.loot);
			result.hotkey = HotkeyData.Combine(baseSettings?.hotkey, offSettings?.hotkey);
			result.champSelect = ChampSelectData.Combine(baseSettings?.champSelect, offSettings?.champSelect);

			return result;
		}
	}
}
