// Decompiled with JetBrains decompiler
// Type: Game.WebManager
// Assembly: RuthlessConquest, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 09ABF203-5B7E-4C78-ACFB-2EE5FE9ADF6E
// Assembly location: d:\Users\12464\Desktop\Ruthless Conquest\RuthlessConquest.exe

using Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public static class WebManager
    {
        public static bool IsInternetConnectionAvailable() => true;

        public static void Get(
          string address,
          Dictionary<string, string> parameters,
          Dictionary<string, string> headers,
          CancellableProgress progress,
          Action<byte[]> success,
          Action<Exception> failure)
        {
            Task.Run(async () =>
            {
                try
                {
                    progress = progress ?? new CancellableProgress();
                    if (!IsInternetConnectionAvailable())
                        throw new InvalidOperationException("Internet connection is unavailable.");
                    using (HttpClient client = new HttpClient())
                    {
                        Uri requestUri = parameters == null || parameters.Count <= 0 ? new Uri(address) : new Uri(string.Format("{0}?{1}", address, UrlParametersToString(parameters)));
                        client.DefaultRequestHeaders.Referrer = new Uri(address);
                        if (headers != null)
                        {
                            foreach (KeyValuePair<string, string> header in headers)
                                client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                        HttpResponseMessage responseMessage = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, progress.CancellationToken);
                        await VerifyResponse(responseMessage);
                        long? contentLength = responseMessage.Content.Headers.ContentLength;
                        progress.Total = contentLength.GetValueOrDefault();
                        using (Stream responseStream = await responseMessage.Content.ReadAsStreamAsync())
                        {
                            using (MemoryStream targetStream = new MemoryStream())
                            {
                                long written = 0;
                                byte[] buffer = new byte[1024];
                                int count;
                                do
                                {
                                    count = await responseStream.ReadAsync(buffer, 0, buffer.Length, progress.CancellationToken);
                                    if (count > 0)
                                    {
                                        targetStream.Write(buffer, 0, count);
                                        written += count;
                                        progress.Completed = written;
                                    }
                                }
                                while (count > 0);
                                Dispatcher.Dispatch(() => success(targetStream.ToArray()));
                                buffer = null;
                            }
                        }
                        responseMessage = null;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ExceptionManager.MakeFullErrorMessage(ex));
                    Dispatcher.Dispatch(() => failure(ex));
                }
            });
        }

        public static void Put(
          string address,
          Dictionary<string, string> parameters,
          Dictionary<string, string> headers,
          Stream data,
          CancellableProgress progress,
          Action<byte[]> success,
          Action<Exception> failure)
        {
            PutOrPost(false, address, parameters, headers, data, progress, success, failure);
        }

        public static void Post(
          string address,
          Dictionary<string, string> parameters,
          Dictionary<string, string> headers,
          Stream data,
          CancellableProgress progress,
          Action<byte[]> success,
          Action<Exception> failure)
        {
            PutOrPost(true, address, parameters, headers, data, progress, success, failure);
        }

        public static string UrlParametersToString(Dictionary<string, string> values)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string str = string.Empty;
            foreach (KeyValuePair<string, string> keyValuePair in values)
            {
                stringBuilder.Append(str);
                str = "&";
                stringBuilder.Append(Uri.EscapeDataString(keyValuePair.Key));
                stringBuilder.Append('=');
                if (!string.IsNullOrEmpty(keyValuePair.Value))
                    stringBuilder.Append(Uri.EscapeDataString(keyValuePair.Value));
            }
            return stringBuilder.ToString();
        }

        public static byte[] UrlParametersToBytes(Dictionary<string, string> values) => Encoding.UTF8.GetBytes(UrlParametersToString(values));

        public static MemoryStream UrlParametersToStream(Dictionary<string, string> values) => new MemoryStream(Encoding.UTF8.GetBytes(UrlParametersToString(values)));

        public static Dictionary<string, string> UrlParametersFromString(string s)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string str = s;
            char[] separator = new char[1] { '&' };
            foreach (string stringToUnescape in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] strArray = Uri.UnescapeDataString(stringToUnescape).Split('=');
                if (strArray.Length == 2)
                    dictionary[strArray[0]] = strArray[1];
            }
            return dictionary;
        }

        public static Dictionary<string, string> UrlParametersFromBytes(byte[] bytes) => UrlParametersFromString(Encoding.UTF8.GetString(bytes, 0, bytes.Length));

        public static object JsonFromString(string s) => SimpleJson.SimpleJson.DeserializeObject(s);

        public static object JsonFromBytes(byte[] bytes) => JsonFromString(Encoding.UTF8.GetString(bytes, 0, bytes.Length));

        private static void PutOrPost(
          bool isPost,
          string address,
          Dictionary<string, string> parameters,
          Dictionary<string, string> headers,
          Stream data,
          CancellableProgress progress,
          Action<byte[]> success,
          Action<Exception> failure)
        {
            Task.Run(async () =>
            {
                try
                {
                    if (!IsInternetConnectionAvailable())
                        throw new InvalidOperationException("Internet connection is unavailable.");
                    using (HttpClient client = new HttpClient())
                    {
                        Dictionary<string, string> dictionary = new Dictionary<string, string>();
                        if (headers != null)
                        {
                            foreach (KeyValuePair<string, string> header in headers)
                            {
                                if (!client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value))
                                    dictionary.Add(header.Key, header.Value);
                            }
                        }
                        Uri requestUri = parameters == null || parameters.Count <= 0 ? new Uri(address) : new Uri(string.Format("{0}?{1}", address, UrlParametersToString(parameters)));
                        WebManager.ProgressHttpContent progressHttpContent = new WebManager.ProgressHttpContent(data, progress);
                        foreach (KeyValuePair<string, string> keyValuePair in dictionary)
                            progressHttpContent.Headers.Add(keyValuePair.Key, keyValuePair.Value);
                        HttpResponseMessage responseMessage;
                        if (isPost)
                            responseMessage = await client.PostAsync(requestUri, progressHttpContent, progress.CancellationToken);
                        else
                            responseMessage = await client.PutAsync(requestUri, progressHttpContent, progress.CancellationToken);
                        await VerifyResponse(responseMessage);
                        byte[] responseData = await responseMessage.Content.ReadAsByteArrayAsync();
                        Dispatcher.Dispatch(() => success(responseData));
                        responseMessage = null;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ExceptionManager.MakeFullErrorMessage(ex));
                    Dispatcher.Dispatch(() => failure(ex));
                }
            });
        }

        private static async Task VerifyResponse(HttpResponseMessage message)
        {
            if (!message.IsSuccessStatusCode)
            {
                string responseText = string.Empty;
                try
                {
                    responseText = await message.Content.ReadAsStringAsync();
                }
                catch
                {
                }
                throw new InvalidOperationException(string.Format("{0} ({1})\n{2}", message.StatusCode.ToString(), (int)message.StatusCode, responseText));
            }
        }

        private class ProgressHttpContent : HttpContent
        {
            private Stream m_sourceStream;
            private CancellableProgress m_progress;

            public ProgressHttpContent(Stream sourceStream, CancellableProgress progress)
            {
                this.m_sourceStream = sourceStream;
                this.m_progress = progress ?? new CancellableProgress();
            }

            protected override bool TryComputeLength(out long length)
            {
                length = this.m_sourceStream.Length;
                return true;
            }

            protected override async Task SerializeToStreamAsync(
              Stream targetStream,
              TransportContext context)
            {
                byte[] buffer = new byte[1024];
                int read = 0;
                long written = 0;
                do
                {
                    this.m_progress.Total = m_sourceStream.Length;
                    this.m_progress.Completed = written;
                    if (this.m_progress.CancellationToken.IsCancellationRequested)
                        throw new OperationCanceledException("Operation cancelled.");
                    read = this.m_sourceStream.Read(buffer, 0, buffer.Length);
                    if (read > 0)
                    {
                        await targetStream.WriteAsync(buffer, 0, read, this.m_progress.CancellationToken);
                        written += read;
                    }
                }
                while (read > 0);
            }
        }
    }
}



/*using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SimpleJson;

namespace Game
{
    public static class WebManager
    {
        public static bool IsInternetConnectionAvailable()
        {
            return true;
        }

        public static void Get(string address, Dictionary<string, string> parameters, Dictionary<string, string> headers, CancellableProgress progress, Action<byte[]> success, Action<Exception> failure)
        {
            WebManager.<> c__DisplayClass2_0 CS$<> 8__locals1 = new WebManager.<> c__DisplayClass2_0();
            CS$<> 8__locals1.progress = progress;
            CS$<> 8__locals1.parameters = parameters;
            CS$<> 8__locals1.address = address;
            CS$<> 8__locals1.headers = headers;
            CS$<> 8__locals1.success = success;
            CS$<> 8__locals1.failure = failure;
            Task.Run(delegate ()
            {
                WebManager.<> c__DisplayClass2_0.<< Get > b__0 > d << Get > b__0 > d;

                << Get > b__0 > d.<> 4__this = CS$<> 8__locals1;

                << Get > b__0 > d.<> t__builder = AsyncTaskMethodBuilder.Create();

                << Get > b__0 > d.<> 1__state = -1;
                AsyncTaskMethodBuilder<> t__builder = << Get > b__0 > d.<> t__builder;

                <> t__builder.Start < WebManager.<> c__DisplayClass2_0.<< Get > b__0 > d > (ref << Get > b__0 > d);
                return << Get > b__0 > d.<> t__builder.Task;
            });
        }

        public static void Put(string address, Dictionary<string, string> parameters, Dictionary<string, string> headers, Stream data, CancellableProgress progress, Action<byte[]> success, Action<Exception> failure)
        {
            WebManager.PutOrPost(false, address, parameters, headers, data, progress, success, failure);
        }

        public static void Post(string address, Dictionary<string, string> parameters, Dictionary<string, string> headers, Stream data, CancellableProgress progress, Action<byte[]> success, Action<Exception> failure)
        {
            WebManager.PutOrPost(true, address, parameters, headers, data, progress, success, failure);
        }

        public static string UrlParametersToString(Dictionary<string, string> values)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string value = string.Empty;
            foreach (KeyValuePair<string, string> keyValuePair in values)
            {
                stringBuilder.Append(value);
                value = "&";
                stringBuilder.Append(Uri.EscapeDataString(keyValuePair.Key));
                stringBuilder.Append('=');
                if (!string.IsNullOrEmpty(keyValuePair.Value))
                {
                    stringBuilder.Append(Uri.EscapeDataString(keyValuePair.Value));
                }
            }
            return stringBuilder.ToString();
        }

        public static byte[] UrlParametersToBytes(Dictionary<string, string> values)
        {
            return Encoding.UTF8.GetBytes(WebManager.UrlParametersToString(values));
        }

        public static MemoryStream UrlParametersToStream(Dictionary<string, string> values)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(WebManager.UrlParametersToString(values)));
        }

        public static Dictionary<string, string> UrlParametersFromString(string s)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string[] array = s.Split(new char[]
            {
                '&'
            }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < array.Length; i++)
            {
                string[] array2 = Uri.UnescapeDataString(array[i]).Split(new char[]
                {
                    '='
                });
                if (array2.Length == 2)
                {
                    dictionary[array2[0]] = array2[1];
                }
            }
            return dictionary;
        }

        public static Dictionary<string, string> UrlParametersFromBytes(byte[] bytes)
        {
            return WebManager.UrlParametersFromString(Encoding.UTF8.GetString(bytes, 0, bytes.Length));
        }

        public static object JsonFromString(string s)
        {
            return SimpleJson.DeserializeObject(s);
        }

        public static object JsonFromBytes(byte[] bytes)
        {
            return WebManager.JsonFromString(Encoding.UTF8.GetString(bytes, 0, bytes.Length));
        }

        private static void PutOrPost(bool isPost, string address, Dictionary<string, string> parameters, Dictionary<string, string> headers, Stream data, CancellableProgress progress, Action<byte[]> success, Action<Exception> failure)
        {
            WebManager.<> c__DisplayClass12_0 CS$<> 8__locals1 = new WebManager.<> c__DisplayClass12_0();
            CS$<> 8__locals1.headers = headers;
            CS$<> 8__locals1.parameters = parameters;
            CS$<> 8__locals1.address = address;
            CS$<> 8__locals1.data = data;
            CS$<> 8__locals1.progress = progress;
            CS$<> 8__locals1.isPost = isPost;
            CS$<> 8__locals1.success = success;
            CS$<> 8__locals1.failure = failure;
            Task.Run(delegate ()
            {
                WebManager.<> c__DisplayClass12_0.<< PutOrPost > b__0 > d << PutOrPost > b__0 > d;

                << PutOrPost > b__0 > d.<> 4__this = CS$<> 8__locals1;

                << PutOrPost > b__0 > d.<> t__builder = AsyncTaskMethodBuilder.Create();

                << PutOrPost > b__0 > d.<> 1__state = -1;
                AsyncTaskMethodBuilder<> t__builder = << PutOrPost > b__0 > d.<> t__builder;

                <> t__builder.Start < WebManager.<> c__DisplayClass12_0.<< PutOrPost > b__0 > d > (ref << PutOrPost > b__0 > d);
                return << PutOrPost > b__0 > d.<> t__builder.Task;
            });
        }

        private static async Task VerifyResponse(HttpResponseMessage message)
        {
            if (!message.IsSuccessStatusCode)
            {
                string responseText = string.Empty;
                try
                {
                    string text = await message.Content.ReadAsStringAsync();
                    responseText = text;
                }
                catch
                {
                }
                throw new InvalidOperationException(string.Format("{0} ({1})\n{2}", message.StatusCode.ToString(), (int)message.StatusCode, responseText));
            }
        }

        private class ProgressHttpContent : HttpContent
        {
            public ProgressHttpContent(Stream sourceStream, CancellableProgress progress)
            {
                this.m_sourceStream = sourceStream;
                this.m_progress = (progress ?? new CancellableProgress());
            }

            protected override bool TryComputeLength(out long length)
            {
                length = this.m_sourceStream.Length;
                return true;
            }

            protected override async Task SerializeToStreamAsync(Stream targetStream, TransportContext context)
            {
                byte[] buffer = new byte[1024];
                int read = 0;
                long written = 0L;
                for (; ; )
                {
                    this.m_progress.Total = (float)this.m_sourceStream.Length;
                    this.m_progress.Completed = (float)written;
                    if (this.m_progress.CancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    read = this.m_sourceStream.Read(buffer, 0, buffer.Length);
                    if (read > 0)
                    {
                        await targetStream.WriteAsync(buffer, 0, read, this.m_progress.CancellationToken);
                        written += (long)read;
                    }
                    if (read <= 0)
                    {
                        return;
                    }
                }
                throw new OperationCanceledException("Operation cancelled.");
            }

            private Stream m_sourceStream;

            private CancellableProgress m_progress;
        }
    }
}
*/