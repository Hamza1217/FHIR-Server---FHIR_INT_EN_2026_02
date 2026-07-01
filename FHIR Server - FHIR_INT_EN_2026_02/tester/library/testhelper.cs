using System;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
namespace fhirserver_dotnet_library
{

    public static class testhelper
    {
        public static string MedicationRequestFindRequesterDisplay(string server, string id)
        {
            string result = "<<notfound>>";
            MedicationRequest mr = MedicationRequestGet(server, id);
            if (mr.Requester != null)
            {
                if (mr.Requester.Display != null)
                {
                    result = mr.Requester.Display;
                }
            }
            return result;
        }
        public static string MedicationRequestFindWarningSIG(string server, string id)
        {
            string result = "<<warningnotfound>>";
            MedicationRequest mr = MedicationRequestGet(server, id);
            if (mr.DosageInstruction.Count > 0)
            {
                if (mr.DosageInstruction[0].Text != "")
                {
                    result =  mr.DosageInstruction[0].Text;

                }
                if (mr.DosageInstruction.Count > 1)
                {
                    if (mr.DosageInstruction[1].Text != "")
                    {
                        result =  mr.DosageInstruction[1].Text;

                    }
                }
            }
            return result;
        }

        public static string MedicationRequestGetAndValidate(string server, string id, string ValidationServer)
        {
            string result = "";
            MedicationRequest mr = MedicationRequestGet(server, id);
            Hl7.Fhir.Serialization.FhirJsonSerializer fjs = new FhirJsonSerializer();
            String MyMedicationRequest = fjs.SerializeToString(mr);

            if (MyMedicationRequest != "")
            {
                result = MedicationRequestValidate_USCORE(MyMedicationRequest, ValidationServer);
            }
            return result;
        }
        public static string MedicationRequestValidate_USCORE(string JsonMedicationRequest, string server)
        {
            string aux = "";
            Hl7.Fhir.Model.MedicationRequest o = new Hl7.Fhir.Model.MedicationRequest();
            var parser = new Hl7.Fhir.Serialization.FhirJsonParser();

            try
            {
                o = parser.Parse<MedicationRequest>(JsonMedicationRequest);
            }
            catch
            {
                aux = "Error:Invalid_MedicationRequest_Resource";
            }

            if (aux == "")
            {
                var client = new Hl7.Fhir.Rest.FhirClient(server);
                Hl7.Fhir.Model.FhirUri profile = new FhirUri("http://hl7.org/fhir/us/core/StructureDefinition/us-core-medicationrequest");

                Parameters inParams = new Parameters();
                inParams.Add("resource", o);
                OperationOutcome bu = client.ValidateResource(o);
                if (bu.Issue[0].Details.Text != "All OK")
                {
                    aux = "Error:" + bu.Issue[0].Details.Text;
                }
                else
                {
                    aux="All OK";
                }
            }
            return aux;

        }


        public static MedicationRequest MedicationRequestGet(String serverEndpoint, String Id)
        {

            MedicationRequest result;

            var client = new FhirClient(serverEndpoint);
            try
            {
                MedicationRequest mr = client.Read<MedicationRequest>(Id);
                result = (MedicationRequest)mr;
            }
            catch (System.Exception e)
            {

                throw (e);
            }

            return result;

        }
        public static String GetPractitionersAll(String ServerEndpoint)
        {
            string result = PractitionerSearch(ServerEndpoint, "", "");
            return result;
        }

        public static String PractitionerSearch(String ServerEndpoint, string SearchParameter, string Value)
        {
            string result = "";
            var client = new FhirClient(ServerEndpoint);
            Bundle bu;
            try
            {
                if (SearchParameter != "")
                {
                    var q = new SearchParams(SearchParameter, Value);
                    bu = client.Search<Practitioner>(q);
                }
                else
                { bu = client.Search<Practitioner>(); }

                int totP = 0;
                int totNP = 0;
                foreach (Bundle.EntryComponent ent in bu.Entry)
                {
                    Practitioner pa = (Practitioner)ent.Resource;

                    totP = totP + 1;
                }
                result = totP.ToString() + ":" + totNP.ToString();
            }
            catch (System.Exception e)
            {

                result = e.Message;
            }

            return result;
        }
        public static String PractitionerGetById(String ServerEndpoint, string Id)
        {

            String result = "<<NOT EXISTING>>";

            var client = new FhirClient(ServerEndpoint);
            try
            {
                Practitioner pa = client.Read<Practitioner>(Id);
                String MyPractitioner = pa.Name[0].Family + " " + pa.Name[0].GivenElement[0].Value.ToString();
                if (pa.Name[0].GivenElement.Count > 1)
                {
                    MyPractitioner = MyPractitioner + " " + pa.Name[0].GivenElement[1].Value.ToString();
                }
                result = MyPractitioner;
            }
            catch (System.Exception e)
            {

                result = e.Message;
            }

            return result;

        }
        public static String CapabilityCheckInteraction(String ServerEndpoint, String resource, String interaction)
        {
            String result = "";

            CapabilityStatement cs = GetCapabilityStatement(ServerEndpoint);
            CapabilityStatement.RestComponent csr = cs.Rest[0];
            Console.WriteLine(csr.Resource.Count);

            int rc = csr.Resource.Count;
            for (int i = 0; i < rc; i++)
            {
                CapabilityStatement.ResourceComponent csrc = csr.Resource[i];
                if (csrc.Type.ToString() == resource)
                {


                    int intc = csrc.Interaction.Count;
                    Console.WriteLine("Intercount");
                    Console.WriteLine(intc);
                    for (int j = 0; j < intc; j++)
                    {

                        CapabilityStatement.ResourceInteractionComponent cinc = csrc.Interaction[j];
                        Console.WriteLine("code");

                        Console.WriteLine(cinc.Code.ToString());

                        if (cinc.Code.ToString() == interaction)
                        {
                            result = interaction;
                            break;
                        }
                    }
                    break;

                }
            }
            return result;
        }
        public static String CapabilityCheckParameterType(String ServerEndpoint, String resource, String name)
        {
            String result = "notsupported";
            CapabilityStatement cs = GetCapabilityStatement(ServerEndpoint);
            CapabilityStatement.RestComponent csr = cs.Rest[0];
            Console.WriteLine(csr.Resource.Count);

            int rc = csr.Resource.Count;
            for (int i = 0; i < rc; i++)
            {
                CapabilityStatement.ResourceComponent csrc = csr.Resource[i];
                if (csrc.Type.ToString() == resource)
                {

                    int spc = csrc.SearchParam.Count;
                    for (int j = 0; j < spc; j++)
                    {

                        CapabilityStatement.SearchParamComponent csrp = csrc.SearchParam[j];
                        if (csrp.Name.ToString() == name)
                        {
                            result = csrp.Type.ToString();
                            break;
                        }
                    }
                    break;

                }

            }
            return result;
        }
        private static CapabilityStatement GetCapabilityStatement(String ServerEndPoint)
        {
            var client = new FhirClient(ServerEndPoint);
            CapabilityStatement cs = client.CapabilityStatement();
            return cs;
        }
        public static String PatientSearchByTelecom(String ServerEndpoint, String telecom)
        {
            String result = "<<NOT EXISTING>>";

            var client = new FhirClient(ServerEndpoint);
            try
            {
                var q = new SearchParams("telecom", telecom);
                Bundle bu = client.Search<Patient>(q);
                foreach (Bundle.EntryComponent ent in bu.Entry)
                {
                    Patient pa = (Patient)ent.Resource;
                    String MyPatient = pa.Name[0].Family + " " + pa.Name[0].GivenElement[0].Value.ToString();
                    if (pa.Name[0].GivenElement.Count > 1)
                    {
                        MyPatient = MyPatient + " " + pa.Name[0].GivenElement[1].Value.ToString();
                    }
                    result = MyPatient;
                }

            }
            catch (System.Exception e)
            {

                result = e.Message;
            }

            return result;
        }
        public static String PatientSearchByEmail(String ServerEndpoint, String email)
        {
            String result = "<<NOT EXISTING>>";

            var client = new FhirClient(ServerEndpoint);

            var q = new SearchParams("email", email);
            Bundle bu = client.Search<Patient>(q);

            foreach (Bundle.EntryComponent ent in bu.Entry)
            {
                Patient pa = (Patient)ent.Resource;
                String MyPatient = pa.Name[0].Family + " " + pa.Name[0].GivenElement[0].Value.ToString();
                if (pa.Name[0].GivenElement.Count > 1)
                {
                    MyPatient = MyPatient + " " + pa.Name[0].GivenElement[1].Value.ToString();
                }
                result = MyPatient;
            }
            return result;
        }


    }
}
