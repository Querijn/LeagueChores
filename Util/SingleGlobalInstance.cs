using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace LeagueChores
{
	public class ApplicationAlreadyRunningException : Exception
	{
		public ApplicationAlreadyRunningException()
		{
		}
	}

	class SingleGlobalInstance : IDisposable
	{
		private bool m_hasHandle = false;
		private Mutex m_mutex;

		public SingleGlobalInstance(int timeout)
		{
			string process = Process.GetCurrentProcess().MainModule.FileName;
			FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(process);
			string mutexId = $"Global\\{fileInfo.CompanyName}.{fileInfo.ProductName}";
			m_mutex = new Mutex(false, mutexId);

			var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
			var secSettings = new MutexSecurity();
			secSettings.AddAccessRule(allowEveryoneRule);
			m_mutex.SetAccessControl(secSettings);

			try
			{
				m_hasHandle = m_mutex.WaitOne(timeout <= 0 ? Timeout.Infinite : timeout, false);
				if (!m_hasHandle)
					throw new ApplicationAlreadyRunningException();
			}
			catch (AbandonedMutexException)
			{
				m_hasHandle = true;
			}
		}

		public void Dispose()
		{
			if (m_mutex != null)
			{
				if (m_hasHandle)
					m_mutex.ReleaseMutex();
				m_mutex.Dispose();
				m_mutex = null;
			}

			m_hasHandle = false;
		}
	}
}
