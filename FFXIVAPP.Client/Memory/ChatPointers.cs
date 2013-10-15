﻿// FFXIVAPP.Client
// ChatPointers.cs
// 
// © 2013 Ryan Wilson

#region Usings

using System.Runtime.InteropServices;
using SmartAssembly.Attributes;

#endregion

namespace FFXIVAPP.Client.Memory
{
    [DoNotObfuscate]
    [StructLayout(LayoutKind.Sequential)]
    internal struct ChatPointers
    {
        public uint LineCount1;
        //public uint LineCount2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
        public byte[] Unk1;

        public uint OffsetArrayStart;
        public uint OffsetArrayPos;
        public uint OffsetArrayEnd;
        public uint Unk2;
        public uint LogStart;
        public uint LogNext;
        public uint LogEnd;
    }
}
