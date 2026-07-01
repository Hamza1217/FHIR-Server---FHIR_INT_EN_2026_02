using System;

namespace fhir_server_entity_model
{
    public class LegacyPerson
    {
		public long PRSN_ID { get; set; }
		public string PRSN_FIRST_NAME { get; set; }
		public string PRSN_SECOND_NAME { get; set; }
		public string PRSN_LAST_NAME { get; set; }
		public string PRSN_BIRTH_DATE { get; set; }
		public string PRSN_GENDER { get; set; }
		public string PRSN_EMAIL { get; set; }
		public string PRSN_NICK_NAME { get; set; }
		public string PRSN_CREATE_DATE { get; set; }
		public string PRSN_UPDATE_DATE { get; set; }
	}
	public class LegacyFilter
	{
		public enum field
		{
			name ,
			family,
			given,
			id,
			_id,
			birthdate,
			email,
			identifier,
			gender,
			subject,
			date

		}
		public field criteria  { get; set; }
		public String value  { get; set; }
	}
}
