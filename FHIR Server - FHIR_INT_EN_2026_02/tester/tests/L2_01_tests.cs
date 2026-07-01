using System;
using Xunit;
using fhirserver_dotnet_library;
namespace fhirserver_dotnet_tests
{

    public class L02_1_Tests
    {
/*
U3-L02_1:Practitioner - Verify Interactions and All Search Parameters
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

        [Fact]
        public void L02_1_T01_Verify_Practitioner_Search_Parameter_name()

        {
            String result=L02_1_testrunner.L02_1_T01();
            String expected = "String";
            Assert.Equal(expected, result);
            
        }
      [Fact]
      public void L02_1_T02_Verify_Practitioner_Search_Parameter_family()

        {
            String result=L02_1_testrunner.L02_1_T02();
            String expected = "String";
            Assert.Equal(expected, result);
            
        }
        [Fact]
      public void L02_1_T03_Verify_Practitioner_Search_Parameter_given()

        {
            String result=L02_1_testrunner.L02_1_T03();
            String expected = "String";
            Assert.Equal(expected, result);
            
        }
        [Fact]
      public void L02_1_T04_Verify_Practitioner_Search_Parameter__id()

        {
            String result=L02_1_testrunner.L02_1_T04();
            String expected = "Token";
            Assert.Equal(expected, result);
            
        }
         [Fact]
      public void L02_1_T05_Verify_Practitioner_Search_Parameter_email()

        {
            String result=L02_1_testrunner.L02_1_T05();
            String expected = "Token";
            Assert.Equal(expected, result);
            
        }
         [Fact]
      public void L02_1_T06_Verify_Practitioner_Search_Parameter_telecom()

        {
            String result=L02_1_testrunner.L02_1_T06();
            String expected = "Token";
            Assert.Equal(expected, result);
            
        }

         [Fact]
      public void L02_1_T07_Verify_Practitioner_Search_Parameter_identifier()

        {
            String result=L02_1_testrunner.L02_1_T07();
            String expected = "Token";
            Assert.Equal(expected, result);
            
        }

[Fact]
    public void L02_1_T08_Verify_Practitioner_Search_Parameter_birthdate()

        {
            String result=L02_1_testrunner.L02_1_T08();
            String expected = "notsupported";
            Assert.Equal(expected, result);
            
        }
        [Fact]
        public void L02_1_T09_Verify_Practitioner_Interaction_read()

        {
            String result=L02_1_testrunner.L02_1_T09();
            String expected = "Read";
            Assert.Equal(expected, result);
            
        }
        [Fact]
        public void L02_1_T10_Verify_Practitioner_Interaction_searchtype()

        {
            String result=L02_1_testrunner.L02_1_T10();
            String expected = "SearchType";
            Assert.Equal(expected, result);
            
        }

}
}

