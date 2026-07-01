using System.Collections.Generic;
using System.Linq;
using fhir_server_entity_model;

namespace fhir_server_dataaccess
{
    public static class PractitionerDataAccess
    {
        public static bool HasNPI(long personId)
        {
            return PatientDataAccess.GetPersonDocType(personId)
                .Any(docType => LegacyAPIAccess.getLegacyIdentifierCode(docType.identifier_type_id) == "NPI");
        }

        public static List<LegacyPerson> GetAllPractitioners()
        {
            return PatientDataAccess.GetAllPatients().Where(p => HasNPI(p.PRSN_ID)).ToList();
        }

        public static List<LegacyPerson> GetPractitioner(List<LegacyFilter> criteria)
        {
            return PatientDataAccess.GetPerson(criteria).Where(p => HasNPI(p.PRSN_ID)).ToList();
        }
    }
}
