using System.Diagnostics;

namespace LeagueChores
{
	static internal class CombineHelper
	{
		public static T Combine<T>(T? baseSetting, T? offSetting, T defaultSetting = default(T)) where T : struct
		{
			if (offSetting.HasValue)
				return offSetting.Value;

			if (baseSetting.HasValue)
				return baseSetting.Value;

			return defaultSetting;
		}
		public static T Combine<T>(Optional<T> baseSetting, Optional<T> offSetting, T defaultSetting = default(T)) where T : struct
		{
			if (offSetting.hasValue)
				return offSetting.value;

			if (baseSetting.hasValue)
				return baseSetting.value;

			return defaultSetting;
		}

		public static T Combine<T>(T baseSetting, T offSetting, T defaultSetting = default(T))
		{
			if (offSetting != null)
				return offSetting;

			if (baseSetting != null)
				return baseSetting;

			Debug.Assert(defaultSetting != null);
			return defaultSetting;
		}
	}
}
