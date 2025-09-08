using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Launcher
{
	public class LauncherForm : Form
	{
		private ComboBox[] dvarComboBoxes = new ComboBox[0];
		private readonly Hashtable processTable = new Hashtable();
		private readonly ArrayList processList = new ArrayList();
		private Process consoleProcess;
		private DateTime consoleProcessStartTime;
		private string mapName;
		private string modName;
		private long consoleTicksWhenLastFocus = DateTime.Now.Ticks;
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
		private Timer LauncherTimer;
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
		internal RichTextBox LauncherConsole;
		private Button LauncherDeleteMapButton;
		private LinkLabel LauncherAboutLabel;
		private FileSystemWatcher LauncherMapFilesSystemWatcher;
		private FileSystemWatcher LauncherModsDirectorySystemWatcher;
		private CheckBox LauncherGridCollectDotsCheckBox;
		private GroupBox LauncherGridFileGroupBox;
		private Button LauncherGridMakeNewButton;
		private Button LauncherGridEditExistingButton;

		internal LauncherForm()
		{
			InitializeComponent();
		}

		private void UpdateDVars()
		{
			var gameOptionsPanel = LauncherGameOptionsPanel;

			const int height = 34;
			int yPos = -height;
			int index = 0;
			bool useAltColour = true;

			var backColour = gameOptionsPanel.BackColor;
			var altColour = Color.FromArgb(
				backColour.R * 14 / 15,
				backColour.G * 14 / 15,
				backColour.B * 14 / 15);

			dvarComboBoxes = new ComboBox[Launcher.DVars.Length];

			foreach (var dvar in Launcher.DVars)
			{
				var panel = new Panel
				{
					BackColor = (useAltColour = !useAltColour) ? backColour : altColour,
					Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
				};
				panel.SetBounds(0, yPos += height, gameOptionsPanel.ClientSize.Width, height);
				panel.Click += LauncherGameOptionsFlowPanel_Click;

				var label = new Label
				{
					Text = $"{dvar.Name} ({dvar.Description})",
					TextAlign = ContentAlignment.MiddleLeft
				};
				label.SetBounds(4, 0, gameOptionsPanel.ClientSize.Width - 220, height);
				label.Click += LauncherGameOptionsFlowPanel_Click;
				panel.Controls.Add(label);

				var comboBox = new ComboBox
				{
					Tag = dvar.Name,
					AutoCompleteMode = AutoCompleteMode.SuggestAppend,
					AutoCompleteSource = AutoCompleteSource.ListItems
				};

				comboBox.Items.Add("(not set)");

				if (dvar.IsDecimal)
				{
					var currentValue = dvar.DecimalMin;
					while (currentValue <= dvar.DecimalMax)
					{
						comboBox.Items.Add(currentValue.ToString());
						currentValue += dvar.DecimalIncrement;
					}
				}
				else if (dvar.Name == "devmap")
				{
					comboBox.Items.AddRange(Launcher.GetMapList());
				}

				comboBox.SelectedIndex = 0;
				comboBox.SetBounds(gameOptionsPanel.ClientSize.Width - 205, 8, 200, height);
				comboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;

				panel.Controls.Add(comboBox);
				comboBox.BringToFront();

				gameOptionsPanel.Controls.Add(panel);
				dvarComboBoxes[index++] = comboBox;
			}
		}

		private string FormatDVar(ComboBox comboBox)
		{
			string value = string.Empty;
			if (comboBox.SelectedItem != null && comboBox.SelectedIndex > 0)
			{
				value = comboBox.SelectedItem.ToString();
			}
			else if (comboBox.Items.Count > 0 && comboBox.Items[0].ToString() != comboBox.Text)
			{
				value = comboBox.Text;
			}

			value = value.Trim();
			if (string.IsNullOrEmpty(value))
			{
				return string.Empty;
			}
			return $"+set {comboBox.Tag} {value} ";
		}

		private string FormatDvars()
		{
			var builder = new StringBuilder();
			foreach (var comboBox in dvarComboBoxes)
			{
				builder.Append(FormatDVar(comboBox));
			}
			return builder.ToString();
		}

		private void LauncherForm_Load(object sender, EventArgs e)
		{
			UpdateDVars();
			UpdateMapList();
			UpdateModList();
			EnableMapList();
			UpdateStopProcessButton();

			LauncherMapFilesSystemWatcher.Path = Launcher.GetMapSourceDirectory();
			LauncherModsDirectorySystemWatcher.Path = Launcher.GetModsDirectory();
			LauncherMapFilesSystemWatcher.EnableRaisingEvents = true;
			LauncherModsDirectorySystemWatcher.EnableRaisingEvents = true;

			// Set window title to show root directory
			Text += $" - {Launcher.GetRootDirectory()}";
		}

		private event ProcessFinishedDelegate processFinishedDelegate;

		private void UpdateRunGameCommandLine()
		{
		}

		private void UpdateConsoleColour()
		{
			LauncherConsole.BackColor = (consoleProcess == null)
				? Color.White
				: Color.LightGoldenrodYellow;
		}

		private void UpdateStopProcessButton()
		{
			int selectedIndex = LauncherProcessList.SelectedIndex;
			if (selectedIndex < 0)
			{
				LauncherButtonCancel.Enabled = false;
				LauncherButtonCancel.Text = "No Active Process\n\nStart one and then use this button to stop it";
				return;
			}

			LauncherButtonCancel.Enabled = true;

			var entry = (DictionaryEntry)processList[selectedIndex];
			string fileName = Path.GetFileNameWithoutExtension(entry.Value.ToString());

			if (entry.Key == consoleProcess)
			{
				LauncherButtonCancel.Text = $"Stop Console Process\n\n{fileName}";
			}
			else
			{
				LauncherButtonCancel.Text = $"Stop Application\n\n{fileName}";
			}
		}

		private void UpdateMapList()
		{
			var previousSelectedItem = LauncherMapList.SelectedItem;
			int previousSelectedIndex = LauncherMapList.SelectedIndex;

			// Refresh the map list
			LauncherMapList.Items.Clear();
			LauncherMapList.Items.AddRange(Launcher.GetMapList());

			if (LauncherMapList.Items.Count == 0)
			{
				return;
			}

			// Try to restore previous selection
			LauncherMapList.SelectedItem = previousSelectedItem;

			if (LauncherMapList.SelectedItem == null)
			{
				// If previous item no longer exists, select a valid index
				LauncherMapList.SelectedIndex = Math.Max(0, Math.Min(previousSelectedIndex, LauncherMapList.Items.Count - 1));
			}
		}

		private void UpdateModList()
		{
			var comboBoxes = new[]
			{
				LauncherRunGameModComboBox,
				LauncherModComboBox,
				LauncherModSpecificMapComboBox
			};

			var modList = Launcher.GetModList();

			// Clear existing items
			foreach (var comboBox in comboBoxes)
			{
				comboBox.Items.Clear();
			}

			LauncherRunGameModComboBox.Items.Add("(not set)");

			// Populate combo boxes with mod list and select first item
			foreach (var comboBox in comboBoxes)
			{
				comboBox.Items.AddRange(modList);
				if (comboBox.Items.Count > 0)
				{
					comboBox.SelectedIndex = 0;
				}
			}
		}

		private void UpdateProcessList()
		{
			LauncherProcessList.Invoke((Action)(() =>
			{
				// Clear existing process entries
				processList.Clear();
				LauncherProcessList.Items.Clear();

				// Populate process list and display names
				foreach (DictionaryEntry entry in processTable)
				{
					processList.Add(entry);
					LauncherProcessList.Items.Add(Path.GetFileNameWithoutExtension((string)entry.Value));
				}

				if (LauncherProcessList.SelectedIndex < 0 && LauncherProcessList.Items.Count > 0)
				{
					LauncherProcessList.SelectedIndex = 0;
				}

				// Update the stop button state
				UpdateStopProcessButton();
			}));
		}

		private void WriteConsole(string str, bool isStdError)
		{
			if (str == null)
			{
				return;
			}

			long ticks = DateTime.Now.Ticks;
			bool doFocus = ticks - consoleTicksWhenLastFocus > 10000000L;
			if (doFocus)
			{
				consoleTicksWhenLastFocus = ticks;
			}

			LauncherConsole.Invoke((Action)(() =>
			{
				Color selectionColour = LauncherConsole.SelectionColor;
				Font selectionFont = LauncherConsole.SelectionFont;
				bool flag1 = isStdError || str.Contains("ERROR:");
				bool flag2 = str.Contains("WARNING:");

				if (flag1 || flag2)
				{
					LauncherConsole.SelectionFont = new Font(LauncherConsole.SelectionFont, FontStyle.Bold);
					LauncherConsole.SelectionColor = flag1 ? Color.Red : Color.Green;
				}

				LauncherConsole.AppendText(str + "\n");

				if (doFocus)
				{
					LauncherConsole.Focus();
				}

				if (flag1 || flag2)
				{
					LauncherConsole.SelectionColor = selectionColour;
					LauncherConsole.SelectionFont = selectionFont;
				}
			}));
		}

		private void WriteMessage(string str)
		{
			WriteConsole(str, false);
		}

		private void WriteError(string str)
		{
			WriteConsole(str, true);
		}

		private void LaunchProcessHelper(
			bool shouldRun,
			ProcessFinishedDelegate nextStage,
			Process lastProcess,
			string processName,
			string processOptions,
			string workingDirectory)
		{
			// If the previous process failed or we shouldn't run, call the next stage immediately
			if ((lastProcess != null && lastProcess.ExitCode != 0) || !shouldRun)
			{
				nextStage(lastProcess);
				return;
			}

			// Otherwise, launch the process and continue with the next stage when finished
			LaunchProcess(
				processName,
				processOptions,
				workingDirectory,
				true,
				nextStage);
		}

		private void LaunchProcessHelper(
			bool shouldRun,
			ProcessFinishedDelegate nextStage,
			Process lastProcess,
			string processName,
			string processOptions)
		{
			LaunchProcessHelper(
				shouldRun,
				nextStage,
				lastProcess,
				processName,
				processOptions,
				null);
		}


		private void LaunchProcess(
			string processFileName,
			string arguments,
			string workingDirectory,
			bool consoleAttached,
			ProcessFinishedDelegate theProcessFinishedDelegate)
		{
			// Prevent multiple console processes
			if (consoleProcess != null && consoleAttached)
			{
				LauncherConsole.Invoke((Action)(() =>
				{
					string message;
					if (!Equals(processFileName, processTable[consoleProcess]))
					{
						message = $"Cannot start console process {processFileName}!\n\n" +
								  $"Another console process ({processTable[consoleProcess]}) is already running";
					}
					else
					{
						message = $"Console process ({processFileName}) is already running!";
					}

					MessageBox.Show(message, "Console Busy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}));
				return;
			}

			try
			{
				var process = new Process
				{
					StartInfo =
					{
						FileName = Path.Combine(Launcher.GetStartupDirectory(), processFileName),
						Arguments = arguments,
						CreateNoWindow = true,
						UseShellExecute = false
					},
					EnableRaisingEvents = true
				};

				process.StartInfo.WorkingDirectory = workingDirectory ?? Path.GetDirectoryName(process.StartInfo.FileName);

				// Remove from process table and update UI when process exits
				process.Exited += (sender, e) =>
				{
					processTable.Remove(process);
					UpdateProcessList();
				};

				if (consoleAttached)
				{
					processFinishedDelegate = theProcessFinishedDelegate;

					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.RedirectStandardError = true;

					process.OutputDataReceived += (s, e) => WriteConsole(e.Data, false);
					process.ErrorDataReceived += (s, e) => WriteConsole(e.Data, true);

					process.Exited += (sender, e) =>
					{
						LauncherButtonCancel.Invoke((Action)(() =>
						{
							LauncherProcessTimeElapsedTextBox.Text = (process.ExitCode != 0)
								? $"Error {process.ExitCode}"
								: "Success";

							LauncherConsole.Focus();
							consoleProcess = null;
							UpdateConsoleColour();

							if (processFinishedDelegate != null)
							{
								var finishedDelegate = processFinishedDelegate;
								processFinishedDelegate = null;
								finishedDelegate(process);
							}
						}));
					};
				}

				process.Exited += (sender, e) => process.Dispose();

				process.Start();

				if (consoleAttached)
				{
					consoleProcess = process;
					consoleProcessStartTime = DateTime.Now;

					UpdateConsoleColour();
					LauncherProcessTextBox.Text = $"{(workingDirectory != null ? workingDirectory + "> " : "")}{processFileName} {arguments}";

					process.BeginOutputReadLine();
					process.BeginErrorReadLine();
				}

				processTable.Add(process, processFileName);
				UpdateProcessList();
			}
			catch
			{
				WriteConsole($"FAILED TO EXECUTE: {processFileName} {arguments}", true);

				if (processFinishedDelegate != null)
				{
					var finishedDelegate = processFinishedDelegate;
					processFinishedDelegate = null;
					finishedDelegate(null);
				}
			}
		}

		private void LauncherButtonCancel_Click(object sender, EventArgs e)
		{
			if (LauncherProcessList.SelectedIndex < 0)
			{
				return;
			}

			if (processList[LauncherProcessList.SelectedIndex] is DictionaryEntry entry &&
				entry.Key is Process process)
			{
				process.Kill();
			}
		}

		private void LauncherButtonRunConverter_Click(object sender, EventArgs e)
		{
			LaunchProcess(
				"converter",
				"-nopause",
				null,
				true,
				null);
		}

		private void LauncherButtonRadiant_Click(object sender, EventArgs e)
		{
			string args = (mapName != null)
				? Path.Combine(Launcher.GetMapSourceDirectory(), mapName + ".map")
				: "";

			LaunchProcess(
				"CoDWaWRadiant",
				args,
				null,
				false,
				null);
		}

		private void LauncherButtonEffectsEd_Click(object sender, EventArgs e)
		{
			LaunchProcess(
				"EffectsEd3",
				"",
				null,
				false,
				null);
		}

		private void LauncherButtonAssetManager_Click(object sender, EventArgs e)
		{
			LaunchProcess(
				"asset_manager",
				"",
				null,
				false,
				null);
		}

		private void LauncherButtonAssetViewer_Click(object sender, EventArgs e)
		{
			LaunchProcess(
				"AssetViewer",
				"",
				null,
				false,
				null);
		}

		private void LauncherProcessList_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateStopProcessButton();
		}

		private bool IsMP()
		{
			return Launcher.IsMP(mapName);
		}

		private string GetSourceBsp()
		{
			return Path.Combine(Launcher.GetMapSourceDirectory(), mapName);
		}

		private string GetDestinationBsp()
		{
			var rawDir = Launcher.GetRawMapsDirectory();
			return Path.Combine(rawDir, (IsMP()) ? "mp" : "", mapName);
		}

		private void LauncherButtonCreateMap_Click(object sender, EventArgs e)
		{
			if (new CreateMapForm().ShowDialog() != DialogResult.OK)
			{
				return;
			}

			UpdateMapList();
			EnableMapList();
		}

		private void CompileLevel()
		{
			EnableControls(false);
			UpdateMapSettings();
			CompileLevelBspDelegate(null);
		}

		private void CompileLevelHelper(
			string mapSettingsOption,
			ProcessFinishedDelegate nextStage,
			Process lastProcess,
			string processName,
			string processOptions,
			string workingDirectory)
		{
			bool shouldRun = Launcher.mapSettings.GetBoolean(mapSettingsOption);

			LaunchProcessHelper(
				shouldRun,
				nextStage,
				lastProcess,
				processName,
				processOptions,
				workingDirectory);
		}

		private void CompileLevelHelper(
			string mapSettingsOption,
			ProcessFinishedDelegate nextStage,
			Process lastProcess,
			string processName,
			string processOptions)
		{
			CompileLevelHelper(
				mapSettingsOption,
				nextStage,
				lastProcess,
				processName,
				processOptions,
				null);
		}

		private void CompileLevelBspDelegate(Process lastProcess)
		{
			string args = $"-platform pc -loadFrom \"{GetSourceBsp()}.map\"{Launcher.GetBspOptions()} \"{GetDestinationBsp()}\"";

			CompileLevelHelper("compile_bsp", CompileLevelVisDelegate, lastProcess, "cod2map", args);
		}

		private void CompileLevelVisDelegate(Process lastProcess)
		{
			string args = $"-vis -platform pc \"{GetDestinationBsp()}\"";

			CompileLevelHelper("compile_vis", CompileLevelLightsDelegate, lastProcess, "cod2map", args);
		}

		private void CompileLevelLightsDelegate(Process lastProcess)
		{
			string args = $"-platform pc {Launcher.GetLightOptions()} \"{GetDestinationBsp()}\"";

			CompileLevelHelper("compile_lights", CompileLevelCleanupDelegate, lastProcess, "cod2rad", args);
		}

		private void CompileLevelCleanupDelegate(Process lastProcess)
		{
			string destinationBsp = GetDestinationBsp();
			string sourceBsp = GetSourceBsp();

			// Copy the .lin file back to the source BSP
			Launcher.CopyFileSmart(destinationBsp + ".lin", sourceBsp + ".lin");

			// Delete intermediate compile files
			string[] extensions =
			{
				".lin",
				".map",
				".d3dpoly",
				".vclog",
				".grid"
			};

			foreach (string ext in extensions)
			{
				Launcher.DeleteFile(destinationBsp + ext, false);
			}

			CompileLevelPathsDelegate(lastProcess);
		}

		private void CompileLevelPathsDelegate(Process lastProcess)
		{
			string args = $"allowdupe +set developer 1 +set logfile 2 +set thereisacow 1337 " +
						  $"+set com_introplayed 1 +set r_fullscreen 0 +set fs_usedevdir 1 " +
						  $"+set g_connectpaths 2 +set useFastFile 0 +devmap {mapName}";

			CompileLevelHelper("compile_paths", CompileLevelReflectionsDelegate, lastProcess, Launcher.GetGameTool(IsMP()), args);
		}

		private void CompileLevelReflectionsDelegate(Process lastProcess)
		{
			string args = $"allowdupe +set developer 1 +set logfile 2 +set thereisacow 1337 " +
						  $"+set com_introplayed 1 +set r_fullscreen 0 +set fs_usedevdir 1 +set ui_autoContinue 1 " +
						  $"+set r_reflectionProbeGenerateExit 1 +set sys_smp_allowed 0 +set useFastFile 0 " +
						  $"+set r_fullscreen 0 +set com_hunkMegs 512 +set r_reflectionProbeRegenerateAll 1 " +
						  $"+set r_zFeather 1 +set r_smp_backend_allowed 1 +set r_reflectionProbeGenerate 1 +devmap {mapName}";

			CompileLevelHelper("compile_reflections", CompileLevelBspInfoDelegate, lastProcess, Launcher.GetGameTool(IsMP()), args);
		}

		private void CompileLevelBspInfoDelegate(Process lastProcess)
		{
			string destination = GetDestinationBsp();

			CompileLevelHelper("compile_bspinfo", CompileLevelFastFilesDelegate, lastProcess, "cod2map", $"-info \"{destination}\"");
		}

		private void CompileLevelBuildFastFile(string name, Process lastProcess, LauncherForm.ProcessFinishedDelegate nextStage)
		{
			string modArg = (Launcher.mapSettings.GetBoolean("compile_modenabled"))
				? $"-moddir {Launcher.mapSettings.GetString("compile_modname")} "
				: "";
			string zoneFile = (File.Exists(Launcher.GetLoadZone(mapName))) ? " " + Launcher.GetLoadZone(mapName) : "";
			string args = $"-nopause -language {Launcher.GetLanguage()} {modArg}{name}{zoneFile}";

			CompileLevelHelper("compile_buildffs", nextStage, lastProcess, "linker_pc", args);
		}

		private void CompileLevelFastFilesDelegate(Process lastProcess)
		{
			if (!CheckZoneSourceFiles())
			{
				CompileLevelRunGameDelegate(lastProcess);
				return;
			}

			if (IsMP())
			{
				CompileLevelBuildFastFile(mapName, lastProcess, CompileLevelFastFilesLocalizedDelegate);
			}
			else
			{
				CompileLevelBuildFastFile(mapName, lastProcess, CompileLevelMoveFastFilesDelegate);
			}
		}

		private void CompileLevelFastFilesLocalizedDelegate(Process lastProcess)
		{
			CompileLevelBuildFastFile(
				$"localized_{mapName}",
				lastProcess,
				CompileLevelMoveFastFilesDelegate);
		}

		private void CompileLevelMoveFastFilesDelegate(Process lastProcess)
		{
			string zoneDirectory = Launcher.GetZoneDirectory();
			string targetDirectory = (Launcher.mapSettings.GetBoolean("compile_modenabled"))
				? Launcher.GetModDirectory(Launcher.mapSettings.GetString("compile_modname"))
				: Path.Combine(Launcher.GetUsermapsDirectory(), mapName);

			string ffFile = mapName + ".ff";
			string ffLoadFile = mapName + "_load.ff";
			string localizedFFFile = "localized_" + ffFile;

			Launcher.MoveFile(Path.Combine(zoneDirectory, ffFile), Path.Combine(targetDirectory, ffFile));
			Launcher.MoveFile(Path.Combine(zoneDirectory, localizedFFFile), Path.Combine(targetDirectory, localizedFFFile));
			Launcher.MoveFile(Path.Combine(zoneDirectory, ffLoadFile), Path.Combine(targetDirectory, ffLoadFile));

			Launcher.Publish();
			CompileLevelRunGameDelegate(lastProcess);
		}

		private void CompileLevelRunGameDelegate(Process lastProcess)
		{
			string fsGame = (Launcher.mapSettings.GetBoolean("compile_modenabled"))
				? "mods/" + Launcher.mapSettings.GetString("compile_modname")
				: "raw";

			string extraOptions = (IsMP()) ? "+set sv_pure 0 +set g_gametype tdm " : "";
			string arguments = $"+set useFastFile 1 +set fs_usedevdir 1 +set logfile 2 +set thereisacow 1337 +set com_introplayed 1 {extraOptions}+devmap {mapName} +set fs_game {fsGame} ";

			CompileLevelHelper("compile_runafter", CompileLevelFinished, lastProcess, Launcher.GetGameApplication(IsMP()), arguments);
		}

		private void CompileLevelFinished(Process lastProcess)
		{
			EnableControls(true);
		}

		private void TestProcessFinishedDelegate(Process p)
		{
			LaunchProcess("help.exe", "", null, true, null);
		}

		private void LauncherButtonTest_Click(object sender, EventArgs e)
		{
			LaunchProcess("cmd.exe", "/c dir c:\\", null, true, TestProcessFinishedDelegate);
		}

		private void LauncherRunGameCustomCommandLineTextBox_TextChanged(object sender, EventArgs e)
		{
			// Empty
		}

		private void LauncherRunGameCustomCommandLineTextBox_Validating(object sender, CancelEventArgs e)
		{
			// Empty
		}

		private void EnableControls(bool enabled)
		{
			EnableControls(enabled, null);
		}

		private void EnableControls(bool enabled, TabPage onlyForTabPage)
		{
			TabPage[] tabPages =
			{
				LauncherTabCompileLevel,
				LauncherTabModBuilder,
				LauncherTabRunGame
			};

			foreach (var tabPage in tabPages)
			{
				if (onlyForTabPage == null || onlyForTabPage == tabPage)
				{
					foreach (Control control in tabPage.Controls)
					{
						control.Enabled = enabled;
					}
				}
			}

			// Enable mod-specific map combobox only if controls are enabled and the checkbox is checked
			if (enabled)
			{
				LauncherModSpecificMapComboBox.Enabled = LauncherModSpecificMapCheckBox.Checked;
			}
		}

		private void UpdateMapSettings()
		{
			// Save current map settings if a map is selected
			if (mapName != null)
			{
				Launcher.mapSettings.SetBoolean("compile_bsp", LauncherCompileBSPCheckBox.Checked);
				Launcher.mapSettings.SetBoolean("compile_lights", LauncherCompileLightsCheckBox.Checked);
				Launcher.mapSettings.SetBoolean("compile_vis", LauncherCompileVisCheckBox.Checked);
				Launcher.mapSettings.SetBoolean("compile_paths", LauncherConnectPathsCheckBox.Checked);
				Launcher.mapSettings.SetBoolean("compile_reflections", LauncherCompileReflectionsCheckBox.Checked);
				Launcher.mapSettings.SetBoolean("compile_buildffs", LauncherBuildFastFilesCheckBox.Checked);
				Launcher.mapSettings.SetBoolean("compile_bspinfo", LauncherBspInfoCheckBox.Checked);
				Launcher.mapSettings.SetBoolean("compile_runafter", LauncherRunMapAfterCompileCheckBox.Checked);
				Launcher.mapSettings.SetBoolean("compile_useruntab", LauncherUseRunGameTypeOptionsCheckBox.Checked);
				Launcher.mapSettings.SetString("compile_runoptions", LauncherCustomRunOptionsTextBox.Text);
				Launcher.mapSettings.SetBoolean("compile_modenabled", LauncherModSpecificMapCheckBox.Checked);
				Launcher.mapSettings.SetString("compile_modname", LauncherModSpecificMapComboBox.Text);
				Launcher.mapSettings.SetBoolean("compile_collectdots", LauncherGridCollectDotsCheckBox.Checked);

				Launcher.SaveMapSettings(mapName, new Hashtable(Launcher.mapSettings.Get()));
				mapName = null;
			}

			// Load settings for the currently selected map
			if (LauncherMapList.SelectedItem == null)
			{
				return;
			}

			mapName = LauncherMapList.SelectedItem.ToString();

			var loadedSettings = (Launcher.LoadMapSettings(mapName))
				.Cast<DictionaryEntry>()
				.ToDictionary(de => (string)de.Key, de => (string)de.Value);

			Launcher.mapSettings.Set(loadedSettings);

			// Update UI checkboxes and textboxes
			LauncherCompileBSPCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_bsp");
			LauncherCompileLightsCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_lights");
			LauncherCompileVisCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_vis");
			LauncherConnectPathsCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_paths");
			LauncherCompileReflectionsCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_reflections");
			LauncherBuildFastFilesCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_buildffs");
			LauncherBspInfoCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_bspinfo");
			LauncherRunMapAfterCompileCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_runafter");
			LauncherUseRunGameTypeOptionsCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_useruntab");
			LauncherCustomRunOptionsTextBox.Text = Launcher.mapSettings.GetString("compile_runoptions");
			LauncherModSpecificMapCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_modenabled");
			LauncherModSpecificMapComboBox.Text = Launcher.mapSettings.GetString("compile_modname");
			LauncherGridCollectDotsCheckBox.Checked = Launcher.mapSettings.GetBoolean("compile_collectdots");
		}

		private void LauncherCompileLightsButton_Click(object sender, EventArgs e)
		{
			using (var bspOptionsForm = new BspOptionsForm())
			{
				bspOptionsForm.ShowDialog();
			}
		}

		private void LauncherCompileBSPButton_Click(object sender, EventArgs e)
		{
			using (var lightOptionsForm = new LightOptionsForm())
			{
				lightOptionsForm.ShowDialog();
			}
		}

		private void LauncherCompileLevelButton_Click(object sender, EventArgs e)
		{
			CompileLevel();
		}

		private string GetGameOptions()
		{
			// Determine the game directory
			string gameDir = (LauncherRunGameModComboBox.SelectedIndex > 0)
				? "mods/" + LauncherRunGameModComboBox.Text
				: "raw";

			// Build the full command line options
			string options = $"+set fs_game {gameDir} {FormatDvars()} {LauncherRunGameCustomCommandLineTextBox.Text} ";
			return options;
		}

		private void LauncherTimer_Tick(object sender, EventArgs e)
		{
			// Update elapsed time for the console process
			if (consoleProcess != null)
			{
				TimeSpan elapsed = DateTime.Now - consoleProcessStartTime;
				LauncherProcessTimeElapsedTextBox.Text = elapsed.ToString(@"hh\:mm\:ss");
			}

			// Update the command line text box if it differs from the current game options
			string gameOptions = GetGameOptions();
			if (LauncherRunGameCommandLineTextBox.Text != gameOptions)
			{
				LauncherRunGameCommandLineTextBox.Text = gameOptions;
			}
		}

		private bool CheckZoneSourceFiles()
		{
			string zoneFile = Launcher.GetZoneSourceFile(mapName);

			if (File.Exists(zoneFile))
			{
				return true;
			}

			var result = MessageBox.Show(
				$"There are no zone files for {mapName}. Would you like to create them?",
				"Warning",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Exclamation);

			if (result == DialogResult.No)
			{
				return false;
			}

			Launcher.CreateZoneSourceFiles(mapName);
			return true;
		}

		private void EnableMapList()
		{
			bool hasSelection = LauncherMapList.SelectedItem != null;

			// Enable or disable compile button based on selection
			LauncherCompileLevelButton.Enabled = hasSelection;

			// Enable or disable other controls on the Compile Level tab
			EnableControls(hasSelection, LauncherTabCompileLevel);

			// Map list and create map button are always enabled
			LauncherMapList.Enabled = true;
			LauncherCreateMapButton.Enabled = true;
		}

		private void LauncherMapList_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateMapSettings();
			EnableMapList();
		}

		private void LauncherMapList_DoubleClick(object sender, EventArgs e)
		{
			// Clear the selected map when double-clicked
			LauncherMapList.SelectedItem = null;
		}

		private void ModBuildStart()
		{
			EnableControls(false);
			ModBuildSoundDelegate(null);
		}

		private void ModBuildSoundDelegate(Process lastProcess)
		{
			bool shouldRun = LauncherModBuildSoundsCheckBox.Checked;
			string arguments = "-pc -ignore_orphans " + ((LauncherModVerboseCheckBox.Checked) ? "-verbose " : "");

			LaunchProcessHelper(
				shouldRun,
				new ProcessFinishedDelegate(ModBuildFastFileDelegate),
				lastProcess,
				"MODSound",
				arguments);
		}

		private void ModBuildFastFileDelegate(Process lastProcess)
		{
			bool shouldRun = LauncherModBuildFastFilesCheckBox.Checked;
			if (shouldRun)
			{
				string sourceCsv = Path.Combine(Launcher.GetModDirectory(modName), "mod.csv");
				string destinationCsv = Path.Combine(Launcher.GetZoneSourceDirectory(), "mod.csv");
				Launcher.CopyFileSmart(sourceCsv, destinationCsv);
			}

			string arguments = $"-nopause -language {Launcher.GetLanguage()} -moddir {modName} mod";

			LaunchProcessHelper(
				shouldRun,
				new ProcessFinishedDelegate(ModBuildMoveModFastFileDelegate),
				lastProcess,
				"linker_pc",
				arguments);
		}

		private void ModBuildMoveModFastFileDelegate(Process lastProcess)
		{
			if (LauncherModBuildFastFilesCheckBox.Checked)
			{
				string sourcePath = Path.Combine(Launcher.GetZoneDirectory(), "mod.ff");
				string destinationPath = Path.Combine(Launcher.GetModDirectory(modName), "mod.ff");

				Launcher.MoveFile(sourcePath, destinationPath);
			}

			// Continue with IWD file build
			ModBuildIwdFileDelegate(lastProcess);
		}

		private void ModBuildIwdFileDelegate(Process lastProcess)
		{
			string modDirectory = Launcher.GetModDirectory(modName);
			string iwdFilePath = Path.Combine(modDirectory, modName + ".iwd");
			string filesListPath = Path.Combine(modDirectory, modName + ".files");

			// Delete existing IWD file if the checkbox is checked
			if (LauncherModBuildIwdFileCheckBox.Checked)
			{
				Launcher.DeleteFile(iwdFilePath, false);
			}

			// Build the IWD archive using 7-Zip
			bool shouldRun = LauncherModBuildIwdFileCheckBox.Checked;
			string arguments = $"a \"{iwdFilePath}\" -tzip -r \"@{filesListPath}\"";

			LaunchProcessHelper(
				shouldRun,
				new ProcessFinishedDelegate(ModBuildFinishedDelegate),
				lastProcess,
				"7za",
				arguments,
				modDirectory);
		}

		private void ModBuildFinishedDelegate(Process lastProcess)
		{
			Launcher.Publish();
			EnableControls(true);
		}

		private void LauncherModBuildButton_Click(object sender, EventArgs e)
		{
			LauncherModComboBoxApplySettings();
			ModBuildStart();
		}

		private void AddFilesToTreeView(string directoryPath, TreeNodeCollection nodes, bool firstTime)
		{
			TreeNode currentNode = null;

			// If not the first level, add a directory node
			if (!firstTime)
			{
				currentNode = nodes.Add(new DirectoryInfo(directoryPath).Name);
				nodes = currentNode.Nodes;
			}

			// Recursively add subdirectories
			foreach (var subDir in new DirectoryInfo(directoryPath).GetDirectories())
			{
				AddFilesToTreeView(Path.Combine(directoryPath, subDir.Name), nodes, false);
			}

			// Add files, excluding certain extensions
			foreach (var file in new DirectoryInfo(directoryPath).GetFiles())
			{
				string ext = file.Extension.ToLower();
				if (ext != ".ff" && ext != ".iwd" && ext != ".files")
				{
					var fileNode = nodes.Add(file.Name);
					fileNode.ForeColor = Color.Blue;
					fileNode.Tag = file;
				}
			}

			// Handle empty directories
			if (currentNode != null)
			{
				if (currentNode.Nodes.Count > 0)
				{
					currentNode.ExpandAll();
				}
				else
				{
					currentNode.Remove();
				}
			}
		}

		private void LauncherModComboBoxApplySettings()
		{
			LauncherIwdFileTreeBeginUpdate();

			// Save current mod settings if a mod is already selected
			if (!string.IsNullOrEmpty(modName))
			{
				string modDirectory = Launcher.GetModDirectory(modName);
				string filesPath = Path.Combine(modDirectory, modName + ".files");
				string csvPath = Path.Combine(modDirectory, "mod.csv");

				Launcher.SaveTextFile(filesPath, Launcher.HashTableToStringArray(TreeViewToHashTable(LauncherIwdFileTree.Nodes)));
				Launcher.SaveTextFile(csvPath, LauncherFastFileCsvTextBox.Lines);
			}

			// Load new mod settings if a mod is selected in the combo box
			if (LauncherModComboBox.SelectedItem != null)
			{
				modName = LauncherModComboBox.SelectedItem.ToString();
				string modDirectory = Launcher.GetModDirectory(modName);
				string filesPath = Path.Combine(modDirectory, modName + ".files");
				string csvPath = Path.Combine(modDirectory, "mod.csv");

				// Clear existing TreeView nodes
				LauncherIwdFileTree.Nodes.Clear();

				AddFilesToTreeView(modDirectory, LauncherIwdFileTree.Nodes, true);
				HashTableToTreeView(Launcher.StringArrayToHashTable(Launcher.LoadTextFile(filesPath)), LauncherIwdFileTree.Nodes);

				LauncherFastFileCsvTextBox.Lines = Launcher.LoadTextFile(csvPath);
			}

			LauncherIwdFileTreeEndUpdate();
		}

		private void LauncherModComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			LauncherModComboBoxApplySettings();
		}

		private void LauncherIwdFileTree_DoubleClick(object sender, EventArgs e)
		{
			var selectedNode = LauncherIwdFileTree.SelectedNode;
			if (selectedNode == null)
			{
				return;
			}

			// Build the full path to the selected file in the mod directory
			string filePath = Path.Combine(Launcher.GetModDirectory(modName), selectedNode.FullPath);

			// Launch the file using the default associated application
			var process = new Process
			{
				StartInfo =
				{
					FileName = filePath,
					ErrorDialog = true
				}
			};
			process.Start();
		}

		private void TreeViewToHashTable(TreeNodeCollection nodes, Hashtable hashTable)
		{
			if (nodes == null)
			{
				return;
			}

			foreach (TreeNode node in nodes)
			{
				if (node.Checked && node.Tag != null)
				{
					// Add the node's full path to the hash table
					hashTable[node.FullPath] = null;
				}
				else
				{
					// Remove the node's full path if it exists
					hashTable.Remove(node.FullPath);
				}
				TreeViewToHashTable(node.Nodes, hashTable);
			}
		}


		private Hashtable TreeViewToHashTable(TreeNodeCollection nodes)
		{
			var hashTable = new Hashtable();
			TreeViewToHashTable(nodes, hashTable);
			return hashTable;
		}

		private void HashTableToTreeView(Hashtable hashTable, TreeNodeCollection nodes)
		{
			if (nodes == null)
			{
				return;
			}

			foreach (TreeNode node in nodes)
			{
				// If the node's full path exists in the hash table, check it and propagate up
				if (hashTable.Contains(node.FullPath))
				{
					node.Checked = true;
					RecursiveCheckNodesUp(node, true);
				}
				HashTableToTreeView(hashTable, node.Nodes);
			}
		}


		private void RecursiveCheckNodesDown(TreeNodeCollection nodes, bool isChecked)
		{
			if (nodes == null)
			{
				return;
			}

			foreach (TreeNode node in nodes)
			{
				// Set the node's checked state
				node.Checked = isChecked;

				RecursiveCheckNodesDown(node.Nodes, isChecked);
			}
		}


		private void RecursiveCheckNodesUp(TreeNode node, bool isChecked)
		{
			if (node == null)
			{
				return;
			}

			// Set the current node's checked state
			node.Checked = isChecked;

			RecursiveCheckNodesUp(node.Parent, isChecked);
		}


		private void LauncherIwdFileTreeBeginUpdate()
		{
			LauncherIwdFileTree.BeginUpdate();
			LauncherIwdFileTree.AfterCheck -= LauncherIwdFileTree_AfterCheck;
		}


		private void LauncherIwdFileTreeEndUpdate()
		{
			LauncherIwdFileTree.AfterCheck += LauncherIwdFileTree_AfterCheck;
			LauncherIwdFileTree.EndUpdate();
		}


		private void LauncherIwdFileTree_AfterCheck(object sender, TreeViewEventArgs e)
		{
			LauncherIwdFileTreeBeginUpdate();
			RecursiveCheckNodesDown(e.Node.Nodes, e.Node.Checked);
			// If the node is checked, propagate the check state up to parent nodes
			if (e.Node.Checked)
			{
				RecursiveCheckNodesUp(e.Node.Parent, e.Node.Checked);
			}
			LauncherIwdFileTreeEndUpdate();
		}

		private void CheckBox1_CheckedChanged(object sender, EventArgs e)
		{
			LauncherModSpecificMapComboBox.Enabled = LauncherModSpecificMapCheckBox.Checked;
		}

		private void LauncherClearConsoleButton_Click(object sender, EventArgs e)
		{
			LauncherConsole.Clear();
		}

		private void LauncherGameOptionsFlowPanel_Click(object sender, EventArgs e)
		{
			if (!(e is MouseEventArgs mouseEventArgs) || mouseEventArgs.Button != MouseButtons.Right)
			{
				return;
			}

			if (!(sender is Control control))
			{
				return;
			}

			// Create context menu for DVar options
			var contextMenu = new ContextMenuStrip();
			contextMenu.Items.Add("Edit dvar");
			contextMenu.Items.Add("Remove dvar");
			contextMenu.Items.Add("Add new dvar");
			contextMenu.Items.Add("Duplicate dvar");

			// Show the menu at the cursor location
			contextMenu.Show(control.PointToScreen(mouseEventArgs.Location));
		}

		private void LauncherWikiLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://web.archive.org/web/20090228020203/http://wiki.treyarch.com/wiki/Main_Page");
		}

		private void LauncherRunGameButton_Click(object sender, EventArgs e)
		{
			// Add any custom DVar text that isn't already in the combo box items
			foreach (ComboBox dvarComboBox in dvarComboBoxes)
			{
				string customText = dvarComboBox.Text.Trim();
				if (!string.IsNullOrEmpty(customText))
				{
					bool exists = false;
					foreach (string item in dvarComboBox.Items)
					{
						if (string.Equals(customText, item, StringComparison.OrdinalIgnoreCase))
						{
							exists = true;
							break;
						}
					}

					if (!exists)
					{
						dvarComboBox.Items.Add(customText);
					}
				}
			}

			// Launch the game with the selected options
			string gameApplication = Launcher.GetGameApplication(!LauncherRunGameTypeRadioButton.Checked);
			string gameOptions = GetGameOptions();

			LaunchProcess(
				processFileName: gameApplication,
				arguments: gameOptions,
				workingDirectory: null,
				consoleAttached: false,
				theProcessFinishedDelegate: null);
		}

		private void LauncherDeleteMap_Click(object sender, EventArgs e)
		{
			// Get all files related to the selected map
			string[] mapFiles = Launcher.GetMapFiles(mapName);

			// Ask for user confirmation
			var confirmResult = MessageBox.Show(
				"The following files would be deleted:\n\n" + Launcher.StringArrayToString(mapFiles),
				"Are you sure you want to delete these files?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Exclamation);

			if (confirmResult != DialogResult.Yes)
			{
				return;
			}

			// Delete each file
			foreach (string file in mapFiles)
			{
				Launcher.DeleteFile(file);
			}

			// Check for any remaining files
			string[] remainingFiles = Launcher.GetMapFiles(mapName);
			if (remainingFiles.Length > 0)
			{
				MessageBox.Show(
					"Could not delete the following files:\n\n" + Launcher.StringArrayToString(remainingFiles),
					"Error deleting files",
					MessageBoxButtons.OK,
					MessageBoxIcon.Hand);
			}

			// Refresh the map list UI
			UpdateMapList();
			EnableMapList();
		}

		private void LauncherAboutLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string aboutText =
				"Original launcher by\n" +
				"    Mike Denny\n\n" +
				"PC Programming Lead\n" +
				"    Krassimir Touevsky\n\n" +
				"PC Programming Team\n" +
				"    Yanbing Chen\n" +
				"    Juan Morelli\n" +
				"    Ewan Oughton\n" +
				"    Valeria Pelova\n" +
				"    Dimiter \"malkia\" Stanev\n\n" +
				"PC Production Team\n" +
				"    Adam Saslow\n" +
				"    Cesar Stastny\n\n" +
				"PC Modding Team\n" +
				"    Tony Kramer\n" +
				"    Gavin Niebel\n" +
				"    Alex 'Sparks' Romo\n\n" +
				"Decompiled by\n" +
				"    hindercanrun";

			MessageBox.Show(aboutText, "About Launcher");
		}


		private void LauncherForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (processTable.Count > 0)
			{
				var result = MessageBox.Show(
					"But there are still processes running!\n\n" +
					"Do you want to close them, or cancel exiting from the application?",
					"Application will exit!",
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Exclamation);

				switch (result)
				{
					case DialogResult.Cancel:
						e.Cancel = true;
						return;

					case DialogResult.Yes:
						// Kill all running processes
						foreach (DictionaryEntry entry in processTable)
						{
							try
							{
								((Process)entry.Key).Kill();
							}
							catch
							{
								// Ignore failures; process may already be exited
							}
						}
						break;

					default: // DialogResult.No
						// List all active processes for the user
						var activeProcesses = new string[processTable.Count];
						int index = 0;

						foreach (DictionaryEntry entry in processTable)
						{
							try
							{
								activeProcesses[index] = ((Process)entry.Key).MainModule.FileName;
							}
							catch
							{
								activeProcesses[index] = entry.Value as string ?? "<unknown>";
							}

							index++;
						}

						if (activeProcesses.Length > 0)
						{
							string message = "The following processes are still active:\n\n" +
											 Launcher.StringArrayToString(activeProcesses) +
											 "\nPlease close them if necessary using the Task Manager, or a similar program!";
							MessageBox.Show(message, "Note before exiting the application", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
						break;
				}
			}

			UpdateMapSettings();
		}

		private void LauncherMapFilesSystemWatcher_Changed(object sender, FileSystemEventArgs e)
		{
			UpdateMapList();
		}

		private void LauncherMapFilesSystemWatcher_Created(object sender, FileSystemEventArgs e)
		{
			UpdateMapList();
		}

		private void LauncherMapFilesSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
		{
			UpdateMapList();
		}

		private void LauncherMapFilesSystemWatcher_Renamed(object sender, RenamedEventArgs e)
		{
			UpdateMapList();
		}

		private void LauncherModsDirectorySystemWatcher_Changed(object sender, FileSystemEventArgs e)
		{
			UpdateModList();
		}

		private void LauncherModsDirectorySystemWatcher_Created(object sender, FileSystemEventArgs e)
		{
			UpdateModList();
		}

		private void LauncherModsDirectorySystemWatcher_Deleted(object sender, FileSystemEventArgs e)
		{
			UpdateModList();
		}

		private void LauncherModsDirectorySystemWatcher_Renamed(object sender, RenamedEventArgs e)
		{
			UpdateModList();
		}

		private void BuildGridDelegate(int r_vc_makelog)
		{
			EnableControls(false);

			string fileName = mapName + ".grid";
			string sourcePath = Path.Combine(Launcher.GetMapSourceDirectory(), fileName);
			string destPath = Path.Combine(Launcher.GetRawMapsDirectory(), (IsMP()) ? "mp" : "", fileName);

			// Copy the grid file to the raw maps directory
			Launcher.CopyFile(sourcePath, destPath);

			// Build the process arguments
			string gameApp = Launcher.GetGameApplication(IsMP());
			string devMapArg = $"+set developer 1 +set logfile 2 +set r_vc_makelog {r_vc_makelog} " +
							   $"+set r_vc_showlog 16 +set r_cullxmodel {(Launcher.mapSettings.GetBoolean("compile_collectdots") ? "0" : "1")} " +
							   $"+set thereisacow 1337 +set com_introplayed 1 +set fs_game raw +set fs_usedevdir 1 +devmap {mapName}";

			// Launch the game process to build the grid
			LaunchProcessHelper(
				shouldRun: true,
				nextStage: BuildGridFinishedDelegate,
				lastProcess: null,
				processName: gameApp,
				processOptions: devMapArg);
		}

		private void BuildGridFinishedDelegate(Process lastProcess)
		{
			string fileName = mapName + ".grid";
			string sourcePath = Path.Combine(Launcher.GetRawMapsDirectory(), (IsMP()) ? "mp" : "", fileName);
			string destPath = Path.Combine(Launcher.GetMapSourceDirectory(), fileName);

			// Move the grid file back to the source directory
			Launcher.MoveFile(sourcePath, destPath);

			EnableControls(true);
		}

		private void LauncherGridMakeNewButton_Click(object sender, EventArgs e)
		{
			BuildGridDelegate(1);
		}

		private void LauncherGridEditExistingButton_Click(object sender, EventArgs e)
		{
			BuildGridDelegate(2);
		}

		private void InitializeComponent()
		{
			components = (IContainer) new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (LauncherForm));

			LauncherConsole = new RichTextBox();
			LauncherSplitter = new SplitContainer();
			LauncherApplicationsGroupBox = new GroupBox();
			LauncherClearConsoleButton = new Button();
			LauncherButtonTest = new Button();
			LauncherButtonAssetViewer = new Button();
			LauncherButtonRunConverter = new Button();
			LauncherButtonAssetManager = new Button();
			LauncherButtonEffectsEd = new Button();
			LauncherButtonRadiant = new Button();
			LauncherTab = new TabControl();
			LauncherTabCompileLevel = new TabPage();
			LauncherDeleteMapButton = new Button();
			LauncherCompileLevelOptionsGroupBox = new GroupBox();
			LauncherGridFileGroupBox = new GroupBox();
			LauncherGridEditExistingButton = new Button();
			LauncherGridMakeNewButton = new Button();
			LauncherGridCollectDotsCheckBox = new CheckBox();
			LauncherModSpecificMapComboBox = new ComboBox();
			LauncherModSpecificMapCheckBox = new CheckBox();
			LauncherCustomRunOptionsLabel = new Label();
			LauncherCustomRunOptionsTextBox = new TextBox();
			LauncherCompileLevelOptionsSplitterGroupBox = new GroupBox();
			LauncherCompileLevelButton = new Button();
			LauncherCompileLightsButton = new Button();
			LauncherCompileBSPButton = new Button();
			LauncherUseRunGameTypeOptionsCheckBox = new CheckBox();
			LauncherRunMapAfterCompileCheckBox = new CheckBox();
			LauncherBspInfoCheckBox = new CheckBox();
			LauncherBuildFastFilesCheckBox = new CheckBox();
			LauncherCompileReflectionsCheckBox = new CheckBox();
			LauncherConnectPathsCheckBox = new CheckBox();
			LauncherCompileVisCheckBox = new CheckBox();
			LauncherCompileLightsCheckBox = new CheckBox();
			LauncherCompileBSPCheckBox = new CheckBox();
			LauncherCreateMapButton = new Button();
			LauncherMapList = new ListBox();
			LauncherTabModBuilder = new TabPage();
			LauncherIwdFileGroupBox = new GroupBox();
			LauncherIwdFileTree = new TreeView();
			LauncherFastFileCsvGroupBox = new GroupBox();
			LauncherFastFileCsvTextBox = new RichTextBox();
			LauncherModGroupBox = new GroupBox();
			LauncherModBuildButton = new Button();
			LauncherModBuildSoundsCheckBox = new CheckBox();
			LauncherModVerboseCheckBox = new CheckBox();
			LauncherModBuildIwdFileCheckBox = new CheckBox();
			LauncherModBuildFastFilesCheckBox = new CheckBox();
			LauncherModComboBox = new ComboBox();
			LauncherTabRunGame = new TabPage();
			LauncherGameOptionsPanel = new Panel();
			LauncherRunGameButton = new Button();
			LauncherRunGameCustomCommandLineGroupBox = new GroupBox();
			LauncherRunGameCustomCommandLineTextBox = new TextBox();
			LauncherRunGameCommandLineGroupBox = new GroupBox();
			LauncherRunGameCommandLineTextBox = new TextBox();
			LauncherRunGameModGroupBox = new GroupBox();
			LauncherRunGameModComboBox = new ComboBox();
			LauncherRunGameExeTypeGroupBox = new GroupBox();
			LauncherRunGameExeTypeMpRadioButton = new RadioButton();
			LauncherRunGameTypeRadioButton = new RadioButton();
			LauncherProcessTimeElapsedTextBox = new TextBox();
			LauncherProcessTextBox = new TextBox();
			LauncherProcessGroupBox = new GroupBox();
			LauncherButtonCancel = new Button();
			LauncherProcessList = new ListBox();
			LauncherTimer = new System.Windows.Forms.Timer(components);
			LauncherWikiLabel = new LinkLabel();
			LauncherAboutLabel = new LinkLabel();
			LauncherMapFilesSystemWatcher = new FileSystemWatcher();
			LauncherModsDirectorySystemWatcher = new FileSystemWatcher();

			LauncherSplitter.Panel1.SuspendLayout();
			LauncherSplitter.Panel2.SuspendLayout();
			LauncherSplitter.SuspendLayout();
			LauncherApplicationsGroupBox.SuspendLayout();
			LauncherTab.SuspendLayout();
			LauncherTabCompileLevel.SuspendLayout();
			LauncherCompileLevelOptionsGroupBox.SuspendLayout();
			LauncherGridFileGroupBox.SuspendLayout();
			LauncherTabModBuilder.SuspendLayout();
			LauncherIwdFileGroupBox.SuspendLayout();
			LauncherFastFileCsvGroupBox.SuspendLayout();
			LauncherModGroupBox.SuspendLayout();
			LauncherTabRunGame.SuspendLayout();
			LauncherRunGameCustomCommandLineGroupBox.SuspendLayout();
			LauncherRunGameCommandLineGroupBox.SuspendLayout();
			LauncherRunGameModGroupBox.SuspendLayout();
			LauncherRunGameExeTypeGroupBox.SuspendLayout();
			LauncherProcessGroupBox.SuspendLayout();
			LauncherMapFilesSystemWatcher.BeginInit();
			LauncherModsDirectorySystemWatcher.BeginInit();

			SuspendLayout();
			LauncherConsole.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LauncherConsole.Font = new Font("Courier New", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
			LauncherConsole.Location = new Point(149, 3);
			LauncherConsole.Name = "LauncherConsole";
			LauncherConsole.Size = new Size(662, 229);
			LauncherConsole.TabIndex = 0;
			LauncherConsole.Text = "";
			LauncherConsole.WordWrap = false;

			LauncherSplitter.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LauncherSplitter.BackColor = SystemColors.Control;
			LauncherSplitter.FixedPanel = FixedPanel.Panel1;
			LauncherSplitter.Location = new Point(12, 12);
			LauncherSplitter.Name = "LauncherSplitter";
			LauncherSplitter.Orientation = Orientation.Horizontal;
			LauncherSplitter.Panel1.Controls.Add((Control) LauncherApplicationsGroupBox);
			LauncherSplitter.Panel1.Controls.Add((Control) LauncherTab);
			LauncherSplitter.Panel1MinSize = 380;
			LauncherSplitter.Panel2.Controls.Add((Control) LauncherProcessTimeElapsedTextBox);
			LauncherSplitter.Panel2.Controls.Add((Control) LauncherProcessTextBox);
			LauncherSplitter.Panel2.Controls.Add((Control) LauncherConsole);
			LauncherSplitter.Panel2.Controls.Add((Control) LauncherProcessGroupBox);
			LauncherSplitter.Panel2MinSize = 100;
			LauncherSplitter.Size = new Size(814, 640);
			LauncherSplitter.SplitterDistance = 380;
			LauncherSplitter.TabIndex = 1;

			LauncherApplicationsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			LauncherApplicationsGroupBox.Controls.Add((Control) LauncherClearConsoleButton);
			LauncherApplicationsGroupBox.Controls.Add((Control) LauncherButtonTest);
			LauncherApplicationsGroupBox.Controls.Add((Control) LauncherButtonAssetViewer);
			LauncherApplicationsGroupBox.Controls.Add((Control) LauncherButtonRunConverter);
			LauncherApplicationsGroupBox.Controls.Add((Control) LauncherButtonAssetManager);
			LauncherApplicationsGroupBox.Controls.Add((Control) LauncherButtonEffectsEd);
			LauncherApplicationsGroupBox.Controls.Add((Control) LauncherButtonRadiant);
			LauncherApplicationsGroupBox.Location = new Point(4, 4);
			LauncherApplicationsGroupBox.Name = "LauncherApplicationsGroupBox";
			LauncherApplicationsGroupBox.Size = new Size(139, 373);
			LauncherApplicationsGroupBox.TabIndex = 1;
			LauncherApplicationsGroupBox.TabStop = false;
			LauncherApplicationsGroupBox.Text = "Applications";

			LauncherClearConsoleButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LauncherClearConsoleButton.Location = new Point(6, 335);
			LauncherClearConsoleButton.Name = "LauncherClearConsoleButton";
			LauncherClearConsoleButton.Size = new Size((int) sbyte.MaxValue, 32);
			LauncherClearConsoleButton.TabIndex = 5;
			LauncherClearConsoleButton.Text = "Clear Console";
			LauncherClearConsoleButton.UseVisualStyleBackColor = true;
			LauncherClearConsoleButton.Click += new EventHandler(LauncherClearConsoleButton_Click);

			LauncherButtonTest.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LauncherButtonTest.Location = new Point(5, 297);
			LauncherButtonTest.Name = "LauncherButtonTest";
			LauncherButtonTest.Size = new Size(128, 32);
			LauncherButtonTest.TabIndex = 6;
			LauncherButtonTest.Text = "Test (devonly)";
			LauncherButtonTest.UseVisualStyleBackColor = true;
			LauncherButtonTest.Visible = false;
			LauncherButtonTest.Click += new EventHandler(LauncherButtonTest_Click);

			LauncherButtonAssetViewer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			LauncherButtonAssetViewer.Location = new Point(5, 133);
			LauncherButtonAssetViewer.Name = "LauncherButtonAssetViewer";
			LauncherButtonAssetViewer.Size = new Size(128, 32);
			LauncherButtonAssetViewer.TabIndex = 5;
			LauncherButtonAssetViewer.Text = "Asset Viewer";
			LauncherButtonAssetViewer.UseVisualStyleBackColor = true;
			LauncherButtonAssetViewer.Click += new EventHandler(LauncherButtonAssetViewer_Click);

			LauncherButtonRunConverter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			LauncherButtonRunConverter.Location = new Point(5, 171);
			LauncherButtonRunConverter.Name = "LauncherButtonRunConverter";
			LauncherButtonRunConverter.Size = new Size(128, 32);
			LauncherButtonRunConverter.TabIndex = 3;
			LauncherButtonRunConverter.Text = "Converter";
			LauncherButtonRunConverter.UseVisualStyleBackColor = true;
			LauncherButtonRunConverter.Click += new EventHandler(LauncherButtonRunConverter_Click);

			LauncherButtonAssetManager.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			LauncherButtonAssetManager.Location = new Point(6, 95);
			LauncherButtonAssetManager.Name = "LauncherButtonAssetManager";
			LauncherButtonAssetManager.Size = new Size(128, 32);
			LauncherButtonAssetManager.TabIndex = 2;
			LauncherButtonAssetManager.Text = "Asset Manager";
			LauncherButtonAssetManager.UseVisualStyleBackColor = true;
			LauncherButtonAssetManager.Click += new EventHandler(LauncherButtonAssetManager_Click);

			LauncherButtonEffectsEd.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			LauncherButtonEffectsEd.Location = new Point(6, 57);
			LauncherButtonEffectsEd.Name = "LauncherButtonEffectsEd";
			LauncherButtonEffectsEd.Size = new Size(128, 32);
			LauncherButtonEffectsEd.TabIndex = 1;
			LauncherButtonEffectsEd.Text = "Effects Editor";
			LauncherButtonEffectsEd.UseVisualStyleBackColor = true;
			LauncherButtonEffectsEd.Click += new EventHandler(LauncherButtonEffectsEd_Click);

			LauncherButtonRadiant.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			LauncherButtonRadiant.Location = new Point(6, 19);
			LauncherButtonRadiant.Name = "LauncherButtonRadiant";
			LauncherButtonRadiant.Size = new Size(128, 32);
			LauncherButtonRadiant.TabIndex = 0;
			LauncherButtonRadiant.Text = "Radiant";
			LauncherButtonRadiant.UseVisualStyleBackColor = true;
			LauncherButtonRadiant.Click += new EventHandler(LauncherButtonRadiant_Click);

			LauncherTab.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LauncherTab.Controls.Add((Control) LauncherTabCompileLevel);
			LauncherTab.Controls.Add((Control) LauncherTabModBuilder);
			LauncherTab.Controls.Add((Control) LauncherTabRunGame);
			LauncherTab.Location = new Point(149, 1);
			LauncherTab.Name = "LauncherTab";
			LauncherTab.SelectedIndex = 0;
			LauncherTab.Size = new Size(664, 380);
			LauncherTab.TabIndex = 0;

			LauncherTabCompileLevel.Controls.Add((Control) LauncherDeleteMapButton);
			LauncherTabCompileLevel.Controls.Add((Control) LauncherCompileLevelOptionsGroupBox);
			LauncherTabCompileLevel.Controls.Add((Control) LauncherCreateMapButton);
			LauncherTabCompileLevel.Controls.Add((Control) LauncherMapList);
			LauncherTabCompileLevel.Location = new Point(4, 22);
			LauncherTabCompileLevel.Name = "LauncherTabCompileLevel";
			LauncherTabCompileLevel.Padding = new Padding(3);
			LauncherTabCompileLevel.Size = new Size(656, 354);
			LauncherTabCompileLevel.TabIndex = 0;
			LauncherTabCompileLevel.Text = "Compile Level";
			LauncherTabCompileLevel.UseVisualStyleBackColor = true;

			LauncherDeleteMapButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			LauncherDeleteMapButton.Location = new Point(6, 316);
			LauncherDeleteMapButton.Name = "LauncherDeleteMapButton";
			LauncherDeleteMapButton.Size = new Size(72, 32);
			LauncherDeleteMapButton.TabIndex = 4;
			LauncherDeleteMapButton.Text = "Delete Map";
			LauncherDeleteMapButton.UseVisualStyleBackColor = true;
			LauncherDeleteMapButton.Click += new EventHandler(LauncherDeleteMap_Click);

			LauncherCompileLevelOptionsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) LauncherGridFileGroupBox);
			LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) LauncherModSpecificMapComboBox);
			LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) LauncherModSpecificMapCheckBox);
			LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) LauncherCustomRunOptionsLabel);
			LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) LauncherCustomRunOptionsTextBox);
			LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) LauncherCompileLevelOptionsSplitterGroupBox);
			LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) LauncherCompileLevelButton);
			LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) LauncherCompileLightsButton);
			LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) LauncherCompileBSPButton);
			LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) LauncherUseRunGameTypeOptionsCheckBox);
			LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) LauncherRunMapAfterCompileCheckBox);
			LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) LauncherBspInfoCheckBox);
			LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) LauncherBuildFastFilesCheckBox);
			LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) LauncherCompileReflectionsCheckBox);
			LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) LauncherConnectPathsCheckBox);
			LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) LauncherCompileVisCheckBox);
			LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) LauncherCompileLightsCheckBox);
			LauncherCompileLevelOptionsGroupBox.Controls.Add((Control) LauncherCompileBSPCheckBox);
			LauncherCompileLevelOptionsGroupBox.Location = new Point(162, 6);
			LauncherCompileLevelOptionsGroupBox.MinimumSize = new Size(364, 332);
			LauncherCompileLevelOptionsGroupBox.Name = "LauncherCompileLevelOptionsGroupBox";
			LauncherCompileLevelOptionsGroupBox.Size = new Size(478, 342);
			LauncherCompileLevelOptionsGroupBox.TabIndex = 3;
			LauncherCompileLevelOptionsGroupBox.TabStop = false;
			LauncherCompileLevelOptionsGroupBox.Text = "Compile Level Options";

			LauncherGridFileGroupBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			LauncherGridFileGroupBox.Controls.Add((Control) LauncherGridEditExistingButton);
			LauncherGridFileGroupBox.Controls.Add((Control) LauncherGridMakeNewButton);
			LauncherGridFileGroupBox.Controls.Add((Control) LauncherGridCollectDotsCheckBox);
			LauncherGridFileGroupBox.Location = new Point(6, 260);
			LauncherGridFileGroupBox.Name = "LauncherGridFileGroupBox";
			LauncherGridFileGroupBox.Size = new Size(223, 76);
			LauncherGridFileGroupBox.TabIndex = 18;
			LauncherGridFileGroupBox.TabStop = false;
			LauncherGridFileGroupBox.Text = "Grid File";

			LauncherGridEditExistingButton.Location = new Point(112, 42);
			LauncherGridEditExistingButton.Name = "LauncherGridEditExistingButton";
			LauncherGridEditExistingButton.Size = new Size(100, 23);
			LauncherGridEditExistingButton.TabIndex = 19;
			LauncherGridEditExistingButton.Text = "Edit Existing Grid";
			LauncherGridEditExistingButton.UseVisualStyleBackColor = true;
			LauncherGridEditExistingButton.Click += new EventHandler(LauncherGridEditExistingButton_Click);

			LauncherGridMakeNewButton.Location = new Point(6, 42);
			LauncherGridMakeNewButton.Name = "LauncherGridMakeNewButton";
			LauncherGridMakeNewButton.Size = new Size(100, 23);
			LauncherGridMakeNewButton.TabIndex = 18;
			LauncherGridMakeNewButton.Text = "Make New Grid";
			LauncherGridMakeNewButton.UseVisualStyleBackColor = true;
			LauncherGridMakeNewButton.Click += new EventHandler(LauncherGridMakeNewButton_Click);

			LauncherGridCollectDotsCheckBox.AutoSize = true;
			LauncherGridCollectDotsCheckBox.Location = new Point(6, 19);
			LauncherGridCollectDotsCheckBox.Name = "LauncherGridCollectDotsCheckBox";
			LauncherGridCollectDotsCheckBox.Size = new Size(120, 17);
			LauncherGridCollectDotsCheckBox.TabIndex = 17;
			LauncherGridCollectDotsCheckBox.Text = "Models Collect Dots";
			LauncherGridCollectDotsCheckBox.UseVisualStyleBackColor = true;

			LauncherModSpecificMapComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			LauncherModSpecificMapComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			LauncherModSpecificMapComboBox.Enabled = false;
			LauncherModSpecificMapComboBox.FormattingEnabled = true;

			LauncherModSpecificMapComboBox.Items.AddRange(new object[3]
			{
				(object) "HumorOneMod",
				(object) "HumorTwoMod",
				(object) "BlahBlahMod"
			});

			LauncherModSpecificMapComboBox.Location = new Point(149, 41);
			LauncherModSpecificMapComboBox.Name = "LauncherModSpecificMapComboBox";
			LauncherModSpecificMapComboBox.Size = new Size(323, 21);
			LauncherModSpecificMapComboBox.TabIndex = 4;

			LauncherModSpecificMapCheckBox.AutoSize = true;
			LauncherModSpecificMapCheckBox.Location = new Point(149, 16);
			LauncherModSpecificMapCheckBox.Name = "LauncherModSpecificMapCheckBox";
			LauncherModSpecificMapCheckBox.Size = new Size(112, 17);
			LauncherModSpecificMapCheckBox.TabIndex = 5;
			LauncherModSpecificMapCheckBox.Text = "Mod Specific Map";
			LauncherModSpecificMapCheckBox.UseVisualStyleBackColor = true;
			LauncherModSpecificMapCheckBox.CheckedChanged += new EventHandler(CheckBox1_CheckedChanged);

			LauncherCustomRunOptionsLabel.AutoSize = true;
			LauncherCustomRunOptionsLabel.Location = new Point(6, 237);
			LauncherCustomRunOptionsLabel.Name = "LauncherCustomRunOptionsLabel";
			LauncherCustomRunOptionsLabel.Size = new Size(107, 13);
			LauncherCustomRunOptionsLabel.TabIndex = 14;
			LauncherCustomRunOptionsLabel.Text = "Custom Run Options:";
			LauncherCustomRunOptionsLabel.Visible = false;

			LauncherCustomRunOptionsTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			LauncherCustomRunOptionsTextBox.Location = new Point(119, 234);
			LauncherCustomRunOptionsTextBox.Name = "LauncherCustomRunOptionsTextBox";
			LauncherCustomRunOptionsTextBox.Size = new Size(353, 20);
			LauncherCustomRunOptionsTextBox.TabIndex = 13;
			LauncherCustomRunOptionsTextBox.Visible = false;

			LauncherCompileLevelOptionsSplitterGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			LauncherCompileLevelOptionsSplitterGroupBox.Location = new Point(2, 134);
			LauncherCompileLevelOptionsSplitterGroupBox.Name = "LauncherCompileLevelOptionsSplitterGroupBox";
			LauncherCompileLevelOptionsSplitterGroupBox.Size = new Size(476, 2);
			LauncherCompileLevelOptionsSplitterGroupBox.TabIndex = 12;
			LauncherCompileLevelOptionsSplitterGroupBox.TabStop = false;

			LauncherCompileLevelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			LauncherCompileLevelButton.Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
			LauncherCompileLevelButton.Location = new Point(344, 260);
			LauncherCompileLevelButton.Name = "LauncherCompileLevelButton";
			LauncherCompileLevelButton.Size = new Size(128, 76);
			LauncherCompileLevelButton.TabIndex = 4;
			LauncherCompileLevelButton.Text = "Compile Level";
			LauncherCompileLevelButton.UseVisualStyleBackColor = true;
			LauncherCompileLevelButton.Click += new EventHandler(LauncherCompileLevelButton_Click);

			LauncherCompileLightsButton.Location = new Point(107, 16);
			LauncherCompileLightsButton.Name = "LauncherCompileLightsButton";
			LauncherCompileLightsButton.Size = new Size(26, 23);
			LauncherCompileLightsButton.TabIndex = 11;
			LauncherCompileLightsButton.Text = "...";
			LauncherCompileLightsButton.UseVisualStyleBackColor = true;
			LauncherCompileLightsButton.Click += new EventHandler(LauncherCompileLightsButton_Click);

			LauncherCompileBSPButton.Location = new Point(107, 39);
			LauncherCompileBSPButton.Name = "LauncherCompileBSPButton";
			LauncherCompileBSPButton.Size = new Size(26, 23);
			LauncherCompileBSPButton.TabIndex = 10;
			LauncherCompileBSPButton.Text = "...";
			LauncherCompileBSPButton.UseVisualStyleBackColor = true;
			LauncherCompileBSPButton.Click += new EventHandler(LauncherCompileBSPButton_Click);

			LauncherUseRunGameTypeOptionsCheckBox.AutoSize = true;
			LauncherUseRunGameTypeOptionsCheckBox.FlatStyle = FlatStyle.Popup;
			LauncherUseRunGameTypeOptionsCheckBox.Location = new Point(9, 211);
			LauncherUseRunGameTypeOptionsCheckBox.Name = "LauncherUseRunGameTypeOptionsCheckBox";
			LauncherUseRunGameTypeOptionsCheckBox.Size = new Size(162, 17);
			LauncherUseRunGameTypeOptionsCheckBox.TabIndex = 9;
			LauncherUseRunGameTypeOptionsCheckBox.Text = "Use 'Run Game Tab' Options";
			LauncherUseRunGameTypeOptionsCheckBox.UseVisualStyleBackColor = true;
			LauncherUseRunGameTypeOptionsCheckBox.Visible = false;

			LauncherRunMapAfterCompileCheckBox.AutoSize = true;
			LauncherRunMapAfterCompileCheckBox.FlatStyle = FlatStyle.Popup;
			LauncherRunMapAfterCompileCheckBox.Location = new Point(9, 188);
			LauncherRunMapAfterCompileCheckBox.Name = "LauncherRunMapAfterCompileCheckBox";
			LauncherRunMapAfterCompileCheckBox.Size = new Size(133, 17);
			LauncherRunMapAfterCompileCheckBox.TabIndex = 8;
			LauncherRunMapAfterCompileCheckBox.Text = "Run Map After Compile";
			LauncherRunMapAfterCompileCheckBox.UseVisualStyleBackColor = true;

			LauncherBspInfoCheckBox.AutoSize = true;
			LauncherBspInfoCheckBox.FlatStyle = FlatStyle.Popup;
			LauncherBspInfoCheckBox.Location = new Point(9, 165);
			LauncherBspInfoCheckBox.Name = "LauncherBspInfoCheckBox";
			LauncherBspInfoCheckBox.Size = new Size(66, 17);
			LauncherBspInfoCheckBox.TabIndex = 7;
			LauncherBspInfoCheckBox.Text = "BSP Info";
			LauncherBspInfoCheckBox.UseVisualStyleBackColor = true;

			LauncherBuildFastFilesCheckBox.AutoSize = true;
			LauncherBuildFastFilesCheckBox.FlatStyle = FlatStyle.Popup;
			LauncherBuildFastFilesCheckBox.Location = new Point(9, 142);
			LauncherBuildFastFilesCheckBox.Name = "LauncherBuildFastFilesCheckBox";
			LauncherBuildFastFilesCheckBox.Size = new Size(88, 17);
			LauncherBuildFastFilesCheckBox.TabIndex = 6;
			LauncherBuildFastFilesCheckBox.Text = "Build Fastfiles";
			LauncherBuildFastFilesCheckBox.UseVisualStyleBackColor = true;

			LauncherCompileReflectionsCheckBox.AutoSize = true;
			LauncherCompileReflectionsCheckBox.FlatStyle = FlatStyle.Popup;
			LauncherCompileReflectionsCheckBox.Location = new Point(9, 111);
			LauncherCompileReflectionsCheckBox.Name = "LauncherCompileReflectionsCheckBox";
			LauncherCompileReflectionsCheckBox.Size = new Size(117, 17);
			LauncherCompileReflectionsCheckBox.TabIndex = 4;
			LauncherCompileReflectionsCheckBox.Text = "Compile Reflections";
			LauncherCompileReflectionsCheckBox.UseVisualStyleBackColor = true;

			LauncherConnectPathsCheckBox.AutoSize = true;
			LauncherConnectPathsCheckBox.FlatStyle = FlatStyle.Popup;
			LauncherConnectPathsCheckBox.Location = new Point(9, 88);
			LauncherConnectPathsCheckBox.Name = "LauncherConnectPathsCheckBox";
			LauncherConnectPathsCheckBox.Size = new Size(94, 17);
			LauncherConnectPathsCheckBox.TabIndex = 3;
			LauncherConnectPathsCheckBox.Text = "Connect Paths";
			LauncherConnectPathsCheckBox.UseVisualStyleBackColor = true;

			LauncherCompileVisCheckBox.AutoSize = true;
			LauncherCompileVisCheckBox.FlatStyle = FlatStyle.Popup;
			LauncherCompileVisCheckBox.Location = new Point(9, 65);
			LauncherCompileVisCheckBox.Name = "LauncherCompileVisCheckBox";
			LauncherCompileVisCheckBox.Size = new Size(78, 17);
			LauncherCompileVisCheckBox.TabIndex = 2;
			LauncherCompileVisCheckBox.Text = "Compile Vis";
			LauncherCompileVisCheckBox.UseVisualStyleBackColor = true;

			LauncherCompileLightsCheckBox.AutoSize = true;
			LauncherCompileLightsCheckBox.FlatStyle = FlatStyle.Popup;
			LauncherCompileLightsCheckBox.Location = new Point(9, 42);
			LauncherCompileLightsCheckBox.Name = "LauncherCompileLightsCheckBox";
			LauncherCompileLightsCheckBox.Size = new Size(92, 17);
			LauncherCompileLightsCheckBox.TabIndex = 1;
			LauncherCompileLightsCheckBox.Text = "Compile Lights";
			LauncherCompileLightsCheckBox.UseVisualStyleBackColor = true;

			LauncherCompileBSPCheckBox.AutoSize = true;
			LauncherCompileBSPCheckBox.FlatStyle = FlatStyle.Popup;
			LauncherCompileBSPCheckBox.Location = new Point(9, 19);
			LauncherCompileBSPCheckBox.Name = "LauncherCompileBSPCheckBox";
			LauncherCompileBSPCheckBox.Size = new Size(85, 17);
			LauncherCompileBSPCheckBox.TabIndex = 0;
			LauncherCompileBSPCheckBox.Text = "Compile BSP";
			LauncherCompileBSPCheckBox.UseVisualStyleBackColor = true;

			LauncherCreateMapButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			LauncherCreateMapButton.Location = new Point(84, 316);
			LauncherCreateMapButton.Name = "LauncherCreateMapButton";
			LauncherCreateMapButton.Size = new Size(72, 32);
			LauncherCreateMapButton.TabIndex = 2;
			LauncherCreateMapButton.Text = "Create Map";
			LauncherCreateMapButton.UseVisualStyleBackColor = true;
			LauncherCreateMapButton.Click += new EventHandler(LauncherButtonCreateMap_Click);

			LauncherMapList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			LauncherMapList.FormattingEnabled = true;
			LauncherMapList.IntegralHeight = false;
			LauncherMapList.Location = new Point(6, 6);
			LauncherMapList.Name = "LauncherMapList";
			LauncherMapList.Size = new Size(150, 304);
			LauncherMapList.TabIndex = 1;
			LauncherMapList.SelectedIndexChanged += new EventHandler(LauncherMapList_SelectedIndexChanged);
			LauncherMapList.DoubleClick += new EventHandler(LauncherMapList_DoubleClick);

			LauncherTabModBuilder.Controls.Add((Control) LauncherIwdFileGroupBox);
			LauncherTabModBuilder.Controls.Add((Control) LauncherFastFileCsvGroupBox);
			LauncherTabModBuilder.Controls.Add((Control) LauncherModGroupBox);
			LauncherTabModBuilder.Location = new Point(4, 22);
			LauncherTabModBuilder.Name = "LauncherTabModBuilder";
			LauncherTabModBuilder.Padding = new Padding(3);
			LauncherTabModBuilder.Size = new Size(656, 354);
			LauncherTabModBuilder.TabIndex = 1;
			LauncherTabModBuilder.Text = "Mod Builder";
			LauncherTabModBuilder.UseVisualStyleBackColor = true;

			LauncherIwdFileGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LauncherIwdFileGroupBox.Controls.Add((Control) LauncherIwdFileTree);
			LauncherIwdFileGroupBox.Location = new Point(298, 6);
			LauncherIwdFileGroupBox.Name = "LauncherIwdFileGroupBox";
			LauncherIwdFileGroupBox.Size = new Size(357, 342);
			LauncherIwdFileGroupBox.TabIndex = 2;
			LauncherIwdFileGroupBox.TabStop = false;
			LauncherIwdFileGroupBox.Text = "IWD File List";

			LauncherIwdFileTree.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LauncherIwdFileTree.CheckBoxes = true;
			LauncherIwdFileTree.Indent = 15;
			LauncherIwdFileTree.Location = new Point(6, 19);
			LauncherIwdFileTree.Name = "LauncherIwdFileTree";
			LauncherIwdFileTree.Size = new Size(345, 316);
			LauncherIwdFileTree.TabIndex = 1;
			LauncherIwdFileTree.AfterCheck += new TreeViewEventHandler(LauncherIwdFileTree_AfterCheck);
			LauncherIwdFileTree.DoubleClick += new EventHandler(LauncherIwdFileTree_DoubleClick);

			LauncherFastFileCsvGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			LauncherFastFileCsvGroupBox.Controls.Add((Control) LauncherFastFileCsvTextBox);
			LauncherFastFileCsvGroupBox.Location = new Point(6, 138);
			LauncherFastFileCsvGroupBox.Name = "LauncherFastFileCsvGroupBox";
			LauncherFastFileCsvGroupBox.Size = new Size(286, 210);
			LauncherFastFileCsvGroupBox.TabIndex = 19;
			LauncherFastFileCsvGroupBox.TabStop = false;
			LauncherFastFileCsvGroupBox.Text = "Fastfile mod.csv";

			LauncherFastFileCsvTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LauncherFastFileCsvTextBox.Font = new Font("Courier New", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
			LauncherFastFileCsvTextBox.Location = new Point(6, 17);
			LauncherFastFileCsvTextBox.Name = "LauncherFastFileCsvTextBox";
			LauncherFastFileCsvTextBox.Size = new Size(274, 186);
			LauncherFastFileCsvTextBox.TabIndex = 0;
			LauncherFastFileCsvTextBox.Text = "";
			LauncherFastFileCsvTextBox.WordWrap = false;

			LauncherModGroupBox.Controls.Add((Control) LauncherModBuildButton);
			LauncherModGroupBox.Controls.Add((Control) LauncherModBuildSoundsCheckBox);
			LauncherModGroupBox.Controls.Add((Control) LauncherModVerboseCheckBox);
			LauncherModGroupBox.Controls.Add((Control) LauncherModBuildIwdFileCheckBox);
			LauncherModGroupBox.Controls.Add((Control) LauncherModBuildFastFilesCheckBox);
			LauncherModGroupBox.Controls.Add((Control) LauncherModComboBox);
			LauncherModGroupBox.Location = new Point(6, 6);
			LauncherModGroupBox.Name = "LauncherModGroupBox";
			LauncherModGroupBox.Size = new Size(286, 126);
			LauncherModGroupBox.TabIndex = 4;
			LauncherModGroupBox.TabStop = false;
			LauncherModGroupBox.Text = "MOD";

			LauncherModBuildButton.Location = new Point(7, 93);
			LauncherModBuildButton.Name = "LauncherModBuildButton";
			LauncherModBuildButton.Size = new Size(88, 26);
			LauncherModBuildButton.TabIndex = 18;
			LauncherModBuildButton.Text = "Build MOD";
			LauncherModBuildButton.UseVisualStyleBackColor = true;
			LauncherModBuildButton.Click += new EventHandler(LauncherModBuildButton_Click);

			LauncherModBuildSoundsCheckBox.AutoSize = true;
			LauncherModBuildSoundsCheckBox.Location = new Point(130, 47);
			LauncherModBuildSoundsCheckBox.Name = "LauncherModBuildSoundsCheckBox";
			LauncherModBuildSoundsCheckBox.Size = new Size(88, 17);
			LauncherModBuildSoundsCheckBox.TabIndex = 17;
			LauncherModBuildSoundsCheckBox.Text = "Build Sounds";
			LauncherModBuildSoundsCheckBox.UseVisualStyleBackColor = true;

			LauncherModVerboseCheckBox.AutoSize = true;
			LauncherModVerboseCheckBox.Location = new Point(130, 70);
			LauncherModVerboseCheckBox.Name = "LauncherModVerboseCheckBox";
			LauncherModVerboseCheckBox.Size = new Size(65, 17);
			LauncherModVerboseCheckBox.TabIndex = 15;
			LauncherModVerboseCheckBox.Text = "Verbose";
			LauncherModVerboseCheckBox.UseVisualStyleBackColor = true;

			LauncherModBuildIwdFileCheckBox.AutoSize = true;
			LauncherModBuildIwdFileCheckBox.Location = new Point(7, 70);
			LauncherModBuildIwdFileCheckBox.Name = "LauncherModBuildIwdFileCheckBox";
			LauncherModBuildIwdFileCheckBox.Size = new Size(93, 17);
			LauncherModBuildIwdFileCheckBox.TabIndex = 14;
			LauncherModBuildIwdFileCheckBox.Text = "Build IWD File";
			LauncherModBuildIwdFileCheckBox.UseVisualStyleBackColor = true;

			LauncherModBuildFastFilesCheckBox.AutoSize = true;
			LauncherModBuildFastFilesCheckBox.Location = new Point(7, 47);
			LauncherModBuildFastFilesCheckBox.Name = "LauncherModBuildFastFilesCheckBox";
			LauncherModBuildFastFilesCheckBox.Size = new Size(117, 17);
			LauncherModBuildFastFilesCheckBox.TabIndex = 13;
			LauncherModBuildFastFilesCheckBox.Text = "Build mod.ff Fastfile";
			LauncherModBuildFastFilesCheckBox.UseVisualStyleBackColor = true;

			LauncherModComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			LauncherModComboBox.FormattingEnabled = true;
			LauncherModComboBox.Location = new Point(6, 20);
			LauncherModComboBox.Name = "LauncherModComboBox";
			LauncherModComboBox.Size = new Size(274, 21);
			LauncherModComboBox.TabIndex = 3;
			LauncherModComboBox.SelectedIndexChanged += new EventHandler(LauncherModComboBox_SelectedIndexChanged);

			LauncherTabRunGame.Controls.Add((Control) LauncherGameOptionsPanel);
			LauncherTabRunGame.Controls.Add((Control) LauncherRunGameButton);
			LauncherTabRunGame.Controls.Add((Control) LauncherRunGameCustomCommandLineGroupBox);
			LauncherTabRunGame.Controls.Add((Control) LauncherRunGameCommandLineGroupBox);
			LauncherTabRunGame.Controls.Add((Control) LauncherRunGameModGroupBox);
			LauncherTabRunGame.Controls.Add((Control) LauncherRunGameExeTypeGroupBox);
			LauncherTabRunGame.Location = new Point(4, 22);
			LauncherTabRunGame.Name = "LauncherTabRunGame";
			LauncherTabRunGame.Padding = new Padding(3);
			LauncherTabRunGame.Size = new Size(656, 354);
			LauncherTabRunGame.TabIndex = 2;
			LauncherTabRunGame.Text = "Run Game";
			LauncherTabRunGame.UseVisualStyleBackColor = true;

			LauncherGameOptionsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LauncherGameOptionsPanel.AutoScroll = true;
			LauncherGameOptionsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			LauncherGameOptionsPanel.BorderStyle = BorderStyle.Fixed3D;
			LauncherGameOptionsPanel.Location = new Point(6, 59);
			LauncherGameOptionsPanel.Margin = new Padding(0);
			LauncherGameOptionsPanel.Name = "LauncherGameOptionsPanel";
			LauncherGameOptionsPanel.Size = new Size(644, 150);
			LauncherGameOptionsPanel.TabIndex = 5;
			LauncherGameOptionsPanel.Click += new EventHandler(LauncherGameOptionsFlowPanel_Click);

			LauncherRunGameButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			LauncherRunGameButton.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
			LauncherRunGameButton.Location = new Point(522, 265);
			LauncherRunGameButton.Name = "LauncherRunGameButton";
			LauncherRunGameButton.Size = new Size(128, 83);
			LauncherRunGameButton.TabIndex = 2;
			LauncherRunGameButton.Text = "Run Game";
			LauncherRunGameButton.UseVisualStyleBackColor = true;
			LauncherRunGameButton.Click += new EventHandler(LauncherRunGameButton_Click);

			LauncherRunGameCustomCommandLineGroupBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LauncherRunGameCustomCommandLineGroupBox.Controls.Add((Control) LauncherRunGameCustomCommandLineTextBox);
			LauncherRunGameCustomCommandLineGroupBox.Location = new Point(6, 215);
			LauncherRunGameCustomCommandLineGroupBox.Name = "LauncherRunGameCustomCommandLineGroupBox";
			LauncherRunGameCustomCommandLineGroupBox.Size = new Size(644, 44);
			LauncherRunGameCustomCommandLineGroupBox.TabIndex = 4;
			LauncherRunGameCustomCommandLineGroupBox.TabStop = false;
			LauncherRunGameCustomCommandLineGroupBox.Text = "Custom Command Line";

			LauncherRunGameCustomCommandLineTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LauncherRunGameCustomCommandLineTextBox.Location = new Point(6, 17);
			LauncherRunGameCustomCommandLineTextBox.Name = "LauncherRunGameCustomCommandLineTextBox";
			LauncherRunGameCustomCommandLineTextBox.Size = new Size(632, 20);
			LauncherRunGameCustomCommandLineTextBox.TabIndex = 0;
			LauncherRunGameCustomCommandLineTextBox.TextChanged += new EventHandler(LauncherRunGameCustomCommandLineTextBox_TextChanged);
			LauncherRunGameCustomCommandLineTextBox.Validating += new CancelEventHandler(LauncherRunGameCustomCommandLineTextBox_Validating);

			LauncherRunGameCommandLineGroupBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LauncherRunGameCommandLineGroupBox.Controls.Add((Control) LauncherRunGameCommandLineTextBox);
			LauncherRunGameCommandLineGroupBox.Location = new Point(6, 265);
			LauncherRunGameCommandLineGroupBox.Name = "LauncherRunGameCommandLineGroupBox";
			LauncherRunGameCommandLineGroupBox.Size = new Size(510, 83);
			LauncherRunGameCommandLineGroupBox.TabIndex = 3;
			LauncherRunGameCommandLineGroupBox.TabStop = false;
			LauncherRunGameCommandLineGroupBox.Text = "Command Line";

			LauncherRunGameCommandLineTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LauncherRunGameCommandLineTextBox.BorderStyle = BorderStyle.None;
			LauncherRunGameCommandLineTextBox.Location = new Point(6, 19);
			LauncherRunGameCommandLineTextBox.Multiline = true;
			LauncherRunGameCommandLineTextBox.Name = "LauncherRunGameCommandLineTextBox";
			LauncherRunGameCommandLineTextBox.ReadOnly = true;
			LauncherRunGameCommandLineTextBox.Size = new Size(498, 58);
			LauncherRunGameCommandLineTextBox.TabIndex = 0;

			LauncherRunGameModGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			LauncherRunGameModGroupBox.Controls.Add((Control) LauncherRunGameModComboBox);
			LauncherRunGameModGroupBox.Location = new Point(109, 6);
			LauncherRunGameModGroupBox.Name = "LauncherRunGameModGroupBox";
			LauncherRunGameModGroupBox.Size = new Size(541, 47);
			LauncherRunGameModGroupBox.TabIndex = 1;
			LauncherRunGameModGroupBox.TabStop = false;
			LauncherRunGameModGroupBox.Text = "Mod";

			LauncherRunGameModComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LauncherRunGameModComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			LauncherRunGameModComboBox.FormattingEnabled = true;
			LauncherRunGameModComboBox.Location = new Point(6, 18);
			LauncherRunGameModComboBox.Name = "LauncherRunGameModComboBox";
			LauncherRunGameModComboBox.Size = new Size(529, 21);
			LauncherRunGameModComboBox.TabIndex = 0;

			LauncherRunGameExeTypeGroupBox.Controls.Add((Control) LauncherRunGameExeTypeMpRadioButton);
			LauncherRunGameExeTypeGroupBox.Controls.Add((Control) LauncherRunGameTypeRadioButton);
			LauncherRunGameExeTypeGroupBox.Location = new Point(6, 6);
			LauncherRunGameExeTypeGroupBox.Name = "LauncherRunGameExeTypeGroupBox";
			LauncherRunGameExeTypeGroupBox.Size = new Size(97, 47);
			LauncherRunGameExeTypeGroupBox.TabIndex = 0;
			LauncherRunGameExeTypeGroupBox.TabStop = false;
			LauncherRunGameExeTypeGroupBox.Text = "Exe Type";

			LauncherRunGameExeTypeMpRadioButton.AutoSize = true;
			LauncherRunGameExeTypeMpRadioButton.Location = new Point(50, 19);
			LauncherRunGameExeTypeMpRadioButton.Name = "LauncherRunGameExeTypeMpRadioButton";
			LauncherRunGameExeTypeMpRadioButton.Size = new Size(41, 17);
			LauncherRunGameExeTypeMpRadioButton.TabIndex = 1;
			LauncherRunGameExeTypeMpRadioButton.Text = "MP";
			LauncherRunGameExeTypeMpRadioButton.UseVisualStyleBackColor = true;

			LauncherRunGameTypeRadioButton.AutoSize = true;
			LauncherRunGameTypeRadioButton.Checked = true;
			LauncherRunGameTypeRadioButton.Location = new Point(6, 19);
			LauncherRunGameTypeRadioButton.Name = "LauncherRunGameTypeRadioButton";
			LauncherRunGameTypeRadioButton.Size = new Size(39, 17);
			LauncherRunGameTypeRadioButton.TabIndex = 0;
			LauncherRunGameTypeRadioButton.TabStop = true;
			LauncherRunGameTypeRadioButton.Text = "SP";
			LauncherRunGameTypeRadioButton.UseVisualStyleBackColor = true;

			LauncherProcessTimeElapsedTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			LauncherProcessTimeElapsedTextBox.Location = new Point(756, 233);
			LauncherProcessTimeElapsedTextBox.Name = "LauncherProcessTimeElapsedTextBox";
			LauncherProcessTimeElapsedTextBox.ReadOnly = true;
			LauncherProcessTimeElapsedTextBox.Size = new Size(55, 20);
			LauncherProcessTimeElapsedTextBox.TabIndex = 4;

			LauncherProcessTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LauncherProcessTextBox.Location = new Point(149, 233);
			LauncherProcessTextBox.Name = "LauncherProcessTextBox";
			LauncherProcessTextBox.ReadOnly = true;
			LauncherProcessTextBox.Size = new Size(607, 20);
			LauncherProcessTextBox.TabIndex = 3;

			LauncherProcessGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			LauncherProcessGroupBox.Controls.Add((Control) LauncherButtonCancel);
			LauncherProcessGroupBox.Controls.Add((Control) LauncherProcessList);
			LauncherProcessGroupBox.Location = new Point(3, 3);
			LauncherProcessGroupBox.Name = "LauncherProcessGroupBox";
			LauncherProcessGroupBox.Size = new Size(140, 250);
			LauncherProcessGroupBox.TabIndex = 2;
			LauncherProcessGroupBox.TabStop = false;
			LauncherProcessGroupBox.Text = "Processes";

			LauncherButtonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LauncherButtonCancel.BackColor = Color.LightCoral;
			LauncherButtonCancel.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
			LauncherButtonCancel.ForeColor = SystemColors.Info;
			LauncherButtonCancel.Location = new Point(6, 179);
			LauncherButtonCancel.Name = "LauncherButtonCancel";
			LauncherButtonCancel.Size = new Size(128, 65);
			LauncherButtonCancel.TabIndex = 4;
			LauncherButtonCancel.Text = "Cancel";
			LauncherButtonCancel.UseVisualStyleBackColor = false;
			LauncherButtonCancel.Click += new EventHandler(LauncherButtonCancel_Click);

			LauncherProcessList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LauncherProcessList.BackColor = SystemColors.Info;
			LauncherProcessList.BorderStyle = BorderStyle.FixedSingle;
			LauncherProcessList.Font = new Font("Lucida Console", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
			LauncherProcessList.ForeColor = SystemColors.HotTrack;
			LauncherProcessList.FormattingEnabled = true;
			LauncherProcessList.IntegralHeight = false;
			LauncherProcessList.ItemHeight = 11;
			LauncherProcessList.Location = new Point(6, 19);
			LauncherProcessList.Name = "LauncherProcessList";
			LauncherProcessList.Size = new Size(128, 154);
			LauncherProcessList.TabIndex = 1;
			LauncherProcessList.SelectedIndexChanged += new EventHandler(LauncherProcessList_SelectedIndexChanged);

			LauncherTimer.Enabled = true;
			LauncherTimer.Interval = 1000;
			LauncherTimer.Tick += new EventHandler(LauncherTimer_Tick);

			LauncherWikiLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			LauncherWikiLabel.AutoSize = true;
			LauncherWikiLabel.Font = new Font("Courier New", 11.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
			LauncherWikiLabel.Location = new Point(779, 3);
			LauncherWikiLabel.Name = "LauncherWikiLabel";
			LauncherWikiLabel.Size = new Size(44, 17);
			LauncherWikiLabel.TabIndex = 6;
			LauncherWikiLabel.TabStop = true;
			LauncherWikiLabel.Text = "WIKI";
			LauncherWikiLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(LauncherWikiLabel_LinkClicked);

			LauncherAboutLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			LauncherAboutLabel.AutoSize = true;
			LauncherAboutLabel.Font = new Font("Courier New", 11.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
			LauncherAboutLabel.Location = new Point(723, 3);
			LauncherAboutLabel.Name = "LauncherAboutLabel";
			LauncherAboutLabel.Size = new Size(53, 17);
			LauncherAboutLabel.TabIndex = 7;
			LauncherAboutLabel.TabStop = true;
			LauncherAboutLabel.Text = "ABOUT";
			LauncherAboutLabel.TextAlign = ContentAlignment.MiddleRight;
			LauncherAboutLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(LauncherAboutLabel_LinkClicked);

			LauncherMapFilesSystemWatcher.EnableRaisingEvents = true;
			LauncherMapFilesSystemWatcher.Filter = "*.map";
			LauncherMapFilesSystemWatcher.NotifyFilter = NotifyFilters.FileName;
			LauncherMapFilesSystemWatcher.SynchronizingObject = (ISynchronizeInvoke)this;
			LauncherMapFilesSystemWatcher.Renamed += new RenamedEventHandler(LauncherMapFilesSystemWatcher_Renamed);
			LauncherMapFilesSystemWatcher.Deleted += new FileSystemEventHandler(LauncherMapFilesSystemWatcher_Deleted);
			LauncherMapFilesSystemWatcher.Created += new FileSystemEventHandler(LauncherMapFilesSystemWatcher_Created);
			LauncherMapFilesSystemWatcher.Changed += new FileSystemEventHandler(LauncherMapFilesSystemWatcher_Changed);

			LauncherModsDirectorySystemWatcher.EnableRaisingEvents = true;
			LauncherModsDirectorySystemWatcher.NotifyFilter = NotifyFilters.DirectoryName;
			LauncherModsDirectorySystemWatcher.SynchronizingObject = (ISynchronizeInvoke)this;
			LauncherModsDirectorySystemWatcher.Renamed += new RenamedEventHandler(LauncherModsDirectorySystemWatcher_Renamed);
			LauncherModsDirectorySystemWatcher.Deleted += new FileSystemEventHandler(LauncherModsDirectorySystemWatcher_Deleted);
			LauncherModsDirectorySystemWatcher.Created += new FileSystemEventHandler(LauncherModsDirectorySystemWatcher_Created);
			LauncherModsDirectorySystemWatcher.Changed += new FileSystemEventHandler(LauncherModsDirectorySystemWatcher_Changed);

			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(838, 664);
			Controls.Add((Control)LauncherAboutLabel);
			Controls.Add((Control)LauncherWikiLabel);
			Controls.Add((Control)LauncherSplitter);
			Icon = new Icon(new MemoryStream(Properties.Resources.Icon));
			Name = "Launcher";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Launcher";
			Load += new EventHandler(LauncherForm_Load);
			FormClosing += new FormClosingEventHandler(LauncherForm_FormClosing);

			LauncherSplitter.Panel1.ResumeLayout(false);
			LauncherSplitter.Panel2.ResumeLayout(false);
			LauncherSplitter.Panel2.PerformLayout();
			LauncherSplitter.ResumeLayout(false);
			LauncherApplicationsGroupBox.ResumeLayout(false);
			LauncherTab.ResumeLayout(false);
			LauncherTabCompileLevel.ResumeLayout(false);
			LauncherCompileLevelOptionsGroupBox.ResumeLayout(false);
			LauncherCompileLevelOptionsGroupBox.PerformLayout();
			LauncherGridFileGroupBox.ResumeLayout(false);
			LauncherGridFileGroupBox.PerformLayout();
			LauncherTabModBuilder.ResumeLayout(false);
			LauncherIwdFileGroupBox.ResumeLayout(false);
			LauncherFastFileCsvGroupBox.ResumeLayout(false);
			LauncherModGroupBox.ResumeLayout(false);
			LauncherModGroupBox.PerformLayout();
			LauncherTabRunGame.ResumeLayout(false);
			LauncherRunGameCustomCommandLineGroupBox.ResumeLayout(false);
			LauncherRunGameCustomCommandLineGroupBox.PerformLayout();
			LauncherRunGameCommandLineGroupBox.ResumeLayout(false);
			LauncherRunGameCommandLineGroupBox.PerformLayout();
			LauncherRunGameModGroupBox.ResumeLayout(false);
			LauncherRunGameExeTypeGroupBox.ResumeLayout(false);
			LauncherRunGameExeTypeGroupBox.PerformLayout();
			LauncherProcessGroupBox.ResumeLayout(false);
			LauncherMapFilesSystemWatcher.EndInit();
			LauncherModsDirectorySystemWatcher.EndInit();

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

		public delegate void ProcessFinishedDelegate(Process lastProcess);
	}
}