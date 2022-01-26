using System;

namespace LeagueChores
{
	internal class ExecutionPlan : IDisposable
	{
		private System.Timers.Timer m_planTimer;
		private Action m_planAction;
		bool m_isRepeatedPlan;
		bool m_isBusy = false;
		System.Timers.ElapsedEventHandler m_callback;

		private ExecutionPlan(int millisecondsDelay, Action planAction, bool isRepeatedPlan)
		{
			m_callback = (s, e) => GenericTimerCallback();

			m_planTimer = new System.Timers.Timer(millisecondsDelay);
			m_planTimer.Elapsed += m_callback;
			m_planTimer.Enabled = true;

			this.m_planAction = planAction;
			this.m_isRepeatedPlan = isRepeatedPlan;
			if (isRepeatedPlan) // Call immediately if we're on a loop.
				GenericTimerCallback();
		}

		public static ExecutionPlan Delay(int millisecondsDelay, Action planAction)
		{
			return new ExecutionPlan(millisecondsDelay, planAction, false);
		}

		public static ExecutionPlan Repeat(int millisecondsInterval, Action planAction)
		{
			return new ExecutionPlan(millisecondsInterval, planAction, true);
		}

		private void GenericTimerCallback()
		{
			if (m_isBusy == false)
			{
				m_isBusy = true;

				m_planAction();
				if (!m_isRepeatedPlan)
					Abort();

				m_isBusy = false;
			}
		}

		public void Abort()
		{
			m_planTimer.Enabled = false;
			m_planTimer.Elapsed -= m_callback;
		}

		public void Dispose()
		{
			if (m_planTimer == null)
				throw new ObjectDisposedException(typeof(ExecutionPlan).Name);

			Abort();
			m_planTimer.Dispose();
			m_planTimer = null;
		}
	}
}
