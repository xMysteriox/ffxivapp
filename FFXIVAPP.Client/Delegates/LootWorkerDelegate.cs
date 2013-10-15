﻿// FFXIVAPP.Client
// LootWorkerDelegate.cs
// 
// © 2013 Ryan Wilson

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVAPP.Client.Helpers;
using FFXIVAPP.Client.Models;
using FFXIVAPP.Client.Properties;
using FFXIVAPP.Client.ViewModels;
using Newtonsoft.Json;
using SmartAssembly.Attributes;

#endregion

namespace FFXIVAPP.Client.Delegates
{
    [DoNotObfuscate]
    internal static class LootWorkerDelegate
    {
        #region Declarations

        public static readonly IList<LootEntry> LootEntries = new List<LootEntry>();

        private static readonly UploadHelper UploadHelper = new UploadHelper(5);

        #endregion

        /// <summary>
        /// </summary>
        public static void OnNewLoot(LootEntry lootEntry)
        {
            Func<bool> saveToDictionary = delegate
            {
                if (!lootEntry.IsValid())
                {
                    return false;
                }
                if (HttpPostHelper.IsValidJson(JsonConvert.SerializeObject(lootEntry)))
                {
                    LootEntries.Add(lootEntry);
                }
                XIVDBViewModel.Instance.LootSeen++;
                return true;
            };
            saveToDictionary.BeginInvoke(delegate
            {
                if (UploadHelper.Processing || !Settings.Default.AllowXIVDBIntegration || !Constants.IsOpen)
                {
                    return;
                }
                var chunkSize = UploadHelper.ChunkSize;
                var chunksProcessed = UploadHelper.ChunksProcessed;
                if (LootEntries.Count <= (chunkSize * (chunksProcessed + 1)))
                {
                    return;
                }
                try
                {
                    UploadHelper.Processing = true;
                    UploadHelper.PostUpload("loot", new List<LootEntry>(LootEntries.ToList()
                                                                                   .Skip(chunksProcessed * chunkSize)));
                    XIVDBViewModel.Instance.LootProcessed++;
                }
                catch (Exception ex)
                {
                    UploadHelper.Processing = false;
                }
            }, saveToDictionary);
        }

        /// <summary>
        /// </summary>
        public static void ProcessRemaining()
        {
            var chunkSize = UploadHelper.ChunkSize;
            var chunksProcessed = UploadHelper.ChunksProcessed;
            try
            {
                UploadHelper.PostUpload("loot", new List<LootEntry>(LootEntries.ToList()
                                                                               .Skip(chunksProcessed * chunkSize)));
            }
            catch (Exception ex)
            {
            }
        }
    }
}
