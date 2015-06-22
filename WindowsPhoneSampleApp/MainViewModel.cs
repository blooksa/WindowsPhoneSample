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

        public MainViewModel(ILogger logger, IWebServer webServer, ISessionService sessionService)
            : base(logger, webServer)
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
                .ObserveOnDispatcher()
                .Subscribe(
                    _ =>
                    {
                        // TODO: do something
                    },
                    ex =>
                    {
                        // TODO: log something
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

        protected override void DoClearState(IDictionary<string, object> state)
        {
            state.Remove("PageContent");
        }
    }
}