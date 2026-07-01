using System;
namespace fhirserver_dotnet_library
{
    /*
    U3-L02_1_T01:metadata: Verify name as Search Param. for Practitioner
    U3-L02_1_T02:metadata: Verify family as Search Param. for Practitioner
    U3-L02_1_T03:metadata: Verify given as search parameter for Practitioner
    U3-L02_1_T04:metadata: Verify _id as search parameter for Practitioner
    U3-L02_1_T05:metadata: Verify email as search parameter for Practitioner
    U3-L02_1_T06:metadata: Verify telecom as search parameter for Practitioner
    U3-L02_1_T07:metadata: Verify identifier as search parameter for Practitioner
    U3-L02_1_T08:metadata: Verify birthdate is NOT a search parameter for Practitioner
    U3-L02_1_T09:metadata: Verify Practitioner/read as Interaction
    U3-L02_1_T10:metadata: Verify Practitioner/search-type as Interaction
    */
    public static class L02_1_testrunner
    {
        public static string L02_1_T01()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.CapabilityCheckParameterType(server, "Practitioner", "name");
            return rm;
        }

        public static string L02_1_T02()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.CapabilityCheckParameterType(server, "Practitioner", "family");
            return rm;
        }

        public static string L02_1_T03()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.CapabilityCheckParameterType(server, "Practitioner", "given");
            return rm;
        }
        public static string L02_1_T04()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.CapabilityCheckParameterType(server, "Practitioner", "_id");
            return rm;

        }
        public static string L02_1_T05()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.CapabilityCheckParameterType(server, "Practitioner", "email");
            return rm;

        }
        public static string L02_1_T06()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.CapabilityCheckParameterType(server, "Practitioner", "telecom");
            return rm;

        }
        public static string L02_1_T07()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.CapabilityCheckParameterType(server, "Practitioner", "identifier");
            return rm;

        }
        public static string L02_1_T08()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.CapabilityCheckParameterType(server, "Practitioner", "birthdate");
            return rm;

        }
        public static string L02_1_T09()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.CapabilityCheckInteraction(server, "Practitioner", "Read");
            return rm;

        }
        public static string L02_1_T10()

        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.CapabilityCheckInteraction(server, "Practitioner", "SearchType");
            return rm;

        }
    }
}