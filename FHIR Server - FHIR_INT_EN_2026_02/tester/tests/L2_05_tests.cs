using System;
using Xunit;
using fhirserver_dotnet_library;
namespace fhirserver_dotnet_tests
{

    public class L02_5_Tests
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
[Fact]
 public void L02_5_T01A_Practitioner_SearchByEmail_Exists()

        {
            String result = L02_5_testrunner.L02_5_T01A();
            String expected = "1:0";
            Assert.Equal(expected, result);

        }
[Fact]
 public void L02_5_T01B_Practitioner_SearchByEmail_NotExists()

        {
            String result = L02_5_testrunner.L02_5_T01B();
            String expected = "0:0";
            Assert.Equal(expected, result);

        }
[Fact]
 public void L02_5_T02A_Practitioner_SearchByTelecom_Phone_Error()

        {
            MyConfiguration c=new MyConfiguration();
            String result = L02_5_testrunner.L02_5_T02A();
            String expected = c.MSG_PractitionerTelecomSearchEmailOnly;
            Assert.Contains(expected, result);

        }
[Fact]
 public void L02_5_T02B_Practitioner_SearchByTelecom_Email_Exists()

        {
            String result = L02_5_testrunner.L02_5_T02B();
            String expected = "1:0";
            Assert.Equal(expected, result);

        }
[Fact]
 public void L02_5_T02C_Practitioner_SearchByTelecom_Email_NotExists()

        {
            String result = L02_5_testrunner.L02_5_T02C();
            String expected = "0:0";
            Assert.Equal(expected, result);

        }
[Fact]
 public void L02_5_T02D_Practitioner_SearchByTelecom_NoSystem_Exists()

        {
            String result = L02_5_testrunner.L02_5_T02D();
            String expected = "1:0";
            Assert.Equal(expected, result);

        }

    }
}
