using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using WindowsPhoneSample.Core.Extensions;
using WindowsPhoneSample.Core.Web;

namespace WindowsPhoneSample.Core.ViewModels
{
    public class ViewModel : DependencyObject
    {
        public event EventHandler DataLoaded;

        private readonly List<IDisposable> disposables;
        private readonly List<ViewModel> childViewModels;

        protected ViewModel(ILogger logger, IWebServer webServer)
        {
            Contract.AssertNotNull(logger, "logger");
            Contract.AssertNotNull(webServer, "webServer");
            Logger = logger;
            WebServer = webServer;
            disposables = new List<IDisposable>();
            childViewModels = new List<ViewModel>();
        }

        protected void AddChildViewModel(ViewModel childViewModel)
        {
            if (childViewModel != null && !childViewModels.Contains(childViewModel))
            {
                childViewModels.Add(childViewModel);
            }
        }

        protected void RemoveChildViewModel(ViewModel childViewModel)
        {
            if (childViewModel != null && childViewModels.Contains(childViewModel))
            {
                childViewModels.Add(childViewModel);
            }
        }

        public bool IsNetworkAvailable { get { return WebServer.IsNetworkAvailable; } }
        protected ILogger Logger { get; private set; }
        protected IWebServer WebServer { get; private set; }

        public bool IsBound { get; private set; }

        private bool IsBinding { get; set; }

        protected void OnDataLoaded(EventArgs e)
        {
            var eventHandler = DataLoaded;
            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        public void Bind()
        {
            if (!IsBound && !IsBinding)
            {
                IsBinding = true;
                try
                {
                    var itemsToDispose = DoBind();
                    if (childViewModels.Count > 0)
                    {
                        foreach (ViewModel childViewModel in childViewModels)
                        {
                            disposables.AddRange(childViewModel.DoBind());
                        }
                    }
                    if (itemsToDispose != null)
                    {
                        var list = itemsToDispose.ToList();
                        if (list.Count > 0)
                        {
                            disposables.AddRange(list);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Exception(e, GetType().Name + ".Bind() Failed");
                }
                IsBinding = false;
                IsBound = true;
            }
        }

        public void Unbind()
        {
            if (IsBound)
            {
                if (childViewModels.Count > 0)
                {
                    childViewModels.ForEach(x => x.DoUnbind());
                }
                DoUnbind();
                string classAndMethod = GetType().Name + ".Unbind()";
                Task.Factory.StartNew(() =>
                {
                    disposables.ForEach(x =>
                    {
                        if (x != null)
                            try
                            {
                                x.Dispose();
                            }
                            catch (Exception e)
                            {
                                Logger.Exception(e, classAndMethod + " Dispose observable failed");
                            }
                    });
                    disposables.Clear();
                });
                IsBound = false;
            }
        }

        public void SaveState(IDictionary<string, object> state)
        {
            childViewModels.ForEach(x => x.SaveState(state));
            DoSaveState(state);
        }

        public void RestoreState(IDictionary<string, object> state, IDictionary<string, string> queryString)
        {
            childViewModels.ForEach(x => x.RestoreState(state, queryString));
            DoRestoreState(state, queryString);
        }

        public void ClearState(IDictionary<string, object> state)
        {
            childViewModels.ForEach(x => x.ClearState(state));
            DoClearState(state);
        }

        protected virtual void DoSaveState(IDictionary<string, object> state)
        {
        }

        protected virtual void DoRestoreState(IDictionary<string, object> state, IDictionary<string, string> queryString)
        {
        }

        protected virtual void DoClearState(IDictionary<string, object> state)
        {
        }

        protected virtual IEnumerable<IDisposable> DoBind()
        {
            return Enumerable.Empty<IDisposable>();
        }

        protected virtual void DoUnbind()
        {
        }

        protected string GetStringFromStateOrQueryString(IDictionary<string, object> state, IDictionary<string, string> queryString, string variableName, bool unEscape = true)
        {
            string s = null;

            if (state.Count > 0)
            {
                object obj;
                if (state.TryGetValue(variableName, out obj) && obj is string)
                {
                    s = (string)obj;
                }
            }

            string str;
            if (queryString.TryGetValue(variableName, out str) && !string.IsNullOrWhiteSpace(str))
            {
                if (unEscape)
                {
                    s = Uri.UnescapeDataString(str);
                }
                else
                {
                    s = str;
                }
            }

            return s;
        }

        protected int GetIntFromStateOrQueryString(IDictionary<string, object> state, IDictionary<string, string> queryString, string variableName)
        {
            int i = 0;

            if (state.Count > 0)
            {
                object obj;
                if (state.TryGetValue(variableName, out obj) && obj is int)
                {
                    i = (int)obj;
                }
            }

            string str;
            int tmp;
            if (queryString.TryGetValue(variableName, out str) && !string.IsNullOrWhiteSpace(str) && int.TryParse(str, out tmp))
            {
                i = tmp;
            }

            return i;
        }

        protected bool GetBoolFromPageStateOrQueryString(IDictionary<string, object> state, IDictionary<string, string> queryString, string variableName)
        {
            bool b = false;

            if (state.Count > 0)
            {
                object obj;
                if (state.TryGetValue(variableName, out obj) && obj is bool)
                {
                    b = (bool)obj;
                }
            }

            string str;
            bool tmp;
            if (queryString.TryGetValue(variableName, out str) && !string.IsNullOrWhiteSpace(str) && bool.TryParse(str, out tmp))
            {
                b = tmp;
            }

            return b;
        }
    }
}