using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace fhir_server_sharedservices
{
    public static class FhirServerConfig
    {
        private static readonly IConfigurationRoot configuration;

        static FhirServerConfig() 
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            PortListening = Convert.ToInt32(configuration["ListenerConfig:PortListening"]);
            FHIRServerUrl = Convert.ToString(configuration["ListenerConfig:FHIRServerUrl"]);
            ValidatePractitionerBundleAndResource = Convert.ToBoolean(configuration["ResourceValidation:ValidatePractitionerBundleAndResource"]);
            ValidatePatientBundleAndResource = Convert.ToBoolean(configuration["ResourceValidation:ValidatePatientBundleAndResource"]);
            ValidateMedicationRequestBundleAndResource = Convert.ToBoolean(configuration["ResourceValidation:ValidateMedicationRequestBundleAndResource"]);
            ValidateCapabilityStatementResource = Convert.ToBoolean(configuration["ResourceValidation:ValidateCapabilityStatementResource"]);
        }

        public static int PortListening { get; private set; }
        public static string FHIRServerUrl { get; private set; }
        public static bool ValidatePractitionerBundleAndResource { get; private set; }
        public static bool ValidatePatientBundleAndResource { get; private set; }
        public static bool ValidateMedicationRequestBundleAndResource { get; private set; }
        public static bool ValidateCapabilityStatementResource { get; private set; }
    }
}
