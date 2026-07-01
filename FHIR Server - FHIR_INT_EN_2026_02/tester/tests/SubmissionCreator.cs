using System;
using System.Collections.Generic;
using fhirserver_dotnet_library;
using Hl7.Fhir.Model;
using Xunit;
namespace fhirserver_dotnet_tests
{

public class SubmissionCreator
    {
        [Fact]
        public void CreateSubmission()
        {
            MyConfiguration c=new MyConfiguration();
            Dictionary<string,string> TestList=new System.Collections.Generic.Dictionary<string, string>();
            TestList=AddAll (TestList,L01_1_Tests_Results());
            TestList=AddAll (TestList,L01_2_Tests_Results());
            TestList=AddAll (TestList,L02_1_Tests_Results());
            TestList=AddAll (TestList,L02_2_Tests_Results());
            TestList=AddAll (TestList,L02_3_Tests_Results());
            TestList=AddAll (TestList,L02_4_Tests_Results());
            TestList=AddAll (TestList,L02_5_Tests_Results());
            TestList=AddAll (TestList,L03_1_Tests_Results());
          
            
            Hl7.Fhir.Model.TestReport  tr=new  Hl7.Fhir.Model.TestReport();
            tr.Result=Hl7.Fhir.Model.TestReport.TestReportResult.Pending;
            String datee=DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            datee=datee.Replace(":","");
            
            tr.Identifier=new Identifier("http://fhirintermediate.org/test_report/id",c.StudentId+"-"+datee);
            tr.Issued=datee;
            tr.Status=TestReport.TestReportStatus.InProgress;
            ResourceReference r=new ResourceReference();
            r.Identifier=new Identifier("http://fhirintermediate.org/test_script/id","FHIR_INTERMEDIATE_U03-.NET");
            tr.TestScript=r;
            tr.Tester=c.StudentId;

            TestReport.ParticipantComponent pcS=new TestReport.ParticipantComponent();
            pcS.Type=TestReport.TestReportParticipantType.Server;
            pcS.Uri=c.ServerEndpoint;
            pcS.Display="Resource Server";
            tr.Participant.Add(pcS);
            
            
            TestReport.ParticipantComponent pcC=new TestReport.ParticipantComponent();
            pcC.Type=TestReport.TestReportParticipantType.Client;
            pcC.Uri="http://localhost";
            pcC.Display=c.StudentName;
            tr.Participant.Add(pcC);

            foreach(KeyValuePair<string,string> test in TestList)
            {
                
                TestReport.TestComponent testComponent=new TestReport.TestComponent();
                testComponent.Name=test.Key;
                
                testComponent.Description=test.Key;
                TestReport.TestActionComponent ta=new TestReport.TestActionComponent();
                TestReport.AssertComponent tac=new TestReport.AssertComponent();
                if (test.Value=="")
                {
                    tac.Result=TestReport.TestReportActionResult.Fail;
                    tac.Message=new Markdown("Not Attempted");
                }
                else
                {
                    tac.Result=TestReport.TestReportActionResult.Pass;
                    tac.Message=new Markdown(test.Value);
                    
                }
                ta.Assert=tac;
                testComponent.Action.Add(ta);
                tr.Test.Add(testComponent);
            }
            
            
            Hl7.Fhir.Serialization.FhirJsonSerializer s = new Hl7.Fhir.Serialization.FhirJsonSerializer();  
            String results = s.SerializeToString(tr);  
            String filename=@"FHIR_INTERMEDIATE_U3_SUBMISSION_" + c.StudentId+"_"+datee+".JSON";
            System.IO.File.WriteAllText(filename, results);
            Console.WriteLine(filename);
            Assert.Equal(filename, filename);
            


        }

        public Dictionary<string,string> L01_1_Tests_Results()
        {
            
            Dictionary<string,string> tr=new System.Collections.Generic.Dictionary<string, string>();
            
            String result="";
            //Test T01
            result=L01_1_testrunner.L01_1_T01();
            tr.Add("L01_1_T01",result);
            //Test T02
            result=L01_1_testrunner.L01_1_T02();
            tr.Add("L01_1_T02",result);
            //Test T03
            result=L01_1_testrunner.L01_1_T03();
            tr.Add("L01_1_T03",result);
            return tr;
        }
        public Dictionary<string,string> L01_2_Tests_Results()
        {
            
            Dictionary<string,string> tr=new System.Collections.Generic.Dictionary<string, string>();
            
            String result="";
            MyConfiguration c=new MyConfiguration();
            //Test T01
            result=L01_2_testrunner.L01_2_T01();
            tr.Add("L01_2_T01",result);
            //Test T02
            result=L01_2_testrunner.L01_2_T02();
            tr.Add("L01_2_T02",result);
            //Test T03
            result=L01_2_testrunner.L01_2_T03();
            tr.Add("L01_2_T03",result);
            //Test T04
            result=L01_2_testrunner.L01_2_T04();
            if (result.Contains(c.MSG_PatientTelecomSearchEmailOnly))
            {
                result=c.MSG_PatientTelecomSearchEmailOnly;
            }
            tr.Add("L01_2_T04",result);
            //Test T05
            result=L01_2_testrunner.L01_2_T05();
            tr.Add("L01_2_T05",result);
            return tr;
        }
        public Dictionary<string,string> L02_1_Tests_Results()
        {
            
            Dictionary<string,string> tr=new System.Collections.Generic.Dictionary<string, string>();
            
            String result="";
            //Test T01
            result=L02_1_testrunner.L02_1_T01();
            tr.Add("L02_1_T01",result);
            //Test T02
            result=L02_1_testrunner.L02_1_T02();
            tr.Add("L02_1_T02",result);
            //Test T03
            result=L02_1_testrunner.L02_1_T03();
            tr.Add("L02_1_T03",result);
            //Test T04
            result=L02_1_testrunner.L02_1_T04();
            tr.Add("L02_1_T04",result);
            //Test T05
            result=L02_1_testrunner.L02_1_T05();
            tr.Add("L02_1_T05",result);
            //Test T06
            result=L02_1_testrunner.L02_1_T06();
            tr.Add("L02_1_T06",result);
            //Test T07
            result=L02_1_testrunner.L02_1_T07();
            tr.Add("L02_1_T07",result);
            //Test T08
            result=L02_1_testrunner.L02_1_T08();
            tr.Add("L02_1_T08",result);
            //Test T09
            result=L02_1_testrunner.L02_1_T09();
            tr.Add("L02_1_T09",result);
            //Test T10
            result=L02_1_testrunner.L02_1_T10();
            tr.Add("L02_1_T10",result);
            return tr;
            
            
        }
        public Dictionary<string,string> L02_2_Tests_Results()
        {
            
            Dictionary<string,string> tr=new System.Collections.Generic.Dictionary<string, string>();
            MyConfiguration c=new MyConfiguration();
            String result="";
            //Test T01
            result=L02_2_testrunner.L02_2_T01();
            tr.Add("L02_2_T01",result);
            //Test T02
            result=L02_2_testrunner.L02_2_T02();
            if (result.Contains(c.MSG_PractitionerNotFound))
            {
                result=c.MSG_PractitionerNotFound;
            }
            
            tr.Add("L02_2_T02",result);
            //Test T03
            result=L02_2_testrunner.L02_2_T03();
            if (result.Contains(c.MSG_PersonNotAPractitioner))
            {
                result=c.MSG_PersonNotAPractitioner;
            }
            tr.Add("L02_2_T03",result);
            return tr;
          
        }
        public Dictionary<string,string> L02_3_Tests_Results()
        {
            
            Dictionary<string,string> tr=new System.Collections.Generic.Dictionary<string, string>();
            
            String result="";
            //Test T01
            result=L02_3_testrunner.L02_3_T01();
            tr.Add("L02_3_T01",result);
            //Test T02A
            result=L02_3_testrunner.L02_3_T02A();
            tr.Add("L02_3_T02A",result);
            //Test T02B
            result=L02_3_testrunner.L02_3_T02B();
            tr.Add("L02_3_T02B",result);
            //Test T03A
            result=L02_3_testrunner.L02_3_T03A();
            tr.Add("L02_3_T03A",result);
            //Test T03B
            result=L02_3_testrunner.L02_3_T03B();
            tr.Add("L02_3_T03B",result);
            //Test T04A
            result=L02_3_testrunner.L02_3_T04A();
            tr.Add("L02_3_T04A",result);
            //Test T04B
            result=L02_3_testrunner.L02_3_T04B();
            tr.Add("L02_3_T04B",result);
            return tr;
          
        }
        public Dictionary<string,string> L02_4_Tests_Results()
        {
            
            Dictionary<string,string> tr=new System.Collections.Generic.Dictionary<string, string>();
            
            String result="";
            MyConfiguration c=new MyConfiguration();
            //Test T01A
            result=L02_4_testrunner.L02_4_T01A();
            tr.Add("L02_4_T01A",result);
            //Test T01B
            result=L02_4_testrunner.L02_4_T01B();
            if (result.Contains(c.MSG_PractitionerOnlyIdentifierNPI))
            {
                result=c.MSG_PractitionerOnlyIdentifierNPI;
            }
            
            tr.Add("L02_4_T01B",result);
            //Test T01C
            result=L02_4_testrunner.L02_4_T01C();
            tr.Add("L02_4_T01C",result);
            
            //Test T02A
            result=L02_4_testrunner.L02_4_T02A();
            tr.Add("L02_4_T02A",result);
            //Test T02B
            result=L02_4_testrunner.L02_4_T02B();
            tr.Add("L02_4_T02B",result);
            //Test T02C
            result=L02_4_testrunner.L02_4_T02C();
            tr.Add("L02_4_T02C",result);
            
            return tr;
          
        }
        public Dictionary<string,string> L02_5_Tests_Results()
        {
            
            Dictionary<string,string> tr=new System.Collections.Generic.Dictionary<string, string>();
            
            String result="";
            MyConfiguration c=new MyConfiguration();
            //Test T01A
            result=L02_5_testrunner.L02_5_T01A();
            tr.Add("L02_5_T01A",result);
            //Test T01B
            result=L02_5_testrunner.L02_5_T01B();
            tr.Add("L02_5_T01B",result);
            //Test T02A
            result=L02_5_testrunner.L02_5_T02A();
            if (result.Contains(c.MSG_PractitionerTelecomSearchEmailOnly))
            {
                result=c.MSG_PractitionerTelecomSearchEmailOnly;
            }
            tr.Add("L02_5_T02A",result);
            //Test T02B
            result=L02_5_testrunner.L02_5_T02B();
            tr.Add("L02_5_T02B",result);
            //Test T02C
            result=L02_5_testrunner.L02_5_T02C();
            tr.Add("L02_5_T02C",result);
            //Test T02D
            result=L02_5_testrunner.L02_5_T02D();
            tr.Add("L02_5_T02D",result);
            
            return tr;
        }
        public Dictionary<string,string> L03_1_Tests_Results()
        {
            
            Dictionary<string,string> tr=new System.Collections.Generic.Dictionary<string, string>();
            
            String result="";
              //Test T01
            result=L03_3_testrunner.L03_3_T01A();
            tr.Add("L03_3_T01A",result);
            //Test T02
            result=L03_3_testrunner.L03_3_T02A();
            tr.Add("L03_3_T02A",result);
            //Test T03
            result=L03_3_testrunner.L03_3_T03A();
            tr.Add("L03_3_T03A",result);
            return tr;
        }
        
        public Dictionary<String,String> AddAll(Dictionary<String,String> tAll,Dictionary<String,String> tOne)
        {
            Dictionary<string,string> tl=new System.Collections.Generic.Dictionary<string, string>();
            foreach(KeyValuePair<string,string> test in tAll)
            {
                tl.Add(test.Key,test.Value);

            }
            foreach(KeyValuePair<string,string> test in tOne)
            {
                Console.WriteLine("adding...");
                Console.WriteLine(test.Key);     
                tl.Add(test.Key,test.Value);

            }
            return tl;  

        }
            }
}