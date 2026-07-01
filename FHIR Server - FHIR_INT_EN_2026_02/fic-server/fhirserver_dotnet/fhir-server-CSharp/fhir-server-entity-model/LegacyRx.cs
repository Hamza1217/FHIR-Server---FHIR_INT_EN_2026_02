

namespace fhir_server_entity_model
{
    public class LegacyRx
    {
        //{"rxnorm_code":"1049623","rxnorm_display":"Oxycodone Hydrochloride 5mg oral tablet (Roxicodone)","sig":"10 milligrams (2 tablets) every 12 hours","prescription_date":"2022-10-10","patient_id":2,"prescriber_id":28}
		public string rxnorm_code { get; set; }
		public string rxnorm_display { get; set; }
		public string prescription_date { get; set; }
		public string sig { get; set; }
        public long patient_id { get; set; }
        public long prescriber_id { get; set; }
    }    
		
}
