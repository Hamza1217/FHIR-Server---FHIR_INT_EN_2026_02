using System;
using Xunit;
using fhirserver_dotnet_library;
namespace fhirserver_dotnet_tests
{

    public class L03_1_Tests
    {

        [Fact]
        public void L03_1_T01A_MedicationRequestDirectGetValidate()

        {
            String result = L03_3_testrunner.L03_3_T01A();
            String expected = "All OK";
            Assert.Equal(expected, result);

        }

       [Fact]
        public void L03_1_T02A_MedicationRequestPractitionerDisplay()

        {
            String result = L03_3_testrunner.L03_3_T02A();
            String expected = "Lewis Jerry";
            Assert.Equal(expected, result);

        }
        [Fact]
        public void L03_1_T03A_MessageForOpioids()

        {
            MyConfiguration c=new MyConfiguration();
            String result = L03_3_testrunner.L03_3_T03A();
            String expected = c.MSG_OpioidWarning;
            Assert.Equal(expected, result);

        }
        
    }

}

