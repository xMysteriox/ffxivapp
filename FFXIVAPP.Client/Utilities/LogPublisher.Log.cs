﻿// FFXIVAPP.Client
// LogPublisher.Log.cs
// 
// © 2013 Ryan Wilson

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using FFXIVAPP.Client.Memory;
using FFXIVAPP.Client.Plugins.Log;
using FFXIVAPP.Client.Plugins.Log.Views;
using FFXIVAPP.Client.Properties;
using FFXIVAPP.Common.Controls;
using FFXIVAPP.Common.RegularExpressions;
using FFXIVAPP.Common.Utilities;
using NLog;
using SmartAssembly.Attributes;

namespace FFXIVAPP.Client.Utilities
{
    public static partial class LogPublisher
    {
        [DoNotObfuscate]
        public static class Log
        {
            public static void Process(ChatEntry chatEntry)
            {
                // setup variables
                var timeStampColor = Settings.Default.TimeStampColor.ToString();
                var timeStamp = chatEntry.TimeStamp.ToString("[HH:mm:ss] ");
                var line = chatEntry.Line.Replace("  ", " ");
                var rawLine = line;
                var color = (Constants.Colors.ContainsKey(chatEntry.Code)) ? Constants.Colors[chatEntry.Code][0] : "FFFFFF";
                var isLS = Constants.Log.Linkshells.ContainsKey(chatEntry.Code);
                line = isLS ? Constants.Log.Linkshells[chatEntry.Code] + line : line;
                var playerName = "";
                try
                {
                    // handle tabs
                    if (CheckMode(chatEntry.Code, Constants.Log.ChatPublic))
                    {
                        playerName = line.Substring(0, line.IndexOf(":", StringComparison.Ordinal));
                        line = line.Replace(playerName + ":", "");
                    }
                    Common.Constants.FD.AppendFlow(timeStamp, playerName, line, new[]
                    {
                        timeStampColor, "#" + color
                    }, MainView.View.AllFD._FDR);
                    foreach (var flowDoc in PluginViewModel.Instance.Tabs.Select(ti => (xFlowDocument) ((TabItem) ti).Content))
                    {
                        var resuccess = false;
                        var xRegularExpression = flowDoc.RegEx.Text;
                        switch (xRegularExpression)
                        {
                            case "*":
                                resuccess = true;
                                break;
                            default:
                                try
                                {
                                    var check = new Regex(xRegularExpression);
                                    if (SharedRegEx.IsValidRegex(xRegularExpression))
                                    {
                                        var reg = check.Match(line);
                                        if (reg.Success)
                                        {
                                            resuccess = true;
                                        }
                                    }
                                }
                                catch
                                {
                                    resuccess = true;
                                }
                                break;
                        }
                        if (resuccess && flowDoc.Codes.Items.Contains(chatEntry.Code))
                        {
                            Common.Constants.FD.AppendFlow(timeStamp, playerName, line, new[]
                            {
                                timeStampColor, "#" + color
                            }, flowDoc._FDR);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
                }
                try
                {
                    // handle translation
                    if (Constants.Log.PluginSettings.EnableTranslate)
                    {
                        if (CheckMode(chatEntry.Code, Constants.Log.ChatSay) && Constants.Log.PluginSettings.TranslateSay)
                        {
                            GoogleTranslate.RetreiveLang(rawLine, chatEntry.JP);
                        }
                        if (CheckMode(chatEntry.Code, Constants.Log.ChatTell) && Constants.Log.PluginSettings.TranslateTell)
                        {
                            GoogleTranslate.RetreiveLang(rawLine, chatEntry.JP);
                        }
                        if (CheckMode(chatEntry.Code, Constants.Log.ChatParty) && Constants.Log.PluginSettings.TranslateParty)
                        {
                            GoogleTranslate.RetreiveLang(rawLine, chatEntry.JP);
                        }
                        if (CheckMode(chatEntry.Code, Constants.Log.ChatShout) && Constants.Log.PluginSettings.TranslateShout)
                        {
                            GoogleTranslate.RetreiveLang(rawLine, chatEntry.JP);
                        }
                        if (CheckMode(chatEntry.Code, Constants.Log.ChatYell) && Constants.Log.PluginSettings.TranslateYell)
                        {
                            GoogleTranslate.RetreiveLang(rawLine, chatEntry.JP);
                        }
                        if (CheckMode(chatEntry.Code, Constants.Log.ChatLS) && Constants.Log.PluginSettings.TranslateLS)
                        {
                            GoogleTranslate.RetreiveLang(rawLine, chatEntry.JP);
                        }
                        if (CheckMode(chatEntry.Code, Constants.Log.ChatFC) && Constants.Log.PluginSettings.TranslateFC)
                        {
                            GoogleTranslate.RetreiveLang(rawLine, chatEntry.JP);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
                }
                try
                {
                    // handle debug tab
                    if (Constants.Log.PluginSettings.ShowAsciiDebug)
                    {
                        var asciiString = "";
                        for (var j = 0; j < chatEntry.Bytes.Length; j++)
                        {
                            asciiString += chatEntry.Bytes[j].ToString(CultureInfo.CurrentUICulture) + " ";
                        }
                        asciiString = asciiString.Trim();
                        Common.Constants.FD.AppendFlow("", "", asciiString, new[]
                        {
                            "", "#FFFFFFFF"
                        }, MainView.View.DebugFD._FDR);
                    }
                    var raw = String.Format("{0}[{1}]{2}", chatEntry.Raw.Substring(0, 8), chatEntry.Code, chatEntry.Raw.Substring(12));
                    Common.Constants.FD.AppendFlow("", "", raw, new[]
                    {
                        "", "#FFFFFFFF"
                    }, MainView.View.DebugFD._FDR);
                }
                catch (Exception ex)
                {
                    Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
                }
            }

            /// <summary>
            /// </summary>
            /// <param name="chatMode"> </param>
            /// <param name="log"> </param>
            /// <returns> </returns>
            private static bool CheckMode(string chatMode, IEnumerable<string> log)
            {
                return log.Any(t => t == chatMode);
            }
        }
    }
}
