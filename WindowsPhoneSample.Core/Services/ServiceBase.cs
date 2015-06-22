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
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WindowsPhoneSample.Core.Extensions;
using WindowsPhoneSample.Core.Web;
using Newtonsoft.Json;

namespace WindowsPhoneSample.Core.Services
{
    internal class ServiceBase
    {
        protected ServiceBase(ILogger logger, IWebServer webServer)
        {
            Contract.AssertNotNull(logger, "logger");
            Contract.AssertNotNull(webServer, "webServer");
            Logger = logger;
            WebServer = webServer;
        }

        public ILogger Logger { get; private set; }

        public IWebServer WebServer { get; private set; }

        protected string UrlEncode(string value)
        {
            return Uri.EscapeDataString(value);
        }


        protected Task<T> PutResponseAsync<T>(string url, string body, Dictionary<string, string> headers = null) where T : class
        {
            return PutResponseAsync<T>(url, body, WebServer.Timeout, headers);
        }

        protected Task<T> PostResponseAsync<T>(string url, string body, Dictionary<string, string> headers = null) where T : class
        {
            return PostResponseAsync<T>(url, body, WebServer.Timeout, headers);
        }

        protected Task<T> GetResponseAsync<T>(string url, Dictionary<string, string> headers = null) where T : class
        {
            return GetResponseAsync<T>(url, WebServer.Timeout, headers);
        }

        protected Task<T> GetResponseAsync<T>(string url, TimeSpan timeout, Dictionary<string, string> headers = null) where T : class
        {
            return WebServer
                .GetAsync(url, timeout)
                .ContinueWith(x => ProcessResponse<T>(url, x, headers));
            ;
        }

        protected Task<T> PutResponseAsync<T>(string url, string body, TimeSpan timeout, Dictionary<string, string> headers = null) where T : class
        {
            return WebServer
                .PutAsync(url, body, timeout)
                .ContinueWith(x => ProcessResponse<T>(url, x, headers));
        }

        protected Task<T> PostResponseAsync<T>(string url, string body, TimeSpan timeout, Dictionary<string, string> headers = null) where T : class
        {
            return WebServer
                .PostAsync(url, body, timeout)
                .ContinueWith(x => ProcessResponse<T>(url, x, headers));
        }

        private T ProcessResponse<T>(string url, Task<IHttpWebResponse> x, Dictionary<string, string> headers = null) where T : class
        {
            IHttpWebResponse response = null;
            WebException we = null;
            TimeoutException te = null;

            try
            {
                response = x.Result;
            }
            catch (AggregateException e)
            {
                if (e.IsOfTypeOrWraps(out we) && we.Response is HttpWebResponse)
                {
                    HttpWebResponse httpResponse = we.Response as HttpWebResponse;
                    response = new HttpWebResponseWrapper(httpResponse);
                }
                else
                {
                    // See if it is a timeout
                    e.IsOfTypeOrWraps(out te);
                }
            }
            if (response != null && response.StatusCode.ResponseWasOk())
            {
                if (headers != null)
                {
                    foreach (var header in response.Headers)
                    {
                        headers.Add(header.Key, header.Value);
                    }
                }
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string jsonStr = reader.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(jsonStr) && jsonStr != "null")
                    {
                        if (typeof(T) == typeof(string))
                        {
                            return jsonStr as T;
                        }

                        return JsonConvert.DeserializeObject<T>(jsonStr);
                    }
                    return default(T);
                }
            }
            if (response != null) // not ok
            {
                Logger.Error("Http request '{0}' failed with status code {1}", url, response.StatusCode);
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string jsonStr = reader.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(jsonStr) && jsonStr != "null")
                    {
                        Logger.Error("Failed to parse Json '{0}'", jsonStr);
                    }
                }
            }
            if (we != null)
            {
                throw we;
            }
            if (te != null)
            {
                throw te;
            }
            if (response != null)
            {
                throw new WebException("HTTP Error: " + response.StatusCode);
            }
            throw new WebException("HTTP error");
        }
    }
}