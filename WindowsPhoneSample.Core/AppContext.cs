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
using System.Threading.Tasks;
using WindowsPhoneSample.Core.Services;
using WindowsPhoneSample.Core.Web;

namespace WindowsPhoneSample.Core
{
    public sealed class AppContext : IAppContext
    {
        private static readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(5);
        private readonly ILogger logger;

        public AppContext(ILogger logger)
            : this(
            logger,
            new WebServer(logger, defaultTimeout))
        {
        }

        internal AppContext(
            ILogger logger,
            IWebServer webServer)
            : this(
            logger,
            webServer,
            new SettingsService(logger, webServer, new StorageProvider()),
            new SessionService(logger, webServer))
        {
        }

        /// <summary>
        /// For unit testing purposes.
        /// </summary>
        internal AppContext(
            ILogger logger,
            IWebServer webServer,
            ISettingsService settingsService,
            ISessionService sessionService)
        {
            Contract.AssertNotNull(logger, "logger");
            Contract.AssertNotNull(webServer, "webServer");
            Contract.AssertNotNull(settingsService, "settingsService");
            Contract.AssertNotNull(sessionService, "sessionService");

            this.logger = logger;
            WebServer = webServer;
            SettingsService = settingsService;
            SessionService = sessionService;
        }

        public async Task ShutdownAsync()
        {
            IInternalSettingsService settings = SettingsService as IInternalSettingsService;
            if (settings != null)
            {
                await settings.SaveAsync();
                settings.Unload(true);
            }
        }

        public async Task StartupAsync()
        {
            await StartupAsync(string.Empty, Constants.DefaultTimeout);
        }

        public async Task StartupAsync(TimeSpan timeout)
        {
            await StartupAsync(string.Empty, Constants.DefaultTimeout);
        }

        public async Task StartupAsync(string endpoint)
        {
            await StartupAsync(string.Empty, Constants.DefaultTimeout);
        }

        public async Task StartupAsync(string endpoint, TimeSpan timeout)
        {
            WebServer.Timeout = timeout;
            IInternalSettingsService settings = SettingsService as IInternalSettingsService;
            if (settings != null)
            {
                await settings.LoadAsync();
            }
        }

        public IWebServer WebServer { get; private set; }

        public ISettingsService SettingsService { get; private set; }

        public ISessionService SessionService { get; private set; }
    }
}