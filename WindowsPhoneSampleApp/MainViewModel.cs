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
using System.Reactive.Linq;
using Windows.UI.Xaml;
using WindowsPhoneSample.Core;
using WindowsPhoneSample.Core.Services;
using WindowsPhoneSample.Core.ViewModels;
using WindowsPhoneSample.Core.Web;

namespace WindowsPhoneSampleApp
{
    public class MainViewModel : ViewModel
    {
        private readonly ISessionService sessionService;

        public MainViewModel(ILogger logger, IWebServer webServer, ISchedulerService schedulerService, ISessionService sessionService)
            : base(logger, webServer, schedulerService)
        {
            this.sessionService = sessionService;
        }

        public static readonly DependencyProperty PageContentProperty = DependencyProperty.Register(
            "PageContent", typeof(string), typeof(MainViewModel), new PropertyMetadata(default(string)));

        public string PageContent
        {
            get { return (string)GetValue(PageContentProperty); }
            set { SetValue(PageContentProperty, value); }
        }

        protected override IEnumerable<IDisposable> DoBind()
        {
            yield return sessionService.IsLoggedOn
                .Where(x => x)
                .Take(1)
                .ObserveOn(Scheduler.Dispatcher)
                .Subscribe(
                    _ =>
                    {
                        // TODO: do something
                    },
                    ex =>
                    {
                        Logger.Exception(ex, "Failed to Bind()");
                    });
        }

        protected override void DoRestoreState(IDictionary<string, object> state, IDictionary<string, string> queryString)
        {
            PageContent = GetStringFromStateOrQueryString(state, queryString, "PageContent");
        }

        protected override void DoSaveState(IDictionary<string, object> state)
        {
            state["PageContent"] = PageContent;
        }
    }
}