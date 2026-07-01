
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace fhir_server_entity_model
{
    public class LegacySystemMapHelper
    {
        [JsonPropertyName("SystemMap")]
        public List<MapEntity> SystemMap { get; set; }

        [JsonPropertyName("HospitalIdentifierUrl")]
        public string HospitalIdentifierUrl { get; set; }
        
        [JsonPropertyName("PractitionerIdentifierUrl")]
        public string PractitionerIdentifierUrl { get; set; }
    }

    public class MapEntity 
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("system")]
        public string System { get; set; }

        [JsonPropertyName("use")]
        public string Use { get; set; }
    }
}
