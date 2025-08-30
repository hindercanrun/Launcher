using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Launcher
{
  public class CreateMapForm : Form
  {
		private IContainer components;
		private GroupBox MapTemplatesGroupBox;
		private ListBox MapTemplatesListBox;
		private GroupBox MapNameGroupBox;
		private TextBox MapNameTextBox;
		private Button MapCreateButtonOK;
		private Button MapCreateButtonCancel;

		public CreateMapForm() => InitializeComponent();

		private void MapCreateButtonOK_Click(object sender, EventArgs e)
		{
			string mapTemplate = MapTemplatesListBox.Items[MapTemplatesListBox.SelectedIndex].ToString();
			string mapName = Launcher.FilterMP(MapNameTextBox.Text);

			bool flag = true;

			string[] mapFromTemplate = Launcher.CreateMapFromTemplate(mapTemplate, mapName, true);
			if (mapFromTemplate.Length > 0 && DialogResult.No == MessageBox.Show("Certain files would be overwritten:\n\n" + Launcher.StringArrayToString(mapFromTemplate) + "\nDo you want to continue?", "Should overwrite files?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
			{
				flag = false;
			}

			if (flag)
			{
				Launcher.CreateMapFromTemplate(mapTemplate, mapName);
			}

			DialogResult = DialogResult.OK;
			Close();
		}

		private void MapTemplatesListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			int selectedIndex = MapTemplatesListBox.SelectedIndex;
			MapCreateButtonOK.Enabled = selectedIndex >= 0;
			if (selectedIndex < 0)
			{
				return;
			}

			bool flag = Launcher.IsMultiplayerMapTemplate(MapTemplatesListBox.Items[selectedIndex].ToString());
			string name = Launcher.FilterMP(MapNameTextBox.Text);

			MapNameTextBox.Text = flag ? Launcher.MakeMP(name) : name;
		}

		private void LauncherCreateMapForm_Load(object sender, EventArgs e)
		{
			MapTemplatesListBox.Items.Clear();
			MapTemplatesListBox.Items.AddRange((object[]) Launcher.GetMapTemplatesList());
			MapTemplatesListBox.SelectedIndex = 0;
		}

		private void InitializeComponent()
		{
			components = new Container();

			MapTemplatesGroupBox = new GroupBox();
			MapTemplatesListBox = new ListBox();
			MapNameGroupBox = new GroupBox();
			MapNameTextBox = new TextBox();
			MapCreateButtonOK = new Button();
			MapCreateButtonCancel = new Button();

			SuspendLayout();
			ClientSize = new Size(414, 159);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			StartPosition = FormStartPosition.CenterParent;
			ShowInTaskbar = false;
			Text = "Create a New Map";
			Load += new EventHandler(LauncherCreateMapForm_Load);

			MapTemplatesGroupBox.Location = new Point(12, 12);
			MapTemplatesGroupBox.Size = new Size(132, 135);
			MapTemplatesGroupBox.Text = "Map Templates";

			MapTemplatesListBox.Location = new Point(6, 19);
			MapTemplatesListBox.Size = new Size(120, 108);
			MapTemplatesListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			MapTemplatesListBox.SelectedIndexChanged += new EventHandler(MapTemplatesListBox_SelectedIndexChanged);

			MapTemplatesGroupBox.Controls.Add(MapTemplatesListBox);

			MapNameGroupBox.Location = new Point(150, 12);
			MapNameGroupBox.Size = new Size(260, 49);
			MapNameGroupBox.Text = "Map Name";

			MapNameTextBox.Location = new Point(6, 19);
			MapNameTextBox.Size = new Size(248, 20);
			MapNameTextBox.MaxLength = 15;
			MapNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

			MapNameGroupBox.Controls.Add(MapNameTextBox);

			MapCreateButtonOK.Location = new Point(246, 124);
			MapCreateButtonOK.Size = new Size(75, 23);
			MapCreateButtonOK.Text = "OK";
			MapCreateButtonOK.Enabled = false;
			MapCreateButtonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			MapCreateButtonOK.Click += new EventHandler(MapCreateButtonOK_Click);

			MapCreateButtonCancel.Location = new Point(327, 124);
			MapCreateButtonCancel.Size = new Size(75, 23);
			MapCreateButtonCancel.Text = "Cancel";
			MapCreateButtonCancel.DialogResult = DialogResult.Cancel;
			MapCreateButtonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

			Controls.Add(MapTemplatesGroupBox);
			Controls.Add(MapNameGroupBox);
			Controls.Add(MapCreateButtonOK);
			Controls.Add(MapCreateButtonCancel);

			AcceptButton = MapCreateButtonOK;
			CancelButton = MapCreateButtonCancel;

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