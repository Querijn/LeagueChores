using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeagueChores
{
	public enum LCUState
	{
		IsDown,
		NoConnectionAvailable,
		IsUp
	};

	public class StateChangedEventArgs
	{
		public LCUState state { get; private set; }

		public StateChangedEventArgs(LCUState state)
		{
			this.state = state;
		}
	}

	static internal class LCU
	{
		static readonly string lcuProcessName = "LeagueClientUx";

		public delegate void StateChangedEventHandler(object sender, StateChangedEventArgs e);
		public static event StateChangedEventHandler			onStateChanged;
		public static event StateChangedEventHandler			onConnect;
		public static event StateChangedEventHandler			onDisconnect;
		public static event LCUSocket.LCUMessageEventHandler	onMessage;
		public static event EventHandler						onValid;

		public static LCUState state { get; private set;}
		public static ushort port { get; private set; }
		public static string password { get; private set; }
		public static UInt64 currentSummonerId { get; private set; }
		public static bool isValid { get { return hasSummonerId && hasPasswordAndPort; } }
		public static bool hasSummonerId { get { return currentSummonerId != 0; } }
		public static bool hasPasswordAndPort { get { return port != 0 && string.IsNullOrWhiteSpace(password) == false; } }
		public static IEnumerable<Process> runningProcesses => Process.GetProcesses().Where(p => p.ProcessName == lcuProcessName && p.HasExited == false);
		public static Settings.SummonerData validatedSummonerSettings => Settings.SummonerData.Combine(baseSummonerSettings, summonerSettingsCache);

		static Settings.SummonerData baseSummonerSettings;
		static Settings.SummonerData summonerSettingsCache;
		static Requester m_requester = new Requester();
		static LCUSocket m_socket = new LCUSocket();
		static ExecutionPlan m_executionPlan;
		static bool m_isFirstTick = true;
		static bool m_isInitialisingBaseData = false;
		static int failedLogins = 0;

		static LCU()
		{
			onConnect += (s, e) => InitBaseData();

			m_executionPlan = ExecutionPlan.Repeat(2000, () => LCU.Update());
			m_socket.onMessage += SocketMessageHandler;
			CacheSummonerSettings(1, out baseSummonerSettings);
		}

		static async void InitBaseData()
		{
			if (isValid || m_isInitialisingBaseData)
				return;

			m_isInitialisingBaseData = true;

			var loginData = await Get("/lol-login/v1/session");
			if (loginData.ok)
			{
				var summonerIdSrc = loginData.body["summonerId"];
				if (summonerIdSrc != null && summonerIdSrc.Type == Newtonsoft.Json.Linq.JTokenType.Integer)
				{
					currentSummonerId = summonerIdSrc.ToObject<UInt64>();
					var currentSummonerResponse = await Get("/lol-summoner/v1/current-summoner");
					string userName = currentSummonerResponse.ok ? 
						currentSummonerResponse.body["displayName"].ToString() : 
						loginData.body["username"].ToString();

					CacheSummonerSettings(currentSummonerId, out summonerSettingsCache);
					if (string.IsNullOrWhiteSpace(summonerSettingsCache.userName) ||
						summonerSettingsCache.userName != userName)
					{
						summonerSettingsCache.userName = userName;
						Settings.File.Save();
					}

					if (isValid && onValid != null)
					{
						failedLogins = 0;
						onValid(null, EventArgs.Empty);
						Console.WriteLine($"Connected to the LCU.");
					}
				}
			}
			else if (loginData.statusCode == HttpStatusCode.Forbidden || loginData.statusCode == HttpStatusCode.Unauthorized) // Password incorrect?
			{
				Console.WriteLine($"Unable to connect to the LCU api! Attempts: {failedLogins + 1}");
				UnsetPortAndPassword();
				failedLogins++;

				GetPortAndPassword();
			}
			//else if (loginData.statusCode != HttpStatusCode.NotFound) // Not logged in
			//{
			//	throw new Exception("Unexpected login issue!");
			//}

			m_isInitialisingBaseData = false;
		}

		// Update the LCU state based on the running process count
		static async void UpdateLCUState()
		{
			bool hasServer = LCU.runningProcesses.FirstOrDefault() != null;
			if (state != LCUState.IsDown && hasServer == false) // We just went offline
			{
				ChangeState(LCUState.IsDown);
				return;
			}

			switch (state)
			{
				case LCUState.IsDown:
				{
					if (hasServer == false)
						break;

					if (!m_isFirstTick)
					{
						ChangeState(LCUState.NoConnectionAvailable); // Just came alive, check for connection in a couple of seconds
						goto case LCUState.NoConnectionAvailable;
					}
					else
					{
						ChangeState(LCUState.IsUp); // First attempt, League is probably already running
						break;
					}
				}

				case LCUState.NoConnectionAvailable:
				{
					DefaultResponse response = await Get("/lol-login/v1/session");
					if (response.ok)
						ChangeState(LCUState.IsUp);
					break;
				}
			}

			m_isFirstTick = false;
		}

		static void Update()
		{
			UpdateLCUState();

			if (hasSummonerId == false)
				InitBaseData();
		}

		// Small function to update the LCU state and message the listeners
		static void ChangeState(LCUState newState)
		{
			if (newState == state && m_isFirstTick == false)
				return;

			state = newState;
			var args = new StateChangedEventArgs(newState);

			if (state == LCUState.IsDown)
			{
				UnsetPortAndPassword();
				if (onDisconnect != null)
					onDisconnect(null, args);
			}
			else
			{
				GetPortAndPassword();
			}

			if (state == LCUState.IsUp)
			{
				if (onConnect != null)
					onConnect(null, args);
			}

			if (onStateChanged != null)
				onStateChanged(null, args);
		}

		// remove current port, password and everything depending on these.
		static void UnsetPortAndPassword()
		{
			port = 0;
			password = "";
			currentSummonerId = 0;
			summonerSettingsCache = null;
			m_requester.UnsetUserData();
			m_socket.UnsetCredentials();
		}

		// Update the current port and password to validate the LCU state
		static bool GetPortAndPassword()
		{
			if (hasPasswordAndPort)
				return true;

			// No league client running
			var processes = runningProcesses.ToArray();
			if (processes.FirstOrDefault() == null)
				return false;

			// Try to find it in the running processes
			if (failedLogins == 0)
			{
				foreach (var process in processes)
				{
					try
					{
						string cmdLine = process.GetCommandLine();

						Match portMatch = (new Regex(@"--app-port=(?<port>\d+)")).Match(cmdLine);
						if (portMatch.Success == false)
							continue;
						port = ushort.Parse(portMatch.Groups["port"].Value);

						Match passwordMatch = (new Regex(@"--remoting-auth-token=(?<password>[a-zA-Z0-9_]+)")).Match(cmdLine);
						if (passwordMatch.Success == false)
							continue;
						password = passwordMatch.Groups["password"].Value;
						break;
					}
					catch (Win32Exception ex) when ((uint)ex.ErrorCode == 0x80004005) // E_FAIL: Unspecified failure
					{
						// Intentionally empty.
					}
				}
			}

			// Try it with the lockfile otherwise
			if (hasPasswordAndPort == false && failedLogins < 2)
			{
				string lockFileFolder = Settings.File.data.lcuLocation;
				if (IsValidClientFolder(lockFileFolder) == false)
				{
					lockFileFolder = "C:/Riot Games/League of Legends";
					if (IsValidClientFolder(lockFileFolder) == false)
					{
						foreach (var process in processes)
						{
							try
							{
								string cmdLine = process.GetCommandLine();
								Match match = (new Regex(@"--install-directory=""(?<installDir>.*?)""")).Match(cmdLine);
								if (match.Success == false)
									continue;
								lockFileFolder = match.Groups["lockFileLocation"].Value;
							}
							catch (Win32Exception ex) when ((uint)ex.ErrorCode == 0x80004005)
							{
								// Intentionally empty.
							}
						}
					}
				}

				string lockFileLocation = Path.Combine(lockFileFolder, "lockfile");
				if (IsValidClientFolder(lockFileFolder) && File.Exists(lockFileLocation))
				{
					if (Settings.File.data.lcuLocation != lockFileFolder)
					{
						Settings.File.data.lcuLocation = lockFileFolder;
						Settings.File.Save();
					}

					string tempFileLocation = lockFileLocation + "-temp";
					try
					{
						File.Copy(lockFileLocation, tempFileLocation);
						LockFile f = new LockFile(File.ReadAllText(tempFileLocation, Encoding.UTF8));
						File.Delete(tempFileLocation);

						password = f.password;
						port = f.port;
					}
					catch (System.UnauthorizedAccessException)
					{
						// Too bad, can't use this folder
					}
				}
			}

			// Make sure our data is correct
			if (hasPasswordAndPort)
				m_requester.SetUserData($"https://127.0.0.1:{port}", "riot", password);
			else
				throw new Exception("Cannot connect to the LCU!");

			if (isValid && onValid != null)
				onValid(null, EventArgs.Empty);
			return hasPasswordAndPort;
		}

		// Checks if the client folder we found is valid.
		static bool IsValidClientFolder(string folder)
		{
			try
			{
				if (string.IsNullOrEmpty(folder))
					return false;

				string processLocation = Path.Combine(folder, $"{lcuProcessName}.exe");
				bool processExists = File.Exists(processLocation);
				bool configFolderExists = Directory.Exists(folder + "/Config");
				return processExists && configFolderExists;
			}
			catch
			{
				return false;
			}
		}

		// Simple message deferer from our socket
		static void SocketMessageHandler(object sender, LCUMessageEventArgs e)
		{
			if (onMessage != null)
				onMessage(sender, e);

			if (e.uri.StartsWith("/lol-login/") && e.eventName != "Delete")
			{
				if (e.msg.Contains("summonerId") && e.msg["summonerId"].Type == Newtonsoft.Json.Linq.JTokenType.Integer)
					currentSummonerId = e.msg["summonerId"].ToObject<UInt64>();
			}
		}

		static void CacheSummonerSettings(UInt64 id, out Settings.SummonerData data)
		{
			if (Settings.File.data.summonerSettings.TryGetValue(id, out data))
				return;

			data = Settings.File.data.summonerSettings[id] = new Settings.SummonerData();
			if (id == 1) data.userName = "Default Summoner Settings";
			Settings.File.Save();
		}

		public static async Task<DefaultResponse> Get(string uri)
		{
			if (hasPasswordAndPort == false)
				return new DefaultResponse(FailureCode.CannotConnect);
			return await m_requester.Get(uri);
		}

		public static async Task<Response<T>> GetAs<T>(string uri)
		{
			if (hasPasswordAndPort == false)
				return new Response<T>(FailureCode.CannotConnect);
			return await m_requester.GetAs<T>(uri);
		}

		public static async Task<DefaultResponse> Post(string uri, string body)
		{
			if (hasPasswordAndPort == false)
				return new DefaultResponse(FailureCode.CannotConnect);
			return await m_requester.Post(uri, body);
		}

		public static async Task<Response<T>> PostAs<T>(string uri, string body)
		{
			if (hasPasswordAndPort == false)
				return new Response<T>(FailureCode.CannotConnect);
			return await m_requester.PostAs<T>(uri, body);
		}

		public static async Task<DefaultResponse> Put(string uri, string body)
		{
			if (hasPasswordAndPort == false)
				return new DefaultResponse(FailureCode.CannotConnect);
			return await m_requester.Put(uri, body);
		}

		public static async Task<Response<T>> PutAs<T>(string uri, string body)
		{
			if (hasPasswordAndPort == false)
				return new Response<T>(FailureCode.CannotConnect);
			return await m_requester.PutAs<T>(uri, body);
		}
	}
}
