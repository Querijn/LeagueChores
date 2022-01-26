using LeagueChores.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LeagueChores.Settings
{
	internal static class HotkeyDataDefaults
	{
		public static readonly bool canAcceptQueueWithHotkey = true;
		public static readonly List<Keys> acceptQueueHotkeys = new List<Keys>(new Keys[1] { Keys.Return });

		public static readonly bool canDeclineQueueWithHotkey = false;
		public static readonly List<Keys> declineQueueHotkeys = new List<Keys>(new Keys[1] { Keys.Escape });
	}

	internal class HotkeyData : SettingsExtensions
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public bool? canAcceptQueueWithHotkey;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public List<Keys> acceptQueueHotkeys;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public bool? canDeclineQueueWithHotkey;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public List<Keys> declineQueueHotkeys;

		public static HotkeyData Combine(HotkeyData? baseSettings, HotkeyData? offSettings)
		{
			HotkeyData result = new HotkeyData();

			result.canAcceptQueueWithHotkey = CombineHelper.Combine(baseSettings?.canAcceptQueueWithHotkey, offSettings?.canAcceptQueueWithHotkey, HotkeyDataDefaults.canAcceptQueueWithHotkey);
			result.acceptQueueHotkeys = CombineHelper.Combine(baseSettings?.acceptQueueHotkeys, offSettings?.acceptQueueHotkeys, HotkeyDataDefaults.acceptQueueHotkeys);

			result.canDeclineQueueWithHotkey = CombineHelper.Combine(baseSettings?.canDeclineQueueWithHotkey, offSettings?.canDeclineQueueWithHotkey, HotkeyDataDefaults.canDeclineQueueWithHotkey);
			result.declineQueueHotkeys = CombineHelper.Combine(baseSettings?.declineQueueHotkeys, offSettings?.declineQueueHotkeys, HotkeyDataDefaults.declineQueueHotkeys);

			return result;
		}
	}
}
