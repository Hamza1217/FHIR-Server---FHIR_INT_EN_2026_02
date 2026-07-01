using System.Collections.Generic;
using Hl7.Fhir.Model;

namespace fhir_server_mapping
{
    public static class MapPractitioner
    {
        public static Practitioner GetFHIRPractitionerResource(fhir_server_entity_model.LegacyPerson person)
        {
            List<Identifier> identifiers = new List<Identifier>();
            List<fhir_server_entity_model.LegacyPersonIdentifier> personIdentifiers = fhir_server_dataaccess.PatientDataAccess.GetPersonDocType(person.PRSN_ID);

            foreach (var docType in personIdentifiers)
            {
                identifiers.Add(new Identifier()
                {
                    System = "http://fhirintermediatecourse.org/" + fhir_server_dataaccess.LegacyAPIAccess.getLegacyIdentifierCode(docType.identifier_type_id),
                    Value = docType.value,
                    Period = new Period() { Start = person.PRSN_CREATE_DATE }
                });
            }

            Practitioner p = new Practitioner()
            {
                Id = person.PRSN_ID.ToString(),
                Meta = new Meta()
                {
                    Profile = new List<string>() { "http://hl7.org/fhir/us/core/StructureDefinition/us-core-practitioner" }
                },
                Text = new Narrative()
                {
                    Status = Narrative.NarrativeStatus.Generated,
                    Div = $"<div xmlns=\"http://www.w3.org/1999/xhtml\"><p><b>Generated Narrative</b></p><p>{person.PRSN_FIRST_NAME} {person.PRSN_SECOND_NAME} {person.PRSN_LAST_NAME}</p></div>"
                },
                Identifier = identifiers,
                Name = new List<HumanName>()
                {
                    new HumanName()
                    {
                        Use = HumanName.NameUse.Official,
                        Family = person.PRSN_LAST_NAME,
                        Given = new List<string>() { person.PRSN_FIRST_NAME },
                        Text = $"{(string.IsNullOrEmpty(person.PRSN_FIRST_NAME) ? "" : person.PRSN_FIRST_NAME)} {(string.IsNullOrEmpty(person.PRSN_LAST_NAME) ? "" : person.PRSN_LAST_NAME)}"
                    }
                },
                Telecom = new List<ContactPoint>()
                {
                    new ContactPoint()
                    {
                        System = ContactPoint.ContactPointSystem.Email,
                        Value = person.PRSN_EMAIL
                    }
                },
                Gender = getGender(person.PRSN_GENDER)
            };

            if (!string.IsNullOrEmpty(person.PRSN_SECOND_NAME))
            {
                List<string> gvnNames = new List<string>();
                foreach (var item in p.Name[0].Given)
                {
                    gvnNames.Add(item);
                }
                gvnNames.Add(person.PRSN_SECOND_NAME);

                p.Name[0].Given = gvnNames;
                p.Name[0].Text = $"{(string.IsNullOrEmpty(person.PRSN_FIRST_NAME) ? "" : person.PRSN_FIRST_NAME)} {(string.IsNullOrEmpty(person.PRSN_SECOND_NAME) ? "" : person.PRSN_SECOND_NAME)} {(string.IsNullOrEmpty(person.PRSN_LAST_NAME) ? "" : person.PRSN_LAST_NAME)}";
            }

            if (!string.IsNullOrEmpty(person.PRSN_NICK_NAME))
            {
                HumanName nickName = new HumanName()
                {
                    Use = HumanName.NameUse.Nickname,
                    GivenElement = new List<FhirString>() { new FhirString() { Value = person.PRSN_NICK_NAME } }
                };
                p.Name.Add(nickName);
            }

            AdministrativeGender getGender(string gender)
            {
                AdministrativeGender adGender = AdministrativeGender.Unknown;

                switch (gender.Trim().ToLower())
                {
                    case "0":
                        return adGender;
                    case "male":
                        adGender = AdministrativeGender.Male;
                        goto case "0";
                    case "female":
                        adGender = AdministrativeGender.Female;
                        goto case "0";
                    case "other":
                        adGender = AdministrativeGender.Other;
                        goto case "0";
                    case "unknown":
                        adGender = AdministrativeGender.Unknown;
                        goto case "0";
                }

                return adGender;
            }

            return p;
        }
    }
}
