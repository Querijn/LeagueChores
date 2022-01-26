using LeagueChores.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LeagueChores.Settings
{
	internal static class LootDataDefaults
	{
		public static readonly int noChampKeepCount = 1;	// How many Champion Tokens do you want to keep when you don't have the champion?
		public static readonly int lowLevelKeepCount = 2;	// How many Champion Tokens do you want to keep when you're between level 0~4 on the champion?
		public static readonly int level5KeepCount = 2;		// How many Champion Tokens do you want to keep when you're between level 5 on the champion?
		public static readonly int level6KeepCount = 1;		// How many Champion Tokens do you want to keep when you're between level 6 on the champion?

		public static readonly bool combineKeys = true;
		public static readonly bool openCapsules = false;
		// public static readonly bool openChests = false;
		public static readonly bool disenchantEternals = false;
		public static readonly bool disenchantDuplicateSkins = false;
		public static readonly bool rerollAllOwnedSkins = false;
		public static readonly bool disenchantChampions = false;

		public static readonly bool showDisenchantNotification = true;
		public static readonly bool onlyRunManually = true;
	}

	internal class LootData : SettingsExtensions
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public bool? onlyRunManually;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public bool? showDisenchantNotification;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public bool? combineKeys;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public bool? openCapsules;
		// [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public bool? openChests;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public bool? disenchantEternals;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public bool? disenchantDuplicateSkins;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public bool? rerollAllOwnedSkins;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public bool? disenchantChampions;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public int? noChampKeepCount;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public int? lowLevelKeepCount;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public int? level5KeepCount;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public int? level6KeepCount;

		public static LootData Combine(LootData? baseSettings, LootData? offSettings)
		{
			LootData result = new LootData();

			result.noChampKeepCount = CombineHelper.Combine(baseSettings?.noChampKeepCount, offSettings?.noChampKeepCount, LootDataDefaults.noChampKeepCount);
			result.lowLevelKeepCount = CombineHelper.Combine(baseSettings?.lowLevelKeepCount, offSettings?.lowLevelKeepCount, LootDataDefaults.lowLevelKeepCount);
			result.level5KeepCount = CombineHelper.Combine(baseSettings?.level5KeepCount, offSettings?.level5KeepCount, LootDataDefaults.level5KeepCount);
			result.level6KeepCount = CombineHelper.Combine(baseSettings?.level6KeepCount, offSettings?.level6KeepCount, LootDataDefaults.level6KeepCount);

			result.combineKeys = CombineHelper.Combine(baseSettings?.combineKeys, offSettings?.combineKeys, LootDataDefaults.combineKeys);
			result.openCapsules = CombineHelper.Combine(baseSettings?.openCapsules, offSettings?.openCapsules, LootDataDefaults.openCapsules);
			// result.openChests = CombineHelper.Combine(baseSettings?.openChests, offSettings?.openChests, LootDataDefaults.openChests);
			result.disenchantEternals = CombineHelper.Combine(baseSettings?.disenchantEternals, offSettings?.disenchantEternals, LootDataDefaults.disenchantEternals);
			result.disenchantDuplicateSkins = CombineHelper.Combine(baseSettings?.disenchantDuplicateSkins, offSettings?.disenchantDuplicateSkins, LootDataDefaults.disenchantDuplicateSkins);
			result.rerollAllOwnedSkins = CombineHelper.Combine(baseSettings?.rerollAllOwnedSkins, offSettings?.rerollAllOwnedSkins, LootDataDefaults.rerollAllOwnedSkins);
			result.disenchantChampions = CombineHelper.Combine(baseSettings?.disenchantChampions, offSettings?.disenchantChampions, LootDataDefaults.disenchantChampions);

			result.showDisenchantNotification = CombineHelper.Combine(baseSettings?.showDisenchantNotification, offSettings?.showDisenchantNotification, LootDataDefaults.showDisenchantNotification);
			result.onlyRunManually = CombineHelper.Combine(LootDataDefaults.onlyRunManually, offSettings?.onlyRunManually, LootDataDefaults.onlyRunManually); // No default setting

			return result;
		}
	}
}
