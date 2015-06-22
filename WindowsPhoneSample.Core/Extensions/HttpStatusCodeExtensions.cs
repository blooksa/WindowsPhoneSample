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
using System.Net;

namespace WindowsPhoneSample.Core.Extensions
{
    public static class HttpStatusCodeExtensions
    {
        public static bool ResponseWasOk(this HttpStatusCode code)
        {
            int status = (int)code;
            return status >= 200 && status < 300;
        }

        public static bool ResponseWasRedirected(this HttpStatusCode code)
        {
            int status = (int)code;
            return status >= 300 && status < 400;
        }

        public static bool ResponseWasRejected(this HttpStatusCode code)
        {
            int status = (int)code;
            return status >= 400 && status < 500;
        }

        public static bool RequestFailedDueToServerError(this HttpStatusCode code)
        {
            int status = (int)code;
            return status >= 500 && status < 600;
        }
    }
}