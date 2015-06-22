using System;
using System.IO;
using System.Threading.Tasks;

namespace WindowsPhoneSample.Core.Web
{
    public interface IWebServer
    {
        bool IsNetworkAvailable { get; }
        IObservable<bool> NetworkStatus { get; }
        TimeSpan Timeout { get; set; }
        Task<Stream> DownloadAsync(Uri uri);
        Task<Stream> DownloadAsync(string url);
        Task<IHttpWebResponse> GetAsync(string url);
        Task<IHttpWebResponse> GetAsync(string url, TimeSpan timeout);
        Task<IHttpWebResponse> PutAsync(string url, string jsonBody);
        Task<IHttpWebResponse> PutAsync(string url, string jsonBody, TimeSpan timeout);
        Task<IHttpWebResponse> PostAsync(string url, string jsonBody);
        Task<IHttpWebResponse> PostAsync(string url, string jsonBody, TimeSpan timeout);
        Task<IHttpWebResponse> DeleteAsync(string url);
        Task<IHttpWebResponse> DeleteAsync(string url, TimeSpan timeout);
    }
}