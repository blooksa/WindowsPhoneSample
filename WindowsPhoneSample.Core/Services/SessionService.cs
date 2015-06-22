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