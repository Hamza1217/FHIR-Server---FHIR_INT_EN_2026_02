using System;
namespace fhirserver_dotnet_library
{
    /*
   Tests for 
     Tests for U3-L02_3:Practitioner - Searches by Name & Gender
        U3-L02_3_T01:Get All Practitioners - 5 found w/NPI
        U3-L02_3_T02A:Search All Practitioners by Family Name Lennon- None
        U3-L02_3_T02B:Search All Practitioners by Family Name = McEnroe- 1 match with NPI
        U3-L02_3_T03A:Search All Practitioners by Name = Lennon- None
        U3-L02_3_T03B:Search All Practitioners by Name = John- 3 match with NPI
        U3-L02_3_T04A:Search All male Practitioners : 5
        U3-L02_3_T04B:Search All female Practitioners : 0
       
 */
    public static class L02_3_testrunner
    {
        public static string L02_3_T01()
        {
            
             MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.GetPractitionersAll(server);
            return rm;
        
        }
        public static string L02_3_T02A()
        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.PractitionerSearch(server,"family","Lennon");
            return rm;
        
        }
        public static string L02_3_T02B()
        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.PractitionerSearch(server,"family","McEnroe");
            return rm;
        
        }
        public static string L02_3_T03A()
        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.PractitionerSearch(server,"name","Lennon");
            return rm;
        

        }
        public static string L02_3_T03B()
        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.PractitionerSearch(server,"name","John");
            return rm;
        

        }
        public static string L02_3_T04A()
        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.PractitionerSearch(server,"gender","male");
            return rm;
        
        }
        public static string L02_3_T04B()
        {
            MyConfiguration c = new MyConfiguration();
            var server = c.ServerEndpoint;
            String rm = testhelper.PractitionerSearch(server,"gender","female");
            return rm;
        
        }
    }
}

 