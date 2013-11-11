﻿// FFXIVAPP.Client
// ChatWorker.cs
// 
// © 2013 Ryan Wilson

#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using FFXIVAPP.Common.Utilities;
using NLog;
using SmartAssembly.Attributes;
using Timer = System.Timers.Timer;

#endregion

namespace FFXIVAPP.Client.Memory
{
    [DoNotObfuscate]
    internal class ChatWorker : INotifyPropertyChanged, IDisposable
    {
        #region Property Bindings

        private uint ChatPointerMap { get; set; }

        #endregion

        #region Declarations

        private static readonly Logger Tracer = LogManager.GetCurrentClassLogger();
        private readonly Timer _scanTimer;
        private readonly BackgroundWorker _scanner = new BackgroundWorker();
        private readonly List<uint> _spots = new List<uint>();
        private readonly SynchronizationContext _sync = SynchronizationContext.Current;
        private bool _isScanning;
        private uint _lastChatNum;
        private int _lastCount;

        #endregion

        #region Events

        public event NewLineEventHandler OnNewline = delegate { };

        /// <summary>
        /// </summary>
        /// <param name="chatEntry"> </param>
        private void PostLineEvent(ChatEntry chatEntry)
        {
            _sync.Post(RaiseLineEvent, chatEntry);
        }

        /// <summary>
        /// </summary>
        /// <param name="state"> </param>
        private void RaiseLineEvent(object state)
        {
            OnNewline((ChatEntry) state);
        }

        #endregion

        #region Delegates

        public delegate void NewLineEventHandler(ChatEntry chatEntry);

        #endregion

        public ChatWorker()
        {
            _scanTimer = new Timer(10);
            _scanTimer.Elapsed += ScanTimerElapsed;
        }

        #region Timer Controls

        /// <summary>
        /// </summary>
        public void StartScanning()
        {
            _scanTimer.Enabled = true;
        }

        /// <summary>
        /// </summary>
        public void StopScanning()
        {
            _scanTimer.Enabled = false;
        }

        #endregion

        #region Threads

        /// <summary>
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void ScanTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_isScanning)
            {
                return;
            }
            _isScanning = true;
            Func<bool> scannerWorker = delegate
            {
                if (MemoryHandler.Instance.SigScanner.Locations.ContainsKey("GAMEMAIN"))
                {
                    if (MemoryHandler.Instance.SigScanner.Locations.ContainsKey("CHATLOG"))
                    {
                        ChatPointerMap = MemoryHandler.Instance.SigScanner.Locations["CHATLOG"];
                        if (ChatPointerMap <= 20)
                        {
                            return false;
                        }
                        var chatPointers = MemoryHandler.Instance.GetStructure<Structures.ChatLog>(ChatPointerMap);
                        try
                        {
                            if (_lastCount == 0)
                            {
                                _lastCount = (int) chatPointers.LineCount;
                            }
                            if (_lastCount != chatPointers.LineCount)
                            {
                                _spots.Clear();
                                var index = (int) (chatPointers.OffsetArrayPos - chatPointers.OffsetArrayStart) / 4;
                                var offset = (int) (chatPointers.OffsetArrayEnd - chatPointers.OffsetArrayStart) / 4;
                                var lengths = new List<int>();
                                for (var i = chatPointers.LineCount - _lastCount; i > 0; i--)
                                {
                                    var getline = ((index - i) < 0) ? (index - i) + offset : index - i;
                                    int lineLen;
                                    if (getline == 0)
                                    {
                                        lineLen = MemoryHandler.Instance.GetInt32(chatPointers.OffsetArrayStart);
                                    }
                                    else
                                    {
                                        var previousAddress = chatPointers.OffsetArrayStart + (uint) ((getline - 1) * 4);
                                        var previous = MemoryHandler.Instance.GetInt32(previousAddress);
                                        var currentAddress = chatPointers.OffsetArrayStart + (uint) (getline * 4);
                                        var current = MemoryHandler.Instance.GetInt32(currentAddress);
                                        lineLen = current - previous;
                                    }
                                    lengths.Add(lineLen);
                                    var spotAddress = chatPointers.OffsetArrayStart + (uint) ((getline - 1) * 4);
                                    _spots.Add(chatPointers.LogStart + (uint) MemoryHandler.Instance.GetInt32(spotAddress));
                                }
                                var limit = _spots.Count;
                                for (var i = 0; i < limit; i++)
                                {
                                    _spots[i] = (_spots[i] > _lastChatNum) ? _spots[i] : chatPointers.LogStart;
                                    var text = MemoryHandler.Instance.GetByteArray(_spots[i], lengths[i]);
                                    var chatEntry = new ChatEntry(text.ToArray());
                                    if (Regex.IsMatch(chatEntry.Combined, @"[\w\d]{4}::?.+"))
                                    {
                                        PostLineEvent(chatEntry);
                                    }
                                    _lastChatNum = _spots[i];
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
                        }
                        _lastCount = (int) chatPointers.LineCount;
                    }
                    else
                    {
                        try
                        {
                            ChatPointerMap = MemoryHandler.Instance.GetUInt32(MemoryHandler.Instance.SigScanner.Locations["GAMEMAIN"]) + 20;
                            MemoryHandler.Instance.SigScanner.Locations.Add("CHATLOG", ChatPointerMap);
                        }
                        catch (Exception ex)
                        {
                            Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
                        }
                    }
                }
                _isScanning = false;
                return true;
            };
            scannerWorker.BeginInvoke(delegate { }, scannerWorker);
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _scanTimer.Elapsed -= ScanTimerElapsed;
        }

        #endregion
    }
}
