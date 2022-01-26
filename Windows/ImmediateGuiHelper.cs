using System;
using System.Windows.Forms;

namespace LeagueChores.Windows
{
	internal class ImmediateGuiHelper : IDisposable
	{
		private static readonly int baseLineX = 10;

		private int lastLineX = 0;
		private int lastLineY = 0; 
		private int x = baseLineX;
		private int y = 20;
		private int tabIndex = 9999;
		private bool isOwner;
		private bool canDraw = true;
		private Control control;
		private ImmediateGuiHelper parent = null;
		private Control.ControlCollection controls => control.Controls;

		public ImmediateGuiHelper(Control control, bool isOwner = false)
		{
			this.control = control;
			this.isOwner = isOwner;
		}

		public CheckBox AddCheckbox(string name, string text)
		{
			if (canDraw == false)
				throw new Exception("Can't draw! We're in the middle of another ImmediateGuiHelper action!");

			CheckBox box = new CheckBox();
			box.Name = name;
			box.Text = text;
			box.AutoSize = true;
			box.Location = new System.Drawing.Point(x, y);
			box.FlatStyle = FlatStyle.Flat;
			box.TabIndex = tabIndex--;
			box.UseVisualStyleBackColor = true;
			controls.Add(box);

			CreateNewline(box.Size.Width, box.Size.Height);
			return box;
		}

		public Label AddLabel(string name, string text)
		{
			if (canDraw == false)
				throw new Exception("Can't draw! We're in the middle of another ImmediateGuiHelper action!");

			var label = new Label();
			label.Name = name;
			label.Text = text;
			label.Location = new System.Drawing.Point(x, y);
			label.AutoSize = true;
			label.TabIndex = tabIndex--;
			controls.Add(label);

			CreateNewline(label.Size.Width, label.Size.Height);
			return label;
		}

		public TextBox AddTextbox(string name, string text, int width, int height = 20)
		{
			if (canDraw == false)
				throw new Exception("Can't draw! We're in the middle of another ImmediateGuiHelper action!");

			AddLabel($"{name}Label", text); SameLine();

			TextBox box = new TextBox();
			box.Name = name;
			box.Location = new System.Drawing.Point(x - 5, y - 3);
			box.Size = new System.Drawing.Size(width, height);
			box.BorderStyle = BorderStyle.FixedSingle;
			box.TabIndex = tabIndex--;
			controls.Add(box);

			CreateNewline(box.Size.Width + 3, box.Size.Height + 3);
			return box;
		}

		public TextBox AddNumberTextbox(string name, string text, int width, int height = 20)
		{
			if (canDraw == false)
				throw new Exception("Can't draw! We're in the middle of another ImmediateGuiHelper action!");

			var textBox = AddTextbox(name, text, width, height);

			textBox.KeyPress += ValidateTextboxNumberKeypress;
			textBox.TextChanged += ValidateTextboxNumberText;

			return textBox;
		}

		public TextBox AddHotkeyTextbox(string name, string text, int width, int height = 20)
		{
			if (canDraw == false)
				throw new Exception("Can't draw! We're in the middle of another ImmediateGuiHelper action!");

			var textBox = AddTextbox(name, text, width, height);
			textBox.KeyDown += ValidateHotkeyTextbox;
			textBox.KeyPress += (s,e) => e.Handled = true;
			return textBox;
		}

		private void ValidateHotkeyTextbox(object sender, KeyEventArgs e)
		{
			var textBox = (TextBox)sender;
			textBox.Parent.Focus();
			textBox.Text = e.KeyCode.ToString();
			e.Handled = true;
		}

		public Button AddButton(string name, string text)
		{
			if (canDraw == false)
				throw new Exception("Can't draw! We're in the middle of another ImmediateGuiHelper action!");

			var button = new System.Windows.Forms.Button();
			button.Name = name;
			button.Text = text;
			button.AutoSize = true;
			button.Location = new System.Drawing.Point(x, y);
			button.UseVisualStyleBackColor = true;
			button.TabIndex = tabIndex--;
			controls.Add(button);

			CreateNewline(button.Size.Width, button.Size.Height + 10);
			return button;
		}

		public ImmediateGuiHelper AddGroupbox(Control parent, int initialHeight = 20)
		{
			if (canDraw == false)
				throw new Exception("Can't draw! We're in the middle of another ImmediateGuiHelper action!");

			var group = new GroupBox();

			group.Name = "champDisenchantGroup";
			group.Text = "Champion Settings";
			group.Location = new System.Drawing.Point(x, y);
			group.Size = new System.Drawing.Size(parent.Size.Width - 20, initialHeight);
			group.TabIndex = tabIndex--;
			group.TabStop = false;
			group.SuspendLayout();
			controls.Add(group);

			var newHelper = new ImmediateGuiHelper(group, true);
			newHelper.parent = this;
			canDraw = false;
			return newHelper;
		}

		public void SetOffset(int offsetX, int offsetY)
		{
			x += offsetX;
			y += offsetY;
		}

		public void SameLine()
		{
			if (canDraw == false)
				throw new Exception("Can't draw! We're in the middle of another ImmediateGuiHelper action!");

			x = lastLineX + 10;
			y = lastLineY;
		}

		public void NewLine(int height = 10)
		{
			if (canDraw == false)
				throw new Exception("Can't draw! We're in the middle of another ImmediateGuiHelper action!");

			CreateNewline(0, height);
		}

		private void CreateNewline(int width, int height)
		{
			if (canDraw == false)
				throw new Exception("Can't draw! We're in the middle of another ImmediateGuiHelper action!");

			// Append to last
			lastLineX = x + width;
			lastLineY = y;

			// Return X, new Y
			x = baseLineX;
			y += height;
		}

		public void Dispose()
		{
			if (isOwner)
			{
				control.Size = new System.Drawing.Size(control.Size.Width, y);

				control.ResumeLayout(false);
				control.PerformLayout();

				if (parent != null)
				{
					parent.canDraw = true;
					parent.CreateNewline(control.Size.Width, control.Size.Height + 10);
				}
			}
		}

		private void ValidateTextboxNumberKeypress(object sender, KeyPressEventArgs e)
		{
			e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
		}

		private void ValidateTextboxNumberText(object sender, EventArgs e)
		{
			var textbox = (TextBox)sender;
			textbox.Text = textbox.Text.Replace("[^0-9]", "");
		}
	}
}
