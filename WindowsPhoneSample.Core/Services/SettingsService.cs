// The MIT License (MIT)
// 
// Copyright (c) 2015 Capsor Consulting AB
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using WindowsPhoneSample.Core.Web;

namespace WindowsPhoneSample.Core.Services
{
    internal class SettingsService : ServiceBase, IInternalSettingsService
    {
        private const string settingsFileName = "Settings.xml";

        private static readonly List<Type> knownTypes;

        private readonly Dictionary<string, object> settings;
        private readonly List<string> tmpSettings;
        private string settingsFileLoaded;
        private volatile bool dirtyMonitorIsRunning;
        private readonly CancellationTokenSource autoSaveTokenSource;
        private readonly IStorageProvider storageProvider;

        static SettingsService()
        {
            knownTypes = new List<Type>
            {
                typeof(Windows.Foundation.Size),
                typeof(Guid),
                typeof(Uri)
            };
        }

        internal SettingsService(ILogger logger, IWebServer webServer, IStorageProvider storageProvider)
            : base(logger, webServer)
        {
            this.storageProvider = storageProvider;
            settings = new Dictionary<string, object>();
            tmpSettings = new List<string>();
            autoSaveTokenSource = new CancellationTokenSource();
        }

        public async Task LoadAsync()
        {
            if (!IsLoaded)
            {
                await LoadSettingsAsync(settingsFileName);
                tmpSettings.Clear();
                IsLoaded = true;
            }
        }

        public async Task SaveAsync()
        {
            await SaveSettingsAsync(settingsFileName);
            SaveGlobal();
        }

        private void SaveGlobal()
        {
#if !WINDOWS_PHONE_APP
            IsolatedStorageSettings.ApplicationSettings.Save();
#endif
        }

        public bool IsLoaded { get; private set; }

        public void Unload(bool isClosing)
        {
            if (IsLoaded)
            {
                autoSaveTokenSource.Cancel();

                foreach (string key in tmpSettings)
                {
                    settings.Remove(key);
                }
                tmpSettings.Clear();
                if (isClosing)
                {
                    settings.Clear();
                    settingsFileLoaded = null;
                    IsLoaded = false;
                }
            }
        }

        private async Task LoadSettingsAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(settingsFileLoaded) || settingsFileLoaded != fileName)
            {
                    if (await storageProvider.FileExistsAsync(fileName))
                    {
                        using (Stream fs = await storageProvider.OpenFileAsync(fileName, StorageCreateMode.Open, StorageMode.Read, StorageShareMode.Read))
                        {
                            try
                            {
                                DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>), knownTypes);
                                var loadedSettings = (Dictionary<string, object>)serializer.ReadObject(fs);
                                foreach (var setting in loadedSettings)
                                {
                                    settings[setting.Key] = setting.Value;
                                }
                                settingsFileLoaded = fileName;
                            }
                            catch (Exception e)
                            {
                                Logger.Exception(e, string.Empty, "SettingsService.LoadSettingsAsync() Failed");
                            }
                        }
                    }
                }
            }

        private async Task SaveSettingsAsync(string fileName)
        {
            using (Stream fs = await storageProvider.CreateFileAsync(fileName))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>), knownTypes);
                try
                {
                    Dictionary<string, object> settingsToSave = new Dictionary<string, object>(settings);
                    foreach (string key in tmpSettings)
                    {
                        settingsToSave.Remove(key);
                    }
                    serializer.WriteObject(fs, settingsToSave);
                }
                catch (Exception e)
                {
                    Logger.Exception(e, string.Empty, "SettingsService.SaveSettingsAsync() Failed");
                }
            }
        }

        public void AddKnownType<T>()
        {
            if (!knownTypes.Contains(typeof(T)))
            {
                knownTypes.Add(typeof(T));
            }
        }

        public bool Remove(string key)
        {
            return settings.Remove(key);
        }

        public bool RemoveGlobal(string key)
        {
#if WINDOWS_PHONE_APP
            return Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(key);
#else
            return IsolatedStorageSettings.ApplicationSettings.Remove(key);
#endif
        }

        public void RemoveAll(Func<string, object, bool> selector)
        {
            if (selector != null)
            {
                var settingsToRemove = settings.Where(x => selector(x.Key, x.Value)).Select(x => x.Key).ToList();
                foreach (string key in settingsToRemove)
                {
                    settings.Remove(key);
                }
            }
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll(Func<string, object, bool> selector)
        {
            if (selector != null)
            {
                return settings.Where(x => selector(x.Key, x.Value));
            }

            return Enumerable.Empty<KeyValuePair<string, object>>();
        }

        public T Get<T>(string key)
        {
            object obj;
            if (settings.TryGetValue(key, out obj))
            {
                return (T)obj;
            }
            return default(T);
        }

        public void Set<T>(string key, T value)
        {
            settings[key] = value;
            if (!dirtyMonitorIsRunning)
            {
                dirtyMonitorIsRunning = true;
                Task.Delay(TimeSpan.FromSeconds(5))
#if WINDOWS_PHONE_APP
                    .ContinueWith(async _ =>
                    {
                        await SaveAsync();
                        dirtyMonitorIsRunning = false;
                    }, autoSaveTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
#else
                    .ContinueWith(_ =>
                    {
                        Save();
                        dirtyMonitorIsRunning = false;
                    }, autoSaveTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
#endif
            }
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            object obj;
            if (settings.TryGetValue(key, out obj))
            {
                value = (T)obj;
                return true;
            }
            value = default(T);
            return false;
        }

        public T GetValueOrDefault<T>(string key)
        {
            object obj;
            if (settings.TryGetValue(key, out obj))
            {
                return (T)obj;
            }
            return default(T);
        }

        public T GetValueOrDefault<T>(string key, T defaultValue)
        {
            object obj;
            if (settings.TryGetValue(key, out obj))
            {
                return (T)obj;
            }
            return defaultValue;
        }

        public T GetValueOrDefault<T>(string key, T defaultValue, Func<T, bool> useDefaultValueSelector)
        {
            object obj;
            if (settings.TryGetValue(key, out obj))
            {
                if (useDefaultValueSelector((T)obj))
                {
                    return defaultValue;
                }
                return (T)obj;
            }
            return defaultValue;
        }

        public bool Contains(string key)
        {
            return !string.IsNullOrEmpty(key) && settings.ContainsKey(key);
        }

        public void SetGlobal<T>(string key, T value)
        {
#if WINDOWS_PHONE_APP
            Windows.Storage.ApplicationData.Current.LocalSettings.Values[key] = value;
#else
            IsolatedStorageSettings.ApplicationSettings[key] = value;
#endif
        }

        public T GetGlobalValueOrDefault<T>(string key)
        {
            object obj;
#if WINDOWS_PHONE_APP
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out obj))
            {
                return (T)obj;
            }
#else
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out obj))
            {
                return (T)obj;
            }
#endif
            return default(T);
        }

        /// <summary>
        /// Adds a temporary setting. A temporary setting is not saved between sessions, it only exists for the current run.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetTemp<T>(string key, T value)
        {
            Set(key, value);
            if (!tmpSettings.Contains(key))
            {
                tmpSettings.Add(key);
            }
        }
    }
}