using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Launcher
{
	public class LightOptionsForm : Form
	{
		private IContainer components;
		private GroupBox LightOptionsGroupBox;
		private Button LightOptionsButtonOK;
		private Button LightOptionsButtonCancel;
		private CheckBox LightOptionsJitterCheckBox;
		private CheckBox LightOptionsMaxBouncesCheckBox;
		private CheckBox LightOptionsTracesCheckBox;
		private CheckBox LightOptionsVerboseCheckBox;
		private CheckBox LightOptionsNoModelShadowsCheckBox;
		private NumericUpDown LightOptionsJitterNumericUpDown;
		private NumericUpDown LightOptionsMaxBouncesNumericUpDown;
		private NumericUpDown LightOptionsTracesNumericUpDown;
		private GroupBox LightOptionsFastExtraGroupBox;
		private RadioButton LightOptionsExtraRadioButton;
		private RadioButton LightOptionsFastRadioButton;

		public LightOptionsForm() => InitializeComponent();

		private void LightOptionsFormUpdate()
		{
			LightOptionsTracesNumericUpDown.Enabled = LightOptionsTracesCheckBox.Checked;
			LightOptionsMaxBouncesNumericUpDown.Enabled = LightOptionsMaxBouncesCheckBox.Checked;
			LightOptionsJitterNumericUpDown.Enabled = LightOptionsJitterCheckBox.Checked;
		}

		private void LightOptionsForm_Load(object sender, EventArgs e)
		{
			LightOptionsExtraRadioButton.Checked = Launcher.mapSettings.GetBoolean("lightoptions_extra");
			LightOptionsFastRadioButton.Checked = !LightOptionsExtraRadioButton.Checked;
			LightOptionsNoModelShadowsCheckBox.Checked = Launcher.mapSettings.GetBoolean("lightoptions_nomodelshadow");
			LightOptionsVerboseCheckBox.Checked = Launcher.mapSettings.GetBoolean("lightoptions_verbose");
			LightOptionsTracesCheckBox.Checked = Launcher.mapSettings.GetBoolean("lightoptions_traces");
			LightOptionsMaxBouncesCheckBox.Checked = Launcher.mapSettings.GetBoolean("lightoptions_maxbounces");
			LightOptionsJitterCheckBox.Checked = Launcher.mapSettings.GetBoolean("lightoptions_jitter");

			Launcher.SetNumericUpDownValue(LightOptionsTracesNumericUpDown, Launcher.mapSettings.GetDecimal("lightoptions_traces_val"));
			Launcher.SetNumericUpDownValue(LightOptionsMaxBouncesNumericUpDown, Launcher.mapSettings.GetDecimal("lightoptions_maxbounces_val"));
			Launcher.SetNumericUpDownValue(LightOptionsJitterNumericUpDown, Launcher.mapSettings.GetDecimal("lightoptions_jitter_val"));

			LightOptionsFormUpdate();
		}

		private void LightOptionsTracesCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			LightOptionsFormUpdate();
		}

		private void LightOptionsMaxBouncesCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			LightOptionsFormUpdate();
		}

		private void LightOptionsJitterCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			LightOptionsFormUpdate();
		}

		private void LightOptionsButtonOK_Click(object sender, EventArgs e)
		{
			Launcher.mapSettings.SetBoolean("lightoptions_extra", LightOptionsExtraRadioButton.Checked);
			Launcher.mapSettings.SetBoolean("lightoptions_nomodelshadow", LightOptionsNoModelShadowsCheckBox.Checked);
			Launcher.mapSettings.SetBoolean("lightoptions_verbose", LightOptionsVerboseCheckBox.Checked);
			Launcher.mapSettings.SetBoolean("lightoptions_traces", LightOptionsTracesCheckBox.Checked);
			Launcher.mapSettings.SetBoolean("lightoptions_maxbounces", LightOptionsMaxBouncesCheckBox.Checked);
			Launcher.mapSettings.SetBoolean("lightoptions_jitter", LightOptionsJitterCheckBox.Checked);
			Launcher.mapSettings.SetDecimal("lightoptions_traces_val", LightOptionsTracesNumericUpDown.Value);
			Launcher.mapSettings.SetDecimal("lightoptions_maxbounces_val", LightOptionsMaxBouncesNumericUpDown.Value);
			Launcher.mapSettings.SetDecimal("lightoptions_jitter_val", LightOptionsJitterNumericUpDown.Value);

			Close();
		}

		private void LightOptionsButtonCancel_Click(object sender, EventArgs e) => Close();

		private void InitializeComponent()
		{
			components = new Container();

			LightOptionsGroupBox = new GroupBox();
			LightOptionsFastExtraGroupBox = new GroupBox();
			LightOptionsExtraRadioButton = new RadioButton();
			LightOptionsFastRadioButton = new RadioButton();
			LightOptionsJitterNumericUpDown = new NumericUpDown();
			LightOptionsMaxBouncesNumericUpDown = new NumericUpDown();
			LightOptionsTracesNumericUpDown = new NumericUpDown();
			LightOptionsJitterCheckBox = new CheckBox();
			LightOptionsMaxBouncesCheckBox = new CheckBox();
			LightOptionsTracesCheckBox = new CheckBox();
			LightOptionsVerboseCheckBox = new CheckBox();
			LightOptionsNoModelShadowsCheckBox = new CheckBox();
			LightOptionsButtonOK = new Button();
			LightOptionsButtonCancel = new Button();

			SuspendLayout();
			ClientSize = new Size(345, 170);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			ControlBox = false;
			StartPosition = FormStartPosition.CenterParent;
			ShowInTaskbar = false;
			Text = "Advanced Users Options";
			Load += new EventHandler(LightOptionsForm_Load);

			LightOptionsGroupBox.Location = new Point(8, 11);
			LightOptionsGroupBox.Size = new Size(325, 121);
			LightOptionsGroupBox.Text = "Compile Light Options";

			LightOptionsFastExtraGroupBox.Location = new Point(12, 19);
			LightOptionsFastExtraGroupBox.Size = new Size(117, 32);
			LightOptionsFastExtraGroupBox.TabStop = false;

			LightOptionsFastRadioButton.Location = new Point(6, 9);
			LightOptionsFastRadioButton.Size = new Size(45, 17);
			LightOptionsFastRadioButton.Text = "Fast";
			LightOptionsFastRadioButton.AutoSize = true;
			LightOptionsFastRadioButton.Checked = true;

			LightOptionsExtraRadioButton.Location = new Point(57, 9);
			LightOptionsExtraRadioButton.Size = new Size(49, 17);
			LightOptionsExtraRadioButton.Text = "Extra";
			LightOptionsExtraRadioButton.AutoSize = true;

			LightOptionsFastExtraGroupBox.Controls.Add(LightOptionsFastRadioButton);
			LightOptionsFastExtraGroupBox.Controls.Add(LightOptionsExtraRadioButton);

			LightOptionsNoModelShadowsCheckBox.Location = new Point(12, 57);
			LightOptionsNoModelShadowsCheckBox.Text = "No Model Shadows";
			LightOptionsNoModelShadowsCheckBox.AutoSize = true;
			LightOptionsNoModelShadowsCheckBox.FlatStyle = FlatStyle.Popup;

			LightOptionsVerboseCheckBox.Location = new Point(12, 80);
			LightOptionsVerboseCheckBox.Text = "Verbose";
			LightOptionsVerboseCheckBox.AutoSize = true;
			LightOptionsVerboseCheckBox.FlatStyle = FlatStyle.Popup;

			LightOptionsTracesCheckBox.Location = new Point(147, 34);
			LightOptionsTracesCheckBox.Text = "Traces";
			LightOptionsTracesCheckBox.AutoSize = true;
			LightOptionsTracesCheckBox.FlatStyle = FlatStyle.Popup;
			LightOptionsTracesCheckBox.CheckedChanged += new EventHandler(LightOptionsTracesCheckBox_CheckedChanged);

			LightOptionsMaxBouncesCheckBox.Location = new Point(147, 57);
			LightOptionsMaxBouncesCheckBox.Text = "Max Bounces";
			LightOptionsMaxBouncesCheckBox.AutoSize = true;
			LightOptionsMaxBouncesCheckBox.FlatStyle = FlatStyle.Popup;
			LightOptionsMaxBouncesCheckBox.CheckedChanged += new EventHandler(LightOptionsMaxBouncesCheckBox_CheckedChanged);

			LightOptionsJitterCheckBox.Location = new Point(147, 80);
			LightOptionsJitterCheckBox.Text = "Jitter";
			LightOptionsJitterCheckBox.AutoSize = true;
			LightOptionsJitterCheckBox.FlatStyle = FlatStyle.Popup;
			LightOptionsJitterCheckBox.CheckedChanged += new EventHandler(LightOptionsJitterCheckBox_CheckedChanged);

			LightOptionsTracesNumericUpDown.Location = new Point(243, 34);
			LightOptionsTracesNumericUpDown.Size = new Size(68, 20);
			LightOptionsTracesNumericUpDown.Maximum = 500;

			LightOptionsMaxBouncesNumericUpDown.Location = new Point(242, 60);
			LightOptionsMaxBouncesNumericUpDown.Size = new Size(68, 20);

			LightOptionsJitterNumericUpDown.Location = new Point(243, 86);
			LightOptionsJitterNumericUpDown.Size = new Size(68, 20);
			LightOptionsJitterNumericUpDown.DecimalPlaces = 3;
			LightOptionsJitterNumericUpDown.Increment = 0.003m;
			LightOptionsJitterNumericUpDown.Maximum = 4;

			LightOptionsButtonOK.Location = new Point(169, 138);
			LightOptionsButtonOK.Size = new Size(75, 23);
			LightOptionsButtonOK.Text = "OK";
			LightOptionsButtonOK.Click += new EventHandler(LightOptionsButtonOK_Click);

			LightOptionsButtonCancel.Location = new Point(258, 138);
			LightOptionsButtonCancel.Size = new Size(75, 23);
			LightOptionsButtonCancel.Text = "Cancel";
			LightOptionsButtonCancel.DialogResult = DialogResult.Cancel;
			LightOptionsButtonCancel.Click += new EventHandler(LightOptionsButtonCancel_Click);

			LightOptionsGroupBox.Controls.Add(LightOptionsFastExtraGroupBox);
			LightOptionsGroupBox.Controls.Add(LightOptionsNoModelShadowsCheckBox);
			LightOptionsGroupBox.Controls.Add(LightOptionsVerboseCheckBox);
			LightOptionsGroupBox.Controls.Add(LightOptionsTracesCheckBox);
			LightOptionsGroupBox.Controls.Add(LightOptionsMaxBouncesCheckBox);
			LightOptionsGroupBox.Controls.Add(LightOptionsJitterCheckBox);
			LightOptionsGroupBox.Controls.Add(LightOptionsTracesNumericUpDown);
			LightOptionsGroupBox.Controls.Add(LightOptionsMaxBouncesNumericUpDown);
			LightOptionsGroupBox.Controls.Add(LightOptionsJitterNumericUpDown);

			Controls.Add(LightOptionsGroupBox);
			Controls.Add(LightOptionsButtonOK);
			Controls.Add(LightOptionsButtonCancel);

			AcceptButton = LightOptionsButtonOK;
			CancelButton = LightOptionsButtonCancel;

			ResumeLayout(false);
			PerformLayout();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}