using System;
using Xunit;
using fhirserver_dotnet_library;
namespace fhirserver_dotnet_tests
{

    public class L02_4_Tests
    {

/*
Tests for U3-L02_4:Practitioner - Searches by Id & Identifier
U3-L02_4_T01A:Search by identifier / system=NPI - existing
U3-L02_4_T01B:Search by Identifier / but system not NPI
U3-L02_4_T01C:Search by identifier / NPI but does not exist
U3-L02_4_T02A:Search by _id : Existing Practitioner
U3-L02_4_T02B:Search by _id but the person is not a practitioner
U3-L02_4_T02C:Search by _id: non existing id

 { key: "L02_4_T01A", value: "1:0" },
        { key: "L02_4_T01B", value: "HTTP 400 Bad Request: Practitioners can only be found knowing the NPI identifier - You are specifying : PP" },
        { key: "L02_4_T01C", value: "0:0" },
        { key: "L02_4_T02A", value: "1:0" },
        { key: "L02_4_T02B", value: "0:0" },
        { key: "L02_4_T02C", value: "0:0" },

*/
[Fact]
 public void L02_4_T01A_Practitioner_SearchByIdentifier_NPI_Exists()

        {
            String result = L02_4_testrunner.L02_4_T01A();
            String expected = "1:0";
            Assert.Equal(expected, result);

        }
        [Fact]
        public void L02_4_T01B_Practitioner_SearchByIdentifier_NotNPI()

        {
            MyConfiguration c=new MyConfiguration();
            String result = L02_4_testrunner.L02_4_T01B();
            String expected = c.MSG_PractitionerOnlyIdentifierNPI;
            Assert.Contains(expected, result);

        }
        [Fact]
        public void L02_4_T01C_Practitioner_SearchByIdentifierNotFound()

        {
            String result = L02_4_testrunner.L02_4_T01C();
            String expected = "0:0";
            Assert.Equal(expected, result);

        }

        [Fact]
        public void L02_4_T02A_Practitioner_SearchById_Found()

        {
            String result = L02_4_testrunner.L02_4_T02A();
            String expected = "1:0";
            Assert.Equal(expected, result);

        }

        [Fact]
        public void L02_4_T02B_Practitioner_SearchById_NotPractitioner()

        {
            String result = L02_4_testrunner.L02_4_T02B();
            String expected = "0:0";
            Assert.Equal(expected, result);

        }

        [Fact]
        public void L02_4_T02C_Practitioner_SearchById_NotFound()

        {
            String result = L02_4_testrunner.L02_4_T02C();
            String expected = "0:0";
            Assert.Equal(expected, result);

        }


    }
}
