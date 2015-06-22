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
using System.Net;

namespace WindowsPhoneSample.Core.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Determines if the exception itself or any of its inner exceptions are of the specified type.
        /// </summary>
        public static bool IsOfTypeOrWraps<T>(this Exception e, out T te) where T : Exception
        {
            T tException = e as T;
            if (tException != null)
            {
                te = tException;
                return true;
            }

            Exception innerException = e.InnerException;
            while (innerException != null)
            {
                T innerTException = innerException as T;
                if (innerTException != null)
                {
                    te = innerTException;
                    return true;
                }
                innerException = innerException.InnerException;
            }

            te = null;
            return false;
        }

        public static HttpStatusCode GetStatus(this WebException we)
        {
            if (we != null)
            {
                HttpWebResponse response = we.Response as HttpWebResponse;
                if (response != null)
                {
                    return response.StatusCode;
                }
            }

            return HttpStatusCode.NotFound;
        }

    }
}