using System;
using System.Collections.Generic;
using System.Text;

namespace fhir_server_CSharp
{
    internal static class Utilz
    {
        internal static void PrintRequest(System.Net.HttpListenerRequest request, ref int RequestCounter)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"RQ {RequestCounter.ToString()} : {request.HttpMethod} ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(request.Url.ToString());
            //foreach (var item in request.AcceptTypes)   
            //{
            //    Console.WriteLine($"{new string(' ', 7)}Accept : {item}");
            //}
            if (request.UserLanguages != null && request.UserLanguages.Length > 0)
            {
                Console.WriteLine($"{new string(' ', 7)}Accept-Language : {request.UserLanguages[0]}");
            }
            if (request.LocalEndPoint != null)
            {
                Console.WriteLine($"{new string(' ', 7)}Origin-Address : {request.LocalEndPoint.Address.ToString()}");
                Console.WriteLine($"{new string(' ', 7)}Origin-Port : {request.LocalEndPoint.Port.ToString()}");
            }
            if (request.RemoteEndPoint != null)
            {
                Console.WriteLine($"{new string(' ', 7)}Remote-Address : {request.RemoteEndPoint.Address.ToString()}");
                Console.WriteLine($"{new string(' ', 7)}Remote-Port : {request.RemoteEndPoint.Port.ToString()}");
            }
            Console.WriteLine($"{new string(' ', 7)}Host : {request.UserHostName}");
            Console.WriteLine($"{new string(' ', 7)}User-Agent : {request.UserAgent}");
            //Console.WriteLine("");
        }

        internal static Hl7.Fhir.Model.OperationOutcome getErrorOperationOutcome(string message, Hl7.Fhir.Model.OperationOutcome.IssueSeverity severity = Hl7.Fhir.Model.OperationOutcome.IssueSeverity.Error)
        {
            return new Hl7.Fhir.Model.OperationOutcome()
            {
                Text = new Hl7.Fhir.Model.Narrative()
                {
                    Status = Hl7.Fhir.Model.Narrative.NarrativeStatus.Generated,
                    Div = $"<div xmlns=\"http://www.w3.org/1999/xhtml\"><h1>Operation Outcome</h1><table border=\"0\"><tr><td style=\"font-weight: bold;\">ERROR</td><td>[]</td><td><pre>{message}</pre></td>\n\t\t\t\t\t\n\t\t\t\t\n\t\t\t</tr>\n\t\t</table>\n\t</div>"
                },
                Issue = new List<Hl7.Fhir.Model.OperationOutcome.IssueComponent>()
                    {
                        new Hl7.Fhir.Model.OperationOutcome.IssueComponent()
                        {
                            Severity = severity,
                            Code = Hl7.Fhir.Model.OperationOutcome.IssueType.Processing,
                            Diagnostics = $"{message}"
                        }
                    }
            };
        }

        internal static string FetchRequestBody(System.Net.HttpListenerRequest request)
        {
            using (var bodyStream = new System.IO.StreamReader(request.InputStream))
            {
                string bodyText = null;

                if (request.InputStream.CanSeek)
                {
                    bodyStream.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                    bodyText = bodyStream.ReadToEnd();
                }
                else if (request.InputStream.CanRead)
                {
                    //Some streams don't support seek. So use the ReadToEnd directly.
                    bodyText = bodyStream.ReadToEnd();
                }
                else
                {
                    throw new InvalidOperationException("State of 'request.InputStream' is unknown. Therefore cannot read request body.");
                }

                return bodyText;
            }
        }
    }
}
