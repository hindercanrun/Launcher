using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace Launcher
{
	internal static class Launcher
	{
		internal static LauncherForm TheLauncherForm = (LauncherForm) null;
		internal static Settings launcherSettings = new Settings();
		internal static Settings mapSettings = new Settings();

		internal static DVar[] DVars = new DVar[10]
		{
			new DVar("devmap", "Loads map"),
			new DVar("thereisacow", "Enables Cheats", 0M, 1M),
			new DVar("developer", "Used to give more debug", 0M, 2M),
			new DVar("developer_script", "Used to give more script debug", 0M, 1M),
			new DVar("logfile", "Spits out a console.log", 0M, 2M),
			new DVar("com_introplayed", "Skips the intro movies", 0M, 1M),
			new DVar("sv_pure", "Determines if the game will use the shipped iwd files only", 0M, 1M),
			new DVar("r_fullscreen", "Enables fullscreen", 0M, 1M),
			new DVar("sv_punkbuster", "Toggles the use of PunkBuster", 0M, 2M),
			new DVar("scr_testclients", "How many test bots do you want?", 0M, 10M)
		};

		[STAThread]
		private static void Main()
		{
			string mutexName = (GetRootDirectory() + Application.ProductName)
				.ToLower()
				.Replace("\\", "-")
				.Replace(":", "-");
			using (var mutex = new Mutex(true, mutexName, out bool createdNew))
			{
				if (!createdNew)
				{
					MessageBox.Show("Only one instance of " + Application.ProductName + " is allowed at a time.");
					return;
				}

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				TheLauncherForm = new LauncherForm();
				Application.Run(TheLauncherForm);
			}
		}

		// TODO: Should we hardcode this?
		internal static string GetLanguage()
		{
			return "english";
		}

		internal static void MakeDirectory(string directoryName)
		{
			if (string.IsNullOrWhiteSpace(directoryName))
			{
				return;
			}
			Directory.CreateDirectory(directoryName);
		}

		internal static string CanonicalDirectory(string path)
		{
			string fullPath = Path.GetFullPath(path);
			MakeDirectory(fullPath);

			if (!fullPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
			{
				fullPath += Path.DirectorySeparatorChar;
			}
			return fullPath;
		}

		internal static string GetLocalApplicationDirectory()
		{
			// Get the local application data folder for Call of Duty: World at War
			string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			string codDir = Path.Combine(localAppData, "Activision", "CodWaW");
			return CanonicalDirectory(codDir);
		}

		internal static string GetLocalApplicationUsermapsDirectory()
		{
			return CanonicalDirectory(Path.Combine(GetLocalApplicationDirectory(), "usermaps"));
		}

		internal static string GetLocalApplicationModsDirectory()
		{
			return CanonicalDirectory(Path.Combine(GetLocalApplicationDirectory(), "mods"));
		}

		internal static string GetLocalApplicationModDirectory(string modName)
		{
			return CanonicalDirectory(Path.Combine(GetLocalApplicationModsDirectory(), modName));
		}

		internal static string GetStartupDirectory()
		{
			return CanonicalDirectory(Path.GetFullPath("."));
		}

		internal static string GetRootDirectory()
		{
			return CanonicalDirectory(Path.Combine(GetStartupDirectory(), ".."));
		}

		internal static string GetBinDirectory()
		{
			return CanonicalDirectory(Path.Combine(GetRootDirectory(), "bin"));
		}

		internal static string GetMapSourceDirectory()
		{
			return CanonicalDirectory(Path.Combine(GetRootDirectory(), "map_source"));
		}

		internal static string GetRawDirectory()
		{
			return CanonicalDirectory(Path.Combine(GetRootDirectory(), "raw"));
		}

		internal static string GetRawMapsDirectory()
		{
			return CanonicalDirectory(Path.Combine(GetRawDirectory(), "maps"));
		}

		internal static string GetModsDirectory()
		{
			return CanonicalDirectory(Path.Combine(GetRootDirectory(), "mods"));
		}

		internal static string GetUsermapsDirectory()
		{
			return CanonicalDirectory(Path.Combine(GetRootDirectory(), "usermaps"));
		}

		internal static string GetModDirectory(string modName)
		{
			return CanonicalDirectory(Path.Combine(GetModsDirectory(), modName));
		}

		internal static string GetMapSettingsDirectory()
		{
			return CanonicalDirectory(Path.Combine(GetStartupDirectory(), Application.ProductName, "map_settings"));
		}

		internal static string GetMapTemplatesDirectory()
		{
			return CanonicalDirectory(Path.Combine(GetStartupDirectory(), Path.Combine(Application.ProductName, "map_templates")));
		}

		internal static string GetZoneSourceDirectory()
		{
			return CanonicalDirectory(Path.Combine(GetRootDirectory(), "zone_source"));
		}

		internal static string GetZoneDirectory()
		{
			return CanonicalDirectory(Path.Combine(Path.Combine(GetRootDirectory(), "zone"), GetLanguage()));
		}

		internal static string GetZoneSourceFile(string mapName)
		{
			return Path.Combine(GetZoneSourceDirectory(), mapName + ".csv");
		}

		internal static string GetLoadZone(string mapName)
		{
			return mapName + "_load";
		}

		internal static string GetZoneSourceLoadCSVFile(string mapName)
		{
			return Path.Combine(GetZoneSourceDirectory(), GetLoadZone(mapName) + ".csv");
		}

		internal static void StringArrayAdd(ref string[] stringArray, string stringItem)
		{
			// Add item to array by resizing
			Array.Resize(ref stringArray, stringArray.Length + 1);
			stringArray[stringArray.Length - 1] = stringItem;
		}

		internal static string GetGameTool(bool mpVersion)
		{
			return (mpVersion) ? "../mp_tool" : "../sp_tool";
		}

		internal static string GetGameApplication(bool mpVersion)
		{
			return (mpVersion) ? "../CoDWaWmp" : "../CoDWaW";
		}

		internal static string[] GetDirs(string directory)
		{
			// Get all subdirectory names in the specified directory
			var dirs = new List<string>();
			foreach (var subDir in new DirectoryInfo(directory).GetDirectories())
			{
				dirs.Add(subDir.Name);
			}
			return dirs.ToArray();
		}

		internal static string[] GetFilesRecursively(string directory)
		{
			return GetFilesRecursively(directory, "*");
		}

		internal static string[] GetFilesRecursively(string directory, string filesToIncludeFilter)
		{
			string[] files = new string[0];
			Launcher.GetFilesRecursively(directory, filesToIncludeFilter, ref files);
			return files;
		}

		internal static void GetFilesRecursively(
			string directory,
			string filesToIncludeFilter,
			ref string[] files)
		{
			foreach (DirectoryInfo directory1 in new DirectoryInfo(directory).GetDirectories())
			{
				Launcher.GetFilesRecursively(Path.Combine(directory, directory1.Name), filesToIncludeFilter, ref files);
			}

			foreach (FileInfo file in new DirectoryInfo(directory).GetFiles(filesToIncludeFilter))
			{
				Launcher.StringArrayAdd(ref files, Path.Combine(directory, file.Name.ToLower()));
			}
		}

		internal static string[] GetFiles(string directory, string searchFilter)
		{
			// Get all files matching the search filter and return their filenames
			var files = new List<string>();
			foreach (var file in new DirectoryInfo(directory).GetFiles(searchFilter))
			{
				files.Add(file.Name);
			}
			return files.ToArray();
		}

		internal static string[] GetFilesWithoutExtension(string directory, string searchFilter)
		{
			// Get all files matching the search filter and return their filenames without extension
			var files = new List<string>();
			foreach (var file in new DirectoryInfo(directory).GetFiles(searchFilter))
			{
				files.Add(Path.GetFileNameWithoutExtension(file.Name));
			}
			return files.ToArray();
		}

		internal static string[] GetMapList()
		{
			return GetFilesWithoutExtension(GetMapSourceDirectory(), "*.map");
		}

		internal static string[] GetMapTemplatesList()
		{
			return GetDirs(GetMapTemplatesDirectory());
		}

		internal static string[] GetModList()
		{
			return GetDirs(GetModsDirectory());
		}

		internal static string[] LoadTextFile(string textFile, string skipCommentLinesStartingWith)
		{
			var lines = new List<string>();

			using (var reader = new StreamReader(textFile))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					line = line.Trim();
					if  (line != "" &&
						(skipCommentLinesStartingWith == null ||
						!line.StartsWith(skipCommentLinesStartingWith)))
					{
						lines.Add(line);
					}
				}
			}
			return lines.ToArray();
		}

		internal static string[] LoadTextFile(string textFile)
		{
			return LoadTextFile(textFile, null);
		}

		internal static void SaveTextFile(string textFile, string[] text)
		{
			using (var writer = new StreamWriter(textFile))
			{
				foreach (string str in text)
				{
					writer.WriteLine(str);
				}
			}
		}

		internal static Hashtable StringArrayToHashTable(string[] stringArray)
		{
			var hashTable = new Hashtable(stringArray.Length);

			foreach (string line in stringArray)
			{
				// Split each line by the first comma
				string[] parts = line.Split(new[] { ',' }, 2);
				if (parts.Length > 0)
				{
					hashTable.Add(parts[0], (parts.Length > 1) ? parts[1] : null);
				}
			}
			return hashTable;
		}

		internal static string[] HashTableToStringArray(Hashtable hashTable)
		{
			// Convert each key-value pair to "key,value" format
			string[] array = new string[hashTable.Count];
			int i = 0;

			foreach (DictionaryEntry entry in hashTable)
			{
				string valuePart = (entry.Value != null) ? "," + entry.Value : "";
				array[i++] = entry.Key + valuePart;
			}

			// Sort the array alphabetically
			Array.Sort(array);
			return array;
		}

		internal static string GetMapSettingsFilename(string mapName)
		{
			return Path.Combine(GetMapSettingsDirectory(), mapName + ".cfg");
		}

		internal static Hashtable LoadMapSettings(string mapName)
		{
			string[] lines = LoadTextFile(GetMapSettingsFilename(mapName));
			return StringArrayToHashTable(lines);
		}

		internal static void SaveMapSettings(string mapName, Hashtable settings)
		{
			string[] lines = HashTableToStringArray(settings);
			SaveTextFile(GetMapSettingsFilename(mapName), lines);
		}

		internal static string StringArrayToString(string[] stringArray)
		{
			// Combine all strings in the array into a single string with line breaks
			var stringBuilder = new StringBuilder();
			foreach (string line in stringArray)
			{
				stringBuilder.AppendLine(line);
			}
			return stringBuilder.ToString();
		}

		internal static bool IsMultiplayerMapTemplate(string mapTemplate)
		{
			string filePath = Path.Combine(GetMapTemplatesDirectory(), mapTemplate, "mp.txt");
			return File.Exists(filePath);
		}

		internal static string GetLightOptions()
		{
			// Build lighting options string based on map settings
			var options = new StringBuilder();

			options.Append(mapSettings.GetBoolean("lightoptions_extra") ? " -extra" : " -fast");
			options.Append(mapSettings.GetBoolean("lightoptions_nomodelshadow") ? " -nomodelshadow" : " -modelshadow");

			if (mapSettings.GetBoolean("lightoptions_traces"))
			{
				options.Append(" -traces ").Append(mapSettings.GetDecimal("lightoptions_traces_val"));
			}

			if (mapSettings.GetBoolean("lightoptions_maxbounces"))
			{
				options.Append(" -maxbounces ").Append(mapSettings.GetDecimal("lightoptions_maxbounces_val"));
				options.Append(" -jitter ").Append(mapSettings.GetDecimal("lightoptions_jitter_val"));
			}

			if (mapSettings.GetBoolean("lightoptions_verbose"))
			{
				options.Append(" -verbose");
			}

			options.Append(" ");
			return options.ToString();
		}

		internal static string GetBspOptions()
		{
			// Build BSP options string based on map settings
			var options = new StringBuilder();

			if (mapSettings.GetBoolean("bspoptions_onlyents"))
			{
				options.Append(" -onlyents");
			}

			if (mapSettings.GetBoolean("bspoptions_blocksize"))
			{
				options.Append(" -blocksize ").Append(mapSettings.GetDecimal("lightoptions_blocksize_val"));
			}

			if (mapSettings.GetBoolean("bspoptions_samplescale"))
			{
				options.Append(" -samplescale ").Append(mapSettings.GetDecimal("lightoptions_samplescale_val"));
			}

			if (mapSettings.GetBoolean("bspoptions_debuglightmaps"))
			{
				options.Append(" -debugLightmaps");
			}

			// Append any extra options
			options.Append(mapSettings.GetString("bspoptions_extraoptions")).Append(" ");
			return options.ToString();
		}

		internal static bool IsMP(string name)
		{
			return name.StartsWith("mp_", StringComparison.OrdinalIgnoreCase);
		}

		internal static string MakeMP(string name)
		{
			return (IsMP(name)) ? name : "mp_" + name;
		}

		internal static string FilterMP(string name)
		{
			return (IsMP(name)) ? name.Substring(3) : name;
		}

		internal static void WriteMessage(string str, Color messageColour)
		{
			var console = TheLauncherForm.LauncherConsole;

			Color originalColour = console.SelectionColor;
			console.SelectionColor = messageColour;
			console.AppendText(str);
			console.SelectionColor = originalColour;
			console.Focus();
		}

		internal static void WriteMessage(string str)
		{
			WriteMessage(str, Color.SlateBlue);
		}

		internal static void WriteError(string str)
		{
			WriteMessage(str, Color.Red);
		}

		internal static string[] CreateMapFromTemplate(string mapTemplate, string mapName)
		{
			return CreateMapFromTemplate(mapTemplate, mapName, false);
		}

		internal static string[] CreateMapFromTemplate(
			string mapTemplate,
			string mapName,
			bool justCheckForOverwrite)
		{
			string[] overwriteFiles = Array.Empty<string>();
			string templateDir = CanonicalDirectory(Path.Combine(GetMapTemplatesDirectory(), mapTemplate));

			// Iterate all files in template directory matching "*template*"
			foreach (string templateFile in Launcher.GetFilesRecursively(templateDir, "*template*"))
			{
				string relativePath = templateFile.Substring(templateDir.Length).Replace("template", mapName);
				string destinationPath = Path.Combine(Launcher.GetRootDirectory(), relativePath);

				if (justCheckForOverwrite)
				{
					if (File.Exists(destinationPath))
					{
						StringArrayAdd(ref overwriteFiles, relativePath);
					}
				}
				else
				{
					string[] lines = LoadTextFile(templateFile);
					for (int i = 0; i < lines.Length; i++)
					{
						lines[i] = lines[i].Replace("template", mapName);
					}

					SaveTextFile(destinationPath, lines);
				}
			}
			return overwriteFiles;
		}

		internal static Decimal SetNumericUpDownValue(NumericUpDown ctrl, Decimal value)
		{
			decimal previousValue = ctrl.Value;
			if (value < ctrl.Minimum)
			{
				ctrl.Value = ctrl.Minimum;
			}
			else if (value > ctrl.Maximum)
			{
				ctrl.Value = ctrl.Maximum;
			}
			else
			{
				ctrl.Value = value;
			}
			return previousValue;
		}

		internal static void CreateZoneSourceFiles(string mapName)
		{
			using (var writer = new StreamWriter(GetZoneSourceFile(mapName)))
			{
				if (IsMP(mapName))
				{
					// Multiplayer

					writer.WriteLine("ignore,code_post_gfx_mp");
					writer.WriteLine("ignore,common_mp");
					writer.WriteLine($"col_map_mp,maps/mp/{mapName}.d3dbsp");
					writer.WriteLine($"rawfile,maps/mp/{mapName}.gsc");
					writer.WriteLine($"rawfile,maps/mp/{mapName}_fx.gsc");
					writer.WriteLine($"sound,common,{mapName},all_mp");
					writer.WriteLine($"sound,generic,{mapName},all_mp");
					writer.WriteLine($"sound,voiceovers,{mapName},all_mp");
					writer.WriteLine($"sound,multiplayer,{mapName},all_mp");
				}
				else
				{
					// Campaign/Nazi Zombies

					writer.WriteLine("ignore,code_post_gfx");
					writer.WriteLine("ignore,common");
					writer.WriteLine($"rawfile,maps/{mapName}.gsc");
					writer.WriteLine($"rawfile,maps/{mapName}_anim.gsc");
					writer.WriteLine($"rawfile,maps/{mapName}_amb.gsc");
					writer.WriteLine($"rawfile,maps/{mapName}_fx.gsc");
					writer.WriteLine($"sound,common,{mapName},all_sp");
					writer.WriteLine($"sound,generic,{mapName},all_sp");
					writer.WriteLine($"sound,voiceovers,{mapName},all_sp");
					writer.WriteLine($"sound,requests,{mapName},all_sp");
				}
			}
		}

		internal static string[] GetMapFiles(string mapName)
		{
			string[] mapFiles = Array.Empty<string>();
			string[] searchDirs = new string[]
			{
				GetMapSourceDirectory(),
				GetRawMapsDirectory(),
				GetZoneSourceDirectory()
			};

			string[] filePatterns = new string[]
			{
				".*",
				"_*.*"
			};

			string[] prefixes = new string[]
			{
				"",				// None/Regular IWDs
				"localized_"	// Localized IWDs
			};

			// Sigh... yes we're nesting here, but I don't care because it works fine
			foreach (string dir in searchDirs)
			{
				foreach (string pattern in filePatterns)
				{
					foreach (string prefix in prefixes)
					{
						// Enumerate files in the directory matching the current prefix + map name + pattern
						foreach (FileInfo file in new DirectoryInfo(dir).GetFiles(prefix + mapName + pattern))
						{
							StringArrayAdd(ref mapFiles, Path.Combine(dir, file.Name));
						}
					}
				}
			}
			return mapFiles;
		}

		internal static bool DeleteFile(string fileName)
		{
			return DeleteFile(fileName, true);
		}

		internal static bool DeleteFile(string fileName, bool verbose)
		{
			if (!File.Exists(fileName))
			{
				return true;
			}

			if (verbose)
			{
				WriteMessage($"Deleting {fileName}\n");
			}

			try
			{
				File.SetAttributes(fileName, FileAttributes.Normal);
				File.Delete(fileName);
				return true;
			}
			catch (Exception ex)
			{
				if (verbose)
				{
					WriteError($"ERROR: {ex.Message}\n");
				}
				return false;
			}
		}

		internal static bool CopyFile(string sourceFileName, string destinationFileName)
		{
			return CopyFile(sourceFileName, destinationFileName, false);
		}

		internal static bool CopyFileSmart(string sourceFileName, string destinationFileName)
		{
			return CopyFile(sourceFileName, destinationFileName, true);
		}

		internal static bool CopyFile(string sourceFileName, string destinationFileName, bool smartCopy)
		{
			if (!File.Exists(sourceFileName))
			{
				if (smartCopy)
				{
					DeleteFile(destinationFileName, false);
				}
				return false;
			}

			var sourceInfo = new FileInfo(sourceFileName);

			if (smartCopy)
			{
				var destInfo = new FileInfo(destinationFileName);
				if (destInfo.Exists &&
					sourceInfo.CreationTime == destInfo.CreationTime &&
					sourceInfo.LastWriteTime == destInfo.LastWriteTime &&
					sourceInfo.Length == destInfo.Length)
				{
					return true; // No need to copy, files are identical
				}
			}

			WriteMessage($"Copying  {sourceFileName}\n     to  {destinationFileName}\n");

			if (!DeleteFile(destinationFileName, false))
			{
				return false;
			}

			MakeDirectory(Path.GetDirectoryName(destinationFileName));

			try
			{
				File.Copy(sourceFileName, destinationFileName);
				if (smartCopy)
				{
					File.SetCreationTime(destinationFileName, sourceInfo.CreationTime);
					File.SetLastWriteTime(destinationFileName, sourceInfo.LastWriteTime);
				}
				return true;
			}
			catch (Exception ex)
			{
				WriteError($"ERROR: {ex.Message}\n");
				return false;
			}
		}

		internal static bool MoveFile(string sourceFileName, string destinationFileName)
		{
			if (string.IsNullOrWhiteSpace(sourceFileName) ||
				string.IsNullOrWhiteSpace(destinationFileName))
			{
				return false;
			}

			if (!File.Exists(sourceFileName))
			{
				return false;
			}

			WriteMessage($"Moving   {sourceFileName}\n    to   {destinationFileName}\n");

			if (!DeleteFile(destinationFileName, false))
			{
				return false;
			}

			MakeDirectory(Path.GetDirectoryName(destinationFileName));

			try
			{
				File.Move(sourceFileName, destinationFileName);
				return true;
			}
			catch (Exception ex)
			{
				WriteError($"ERROR: {ex.Message}\n");
				return false;
			}
		}

		internal static void PublishUsermaps()
		{
			string usermapsDirectory = GetUsermapsDirectory();
			foreach (string sourceFileName in GetFilesRecursively(usermapsDirectory, "*.ff"))
			{
				CopyFileSmart(
					sourceFileName,
					Path.Combine(GetLocalApplicationUsermapsDirectory(),
					sourceFileName.Substring(usermapsDirectory.Length)));
			}
		}

		internal static string[] GetModFilesByDirectory(string directory)
		{
			// Get all mod files recursively for specific extensions
			var ffFiles = GetFilesRecursively(directory, "*.ff");
			var iwdFiles = GetFilesRecursively(directory, "*.iwd");
			var arenaFiles = GetFilesRecursively(directory, "*.arena");

			// Combine all arrays into one
			var allFiles = new string[ffFiles.Length + iwdFiles.Length + arenaFiles.Length];
			ffFiles.CopyTo(allFiles, 0);
			iwdFiles.CopyTo(allFiles, ffFiles.Length);
			arenaFiles.CopyTo(allFiles, ffFiles.Length + iwdFiles.Length);
			return allFiles;
		}

		internal static string[] GetLocalApplicationModFiles(string modName)
		{
			return GetModFilesByDirectory(GetLocalApplicationModDirectory(modName));
		}

		internal static string[] GetModFiles(string modName)
		{
			return GetModFilesByDirectory(GetModDirectory(modName));
		}

		internal static void PublishMod(string modName)
		{
			string modDirectory = GetModDirectory(modName);
			foreach (string modFile in GetModFiles(modName))
			{
				CopyFileSmart(
					modFile,
					Path.Combine(GetLocalApplicationModDirectory(modName),
					modFile.Substring(modDirectory.Length)));
			}
		}

		internal static void PublishMods()
		{
			foreach (string mod in GetModList())
			{
				PublishMod(mod);
			}
		}

		internal static void Publish()
		{
			PublishUsermaps();
			PublishMods();
		}
	}
}