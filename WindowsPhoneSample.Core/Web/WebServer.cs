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
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using System.Net.NetworkInformation;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WindowsPhoneSample.Core.Extensions;

namespace WindowsPhoneSample.Core.Web
{
    internal sealed class WebServer : IWebServer
    {
        private bool currentNetworkStatus;
        private readonly ReplaySubject<bool> networkStatus;
        private readonly ILogger logger;
 
        public WebServer(ILogger logger, TimeSpan timeout)
        {
            Contract.AssertNotNull(logger, "logger");
            this.logger = logger;
            Timeout = timeout;
            networkStatus = new ReplaySubject<bool>(1);
            currentNetworkStatus = IsNetworkAvailable;
            networkStatus.OnNext(currentNetworkStatus);
            NetworkInformation.NetworkStatusChanged += (s) =>
            {
                bool newValue = IsNetworkAvailable;
                if (currentNetworkStatus != newValue)
                {
                    currentNetworkStatus = newValue;
                    networkStatus.OnNext(currentNetworkStatus);
                }
            };
        }

        public TimeSpan Timeout { get; set; }

        public bool IsNetworkAvailable
        {
            get { return NetworkInterface.GetIsNetworkAvailable(); }
        }

        public IObservable<bool> NetworkStatus { get { return networkStatus.AsObservable(); } }

        private async Task<IHttpWebResponse> ExecuteRequest(HttpWebRequest request, TimeSpan timeout)
        {
            logger.Debug("HTTP {0} {1}", request.Method, request.RequestUri.ToString());
            var timeoutTask = Task.Delay(timeout);
            var task = await Task.WhenAny(Task.Factory.FromAsync<WebResponse>(((WebRequest)request).BeginGetResponse, ((WebRequest)request).EndGetResponse, null), timeoutTask);
            if (task == timeoutTask)
            {
                request.Abort();
                throw new TimeoutException("The http request timed out");
            }
            if (task.IsFaulted)
            {
                // Unwrap the AggregateException so we get to the WebException, which tells us what the error really is.
                WebException we;
                if (task.Exception != null && task.Exception.IsOfTypeOrWraps(out we))
                {
                    throw we;
                }
                if (task.Exception != null)
                {
                    throw task.Exception;
                }
            }
            return new HttpWebResponseWrapper((HttpWebResponse)((Task<WebResponse>)task).Result);
        }

        public async Task<Stream> DownloadAsync(Uri uri)
        {
            Contract.AssertNotNull(uri, "uri");
            return await DownloadAsync(uri.ToString(), Constants.DefaultTimeout);
        }

        public async Task<Stream> DownloadAsync(string url)
        {
            Contract.AssertNotNullOrWhitespace(url, "url");
            return await DownloadAsync(url, Constants.DefaultTimeout);
        }

        public async Task<Stream> DownloadAsync(string url, TimeSpan timeout)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = "GET";
            request.AllowReadStreamBuffering = true;
            IHttpWebResponse response = await ExecuteRequest(request, timeout);
            return response.GetResponseStream();
        }

        public async Task<IHttpWebResponse> GetAsync(string path)
        {
            return await GetAsync(path, Timeout);
        }

        public async Task<IHttpWebResponse> GetAsync(string path, TimeSpan timeout)
        {
            HttpWebRequest request = WebRequest.CreateHttp(path);
            request.Method = "GET";
            return await ExecuteRequest(request, timeout);
        }

        public async Task<IHttpWebResponse> PutAsync(string path, string jsonBody)
        {
            return await PutAsync(path, jsonBody, Timeout);
        }

        public async Task<IHttpWebResponse> PutAsync(string path, string jsonBody, TimeSpan timeout)
        {
            HttpWebRequest request = WebRequest.CreateHttp(path);
            request.Method = "PUT";
            request.ContentType = "application/json";
            if (!string.IsNullOrWhiteSpace(jsonBody))
            {
                byte[] data = Encoding.UTF8.GetBytes(jsonBody);
#if !WINDOWS_PHONE_APP
                request.ContentLength = data.Length;
#endif
                using (var requestStream = await Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, request))
                {
                    await requestStream.WriteAsync(data, 0, data.Length);
                }
            }
            return await ExecuteRequest(request, timeout);
        }

        public async Task<IHttpWebResponse> PostAsync(string path, string jsonBody)
        {
            return await PostAsync(path, jsonBody, Timeout);
        }

        public async Task<IHttpWebResponse> PostAsync(string path, string jsonBody, TimeSpan timeout)
        {
            HttpWebRequest request = WebRequest.CreateHttp(path);
            request.Method = "POST";
            request.ContentType = "application/json";
            if (!string.IsNullOrWhiteSpace(jsonBody))
            {
                byte[] data = Encoding.UTF8.GetBytes(jsonBody);
#if !WINDOWS_PHONE_APP
                request.ContentLength = data.Length;
#endif
                using (var requestStream = await Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, request))
                {
                    await requestStream.WriteAsync(data, 0, data.Length);
                }
            }
            return await ExecuteRequest(request, timeout);
        }

        public async Task<IHttpWebResponse> DeleteAsync(string path)
        {
            return await DeleteAsync(path, Timeout);
        }

        public async Task<IHttpWebResponse> DeleteAsync(string path, TimeSpan timeout)
        {
            HttpWebRequest request = WebRequest.CreateHttp(path);
            request.Method = "DELETE";
            return await ExecuteRequest(request, timeout);
        }
    }
}