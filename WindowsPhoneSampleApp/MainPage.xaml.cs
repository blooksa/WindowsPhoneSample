using Windows.UI.Xaml.Navigation;
using WindowsPhoneSample.Core;
using WindowsPhoneSample.Core.Pages;

namespace WindowsPhoneSampleApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : PhonePage
    {
        public MainPage()
            : base(
            new MainViewModel(
                SampleApplication.Current.Logger, 
                SampleApplication.Current.Context.WebServer, 
                SampleApplication.Current.Context.SessionService))
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
        }
    }
}
