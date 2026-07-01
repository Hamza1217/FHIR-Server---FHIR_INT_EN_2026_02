using System;
using fhir_server_entity_model;
using System.Collections.Generic;
using System.Linq;

namespace fhir_server_dataaccess
{
    public static class PatientDataAccess
    {
        
        public static  List<LegacyPerson> GetAllPatients(string SpecificPatientId = null)
        {
            List<LegacyPerson> people = new List<LegacyPerson>();
            LegacyAPIAccess.GetLegacyData();
            var lp=LegacyAPIAccess.LegacyPerson;
            if (lp!=null)
            {
                int max=lp.Length;
                for (int i = 0; i < max; i++)
                {
                    LegacyPerson item=lp[i];
                    people.Add(item);
                }
            }
            return people;
        }
        public static List<LegacyPersonIdentifier> GetPersonDocType(long personId)
        {
            LegacyAPIAccess.GetLegacyData();
           
            List<LegacyPersonIdentifier> docTypes = new List<LegacyPersonIdentifier>();
            var lpi=LegacyAPIAccess.LegacyPersonIdentifiers;
            if (lpi!=null)
            {
                int max=lpi.Length;
                
                for (int i = 0; i < max; i++)
                {
                    if (lpi[i].prsn_id==personId)
                    {docTypes.Add(lpi[i]);}
                }
            }
            return docTypes;
        }
        public static List<LegacyPerson> GetPerson(List<LegacyFilter> Criteria) 
        {
            LegacyAPIAccess.GetLegacyData();
            List<LegacyPerson> people = new List<LegacyPerson>();
            
            var lp=LegacyAPIAccess.LegacyPerson;
            int max=lp.Length;
            for (int i = 0; i < max; i++)
            {
                LegacyPerson item=lp[i];
                bool include=true;
                foreach ( LegacyFilter c in Criteria)
                {
                    switch  (c.criteria)
                    {
                        case LegacyFilter.field.name:
                            {
                                String fullname=item.PRSN_FIRST_NAME.ToLower()+" "+item.PRSN_LAST_NAME.ToLower();
                                if (!(fullname.Contains(c.value.ToLower())))
                                {
                                    include=false;
                                }
                                break;
                            }
                        case LegacyFilter.field.family:
                            if (!(item.PRSN_LAST_NAME.ToLower().Contains(c.value.ToLower())))
                            {
                              include=false;      
                            }
                            break;
                        case LegacyFilter.field.given:
                            if (!(item.PRSN_FIRST_NAME.ToLower().Contains(c.value.ToLower())))
                            {
                              include=false;      
                            }
                            break;
                        case LegacyFilter.field.id:
                            if (item.PRSN_ID.ToString()!=c.value)
                            {
                                include=false;
                            }
                            break;
                        case LegacyFilter.field._id:
                            
                            if (item.PRSN_ID.ToString()!=c.value)
                            
                            {
                                include=false;
                            }
                            break;
                        case LegacyFilter.field.birthdate:
                            String which_date=c.value;
                            
                            if (!(item.PRSN_BIRTH_DATE.ToString()==which_date))
                            {
                                include=false;
                            }
                            break;
                        case LegacyFilter.field.identifier:
                            String search_ident=c.value;
                            List<fhir_server_entity_model.LegacyPersonIdentifier> personIdentifiers = fhir_server_dataaccess.PatientDataAccess.GetPersonDocType(item.PRSN_ID);
                            bool ident_found=false;
                            foreach (var docType in personIdentifiers)
                            {
    
                                var the_System = "http://fhirintermediatecourse.org/"+fhir_server_dataaccess.LegacyAPIAccess.getLegacyIdentifierCode(docType.identifier_type_id);
                                var the_Value = docType.value;
                                String the_ident=the_System+"|"+the_Value;
                                Console.WriteLine(the_ident);
                                Console.WriteLine(search_ident);
                                
                                if (the_ident==search_ident)
                                {
                                    ident_found=true;
                                    break;
                                }
                            }
                            if (!ident_found){include=false;}
                            break;
                        case LegacyFilter.field.gender:
                            if (item.PRSN_GENDER.ToLower()!=c.value.ToLower())
                            {
                                include=false;
                            }
                            break;
                    default:
                        break;
                    }
                    if (!include){break;}

                }
            
                if (include) people.Add(item);
            }
            
            return people;
        }
       
    }
}

