using System;
using System.Collections.Generic;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WindowsPhoneSample.Core.ViewModels;

namespace WindowsPhoneSample.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public class PhonePage : Page
    {
        private readonly NavigationHelper navigationHelper;

        public PhonePage(ViewModel viewModel)
        {
            DataContext = viewModel;
            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += NavigationHelperLoadState;
            navigationHelper.SaveState += NavigationHelperSaveState;
            Loaded += (s, e) => HardwareButtons.BackPressed += HandleBackKeyPress;
            Unloaded += (s, e) => HardwareButtons.BackPressed -= HandleBackKeyPress;
        }

        protected T Resolve<T>() where T : ViewModel
        {
            return (T)DataContext;
        }

        private void HandleBackKeyPress(object sender, BackPressedEventArgs e)
        {
            OnBackKeyPress(e);
        }

        protected virtual void OnBackKeyPress(BackPressedEventArgs e)
        {
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
            ViewModel viewModel = Resolve<ViewModel>();
            if (viewModel != null)
            {
                viewModel.Bind();
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel viewModel = Resolve<ViewModel>();
            if (viewModel != null)
            {
                viewModel.Unbind();
            }
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            navigationHelper.OnNavigatingFrom(e);
            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelperLoadState(object sender, LoadStateEventArgs e)
        {
            ViewModel viewModel = Resolve<ViewModel>();
            if (viewModel == null)
            {
                return;
            }
            IDictionary<string, string> queryString = e.NavigationParameter as IDictionary<string, string>;
            if (queryString == null)
            {
                queryString = new Dictionary<string, string>();
            }
            Dictionary<string, object> state = e.PageState;
            if (state == null)
            {
                state = new Dictionary<string, object>();
            }
            viewModel.RestoreState(state, queryString);
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelperSaveState(object sender, SaveStateEventArgs e)
        {
            ViewModel viewModel = Resolve<ViewModel>();
            if (viewModel == null)
            {
                return;
            }
            viewModel.SaveState(e.PageState);
        }
    }
}
