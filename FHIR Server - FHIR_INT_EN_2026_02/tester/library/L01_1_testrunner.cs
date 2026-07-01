using System;
namespace fhirserver_dotnet_library
{

    public static class L01_1_testrunner
    {
        public static string L01_1_T01()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.CapabilityCheckParameterType(server, "Patient", "email");
            return rm;
        }

        public static string L01_1_T02()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String mail = "john.mcenroe@tennis.com";
            String name = testhelper.PatientSearchByEmail(server, mail);
            return name;
        }

        public static string L01_1_T03()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String mail = "john.mcenroe@tennas.com";
            String name = testhelper.PatientSearchByEmail(server, mail);
            return name;
        }
    }
}