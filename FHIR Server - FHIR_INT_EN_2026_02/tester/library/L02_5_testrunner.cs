using System;
namespace fhirserver_dotnet_library
{
    /*
    Tests for U3-L02_5:Practitioner - Searches by Email and Telecom
    U3-L02_5_T01A:Practitioner - Search by email - existing
    U3-L02_5_T01B:Practitioner / Search by email - not exists
    U3-L02_5_T02A:Practitioner / Search By telecom - phone / not supported
    U3-L02_5_T02B:Practitioner / Search By telecom - email / existing
    U3-L02_5_T02C:Practitioner / Search By telecom - email / not existing
    U3-L02_5_T02D:Practitioner / Search By telecom - w/o system / existing
    
    */
    public static class L02_5_testrunner
    {
        public static string L02_5_T01A()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.PractitionerSearch(server, "email", "john.mcenroe@tennis.com");
            return rm;
        }

        public static string L02_5_T01B()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.PractitionerSearch(server, "email", "john.mcenroe@tennas.com");
            return rm;
        }

        public static string L02_5_T02A()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.PractitionerSearch(server, "telecom", "phone|5555-5555");
            return rm;
        }
        public static string L02_5_T02B()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.PractitionerSearch(server, "telecom", "email|john.mcenroe@tennis.com");
            return rm;

        }
        public static string L02_5_T02C()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.PractitionerSearch(server, "telecom", "email|john.mcenroe@tennas.com");
            return rm;

        }
        public static string L02_5_T02D()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.PractitionerSearch(server, "telecom", "john.mcenroe@tennis.com");
            return rm;

        }
    }
}