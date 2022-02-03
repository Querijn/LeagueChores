using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace LeagueChores.ChampSelect
{
	public enum Phase
	{
		NONE,
		PLANNING,
		BAN_PICK,
		FINALIZATION,
		GAME_STARTING
	}

	public class PhaseEnumConverter : StringEnumConverter
	{
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (string.IsNullOrEmpty(reader.Value.ToString()))
				return Phase.NONE;
			return base.ReadJson(reader, objectType, existingValue, serializer);
		}
	}

	public class Timer
	{
		public int adjustedTimeLeftInPhase { get; set; }
		public UInt64 internalNowInEpochMs { get; set; }
		public bool isInfinite { get; set; }
		[JsonConverter(typeof(PhaseEnumConverter))]
		public Phase phase { get; set; }
		public int totalTimeInPhase { get; set; }
	}

	public class Session
	{
		public Timer timer { get; set; }
	}
}
