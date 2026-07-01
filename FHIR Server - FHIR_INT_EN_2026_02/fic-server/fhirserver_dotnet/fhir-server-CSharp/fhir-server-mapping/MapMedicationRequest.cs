using Hl7.Fhir.Model;
using fhir_server_dataaccess;
using Hl7.Fhir.Serialization;
using System.Collections.Generic;
using fhir_server_entity_model;
using System.Text;
using System;

namespace fhir_server_mapping
{
    public static class MapMedicationRequest
    {
        private const string OpioidWarningText = "WARNINGS - Limitations of use - Because of the risks associated with the use of opioids, [Product] should only be used in patients for whom other treatment options, including non-opioid analgesics, are ineffective, not tolerated or otherwise inadequate to provide appropriate management of pain";

        private static string GetPersonRefDisplay(String PatientId)
        {
            string display="";
            List<LegacyFilter> criteria=new List<LegacyFilter>();
            LegacyFilter item=new LegacyFilter();
            item.criteria=LegacyFilter.field._id;
            item.value=PatientId;
            criteria.Add(item);
            List<LegacyPerson> p=PatientDataAccess.GetPerson(criteria) ;
            if (p.Count>0)
            {
                display=p[0].PRSN_LAST_NAME+" "+p[0].PRSN_FIRST_NAME;
                if (p[0].PRSN_SECOND_NAME!=""){ display=display+" "+p[0].PRSN_SECOND_NAME;}
            }
            return display;
        }
        public static MedicationRequest GetFHIRMedicationRequestResource(LegacyRx rx)
        {
                var mr = new MedicationRequest();
                FhirJsonParser parser = new FhirJsonParser(new ParserSettings() { AcceptUnknownMembers = false, AllowUnrecognizedEnums = false });
                string compoundId = rx.patient_id.ToString() + "-"+rx.prescriber_id.ToString()+"-"+rx.prescription_date.ToString().Replace("-","")+"-"+rx.rxnorm_code.ToString();
                mr.Id = Convert.ToBase64String(Encoding.UTF8.GetBytes(compoundId));
                mr.Status = MedicationRequest.MedicationrequestStatus.Active;
                mr.Intent = MedicationRequest.MedicationRequestIntent.Order;
                string PatientDisplay=GetPersonRefDisplay(rx.patient_id.ToString());
                mr.Subject = new ResourceReference()
                {
                    Reference = $"Patient/{rx.patient_id}",
                    Display = $"{PatientDisplay}"
                };
                mr.AuthoredOn = rx.prescription_date;
                CodeableConcept cc=new CodeableConcept("http://www.nlm.nih.gov/research/umls/rxnorm",rx.rxnorm_code,rx.rxnorm_display);
                mr.Medication=cc;
                bool opioid=LegacyAPIAccess.CheckIfOpioid(rx.rxnorm_code);
                string RequesterDisplay=GetPersonRefDisplay(rx.prescriber_id.ToString());
                mr.Requester =new ResourceReference()
                {
                    Reference =$"Practitioner/{rx.prescriber_id}",
                    Display = $"{RequesterDisplay}"
                };
                List<Dosage> ds = new List<Dosage>();

                if (!string.IsNullOrEmpty(rx.sig))
                {
                    Dosage item = new Dosage();
                    item.Text=rx.sig.ToString();
                    ds.Add(item);

                }
                if (opioid)
                {
                    Dosage warning = new Dosage();
                    warning.Text = OpioidWarningText;
                    ds.Add(warning);
                }
                if (ds.Count>0)
                {mr.DosageInstruction = ds;}
                mr.Meta = new Meta()
                {
                    Profile = new List<string>() { "http://hl7.org/fhir/us/core/StructureDefinition/us-core-medicationrequest" },
                };
                string GeneratedText = "Prescription for "+PatientDisplay+ " Date:"+rx.prescription_date+" of "+rx.rxnorm_code+":"+rx.rxnorm_display+" "+rx.sig;
                if (opioid) {GeneratedText=GeneratedText+" (opioid)";}
                mr.Text = new Narrative()
                {
                    Status = Narrative.NarrativeStatus.Generated,
                    Div = $"<div xmlns=\"http://www.w3.org/1999/xhtml\"><p>"+GeneratedText+"</p></div>"
                };

            return mr;
        }
    }
}
