﻿using System;
using System.Collections.Generic;

namespace WindowsPhoneSample.Core.Services
{
    /// <summary>
    /// Manages settings per user, as well as global settings.
    /// </summary>
    public interface ISettingsService
    {
        void AddKnownType<T>();

        /// <summary>
        /// Removes a user setting.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <returns></returns>
        bool Remove(string key);

        /// <summary>
        /// Removes all user settings that match the criteria determined by the <param name="selector">selector</param>.
        /// </summary>
        /// <param name="selector">A criteria matching functor.</param>
        void RemoveAll(Func<string, object, bool> selector);

        /// <summary>
        /// Returns all user settings that match the criteria determined by the <param name="selector">selector</param>.
        /// </summary>
        /// <param name="selector">A criteria matching functor.</param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, object>> GetAll(Func<string, object, bool> selector);

        /// <summary>
        /// Returns a value for the specified user setting. Throws an exception if not found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key to use for lookup.</param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// Stores a setting with the given key. If one exists already, it will be overwritten.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key to use when storing.</param>
        /// <param name="value">The value to save.</param>
        void Set<T>(string key, T value);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key to use for lookup.</param>
        /// <param name="value">The value, if found, or the default value for the type T.</param>
        /// <returns></returns>
        bool TryGetValue<T>(string key, out T value);

        /// <summary>
        /// Checks to see if a user setting exists with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Contains(string key);

        /// <summary>
        /// Gets the value, if it exists, or the default value for type T.
        /// </summary>
        /// <typeparam name="T">The return value.</typeparam>
        /// <param name="key">The key to use for lookup.</param>
        /// <returns></returns>
        T GetValueOrDefault<T>(string key);

        /// <summary>
        /// Gets the value, if it exists, or the specified default value for type T.
        /// </summary>
        /// <typeparam name="T">The return value.</typeparam>
        /// <param name="key">The key to use for lookup.</param>
        /// <param name="defaultValue">The default value to use if no entry is found.</param>
        /// <returns></returns>
        T GetValueOrDefault<T>(string key, T defaultValue);

        /// <summary>
        /// Gets the value, if it exists, or the specified default value for type T, if the default value condition is met.
        /// </summary>
        /// <typeparam name="T">The return value.</typeparam>
        /// <param name="key">The key to use for lookup.</param>
        /// <param name="defaultValue">The default value to use if no entry is found.</param>
        /// <param name="useDefaultValueSelector">Determines whether the default value should be used or not, by inspecting the value found.</param>
        /// <returns></returns>
        T GetValueOrDefault<T>(string key, T defaultValue, Func<T, bool> useDefaultValueSelector);

        /// <summary>
        /// Gets the global value, if it exists, or the default value for type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key to use for lookup.</param>
        /// <returns></returns>
        T GetGlobalValueOrDefault<T>(string key);

        /// <summary>
        /// Adds a global (not user-specific) setting. Global settings are saved between sessions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key to use.</param>
        /// <param name="value">The value to store.</param>
        void SetGlobal<T>(string key, T value);

        /// <summary>
        /// Removes a global (not user-specific) setting.
        /// </summary>
        /// <param name="key">The key to use.</param>
        /// <returns></returns>
        bool RemoveGlobal(string key);

        /// <summary>
        /// Adds a temporary setting. A temporary setting is NOT saved between sessions, it only exists for the current run.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetTemp<T>(string key, T value);
    }
}