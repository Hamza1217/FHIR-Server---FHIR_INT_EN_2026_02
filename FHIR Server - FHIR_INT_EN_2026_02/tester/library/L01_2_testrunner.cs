using System;
namespace fhirserver_dotnet_library
{

    public static class L01_2_testrunner
    {
        public static string L01_2_T01()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.CapabilityCheckParameterType(server, "Patient", "telecom");
            return rm;
        }

        public static string L01_2_T02()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String mail = "email|john.mcenroe@tennis.com";
            String name = testhelper.PatientSearchByTelecom(server, mail);
            return name;
        }

        public static string L01_2_T03()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String mail = "email|john.mcenroe@tennas.com";
            String name = testhelper.PatientSearchByTelecom(server, mail);
            return name;
        }
        public static string L01_2_T04()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String mail = "phone|5555-5555";
            String name = testhelper.PatientSearchByTelecom(server, mail);
            return name;
        }
        public static string L01_2_T05()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String mail = "dorothea.lange@photographer.com";
            String name = testhelper.PatientSearchByTelecom(server, mail);
            return name;
        }
    }
}