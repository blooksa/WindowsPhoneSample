using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WindowsPhoneSample.Core.Web;

namespace WindowsPhoneSample.Core.Services
{
    internal sealed class SessionService : ServiceBase, ISessionService
    {
        private readonly ReplaySubject<bool> loggedOnSubject;
        private bool isLoggedOn;

        public SessionService(ILogger logger, IWebServer webServer)
            : base(logger, webServer)
        {
            loggedOnSubject = new ReplaySubject<bool>(1);
            isLoggedOn = false;
            // load stream with an initial value
            PushLoggedOnState(isLoggedOn);
        }

        public IObservable<bool> IsLoggedOn { get { return loggedOnSubject; } }

        public IObservable<Unit> PerformLogOn(string username, string password)
        {
            if (!isLoggedOn)
            {
                // todo: replace with real call to server/database/other
                PushLoggedOnState(true);
                return Observable.Return(new Unit());
            }
            return Observable.Throw<Unit>(new InvalidOperationException("User is already logged on"));
        }

        public IObservable<Unit> PerformLogOff()
        {
            if (isLoggedOn)
            {
                // todo: replace with real call to server/database/other
                PushLoggedOnState(false);
                return Observable.Return(new Unit());
            }
            return Observable.Throw<Unit>(new InvalidOperationException("User is already logged off"));
        }

        private void PushLoggedOnState(bool state)
        {
            isLoggedOn = state;
            loggedOnSubject.OnNext(state);
        }
    }
}