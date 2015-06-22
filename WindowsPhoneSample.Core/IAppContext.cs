using System;
using System.Threading.Tasks;
using WindowsPhoneSample.Core.Services;
using WindowsPhoneSample.Core.Web;

namespace WindowsPhoneSample.Core
{
    public interface IAppContext
    {
        IWebServer WebServer { get; }
        ISettingsService SettingsService { get; } 
        ISessionService SessionService { get; } 

        Task StartupAsync();
        Task StartupAsync(TimeSpan timeout);
        Task ShutdownAsync();
    }
}