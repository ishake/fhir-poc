using System;
using System.Text;
using System.Web;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace FhirQuestionnairePoc.Settings
{
    public static class DefaultFhirClientSettings
    {
        public static readonly FhirClientSettings Settings = new FhirClientSettings
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
