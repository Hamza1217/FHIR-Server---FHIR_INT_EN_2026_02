using fhir_server_sharedservices;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System;
using System.Collections.Generic;

namespace fhir_server_mapping
{
    public static class MapMedicationRequestBundle
    {
        public static string GetMedicationRequestBundle(
            List<fhir_server_entity_model.LegacyRx> rxes, string listernerUrl) 
        
        
        {
           Bundle bundle = new Bundle();

            if (listernerUrl != null && listernerUrl.EndsWith("/"))
            {
                listernerUrl = listernerUrl.Substring(0, listernerUrl.LastIndexOf("/"));
            }

            bundle.Id = Guid.NewGuid().ToString();
            bundle.Meta = new Meta { LastUpdated = DateTime.Now };
            bundle.Type = Bundle.BundleType.Searchset;
            bundle.Link = new List<Bundle.LinkComponent>() { new Bundle.LinkComponent() { Relation = "self", Url = $"{listernerUrl}" } };

            List<Bundle.EntryComponent> entry = new List<Bundle.EntryComponent>();
            if (rxes != null && rxes.Count > 0)
            {
                foreach (var rx in rxes)
                {
                    Uri MedicationRequestUri = new Uri(listernerUrl);
                    string rxid="";
                    entry.Add(new Bundle.EntryComponent()
                    {
                        Resource = MapMedicationRequest.GetFHIRMedicationRequestResource(rx),
                        FullUrl = $"{MedicationRequestUri.Scheme}://{MedicationRequestUri.Authority}{string.Join("", MedicationRequestUri.Segments)}/{rxid}",
                        Search = new Bundle.SearchComponent() { Mode = Bundle.SearchEntryMode.Match }
                    });
                }
            }

            bundle.Timestamp = DateTime.Now;
            bundle.Total = entry.Count;
            bundle.Entry = entry;

            if (FhirServerConfig.ValidatePractitionerBundleAndResource)
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
