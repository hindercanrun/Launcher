// Decompiled with JetBrains decompiler
// Type: LauncherCS.Launcher
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 95679BB7-C92C-4A2A-8DBF-1C9AAEB82003
// Assembly location: D:\pluto_t4_full_game\pluto_t4_full_game\bin\Launcher.exe

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace LauncherCS
{
  internal static class Launcher
  {
    public static LauncherForm TheLauncherForm = (LauncherForm) null;
    public static Settings launcherSettings = new Settings();
    public static Settings mapSettings = new Settings();
    public static DVar[] dvars = new DVar[10]
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
      string name = "";
      foreach (char ch in (Launcher.GetRootDirectory() + Application.ProductName).ToLower())
        name += (string) (object) (char) (ch == '\\' || ch == ':' ? 45 : (int) ch);
      bool createdNew;
      Mutex mutex = new Mutex(true, name, out createdNew);
      if (!createdNew)
      {
        int num = (int) MessageBox.Show("Only one instance of the " + Application.ProductName + " is allowed at a time.");
      }
      else
      {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run((Form) (Launcher.TheLauncherForm = new LauncherForm()));
      }
    }

    public static string GetLanguage() => "english";

    public static void MakeDirectory(string directoryName)
    {
      while (!Directory.Exists(directoryName))
      {
        string directoryName1 = Path.GetDirectoryName(directoryName);
        if (directoryName1 != directoryName)
          Launcher.MakeDirectory(directoryName1);
        Directory.CreateDirectory(directoryName);
      }
    }

    public static string CanonicalDirectory(string path)
    {
      FileInfo fileInfo = new FileInfo(path + "." + (object) Path.DirectorySeparatorChar);
      Launcher.MakeDirectory(fileInfo.DirectoryName);
      return fileInfo.DirectoryName + (object) Path.DirectorySeparatorChar;
    }

    private static string GetLocalApplicationDirectory()
    {
      return Launcher.CanonicalDirectory(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Activision"), "CodWaW"));
    }

    private static string GetLocalApplicationUsermapsDirectory()
    {
      return Launcher.CanonicalDirectory(Path.Combine(Launcher.GetLocalApplicationDirectory(), "usermaps"));
    }

    private static string GetLocalApplicationModsDirectory()
    {
      return Launcher.CanonicalDirectory(Path.Combine(Launcher.GetLocalApplicationDirectory(), "mods"));
    }

    private static string GetLocalApplicationModDirectory(string modName)
    {
      return Launcher.CanonicalDirectory(Path.Combine(Launcher.GetLocalApplicationModsDirectory(), modName));
    }

    public static string GetStartupDirectory()
    {
      return Launcher.CanonicalDirectory(Path.GetFullPath("."));
    }

    public static string GetRootDirectory()
    {
      return Launcher.CanonicalDirectory(Path.Combine(Launcher.GetStartupDirectory(), ".."));
    }

    public static string GetBinDirectory()
    {
      return Launcher.CanonicalDirectory(Path.Combine(Launcher.GetRootDirectory(), "bin"));
    }

    public static string GetMapSourceDirectory()
    {
      return Launcher.CanonicalDirectory(Path.Combine(Launcher.GetRootDirectory(), "map_source"));
    }

    public static string GetRawDirectory()
    {
      return Launcher.CanonicalDirectory(Path.Combine(Launcher.GetRootDirectory(), "raw"));
    }

    public static string GetRawMapsDirectory()
    {
      return Launcher.CanonicalDirectory(Path.Combine(Launcher.GetRawDirectory(), "maps"));
    }

    public static string GetModsDirectory()
    {
      return Launcher.CanonicalDirectory(Path.Combine(Launcher.GetRootDirectory(), "mods"));
    }

    public static string GetUsermapsDirectory()
    {
      return Launcher.CanonicalDirectory(Path.Combine(Launcher.GetRootDirectory(), "usermaps"));
    }

    public static string GetModDirectory(string modName)
    {
      return Launcher.CanonicalDirectory(Path.Combine(Launcher.GetModsDirectory(), modName));
    }

    public static string GetMapSettingsDirectory()
    {
      return Launcher.CanonicalDirectory(Path.Combine(Launcher.GetStartupDirectory(), Path.Combine(Application.ProductName, "map_settings")));
    }

    public static string GetMapTemplatesDirectory()
    {
      return Launcher.CanonicalDirectory(Path.Combine(Launcher.GetStartupDirectory(), Path.Combine(Application.ProductName, "map_templates")));
    }

    public static string GetZoneSourceDirectory()
    {
      return Launcher.CanonicalDirectory(Path.Combine(Launcher.GetRootDirectory(), "zone_source"));
    }

    public static string GetZoneDirectory()
    {
      return Launcher.CanonicalDirectory(Path.Combine(Path.Combine(Launcher.GetRootDirectory(), "zone"), Launcher.GetLanguage()));
    }

    public static string GetZoneSourceFile(string mapName)
    {
      return Path.Combine(Launcher.GetZoneSourceDirectory(), mapName + ".csv");
    }

    public static string GetLoadZone(string mapName) => mapName + "_load";

    public static string GetZoneSourceLoadCSVFile(string mapName)
    {
      return Path.Combine(Launcher.GetZoneSourceDirectory(), Launcher.GetLoadZone(mapName) + ".csv");
    }

    public static void StringArrayAdd(ref string[] stringArray, string stringItem)
    {
      Array.Resize<string>(ref stringArray, stringArray.Length + 1);
      stringArray[stringArray.Length - 1] = stringItem;
    }

    public static string GetGameTool(bool mpVersion) => !mpVersion ? "../sp_tool" : "../mp_tool";

    public static string GetGameApplication(bool mpVersion)
    {
      return !mpVersion ? "../CoDWaW" : "../CoDWaWmp";
    }

    public static string[] GetDirs(string directory)
    {
      string[] stringArray = new string[0];
      foreach (DirectoryInfo directory1 in new DirectoryInfo(directory).GetDirectories())
        Launcher.StringArrayAdd(ref stringArray, directory1.Name);
      return stringArray;
    }

    public static string[] GetFilesRecursively(string directory)
    {
      return Launcher.GetFilesRecursively(directory, "*");
    }

    public static string[] GetFilesRecursively(string directory, string filesToIncludeFilter)
    {
      string[] files = new string[0];
      Launcher.GetFilesRecursively(directory, filesToIncludeFilter, ref files);
      return files;
    }

    public static void GetFilesRecursively(
      string directory,
      string filesToIncludeFilter,
      ref string[] files)
    {
      foreach (DirectoryInfo directory1 in new DirectoryInfo(directory).GetDirectories())
        Launcher.GetFilesRecursively(Path.Combine(directory, directory1.Name), filesToIncludeFilter, ref files);
      foreach (FileInfo file in new DirectoryInfo(directory).GetFiles(filesToIncludeFilter))
        Launcher.StringArrayAdd(ref files, Path.Combine(directory, file.Name.ToLower()));
    }

    public static string[] GetFiles(string directory, string searchFilter)
    {
      string[] stringArray = new string[0];
      foreach (FileInfo file in new DirectoryInfo(directory).GetFiles(searchFilter))
        Launcher.StringArrayAdd(ref stringArray, Path.GetFileName(file.Name));
      return stringArray;
    }

    public static string[] GetFilesWithoutExtension(string directory, string searchFilter)
    {
      string[] stringArray = new string[0];
      foreach (FileInfo file in new DirectoryInfo(directory).GetFiles(searchFilter))
        Launcher.StringArrayAdd(ref stringArray, Path.GetFileNameWithoutExtension(file.Name));
      return stringArray;
    }

    public static string[] GetMapList()
    {
      return Launcher.GetFilesWithoutExtension(Launcher.GetMapSourceDirectory(), "*.map");
    }

    public static string[] GetMapTemplatesList()
    {
      return Launcher.GetDirs(Launcher.GetMapTemplatesDirectory());
    }

    public static string[] GetModList() => Launcher.GetDirs(Launcher.GetModsDirectory());

    public static string[] LoadTextFile(string textFile, string skipCommentLinesStartingWith)
    {
      string[] stringArray = new string[0];
      try
      {
        using (StreamReader streamReader = new StreamReader(textFile))
        {
          string stringItem;
          while ((stringItem = streamReader.ReadLine()) != null)
          {
            stringItem.Trim();
            if (!(stringItem == "") && (skipCommentLinesStartingWith == null || !stringItem.StartsWith(skipCommentLinesStartingWith)))
              Launcher.StringArrayAdd(ref stringArray, stringItem);
          }
        }
      }
      catch
      {
      }
      return stringArray;
    }

    public static string[] LoadTextFile(string textFile)
    {
      return Launcher.LoadTextFile(textFile, (string) null);
    }

    public static void SaveTextFile(string textFile, string[] text)
    {
      using (StreamWriter streamWriter = new StreamWriter(textFile))
      {
        foreach (string str in text)
          streamWriter.WriteLine(str);
      }
    }

    public static Hashtable StringArrayToHashTable(string[] stringArray)
    {
      Hashtable hashTable = new Hashtable(stringArray.Length);
      foreach (string str in stringArray)
      {
        char[] chArray = new char[1]{ ',' };
        string[] strArray = str.Split(chArray);
        if (strArray.Length > 0)
          hashTable.Add((object) strArray[0], strArray.Length > 1 ? (object) strArray[1] : (object) (string) null);
      }
      return hashTable;
    }

    public static string[] HashTableToStringArray(Hashtable hashTable)
    {
      int num = 0;
      string[] array = new string[hashTable.Count];
      foreach (DictionaryEntry dictionaryEntry in hashTable)
        array[num++] = dictionaryEntry.Key.ToString() + (dictionaryEntry.Value != null ? (object) ("," + dictionaryEntry.Value) : (object) "");
      Array.Sort<string>(array);
      return array;
    }

    private static string GetMapSettingsFilename(string mapName)
    {
      return Path.Combine(Launcher.GetMapSettingsDirectory(), mapName + ".cfg");
    }

    public static Hashtable LoadMapSettings(string mapName)
    {
      return Launcher.StringArrayToHashTable(Launcher.LoadTextFile(Launcher.GetMapSettingsFilename(mapName)));
    }

    public static void SaveMapSettings(string mapName, Hashtable mapSettings)
    {
      Launcher.SaveTextFile(Launcher.GetMapSettingsFilename(mapName), Launcher.HashTableToStringArray(mapSettings));
    }

    public static string StringArrayToString(string[] stringArray)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string str in stringArray)
        stringBuilder.Append(str).AppendLine();
      return stringBuilder.ToString();
    }

    public static bool IsMultiplayerMapTemplate(string mapTemplate)
    {
      return File.Exists(Path.Combine(Path.Combine(Launcher.GetMapTemplatesDirectory(), mapTemplate), "mp.txt"));
    }

    public static string GetLightOptions()
    {
      return (Launcher.mapSettings.GetBoolean("lightoptions_extra") ? " -extra" : " -fast") + (Launcher.mapSettings.GetBoolean("lightoptions_nomodelshadow") ? " -nomodelshadow" : " -modelshadow") + (Launcher.mapSettings.GetBoolean("lightoptions_traces") ? " -traces " + Launcher.mapSettings.GetDecimal("lightoptions_traces_val").ToString() : "") + (Launcher.mapSettings.GetBoolean("lightoptions_maxbounces") ? " -maxbounces " + Launcher.mapSettings.GetDecimal("lightoptions_maxbounces_val").ToString() : "") + (Launcher.mapSettings.GetBoolean("lightoptions_maxbounces") ? " -jitter " + Launcher.mapSettings.GetDecimal("lightoptions_jitter_val").ToString() : "") + (Launcher.mapSettings.GetBoolean("lightoptions_verbose") ? " -verbose" : "") + " ";
    }

    public static string GetBspOptions()
    {
      return (Launcher.mapSettings.GetBoolean("bspoptions_onlyents") ? " -onlyents" : "") + (Launcher.mapSettings.GetBoolean("bspoptions_blocksize") ? " -blocksize " + Launcher.mapSettings.GetDecimal("lightoptions_blocksize_val").ToString() : "") + (Launcher.mapSettings.GetBoolean("bspoptions_samplescale") ? " -samplescale " + Launcher.mapSettings.GetDecimal("lightoptions_samplescale_val").ToString() : "") + (Launcher.mapSettings.GetBoolean("bspoptions_debuglightmaps") ? " -debugLightmaps" : "") + Launcher.mapSettings.GetString("bspoptions_extraoptions") + " ";
    }

    public static bool IsMP(string name) => name.ToLower().StartsWith("mp_");

    public static string MakeMP(string name) => !Launcher.IsMP(name) ? "mp_" + name : name;

    public static string FilterMP(string name) => !Launcher.IsMP(name) ? name : name.Substring(3);

    public static void WriteMessage(string s, Color messageColor)
    {
      Color selectionColor = Launcher.TheLauncherForm.LauncherConsole.SelectionColor;
      Launcher.TheLauncherForm.LauncherConsole.SelectionColor = messageColor;
      Launcher.TheLauncherForm.LauncherConsole.AppendText(s);
      Launcher.TheLauncherForm.LauncherConsole.SelectionColor = selectionColor;
      Launcher.TheLauncherForm.LauncherConsole.Focus();
    }

    public static void WriteMessage(string s) => Launcher.WriteMessage(s, Color.SlateBlue);

    public static void WriteError(string s) => Launcher.WriteMessage(s, Color.Red);

    public static string[] CreateMapFromTemplate(string mapTemplate, string mapName)
    {
      return Launcher.CreateMapFromTemplate(mapTemplate, mapName, false);
    }

    public static string[] CreateMapFromTemplate(
      string mapTemplate,
      string mapName,
      bool justCheckForOverwrite)
    {
      string[] stringArray = new string[0];
      string directory = Launcher.CanonicalDirectory(Path.Combine(Launcher.GetMapTemplatesDirectory(), mapTemplate));
      foreach (string textFile in Launcher.GetFilesRecursively(directory, "*template*"))
      {
        string str1 = textFile.Substring(directory.Length).Replace("template", mapName);
        string str2 = Path.Combine(Launcher.GetRootDirectory(), str1);
        if (justCheckForOverwrite)
        {
          if (File.Exists(str2))
            Launcher.StringArrayAdd(ref stringArray, str1);
        }
        else
        {
          string[] text = Launcher.LoadTextFile(textFile);
          int num = 0;
          foreach (string str3 in text)
            text[num++] = str3.Replace("template", mapName);
          Launcher.SaveTextFile(str2, text);
        }
      }
      return stringArray;
    }

    public static Decimal SetNumericUpDownValue(NumericUpDown ctrl, Decimal Value)
    {
      Decimal num = ctrl.Value;
      ctrl.Value = !(Value < ctrl.Minimum) ? (!(Value > ctrl.Maximum) ? Value : ctrl.Maximum) : ctrl.Minimum;
      return num;
    }

    public static void CreateZoneSourceFiles(string mapName)
    {
      using (StreamWriter streamWriter = new StreamWriter(Launcher.GetZoneSourceFile(mapName)))
      {
        if (Launcher.IsMP(mapName))
        {
          streamWriter.WriteLine("ignore,code_post_gfx_mp");
          streamWriter.WriteLine("ignore,common_mp");
          streamWriter.WriteLine("col_map_mp,maps/mp/" + mapName + ".d3dbsp");
          streamWriter.WriteLine("rawfile,maps/mp/" + mapName + ".gsc");
          streamWriter.WriteLine("rawfile,maps/mp/" + mapName + "_fx.gsc");
          streamWriter.WriteLine("sound,common," + mapName + ",all_mp");
          streamWriter.WriteLine("sound,generic," + mapName + ",all_mp");
          streamWriter.WriteLine("sound,voiceovers," + mapName + ",all_mp");
          streamWriter.WriteLine("sound,multiplayer," + mapName + ",all_mp");
        }
        else
        {
          streamWriter.WriteLine("ignore,code_post_gfx");
          streamWriter.WriteLine("ignore,common");
          streamWriter.WriteLine("col_map_sp,maps/" + mapName + ".d3dbsp");
          streamWriter.WriteLine("rawfile,maps/" + mapName + ".gsc");
          streamWriter.WriteLine("rawfile,maps/" + mapName + "_anim.gsc");
          streamWriter.WriteLine("rawfile,maps/" + mapName + "_amb.gsc");
          streamWriter.WriteLine("rawfile,maps/" + mapName + "_fx.gsc");
          streamWriter.WriteLine("sound,common," + mapName + ",all_sp");
          streamWriter.WriteLine("sound,generic," + mapName + ",all_sp");
          streamWriter.WriteLine("sound,voiceovers," + mapName + ",all_sp");
          streamWriter.WriteLine("sound,requests," + mapName + ",all_sp");
        }
      }
    }

    public static string[] GetMapFiles(string mapName)
    {
      string[] stringArray = new string[0];
      string[] strArray1 = new string[3]
      {
        Launcher.GetMapSourceDirectory(),
        Launcher.GetRawMapsDirectory(),
        Launcher.GetZoneSourceDirectory()
      };
      foreach (string str1 in strArray1)
      {
        string[] strArray2 = new string[2]{ ".*", "_*.*" };
        foreach (string str2 in strArray2)
        {
          string[] strArray3 = new string[2]
          {
            "",
            "localized_"
          };
          foreach (string str3 in strArray3)
          {
            foreach (FileInfo file in new DirectoryInfo(str1).GetFiles(str3 + mapName + str2))
              Launcher.StringArrayAdd(ref stringArray, Path.Combine(str1, file.Name));
          }
        }
      }
      return stringArray;
    }

    public static bool DeleteFile(string fileName) => Launcher.DeleteFile(fileName, true);

    public static bool DeleteFile(string fileName, bool verbose)
    {
      if (!File.Exists(fileName))
        return true;
      if (verbose)
        Launcher.WriteMessage("Deleting " + fileName + "\n");
      try
      {
        File.SetAttributes(fileName, FileAttributes.Normal);
        File.Delete(fileName);
      }
      catch (Exception ex)
      {
        if (verbose)
          Launcher.WriteError("ERROR: " + ex.Message + "\n");
        return false;
      }
      return true;
    }

    public static bool CopyFile(string sourceFileName, string destinationFileName)
    {
      return Launcher.CopyFile(sourceFileName, destinationFileName, false);
    }

    public static bool CopyFileSmart(string sourceFileName, string destinationFileName)
    {
      return Launcher.CopyFile(sourceFileName, destinationFileName, true);
    }

    public static bool CopyFile(string sourceFileName, string destinationFileName, bool smartCopy)
    {
      if (!File.Exists(sourceFileName))
      {
        if (smartCopy)
          Launcher.DeleteFile(destinationFileName, false);
        return false;
      }
      FileInfo fileInfo1 = new FileInfo(sourceFileName);
      if (smartCopy)
      {
        FileInfo fileInfo2 = new FileInfo(destinationFileName);
        if (fileInfo1.Exists && fileInfo2.Exists && fileInfo1.CreationTime == fileInfo2.CreationTime && fileInfo1.LastWriteTime == fileInfo2.LastWriteTime && fileInfo1.Length == fileInfo2.Length)
          return true;
      }
      Launcher.WriteMessage("Copying  " + sourceFileName + "\n     to  " + destinationFileName + "\n");
      if (!Launcher.DeleteFile(destinationFileName, false))
        return false;
      Launcher.MakeDirectory(Path.GetDirectoryName(destinationFileName));
      try
      {
        File.Copy(sourceFileName, destinationFileName);
        if (smartCopy)
        {
          File.SetCreationTime(destinationFileName, fileInfo1.CreationTime);
          File.SetLastWriteTime(destinationFileName, fileInfo1.LastWriteTime);
        }
      }
      catch (Exception ex)
      {
        Launcher.WriteError("ERROR: " + ex.Message + "\n");
        return false;
      }
      return true;
    }

    public static bool MoveFile(string sourceFileName, string destinationFileName)
    {
      if (!File.Exists(sourceFileName))
        return false;
      Launcher.WriteMessage("Moving   " + sourceFileName + "\n    to   " + destinationFileName + "\n");
      if (!Launcher.DeleteFile(destinationFileName, false))
        return false;
      Launcher.MakeDirectory(Path.GetDirectoryName(destinationFileName));
      try
      {
        File.Move(sourceFileName, destinationFileName);
      }
      catch (Exception ex)
      {
        Launcher.WriteError("ERROR: " + ex.Message + "\n");
        return false;
      }
      return true;
    }

    public static void PublishUsermaps()
    {
      string usermapsDirectory = Launcher.GetUsermapsDirectory();
      foreach (string sourceFileName in Launcher.GetFilesRecursively(usermapsDirectory, "*.ff"))
        Launcher.CopyFileSmart(sourceFileName, Path.Combine(Launcher.GetLocalApplicationUsermapsDirectory(), sourceFileName.Substring(usermapsDirectory.Length)));
    }

    public static string[] GetModFilesByDirectory(string directory)
    {
      string[] filesRecursively1 = Launcher.GetFilesRecursively(directory, "*.ff");
      string[] filesRecursively2 = Launcher.GetFilesRecursively(directory, "*.iwd");
      string[] filesRecursively3 = Launcher.GetFilesRecursively(directory, "*.arena");
      string[] filesByDirectory = new string[filesRecursively1.Length + filesRecursively2.Length + filesRecursively3.Length];
      filesRecursively1.CopyTo((Array) filesByDirectory, 0);
      filesRecursively2.CopyTo((Array) filesByDirectory, filesRecursively1.Length);
      filesRecursively3.CopyTo((Array) filesByDirectory, filesRecursively1.Length + filesRecursively2.Length);
      return filesByDirectory;
    }

    public static string[] GetLocalApplicationModFiles(string modName)
    {
      return Launcher.GetModFilesByDirectory(Launcher.GetLocalApplicationModDirectory(modName));
    }

    public static string[] GetModFiles(string modName)
    {
      return Launcher.GetModFilesByDirectory(Launcher.GetModDirectory(modName));
    }

    public static void PublishMod(string modName)
    {
      string modDirectory = Launcher.GetModDirectory(modName);
      foreach (string modFile in Launcher.GetModFiles(modName))
        Launcher.CopyFileSmart(modFile, Path.Combine(Launcher.GetLocalApplicationModDirectory(modName), modFile.Substring(modDirectory.Length)));
    }

    public static void PublishMods()
    {
      foreach (string mod in Launcher.GetModList())
        Launcher.PublishMod(mod);
    }

    public static void Publish()
    {
      Launcher.PublishUsermaps();
      Launcher.PublishMods();
    }
  }
}
