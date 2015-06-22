using System;
using System.Reactive;

namespace WindowsPhoneSample.Core.Services
{
    public interface ISessionService
    {
        IObservable<bool> IsLoggedOn { get; }
        IObservable<Unit> PerformLogOn(string username, string password);
        IObservable<Unit> PerformLogOff();
    }
}