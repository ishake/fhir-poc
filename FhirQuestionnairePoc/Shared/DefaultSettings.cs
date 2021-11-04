using Hl7.Fhir.Rest;

namespace FhirQuestionnairePoc.Settings
{
    public static class DefaultSettings
    {
        public static readonly string PatientName = "Lopez, Camila";
        public static readonly string PatientId = "erXuFYUfucBZaryVksYEcMg3";
        public static readonly FhirClientSettings FhirClientSettings = new FhirClientSettings
        {
            PreferredFormat = ResourceFormat.Json,
            VerifyFhirVersion = true,
            // PreferredReturn = Prefer.ReturnRepresentation,
            UseFormatParameter = true,
            Timeout = 60000,
            PreferredParameterHandling = SearchParameterHandling.Lenient,
            // PreferCompressedResponses
            // CompressRequestBody
            // ParserSettings
        };
    }
}
