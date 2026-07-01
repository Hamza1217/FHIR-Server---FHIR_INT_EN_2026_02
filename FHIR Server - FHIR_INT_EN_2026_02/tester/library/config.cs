namespace fhirserver_dotnet_library
{
    public  class MyConfiguration
    {

         public string ServerEndpoint{get;}=
            "http://localhost:5834/api/FHIR";
         public string ValidationServerEndpoint{get;}=
            "https://hl7int-server.com/server/fhir";
        
         public string MSG_PractitionerNotFound="HTTP 404 Not Found: Resource Practitioner/1008 is not known";
         public string MSG_PersonNotAPractitioner="HTTP 400 Bad Request: The person you requested is not a practitioner - Lacks a NPI identifier";
         public string MSG_PractitionerOnlyIdentifierNPI="HTTP 400 Bad Request: Practitioners can only be found knowing the NPI identifier - You are specifying : PP";
         public string MSG_PatientTelecomSearchEmailOnly="HTTP 501 Not Implemented: The underlying server only handles email addresses for the patients, thus search by system=phone is not implemented";
         public string MSG_PractitionerTelecomSearchEmailOnly="HTTP 501 Not Implemented: The underlying server only handles email addresses for the practitioners, thus search by system=phone is not implemented";
         public string MSG_OpioidWarning="WARNINGS - Limitations of use - Because of the risks associated with the use of opioids, [Product] should only be used in patients for whom other treatment options, including non-opioid analgesics, are ineffective, not tolerated or otherwise inadequate to provide appropriate management of pain";
        public string StudentId{get;}=
             "kaminker.diego@gmail.com";
        
        public string StudentName{get;}=
            "Diego Kaminker";    
        
    }

}