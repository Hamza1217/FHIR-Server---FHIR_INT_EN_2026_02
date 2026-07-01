
namespace fhir_server_entity_model
{
    public class LegacyPersonIdentifier
    {
		public long prsn_id { get; set; }
		public long identifier_type_id { get; set; }
		public string value { get; set; }
		public string DCTP_ABREV { get; set; }
	}
}
