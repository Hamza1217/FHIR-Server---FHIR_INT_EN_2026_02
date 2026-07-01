using fhir_server_entity_model;
using fhir_server_sharedservices;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Net;

namespace fhir_server_CSharp
{
    public static class MedicationRequestSearchParameterValidation
    {
        public static bool ValidatePOSTParameters(HttpListenerRequest request, out DomainResource operation) 
        {
            bool validationResults;
            if (request.QueryString.Count != 0 || !request.RawUrl.ToUpper().EndsWith("MEDICATIONREQUEST"))
            {
                operation = handleInvalidScenario($"Parameter(s) '{request.Url}' invalid when creating a resource in the server. Use POST body instead.");
                return validationResults;
            }

            string medicationRequestFromBody = Utilz.FetchRequestBody(request);

            if (string.IsNullOrEmpty(medicationRequestFromBody)) 
            {
                operation = handleInvalidScenario($"POST body cannot be empty. Valid MedicationRequest json payload is required.");
                return validationResults;
            }

            if(request.Headers == null || request.Headers.Count == 0) 
            {
                operation = handleInvalidScenario($"Content-Type is not specified. Please check the CapabilityStatement for more details about this server.");
                return validationResults;
            }

            if (request.Headers != null && request.Headers.Count > 0)
            {
                string contentType = request.Headers["Content-Type"];
                if(string.IsNullOrEmpty(contentType) || 
                    !(
                        contentType.Equals("application/json", StringComparison.OrdinalIgnoreCase) || 
                        contentType.Equals("application/fhir+json", StringComparison.OrdinalIgnoreCase)
                     )
                ) 
                {
                    operation = handleInvalidScenario($"Content-Type [{contentType}] is not valid for this server. Please check the CapabilityStatement for more details about this server.");
                    return validationResults;
                }
            }

            try 
            {
                DomainResource medicationRequest = SharedServices.ParseResource(medicationRequestFromBody);

                if (medicationRequest.TypeName != "MedicationRequest") 
                {
                    operation = handleInvalidScenario($"Json being passed via body is not a MedicationRequest. It is a '{medicationRequest.TypeName}' instead.");
                    return validationResults;
                }

                OperationOutcome operationOutcome;
                validationResults = SharedServices.ValidateResource(medicationRequest, out operationOutcome);
                if (!validationResults) 
                {
                    Program.HttpStatusCodeForResponse = (int)HttpStatusCode.UnprocessableEntity;
                    operation = operationOutcome;
                    return validationResults;
                }

                //If all are good, the assigning the MedicationResquest resource to out parameter.
                operation = medicationRequest;
            }
            catch(Exception ex) 
            {
                operation = handleInvalidScenario($"{ex.Message}. Unable to convert POST body to a valid MedicationRequest object.");
                return validationResults;
            }
            

            OperationOutcome handleInvalidScenario(string message) 
            {
                validationResults = false;
                Program.HttpStatusCodeForResponse = (int)HttpStatusCode.UnprocessableEntity;
                return Utilz.getErrorOperationOutcome($"{message}", OperationOutcome.IssueSeverity.Error);
            }

            return validationResults;
        }

        public static bool ValidateSearchParams(HttpListenerRequest request, ref bool hardIdSearch, out DomainResource operation,out List<LegacyFilter> criteria)
        {

            operation = null;
            string searchParamId = string.Empty;
            bool rtnValue = true;
            criteria = new List<fhir_server_entity_model.LegacyFilter>();
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
                Program.HttpStatusCodeForResponse = (int)HttpStatusCode.BadRequest;
                operation = Utilz.getErrorOperationOutcome($"Unsupported http method '{request.HttpMethod}' for MedicationRequest resource- Server knows how to handle: [GET, POST] only for MedicationRequest resource");
            }
            else if (string.IsNullOrEmpty(resourceBeingSearched))
            {
                rtnValue = false;
                Program.HttpStatusCodeForResponse = (int)HttpStatusCode.BadRequest;
                operation = Utilz.getErrorOperationOutcome($"Unknown resource type '{resourceBeingSearched}' - Server knows how to handle: [Patient, Practitioner, MedicationRequest]");
            }
            else if (!resourceBeingSearched.Equals("MedicationRequest", StringComparison.Ordinal))
            {
                rtnValue = false;
                Program.HttpStatusCodeForResponse = (int)HttpStatusCode.BadRequest;
                operation = Utilz.getErrorOperationOutcome($"Unknown resource type '{resourceBeingSearched}' - Server knows how to handle: [Patient, Practitioner, MedicationRequest]");
            }
            else if (resourceBeingSearched.Equals("MedicationRequest", StringComparison.Ordinal) && request.QueryString != null && request.QueryString.Count == 0 && !string.IsNullOrEmpty(searchParamId))
            {
                    hardIdSearch = true;
                    LegacyFilter item=new LegacyFilter();
                    item.criteria=LegacyFilter.field.id;
                    item.value=searchParamId;
                    criteria.Add(item);
                    return rtnValue;
                
            }
            else
            {
                

                if (request.QueryString != null && request.QueryString.Count > 0)
                {
                    var dctCodes = new Dictionary<string, List<string>>();

                    foreach (var param in request.QueryString)
                    {
                        if (param != null && param.ToString().Equals("_id", StringComparison.OrdinalIgnoreCase))
                        {
                            //check the case now;
                            if (!param.ToString().Equals("_id", StringComparison.Ordinal))
                            {
                                rtnValue = false;
                                Program.HttpStatusCodeForResponse = (int)HttpStatusCode.BadRequest;
                                operation = Utilz.getErrorOperationOutcome($"Unknown search parameter \"{param}\". Refer to CapabilityStatement of this server to find supported search parameters for this resource.");
                                break;
                            }
                             LegacyFilter item=new LegacyFilter();
                             item.criteria=LegacyFilter.field._id;
                             item.value=request.QueryString[param.ToString()];
                             criteria.Add(item);
                         }
                        else if (param != null && param.ToString().Equals("subject", StringComparison.OrdinalIgnoreCase))
                        {
                            //check the case now;
                            if (!param.ToString().Equals("subject", StringComparison.Ordinal))
                            {
                                rtnValue = false;
                                Program.HttpStatusCodeForResponse = (int)HttpStatusCode.BadRequest;
                                operation = Utilz.getErrorOperationOutcome($"Unknown search parameter \"{param}\". Refer to CapabilityStatement of this server to find supported search parameters for this resource.");
                                break;
                            }

                            string referenceElement = request.QueryString[param.ToString()];
                            var refData = referenceElement.Split(new string[] { ":", "|" }, StringSplitOptions.RemoveEmptyEntries);
                            if (refData != null && refData.Length == 2)
                            {
                                LegacyFilter item=new LegacyFilter();
                                item.criteria=LegacyFilter.field.subject;
                                item.value=refData[1].Trim();
                                criteria.Add(item);
                                
                            }
                            else
                            {
                                if (refData.Length == 0)
                                {
                                    LegacyFilter item=new LegacyFilter();
                                    item.criteria=LegacyFilter.field.subject;
                                    item.value=$"Patient/{long.MinValue}";
                                    criteria.Add(item);
                                }
                                else
                                {
                                    if (refData[0].StartsWith("Patient/", StringComparison.OrdinalIgnoreCase))
                                    {
                                        LegacyFilter item=new LegacyFilter();
                                        item.criteria=LegacyFilter.field.subject;
                                        item.value=refData[0].Trim();
                                        criteria.Add(item);
                                            
                                    }
                                    else
                                    {
                                        LegacyFilter item=new LegacyFilter();
                                        item.criteria=LegacyFilter.field.subject;
                                        item.value=$"Patient/{refData[0].Trim()}";
                                        criteria.Add(item);
                                        
                                    }
                                }
                            }
                        }
                        else if (param != null && param.ToString().Equals("patient", StringComparison.OrdinalIgnoreCase))
                        {
                            //check the case now;
                            if (!param.ToString().Equals("patient", StringComparison.Ordinal))
                            {
                                rtnValue = false;
                                Program.HttpStatusCodeForResponse = (int)HttpStatusCode.BadRequest;
                                operation = Utilz.getErrorOperationOutcome($"Unknown search parameter \"{param}\". Refer to CapabilityStatement of this server to find supported search parameters for this resource.");
                                break;
                            }

                            string patientId = request.QueryString[param.ToString()].Trim();
                            long.TryParse(patientId, out long parsedResult);

                            if (parsedResult == 0) { parsedResult = long.MinValue; }
                               LegacyFilter item=new LegacyFilter();
                                        item.criteria=LegacyFilter.field.subject;
                                        item.value=$"Patient/{parsedResult}";
                                        criteria.Add(item);
                                     
                        }
                        
                        else
                        {
                            if (param == null)
                            {
                                rtnValue = false;
                                Program.HttpStatusCodeForResponse = (int)HttpStatusCode.BadRequest;
                                operation = Utilz.getErrorOperationOutcome($"The validated expression is false");
                                break;
                            }
                            else
                            {
                                rtnValue = false;
                                Program.HttpStatusCodeForResponse = (int)HttpStatusCode.BadRequest;
                                operation = Utilz.getErrorOperationOutcome($"Unknown search parameter \"{param}\". Refer to CapabilityStatement of this server to find supported search parameters for this resource.");
                                break;
                            }
                        }
                    }
                    
                    
                    
                        
                }

            }

            
            return rtnValue;
        }
    }
}
