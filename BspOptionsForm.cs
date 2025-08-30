using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Launcher
{
	public class BspOptionsForm : Form
	{
		private IContainer components;
		private GroupBox BspOptionsGroupBox;
		private Label BspOptionsExtraOptionsLabelText;
		private CheckBox BspOptionsDebugLightsCheckBox;
		private NumericUpDown BspOptionsBlockSizeNumericUpDown;
		private NumericUpDown BspOptionsSampleScaleNumericUpDown;
		private CheckBox BspOptionsSampleScaleCheckBox;
		private CheckBox BspOptionsBlockSizeCheckBox;
		private CheckBox BspOptionsOnlyEntsCheckBox;
		private Button BspOptionsButtonOK;
		private Button BspOptionsButtonCancel;
		private TextBox BspOptionsExtraOptionsTextBox;

		public BspOptionsForm() => InitializeComponent();

		private void BspOptionsFormUpdate()
		{
			BspOptionsBlockSizeNumericUpDown.Enabled = BspOptionsBlockSizeCheckBox.Checked;
			BspOptionsSampleScaleNumericUpDown.Enabled = BspOptionsSampleScaleCheckBox.Checked;
		}

		private void BspOptionsForm_Load(object sender, EventArgs e)
		{
			BspOptionsOnlyEntsCheckBox.Checked = Launcher.mapSettings.GetBoolean("bspoptions_onlyents");
			BspOptionsBlockSizeCheckBox.Checked = Launcher.mapSettings.GetBoolean("bspoptions_blocksize");
			BspOptionsSampleScaleCheckBox.Checked = Launcher.mapSettings.GetBoolean("bspoptions_samplescale");
			BspOptionsDebugLightsCheckBox.Checked = Launcher.mapSettings.GetBoolean("bspoptions_debuglightmaps");

			Launcher.SetNumericUpDownValue(BspOptionsBlockSizeNumericUpDown, Launcher.mapSettings.GetDecimal("bspoptions_blocksize_val"));
			Launcher.SetNumericUpDownValue(BspOptionsSampleScaleNumericUpDown, Launcher.mapSettings.GetDecimal("bspoptions_samplescale_val"));

			BspOptionsExtraOptionsTextBox.Text = Launcher.mapSettings.GetString("bspoptions_extraoptions");
			BspOptionsFormUpdate();
		}

		private void BspOptionsBlockSizeCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			BspOptionsFormUpdate();
		}

		private void BspOptionsSampleScaleCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			BspOptionsFormUpdate();
		}

		private void BspOptionsButtonOK_Click(object sender, EventArgs e)
		{
			Launcher.mapSettings.SetBoolean("bspoptions_onlyents", BspOptionsOnlyEntsCheckBox.Checked);
			Launcher.mapSettings.SetBoolean("bspoptions_blocksize", BspOptionsBlockSizeCheckBox.Checked);
			Launcher.mapSettings.SetBoolean("bspoptions_samplescale", BspOptionsSampleScaleCheckBox.Checked);
			Launcher.mapSettings.SetBoolean("bspoptions_debuglightmaps", BspOptionsDebugLightsCheckBox.Checked);
			Launcher.mapSettings.SetDecimal("bspoptions_blocksize_val", BspOptionsBlockSizeNumericUpDown.Value);
			Launcher.mapSettings.SetDecimal("bspoptions_samplescale_val", this.BspOptionsSampleScaleNumericUpDown.Value);
			Launcher.mapSettings.SetString("bspoptions_extraoptions", BspOptionsExtraOptionsTextBox.Text);
			Close();
		}

		private void BspOptionsButtonCancel_Click(object sender, EventArgs e) => Close();

		private void InitializeComponent()
		{
			components = new Container();

			BspOptionsGroupBox = new GroupBox();
			BspOptionsExtraOptionsLabelText = new Label();
			BspOptionsDebugLightsCheckBox = new CheckBox();
			BspOptionsExtraOptionsTextBox = new TextBox();
			BspOptionsBlockSizeNumericUpDown = new NumericUpDown();
			BspOptionsSampleScaleNumericUpDown = new NumericUpDown();
			BspOptionsSampleScaleCheckBox = new CheckBox();
			BspOptionsBlockSizeCheckBox = new CheckBox();
			BspOptionsOnlyEntsCheckBox = new CheckBox();
			BspOptionsButtonOK = new Button();
			BspOptionsButtonCancel = new Button();

			BspOptionsGroupBox.Text = "Compile BSP Options";
			BspOptionsGroupBox.Location = new Point(12, 12);
			BspOptionsGroupBox.Size = new Size(291, 126);
			BspOptionsGroupBox.Controls.Add(BspOptionsExtraOptionsLabelText);
			BspOptionsGroupBox.Controls.Add(BspOptionsDebugLightsCheckBox);
			BspOptionsGroupBox.Controls.Add(BspOptionsExtraOptionsTextBox);
			BspOptionsGroupBox.Controls.Add(BspOptionsBlockSizeNumericUpDown);
			BspOptionsGroupBox.Controls.Add(BspOptionsSampleScaleNumericUpDown);
			BspOptionsGroupBox.Controls.Add(BspOptionsSampleScaleCheckBox);
			BspOptionsGroupBox.Controls.Add(BspOptionsBlockSizeCheckBox);
			BspOptionsGroupBox.Controls.Add(BspOptionsOnlyEntsCheckBox);

			BspOptionsExtraOptionsLabelText.Text = "Extra BSP Options:";
			BspOptionsExtraOptionsLabelText.Location = new Point(7, 83);
			BspOptionsExtraOptionsLabelText.AutoSize = true;

			BspOptionsDebugLightsCheckBox.Text = "Debug Lightmaps";
			BspOptionsDebugLightsCheckBox.Location = new Point(10, 50);
			BspOptionsDebugLightsCheckBox.FlatStyle = FlatStyle.Popup;

			BspOptionsSampleScaleCheckBox.Text = "Sample Scale";
			BspOptionsSampleScaleCheckBox.Location = new Point(116, 50);
			BspOptionsSampleScaleCheckBox.FlatStyle = FlatStyle.Popup;
			BspOptionsSampleScaleCheckBox.CheckedChanged += BspOptionsSampleScaleCheckBox_CheckedChanged;

			BspOptionsBlockSizeCheckBox.Text = "Block Size";
			BspOptionsBlockSizeCheckBox.Location = new Point(116, 19);
			BspOptionsBlockSizeCheckBox.FlatStyle = FlatStyle.Popup;
			BspOptionsBlockSizeCheckBox.CheckedChanged += BspOptionsBlockSizeCheckBox_CheckedChanged;

			BspOptionsOnlyEntsCheckBox.Text = "Only Ents";
			BspOptionsOnlyEntsCheckBox.Location = new Point(10, 19);
			BspOptionsOnlyEntsCheckBox.FlatStyle = FlatStyle.Popup;

			BspOptionsBlockSizeNumericUpDown.DecimalPlaces = 2;
			BspOptionsBlockSizeNumericUpDown.Increment = 0.01M;
			BspOptionsBlockSizeNumericUpDown.Maximum = 64;
			BspOptionsBlockSizeNumericUpDown.Location = new Point(211, 19);
			BspOptionsBlockSizeNumericUpDown.Size = new Size(71, 20);

			BspOptionsSampleScaleNumericUpDown.DecimalPlaces = 2;
			BspOptionsSampleScaleNumericUpDown.Increment = 0.01M;
			BspOptionsSampleScaleNumericUpDown.Minimum = 0.01M;
			BspOptionsSampleScaleNumericUpDown.Location = new Point(211, 50);
			BspOptionsSampleScaleNumericUpDown.Size = new Size(71, 20);

			BspOptionsExtraOptionsTextBox.Location = new Point(6, 99);
			BspOptionsExtraOptionsTextBox.Size = new Size(276, 20);

			BspOptionsButtonOK.Text = "OK";
			BspOptionsButtonOK.Location = new Point(147, 154);
			BspOptionsButtonOK.Click += BspOptionsButtonOK_Click;

			BspOptionsButtonCancel.Text = "Cancel";
			BspOptionsButtonCancel.Location = new Point(228, 154);
			BspOptionsButtonCancel.Click += BspOptionsButtonCancel_Click;
			BspOptionsButtonCancel.DialogResult = DialogResult.Cancel;

			AcceptButton = BspOptionsButtonOK;
			CancelButton = BspOptionsButtonCancel;
			ClientSize = new Size(313, 189);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			ControlBox = false;
			StartPosition = FormStartPosition.CenterParent;
			Text = "Advanced Users Options";
			Controls.Add(BspOptionsButtonOK);
			Controls.Add(BspOptionsButtonCancel);
			Controls.Add(BspOptionsGroupBox);

			Load += BspOptionsForm_Load;
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