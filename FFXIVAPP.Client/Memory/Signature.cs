﻿// FFXIVAPP.Client
// Signature.cs
// 
// © 2013 Ryan Wilson

#region Usings

using System.Text.RegularExpressions;

#endregion

namespace FFXIVAPP.Client.Memory
{
    internal class Signature
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public Regex RegularExpress { get; set; }
        public int Offset { get; set; }
    }
}