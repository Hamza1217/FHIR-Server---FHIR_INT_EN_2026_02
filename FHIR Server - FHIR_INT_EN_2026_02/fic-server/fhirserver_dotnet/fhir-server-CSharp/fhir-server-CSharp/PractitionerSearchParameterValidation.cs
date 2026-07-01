using fhir_server_sharedservices;
using Hl7.Fhir.Model;
using fhir_server_entity_model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace fhir_server_CSharp
{
    public static class PractitionerSearchParameterValidation
    {
        public static bool ValidateSearchParams(HttpListenerRequest request, ref bool hardIdSearch, out DomainResource operation, out List<LegacyFilter> criteria)
        {
            operation = null;
            criteria = new List<LegacyFilter>();

            string searchParamId = string.Empty;
            bool rtnValue = true;

            string resourceBeingSearched = request.Url.AbsolutePath.Replace(FhirServerConfig.FHIRServerUrl, string.Empty);

            if (!string.IsNullOrEmpty(resourceBeingSearched) && resourceBeingSearched.Contains("/"))
            {
                searchParamId = resourceBeingSearched.Substring(resourceBeingSearched.IndexOf("/"));
                if (searchParamId.StartsWith("/"))
                {
                    resourceBeingSearched = resourceBeingSearched.Substring(0, resourceBeingSearched.IndexOf("/"));
                    searchParamId = searchParamId.Substring(1);
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

            if (!request.HttpMethod.Trim().ToUpper().Equals("GET"))
            {
                rtnValue = false;
                Program.HttpStatusCodeForResponse = (int)HttpStatusCode.MethodNotAllowed;
                operation = Utilz.getErrorOperationOutcome($"Unsupported http method '{request.HttpMethod}' for Practitioner resource- Server knows how to handle: [GET] only for Practitioner resource");
            }
            else if (string.IsNullOrEmpty(resourceBeingSearched) || !resourceBeingSearched.Equals("Practitioner", StringComparison.Ordinal))
            {
                rtnValue = false;
                Program.HttpStatusCodeForResponse = (int)HttpStatusCode.BadRequest;
                operation = Utilz.getErrorOperationOutcome($"Unknown resource type '{resourceBeingSearched}' - Server knows how to handle: [Patient, Practitioner, MedicationRequest]");
            }
            else if (request.QueryString != null && request.QueryString.Count == 0 && !string.IsNullOrEmpty(searchParamId))
            {
                if (!long.TryParse(searchParamId, out _))
                {
                    rtnValue = false;
                    Program.HttpStatusCodeForResponse = (int)HttpStatusCode.NotFound;
                    operation = Utilz.getErrorOperationOutcome($"HTTP 404 Not Found: Resource {resourceBeingSearched}/{searchParamId} is not known");
                }
                else
                {
                    LegacyFilter sc = new LegacyFilter();
                    sc.criteria = LegacyFilter.field.id;
                    sc.value = searchParamId;
                    criteria.Add(sc);
                    hardIdSearch = true;
                    return true;
                }
            }
            else if (request.QueryString != null && request.QueryString.Count > 0)
            {
                foreach (var param in request.QueryString)
                {
                    string paramName = param.ToString();

                    if (paramName.Equals("_id", StringComparison.OrdinalIgnoreCase))
                    {
                        LegacyFilter sc = new LegacyFilter();
                        sc.criteria = LegacyFilter.field._id;
                        sc.value = request.QueryString[paramName];
                        criteria.Add(sc);
                    }
                    else if (paramName.Equals("identifier", StringComparison.OrdinalIgnoreCase))
                    {
                        string[] systemAndValue = request.QueryString[paramName].Split("|", StringSplitOptions.RemoveEmptyEntries);
                        string searchSystem = systemAndValue.Length > 1 ? systemAndValue[0] : string.Empty;
                        string searchValue = systemAndValue.Length > 1 ? systemAndValue[1] : systemAndValue[0];

                        string searchType = SharedServices.GetSystemTypeMapping().SystemMap
                            .Where(e => e.System.Equals(searchSystem, StringComparison.Ordinal))
                            .Select(e => e.Type).FirstOrDefault();

                        if (!string.Equals(searchType, "NPI", StringComparison.Ordinal))
                        {
                            rtnValue = false;
                            Program.HttpStatusCodeForResponse = (int)HttpStatusCode.BadRequest;
                            operation = Utilz.getErrorOperationOutcome($"HTTP 400 Bad Request: Practitioners can only be found knowing the NPI identifier - You are specifying : {searchType ?? searchSystem}");
                            break;
                        }

                        LegacyFilter sc = new LegacyFilter();
                        sc.criteria = LegacyFilter.field.identifier;
                        sc.value = searchSystem + "|" + searchValue;
                        criteria.Add(sc);
                    }
                    else if (paramName.Equals("family", StringComparison.OrdinalIgnoreCase))
                    {
                        LegacyFilter sc = new LegacyFilter();
                        sc.criteria = LegacyFilter.field.family;
                        sc.value = request.QueryString[paramName];
                        criteria.Add(sc);
                    }
                    else if (paramName.Equals("given", StringComparison.OrdinalIgnoreCase))
                    {
                        LegacyFilter sc = new LegacyFilter();
                        sc.criteria = LegacyFilter.field.given;
                        sc.value = request.QueryString[paramName];
                        criteria.Add(sc);
                    }
                    else if (paramName.Equals("name", StringComparison.OrdinalIgnoreCase))
                    {
                        LegacyFilter sc = new LegacyFilter();
                        sc.criteria = LegacyFilter.field.name;
                        sc.value = request.QueryString[paramName];
                        criteria.Add(sc);
                    }
                    else if (paramName.Equals("gender", StringComparison.OrdinalIgnoreCase))
                    {
                        LegacyFilter sc = new LegacyFilter();
                        sc.criteria = LegacyFilter.field.gender;
                        sc.value = request.QueryString[paramName];
                        criteria.Add(sc);
                    }
                    else if (paramName.Equals("email", StringComparison.OrdinalIgnoreCase))
                    {
                        LegacyFilter sc = new LegacyFilter();
                        sc.criteria = LegacyFilter.field.email;
                        sc.value = request.QueryString[paramName];
                        criteria.Add(sc);
                    }
                    else if (paramName.Equals("telecom", StringComparison.OrdinalIgnoreCase))
                    {
                        string[] telecomParts = request.QueryString[paramName].Split("|", StringSplitOptions.RemoveEmptyEntries);
                        string telecomSystem = telecomParts.Length > 1 ? telecomParts[0] : null;
                        string telecomValue = telecomParts.Length > 1 ? telecomParts[1] : telecomParts[0];

                        if (!string.IsNullOrEmpty(telecomSystem) && !telecomSystem.Equals("email", StringComparison.OrdinalIgnoreCase))
                        {
                            rtnValue = false;
                            Program.HttpStatusCodeForResponse = (int)HttpStatusCode.NotImplemented;
                            operation = Utilz.getErrorOperationOutcome($"HTTP 501 Not Implemented: The underlying server only handles email addresses for the practitioners, thus search by system={telecomSystem} is not implemented");
                            break;
                        }

                        LegacyFilter sc = new LegacyFilter();
                        sc.criteria = LegacyFilter.field.email;
                        sc.value = telecomValue;
                        criteria.Add(sc);
                    }
                    else
                    {
                        rtnValue = false;
                        Program.HttpStatusCodeForResponse = (int)HttpStatusCode.BadRequest;
                        operation = Utilz.getErrorOperationOutcome($"Unknown search parameter \"{paramName}\". Valid search parameters for Practitioner are: [_id, identifier, name, family, given, gender, email, telecom]");
                        break;
                    }
                }
            }

            return rtnValue;
        }
    }
}
