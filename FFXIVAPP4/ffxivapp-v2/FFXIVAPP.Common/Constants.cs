﻿// FFXIVAPP.Common
// Constants.cs
//  
// Created by Ryan Wilson.
// Copyright © 2007-2013 Ryan Wilson - All Rights Reserved

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using FFXIVAPP.Common.Helpers;

#endregion

namespace FFXIVAPP.Common
{
    public static class Constants
    {
        public const string AppPack = "pack://application:,,,/FFXIVAPP.Client;component/";
        public const string ChatPack = "pack://application:,,,/FFXIVAPP.Plugin.Chat;component/";
        public const string EventPack = "pack://application:,,,/FFXIVAPP.Plugin.Event;component/";
        public const string LogPack = "pack://application:,,,/FFXIVAPP.Plugin.Log;component/";
        public const string ParsePack = "pack://application:,,,/FFXIVAPP.Plugin.Parse;component/";
        public const string DefaultIcon = AppPack + "Resources/Media/Images/DefaultIcon.jpg";
        public const string DefaultAvatar = AppPack + "Resources/Media/Images/DefaultAvatar.jpg";

        public static readonly string[] Supported = new[]
        {
            "ja", "fr", "en"
        };

        public static readonly FlowDocHelper FD = new FlowDocHelper();

        public static readonly string[] ChatPublic =
        {
            "000A", "000C", "000D", "000E"
        };

        public static readonly string[] ChatSay =
        {
            "000A"
        };

        public static readonly string[] ChatTell =
        {
            "000C", "000D"
        };

        public static readonly string[] ChatParty =
        {
            "000E"
        };

        public static readonly string[] ChatShout =
        {
            "000B"
        };

        public static readonly string[] ChatYell =
        {
            "001E"
        };

        public static readonly string[] ChatLS =
        {
            "0010", "0011", "0012", "0013", "0014", "0015", "0016", "0017"
        };

        public static readonly string[] ChatFC =
        {
            "0018"
        };

        public static bool IsValidRegex(string pattern)
        {
            if (String.IsNullOrWhiteSpace(pattern))
            {
                return false;
            }
            try
            {
                var regex = new Regex(pattern);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        #region Property Bindings

        private static Dictionary<string, string> _autoTranslate;
        private static Dictionary<string, string> _chatCodes;
        private static Dictionary<string, string[]> _colors;
        private static CultureInfo _cultureInfo;

        public static Dictionary<string, string> AutoTranslate
        {
            get { return _autoTranslate ?? (_autoTranslate = new Dictionary<string, string>()); }
            set { _autoTranslate = value; }
        }

        public static Dictionary<string, string> ChatCodes
        {
            get { return _chatCodes ?? (_chatCodes = new Dictionary<string, string>()); }
            set { _chatCodes = value; }
        }

        public static string ChatCodesXml { get; set; }

        public static Dictionary<string, string[]> Colors
        {
            get { return _colors ?? (_colors = new Dictionary<string, string[]>()); }
            set { _colors = value; }
        }

        public static CultureInfo CultureInfo
        {
            get { return _cultureInfo ?? (_cultureInfo = new CultureInfo("en")); }
            set { _cultureInfo = value; }
        }

        public static string CharacterName { get; set; }

        public static string ServerName { get; set; }

        public static string GameLanguage { get; set; }

        public static IntPtr ProcessHandle { get; set; }

        public static int ProcessID { get; set; }

        public static bool IsOpen { get; set; }

        public static Process[] ProcessIDs { get; set; }

        public static bool EnableNLog { get; set; }

        #endregion
    }
}