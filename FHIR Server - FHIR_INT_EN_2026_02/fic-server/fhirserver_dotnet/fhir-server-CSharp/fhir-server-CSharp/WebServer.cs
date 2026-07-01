using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace fhir_server_CSharp
{
    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderMethod;
        private string _MIMEType;

        public WebServer(IReadOnlyCollection<string> prefixes, Func<HttpListenerRequest, string> method)
        {
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("This OS is not supported for a HttpListener.");
            }

            // URI prefixes are required eg: "http://localhost:8080/test/"
            if (prefixes == null || prefixes.Count == 0)
            {
                throw new ArgumentException("URI prefixes are required");
            }

            if (method == null)
            {
                throw new ArgumentException("responder method required");
            }

            foreach (var s in prefixes)
            {
                _listener.Prefixes.Add(s);
            }

            _responderMethod = method;
            _listener.Start();
        }

        public WebServer(Func<HttpListenerRequest, string> method, string MIMEType, params string[] prefixes)
           : this(prefixes, method)
        {
            
            _MIMEType = MIMEType;
        }

        public void Run(Action<string> PrintMessage, Action ClearStrBuilder, Func<int> BindHttpStatusCode, Action<int> SetHttpStatusCode, Func<string> GetLocationHeaderValue)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem(c =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                if (ctx == null)
                                {
                                    return;
                                }
                                var rstr = _responderMethod(ctx.Request);
                                var buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.ContentType = _MIMEType + "; charset=utf-8";
                                ctx.Response.StatusCode = BindHttpStatusCode();
                                ctx.Response.ContentEncoding = Encoding.UTF8;
                                ctx.Response.StatusDescription = GetHttpStatusCodeDescription(ctx.Response.StatusCode);                               
                                httpResponseAddCommonHeaders(ref ctx, GetLocationHeaderValue());
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);

                                PrintMessage(getPrintableResponseMessage(ctx.Response));
                                ClearStrBuilder();
                            }
                            catch (Exception ex)
                            {
                                SetHttpStatusCode(500);
                                var buf = Encoding.UTF8.GetBytes(fhir_server_sharedservices.SharedServices.GetExceptionAsOperationOutcome(ex.Message + Environment.NewLine + ex.StackTrace, Hl7.Fhir.Model.OperationOutcome.IssueSeverity.Error));
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.ContentType = $"{_MIMEType}; charset=utf-8";
                                ctx.Response.StatusCode = BindHttpStatusCode();
                                ctx.Response.ContentEncoding = Encoding.UTF8;
                                ctx.Response.StatusDescription = GetHttpStatusCodeDescription(ctx.Response.StatusCode);
                                httpResponseAddCommonHeaders(ref ctx);
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);

                                PrintMessage(getPrintableResponseMessage(ctx.Response));
                                ClearStrBuilder();
                            }
                            finally
                            {
                                // always close the stream
                                if (ctx != null)
                                {
                                    ctx.Response.OutputStream.Close();
                                }
                            }
                        }, _listener.GetContext());
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"{ex.StackTrace} + {Environment.NewLine} + {ex.StackTrace}");
                }
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }

        private string getPrintableResponseMessage(HttpListenerResponse Response)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in Response.Headers.Keys)
            {
                sb.AppendLine($"{new string(' ', 7)}{item.ToString()}: {Response.Headers.Get(item.ToString())}");
            }
            if (Response.KeepAlive) { sb.AppendLine($"{new string(' ', 7)}Connection: Keep-Alive"); }

            return $"{Constants.HTTPPROTOCOL.ToUpper()}/{Response.ProtocolVersion.ToString()} {Response.StatusCode.ToString()} {Response.StatusDescription}" +
                   $"\n{new string(' ', 7)}Date: {DateTime.Now.ToString("ddd, dd MMM yyyy hh:mm:ss")}" +
                   $"\n{new string(' ', 7)}Server: {Environment.MachineName}" +
                   $"\n{sb.ToString()}";
        }

        private void httpResponseAddCommonHeaders(ref HttpListenerContext ctx, string ContentLocationHeaderValue = null)
        {
            if (ctx != null)
            {
                ctx.Response.AddHeader("Access-Control-Allow-Origin", "*");
                ctx.Response.AddHeader("Access-Control-Allow-Methods", "POST, GET");
                ctx.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

                if (!string.IsNullOrEmpty(ContentLocationHeaderValue))
                {
                    ctx.Response.AddHeader("Content-Location", ContentLocationHeaderValue);
                }

                ctx.Response.AddHeader("Location", ctx.Request.Url.ToString());
            }
        }

        private string GetHttpStatusCodeDescription(int statusCode)
        {
            string HttpStatusDescription = string.Empty;

            switch (statusCode)
            {
                case 0:
                    return HttpStatusDescription;
                case 202:
                    HttpStatusDescription = "Accepted";
                    goto case 0;
                case 208:
                    HttpStatusDescription = "Already Reported";
                    goto case 0;
                case 300:
                    HttpStatusDescription = "Ambiguous";
                    goto case 0;
                case 502:
                    HttpStatusDescription = "Bad Gateway";
                    goto case 0;
                case 400:
                    HttpStatusDescription = "Bad Request";
                    goto case 0;
                case 409:
                    HttpStatusDescription = "Conflict";
                    goto case 0;
                case 100:
                    HttpStatusDescription = "Continue";
                    goto case 0;
                case 201:
                    HttpStatusDescription = "Created";
                    goto case 0;
                case 103:
                    HttpStatusDescription = "Early Hints";
                    goto case 0;
                case 417:
                    HttpStatusDescription = "Expectation Failed";
                    goto case 0;
                case 424:
                    HttpStatusDescription = "Failed Dependency";
                    goto case 0;
                case 403:
                    HttpStatusDescription = "Forbidden";
                    goto case 0;
                case 302:
                    HttpStatusDescription = "Found";
                    goto case 0;
                case 504:
                    HttpStatusDescription = "Gateway Timeout";
                    goto case 0;
                case 410:
                    HttpStatusDescription = "Gone";
                    goto case 0;
                case 505:
                    HttpStatusDescription = "Http Version Not Supported";
                    goto case 0;
                case 226:
                    HttpStatusDescription = "IM Used";
                    goto case 0;
                case 507:
                    HttpStatusDescription = "Insufficient Storage";
                    goto case 0;
                case 500:
                    HttpStatusDescription = "Internal Server Error";
                    goto case 0;
                case 411:
                    HttpStatusDescription = "Length Required";
                    goto case 0;
                case 423:
                    HttpStatusDescription = "Locked";
                    goto case 0;
                case 508:
                    HttpStatusDescription = "Loop Detected";
                    goto case 0;
                case 405:
                    HttpStatusDescription = "Method Not Allowed";
                    goto case 0;
                case 421:
                    HttpStatusDescription = "Misdirected Request";
                    goto case 0;
                case 301:
                    HttpStatusDescription = "Moved";
                    goto case 0;
                case 207:
                    HttpStatusDescription = "Multi Status";
                    goto case 0;
                case 511:
                    HttpStatusDescription = "Network Authentication Required";
                    goto case 0;
                case 204:
                    HttpStatusDescription = "No Content";
                    goto case 0;
                case 203:
                    HttpStatusDescription = "Non Authoritative Information";
                    goto case 0;
                case 406:
                    HttpStatusDescription = "Not Acceptable";
                    goto case 0;
                case 510:
                    HttpStatusDescription = "Not Extended";
                    goto case 0;
                case 404:
                    HttpStatusDescription = "Not Found";
                    goto case 0;
                case 501:
                    HttpStatusDescription = "Not Implemented";
                    goto case 0;
                case 304:
                    HttpStatusDescription = "Not Modified";
                    goto case 0;
                case 200:
                    HttpStatusDescription = "OK";
                    goto case 0;
                case 206:
                    HttpStatusDescription = "Partial Content";
                    goto case 0;
                case 402:
                    HttpStatusDescription = "Payment Required";
                    goto case 0;
                case 308:
                    HttpStatusDescription = "Permanent Redirect";
                    goto case 0;
                case 412:
                    HttpStatusDescription = "Precondition Failed";
                    goto case 0;
                case 428:
                    HttpStatusDescription = "Precondition Required";
                    goto case 0;
                case 102:
                    HttpStatusDescription = "Processing";
                    goto case 0;
                case 407:
                    HttpStatusDescription = "Proxy Authentication Required";
                    goto case 0;
                case 307:
                    HttpStatusDescription = "Redirect Keep Verb";
                    goto case 0;
                case 303:
                    HttpStatusDescription = "Redirect Method";
                    goto case 0;
                case 416:
                    HttpStatusDescription = "Requested Range Not Satisfiable";
                    goto case 0;
                case 413:
                    HttpStatusDescription = "Request Entity Too Large";
                    goto case 0;
                case 431:
                    HttpStatusDescription = "Request Header Fields Too Large";
                    goto case 0;
                case 408:
                    HttpStatusDescription = "Request Timeout";
                    goto case 0;
                case 414:
                    HttpStatusDescription = "Request Uri Too Long";
                    goto case 0;
                case 205:
                    HttpStatusDescription = "Reset Content";
                    goto case 0;
                case 503:
                    HttpStatusDescription = "Service Unavailable";
                    goto case 0;
                case 101:
                    HttpStatusDescription = "Switching Protocols";
                    goto case 0;
                case 429:
                    HttpStatusDescription = "Too Many Requests";
                    goto case 0;
                case 401:
                    HttpStatusDescription = "Unauthorized";
                    goto case 0;
                case 451:
                    HttpStatusDescription = "Unavailable For Legal Reasons";
                    goto case 0;
                case 422:
                    HttpStatusDescription = "Unprocessable Entity";
                    goto case 0;
                case 415:
                    HttpStatusDescription = "Unsupported Media Type";
                    goto case 0;
                case 306:
                    HttpStatusDescription = "Unused";
                    goto case 0;
                case 426:
                    HttpStatusDescription = "Upgrade Required";
                    goto case 0;
                case 305:
                    HttpStatusDescription = "Use Proxy";
                    goto case 0;
                case 506:
                    HttpStatusDescription = "Variant Also Negotiates";
                    goto case 0;
                default:
                    HttpStatusDescription = string.Empty;
                    goto case 0;
            }
        }
    }
}
