// Decompiled with JetBrains decompiler
// Type: LauncherCS.DVar
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 95679BB7-C92C-4A2A-8DBF-1C9AAEB82003
// Assembly location: D:\pluto_t4_full_game\pluto_t4_full_game\bin\Launcher.exe

using System;

#nullable disable
namespace LauncherCS
{
  public struct DVar
  {
    public string name;
    public string description;
    public bool isDecimal;
    public Decimal decimalMin;
    public Decimal decimalMax;
    public Decimal decimalIncrement;

    public DVar(string name, string description)
    {
      this.name = name;
      this.description = description;
      this.isDecimal = false;
      this.decimalMin = 0M;
      this.decimalMax = 0M;
      this.decimalIncrement = 0M;
    }

    public DVar(
      string name,
      string description,
      Decimal decimalMin,
      Decimal decimalMax,
      Decimal decimalIncrement)
    {
      this.name = name;
      this.description = description;
      this.isDecimal = true;
      this.decimalMin = decimalMin;
      this.decimalMax = decimalMax;
      this.decimalIncrement = decimalIncrement;
    }

    public DVar(string name, string description, Decimal decimalMin, Decimal decimalMax)
    {
      this.name = name;
      this.description = description;
      this.isDecimal = true;
      this.decimalMin = decimalMin;
      this.decimalMax = decimalMax;
      this.decimalIncrement = 1M;
    }
  }
}
