using System;
namespace fhirserver_dotnet_library
{
    /*
   Tests for 
   Tests for U3-L02_4:Practitioner - Searches by Id & Identifier
    U3-L02_4_T01A:Search by identifier / system=NPI - existing
    U3-L02_4_T01B:Search by Identifier / but system not NPI
    U3-L02_4_T01C:Search by identifier / NPI but does not exist
    U3-L02_4_T02A:Search by _id : Existing Practitioner
    U3-L02_4_T02B:Search by _id but the person is not a practitioner
    U3-L02_4_T02C:Search by _id: non existing id
    
 */
    public static class L02_4_testrunner
    {
        public static string L02_4_T01A()
        {
            
             MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.PractitionerSearch(server,"identifier","http://fhirintermediatecourse.org/NPI|54323");
            return rm;
        
        }
        public static string L02_4_T01B()
        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.PractitionerSearch(server,"identifier","http://fhirintermediatecourse.org/PP|241922");
            return rm;
        
        }
        public static string L02_4_T01C()
        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.PractitionerSearch(server,"identifier","http://fhirintermediatecourse.org/NPI|9999999");
            return rm;
        
        }
        public static string L02_4_T02A()
        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.PractitionerSearch(server,"_id","10");
            return rm;
        

        }
        public static string L02_4_T02B()
        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.PractitionerSearch(server,"_id","1008");
            return rm;
        

        }
        public static string L02_4_T02C()
        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.PractitionerSearch(server,"_id","9");
            return rm;
        
        }
        
    }
}

 