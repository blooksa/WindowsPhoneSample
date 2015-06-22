using System;
using Windows.Foundation;
using Windows.UI.Core;

namespace WindowsPhoneSample.Core.Extensions
{
    public static class DispatcherExtensions
    {
        public static IAsyncAction BeginInvoke(this CoreDispatcher dispatcher, Action action)
        {
            return dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
        }
    }
}