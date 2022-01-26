using LeagueChores.Util;
using System;
using System.Collections.Generic;

namespace LeagueChores.Settings
{
	internal class RootData : SettingsExtensions
	{
		public Dictionary<UInt64, SummonerData> summonerSettings = new Dictionary<UInt64, SummonerData>();
		public bool startWithWindows = false;
		public bool openSettingsMenuOnStart = false;
		public bool showConnectedNotification = true;
		public string lcuLocation = "";
		public int version;
	}
}
