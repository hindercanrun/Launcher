// Decompiled with JetBrains decompiler
// Type: LauncherCS.Settings
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 95679BB7-C92C-4A2A-8DBF-1C9AAEB82003
// Assembly location: D:\pluto_t4_full_game\pluto_t4_full_game\bin\Launcher.exe

using System;
using System.Collections;

#nullable disable
namespace LauncherCS
{
  public struct Settings(Hashtable ht)
  {
    public Hashtable settings = ht;

    public void Set(Hashtable newSettings) => this.settings = newSettings;

    public Hashtable Get() => this.settings;

    public bool GetBoolean(string Key)
    {
      bool result = false;
      return bool.TryParse((string) this.settings[(object) Key], out result) && result;
    }

    public Decimal GetDecimal(string Key)
    {
      Decimal result = 0M;
      return !Decimal.TryParse((string) this.settings[(object) Key], out result) ? 0M : result;
    }

    public string GetString(string Key) => (string) this.settings[(object) Key] ?? "";

    public void SetBoolean(string Key, bool Value)
    {
      this.settings[(object) Key] = (object) Value.ToString();
    }

    public void SetDecimal(string Key, Decimal Value)
    {
      this.settings[(object) Key] = (object) Value.ToString();
    }

    public void SetString(string Key, string Value)
    {
      this.settings[(object) Key] = Value != null ? (object) Value : (object) "";
    }
  }
}
