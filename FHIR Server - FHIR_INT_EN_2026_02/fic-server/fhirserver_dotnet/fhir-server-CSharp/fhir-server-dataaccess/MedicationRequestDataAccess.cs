using System;
using fhir_server_entity_model;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fhir_server_dataaccess
{
    public static class MedicationRequestDataAccess
    {
        
        public static  List<LegacyRx> GetAllMedicationRequests(string SpecificRxId = null)
        {
            List<LegacyRx> rxs = new List<LegacyRx>();
            LegacyAPIAccess.GetLegacyData();
            var lr=LegacyAPIAccess.LegacyRxs;
            if (lr!=null)
            {
                int max=lr.Length;
                for (int i = 0; i < max; i++)
                {
                    LegacyRx item=lr[i];
                    rxs.Add(item);
                }
            }
            return rxs;
        }
        public static List<LegacyRx> GetMedicationRequest(List<LegacyFilter> Criteria) 
        {
            LegacyAPIAccess.GetLegacyData();
            List<LegacyRx> rxs = new List<LegacyRx>();
            string compoundId="";
            string thisId="";
                
            var lp=LegacyAPIAccess.LegacyRxs;
            int max=lp.Length;
            for (int i = 0; i < max; i++)
            {
                LegacyRx item=lp[i];
                bool include=true;
                foreach ( LegacyFilter c in Criteria)
                {
                    Console.WriteLine(c.criteria.ToString());
                    switch  (c.criteria)
                    {
                        case LegacyFilter.field.id:
                            compoundId = item.patient_id.ToString() + "-"+item.prescriber_id.ToString()+"-"+item.prescription_date.ToString().Replace("-","")+"-"+item.rxnorm_code.ToString();
                            thisId = Convert.ToBase64String(Encoding.UTF8.GetBytes(compoundId));
                            Console.WriteLine(thisId);
                            Console.WriteLine(c.value);
                            if (c.value!=thisId)    
                            {
                                include=false;
                            }
                            break;
                        case LegacyFilter.field._id:
                            
                            compoundId = item.patient_id.ToString() + "-"+item.prescriber_id.ToString()+"-"+item.prescription_date.ToString()+"-"+item.rxnorm_code.ToString();
                            thisId = Convert.ToBase64String(Encoding.UTF8.GetBytes(compoundId));
                            if (c.value!=thisId)    
                            {
                                include=false;
                            }
                            break;
                        case LegacyFilter.field.subject:
                            string patientId="Patient/"+item.patient_id.ToString();
                            if (c.value!=patientId)
                            {
                                include=false;
                            }
                            break;
                    default:
                        break;
                    }
                    if (!include){break;}

                }
            
                if (include) rxs.Add(item);
            }
            
            return rxs;
        }
       
    }
}

