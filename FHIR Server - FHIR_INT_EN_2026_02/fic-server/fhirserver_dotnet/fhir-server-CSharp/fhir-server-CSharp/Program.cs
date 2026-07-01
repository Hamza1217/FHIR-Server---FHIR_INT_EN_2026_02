using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System;
using System.Configuration;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.Generic;
using fhir_server_sharedservices;
using fhir_server_entity_model;

namespace fhir_server_CSharp
{
    public class Program
    {
        private static int RequestCounter = 1;
        private static StringBuilder sb = new StringBuilder();

        private const string MSG0000000001 = "Web Server Started (STATUS - RUNNING)";
        private const string MSG0000000002 = "Application Error. Unable to open and listen to the port {0}.";

        private static string LISTENERURL = null;
        internal static int HttpStatusCodeForResponse = 200;
        private static string LocationHeaderValue = string.Empty;
        private static void Main(string[] args)
        {
            string listenerPort = FhirServerConfig.PortListening.ToString();
            string serverName = "localhost";//Environment.MachineName.ToUpper().Trim();

            WebServer FhirWebServer = null;

            try
            {
                LISTENERURL = $"{Constants.HTTPPROTOCOL}://{serverName}:{listenerPort}{FhirServerConfig.FHIRServerUrl.Trim()}";
                FhirWebServer = new WebServer
                (
                    SendResponse,
                    Constants.MIMETYPE_JSON,
                    LISTENERURL
                );

                FhirWebServer.Run
                    (
                        s =>
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write($"{sb.ToString()}");

                            if (HttpStatusCodeForResponse >= 400 && HttpStatusCodeForResponse <= 506)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                            }

                            Console.WriteLine($"{s}");
                            Console.WriteLine(Environment.NewLine);
                        },
                        () => { if (sb != null) { sb.Clear(); } },
                        () => { return HttpStatusCodeForResponse; },
                        (statusCode) => { HttpStatusCodeForResponse = statusCode; },
                        () => { return LocationHeaderValue; }
                    );

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(new string('-', Console.BufferWidth));

                /*
                 * Calculates the display buffer length and center the text accordingly
                 */
                //int bufferWidth = Console.BufferWidth;
                int bufferWidth = 80;
                int textLength = MSG0000000001.Length;
                int lengthDifference = bufferWidth - textLength;
                int halfLengthDifference = lengthDifference / 2;
                string textToDisplay = new string('-', halfLengthDifference) + MSG0000000001 + new string('-', halfLengthDifference + 10);

                if (textToDisplay.Length > bufferWidth)
                {
                    textToDisplay = textToDisplay.Substring(0, bufferWidth);
                }

                Console.WriteLine(textToDisplay);
                Console.WriteLine(new string('-', Console.BufferWidth));
                Console.WriteLine($"{new string(' ', halfLengthDifference)} {Constants.HTTPPROTOCOL}://{serverName}:{listenerPort}{FhirServerConfig.FHIRServerUrl.Trim()}");

                Console.WriteLine("");
                Console.WriteLine(new string('-', Console.BufferWidth));

                Console.ForegroundColor = ConsoleColor.Green;

                Console.Read();
            }
            catch (HttpListenerException ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(new string('-', Console.BufferWidth));

                Console.WriteLine(string.Format(MSG0000000002, listenerPort.ToString()) + Environment.NewLine);
                Console.WriteLine(ex.Message);
                Console.WriteLine();

                Console.WriteLine(new string('-', Console.BufferWidth));

                Console.Read();
            }
            finally
            {
                if (FhirWebServer != null)
                {
                    FhirWebServer.Stop();
                }
            }
        }

        private static string SendResponse(HttpListenerRequest request)
        {
            string strResponse = string.Empty;
            HttpStatusCodeForResponse = 200;
            LocationHeaderValue = null;

            Utilz.PrintRequest(request, ref RequestCounter);
            sb.Append($"RS {RequestCounter} : ");

            if (IsResourceAllowedInThisServer(request, out string resource))
            {
                if (resource.Equals(ResourceType.Patient.ToString(), StringComparison.Ordinal))
                {
                    strResponse = Patient_Route(request);
                }
               
                else if (resource.Equals(ResourceType.MedicationRequest.ToString(), StringComparison.Ordinal))
                {
                    strResponse = MedicationRequest_Route(request);
                }
                else if (resource.Equals(ResourceType.CapabilityStatement.ToString(), StringComparison.Ordinal) || resource.Equals("metadata", StringComparison.Ordinal))
                {
                    strResponse = CapabilityStatement_Route(request);
                }
            }
            else
            {
                RequestCounter++;
                HttpStatusCodeForResponse = (int)HttpStatusCode.BadRequest;
                var errorOutcome = Utilz.getErrorOperationOutcome($"Resource '{resource}' is not supported in this server.");
                return errorOutcome.ToJson(new FhirJsonSerializationSettings() { AppendNewLine = false, Pretty = false, IgnoreUnknownElements = true });
            }

            RequestCounter++;

            return strResponse;
        }

        private static bool IsResourceAllowedInThisServer(HttpListenerRequest request, out string resource)
        {
            bool isResourceAllowed = false;

            string resourceBeingSearched = request.Url.AbsolutePath.Replace(FhirServerConfig.FHIRServerUrl, string.Empty);

            string searchParamId;
            if (!string.IsNullOrEmpty(resourceBeingSearched) && resourceBeingSearched.Contains("/"))
            {
                searchParamId = resourceBeingSearched.Substring(resourceBeingSearched.IndexOf("/"));
                if (searchParamId.StartsWith("/"))
                {
                    resourceBeingSearched = resourceBeingSearched.Substring(0, resourceBeingSearched.IndexOf("/"));
                }
            }
            else if (!string.IsNullOrEmpty(resourceBeingSearched) && resourceBeingSearched.Contains("?"))
            {
                searchParamId = resourceBeingSearched.Substring(resourceBeingSearched.IndexOf("?"));
                if (searchParamId.StartsWith("?"))
                {
                    resourceBeingSearched = resourceBeingSearched.Substring(0, resourceBeingSearched.IndexOf("?"));
                }
            }

            if (!string.IsNullOrEmpty(resourceBeingSearched) &&
                    (
                        resourceBeingSearched.Equals(ResourceType.Patient.ToString(), StringComparison.Ordinal) ||
                        resourceBeingSearched.Equals(ResourceType.Practitioner.ToString(), StringComparison.Ordinal) ||
                        resourceBeingSearched.Equals(ResourceType.MedicationRequest.ToString(), StringComparison.Ordinal) ||
                        resourceBeingSearched.Equals(ResourceType.CapabilityStatement.ToString(), StringComparison.Ordinal) ||
                        resourceBeingSearched.Equals("metadata", StringComparison.Ordinal)
                    )
               )
            {
                if (resourceBeingSearched.Equals("metadata"))
                {
                    if (request.QueryString != null && request.QueryString.Count > 0 && request.QueryString.Count <= 2)
                    {
                        string mode = request.QueryString["Mode"];
                        string format = request.QueryString["_format"];

                        if (!string.IsNullOrEmpty(mode) && mode.Equals("full", StringComparison.OrdinalIgnoreCase))
                        {
                            isResourceAllowed = true;

                            if (!string.IsNullOrEmpty(format))
                            {
                                if (!(format.Equals("application/json", StringComparison.OrdinalIgnoreCase) || format.Equals("application/fhir+json", StringComparison.OrdinalIgnoreCase)))
                                {
                                    resourceBeingSearched = request.RawUrl.Replace(FhirServerConfig.FHIRServerUrl, string.Empty);
                                    isResourceAllowed = false;
                                }
                            }

                        }
                        else
                        {
                            resourceBeingSearched = request.RawUrl.Replace(FhirServerConfig.FHIRServerUrl, string.Empty);
                            isResourceAllowed = false;
                        }
                    }
                    else if (request.QueryString != null && request.QueryString.Count > 1)
                    {
                        resourceBeingSearched = request.RawUrl.Replace(FhirServerConfig.FHIRServerUrl, string.Empty);
                        isResourceAllowed = false;
                    }
                    else
                    {
                        isResourceAllowed = true;
                    }
                }
                else if (resourceBeingSearched.Equals(ResourceType.MedicationRequest.ToString()) && request.QueryString.Count > 0)
                {
                    string searchParam = request.QueryString["_include"];
                    if (!string.IsNullOrEmpty(searchParam) && searchParam.Contains(ResourceType.MedicationRequest.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        isResourceAllowed = true;
                    }
                    else if
                        (
                            !string.IsNullOrEmpty(request.QueryString["_id"]) ||
                            !string.IsNullOrEmpty(request.QueryString["status"]) ||
                            !string.IsNullOrEmpty(request.QueryString["intent"]) ||
                            !string.IsNullOrEmpty(request.QueryString["subject"]) ||
                            !string.IsNullOrEmpty(request.QueryString["patient"]) ||
                            !string.IsNullOrEmpty(request.QueryString["requester"])
                        )
                    {
                        isResourceAllowed = true;
                    }
                    else
                    {
                        resourceBeingSearched = request.RawUrl.Replace(FhirServerConfig.FHIRServerUrl, string.Empty);
                        isResourceAllowed = false;
                    }
                }
                else
                {
                    isResourceAllowed = true;
                }
            }
            resource = resourceBeingSearched;


            return isResourceAllowed;
        }
        private static String CapabilityStatement_Route(HttpListenerRequest request)
        {
            string strResponse = String.Empty;
            if (!request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                HttpStatusCodeForResponse = (int)HttpStatusCode.MethodNotAllowed;
                strResponse = Utilz.getErrorOperationOutcome($"CapabilityStatement does not support [{request.HttpMethod}] in this server.").ToJson();
            }
            else
            {
                var capability = CapabilityOfTheServer(request.Url.ToString());

                if (FhirServerConfig.ValidateCapabilityStatementResource)
                {
                    bool isResourceValid = SharedServices.ValidateResource(capability, out OperationOutcome outcome);

                    if (!isResourceValid)
                    {
                        return outcome.ToJson(new FhirJsonSerializationSettings() { Pretty = false, AppendNewLine = false, IgnoreUnknownElements = true });
                    }
                }

                strResponse = capability.ToJson(new FhirJsonSerializationSettings() { AppendNewLine = false, IgnoreUnknownElements = true, Pretty = true });
            }
            return strResponse;
        }
        private static String MedicationRequest_Route(HttpListenerRequest request)
        {
            String strResponse = String.Empty;
            List<fhir_server_entity_model.LegacyFilter> criteria = new List<fhir_server_entity_model.LegacyFilter>();
            bool HardIdSearch = false;
            HttpStatusCodeForResponse = 200;
            LocationHeaderValue = null;

            Utilz.PrintRequest(request, ref RequestCounter);
            sb.Append($"RS {RequestCounter} : ");
            if (!MedicationRequestSearchParameterValidation.ValidateSearchParams(request, ref HardIdSearch, out DomainResource operation,out criteria))
            {
                RequestCounter++;
                return operation.ToJson(new FhirJsonSerializationSettings() { AppendNewLine = false, Pretty = false, IgnoreUnknownElements = true });
            }
            else
            {
                if (criteria.Count() == 0 && !HardIdSearch)
                {
                    
                    var data = fhir_server_dataaccess.MedicationRequestDataAccess.GetAllMedicationRequests();
                    strResponse = fhir_server_mapping.MapMedicationRequestBundle.GetMedicationRequestBundle(data, request.Url.ToString());
                }
                else
                {
                    
                    if (criteria.Count()>0)
                    {
                        Console.WriteLine(criteria[0].criteria.ToString());
                        Console.WriteLine(criteria[0].value.ToString());
                        
                    }
                    var data = fhir_server_dataaccess.MedicationRequestDataAccess.GetMedicationRequest(criteria);

                    if (HardIdSearch)
                    {
                        if (data != null && data.Count == 0)
                        {
                            HttpStatusCodeForResponse = (int)HttpStatusCode.NotFound;
                            strResponse = Utilz.getErrorOperationOutcome($"Unable to find MedicationRequest @ {request.Url}", OperationOutcome.IssueSeverity.Information).ToJson(new FhirJsonSerializationSettings() { AppendNewLine = false, Pretty = false, IgnoreUnknownElements = true });
                        }
                        else
                        {
                            var medRequest = fhir_server_mapping.MapMedicationRequest.GetFHIRMedicationRequestResource(data[0]);

                            if (FhirServerConfig.ValidateMedicationRequestBundleAndResource)
                            {
                                bool isResourceValid = SharedServices.ValidateResource(medRequest, out OperationOutcome outcome);

                                if (!isResourceValid)
                                {
                                    return outcome.ToJson(new FhirJsonSerializationSettings() { Pretty = false, AppendNewLine = false, IgnoreUnknownElements = true });
                                }
                            }

                            strResponse = medRequest.ToJson(new FhirJsonSerializationSettings() { Pretty = false, AppendNewLine = false, IgnoreUnknownElements = true });
                        }
                    }
                    else
                    {
                        
                        strResponse = fhir_server_mapping.MapMedicationRequestBundle.GetMedicationRequestBundle(data, request.Url.ToString());
                    }


                }
            }
            return strResponse;
        }
        private static String Patient_Route(HttpListenerRequest request)
        {
            String strResponse = String.Empty;
            List<fhir_server_entity_model.LegacyFilter> criteria = new List<fhir_server_entity_model.LegacyFilter>();
            bool HardIdSearch = false;
            HttpStatusCodeForResponse = 200;
            LocationHeaderValue = null;

            Utilz.PrintRequest(request, ref RequestCounter);
            sb.Append($"RS {RequestCounter} : ");


            if (!PatientSearchParameterValidation.ValidateSearchParams(request, ref HardIdSearch, out DomainResource operation, out criteria))
            {
                RequestCounter++;
                return operation.ToJson(new FhirJsonSerializationSettings() { AppendNewLine = false, Pretty = false, IgnoreUnknownElements = true });
            }


            if ((criteria.Count == 0) && (!HardIdSearch))
            {
                var data = fhir_server_dataaccess.PatientDataAccess.GetAllPatients();
                strResponse = fhir_server_mapping.MapPatientBundle.GetPeopleBundle(data, request.Url.ToString());
            }
            else
            {
                var data = fhir_server_dataaccess.PatientDataAccess.GetPerson(criteria);

                if (HardIdSearch)
                {
                    if (data != null && data.Count == 0)
                    {
                        HttpStatusCodeForResponse = (int)HttpStatusCode.NotFound;
                        strResponse = Utilz.getErrorOperationOutcome($"Unable to find patient @ {request.Url}", OperationOutcome.IssueSeverity.Information).ToJson(new FhirJsonSerializationSettings() { AppendNewLine = false, Pretty = false, IgnoreUnknownElements = true });
                    }
                    else
                    {
                        var patient = fhir_server_mapping.MapPatient.GetFHIRPatientResource(data[0]);

                        if (FhirServerConfig.ValidatePatientBundleAndResource)
                        {
                            bool isResourceValid = SharedServices.ValidateResource(patient, out OperationOutcome outcome);

                            if (!isResourceValid)
                            {
                                return outcome.ToJson(new FhirJsonSerializationSettings() { Pretty = false, AppendNewLine = false, IgnoreUnknownElements = true });
                            }
                        }

                        strResponse = patient.ToJson(new FhirJsonSerializationSettings() { Pretty = false, AppendNewLine = false, IgnoreUnknownElements = true });
                    }
                }
                else
                {
                    strResponse = fhir_server_mapping.MapPatientBundle.GetPeopleBundle(data, request.Url.ToString());
                }
            }

            return strResponse;
        }
        private static DomainResource CapabilityOfTheServer(string url)
        {
            return new CapabilityStatement()
            {
                Id = Guid.NewGuid().ToString(),
                Url = url,
                Version = "1.0",
                Name = "fic-server-dotnet",
                Title = "HL7 FHIR Intermediate Course - Server Capability Statement",
                Status = PublicationStatus.Active,
                Experimental = true,
                Date = "2023-05-01T12:09:56+00:00",
                Publisher = "HL7 FHIR Intermediate Course Team",
                Kind = CapabilityStatementKind.Capability,
                Software = new CapabilityStatement.SoftwareComponent() { Name = "HL7 FHIR INTERMEDIATE C# SERVER", Version = "1.0" },
                FhirVersion = FHIRVersion.N4_0_0,
                Format = new List<string>() { "application/fhir+json", "application/json" },
                Contact = new List<ContactDetail>()
                {
                    new ContactDetail()
                    {
                        Name = "Web Master",
                        Telecom = new List<ContactPoint>()
                        {
                            new ContactPoint()
                            {
                                System = ContactPoint.ContactPointSystem.Email,
                                Value = "fernando.campos@gmail.com"
                            }
                        }
                    }
                },
                Jurisdiction = new List<CodeableConcept>()
                {
                    new CodeableConcept()
                    {
                        Coding = new List<Coding>()
                        {
                            new Coding()
                            {
                                System = "urn:iso:std:iso:3166",
                                Code = "US",
                                Display = "United States"
                            }
                        },
                        Text = "United States"
                    }
                },
                Copyright = new Markdown("(C) Open Source - This server is just a didactic example. Not intended to use in production"),
                Rest = new List<CapabilityStatement.RestComponent>()
                {
                    new CapabilityStatement.RestComponent()
                    {
                        Mode = CapabilityStatement.RestfulCapabilityMode.Server,
                        Security = new CapabilityStatement.SecurityComponent()
                        {
                            Cors = true,
                            Service = new List<CodeableConcept>()
                            {
                                new CodeableConcept()
                                {
                                    Coding = new List<Coding>()
                                    {
                                        new Coding() { Code = "Basic", System = "http://terminology.hl7.org/CodeSystem/restful-security-service" }
                                    }
                                }
                            }
                        },
                        Resource = new List<CapabilityStatement.ResourceComponent>()
                        {
                            new CapabilityStatement.ResourceComponent()
                            {
                                Type = ResourceType.Patient.ToString(),
                                Profile = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-patient",
                                Documentation = new Markdown("Describes the supported interactions and search parameters for this resource."),
                                Versioning = CapabilityStatement.ResourceVersionPolicy.NoVersion,
                                Interaction = new List<CapabilityStatement.ResourceInteractionComponent>()
                                {
                                    new CapabilityStatement.ResourceInteractionComponent() { Code = CapabilityStatement.TypeRestfulInteraction.Read } ,
                                    new CapabilityStatement.ResourceInteractionComponent() { Code = CapabilityStatement.TypeRestfulInteraction.SearchType }
                                },
                                SearchParam = new List<CapabilityStatement.SearchParamComponent>()
                                {
                                    new CapabilityStatement.SearchParamComponent()
                                    {
                                        Name = "/[id]",
                                        Type = SearchParamType.Number,
                                        Documentation = new Markdown("Search by resource id. Eg. [base]/Patient/1")
                                    },
                                    new CapabilityStatement.SearchParamComponent()
                                    {
                                        Name = "_id",
                                        Type = SearchParamType.Number,
                                        Documentation = new Markdown("Search by the ID of the resource. Eg. [base]/Patient?_id=1")
                                    },
                                    new CapabilityStatement.SearchParamComponent()
                                    {
                                        Name = "identifier",
                                        Type = SearchParamType.Token,
                                        Documentation = new Markdown("A patient identifier. Eg. [base]/Patient?identifier=https://www.national-office.gov/ni|4136541290")
                                    },
                                    new CapabilityStatement.SearchParamComponent()
                                    {
                                        Name = "name",
                                        Type = SearchParamType.String,
                                        Documentation = new Markdown("A server defined search that may match any of the string fields in the HumanName, including family or given. Eg. [base]/Patient?name=Andrew")
                                    },
                                    new CapabilityStatement.SearchParamComponent()
                                    {
                                        Name = "birthdate",
                                        Type = SearchParamType.Date,
                                        Documentation = new Markdown("The patient's date of birth. Eg. [base]/Patient?name=Andrew&birthdate=1978-10-04")
                                    },
                                    new CapabilityStatement.SearchParamComponent()
                                    {
                                        Name = "gender",
                                        Type = SearchParamType.String,
                                        Documentation = new Markdown("Gender of the patient. Refer to http://hl7.org/fhir/R4/valueset-administrative-gender.html for more information. Eg. [base]/Patient?name=Andrew&gender=male")
                                    },
                                    new CapabilityStatement.SearchParamComponent()
                                    {
                                        Name = "family",
                                        Type = SearchParamType.String,
                                        Documentation = new Markdown("A portion of the family name of the patient")
                                    },
                                    new CapabilityStatement.SearchParamComponent()
                                    {
                                        Name = "telecom",
                                        Type = SearchParamType.Token,
                                        Documentation = new Markdown("The value in any kind of telecom details of the patient")
                                    },
                                    new CapabilityStatement.SearchParamComponent()
                                    {
                                        Name = "email",
                                        Type = SearchParamType.Token,
                                        Documentation = new Markdown("Patient's email")
                                    }
                                }
                            },
                            new CapabilityStatement.ResourceComponent()
                            {
                                Type = ResourceType.MedicationRequest.ToString(),
                                Profile = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-medicationrequest",
                                Documentation = new Markdown("Describes the supported interactions and search parameters for this resource. All include searches for this resource is case insensitive searches."),
                                Versioning = CapabilityStatement.ResourceVersionPolicy.NoVersion,
                                Interaction = new List<CapabilityStatement.ResourceInteractionComponent>()
                                {
                                    new CapabilityStatement.ResourceInteractionComponent(){ Code = CapabilityStatement.TypeRestfulInteraction.Read }
                                },
                                SearchParam = new List<CapabilityStatement.SearchParamComponent>()
                                {
                                    new CapabilityStatement.SearchParamComponent()
                                    {
                                        Name = "/[id]",
                                        Type = SearchParamType.Token,
                                        Documentation = new Markdown("Search by resource id. Eg. [base]/MedicationRequest/1")
                                    },
                                    new CapabilityStatement.SearchParamComponent()
                                    {
                                        Name = "_id",
                                        Type = SearchParamType.Token,
                                        Documentation = new Markdown("Search by the ID of the resource. Eg. [base]/MedicationRequest?_id=1")
                                    },
                                    new CapabilityStatement.SearchParamComponent()
                                    {
                                        Name = "subject",
                                        Type = SearchParamType.Reference,
                                        Documentation = new Markdown("The identity of a patient to list medication requests for.")
                                    },
                                    new CapabilityStatement.SearchParamComponent()
                                    {
                                        Name = "patient",
                                        Type = SearchParamType.Reference,
                                        Documentation = new Markdown("Returns medication requests for a specific patient.")
                                    }
                                }
                            }
                        },
                        Documentation = new Markdown
                        (
                            "This FHIR server supports Patient, Practitioner and MedicationRequest resources. " +
                            "Please check the Capability Statement of this server for more information about the allowed operations and search parameters against the resources mentioned above." +
                            "\n\nThis server supports resource validation as well but this feature is disabled by default to enhance the performance of the server. " +
                            "All settings are configured in appsettings.json file.\n\nThis server runs against port 5834 and this is also a configurable vale in appsettings.json file. "
                        )
                    }
                }
            };
        }
    }
}
