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
using System.Diagnostics;

namespace WindowsPhoneSample.Core
{
    public sealed class Logger : ILogger
    {
        private enum LogType
        {
            Trace,
            Debug,
            Message,
            Warning,
            Error
        }

        private readonly object lockObject;

        public Logger()
        {
            lockObject = new object();
        }

        private static string Prefix(LogType type)
        {
            string timeStamp = DateTime.Now.ToString(Constants.HmsfFormat);
            switch (type)
            {
                case LogType.Trace:
                    return timeStamp + " -T- ";
                case LogType.Debug:
                    return timeStamp + " -D- ";
                case LogType.Message:
                    return timeStamp + " -M- ";
                case LogType.Warning:
                    return timeStamp + " -W- ";
            }
            return timeStamp + " -E- ";
        }

        [Conditional("DEBUG")]
        private void AppendLog(LogType type, string format, params object[] args)
        {
#if DEBUG
            string entry;
            if (args == null || args.Length == 0)
                entry = Prefix(type) + format;
            else
                entry = Prefix(type) + string.Format(format, args);

            lock (lockObject)
            {
                System.Diagnostics.Debug.WriteLine(entry);
            }
#endif
        }

        public void Trace(string format, params object[] args)
        {
            AppendLog(LogType.Trace, format, args);
        }

        public void Debug(string format, params object[] args)
        {
            AppendLog(LogType.Debug, format, args);
        }

        public void Message(string format, params object[] args)
        {
            AppendLog(LogType.Message, format, args);
        }

        public void Warning(string format, params object[] args)
        {
            AppendLog(LogType.Warning, format, args);
        }

        public void Error(string format, params object[] args)
        {
            AppendLog(LogType.Error, format, args);
        }

        public void Exception(Exception e, string format, params object[] args)
        {
            string message;
            if (args == null || args.Length == 0)
                message = format;
            else
                message = string.Format(format, args);

            if (e != null)
            {
                AppendLog(LogType.Error, e.GetType().Name + " " + message);
            }
            else
            {
                AppendLog(LogType.Error, message);
            }

            while (e != null)
            {
                AppendLog(LogType.Error, "Message: {0}", e.Message);
                AppendLog(LogType.Error, "Source: {0}", e.Source);
                AppendLog(LogType.Error, "StackTrace: {0}", e.StackTrace);

                System.Diagnostics.Debug.WriteLine(" -E- Message: " + e.Message);
                System.Diagnostics.Debug.WriteLine(" -E- Source: " + e.Source);
                System.Diagnostics.Debug.WriteLine(" -E- StackTrace: " + e.StackTrace);
                e = e.InnerException;
            }
        }
    }
}