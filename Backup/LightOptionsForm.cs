// Decompiled with JetBrains decompiler
// Type: LauncherCS.LightOptionsForm
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 95679BB7-C92C-4A2A-8DBF-1C9AAEB82003
// Assembly location: D:\pluto_t4_full_game\pluto_t4_full_game\bin\Launcher.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace LauncherCS
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

    public LightOptionsForm() => this.InitializeComponent();

    private void LightOptionsFormUpdate()
    {
      this.LightOptionsTracesNumericUpDown.Enabled = this.LightOptionsTracesCheckBox.Checked;
      this.LightOptionsMaxBouncesNumericUpDown.Enabled = this.LightOptionsMaxBouncesCheckBox.Checked;
      this.LightOptionsJitterNumericUpDown.Enabled = this.LightOptionsJitterCheckBox.Checked;
    }

    private void LightOptionsForm_Load(object sender, EventArgs e)
    {
      this.LightOptionsExtraRadioButton.Checked = Launcher.mapSettings.GetBoolean("lightoptions_extra");
      this.LightOptionsFastRadioButton.Checked = !this.LightOptionsExtraRadioButton.Checked;
      this.LightOptionsNoModelShadowsCheckBox.Checked = Launcher.mapSettings.GetBoolean("lightoptions_nomodelshadow");
      this.LightOptionsVerboseCheckBox.Checked = Launcher.mapSettings.GetBoolean("lightoptions_verbose");
      this.LightOptionsTracesCheckBox.Checked = Launcher.mapSettings.GetBoolean("lightoptions_traces");
      this.LightOptionsMaxBouncesCheckBox.Checked = Launcher.mapSettings.GetBoolean("lightoptions_maxbounces");
      this.LightOptionsJitterCheckBox.Checked = Launcher.mapSettings.GetBoolean("lightoptions_jitter");
      Launcher.SetNumericUpDownValue(this.LightOptionsTracesNumericUpDown, Launcher.mapSettings.GetDecimal("lightoptions_traces_val"));
      Launcher.SetNumericUpDownValue(this.LightOptionsMaxBouncesNumericUpDown, Launcher.mapSettings.GetDecimal("lightoptions_maxbounces_val"));
      Launcher.SetNumericUpDownValue(this.LightOptionsJitterNumericUpDown, Launcher.mapSettings.GetDecimal("lightoptions_jitter_val"));
      this.LightOptionsFormUpdate();
    }

    private void LightOptionsTracesCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      this.LightOptionsFormUpdate();
    }

    private void LightOptionsMaxBouncesCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      this.LightOptionsFormUpdate();
    }

    private void LightOptionsJitterCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      this.LightOptionsFormUpdate();
    }

    private void LightOptionsButtonOK_Click(object sender, EventArgs e)
    {
      Launcher.mapSettings.SetBoolean("lightoptions_extra", this.LightOptionsExtraRadioButton.Checked);
      Launcher.mapSettings.SetBoolean("lightoptions_nomodelshadow", this.LightOptionsNoModelShadowsCheckBox.Checked);
      Launcher.mapSettings.SetBoolean("lightoptions_verbose", this.LightOptionsVerboseCheckBox.Checked);
      Launcher.mapSettings.SetBoolean("lightoptions_traces", this.LightOptionsTracesCheckBox.Checked);
      Launcher.mapSettings.SetBoolean("lightoptions_maxbounces", this.LightOptionsMaxBouncesCheckBox.Checked);
      Launcher.mapSettings.SetBoolean("lightoptions_jitter", this.LightOptionsJitterCheckBox.Checked);
      Launcher.mapSettings.SetDecimal("lightoptions_traces_val", this.LightOptionsTracesNumericUpDown.Value);
      Launcher.mapSettings.SetDecimal("lightoptions_maxbounces_val", this.LightOptionsMaxBouncesNumericUpDown.Value);
      Launcher.mapSettings.SetDecimal("lightoptions_jitter_val", this.LightOptionsJitterNumericUpDown.Value);
      this.Close();
    }

    private void LightOptionsButtonCancel_Click(object sender, EventArgs e) => this.Close();

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.LightOptionsGroupBox = new GroupBox();
      this.LightOptionsFastExtraGroupBox = new GroupBox();
      this.LightOptionsExtraRadioButton = new RadioButton();
      this.LightOptionsFastRadioButton = new RadioButton();
      this.LightOptionsJitterNumericUpDown = new NumericUpDown();
      this.LightOptionsMaxBouncesNumericUpDown = new NumericUpDown();
      this.LightOptionsTracesNumericUpDown = new NumericUpDown();
      this.LightOptionsJitterCheckBox = new CheckBox();
      this.LightOptionsMaxBouncesCheckBox = new CheckBox();
      this.LightOptionsTracesCheckBox = new CheckBox();
      this.LightOptionsVerboseCheckBox = new CheckBox();
      this.LightOptionsNoModelShadowsCheckBox = new CheckBox();
      this.LightOptionsButtonOK = new Button();
      this.LightOptionsButtonCancel = new Button();
      this.LightOptionsGroupBox.SuspendLayout();
      this.LightOptionsFastExtraGroupBox.SuspendLayout();
      this.LightOptionsJitterNumericUpDown.BeginInit();
      this.LightOptionsMaxBouncesNumericUpDown.BeginInit();
      this.LightOptionsTracesNumericUpDown.BeginInit();
      this.SuspendLayout();
      this.LightOptionsGroupBox.Controls.Add((Control) this.LightOptionsFastExtraGroupBox);
      this.LightOptionsGroupBox.Controls.Add((Control) this.LightOptionsJitterNumericUpDown);
      this.LightOptionsGroupBox.Controls.Add((Control) this.LightOptionsMaxBouncesNumericUpDown);
      this.LightOptionsGroupBox.Controls.Add((Control) this.LightOptionsTracesNumericUpDown);
      this.LightOptionsGroupBox.Controls.Add((Control) this.LightOptionsJitterCheckBox);
      this.LightOptionsGroupBox.Controls.Add((Control) this.LightOptionsMaxBouncesCheckBox);
      this.LightOptionsGroupBox.Controls.Add((Control) this.LightOptionsTracesCheckBox);
      this.LightOptionsGroupBox.Controls.Add((Control) this.LightOptionsVerboseCheckBox);
      this.LightOptionsGroupBox.Controls.Add((Control) this.LightOptionsNoModelShadowsCheckBox);
      this.LightOptionsGroupBox.Location = new Point(8, 11);
      this.LightOptionsGroupBox.Name = "LightOptionsGroupBox";
      this.LightOptionsGroupBox.Size = new Size(325, 121);
      this.LightOptionsGroupBox.TabIndex = 0;
      this.LightOptionsGroupBox.TabStop = false;
      this.LightOptionsGroupBox.Text = "Compile Light Options";
      this.LightOptionsFastExtraGroupBox.Controls.Add((Control) this.LightOptionsExtraRadioButton);
      this.LightOptionsFastExtraGroupBox.Controls.Add((Control) this.LightOptionsFastRadioButton);
      this.LightOptionsFastExtraGroupBox.Location = new Point(12, 19);
      this.LightOptionsFastExtraGroupBox.Name = "LightOptionsFastExtraGroupBox";
      this.LightOptionsFastExtraGroupBox.RightToLeft = RightToLeft.No;
      this.LightOptionsFastExtraGroupBox.Size = new Size(117, 32);
      this.LightOptionsFastExtraGroupBox.TabIndex = 9;
      this.LightOptionsFastExtraGroupBox.TabStop = false;
      this.LightOptionsExtraRadioButton.AutoSize = true;
      this.LightOptionsExtraRadioButton.Location = new Point(57, 9);
      this.LightOptionsExtraRadioButton.Name = "LightOptionsExtraRadioButton";
      this.LightOptionsExtraRadioButton.Size = new Size(49, 17);
      this.LightOptionsExtraRadioButton.TabIndex = 1;
      this.LightOptionsExtraRadioButton.Text = "Extra";
      this.LightOptionsExtraRadioButton.UseVisualStyleBackColor = true;
      this.LightOptionsFastRadioButton.AutoSize = true;
      this.LightOptionsFastRadioButton.Checked = true;
      this.LightOptionsFastRadioButton.Location = new Point(6, 9);
      this.LightOptionsFastRadioButton.Name = "LightOptionsFastRadioButton";
      this.LightOptionsFastRadioButton.Size = new Size(45, 17);
      this.LightOptionsFastRadioButton.TabIndex = 0;
      this.LightOptionsFastRadioButton.TabStop = true;
      this.LightOptionsFastRadioButton.Text = "Fast";
      this.LightOptionsFastRadioButton.UseVisualStyleBackColor = true;
      this.LightOptionsJitterNumericUpDown.DecimalPlaces = 3;
      this.LightOptionsJitterNumericUpDown.Increment = new Decimal(new int[4]
      {
        1,
        0,
        0,
        196608
      });
      this.LightOptionsJitterNumericUpDown.Location = new Point(243, 86);
      this.LightOptionsJitterNumericUpDown.Maximum = new Decimal(new int[4]
      {
        4,
        0,
        0,
        0
      });
      this.LightOptionsJitterNumericUpDown.Name = "LightOptionsJitterNumericUpDown";
      this.LightOptionsJitterNumericUpDown.Size = new Size(68, 20);
      this.LightOptionsJitterNumericUpDown.TabIndex = 8;
      this.LightOptionsMaxBouncesNumericUpDown.Location = new Point(242, 60);
      this.LightOptionsMaxBouncesNumericUpDown.Name = "LightOptionsMaxBouncesNumericUpDown";
      this.LightOptionsMaxBouncesNumericUpDown.Size = new Size(68, 20);
      this.LightOptionsMaxBouncesNumericUpDown.TabIndex = 7;
      this.LightOptionsTracesNumericUpDown.Location = new Point(243, 34);
      this.LightOptionsTracesNumericUpDown.Maximum = new Decimal(new int[4]
      {
        500,
        0,
        0,
        0
      });
      this.LightOptionsTracesNumericUpDown.Name = "LightOptionsTracesNumericUpDown";
      this.LightOptionsTracesNumericUpDown.Size = new Size(68, 20);
      this.LightOptionsTracesNumericUpDown.TabIndex = 6;
      this.LightOptionsJitterCheckBox.AutoSize = true;
      this.LightOptionsJitterCheckBox.FlatStyle = FlatStyle.Popup;
      this.LightOptionsJitterCheckBox.Location = new Point(147, 80);
      this.LightOptionsJitterCheckBox.Name = "LightOptionsJitterCheckBox";
      this.LightOptionsJitterCheckBox.Size = new Size(46, 17);
      this.LightOptionsJitterCheckBox.TabIndex = 5;
      this.LightOptionsJitterCheckBox.Text = "Jitter";
      this.LightOptionsJitterCheckBox.UseVisualStyleBackColor = true;
      this.LightOptionsJitterCheckBox.CheckedChanged += new EventHandler(this.LightOptionsJitterCheckBox_CheckedChanged);
      this.LightOptionsMaxBouncesCheckBox.AutoSize = true;
      this.LightOptionsMaxBouncesCheckBox.FlatStyle = FlatStyle.Popup;
      this.LightOptionsMaxBouncesCheckBox.Location = new Point(147, 57);
      this.LightOptionsMaxBouncesCheckBox.Name = "LightOptionsMaxBouncesCheckBox";
      this.LightOptionsMaxBouncesCheckBox.Size = new Size(89, 17);
      this.LightOptionsMaxBouncesCheckBox.TabIndex = 4;
      this.LightOptionsMaxBouncesCheckBox.Text = "Max Bounces";
      this.LightOptionsMaxBouncesCheckBox.UseVisualStyleBackColor = true;
      this.LightOptionsMaxBouncesCheckBox.CheckedChanged += new EventHandler(this.LightOptionsMaxBouncesCheckBox_CheckedChanged);
      this.LightOptionsTracesCheckBox.AutoSize = true;
      this.LightOptionsTracesCheckBox.FlatStyle = FlatStyle.Popup;
      this.LightOptionsTracesCheckBox.Location = new Point(147, 34);
      this.LightOptionsTracesCheckBox.Name = "LightOptionsTracesCheckBox";
      this.LightOptionsTracesCheckBox.Size = new Size(57, 17);
      this.LightOptionsTracesCheckBox.TabIndex = 3;
      this.LightOptionsTracesCheckBox.Text = "Traces";
      this.LightOptionsTracesCheckBox.UseVisualStyleBackColor = true;
      this.LightOptionsTracesCheckBox.CheckedChanged += new EventHandler(this.LightOptionsTracesCheckBox_CheckedChanged);
      this.LightOptionsVerboseCheckBox.AutoSize = true;
      this.LightOptionsVerboseCheckBox.FlatStyle = FlatStyle.Popup;
      this.LightOptionsVerboseCheckBox.Location = new Point(12, 80);
      this.LightOptionsVerboseCheckBox.Name = "LightOptionsVerboseCheckBox";
      this.LightOptionsVerboseCheckBox.Size = new Size(63, 17);
      this.LightOptionsVerboseCheckBox.TabIndex = 2;
      this.LightOptionsVerboseCheckBox.Text = "Verbose";
      this.LightOptionsVerboseCheckBox.UseVisualStyleBackColor = true;
      this.LightOptionsNoModelShadowsCheckBox.AutoSize = true;
      this.LightOptionsNoModelShadowsCheckBox.FlatStyle = FlatStyle.Popup;
      this.LightOptionsNoModelShadowsCheckBox.Location = new Point(12, 57);
      this.LightOptionsNoModelShadowsCheckBox.Name = "LightOptionsNoModelShadowsCheckBox";
      this.LightOptionsNoModelShadowsCheckBox.Size = new Size(117, 17);
      this.LightOptionsNoModelShadowsCheckBox.TabIndex = 1;
      this.LightOptionsNoModelShadowsCheckBox.Text = "No Model Shadows";
      this.LightOptionsNoModelShadowsCheckBox.UseVisualStyleBackColor = true;
      this.LightOptionsButtonOK.Location = new Point(169, 138);
      this.LightOptionsButtonOK.Name = "LightOptionsButtonOK";
      this.LightOptionsButtonOK.Size = new Size(75, 23);
      this.LightOptionsButtonOK.TabIndex = 1;
      this.LightOptionsButtonOK.Text = "OK";
      this.LightOptionsButtonOK.UseVisualStyleBackColor = true;
      this.LightOptionsButtonOK.Click += new EventHandler(this.LightOptionsButtonOK_Click);
      this.LightOptionsButtonCancel.DialogResult = DialogResult.Cancel;
      this.LightOptionsButtonCancel.Location = new Point(258, 138);
      this.LightOptionsButtonCancel.Name = "LightOptionsButtonCancel";
      this.LightOptionsButtonCancel.Size = new Size(75, 23);
      this.LightOptionsButtonCancel.TabIndex = 2;
      this.LightOptionsButtonCancel.Text = "Cancel";
      this.LightOptionsButtonCancel.UseVisualStyleBackColor = true;
      this.LightOptionsButtonCancel.Click += new EventHandler(this.LightOptionsButtonCancel_Click);
      this.AcceptButton = (IButtonControl) this.LightOptionsButtonOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.LightOptionsButtonCancel;
      this.ClientSize = new Size(345, 170);
      this.ControlBox = false;
      this.Controls.Add((Control) this.LightOptionsButtonCancel);
      this.Controls.Add((Control) this.LightOptionsButtonOK);
      this.Controls.Add((Control) this.LightOptionsGroupBox);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (LightOptionsForm);
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Advanced Users Options";
      this.Load += new EventHandler(this.LightOptionsForm_Load);
      this.LightOptionsGroupBox.ResumeLayout(false);
      this.LightOptionsGroupBox.PerformLayout();
      this.LightOptionsFastExtraGroupBox.ResumeLayout(false);
      this.LightOptionsFastExtraGroupBox.PerformLayout();
      this.LightOptionsJitterNumericUpDown.EndInit();
      this.LightOptionsMaxBouncesNumericUpDown.EndInit();
      this.LightOptionsTracesNumericUpDown.EndInit();
      this.ResumeLayout(false);
    }
  }
}
