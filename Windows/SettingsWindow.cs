using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LeagueChores.Windows
{
	public partial class SettingsWindow : Form
	{
		static List<ISettingsWindowHandler> m_handlers = new List<ISettingsWindowHandler>();
		static ISettingsWindowHandler m_generalHandler = null;
		static ISettingsWindowHandler m_summonerHandler = null;

		List<HandlerInstance> m_instances = new List<HandlerInstance>();
		Dictionary<TreeNode, HandlerInstance> m_treeToHandler = new Dictionary<TreeNode, HandlerInstance>();
		HandlerInstance m_currentInstance = null;

		TreeNode m_generalTreeNode;
		List<TreeNode> m_summonerTreeNodes = new List<TreeNode>();
		Settings.RootData m_beforeChanges = null;

		internal static void RegisterHandler<T>() where T : ISettingsWindowHandler, new()
		{
			m_handlers.Add(new T());
		}

		internal static void RegisterGeneralHandler<T>() where T : ISettingsWindowHandler, new()
		{
			m_generalHandler = new T();
		}

		internal static void RegisterSummonerHandler<T>() where T : ISettingsWindowHandler, new()
		{
			m_summonerHandler = new T();
		}

		public SettingsWindow()
		{
			InitializeComponent();

			LCU.onStateChanged += (s, e) => OnStatusChanged();
			LCU.onValid += (s, e) => OnStatusChanged();
			if (LCU.isValid)
				OnStatusChanged();

			GenerateSettingsList();

			FormClosing += OnClose;
			settingsList.AfterSelect += (s, e) => UpdateSelection();
			settingsList.BeforeCollapse += (s, e) => e.Cancel = true;
			m_beforeChanges = Settings.File.data.Copy();
			saveButton.Enabled = false;
		}

		private void OnClose(object sender, FormClosingEventArgs e)
		{
			try // Don't avoid close due to exception
			{
				if (saveButton.Enabled == false) // if this is enabled, we've got unsaved changes
					return;

				var closeReason = (CloseReason)e.CloseReason;
				if (closeReason != CloseReason.UserClosing) // Discard changes, your loss I guess
				{
					ResetSettings();
					return;
				}

				var result = MessageBox.Show("You haven't saved your changes, are you sure you want to close this window?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (result == DialogResult.Yes)
				{
					ResetSettings();
					return;
				}

				// We said no, don't close
				e.Cancel = true; 
			}
			catch
			{

			}
		}

		void ResetSettings()
		{
			Settings.File.data = m_beforeChanges;
			saveButton.Enabled = false;
		}

		void GenerateSettingsList()
		{
			settingsList.Nodes.Clear();

			// Create general settings handler
			m_generalTreeNode = settingsList.Nodes.Add("General Settings");
			if (m_generalHandler != null)
			{
				HandlerInstance instance = new HandlerInstance(m_generalHandler);
				m_instances.Add(instance);
				m_treeToHandler[m_generalTreeNode] = instance;
			}

			// Add general settings handlers
			foreach (var handler in m_handlers)
			{
				if (handler.AreGeneralSettings() == false)
					continue;

				TreeNode node = m_generalTreeNode.Nodes.Add(handler.GetSettingsName());

				HandlerInstance instance = new HandlerInstance(handler);
				m_instances.Add(instance);
				m_treeToHandler[node] = instance;
			}

			if (Settings.File.data.summonerSettings.ContainsKey(1) == false) // Init default settings
			{
				Settings.File.data.summonerSettings[1] = new Settings.SummonerData();
				Settings.File.Save();
			}

			var baseSettings = Settings.File.data.summonerSettings[1];

			// Create chore setting handlers
			foreach (var summonerSettings in Settings.File.data.summonerSettings)
			{
				if (summonerSettings.Key != 1 && summonerSettings.Key != LCU.currentSummonerId)
					continue;

				TreeNode summonerTreeNode = settingsList.Nodes.Add(summonerSettings.Value.userName);
				m_summonerTreeNodes.Add(summonerTreeNode);
				if (m_summonerHandler != null)
				{
					HandlerInstance instance = new HandlerInstance(m_summonerHandler);
					m_instances.Add(instance);
					m_treeToHandler[summonerTreeNode] = instance;
				}

				foreach (var handler in m_handlers)
				{
					if (handler.AreGeneralSettings())
						continue;

					TreeNode node = summonerTreeNode.Nodes.Add(handler.GetSettingsName());

					HandlerInstance instance = new HandlerInstance(handler, summonerSettings.Value, baseSettings);
					m_instances.Add(instance);
					m_treeToHandler[node] = instance;
				}
			}

			settingsList.ExpandAll();
			settingsList.SelectedNode = settingsList.Nodes[0];
		}

		public void OnSettingsChanged()
		{
			saveButton.Enabled = true; // If changed, enable save button
		}

		private void OnStatusChanged()
		{
			if (InvokeRequired) // Ensure this is running on the UI thread
			{
				Invoke(new MethodInvoker(OnStatusChanged));
				return;
			}

			switch (LCU.state)
			{
				case LCUState.IsDown:
					this.statusLabel.Text = "Not connected to League of Legends.";
					break;
				case LCUState.NoConnectionAvailable:
					this.statusLabel.Text = "Waiting for LCU service to start..";
					break;
				case LCUState.IsUp:
					this.statusLabel.Text = "Connected to League of Legends.";
					break;
			}

			GenerateSettingsList();
		}

		private void UpdateSelection()
		{
			this.SuspendLayout();
			m_currentInstance?.handler.OnWindowClose();
			settingsGroup.Controls.Clear();

			if (m_treeToHandler.ContainsKey(settingsList.SelectedNode))
			{
				m_currentInstance = m_treeToHandler[settingsList.SelectedNode];
				this.settingsGroup.Text = m_currentInstance.handler.GetSettingsName();
				m_currentInstance.handler.OnWindowOpen(this, settingsGroup.Controls, m_currentInstance.summoner, m_currentInstance.baseSummoner);
			}
			else
			{
				m_currentInstance = null;
				this.settingsGroup.Text = settingsList.SelectedNode.Text;

				var redirectLabel = new Label();
				redirectLabel.AutoSize = true;
				redirectLabel.Location = new System.Drawing.Point(7, 20);
				redirectLabel.Name = "label1";
				redirectLabel.Size = new System.Drawing.Size(217, 13);
				redirectLabel.TabIndex = 0;
				redirectLabel.Text = "Please select a menu from the list on the left.";

				settingsGroup.Controls.Add(redirectLabel);
			}

			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private void OnSave(object sender, EventArgs e)
		{
			Settings.File.Save();
			foreach (var handler in m_treeToHandler)
				handler.Value.handler.OnSave();
			saveButton.Enabled = false;
			m_beforeChanges = Settings.File.data.Copy();
		}

		public bool hasUnsavedChanges => saveButton.Enabled;
	}

	internal class HandlerInstance
	{
		public ISettingsWindowHandler handler { get; private set; }
		public Settings.SummonerData summoner { get; private set; }
		public Settings.SummonerData baseSummoner { get; private set; }

		public HandlerInstance(ISettingsWindowHandler handler, Settings.SummonerData summoner = null, Settings.SummonerData baseSummoner = null)
		{
			this.handler = handler;
			this.summoner = summoner;
			this.baseSummoner = baseSummoner;
		}
	}
}
