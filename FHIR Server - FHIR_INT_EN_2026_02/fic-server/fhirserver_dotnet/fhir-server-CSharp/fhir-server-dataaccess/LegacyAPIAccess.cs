using System;
using fhir_server_entity_model;
using System.Net.Http;
using Newtonsoft.Json;
namespace fhir_server_dataaccess
{
 public static class LegacyAPIAccess
    {
        
        public static LegacyPerson[] LegacyPerson=null;
        public static LegacyPersonIdentifier[] LegacyPersonIdentifiers=null;
        public static LegacyIdentifierType[] LegacyIdentifierTypes=null;
        public static LegacyRx[] LegacyRxs=null;
        public static LegacyMed[] LegacyMeds=null;
        public static void GetLegacyData()
        {
            GetLegacyPersons();
            GetLegacyPersonIdes();
            GetLegacyIdentifierTypes();
            GetLegacyRxs();
            GetLegacyMeds();
        }
        private static void GetLegacyMeds()
        {
            if (LegacyMeds==null)
            {
                string url = "http://3.221.164.25:9080/meds";
                HttpClient client = new HttpClient();
                string response = client.GetStringAsync(url).Result;
                LegacyMeds= JsonConvert.DeserializeObject<LegacyMed[]>(response);
            }

        }
        
        private static void GetLegacyRxs()
        {
            if (LegacyRxs==null)
            {
                string url = "http://3.221.164.25:9080/rx";
                HttpClient client = new HttpClient();
                string response = client.GetStringAsync(url).Result;
                LegacyRxs= JsonConvert.DeserializeObject<LegacyRx[]>(response);
            }

        }
        public static bool CheckIfOpioid(String rxnorm_code)
        {
            GetLegacyData();
            bool opioid=false;
            LegacyMed[] ms=LegacyMeds; 
            for (int i = 0; i < ms.Length; i++)
            {
                LegacyMed m = ms[i];
                if (m.code==rxnorm_code)
                {
                    if (m.opioid=="yes")
                    {
                        opioid=true;
                    }
                    break;
                }
            }
            return opioid;
        }
        private static void GetLegacyIdentifierTypes()
        {
            if (LegacyIdentifierTypes==null)
            {
                string url = "http://3.221.164.25:9080/identifier_type";
                HttpClient client = new HttpClient();
                string response = client.GetStringAsync(url).Result;
                LegacyIdentifierTypes = JsonConvert.DeserializeObject<LegacyIdentifierType[]>(response);
            }
        }
        public static String getLegacyIdentifierCode(long IdentifierTypeId)
        {
            String Code ="";
            GetLegacyData();
            if (LegacyIdentifierTypes!=null)
            {
                int max=LegacyIdentifierTypes.Length;
                for (int i = 0; i < max; i++)
                {
                    if (LegacyIdentifierTypes[i].identifier_type_id==IdentifierTypeId)
                    {
                        Code=LegacyIdentifierTypes[i].identifier_code;
                        break;
                    }

                }
            }
            return Code;
        
        }
        private static void GetLegacyPersonIdes()
        {
            if (LegacyPersonIdentifiers==null)
            {
                string url = "http://3.221.164.25:9080/person_identifier";
                HttpClient client = new HttpClient();
                string response = client.GetStringAsync(url).Result;
                LegacyPersonIdentifiers = JsonConvert.DeserializeObject<LegacyPersonIdentifier[]>(response);
            }
        }        
        private static void GetLegacyPersons ()
        {
            if (LegacyPerson==null)
            {
                string url = "http://3.221.164.25:9080/person";
                HttpClient client = new HttpClient();
                string response = client.GetStringAsync(url).Result;
                LegacyPerson = JsonConvert.DeserializeObject<LegacyPerson[]>(response);
            }
        }
     
    }
}
