﻿// FFXIVAPP.Client
// DamageOverTimeAction.cs
// 
// © 2013 Ryan Wilson

using SmartAssembly.Attributes;

namespace FFXIVAPP.Client.Plugins.Parse.Models
{
    [DoNotObfuscate]
    public class DamageOverTimeAction
    {
        public int ActionPotency { get; set; }
        public int DamageOverTimePotency { get; set; }
        public int Duration { get; set; }
        public bool ZeroBaseDamageDOT { get; set; }
    }
}
