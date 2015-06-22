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