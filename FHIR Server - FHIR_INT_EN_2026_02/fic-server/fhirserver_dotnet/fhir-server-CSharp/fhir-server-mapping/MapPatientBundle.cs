using fhir_server_sharedservices;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System;
using System.Collections.Generic;


namespace fhir_server_mapping
{
    public static class MapPatientBundle
    {
        public static string GetPeopleBundle(List<fhir_server_entity_model.LegacyPerson> people, string listernerUrl) 
        {
            Bundle bundle = new Bundle();
            
            if(listernerUrl != null && listernerUrl.EndsWith("/")) 
            {
                listernerUrl = listernerUrl.Substring(0, listernerUrl.LastIndexOf("/"));
            }

            bundle.Id = Guid.NewGuid().ToString();
            bundle.Meta = new Meta { LastUpdated = DateTime.Now };
            bundle.Type = Bundle.BundleType.Searchset;
            bundle.Link = new List<Bundle.LinkComponent>() { new Bundle.LinkComponent() { Relation = "self", Url = $"{listernerUrl}" } };
            
            List<Bundle.EntryComponent> entry = new List<Bundle.EntryComponent>();
            if(people != null && people.Count > 0) 
            {
                foreach (var person in people)
                {
                    Uri patientUri = new Uri(listernerUrl);

                    entry.Add(new Bundle.EntryComponent() 
                    { 
                        Resource = MapPatient.GetFHIRPatientResource(person), 
                        FullUrl = $"{patientUri.Scheme}://{patientUri.Authority}{string.Join("", patientUri.Segments)}/{person.PRSN_ID}",
                        Search = new Bundle.SearchComponent() { Mode = Bundle.SearchEntryMode.Match }
                    });
                }
            }

            bundle.Timestamp = DateTime.Now;
            bundle.Total = entry.Count;
            bundle.Entry = entry;

            if (FhirServerConfig.ValidatePatientBundleAndResource)
            {
                bool isResourceValid = SharedServices.ValidateResource(bundle, out OperationOutcome operation);

                if (!isResourceValid)
                {
                    return operation.ToJson(new FhirJsonSerializationSettings() { Pretty = false, AppendNewLine = false, IgnoreUnknownElements = true });
                }
            }

            return bundle.ToJson(new FhirJsonSerializationSettings() { Pretty = false, AppendNewLine = false, IgnoreUnknownElements = true });
        }
    }
}
