using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Validation;
using fhir_server_entity_model;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace fhir_server_sharedservices
{
    public static class SharedServices
    {
        public static LegacySystemMapHelper GetSystemTypeMapping() 
        {
            return LoadSystemMapFromJson();
        }

        public static bool ValidateResource(Resource resource, out OperationOutcome operation)
        {
            string UsCoreJsonRepository = string.Empty;

            using (var pathProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory()))
            {
                if (pathProvider.GetFileInfo(Path.Combine("validation", "us-core.json.zip")).Exists)
                {
                    UsCoreJsonRepository = pathProvider.GetFileInfo(Path.Combine("validation", "us-core.json.zip")).PhysicalPath;
                }
            }
            
            operation = new Hl7.Fhir.Validation.Validator
                            (new ValidationSettings()
                            {
                                ResourceResolver = new MultiResolver(new ZipSource(UsCoreJsonRepository), ZipSource.CreateValidationSource()),
                                GenerateSnapshot = true,
                                Trace = false,
                                EnableXsdValidation = true
                            }
                            ).Validate(resource);

            return operation.Success;
        }

        public static DomainResource ParseResource(string resource)
        {
            FhirJsonParser parser = new FhirJsonParser(new ParserSettings() { AcceptUnknownMembers = false, AllowUnrecognizedEnums = false });
            return parser.Parse<DomainResource>(resource);
        }

        public static string GetExceptionAsOperationOutcome(string message, OperationOutcome.IssueSeverity severity = OperationOutcome.IssueSeverity.Error)
        {
            return new OperationOutcome()
            {
                Text = new Narrative()
                {
                    Status = Hl7.Fhir.Model.Narrative.NarrativeStatus.Generated,
                    Div = $"<div xmlns=\"http://www.w3.org/1999/xhtml\"><h1>Operation Outcome</h1><table border=\"0\"><tr><td style=\"font-weight: bold;\">ERROR</td><td>[]</td><td><pre>{message}</pre></td>\n\t\t\t\t\t\n\t\t\t\t\n\t\t\t</tr>\n\t\t</table>\n\t</div>"
                },
                Issue = new List<OperationOutcome.IssueComponent>()
                    {
                        new Hl7.Fhir.Model.OperationOutcome.IssueComponent()
                        {
                            Severity = severity,
                            Code = Hl7.Fhir.Model.OperationOutcome.IssueType.Processing,
                            Diagnostics = $"{message}"
                        }
                    }
            }.ToJson(new FhirJsonSerializationSettings() { AppendNewLine = true, Pretty = true, IgnoreUnknownElements = false });
        }

        private static LegacySystemMapHelper LoadSystemMapFromJson() 
        {
            LegacySystemMapHelper mappedData = null;
            String location=Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+"/fhir_options.json";
            Console.WriteLine(location);
            
            if (File.Exists(location)) 
            {
                mappedData = JsonSerializer.Deserialize<LegacySystemMapHelper>
                    (
                        File.ReadAllText(location), 
                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull }
                    );
            }
            return mappedData;
        }
    }
}
