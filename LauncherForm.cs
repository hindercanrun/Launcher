using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace Launcher
{
	public class LauncherForm : Form
	{
		private ComboBox[] dvarComboBoxes = new ComboBox[0];
		private Hashtable processTable = new Hashtable();
		private ArrayList processList = new ArrayList();
		private Process consoleProcess;
		private DateTime consoleProcessStartTime;
		private string mapName;
		private string modName;
		private long consoleTicksWhenLastFocus = DateTime.Now.Ticks;
		private Mutex consoleMutex = new Mutex();
		private IContainer components;
		private SplitContainer LauncherSplitter;
		private TabControl LauncherTab;
		private TabPage LauncherTabCompileLevel;
		private TabPage LauncherTabModBuilder;
		private TabPage LauncherTabRunGame;
		private GroupBox LauncherApplicationsGroupBox;
		private Button LauncherButtonRunConverter;
		private Button LauncherButtonAssetManager;
		private Button LauncherButtonEffectsEd;
		private Button LauncherButtonRadiant;
		private ListBox LauncherMapList;
		private Button LauncherButtonCancel;
		private ListBox LauncherProcessList;
		private Button LauncherButtonAssetViewer;
		private GroupBox LauncherProcessGroupBox;
		private Button LauncherButtonTest;
		private Button LauncherCreateMapButton;
		private GroupBox LauncherRunGameExeTypeGroupBox;
		private RadioButton LauncherRunGameExeTypeMpRadioButton;
		private GroupBox LauncherRunGameModGroupBox;
		private ComboBox LauncherRunGameModComboBox;
		private GroupBox LauncherRunGameCommandLineGroupBox;
		private TextBox LauncherRunGameCommandLineTextBox;
		private Button LauncherRunGameButton;
		private GroupBox LauncherRunGameCustomCommandLineGroupBox;
		private TextBox LauncherRunGameCustomCommandLineTextBox;
		private GroupBox LauncherCompileLevelOptionsGroupBox;
		private CheckBox LauncherConnectPathsCheckBox;
		private CheckBox LauncherCompileVisCheckBox;
		private CheckBox LauncherCompileLightsCheckBox;
		private CheckBox LauncherCompileBSPCheckBox;
		private CheckBox LauncherCompileReflectionsCheckBox;
		private Button LauncherCompileLightsButton;
    private Button LauncherCompileBSPButton;
    private CheckBox LauncherUseRunGameTypeOptionsCheckBox;
    private CheckBox LauncherRunMapAfterCompileCheckBox;
    private CheckBox LauncherBspInfoCheckBox;
    private CheckBox LauncherBuildFastFilesCheckBox;
    private Button LauncherCompileLevelButton;
    private GroupBox LauncherCompileLevelOptionsSplitterGroupBox;
    private Label LauncherCustomRunOptionsLabel;
    private TextBox LauncherCustomRunOptionsTextBox;
    private TextBox LauncherProcessTextBox;
    private TextBox LauncherProcessTimeElapsedTextBox;
    private System.Windows.Forms.Timer LauncherTimer;
    private RadioButton LauncherRunGameTypeRadioButton;
    internal GroupBox LauncherIwdFileGroupBox;
    internal GroupBox LauncherFastFileCsvGroupBox;
    internal RichTextBox LauncherFastFileCsvTextBox;
    internal GroupBox LauncherModGroupBox;
    internal Button LauncherModBuildButton;
    internal CheckBox LauncherModBuildSoundsCheckBox;
    internal CheckBox LauncherModVerboseCheckBox;
    internal CheckBox LauncherModBuildIwdFileCheckBox;
    internal CheckBox LauncherModBuildFastFilesCheckBox;
    internal ComboBox LauncherModComboBox;
    private TreeView LauncherIwdFileTree;
    private ComboBox LauncherModSpecificMapComboBox;
    private CheckBox LauncherModSpecificMapCheckBox;
    private Panel LauncherGameOptionsPanel;
    private Button LauncherClearConsoleButton;
    private LinkLabel LauncherWikiLabel;
    public RichTextBox LauncherConsole;
    private Button LauncherDeleteMapButton;
    private LinkLabel LauncherAboutLabel;
    private FileSystemWatcher LauncherMapFilesSystemWatcher;
    private FileSystemWatcher LauncherModsDirectorySystemWatcher;
    private CheckBox LauncherGridCollectDotsCheckBox;
    private GroupBox LauncherGridFileGroupBox;
    private Button LauncherGridMakeNewButton;
    private Button LauncherGridEditExistingButton;

    public LauncherForm() => this.InitializeComponent();

    private void UpdateDVars()
    {
      Panel gameOptionsPanel = this.LauncherGameOptionsPanel;
      int height = 34;
      int num1 = -height;
      int num2 = 0;
      bool flag = true;
      Color backColor = gameOptionsPanel.BackColor;
      Color color = Color.FromArgb((int) backColor.R * 14 / 15, (int) backColor.G * 14 / 15, (int) backColor.B * 14 / 15);
      this.dvarComboBoxes = new ComboBox[Launcher.dvars.Length];
      foreach (DVar dvar in Launcher.dvars)
      {
        Panel panel = new Panel();
        panel.SetBounds(0, num1 += height, gameOptionsPanel.ClientSize.Width, height);
        panel.BackColor = (flag = !flag) ? backColor : color;
        panel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        panel.Click += new EventHandler(this.LauncherGameOptionsFlowPanel_Click);
        Label label = new Label();
        label.SetBounds(4, 0, gameOptionsPanel.ClientSize.Width - 220, height);
        label.TextAlign = ContentAlignment.MiddleLeft;
        label.Text = dvar.name + " (" + dvar.description + ")";
        label.Click += new EventHandler(this.LauncherGameOptionsFlowPanel_Click);
        panel.Controls.Add((Control) label);
        ComboBox comboBox = new ComboBox();
        comboBox.Tag = (object) dvar.name;
        comboBox.Items.Add((object) "(not set)");
        comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        if (dvar.isDecimal)
        {
          Decimal decimalMin = dvar.decimalMin;
          while (decimalMin <= dvar.decimalMax)
          {
            comboBox.Items.Add((object) decimalMin.ToString());
            decimalMin += dvar.decimalIncrement;
          }
        }
        else if (dvar.name == "devmap")
          comboBox.Items.AddRange((object[]) Launcher.GetMapList());
        comboBox.SelectedIndex = 0;
        comboBox.SetBounds(gameOptionsPanel.ClientSize.Width - 205, 8, 200, height);
        comboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        panel.Controls.Add((Control) comboBox);
        comboBox.BringToFront();
        gameOptionsPanel.Controls.Add((Control) panel);
        this.dvarComboBoxes[num2++] = comboBox;
      }
    }

    private string FormatDVar(ComboBox cb)
    {
      string str1 = "";
      if (cb.SelectedItem != null && cb.SelectedIndex > 0)
        str1 = cb.SelectedItem.ToString();
      else if (cb.Items[0].ToString() != cb.Text)
        str1 = cb.Text;
      string str2 = str1.Trim();
      if (!(str2 != ""))
        return "";
      return "+set " + cb.Tag + " " + str2 + " ";
    }

    private string FormatDvars()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (ComboBox dvarComboBox in this.dvarComboBoxes)
        stringBuilder.Append(this.FormatDVar(dvarComboBox));
      return stringBuilder.ToString();
    }

    private void LauncherForm_Load(object sender, EventArgs e)
    {
      this.UpdateDVars();
      this.UpdateMapList();
      this.UpdateModList();
      this.EnableMapList();
      this.UpdateStopProcessButton();
      this.LauncherMapFilesSystemWatcher.Path = Launcher.GetMapSourceDirectory();
      this.LauncherModsDirectorySystemWatcher.Path = Launcher.GetModsDirectory();
      this.LauncherMapFilesSystemWatcher.EnableRaisingEvents = true;
      this.LauncherModsDirectorySystemWatcher.EnableRaisingEvents = true;
      LauncherForm launcherForm = this;
      launcherForm.Text = launcherForm.Text + " - " + Launcher.GetRootDirectory();
    }

    private event LauncherForm.ProcessFinishedDelegate processFinishedDelegate;

    private void UpdateRunGameCommandLine()
    {
    }

    private void UpdateConsoleColor()
    {
      this.LauncherConsole.BackColor = this.consoleProcess == null ? Color.White : Color.LightGoldenrodYellow;
    }

    private void UpdateStopProcessButton()
    {
      int selectedIndex = this.LauncherProcessList.SelectedIndex;
      if (selectedIndex < 0)
      {
        this.LauncherButtonCancel.Enabled = false;
        this.LauncherButtonCancel.Text = "No Active Process\n\nStart one and then use this button to stop it";
      }
      else
      {
        this.LauncherButtonCancel.Enabled = true;
        if (((DictionaryEntry) this.processList[selectedIndex]).Key == this.consoleProcess)
          this.LauncherButtonCancel.Text = "Stop Console Process\n\n" + Path.GetFileNameWithoutExtension(((DictionaryEntry) this.processList[selectedIndex]).Value.ToString());
        else
          this.LauncherButtonCancel.Text = "Stop Application\n\n" + Path.GetFileNameWithoutExtension(((DictionaryEntry) this.processList[selectedIndex]).Value.ToString());
      }
    }

    private void UpdateMapList()
    {
      object selectedItem = this.LauncherMapList.SelectedItem;
      int selectedIndex = this.LauncherMapList.SelectedIndex;
      this.LauncherMapList.Items.Clear();
      this.LauncherMapList.Items.AddRange((object[]) Launcher.GetMapList());
      if (this.LauncherMapList.Items.Count == 0)
        return;
      this.LauncherMapList.SelectedItem = selectedItem;
      if (this.LauncherMapList.SelectedItem != null)
        return;
      this.LauncherMapList.SelectedIndex = Math.Max(0, Math.Min(selectedIndex, this.LauncherMapList.Items.Count - 1));
    }

    private void UpdateModList()
    {
      ComboBox[] comboBoxArray = new ComboBox[3]
      {
        this.LauncherRunGameModComboBox,
        this.LauncherModComboBox,
        this.LauncherModSpecificMapComboBox
      };
      string[] modList = Launcher.GetModList();
      foreach (ComboBox comboBox in comboBoxArray)
        comboBox.Items.Clear();
      this.LauncherRunGameModComboBox.Items.Add((object) "(not set)");
      foreach (ComboBox comboBox in comboBoxArray)
      {
        comboBox.Items.AddRange((object[]) modList);
        if (comboBox.Items.Count > 0)
          comboBox.SelectedIndex = 0;
      }
    }

private void UpdateProcessList()
{
    this.LauncherProcessList.Invoke((Action)(() =>
    {
        this.processList.Clear();
        this.LauncherProcessList.Items.Clear();

        foreach (DictionaryEntry dictionaryEntry in this.processTable)
        {
            this.processList.Add(dictionaryEntry);
            this.LauncherProcessList.Items.Add(Path.GetFileNameWithoutExtension((string)dictionaryEntry.Value));
        }

        if (this.LauncherProcessList.SelectedIndex < 0 && this.LauncherProcessList.Items.Count > 0)
            this.LauncherProcessList.SelectedIndex = 0;

        this.UpdateStopProcessButton();
    }));
}

		private void WriteConsole(string s, bool isStdError)
		{
			if (s == null)
				return;

			long ticks = DateTime.Now.Ticks;
			bool doFocus = ticks - this.consoleTicksWhenLastFocus > 10000000L;
			if (doFocus)
				this.consoleTicksWhenLastFocus = ticks;

			// Use Action or MethodInvoker instead of Delegate
			this.LauncherConsole.Invoke((Action)(() =>
			{
				Color selectionColor = this.LauncherConsole.SelectionColor;
				Font selectionFont = this.LauncherConsole.SelectionFont;
				bool flag1 = isStdError || s.Contains("ERROR:");
				bool flag2 = s.Contains("WARNING:");

				if (flag1 || flag2)
				{
					this.LauncherConsole.SelectionFont = new Font(this.LauncherConsole.SelectionFont, FontStyle.Bold);
					this.LauncherConsole.SelectionColor = flag1 ? Color.Red : Color.Green;
				}

				this.LauncherConsole.AppendText(s + "\n");

				if (doFocus)
					this.LauncherConsole.Focus();

				if (flag1 || flag2)
				{
					this.LauncherConsole.SelectionColor = selectionColor;
					this.LauncherConsole.SelectionFont = selectionFont;
				}
			}));
		}


		private void WriteMessage(string s) => this.WriteConsole(s, false);

    private void WriteError(string s) => this.WriteConsole(s, true);

    private void LaunchProcessHelper(
      bool shouldRun,
      LauncherForm.ProcessFinishedDelegate nextStage,
      Process lastProcess,
      string processName,
      string processOptions,
      string workingDirectory)
    {
      if (lastProcess != null && lastProcess.ExitCode != 0 || !shouldRun)
        nextStage(lastProcess);
      else
        this.LaunchProcess(processName, processOptions, workingDirectory, true, nextStage);
    }

    private void LaunchProcessHelper(
      bool shouldRun,
      LauncherForm.ProcessFinishedDelegate nextStage,
      Process lastProcess,
      string processName,
      string processOptions)
    {
      this.LaunchProcessHelper(shouldRun, nextStage, lastProcess, processName, processOptions, (string) null);
    }

		private void LaunchProcess(
			string processFileName,
			string arguments,
			string workingDirectory,
			bool consoleAttached,
			LauncherForm.ProcessFinishedDelegate theProcessFinishedDelegate)
		{
			if (this.consoleProcess != null)
			{
				if (consoleAttached)
				{
					this.LauncherConsole.Invoke((Action)(() =>
					{
						string text;
						if ((object)processFileName != this.processTable[(object)this.consoleProcess])
							text = "Cannot start console process " + processFileName + "!\n\nAnother console process (" + this.processTable[(object)this.consoleProcess] + ") is already running";
						else
							text = "Console process (" + processFileName + ") is already running!";
						MessageBox.Show(text, "Console Busy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}));
					return;
				}
			}

			try
			{
				Process p = new Process()
				{
					StartInfo =
			{
				FileName = Path.Combine(Launcher.GetStartupDirectory(), processFileName),
				CreateNoWindow = true,
				Arguments = arguments,
				UseShellExecute = false
			}
				};

				p.StartInfo.WorkingDirectory = workingDirectory != null ? workingDirectory : Path.GetDirectoryName(p.StartInfo.FileName);
				p.EnableRaisingEvents = true;

				p.Exited += (sender, e) =>
				{
					this.processTable.Remove((object)p);
					this.UpdateProcessList();
				};

				if (consoleAttached)
				{
					this.processFinishedDelegate = theProcessFinishedDelegate;
					p.StartInfo.RedirectStandardError = true;
					p.StartInfo.RedirectStandardOutput = true;
					p.OutputDataReceived += (s, e) => this.WriteConsole(e.Data, false);
					p.ErrorDataReceived += (s, e) => this.WriteConsole(e.Data, true);

					p.Exited += (sender, e) =>
					{
						this.LauncherButtonCancel.Invoke((Action)(() =>
						{
							this.LauncherProcessTimeElapsedTextBox.Text = p.ExitCode != 0 ? "Error " + p.ExitCode.ToString() : "Success";
							this.LauncherConsole.Focus();
							this.consoleProcess = null;
							this.UpdateConsoleColor();

							if (this.processFinishedDelegate != null)
							{
								var finishedDelegate = this.processFinishedDelegate;
								this.processFinishedDelegate = null;
								finishedDelegate(p);
							}
						}));
					};
				}

				p.Exited += (sender, e) => p.Dispose();

				p.Start();

				if (consoleAttached)
				{
					this.consoleProcess = p;
					this.consoleProcessStartTime = DateTime.Now;
					this.UpdateConsoleColor();
					this.LauncherProcessTextBox.Text = (workingDirectory != null ? workingDirectory + "> " : "") + processFileName + " " + arguments;
					p.BeginOutputReadLine();
					p.BeginErrorReadLine();
				}

				this.processTable.Add((object)p, (object)processFileName);
				this.UpdateProcessList();
			}
			catch (Exception ex)
			{
				this.WriteConsole("FAILED TO EXECUTE: " + processFileName + " " + arguments, true);

				if (this.processFinishedDelegate != null)
				{
					var finishedDelegate = this.processFinishedDelegate;
					this.processFinishedDelegate = null;
					finishedDelegate(null);
				}
			}
		}

		private void LauncherButtonCancel_Click(object sender, EventArgs e)
    {
      int selectedIndex = this.LauncherProcessList.SelectedIndex;
      if (selectedIndex < 0)
        return;
      ((Process) ((DictionaryEntry) this.processList[selectedIndex]).Key).Kill();
    }

    private void LauncherButtonRunConverter_Click(object sender, EventArgs eventArgs)
    {
      this.LaunchProcess("converter", "-nopause", (string) null, true, (LauncherForm.ProcessFinishedDelegate) null);
    }

    private void LauncherButtonRadiant_Click(object sender, EventArgs e)
    {
      this.LaunchProcess("CoDWaWRadiant", this.mapName != null ? Path.Combine(Launcher.GetMapSourceDirectory(), this.mapName + ".map") : "", (string) null, false, (LauncherForm.ProcessFinishedDelegate) null);
    }

    private void LauncherButtonEffectsEd_Click(object sender, EventArgs e)
    {
      this.LaunchProcess("EffectsEd3", "", (string) null, false, (LauncherForm.ProcessFinishedDelegate) null);
    }

    private void LauncherButtonAssetManager_Click(object sender, EventArgs e)
    {
      this.LaunchProcess("asset_manager", "", (string) null, false, (LauncherForm.ProcessFinishedDelegate) null);
    }

    private void LauncherButtonAssetViewer_Click(object sender, EventArgs e)
    {
      this.LaunchProcess("AssetViewer", "", (string) null, false, (LauncherForm.ProcessFinishedDelegate) null);
    }

    private void LauncherProcessList_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.UpdateStopProcessButton();
    }

    private bool IsMP() => Launcher.IsMP(this.mapName);

    private string GetSourceBsp() => Launcher.GetMapSourceDirectory() + this.mapName;

    private string GetDestinationBsp()
    {
      return Launcher.GetRawMapsDirectory() + (this.IsMP() ? "mp\\" : "") + this.mapName;
    }

    private void LauncherButtonCreateMap_Click(object sender, EventArgs e)
    {
      if (new CreateMapForm().ShowDialog() != DialogResult.OK)
        return;
      this.UpdateMapList();
      this.EnableMapList();
    }

    private void CompileLevel()
    {
      this.EnableControls(false);
      this.UpdateMapSettings();
      this.CompileLevelBspDelegate((Process) null);
    }

    private void CompileLevelHelper(
      string mapSettingsOption,
      LauncherForm.ProcessFinishedDelegate nextStage,
      Process lastProcess,
      string processName,
      string processOptions,
      string workingDirectory)
    {
      this.LaunchProcessHelper(Launcher.mapSettings.GetBoolean(mapSettingsOption), nextStage, lastProcess, processName, processOptions, workingDirectory);
    }

    private void CompileLevelHelper(
      string mapSettingsOption,
      LauncherForm.ProcessFinishedDelegate nextStage,
      Process lastProcess,
      string processName,
      string processOptions)
    {
      this.LaunchProcessHelper(Launcher.mapSettings.GetBoolean(mapSettingsOption), nextStage, lastProcess, processName, processOptions);
    }

    private void CompileLevelBspDelegate(Process lastProcess)
    {
      this.CompileLevelHelper("compile_bsp", new LauncherForm.ProcessFinishedDelegate(this.CompileLevelVisDelegate), lastProcess, "cod2map", "-platform pc -loadFrom \"" + this.GetSourceBsp() + ".map\"" + Launcher.GetBspOptions() + " \"" + this.GetDestinationBsp() + "\"");
    }

    private void CompileLevelVisDelegate(Process lastProcess)
    {
      this.CompileLevelHelper("compile_vis", new LauncherForm.ProcessFinishedDelegate(this.CompileLevelLightsDelegate), lastProcess, "cod2map", "-vis -platform pc \"" + this.GetDestinationBsp() + "\"");
    }

    private void CompileLevelLightsDelegate(Process lastProcess)
    {
      this.CompileLevelHelper("compile_lights", new LauncherForm.ProcessFinishedDelegate(this.CompileLevelCleanupDelegate), lastProcess, "cod2rad", "-platform pc " + Launcher.GetLightOptions() + " \"" + this.GetDestinationBsp() + "\"");
    }

    private void CompileLevelCleanupDelegate(Process lastProcess)
    {
      Launcher.CopyFileSmart(this.GetDestinationBsp() + ".lin", this.GetSourceBsp() + ".lin");
      string[] strArray = new string[5]
      {
        ".lin",
        ".map",
        ".d3dpoly",
        ".vclog",
        ".grid"
      };
      foreach (string str in strArray)
        Launcher.DeleteFile(this.GetDestinationBsp() + str, false);
      this.CompileLevelPathsDelegate(lastProcess);
    }

    private void CompileLevelPathsDelegate(Process lastProcess)
    {
      this.CompileLevelHelper("compile_paths", new LauncherForm.ProcessFinishedDelegate(this.CompileLevelReflectionsDelegate), lastProcess, Launcher.GetGameTool(this.IsMP()), "allowdupe +set developer 1 +set logfile 2 +set thereisacow 1337 +set com_introplayed 1 +set r_fullscreen 0 +set fs_usedevdir 1 +set g_connectpaths 2 +set useFastFile 0 +devmap " + this.mapName);
    }

    private void CompileLevelReflectionsDelegate(Process lastProcess)
    {
      this.CompileLevelHelper("compile_reflections", new LauncherForm.ProcessFinishedDelegate(this.CompileLevelBspInfoDelegate), lastProcess, Launcher.GetGameTool(this.IsMP()), "allowdupe +set developer 1 +set logfile 2 +set thereisacow 1337 +set com_introplayed 1 +set r_fullscreen 0 +set fs_usedevdir 1 +set ui_autoContinue 1 +set r_reflectionProbeGenerateExit 1 +set sys_smp_allowed 0 +set useFastFile 0 +set r_fullscreen 0 +set com_hunkMegs 512 +set r_reflectionProbeRegenerateAll 1 +set r_zFeather 1 +set r_smp_backend_allowed 1 +set r_reflectionProbeGenerate 1 +devmap " + this.mapName);
    }

    private void CompileLevelBspInfoDelegate(Process lastProcess)
    {
      this.CompileLevelHelper("compile_bspinfo", new LauncherForm.ProcessFinishedDelegate(this.CompileLevelFastFilesDelegate), lastProcess, "cod2map", "-info \"" + this.GetDestinationBsp() + "\"");
    }

    private void CompileLevelBuildFastFile(
      string name,
      Process lastProcess,
      LauncherForm.ProcessFinishedDelegate nextStage)
    {
      string str = Launcher.mapSettings.GetBoolean("compile_modenabled") ? "-moddir " + Launcher.mapSettings.GetString("compile_modname") + " " : "";
      this.CompileLevelHelper("compile_buildffs", nextStage, lastProcess, "linker_pc", "-nopause -language " + Launcher.GetLanguage() + " " + str + name + (File.Exists(Launcher.GetLoadZone(this.mapName)) ? " " + Launcher.GetLoadZone(this.mapName) : ""));
    }

    private void CompileLevelFastFilesDelegate(Process lastProcess)
    {
      if (this.CheckZoneSourceFiles())
      {
        if (this.IsMP())
          this.CompileLevelBuildFastFile(this.mapName, lastProcess, new LauncherForm.ProcessFinishedDelegate(this.CompileLevelFastFilesLocalizedDelegate));
        else
          this.CompileLevelBuildFastFile(this.mapName, lastProcess, new LauncherForm.ProcessFinishedDelegate(this.CompileLevelMoveFastFilesDelegate));
      }
      else
        this.CompileLevelRunGameDelegate(lastProcess);
    }

    private void CompileLevelFastFilesLocalizedDelegate(Process lastProcess)
    {
      this.CompileLevelBuildFastFile("localized_" + this.mapName, lastProcess, new LauncherForm.ProcessFinishedDelegate(this.CompileLevelMoveFastFilesDelegate));
    }

    private void CompileLevelMoveFastFilesDelegate(Process lastProcess)
    {
      string zoneDirectory = Launcher.GetZoneDirectory();
      string path1 = Launcher.mapSettings.GetBoolean("compile_modenabled") ? Launcher.GetModDirectory(Launcher.mapSettings.GetString("compile_modname")) : Path.Combine(Launcher.GetUsermapsDirectory(), this.mapName);
      string path2_1 = this.mapName + ".ff";
      string path2_2 = this.mapName + "_load.ff";
      Launcher.MoveFile(Path.Combine(zoneDirectory, path2_1), Path.Combine(path1, path2_1));
      Launcher.MoveFile(Path.Combine(zoneDirectory, "localized_" + path2_1), Path.Combine(path1, "localized_" + path2_1));
      Launcher.MoveFile(Path.Combine(zoneDirectory, path2_2), Path.Combine(path1, path2_2));
      Launcher.Publish();
      this.CompileLevelRunGameDelegate(lastProcess);
    }

    private void CompileLevelRunGameDelegate(Process lastProcess)
    {
      string str = Launcher.mapSettings.GetBoolean("compile_modenabled") ? "mods/" + Launcher.mapSettings.GetString("compile_modname") : "raw";
      this.CompileLevelHelper("compile_runafter", new LauncherForm.ProcessFinishedDelegate(this.CompileLevelFinished), lastProcess, Launcher.GetGameApplication(this.IsMP()), "+set useFastFile 1 +set fs_usedevdir 1 +set logfile 2 +set thereisacow 1337 +set com_introplayed 1 " + (this.IsMP() ? "+set sv_pure 0 +set g_gametype tdm " : "") + "+devmap " + this.mapName + " +set fs_game " + str + " ");
    }

    private void CompileLevelFinished(Process lastProcess) => this.EnableControls(true);

    private void TestProcessFinishedDelegate(Process p)
    {
      this.LaunchProcess("help.exe", "", (string) null, true, (LauncherForm.ProcessFinishedDelegate) null);
    }

    private void LauncherButtonTest_Click(object sender, EventArgs e)
    {
      this.LaunchProcess("cmd.exe", "/c dir c:\\", (string) null, true, new LauncherForm.ProcessFinishedDelegate(this.TestProcessFinishedDelegate));
    }

    private void LauncherRunGameCustomCommandLineTextBox_TextChanged(object sender, EventArgs e)
    {
    }

    private void LauncherRunGameCustomCommandLineTextBox_Validating(
      object sender,
      CancelEventArgs e)
    {
    }

    private void EnableControls(bool enabled) => this.EnableControls(enabled, (TabPage) null);

    private void EnableControls(bool enabled, TabPage onlyForTabPage)
    {
      TabPage[] tabPageArray = new TabPage[3]
      {
        this.LauncherTabCompileLevel,
        this.LauncherTabModBuilder,
        this.LauncherTabRunGame
      };
      foreach (TabPage tabPage in tabPageArray)
      {
        if (onlyForTabPage == null || onlyForTabPage == tabPage)
        {
          foreach (Control control in (ArrangedElementCollection) tabPage.Controls)
            control.Enabled = enabled;
        }
      }
      if (!enabled)
        return;
      this.LauncherModSpecificMapComboBox.Enabled = this.LauncherModSpecificMapCheckBox.Checked;
    }

    private void UpdateMapSettings()
    {
      if (this.mapName != null)
      {
        Launcher.mapSettings.SetBoolean("compile_bsp", this.LauncherCompileBSPCheckBox.Checked);
        Launcher.mapSettings.SetBoolean("compile_lights", this.LauncherCompileLightsCheckBox.Checked);
        Launcher.mapSettings.SetBoolean("compile_vis", this.LauncherCompileVisCheckBox.Checked);
        Launcher.mapSettings.SetBoolean("compile_paths", this.LauncherConnectPathsCheckBox.Checked);
        Launcher.mapSettings.SetBoolean("compile_reflections", this.LauncherCompileReflectionsCheckBox.Checked);
        Launcher.mapSettings.SetBoolean("compile_buildffs", this.LauncherBuildFastFilesCheckBox.Checked);
        Launcher.mapSettings.SetBoolean("compile_bspinfo", this.LauncherBspInfoCheckBox.Checked);
        Launcher.mapSettings.SetBoolean("compile_runafter", this.LauncherRunMapAfterCompileCheckBox.Checked);
        Launcher.mapSettings.SetBoolean("compile_useruntab", this.LauncherUseRunGameTypeOptionsCheckBox.Checked);
        Launcher.mapSettings.SetString("compile_runoptions", this.LauncherCustomRunOptionsTextBox.Text);
        Launcher.mapSettings.SetBoolean("compile_modenabled", this.LauncherModSpecificMapCheckBox.Checked);
        Launcher.mapSettings.SetString("compile_modname", this.LauncherModSpecificMapComboBox.Text);
        Launcher.mapSettings.SetBoolean("compile_collectdots", this.LauncherGridCollectDotsCheckBox.Checked);
        Launcher.SaveMapSettings(this.mapName, new Hashtable(Launcher.mapSettings.Get()));
        this.mapName = (string) null;
      }
      if (this.LauncherMapList.SelectedItem == null)
        return;
      this.mapName = this.LauncherMapList.SelectedItem.ToString();
			Launcher.mapSettings.Set(
		  ((Hashtable)Launcher.LoadMapSettings(this.mapName))
			  .Cast<DictionaryEntry>()
			  .ToDictionary(de => (string)de.Key, de => (string)de.Value)
	  );
			this.LauncherCompileBSPCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_bsp");
      this.LauncherCompileLightsCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_lights");
      this.LauncherCompileVisCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_vis");
      this.LauncherConnectPathsCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_paths");
      this.LauncherCompileReflectionsCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_reflections");
      this.LauncherBuildFastFilesCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_buildffs");
      this.LauncherBspInfoCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_bspinfo");
      this.LauncherRunMapAfterCompileCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_runafter");
      this.LauncherUseRunGameTypeOptionsCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_useruntab");
      this.LauncherCustomRunOptionsTextBox.Text = Launcher.mapSettings.GetString("compile_runoptions");
      this.LauncherModSpecificMapCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_modenabled");
      this.LauncherModSpecificMapComboBox.Text = Launcher.mapSettings.GetString("compile_modname");
      this.LauncherGridCollectDotsCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_collectdots");
    }

    private void LauncherCompileLightsButton_Click(object sender, EventArgs e)
    {
      int num = (int) new BspOptionsForm().ShowDialog();
    }

    private void LauncherCompileBSPButton_Click(object sender, EventArgs e)
    {
      int num = (int) new LightOptionsForm().ShowDialog();
    }

    private void LauncherCompileLevelButton_Click(object sender, EventArgs e)
    {
      this.CompileLevel();
    }

    private string GetGameOptions()
    {
      return "" + "+set fs_game " + (this.LauncherRunGameModComboBox.SelectedIndex > 0 ? "mods/" + this.LauncherRunGameModComboBox.Text : "raw") + " " + this.FormatDvars() + " " + this.LauncherRunGameCustomCommandLineTextBox.Text + " ";
    }

    private void LauncherTimer_Tick(object sender, EventArgs e)
    {
      if (this.consoleProcess != null)
        this.LauncherProcessTimeElapsedTextBox.Text = (DateTime.Now - this.consoleProcessStartTime).ToString().Substring(0, 8);
      string gameOptions = this.GetGameOptions();
      if (!(this.LauncherRunGameCommandLineTextBox.Text != gameOptions))
        return;
      this.LauncherRunGameCommandLineTextBox.Text = gameOptions;
    }

    private bool CheckZoneSourceFiles()
    {
      if (File.Exists(Launcher.GetZoneSourceFile(this.mapName)))
        return true;
      if (MessageBox.Show("There are no zone files for " + this.mapName + ". Would you like to create them?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
        return false;
      Launcher.CreateZoneSourceFiles(this.mapName);
      return true;
    }

    private void EnableMapList()
    {
      bool enabled = this.LauncherMapList.SelectedItem != null;
      this.LauncherCompileLevelButton.Enabled = enabled;
      this.EnableControls(enabled, this.LauncherTabCompileLevel);
      this.LauncherMapList.Enabled = true;
      this.LauncherCreateMapButton.Enabled = true;
    }

    private void LauncherMapList_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.UpdateMapSettings();
      this.EnableMapList();
    }

    private void LauncherMapList_DoubleClick(object sender, EventArgs e)
    {
      this.LauncherMapList.SelectedItem = (object) null;
    }

    private void ModBuildStart()
    {
      this.EnableControls(false);
      this.ModBuildSoundDelegate((Process) null);
    }

    private void ModBuildSoundDelegate(Process lastProcess)
    {
      this.LaunchProcessHelper(this.LauncherModBuildSoundsCheckBox.Checked, new LauncherForm.ProcessFinishedDelegate(this.ModBuildFastFileDelegate), lastProcess, "MODSound", "-pc -ignore_orphans " + (this.LauncherModVerboseCheckBox.Checked ? "-verbose " : ""));
    }

    private void ModBuildFastFileDelegate(Process lastProcess)
    {
      if (this.LauncherModBuildFastFilesCheckBox.Checked)
        Launcher.CopyFileSmart(Path.Combine(Launcher.GetModDirectory(this.modName), "mod.csv"), Path.Combine(Launcher.GetZoneSourceDirectory(), "mod.csv"));
      this.LaunchProcessHelper((this.LauncherModBuildFastFilesCheckBox.Checked ? 1 : 0) != 0, new LauncherForm.ProcessFinishedDelegate(this.ModBuildMoveModFastFileDelegate), lastProcess, "linker_pc", "-nopause -language " + Launcher.GetLanguage() + " -moddir " + this.modName + " mod");
    }

    private void ModBuildMoveModFastFileDelegate(Process lastProcess)
    {
      if (this.LauncherModBuildFastFilesCheckBox.Checked)
        Launcher.MoveFile(Path.Combine(Launcher.GetZoneDirectory(), "mod.ff"), Path.Combine(Launcher.GetModDirectory(this.modName), "mod.ff"));
      this.ModBuildIwdFileDelegate(lastProcess);
    }

    private void ModBuildIwdFileDelegate(Process lastProcess)
    {
      string fileName = Path.Combine(Launcher.GetModDirectory(this.modName), this.modName + ".iwd");
      if (this.LauncherModBuildIwdFileCheckBox.Checked)
        Launcher.DeleteFile(fileName, false);
      this.LaunchProcessHelper((this.LauncherModBuildIwdFileCheckBox.Checked ? 1 : 0) != 0, new LauncherForm.ProcessFinishedDelegate(this.ModBuildFinishedDelegate), lastProcess, "7za", "a \"" + fileName + "\" -tzip -r \"@" + Path.Combine(Launcher.GetModDirectory(this.modName), this.modName + ".files") + "\"", Launcher.GetModDirectory(this.modName));
    }

    private void ModBuildFinishedDelegate(Process lastProcess)
    {
      Launcher.Publish();
      this.EnableControls(true);
    }

    private void LauncherModBuildButton_Click(object sender, EventArgs e)
    {
      this.LauncherModComboBoxApplySettings();
      this.ModBuildStart();
    }

    private void AddFilesToTreeView(string Directory, TreeNodeCollection tree, bool firstTime)
    {
      TreeNode treeNode1 = (TreeNode) null;
      if (!firstTime)
      {
        treeNode1 = tree.Add(new DirectoryInfo(Directory).Name);
        tree = treeNode1.Nodes;
      }
      foreach (DirectoryInfo directory in new DirectoryInfo(Directory).GetDirectories())
        this.AddFilesToTreeView(Path.Combine(Directory, directory.Name), tree, false);
      foreach (FileInfo file in new DirectoryInfo(Directory).GetFiles())
      {
        if (file.Extension.ToLower() != ".ff" && file.Extension.ToLower() != ".iwd" && file.Extension.ToLower() != ".files")
        {
          TreeNode treeNode2 = tree.Add(file.Name);
          treeNode2.ForeColor = Color.Blue;
          treeNode2.Tag = (object) file;
        }
      }
      if (treeNode1 == null)
        return;
      if (treeNode1.Nodes.Count != 0)
        treeNode1.ExpandAll();
      else
        treeNode1.Remove();
    }

    private void LauncherModComboBoxApplySettings()
    {
      this.LauncherIwdFileTreeBeginUpdate();
      if (this.modName != null)
      {
        string textFile1 = Path.Combine(Launcher.GetModDirectory(this.modName), this.modName + ".files");
        string textFile2 = Path.Combine(Launcher.GetModDirectory(this.modName), "mod.csv");
        Launcher.SaveTextFile(textFile1, Launcher.HashTableToStringArray(this.TreeViewToHashTable(this.LauncherIwdFileTree.Nodes)));
        Launcher.SaveTextFile(textFile2, this.LauncherFastFileCsvTextBox.Lines);
      }
      if (this.LauncherModComboBox.SelectedItem != null)
      {
        this.modName = this.LauncherModComboBox.SelectedItem.ToString();
        string textFile3 = Path.Combine(Launcher.GetModDirectory(this.modName), this.modName + ".files");
        string textFile4 = Path.Combine(Launcher.GetModDirectory(this.modName), "mod.csv");
        this.LauncherIwdFileTree.Nodes.Clear();
        this.AddFilesToTreeView(Launcher.GetModDirectory(this.modName), this.LauncherIwdFileTree.Nodes, true);
        this.HashTableToTreeView(Launcher.StringArrayToHashTable(Launcher.LoadTextFile(textFile3)), this.LauncherIwdFileTree.Nodes);
        this.LauncherFastFileCsvTextBox.Lines = Launcher.LoadTextFile(textFile4);
      }
      this.LauncherIwdFileTreeEndUpdate();
    }

    private void LauncherModComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.LauncherModComboBoxApplySettings();
    }

    private void LauncherIwdFileTree_DoubleClick(object sender, EventArgs e)
    {
      if (this.LauncherIwdFileTree.SelectedNode == null)
        return;
      try
      {
        new Process()
        {
          StartInfo = {
            ErrorDialog = true,
            FileName = Path.Combine(Launcher.GetModDirectory(this.modName), this.LauncherIwdFileTree.SelectedNode.FullPath)
          }
        }.Start();
      }
      catch
      {
      }
    }

    private void TreeViewToHashTable(TreeNodeCollection tree, Hashtable ht)
    {
      if (tree == null)
        return;
      foreach (TreeNode treeNode in tree)
      {
        if (treeNode.Checked && treeNode.Tag != null)
          ht.Add((object) treeNode.FullPath, (object) null);
        else
          ht.Remove((object) treeNode.FullPath);
        this.TreeViewToHashTable(treeNode.Nodes, ht);
      }
    }

    private Hashtable TreeViewToHashTable(TreeNodeCollection tree)
    {
      Hashtable ht = new Hashtable();
      this.TreeViewToHashTable(tree, ht);
      return ht;
    }

    private void HashTableToTreeView(Hashtable ht, TreeNodeCollection tree)
    {
      if (tree == null)
        return;
      foreach (TreeNode node in tree)
      {
        if (ht.Contains((object) node.FullPath))
          this.RecursiveCheckNodesUp(node, node.Checked = true);
        this.HashTableToTreeView(ht, node.Nodes);
      }
    }

    private void RecursiveCheckNodesDown(TreeNodeCollection tree, bool checkedFlag)
    {
      if (tree == null)
        return;
      foreach (TreeNode treeNode in tree)
        this.RecursiveCheckNodesDown(treeNode.Nodes, treeNode.Checked = checkedFlag);
    }

    private void RecursiveCheckNodesUp(TreeNode node, bool checkedFlag)
    {
      if (node == null)
        return;
      this.RecursiveCheckNodesUp(node.Parent, node.Checked = checkedFlag);
    }

    private void LauncherIwdFileTreeBeginUpdate()
    {
      this.LauncherIwdFileTree.BeginUpdate();
      this.LauncherIwdFileTree.AfterCheck -= new TreeViewEventHandler(this.LauncherIwdFileTree_AfterCheck);
    }

    private void LauncherIwdFileTreeEndUpdate()
    {
      this.LauncherIwdFileTree.AfterCheck += new TreeViewEventHandler(this.LauncherIwdFileTree_AfterCheck);
      this.LauncherIwdFileTree.EndUpdate();
    }

    private void LauncherIwdFileTree_AfterCheck(object sender, TreeViewEventArgs e)
    {
      this.LauncherIwdFileTreeBeginUpdate();
      this.RecursiveCheckNodesDown(e.Node.Nodes, e.Node.Checked);
      if (e.Node.Checked)
        this.RecursiveCheckNodesUp(e.Node.Parent, e.Node.Checked);
      this.LauncherIwdFileTreeEndUpdate();
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
      this.LauncherModSpecificMapComboBox.Enabled = this.LauncherModSpecificMapCheckBox.Checked;
    }

    private void LauncherClearConsoleButton_Click(object sender, EventArgs e)
    {
      this.LauncherConsole.Clear();
    }

    private void LauncherGameOptionsFlowPanel_Click(object sender, EventArgs e)
    {
      MouseEventArgs mouseEventArgs = (MouseEventArgs) e;
      Control control = (Control) sender;
      if (mouseEventArgs.Button != MouseButtons.Right)
        return;
      ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
      contextMenuStrip.Items.Add("Edit dvar");
      contextMenuStrip.Items.Add("Remove dvar");
      contextMenuStrip.Items.Add("Add new dvar");
      contextMenuStrip.Items.Add("Duplicate dvar");
      contextMenuStrip.Show(control.PointToScreen(mouseEventArgs.Location));
    }

    private void LauncherWikiLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      Process.Start("http://wiki.treyarch.com");
    }

    private void LauncherRunGameButton_Click(object sender, EventArgs e)
    {
      foreach (ComboBox dvarComboBox in this.dvarComboBoxes)
      {
        string str1 = dvarComboBox.Text.Trim();
        if (str1 != "")
        {
          foreach (string str2 in dvarComboBox.Items)
          {
            if (str1.ToLower() == str2.ToLower())
            {
              str1 = "";
              break;
            }
          }
        }
        if (str1 != "")
          dvarComboBox.Items.Add((object) dvarComboBox.Text);
      }
      this.LaunchProcess(Launcher.GetGameApplication(!this.LauncherRunGameTypeRadioButton.Checked), this.GetGameOptions(), (string) null, false, (LauncherForm.ProcessFinishedDelegate) null);
    }

    private void LauncherDeleteMap_Click(object sender, EventArgs e)
    {
      string[] mapFiles1 = Launcher.GetMapFiles(this.mapName);
      if (DialogResult.Yes != MessageBox.Show("The following files would be deleted:\n\n" + Launcher.StringArrayToString(mapFiles1), "Are you sure you want to delete these files?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
        return;
      foreach (string fileName in mapFiles1)
        Launcher.DeleteFile(fileName);
      string[] mapFiles2 = Launcher.GetMapFiles(this.mapName);
      if (mapFiles2.Length > 0)
      {
        int num = (int) MessageBox.Show("Could not delete the following files:\n\n" + Launcher.StringArrayToString(mapFiles2), "Error deleting files", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
      this.UpdateMapList();
      this.EnableMapList();
    }

    private void LauncherAboutLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      int num = (int) MessageBox.Show("Original launcher by\n     Mike Denny\n\nPC Programming Lead\n     Krassimir Touevsky\n\nPC Programming Team\n     Yanbing Chen\n     Juan Morelli\n     Ewan Oughton\n     Valeria Pelova\n     Dimiter \"malkia\" Stanev\n\nPC Production Team\n     Adam Saslow\n     Cesar Stastny\n\nPC Modding Team\n     Tony Kramer\n     Gavin Niebel\n     Alex 'Sparks' Romo\n\nDecompiled by\n     hindercanrun", "About Launcher");
    }

    private void LauncherForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (this.processTable.Count != 0)
      {
        switch (MessageBox.Show("But there are still processes running!\n\nDo you want to close them, or cancel exiting from the application?", "Application will exit!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation))
        {
          case DialogResult.Cancel:
            e.Cancel = true;
            return;
          case DialogResult.Yes:
            IDictionaryEnumerator enumerator = this.processTable.GetEnumerator();
            try
            {
              while (enumerator.MoveNext())
                ((Process) ((DictionaryEntry) enumerator.Current).Key).Kill();
              break;
            }
            finally
            {
              if (enumerator is IDisposable disposable)
                disposable.Dispose();
            }
          default:
            string[] stringArray = new string[this.processTable.Count];
            int index = 0;
            foreach (DictionaryEntry dictionaryEntry in this.processTable)
            {
              try
              {
                stringArray[index] = ((Process) dictionaryEntry.Key).MainModule.FileName;
              }
              catch
              {
                stringArray[index] = (string) dictionaryEntry.Value;
              }
              ++index;
            }
            if (stringArray.Length > 0)
            {
              int num = (int) MessageBox.Show("The following processes are still active:\n\n" + Launcher.StringArrayToString(stringArray) + "\nPlease close them if neccessary using the Task Manager, or similar program!\n", "Note before exiting the application", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
              break;
            }
            break;
        }
      }
      this.UpdateMapSettings();
    }

    private void LauncherMapFilesSystemWatcher_Changed(object sender, FileSystemEventArgs e)
    {
      this.UpdateMapList();
    }

    private void LauncherMapFilesSystemWatcher_Created(object sender, FileSystemEventArgs e)
    {
      this.UpdateMapList();
    }

    private void LauncherMapFilesSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
    {
      this.UpdateMapList();
    }

    private void LauncherMapFilesSystemWatcher_Renamed(object sender, RenamedEventArgs e)
    {
      this.UpdateMapList();
    }

    private void LauncherModsDirectorySystemWatcher_Changed(object sender, FileSystemEventArgs e)
    {
      this.UpdateModList();
    }

    private void LauncherModsDirectorySystemWatcher_Created(object sender, FileSystemEventArgs e)
    {
      this.UpdateModList();
    }

    private void LauncherModsDirectorySystemWatcher_Deleted(object sender, FileSystemEventArgs e)
    {
      this.UpdateModList();
    }

    private void LauncherModsDirectorySystemWatcher_Renamed(object sender, RenamedEventArgs e)
    {
      this.UpdateModList();
    }

    private void BuildGridDelegate(int r_vc_makelog)
    {
      this.EnableControls(false);
      string path2 = this.mapName + ".grid";
      Launcher.CopyFile(Path.Combine(Launcher.GetMapSourceDirectory(), path2), Path.Combine(Launcher.GetRawMapsDirectory(), Path.Combine(this.IsMP() ? "mp" : "", path2)));
      this.LaunchProcessHelper(true, new LauncherForm.ProcessFinishedDelegate(this.BuildGridFinishedDelegate), (Process) null, Launcher.GetGameApplication(this.IsMP()), "+set developer 1 +set logfile 2 + set r_vc_makelog " + r_vc_makelog.ToString() + "+set r_vc_showlog 16 +set r_cullxmodel " + (Launcher.mapSettings.GetBoolean("compile_collectdots") ? "0" : "1") + " +set thereisacow 1337 +set com_introplayed 1 +set fs_game raw +set fs_usedevdir 1 +devmap " + this.mapName);
    }

    private void BuildGridFinishedDelegate(Process lastProcess)
    {
      string path2 = this.mapName + ".grid";
      Launcher.MoveFile(Path.Combine(Launcher.GetRawMapsDirectory(), Path.Combine(this.IsMP() ? "mp" : "", path2)), Path.Combine(Launcher.GetMapSourceDirectory(), path2));
      this.EnableControls(true);
    }

    private void LauncherGridMakeNewButton_Click(object sender, EventArgs e)
    {
      this.BuildGridDelegate(1);
    }

    private void LauncherGridEditExistingButton_Click(object sender, EventArgs e)
    {
      this.BuildGridDelegate(2);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (LauncherForm));
      this.LauncherConsole = new RichTextBox();
      this.LauncherSplitter = new SplitContainer();
      this.LauncherApplicationsGroupBox = new GroupBox();
      this.LauncherClearConsoleButton = new Button();
      this.LauncherButtonTest = new Button();
      this.LauncherButtonAssetViewer = new Button();
      this.LauncherButtonRunConverter = new Button();
      this.LauncherButtonAssetManager = new Button();
      this.LauncherButtonEffectsEd = new Button();
      this.LauncherButtonRadiant = new Button();
      this.LauncherTab = new TabControl();
      this.LauncherTabCompileLevel = new TabPage();
      this.LauncherDeleteMapButton = new Button();
      this.LauncherCompileLevelOptionsGroupBox = new GroupBox();
      this.LauncherGridFileGroupBox = new GroupBox();
      this.LauncherGridEditExistingButton = new Button();
      this.LauncherGridMakeNewButton = new Button();
      this.LauncherGridCollectDotsCheckBox = new CheckBox();
      this.LauncherModSpecificMapComboBox = new ComboBox();
      this.LauncherModSpecificMapCheckBox = new CheckBox();
      this.LauncherCustomRunOptionsLabel = new Label();
      this.LauncherCustomRunOptionsTextBox = new TextBox();
      this.LauncherCompileLevelOptionsSplitterGroupBox = new GroupBox();
      this.LauncherCompileLevelButton = new Button();
      this.LauncherCompileLightsButton = new Button();
      this.LauncherCompileBSPButton = new Button();
      this.LauncherUseRunGameTypeOptionsCheckBox = new CheckBox();
      this.LauncherRunMapAfterCompileCheckBox = new CheckBox();
      this.LauncherBspInfoCheckBox = new CheckBox();
      this.LauncherBuildFastFilesCheckBox = new CheckBox();
      this.LauncherCompileReflectionsCheckBox = new CheckBox();
      this.LauncherConnectPathsCheckBox = new CheckBox();
      this.LauncherCompileVisCheckBox = new CheckBox();
      this.LauncherCompileLightsCheckBox = new CheckBox();
      this.LauncherCompileBSPCheckBox = new CheckBox();
      this.LauncherCreateMapButton = new Button();
      this.LauncherMapList = new ListBox();
      this.LauncherTabModBuilder = new TabPage();
      this.LauncherIwdFileGroupBox = new GroupBox();
      this.LauncherIwdFileTree = new TreeView();
      this.LauncherFastFileCsvGroupBox = new GroupBox();
      this.LauncherFastFileCsvTextBox = new RichTextBox();
      this.LauncherModGroupBox = new GroupBox();
      this.LauncherModBuildButton = new Button();
      this.LauncherModBuildSoundsCheckBox = new CheckBox();
      this.LauncherModVerboseCheckBox = new CheckBox();
      this.LauncherModBuildIwdFileCheckBox = new CheckBox();
      this.LauncherModBuildFastFilesCheckBox = new CheckBox();
      this.LauncherModComboBox = new ComboBox();
      this.LauncherTabRunGame = new TabPage();
      this.LauncherGameOptionsPanel = new Panel();
      this.LauncherRunGameButton = new Button();
      this.LauncherRunGameCustomCommandLineGroupBox = new GroupBox();
      this.LauncherRunGameCustomCommandLineTextBox = new TextBox();
      this.LauncherRunGameCommandLineGroupBox = new GroupBox();
      this.LauncherRunGameCommandLineTextBox = new TextBox();
      this.LauncherRunGameModGroupBox = new GroupBox();
      this.LauncherRunGameModComboBox = new ComboBox();
      this.LauncherRunGameExeTypeGroupBox = new GroupBox();
      this.LauncherRunGameExeTypeMpRadioButton = new RadioButton();
      this.LauncherRunGameTypeRadioButton = new RadioButton();
      this.LauncherProcessTimeElapsedTextBox = new TextBox();
      this.LauncherProcessTextBox = new TextBox();
      this.LauncherProcessGroupBox = new GroupBox();
      this.LauncherButtonCancel = new Button();
      this.LauncherProcessList = new ListBox();
      this.LauncherTimer = new System.Windows.Forms.Timer(this.components);
      this.LauncherWikiLabel = new LinkLabel();
      this.LauncherAboutLabel = new LinkLabel();
      this.LauncherMapFilesSystemWatcher = new FileSystemWatcher();
      this.LauncherModsDirectorySystemWatcher = new FileSystemWatcher();
      this.LauncherSplitter.Panel1.SuspendLayout();
      this.LauncherSplitter.Panel2.SuspendLayout();
      this.LauncherSplitter.SuspendLayout();
      this.LauncherApplicationsGroupBox.SuspendLayout();
      this.LauncherTab.SuspendLayout();
      this.LauncherTabCompileLevel.SuspendLayout();
      this.LauncherCompileLevelOptionsGroupBox.SuspendLayout();
      this.LauncherGridFileGroupBox.SuspendLayout();
      this.LauncherTabModBuilder.SuspendLayout();
      this.LauncherIwdFileGroupBox.SuspendLayout();
      this.LauncherFastFileCsvGroupBox.SuspendLayout();
      this.LauncherModGroupBox.SuspendLayout();
      this.LauncherTabRunGame.SuspendLayout();
      this.LauncherRunGameCustomCommandLineGroupBox.SuspendLayout();
      this.LauncherRunGameCommandLineGroupBox.SuspendLayout();
      this.LauncherRunGameModGroupBox.SuspendLayout();
      this.LauncherRunGameExeTypeGroupBox.SuspendLayout();
      this.LauncherProcessGroupBox.SuspendLayout();
      this.LauncherMapFilesSystemWatcher.BeginInit();
      this.LauncherModsDirectorySystemWatcher.BeginInit();
      this.SuspendLayout();
      this.LauncherConsole.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherConsole.Font = new Font("Courier New", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.LauncherConsole.Location = new Point(149, 3);
      this.LauncherConsole.Name = "LauncherConsole";
      this.LauncherConsole.Size = new Size(662, 229);
      this.LauncherConsole.TabIndex = 0;
      this.LauncherConsole.Text = "";
      this.LauncherConsole.WordWrap = false;
      this.LauncherSplitter.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherSplitter.BackColor = SystemColors.Control;
      this.LauncherSplitter.FixedPanel = FixedPanel.Panel1;
      this.LauncherSplitter.Location = new Point(12, 12);
      this.LauncherSplitter.Name = "LauncherSplitter";
      this.LauncherSplitter.Orientation = Orientation.Horizontal;
      this.LauncherSplitter.Panel1.Controls.Add((Control) this.LauncherApplicationsGroupBox);
      this.LauncherSplitter.Panel1.Controls.Add((Control) this.LauncherTab);
      this.LauncherSplitter.Panel1MinSize = 380;
      this.LauncherSplitter.Panel2.Controls.Add((Control) this.LauncherProcessTimeElapsedTextBox);
      this.LauncherSplitter.Panel2.Controls.Add((Control) this.LauncherProcessTextBox);
      this.LauncherSplitter.Panel2.Controls.Add((Control) this.LauncherConsole);
      this.LauncherSplitter.Panel2.Controls.Add((Control) this.LauncherProcessGroupBox);
      this.LauncherSplitter.Panel2MinSize = 100;
      this.LauncherSplitter.Size = new Size(814, 640);
      this.LauncherSplitter.SplitterDistance = 380;
      this.LauncherSplitter.TabIndex = 1;
      this.LauncherApplicationsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
      this.LauncherApplicationsGroupBox.Controls.Add((Control) this.LauncherClearConsoleButton);
      this.LauncherApplicationsGroupBox.Controls.Add((Control) this.LauncherButtonTest);
      this.LauncherApplicationsGroupBox.Controls.Add((Control) this.LauncherButtonAssetViewer);
      this.LauncherApplicationsGroupBox.Controls.Add((Control) this.LauncherButtonRunConverter);
      this.LauncherApplicationsGroupBox.Controls.Add((Control) this.LauncherButtonAssetManager);
      this.LauncherApplicationsGroupBox.Controls.Add((Control) this.LauncherButtonEffectsEd);
      this.LauncherApplicationsGroupBox.Controls.Add((Control) this.LauncherButtonRadiant);
      this.LauncherApplicationsGroupBox.Location = new Point(4, 4);
      this.LauncherApplicationsGroupBox.Name = "LauncherApplicationsGroupBox";
      this.LauncherApplicationsGroupBox.Size = new Size(139, 373);
      this.LauncherApplicationsGroupBox.TabIndex = 1;
      this.LauncherApplicationsGroupBox.TabStop = false;
      this.LauncherApplicationsGroupBox.Text = "Applications";
      this.LauncherClearConsoleButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherClearConsoleButton.Location = new Point(6, 335);
      this.LauncherClearConsoleButton.Name = "LauncherClearConsoleButton";
      this.LauncherClearConsoleButton.Size = new Size((int) sbyte.MaxValue, 32);
      this.LauncherClearConsoleButton.TabIndex = 5;
      this.LauncherClearConsoleButton.Text = "Clear Console";
      this.LauncherClearConsoleButton.UseVisualStyleBackColor = true;
      this.LauncherClearConsoleButton.Click += new EventHandler(this.LauncherClearConsoleButton_Click);
      this.LauncherButtonTest.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherButtonTest.Location = new Point(5, 297);
      this.LauncherButtonTest.Name = "LauncherButtonTest";
      this.LauncherButtonTest.Size = new Size(128, 32);
      this.LauncherButtonTest.TabIndex = 6;
      this.LauncherButtonTest.Text = "Test (devonly)";
      this.LauncherButtonTest.UseVisualStyleBackColor = true;
      this.LauncherButtonTest.Visible = false;
      this.LauncherButtonTest.Click += new EventHandler(this.LauncherButtonTest_Click);
      this.LauncherButtonAssetViewer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherButtonAssetViewer.Location = new Point(5, 133);
      this.LauncherButtonAssetViewer.Name = "LauncherButtonAssetViewer";
      this.LauncherButtonAssetViewer.Size = new Size(128, 32);
      this.LauncherButtonAssetViewer.TabIndex = 5;
      this.LauncherButtonAssetViewer.Text = "Asset Viewer";
      this.LauncherButtonAssetViewer.UseVisualStyleBackColor = true;
      this.LauncherButtonAssetViewer.Click += new EventHandler(this.LauncherButtonAssetViewer_Click);
      this.LauncherButtonRunConverter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherButtonRunConverter.Location = new Point(5, 171);
      this.LauncherButtonRunConverter.Name = "LauncherButtonRunConverter";
      this.LauncherButtonRunConverter.Size = new Size(128, 32);
      this.LauncherButtonRunConverter.TabIndex = 3;
      this.LauncherButtonRunConverter.Text = "Converter";
      this.LauncherButtonRunConverter.UseVisualStyleBackColor = true;
      this.LauncherButtonRunConverter.Click += new EventHandler(this.LauncherButtonRunConverter_Click);
      this.LauncherButtonAssetManager.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherButtonAssetManager.Location = new Point(6, 95);
      this.LauncherButtonAssetManager.Name = "LauncherButtonAssetManager";
      this.LauncherButtonAssetManager.Size = new Size(128, 32);
      this.LauncherButtonAssetManager.TabIndex = 2;
      this.LauncherButtonAssetManager.Text = "Asset Manager";
      this.LauncherButtonAssetManager.UseVisualStyleBackColor = true;
      this.LauncherButtonAssetManager.Click += new EventHandler(this.LauncherButtonAssetManager_Click);
      this.LauncherButtonEffectsEd.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherButtonEffectsEd.Location = new Point(6, 57);
      this.LauncherButtonEffectsEd.Name = "LauncherButtonEffectsEd";
      this.LauncherButtonEffectsEd.Size = new Size(128, 32);
      this.LauncherButtonEffectsEd.TabIndex = 1;
      this.LauncherButtonEffectsEd.Text = "Effects Editor";
      this.LauncherButtonEffectsEd.UseVisualStyleBackColor = true;
      this.LauncherButtonEffectsEd.Click += new EventHandler(this.LauncherButtonEffectsEd_Click);
      this.LauncherButtonRadiant.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherButtonRadiant.Location = new Point(6, 19);
      this.LauncherButtonRadiant.Name = "LauncherButtonRadiant";
      this.LauncherButtonRadiant.Size = new Size(128, 32);
      this.LauncherButtonRadiant.TabIndex = 0;
      this.LauncherButtonRadiant.Text = "Radiant";
      this.LauncherButtonRadiant.UseVisualStyleBackColor = true;
      this.LauncherButtonRadiant.Click += new EventHandler(this.LauncherButtonRadiant_Click);
      this.LauncherTab.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherTab.Controls.Add((Control) this.LauncherTabCompileLevel);
      this.LauncherTab.Controls.Add((Control) this.LauncherTabModBuilder);
      this.LauncherTab.Controls.Add((Control) this.LauncherTabRunGame);
      this.LauncherTab.Location = new Point(149, 1);
      this.LauncherTab.Name = "LauncherTab";
      this.LauncherTab.SelectedIndex = 0;
      this.LauncherTab.Size = new Size(664, 380);
      this.LauncherTab.TabIndex = 0;
      this.LauncherTabCompileLevel.Controls.Add((Control) this.LauncherDeleteMapButton);
      this.LauncherTabCompileLevel.Controls.Add((Control) this.LauncherCompileLevelOptionsGroupBox);
      this.LauncherTabCompileLevel.Controls.Add((Control) this.LauncherCreateMapButton);
      this.LauncherTabCompileLevel.Controls.Add((Control) this.LauncherMapList);
      this.LauncherTabCompileLevel.Location = new Point(4, 22);
      this.LauncherTabCompileLevel.Name = "LauncherTabCompileLevel";
      this.LauncherTabCompileLevel.Padding = new Padding(3);
      this.LauncherTabCompileLevel.Size = new Size(656, 354);
      this.LauncherTabCompileLevel.TabIndex = 0;
      this.LauncherTabCompileLevel.Text = "Compile Level";
      this.LauncherTabCompileLevel.UseVisualStyleBackColor = true;
      this.LauncherDeleteMapButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.LauncherDeleteMapButton.Location = new Point(6, 316);
      this.LauncherDeleteMapButton.Name = "LauncherDeleteMapButton";
      this.LauncherDeleteMapButton.Size = new Size(72, 32);
      this.LauncherDeleteMapButton.TabIndex = 4;
      this.LauncherDeleteMapButton.Text = "Delete Map";
      this.LauncherDeleteMapButton.UseVisualStyleBackColor = true;
      this.LauncherDeleteMapButton.Click += new EventHandler(this.LauncherDeleteMap_Click);
      this.LauncherCompileLevelOptionsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) this.LauncherGridFileGroupBox);
      this.LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) this.LauncherModSpecificMapComboBox);
      this.LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) this.LauncherModSpecificMapCheckBox);
      this.LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) this.LauncherCustomRunOptionsLabel);
      this.LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) this.LauncherCustomRunOptionsTextBox);
      this.LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) this.LauncherCompileLevelOptionsSplitterGroupBox);
      this.LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) this.LauncherCompileLevelButton);
      this.LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) this.LauncherCompileLightsButton);
      this.LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) this.LauncherCompileBSPButton);
      this.LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) this.LauncherUseRunGameTypeOptionsCheckBox);
      this.LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) this.LauncherRunMapAfterCompileCheckBox);
      this.LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) this.LauncherBspInfoCheckBox);
      this.LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) this.LauncherBuildFastFilesCheckBox);
      this.LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) this.LauncherCompileReflectionsCheckBox);
      this.LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) this.LauncherConnectPathsCheckBox);
      this.LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) this.LauncherCompileVisCheckBox);
      this.LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) this.LauncherCompileLightsCheckBox);
      this.LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) this.LauncherCompileBSPCheckBox);
      this.LauncherCompileLevelOptionsGroupBox.Location = new Point(162, 6);
      this.LauncherCompileLevelOptionsGroupBox.MinimumSize = new Size(364, 332);
      this.LauncherCompileLevelOptionsGroupBox.Name = "LauncherCompileLevelOptionsGroupBox";
      this.LauncherCompileLevelOptionsGroupBox.Size = new Size(478, 342);
      this.LauncherCompileLevelOptionsGroupBox.TabIndex = 3;
      this.LauncherCompileLevelOptionsGroupBox.TabStop = false;
      this.LauncherCompileLevelOptionsGroupBox.Text = "Compile Level Options";
      this.LauncherGridFileGroupBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.LauncherGridFileGroupBox.Controls.Add((Control) this.LauncherGridEditExistingButton);
      this.LauncherGridFileGroupBox.Controls.Add((Control) this.LauncherGridMakeNewButton);
      this.LauncherGridFileGroupBox.Controls.Add((Control) this.LauncherGridCollectDotsCheckBox);
      this.LauncherGridFileGroupBox.Location = new Point(6, 260);
      this.LauncherGridFileGroupBox.Name = "LauncherGridFileGroupBox";
      this.LauncherGridFileGroupBox.Size = new Size(223, 76);
      this.LauncherGridFileGroupBox.TabIndex = 18;
      this.LauncherGridFileGroupBox.TabStop = false;
      this.LauncherGridFileGroupBox.Text = "Grid File";
      this.LauncherGridEditExistingButton.Location = new Point(112, 42);
      this.LauncherGridEditExistingButton.Name = "LauncherGridEditExistingButton";
      this.LauncherGridEditExistingButton.Size = new Size(100, 23);
      this.LauncherGridEditExistingButton.TabIndex = 19;
      this.LauncherGridEditExistingButton.Text = "Edit Existing Grid";
      this.LauncherGridEditExistingButton.UseVisualStyleBackColor = true;
      this.LauncherGridEditExistingButton.Click += new EventHandler(this.LauncherGridEditExistingButton_Click);
      this.LauncherGridMakeNewButton.Location = new Point(6, 42);
      this.LauncherGridMakeNewButton.Name = "LauncherGridMakeNewButton";
      this.LauncherGridMakeNewButton.Size = new Size(100, 23);
      this.LauncherGridMakeNewButton.TabIndex = 18;
      this.LauncherGridMakeNewButton.Text = "Make New Grid";
      this.LauncherGridMakeNewButton.UseVisualStyleBackColor = true;
      this.LauncherGridMakeNewButton.Click += new EventHandler(this.LauncherGridMakeNewButton_Click);
      this.LauncherGridCollectDotsCheckBox.AutoSize = true;
      this.LauncherGridCollectDotsCheckBox.Location = new Point(6, 19);
      this.LauncherGridCollectDotsCheckBox.Name = "LauncherGridCollectDotsCheckBox";
      this.LauncherGridCollectDotsCheckBox.Size = new Size(120, 17);
      this.LauncherGridCollectDotsCheckBox.TabIndex = 17;
      this.LauncherGridCollectDotsCheckBox.Text = "Models Collect Dots";
      this.LauncherGridCollectDotsCheckBox.UseVisualStyleBackColor = true;
      this.LauncherModSpecificMapComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherModSpecificMapComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      this.LauncherModSpecificMapComboBox.Enabled = false;
      this.LauncherModSpecificMapComboBox.FormattingEnabled = true;
      this.LauncherModSpecificMapComboBox.Items.AddRange(new object[3]
      {
        (object) "HumorOneMod",
        (object) "HumorTwoMod",
        (object) "BlahBlahMod"
      });
      this.LauncherModSpecificMapComboBox.Location = new Point(149, 41);
      this.LauncherModSpecificMapComboBox.Name = "LauncherModSpecificMapComboBox";
      this.LauncherModSpecificMapComboBox.Size = new Size(323, 21);
      this.LauncherModSpecificMapComboBox.TabIndex = 4;
      this.LauncherModSpecificMapCheckBox.AutoSize = true;
      this.LauncherModSpecificMapCheckBox.Location = new Point(149, 16);
      this.LauncherModSpecificMapCheckBox.Name = "LauncherModSpecificMapCheckBox";
      this.LauncherModSpecificMapCheckBox.Size = new Size(112, 17);
      this.LauncherModSpecificMapCheckBox.TabIndex = 5;
      this.LauncherModSpecificMapCheckBox.Text = "Mod Specific Map";
      this.LauncherModSpecificMapCheckBox.UseVisualStyleBackColor = true;
      this.LauncherModSpecificMapCheckBox.CheckedChanged += new EventHandler(this.checkBox1_CheckedChanged);
      this.LauncherCustomRunOptionsLabel.AutoSize = true;
      this.LauncherCustomRunOptionsLabel.Location = new Point(6, 237);
      this.LauncherCustomRunOptionsLabel.Name = "LauncherCustomRunOptionsLabel";
      this.LauncherCustomRunOptionsLabel.Size = new Size(107, 13);
      this.LauncherCustomRunOptionsLabel.TabIndex = 14;
      this.LauncherCustomRunOptionsLabel.Text = "Custom Run Options:";
      this.LauncherCustomRunOptionsLabel.Visible = false;
      this.LauncherCustomRunOptionsTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherCustomRunOptionsTextBox.Location = new Point(119, 234);
      this.LauncherCustomRunOptionsTextBox.Name = "LauncherCustomRunOptionsTextBox";
      this.LauncherCustomRunOptionsTextBox.Size = new Size(353, 20);
      this.LauncherCustomRunOptionsTextBox.TabIndex = 13;
      this.LauncherCustomRunOptionsTextBox.Visible = false;
      this.LauncherCompileLevelOptionsSplitterGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherCompileLevelOptionsSplitterGroupBox.Location = new Point(2, 134);
      this.LauncherCompileLevelOptionsSplitterGroupBox.Name = "LauncherCompileLevelOptionsSplitterGroupBox";
      this.LauncherCompileLevelOptionsSplitterGroupBox.Size = new Size(476, 2);
      this.LauncherCompileLevelOptionsSplitterGroupBox.TabIndex = 12;
      this.LauncherCompileLevelOptionsSplitterGroupBox.TabStop = false;
      this.LauncherCompileLevelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.LauncherCompileLevelButton.Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.LauncherCompileLevelButton.Location = new Point(344, 260);
      this.LauncherCompileLevelButton.Name = "LauncherCompileLevelButton";
      this.LauncherCompileLevelButton.Size = new Size(128, 76);
      this.LauncherCompileLevelButton.TabIndex = 4;
      this.LauncherCompileLevelButton.Text = "Compile Level";
      this.LauncherCompileLevelButton.UseVisualStyleBackColor = true;
      this.LauncherCompileLevelButton.Click += new EventHandler(this.LauncherCompileLevelButton_Click);
      this.LauncherCompileLightsButton.Location = new Point(107, 16);
      this.LauncherCompileLightsButton.Name = "LauncherCompileLightsButton";
      this.LauncherCompileLightsButton.Size = new Size(26, 23);
      this.LauncherCompileLightsButton.TabIndex = 11;
      this.LauncherCompileLightsButton.Text = "...";
      this.LauncherCompileLightsButton.UseVisualStyleBackColor = true;
      this.LauncherCompileLightsButton.Click += new EventHandler(this.LauncherCompileLightsButton_Click);
      this.LauncherCompileBSPButton.Location = new Point(107, 39);
      this.LauncherCompileBSPButton.Name = "LauncherCompileBSPButton";
      this.LauncherCompileBSPButton.Size = new Size(26, 23);
      this.LauncherCompileBSPButton.TabIndex = 10;
      this.LauncherCompileBSPButton.Text = "...";
      this.LauncherCompileBSPButton.UseVisualStyleBackColor = true;
      this.LauncherCompileBSPButton.Click += new EventHandler(this.LauncherCompileBSPButton_Click);
      this.LauncherUseRunGameTypeOptionsCheckBox.AutoSize = true;
      this.LauncherUseRunGameTypeOptionsCheckBox.FlatStyle = FlatStyle.Popup;
      this.LauncherUseRunGameTypeOptionsCheckBox.Location = new Point(9, 211);
      this.LauncherUseRunGameTypeOptionsCheckBox.Name = "LauncherUseRunGameTypeOptionsCheckBox";
      this.LauncherUseRunGameTypeOptionsCheckBox.Size = new Size(162, 17);
      this.LauncherUseRunGameTypeOptionsCheckBox.TabIndex = 9;
      this.LauncherUseRunGameTypeOptionsCheckBox.Text = "Use 'Run Game Tab' Options";
      this.LauncherUseRunGameTypeOptionsCheckBox.UseVisualStyleBackColor = true;
      this.LauncherUseRunGameTypeOptionsCheckBox.Visible = false;
      this.LauncherRunMapAfterCompileCheckBox.AutoSize = true;
      this.LauncherRunMapAfterCompileCheckBox.FlatStyle = FlatStyle.Popup;
      this.LauncherRunMapAfterCompileCheckBox.Location = new Point(9, 188);
      this.LauncherRunMapAfterCompileCheckBox.Name = "LauncherRunMapAfterCompileCheckBox";
      this.LauncherRunMapAfterCompileCheckBox.Size = new Size(133, 17);
      this.LauncherRunMapAfterCompileCheckBox.TabIndex = 8;
      this.LauncherRunMapAfterCompileCheckBox.Text = "Run Map After Compile";
      this.LauncherRunMapAfterCompileCheckBox.UseVisualStyleBackColor = true;
      this.LauncherBspInfoCheckBox.AutoSize = true;
      this.LauncherBspInfoCheckBox.FlatStyle = FlatStyle.Popup;
      this.LauncherBspInfoCheckBox.Location = new Point(9, 165);
      this.LauncherBspInfoCheckBox.Name = "LauncherBspInfoCheckBox";
      this.LauncherBspInfoCheckBox.Size = new Size(66, 17);
      this.LauncherBspInfoCheckBox.TabIndex = 7;
      this.LauncherBspInfoCheckBox.Text = "BSP Info";
      this.LauncherBspInfoCheckBox.UseVisualStyleBackColor = true;
      this.LauncherBuildFastFilesCheckBox.AutoSize = true;
      this.LauncherBuildFastFilesCheckBox.FlatStyle = FlatStyle.Popup;
      this.LauncherBuildFastFilesCheckBox.Location = new Point(9, 142);
      this.LauncherBuildFastFilesCheckBox.Name = "LauncherBuildFastFilesCheckBox";
      this.LauncherBuildFastFilesCheckBox.Size = new Size(88, 17);
      this.LauncherBuildFastFilesCheckBox.TabIndex = 6;
      this.LauncherBuildFastFilesCheckBox.Text = "Build Fastfiles";
      this.LauncherBuildFastFilesCheckBox.UseVisualStyleBackColor = true;
      this.LauncherCompileReflectionsCheckBox.AutoSize = true;
      this.LauncherCompileReflectionsCheckBox.FlatStyle = FlatStyle.Popup;
      this.LauncherCompileReflectionsCheckBox.Location = new Point(9, 111);
      this.LauncherCompileReflectionsCheckBox.Name = "LauncherCompileReflectionsCheckBox";
      this.LauncherCompileReflectionsCheckBox.Size = new Size(117, 17);
      this.LauncherCompileReflectionsCheckBox.TabIndex = 4;
      this.LauncherCompileReflectionsCheckBox.Text = "Compile Reflections";
      this.LauncherCompileReflectionsCheckBox.UseVisualStyleBackColor = true;
      this.LauncherConnectPathsCheckBox.AutoSize = true;
      this.LauncherConnectPathsCheckBox.FlatStyle = FlatStyle.Popup;
      this.LauncherConnectPathsCheckBox.Location = new Point(9, 88);
      this.LauncherConnectPathsCheckBox.Name = "LauncherConnectPathsCheckBox";
      this.LauncherConnectPathsCheckBox.Size = new Size(94, 17);
      this.LauncherConnectPathsCheckBox.TabIndex = 3;
      this.LauncherConnectPathsCheckBox.Text = "Connect Paths";
      this.LauncherConnectPathsCheckBox.UseVisualStyleBackColor = true;
      this.LauncherCompileVisCheckBox.AutoSize = true;
      this.LauncherCompileVisCheckBox.FlatStyle = FlatStyle.Popup;
      this.LauncherCompileVisCheckBox.Location = new Point(9, 65);
      this.LauncherCompileVisCheckBox.Name = "LauncherCompileVisCheckBox";
      this.LauncherCompileVisCheckBox.Size = new Size(78, 17);
      this.LauncherCompileVisCheckBox.TabIndex = 2;
      this.LauncherCompileVisCheckBox.Text = "Compile Vis";
      this.LauncherCompileVisCheckBox.UseVisualStyleBackColor = true;
      this.LauncherCompileLightsCheckBox.AutoSize = true;
      this.LauncherCompileLightsCheckBox.FlatStyle = FlatStyle.Popup;
      this.LauncherCompileLightsCheckBox.Location = new Point(9, 42);
      this.LauncherCompileLightsCheckBox.Name = "LauncherCompileLightsCheckBox";
      this.LauncherCompileLightsCheckBox.Size = new Size(92, 17);
      this.LauncherCompileLightsCheckBox.TabIndex = 1;
      this.LauncherCompileLightsCheckBox.Text = "Compile Lights";
      this.LauncherCompileLightsCheckBox.UseVisualStyleBackColor = true;
      this.LauncherCompileBSPCheckBox.AutoSize = true;
      this.LauncherCompileBSPCheckBox.FlatStyle = FlatStyle.Popup;
      this.LauncherCompileBSPCheckBox.Location = new Point(9, 19);
      this.LauncherCompileBSPCheckBox.Name = "LauncherCompileBSPCheckBox";
      this.LauncherCompileBSPCheckBox.Size = new Size(85, 17);
      this.LauncherCompileBSPCheckBox.TabIndex = 0;
      this.LauncherCompileBSPCheckBox.Text = "Compile BSP";
      this.LauncherCompileBSPCheckBox.UseVisualStyleBackColor = true;
      this.LauncherCreateMapButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.LauncherCreateMapButton.Location = new Point(84, 316);
      this.LauncherCreateMapButton.Name = "LauncherCreateMapButton";
      this.LauncherCreateMapButton.Size = new Size(72, 32);
      this.LauncherCreateMapButton.TabIndex = 2;
      this.LauncherCreateMapButton.Text = "Create Map";
      this.LauncherCreateMapButton.UseVisualStyleBackColor = true;
      this.LauncherCreateMapButton.Click += new EventHandler(this.LauncherButtonCreateMap_Click);
      this.LauncherMapList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
      this.LauncherMapList.FormattingEnabled = true;
      this.LauncherMapList.IntegralHeight = false;
      this.LauncherMapList.Location = new Point(6, 6);
      this.LauncherMapList.Name = "LauncherMapList";
      this.LauncherMapList.Size = new Size(150, 304);
      this.LauncherMapList.TabIndex = 1;
      this.LauncherMapList.SelectedIndexChanged += new EventHandler(this.LauncherMapList_SelectedIndexChanged);
      this.LauncherMapList.DoubleClick += new EventHandler(this.LauncherMapList_DoubleClick);
      this.LauncherTabModBuilder.Controls.Add((Control) this.LauncherIwdFileGroupBox);
      this.LauncherTabModBuilder.Controls.Add((Control) this.LauncherFastFileCsvGroupBox);
      this.LauncherTabModBuilder.Controls.Add((Control) this.LauncherModGroupBox);
      this.LauncherTabModBuilder.Location = new Point(4, 22);
      this.LauncherTabModBuilder.Name = "LauncherTabModBuilder";
      this.LauncherTabModBuilder.Padding = new Padding(3);
      this.LauncherTabModBuilder.Size = new Size(656, 354);
      this.LauncherTabModBuilder.TabIndex = 1;
      this.LauncherTabModBuilder.Text = "Mod Builder";
      this.LauncherTabModBuilder.UseVisualStyleBackColor = true;
      this.LauncherIwdFileGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherIwdFileGroupBox.Controls.Add((Control) this.LauncherIwdFileTree);
      this.LauncherIwdFileGroupBox.Location = new Point(298, 6);
      this.LauncherIwdFileGroupBox.Name = "LauncherIwdFileGroupBox";
      this.LauncherIwdFileGroupBox.Size = new Size(357, 342);
      this.LauncherIwdFileGroupBox.TabIndex = 2;
      this.LauncherIwdFileGroupBox.TabStop = false;
      this.LauncherIwdFileGroupBox.Text = "IWD File List";
      this.LauncherIwdFileTree.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherIwdFileTree.CheckBoxes = true;
      this.LauncherIwdFileTree.Indent = 15;
      this.LauncherIwdFileTree.Location = new Point(6, 19);
      this.LauncherIwdFileTree.Name = "LauncherIwdFileTree";
      this.LauncherIwdFileTree.Size = new Size(345, 316);
      this.LauncherIwdFileTree.TabIndex = 1;
      this.LauncherIwdFileTree.AfterCheck += new TreeViewEventHandler(this.LauncherIwdFileTree_AfterCheck);
      this.LauncherIwdFileTree.DoubleClick += new EventHandler(this.LauncherIwdFileTree_DoubleClick);
      this.LauncherFastFileCsvGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
      this.LauncherFastFileCsvGroupBox.Controls.Add((Control) this.LauncherFastFileCsvTextBox);
      this.LauncherFastFileCsvGroupBox.Location = new Point(6, 138);
      this.LauncherFastFileCsvGroupBox.Name = "LauncherFastFileCsvGroupBox";
      this.LauncherFastFileCsvGroupBox.Size = new Size(286, 210);
      this.LauncherFastFileCsvGroupBox.TabIndex = 19;
      this.LauncherFastFileCsvGroupBox.TabStop = false;
      this.LauncherFastFileCsvGroupBox.Text = "Fastfile mod.csv";
      this.LauncherFastFileCsvTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherFastFileCsvTextBox.Font = new Font("Courier New", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.LauncherFastFileCsvTextBox.Location = new Point(6, 17);
      this.LauncherFastFileCsvTextBox.Name = "LauncherFastFileCsvTextBox";
      this.LauncherFastFileCsvTextBox.Size = new Size(274, 186);
      this.LauncherFastFileCsvTextBox.TabIndex = 0;
      this.LauncherFastFileCsvTextBox.Text = "";
      this.LauncherFastFileCsvTextBox.WordWrap = false;
      this.LauncherModGroupBox.Controls.Add((Control) this.LauncherModBuildButton);
      this.LauncherModGroupBox.Controls.Add((Control) this.LauncherModBuildSoundsCheckBox);
      this.LauncherModGroupBox.Controls.Add((Control) this.LauncherModVerboseCheckBox);
      this.LauncherModGroupBox.Controls.Add((Control) this.LauncherModBuildIwdFileCheckBox);
      this.LauncherModGroupBox.Controls.Add((Control) this.LauncherModBuildFastFilesCheckBox);
      this.LauncherModGroupBox.Controls.Add((Control) this.LauncherModComboBox);
      this.LauncherModGroupBox.Location = new Point(6, 6);
      this.LauncherModGroupBox.Name = "LauncherModGroupBox";
      this.LauncherModGroupBox.Size = new Size(286, 126);
      this.LauncherModGroupBox.TabIndex = 4;
      this.LauncherModGroupBox.TabStop = false;
      this.LauncherModGroupBox.Text = "MOD";
      this.LauncherModBuildButton.Location = new Point(7, 93);
      this.LauncherModBuildButton.Name = "LauncherModBuildButton";
      this.LauncherModBuildButton.Size = new Size(88, 26);
      this.LauncherModBuildButton.TabIndex = 18;
      this.LauncherModBuildButton.Text = "Build MOD";
      this.LauncherModBuildButton.UseVisualStyleBackColor = true;
      this.LauncherModBuildButton.Click += new EventHandler(this.LauncherModBuildButton_Click);
      this.LauncherModBuildSoundsCheckBox.AutoSize = true;
      this.LauncherModBuildSoundsCheckBox.Location = new Point(130, 47);
      this.LauncherModBuildSoundsCheckBox.Name = "LauncherModBuildSoundsCheckBox";
      this.LauncherModBuildSoundsCheckBox.Size = new Size(88, 17);
      this.LauncherModBuildSoundsCheckBox.TabIndex = 17;
      this.LauncherModBuildSoundsCheckBox.Text = "Build Sounds";
      this.LauncherModBuildSoundsCheckBox.UseVisualStyleBackColor = true;
      this.LauncherModVerboseCheckBox.AutoSize = true;
      this.LauncherModVerboseCheckBox.Location = new Point(130, 70);
      this.LauncherModVerboseCheckBox.Name = "LauncherModVerboseCheckBox";
      this.LauncherModVerboseCheckBox.Size = new Size(65, 17);
      this.LauncherModVerboseCheckBox.TabIndex = 15;
      this.LauncherModVerboseCheckBox.Text = "Verbose";
      this.LauncherModVerboseCheckBox.UseVisualStyleBackColor = true;
      this.LauncherModBuildIwdFileCheckBox.AutoSize = true;
      this.LauncherModBuildIwdFileCheckBox.Location = new Point(7, 70);
      this.LauncherModBuildIwdFileCheckBox.Name = "LauncherModBuildIwdFileCheckBox";
      this.LauncherModBuildIwdFileCheckBox.Size = new Size(93, 17);
      this.LauncherModBuildIwdFileCheckBox.TabIndex = 14;
      this.LauncherModBuildIwdFileCheckBox.Text = "Build IWD File";
      this.LauncherModBuildIwdFileCheckBox.UseVisualStyleBackColor = true;
      this.LauncherModBuildFastFilesCheckBox.AutoSize = true;
      this.LauncherModBuildFastFilesCheckBox.Location = new Point(7, 47);
      this.LauncherModBuildFastFilesCheckBox.Name = "LauncherModBuildFastFilesCheckBox";
      this.LauncherModBuildFastFilesCheckBox.Size = new Size(117, 17);
      this.LauncherModBuildFastFilesCheckBox.TabIndex = 13;
      this.LauncherModBuildFastFilesCheckBox.Text = "Build mod.ff Fastfile";
      this.LauncherModBuildFastFilesCheckBox.UseVisualStyleBackColor = true;
      this.LauncherModComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      this.LauncherModComboBox.FormattingEnabled = true;
      this.LauncherModComboBox.Location = new Point(6, 20);
      this.LauncherModComboBox.Name = "LauncherModComboBox";
      this.LauncherModComboBox.Size = new Size(274, 21);
      this.LauncherModComboBox.TabIndex = 3;
      this.LauncherModComboBox.SelectedIndexChanged += new EventHandler(this.LauncherModComboBox_SelectedIndexChanged);
      this.LauncherTabRunGame.Controls.Add((Control) this.LauncherGameOptionsPanel);
      this.LauncherTabRunGame.Controls.Add((Control) this.LauncherRunGameButton);
      this.LauncherTabRunGame.Controls.Add((Control) this.LauncherRunGameCustomCommandLineGroupBox);
      this.LauncherTabRunGame.Controls.Add((Control) this.LauncherRunGameCommandLineGroupBox);
      this.LauncherTabRunGame.Controls.Add((Control) this.LauncherRunGameModGroupBox);
      this.LauncherTabRunGame.Controls.Add((Control) this.LauncherRunGameExeTypeGroupBox);
      this.LauncherTabRunGame.Location = new Point(4, 22);
      this.LauncherTabRunGame.Name = "LauncherTabRunGame";
      this.LauncherTabRunGame.Padding = new Padding(3);
      this.LauncherTabRunGame.Size = new Size(656, 354);
      this.LauncherTabRunGame.TabIndex = 2;
      this.LauncherTabRunGame.Text = "Run Game";
      this.LauncherTabRunGame.UseVisualStyleBackColor = true;
      this.LauncherGameOptionsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherGameOptionsPanel.AutoScroll = true;
      this.LauncherGameOptionsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.LauncherGameOptionsPanel.BorderStyle = BorderStyle.Fixed3D;
      this.LauncherGameOptionsPanel.Location = new Point(6, 59);
      this.LauncherGameOptionsPanel.Margin = new Padding(0);
      this.LauncherGameOptionsPanel.Name = "LauncherGameOptionsPanel";
      this.LauncherGameOptionsPanel.Size = new Size(644, 150);
      this.LauncherGameOptionsPanel.TabIndex = 5;
      this.LauncherGameOptionsPanel.Click += new EventHandler(this.LauncherGameOptionsFlowPanel_Click);
      this.LauncherRunGameButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.LauncherRunGameButton.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.LauncherRunGameButton.Location = new Point(522, 265);
      this.LauncherRunGameButton.Name = "LauncherRunGameButton";
      this.LauncherRunGameButton.Size = new Size(128, 83);
      this.LauncherRunGameButton.TabIndex = 2;
      this.LauncherRunGameButton.Text = "Run Game";
      this.LauncherRunGameButton.UseVisualStyleBackColor = true;
      this.LauncherRunGameButton.Click += new EventHandler(this.LauncherRunGameButton_Click);
      this.LauncherRunGameCustomCommandLineGroupBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherRunGameCustomCommandLineGroupBox.Controls.Add((Control) this.LauncherRunGameCustomCommandLineTextBox);
      this.LauncherRunGameCustomCommandLineGroupBox.Location = new Point(6, 215);
      this.LauncherRunGameCustomCommandLineGroupBox.Name = "LauncherRunGameCustomCommandLineGroupBox";
      this.LauncherRunGameCustomCommandLineGroupBox.Size = new Size(644, 44);
      this.LauncherRunGameCustomCommandLineGroupBox.TabIndex = 4;
      this.LauncherRunGameCustomCommandLineGroupBox.TabStop = false;
      this.LauncherRunGameCustomCommandLineGroupBox.Text = "Custom Command Line";
      this.LauncherRunGameCustomCommandLineTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherRunGameCustomCommandLineTextBox.Location = new Point(6, 17);
      this.LauncherRunGameCustomCommandLineTextBox.Name = "LauncherRunGameCustomCommandLineTextBox";
      this.LauncherRunGameCustomCommandLineTextBox.Size = new Size(632, 20);
      this.LauncherRunGameCustomCommandLineTextBox.TabIndex = 0;
      this.LauncherRunGameCustomCommandLineTextBox.TextChanged += new EventHandler(this.LauncherRunGameCustomCommandLineTextBox_TextChanged);
      this.LauncherRunGameCustomCommandLineTextBox.Validating += new CancelEventHandler(this.LauncherRunGameCustomCommandLineTextBox_Validating);
      this.LauncherRunGameCommandLineGroupBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherRunGameCommandLineGroupBox.Controls.Add((Control) this.LauncherRunGameCommandLineTextBox);
      this.LauncherRunGameCommandLineGroupBox.Location = new Point(6, 265);
      this.LauncherRunGameCommandLineGroupBox.Name = "LauncherRunGameCommandLineGroupBox";
      this.LauncherRunGameCommandLineGroupBox.Size = new Size(510, 83);
      this.LauncherRunGameCommandLineGroupBox.TabIndex = 3;
      this.LauncherRunGameCommandLineGroupBox.TabStop = false;
      this.LauncherRunGameCommandLineGroupBox.Text = "Command Line";
      this.LauncherRunGameCommandLineTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherRunGameCommandLineTextBox.BorderStyle = BorderStyle.None;
      this.LauncherRunGameCommandLineTextBox.Location = new Point(6, 19);
      this.LauncherRunGameCommandLineTextBox.Multiline = true;
      this.LauncherRunGameCommandLineTextBox.Name = "LauncherRunGameCommandLineTextBox";
      this.LauncherRunGameCommandLineTextBox.ReadOnly = true;
      this.LauncherRunGameCommandLineTextBox.Size = new Size(498, 58);
      this.LauncherRunGameCommandLineTextBox.TabIndex = 0;
      this.LauncherRunGameModGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherRunGameModGroupBox.Controls.Add((Control) this.LauncherRunGameModComboBox);
      this.LauncherRunGameModGroupBox.Location = new Point(109, 6);
      this.LauncherRunGameModGroupBox.Name = "LauncherRunGameModGroupBox";
      this.LauncherRunGameModGroupBox.Size = new Size(541, 47);
      this.LauncherRunGameModGroupBox.TabIndex = 1;
      this.LauncherRunGameModGroupBox.TabStop = false;
      this.LauncherRunGameModGroupBox.Text = "Mod";
      this.LauncherRunGameModComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherRunGameModComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      this.LauncherRunGameModComboBox.FormattingEnabled = true;
      this.LauncherRunGameModComboBox.Location = new Point(6, 18);
      this.LauncherRunGameModComboBox.Name = "LauncherRunGameModComboBox";
      this.LauncherRunGameModComboBox.Size = new Size(529, 21);
      this.LauncherRunGameModComboBox.TabIndex = 0;
      this.LauncherRunGameExeTypeGroupBox.Controls.Add((Control) this.LauncherRunGameExeTypeMpRadioButton);
      this.LauncherRunGameExeTypeGroupBox.Controls.Add((Control) this.LauncherRunGameTypeRadioButton);
      this.LauncherRunGameExeTypeGroupBox.Location = new Point(6, 6);
      this.LauncherRunGameExeTypeGroupBox.Name = "LauncherRunGameExeTypeGroupBox";
      this.LauncherRunGameExeTypeGroupBox.Size = new Size(97, 47);
      this.LauncherRunGameExeTypeGroupBox.TabIndex = 0;
      this.LauncherRunGameExeTypeGroupBox.TabStop = false;
      this.LauncherRunGameExeTypeGroupBox.Text = "Exe Type";
      this.LauncherRunGameExeTypeMpRadioButton.AutoSize = true;
      this.LauncherRunGameExeTypeMpRadioButton.Location = new Point(50, 19);
      this.LauncherRunGameExeTypeMpRadioButton.Name = "LauncherRunGameExeTypeMpRadioButton";
      this.LauncherRunGameExeTypeMpRadioButton.Size = new Size(41, 17);
      this.LauncherRunGameExeTypeMpRadioButton.TabIndex = 1;
      this.LauncherRunGameExeTypeMpRadioButton.Text = "MP";
      this.LauncherRunGameExeTypeMpRadioButton.UseVisualStyleBackColor = true;
      this.LauncherRunGameTypeRadioButton.AutoSize = true;
      this.LauncherRunGameTypeRadioButton.Checked = true;
      this.LauncherRunGameTypeRadioButton.Location = new Point(6, 19);
      this.LauncherRunGameTypeRadioButton.Name = "LauncherRunGameTypeRadioButton";
      this.LauncherRunGameTypeRadioButton.Size = new Size(39, 17);
      this.LauncherRunGameTypeRadioButton.TabIndex = 0;
      this.LauncherRunGameTypeRadioButton.TabStop = true;
      this.LauncherRunGameTypeRadioButton.Text = "SP";
      this.LauncherRunGameTypeRadioButton.UseVisualStyleBackColor = true;
      this.LauncherProcessTimeElapsedTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.LauncherProcessTimeElapsedTextBox.Location = new Point(756, 233);
      this.LauncherProcessTimeElapsedTextBox.Name = "LauncherProcessTimeElapsedTextBox";
      this.LauncherProcessTimeElapsedTextBox.ReadOnly = true;
      this.LauncherProcessTimeElapsedTextBox.Size = new Size(55, 20);
      this.LauncherProcessTimeElapsedTextBox.TabIndex = 4;
      this.LauncherProcessTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherProcessTextBox.Location = new Point(149, 233);
      this.LauncherProcessTextBox.Name = "LauncherProcessTextBox";
      this.LauncherProcessTextBox.ReadOnly = true;
      this.LauncherProcessTextBox.Size = new Size(607, 20);
      this.LauncherProcessTextBox.TabIndex = 3;
      this.LauncherProcessGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
      this.LauncherProcessGroupBox.Controls.Add((Control) this.LauncherButtonCancel);
      this.LauncherProcessGroupBox.Controls.Add((Control) this.LauncherProcessList);
      this.LauncherProcessGroupBox.Location = new Point(3, 3);
      this.LauncherProcessGroupBox.Name = "LauncherProcessGroupBox";
      this.LauncherProcessGroupBox.Size = new Size(140, 250);
      this.LauncherProcessGroupBox.TabIndex = 2;
      this.LauncherProcessGroupBox.TabStop = false;
      this.LauncherProcessGroupBox.Text = "Processes";
      this.LauncherButtonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherButtonCancel.BackColor = Color.LightCoral;
      this.LauncherButtonCancel.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.LauncherButtonCancel.ForeColor = SystemColors.Info;
      this.LauncherButtonCancel.Location = new Point(6, 179);
      this.LauncherButtonCancel.Name = "LauncherButtonCancel";
      this.LauncherButtonCancel.Size = new Size(128, 65);
      this.LauncherButtonCancel.TabIndex = 4;
      this.LauncherButtonCancel.Text = "Cancel";
      this.LauncherButtonCancel.UseVisualStyleBackColor = false;
      this.LauncherButtonCancel.Click += new EventHandler(this.LauncherButtonCancel_Click);
      this.LauncherProcessList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.LauncherProcessList.BackColor = SystemColors.Info;
      this.LauncherProcessList.BorderStyle = BorderStyle.FixedSingle;
      this.LauncherProcessList.Font = new Font("Lucida Console", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.LauncherProcessList.ForeColor = SystemColors.HotTrack;
      this.LauncherProcessList.FormattingEnabled = true;
      this.LauncherProcessList.IntegralHeight = false;
      this.LauncherProcessList.ItemHeight = 11;
      this.LauncherProcessList.Location = new Point(6, 19);
      this.LauncherProcessList.Name = "LauncherProcessList";
      this.LauncherProcessList.Size = new Size(128, 154);
      this.LauncherProcessList.TabIndex = 1;
      this.LauncherProcessList.SelectedIndexChanged += new EventHandler(this.LauncherProcessList_SelectedIndexChanged);
      this.LauncherTimer.Enabled = true;
      this.LauncherTimer.Interval = 1000;
      this.LauncherTimer.Tick += new EventHandler(this.LauncherTimer_Tick);
      this.LauncherWikiLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.LauncherWikiLabel.AutoSize = true;
      this.LauncherWikiLabel.Font = new Font("Courier New", 11.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.LauncherWikiLabel.Location = new Point(779, 3);
      this.LauncherWikiLabel.Name = "LauncherWikiLabel";
      this.LauncherWikiLabel.Size = new Size(44, 17);
      this.LauncherWikiLabel.TabIndex = 6;
      this.LauncherWikiLabel.TabStop = true;
      this.LauncherWikiLabel.Text = "WIKI";
      this.LauncherWikiLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(this.LauncherWikiLabel_LinkClicked);
      this.LauncherAboutLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.LauncherAboutLabel.AutoSize = true;
      this.LauncherAboutLabel.Font = new Font("Courier New", 11.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.LauncherAboutLabel.Location = new Point(723, 3);
      this.LauncherAboutLabel.Name = "LauncherAboutLabel";
      this.LauncherAboutLabel.Size = new Size(53, 17);
      this.LauncherAboutLabel.TabIndex = 7;
      this.LauncherAboutLabel.TabStop = true;
      this.LauncherAboutLabel.Text = "ABOUT";
      this.LauncherAboutLabel.TextAlign = ContentAlignment.MiddleRight;
      this.LauncherAboutLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(this.LauncherAboutLabel_LinkClicked);
      this.LauncherMapFilesSystemWatcher.EnableRaisingEvents = true;
      this.LauncherMapFilesSystemWatcher.Filter = "*.map";
      this.LauncherMapFilesSystemWatcher.NotifyFilter = NotifyFilters.FileName;
      this.LauncherMapFilesSystemWatcher.SynchronizingObject = (ISynchronizeInvoke) this;
      this.LauncherMapFilesSystemWatcher.Renamed += new RenamedEventHandler(this.LauncherMapFilesSystemWatcher_Renamed);
      this.LauncherMapFilesSystemWatcher.Deleted += new FileSystemEventHandler(this.LauncherMapFilesSystemWatcher_Deleted);
      this.LauncherMapFilesSystemWatcher.Created += new FileSystemEventHandler(this.LauncherMapFilesSystemWatcher_Created);
      this.LauncherMapFilesSystemWatcher.Changed += new FileSystemEventHandler(this.LauncherMapFilesSystemWatcher_Changed);
      this.LauncherModsDirectorySystemWatcher.EnableRaisingEvents = true;
      this.LauncherModsDirectorySystemWatcher.NotifyFilter = NotifyFilters.DirectoryName;
      this.LauncherModsDirectorySystemWatcher.SynchronizingObject = (ISynchronizeInvoke) this;
      this.LauncherModsDirectorySystemWatcher.Renamed += new RenamedEventHandler(this.LauncherModsDirectorySystemWatcher_Renamed);
      this.LauncherModsDirectorySystemWatcher.Deleted += new FileSystemEventHandler(this.LauncherModsDirectorySystemWatcher_Deleted);
      this.LauncherModsDirectorySystemWatcher.Created += new FileSystemEventHandler(this.LauncherModsDirectorySystemWatcher_Created);
      this.LauncherModsDirectorySystemWatcher.Changed += new FileSystemEventHandler(this.LauncherModsDirectorySystemWatcher_Changed);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(838, 664);
      this.Controls.Add((Control) this.LauncherAboutLabel);
      this.Controls.Add((Control) this.LauncherWikiLabel);
      this.Controls.Add((Control) this.LauncherSplitter);
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.Name = nameof (LauncherForm);
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "Launcher";
      this.Load += new EventHandler(this.LauncherForm_Load);
      this.FormClosing += new FormClosingEventHandler(this.LauncherForm_FormClosing);
      this.LauncherSplitter.Panel1.ResumeLayout(false);
      this.LauncherSplitter.Panel2.ResumeLayout(false);
      this.LauncherSplitter.Panel2.PerformLayout();
      this.LauncherSplitter.ResumeLayout(false);
      this.LauncherApplicationsGroupBox.ResumeLayout(false);
      this.LauncherTab.ResumeLayout(false);
      this.LauncherTabCompileLevel.ResumeLayout(false);
      this.LauncherCompileLevelOptionsGroupBox.ResumeLayout(false);
      this.LauncherCompileLevelOptionsGroupBox.PerformLayout();
      this.LauncherGridFileGroupBox.ResumeLayout(false);
      this.LauncherGridFileGroupBox.PerformLayout();
      this.LauncherTabModBuilder.ResumeLayout(false);
      this.LauncherIwdFileGroupBox.ResumeLayout(false);
      this.LauncherFastFileCsvGroupBox.ResumeLayout(false);
      this.LauncherModGroupBox.ResumeLayout(false);
      this.LauncherModGroupBox.PerformLayout();
      this.LauncherTabRunGame.ResumeLayout(false);
      this.LauncherRunGameCustomCommandLineGroupBox.ResumeLayout(false);
      this.LauncherRunGameCustomCommandLineGroupBox.PerformLayout();
      this.LauncherRunGameCommandLineGroupBox.ResumeLayout(false);
      this.LauncherRunGameCommandLineGroupBox.PerformLayout();
      this.LauncherRunGameModGroupBox.ResumeLayout(false);
      this.LauncherRunGameExeTypeGroupBox.ResumeLayout(false);
      this.LauncherRunGameExeTypeGroupBox.PerformLayout();
      this.LauncherProcessGroupBox.ResumeLayout(false);
      this.LauncherMapFilesSystemWatcher.EndInit();
      this.LauncherModsDirectorySystemWatcher.EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public delegate void ProcessFinishedDelegate(Process lastProcess);
  }
}
