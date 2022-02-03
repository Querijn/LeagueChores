using LeagueChores.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LeagueChores.Settings
{
	internal static class ChampSelectDataDefaults
	{
		public static readonly bool selectLatestSkin = true;
		public static readonly bool selectAnyChroma = false;
		public static readonly bool enableUrfEffects = true;
	}

	internal class ChampSelectData : SettingsExtensions
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public bool? selectLatestSkin;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public bool? selectAnyChroma;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public bool? enableUrfEffects;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public int defaultIcon = 0;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public int urfIconIndex = 0;

		public static ChampSelectData Combine(ChampSelectData? baseSettings, ChampSelectData? offSettings)
		{
			ChampSelectData result = new ChampSelectData();

			result.selectLatestSkin = CombineHelper.Combine(baseSettings?.selectLatestSkin, offSettings?.selectLatestSkin, ChampSelectDataDefaults.selectLatestSkin);
			result.selectAnyChroma = CombineHelper.Combine(baseSettings?.selectAnyChroma, offSettings?.selectAnyChroma, ChampSelectDataDefaults.selectAnyChroma);
			result.enableUrfEffects = CombineHelper.Combine(baseSettings?.enableUrfEffects, offSettings?.enableUrfEffects, ChampSelectDataDefaults.enableUrfEffects);

			// Not a setting
			result.defaultIcon = offSettings != null ? offSettings.defaultIcon: 0;
			result.urfIconIndex = offSettings != null ? offSettings.urfIconIndex : 0;

			return result;
		}
	}
}
