using Newtonsoft.Json;
using System;
using System.IO;
using Serilog;

namespace LeagueChores.Settings
{
	enum FirstRunState
	{
		Unchecked,
		IsFirstRun,
		IsNotFirstRun
	}

	internal class File
	{
		private static readonly string localFileLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Program.applicationName, "Settings.json");
		private static readonly int version = 1;
		
		private static RootData m_rootData = null;
		private static ExecutionPlan m_savePlan = null;
		private static FirstRunState m_isFirstRun = FirstRunState.Unchecked;
		public static bool isFirstRun
		{
			get
			{
				if (m_isFirstRun == FirstRunState.Unchecked)
					throw new InvalidOperationException("Cannot determine first run without loading");
				return m_isFirstRun == FirstRunState.IsFirstRun;
			}
		}
		public static RootData data
		{
			get
			{
				if (m_rootData == null)
				{
					// Load file if exists
					if (System.IO.File.Exists(localFileLocation))
					{
						m_rootData = JsonConvert.DeserializeObject<RootData>(System.IO.File.ReadAllText(localFileLocation));
						m_isFirstRun = FirstRunState.IsNotFirstRun;

						// TODO: Verify version here
					}

					// Otherwise create new
					if (m_rootData == null)
					{
						m_isFirstRun = FirstRunState.IsFirstRun;
						m_rootData = new RootData();
						m_rootData.version = version;
						Save();
					}
				}

				return m_rootData;
			}
			set
			{
				m_rootData = value; // TODO: is this really a good idea?
			}
		}

		public static void Save()
		{
			if (m_savePlan == null)
				m_savePlan = ExecutionPlan.Delay(1000, () => InternalSave());
		}

		private static void InternalSave()
		{
			System.IO.Directory.CreateDirectory(System.IO.Directory.GetParent(localFileLocation).FullName);
			System.IO.File.WriteAllText(localFileLocation, JsonConvert.SerializeObject(data));

			m_savePlan = null;
			Log.Information("Settings have been saved.");
		}
	}
}
