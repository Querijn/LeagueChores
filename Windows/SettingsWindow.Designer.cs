using System;

namespace LeagueChores.Windows
{
	partial class SettingsWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsWindow));
			this.settingsList = new System.Windows.Forms.TreeView();
			this.settingsGroup = new System.Windows.Forms.GroupBox();
			this.parentPanel = new System.Windows.Forms.Panel();
			this.saveButton = new System.Windows.Forms.Button();
			this.statusLabel = new System.Windows.Forms.Label();
			this.parentPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// settingsList
			// 
			this.settingsList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.settingsList.Dock = System.Windows.Forms.DockStyle.Left;
			this.settingsList.HideSelection = false;
			this.settingsList.Location = new System.Drawing.Point(10, 10);
			this.settingsList.Name = "settingsList";
			this.settingsList.ShowPlusMinus = false;
			this.settingsList.Size = new System.Drawing.Size(240, 418);
			this.settingsList.TabIndex = 4;
			// 
			// settingsGroup
			// 
			this.settingsGroup.Dock = System.Windows.Forms.DockStyle.Right;
			this.settingsGroup.Location = new System.Drawing.Point(256, 10);
			this.settingsGroup.Name = "settingsGroup";
			this.settingsGroup.Padding = new System.Windows.Forms.Padding(10);
			this.settingsGroup.Size = new System.Drawing.Size(620, 418);
			this.settingsGroup.TabIndex = 5;
			this.settingsGroup.TabStop = false;
			// 
			// parentPanel
			// 
			this.parentPanel.AutoSize = true;
			this.parentPanel.Controls.Add(this.settingsGroup);
			this.parentPanel.Controls.Add(this.settingsList);
			this.parentPanel.Location = new System.Drawing.Point(0, 0);
			this.parentPanel.Name = "parentPanel";
			this.parentPanel.Padding = new System.Windows.Forms.Padding(10);
			this.parentPanel.Size = new System.Drawing.Size(886, 438);
			this.parentPanel.TabIndex = 4;
			// 
			// saveButton
			// 
			this.saveButton.Location = new System.Drawing.Point(784, 440);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(92, 23);
			this.saveButton.TabIndex = 6;
			this.saveButton.Text = "Save Settings";
			this.saveButton.UseVisualStyleBackColor = true;
			this.saveButton.Click += new System.EventHandler(this.OnSave);
			// 
			// statusLabel
			// 
			this.statusLabel.AutoSize = true;
			this.statusLabel.Location = new System.Drawing.Point(13, 445);
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(185, 13);
			this.statusLabel.TabIndex = 7;
			this.statusLabel.Text = "Not connected to League of Legends";
			// 
			// SettingsWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(886, 470);
			this.Controls.Add(this.statusLabel);
			this.Controls.Add(this.saveButton);
			this.Controls.Add(this.parentPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "SettingsWindow";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "LeagueChores Settings";
			this.parentPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TreeView settingsList;
		private System.Windows.Forms.GroupBox settingsGroup;
		private System.Windows.Forms.Panel parentPanel;
		private System.Windows.Forms.Button saveButton;
		private System.Windows.Forms.Label statusLabel;
	}
}