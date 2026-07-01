using System;
using Xunit;
using fhirserver_dotnet_library;
namespace fhirserver_dotnet_tests
{

    public class L01_1_Tests
    {
        //L01_1_Tests	Tests for U3-L01_1:Add Support for Searching Patient By Email
        //L01_1_T01	U3-L01_1_T01:Patient - verify email as search parameter in CapabilityStatement
        //L01_1_T02	U3-L01_1_T02:Patient - Search by email - existing
        //L01_1_T03	U3-L01_1_T03:Patient - Search by email - Not Exists

        [Fact]
        public void L01_1_T01_Verify_Email_As_Search_Parameter_In_CapabilityStatement()

        {
            String result=L01_1_testrunner.L01_1_T01();
            String expected = "Token";
            Assert.Equal(expected, result);
            
        }
        [Fact]
        public void L01_1_T02_Search_By_Email_Existing()

        {
            String result=L01_1_testrunner.L01_1_T02();
            String expected = "McEnroe John Patrick";
            Assert.Equal(expected, result);
        
        }

        [Fact]
        public void L01_1_T03_Search_By_Email_Not_Existing()

        {
            String result=L01_1_testrunner.L01_1_T03();
            String expected = "<<NOT EXISTING>>";
            Assert.Equal(expected, result);
        }
    }

}