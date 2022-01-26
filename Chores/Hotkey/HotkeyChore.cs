using LeagueChores.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serilog;

namespace LeagueChores
{
	internal class HotkeyChore
	{
		[DllImport("user32.dll")]
		static extern short GetAsyncKeyState(Keys vKey);
		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();
		[DllImport("user32.dll")]
		static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
		[DllImport("user32.dll")]
		static extern int GetClassName(IntPtr hWnd, StringBuilder text, int count);

		ExecutionPlan m_updater;

		public HotkeyChore()
		{
			LCU.onMessage += OnMessage;
		}

		public static bool IsHotkeyPressed(Keys vKey)
		{
			return (GetAsyncKeyState(vKey) & 0x8000) != 0;
		}

		bool IsLeagueClientInFront()
		{
			string foregroundWindowName = GetActiveWindowTitle();
			Log.Information($"Current foreground window name = '{foregroundWindowName}'");
			return foregroundWindowName != null && foregroundWindowName.Contains("RCLIENT");
		}

		private async void UpdateReadyCheck()
		{
			if (m_updater == null)
				return;

			var settings = LCU.validatedSummonerSettings.hotkey;
			if (settings.canAcceptQueueWithHotkey.Value)
			{
				foreach (var hotkey in settings.acceptQueueHotkeys)
				{
					if (IsHotkeyPressed(hotkey) == false)
						continue;

					if (IsLeagueClientInFront())
					{
						await LCU.Post("/lol-matchmaking/v1/ready-check/accept", "{}");
						Log.Information($"Accepting queue");
						m_updater.Abort();
					}
					return;
				}
			}

			if (settings.canDeclineQueueWithHotkey.Value)
			{
				foreach (var hotkey in settings.declineQueueHotkeys)
				{
					if (IsHotkeyPressed(hotkey) == false)
						break;

					if (IsLeagueClientInFront())
					{
						await LCU.Post("/lol-matchmaking/v1/ready-check/decline", "{}");
						Log.Information($"Declining queue");
						m_updater.Abort();
					}
					return;
				}
			}
		}

		private string GetActiveWindowTitle()
		{
			const int nChars = 256;
			StringBuilder Buff = new StringBuilder(nChars);
			IntPtr handle = GetForegroundWindow();

			if (GetClassName(handle, Buff, nChars) > 0)
				return Buff.ToString();
			return null;
		}

		private void OnMessage(object sender, LCUMessageEventArgs e)
		{
			if (e.uri == "/lol-matchmaking/v1/ready-check")
			{
				bool isActive = e.eventName == "Update" || e.eventName == "Create";
				if (isActive)
				{
					bool hasMessage = e.msg != null;
					bool canAcceptQueue = hasMessage ? e.msg["state"].ToString() == "InProgress" : true;
					bool hasUpdater = m_updater != null;

					if (canAcceptQueue && hasUpdater == false) // Can accept queue, but has no updater
					{
						m_updater = ExecutionPlan.Repeat(10, () => UpdateReadyCheck());
					}
					else if (canAcceptQueue == false && m_updater != null) // Can not accept queue, but has updater running
					{
						m_updater.Dispose();
						m_updater = null;
					}
					Log.Information($"Can accept queue: {canAcceptQueue}");
				}
				else
				{
					Log.Information("No longer in ready check.");
					if (m_updater != null)
					{
						m_updater.Dispose();
						m_updater = null;
					}
				}
			}
		}
	}
}
