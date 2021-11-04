using System;
using System.Text;
using System.Web;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using FhirQuestionnairePoc.Settings;

namespace FhirQuestionnairePoc.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMemoryCache cache;

        public IndexModel(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public string LaunchUrl { get; set; }

        public string PatientId { get; } = DefaultSettings.PatientId;
        public string PatientName { get; } = DefaultSettings.PatientName;

        public void OnGet()
        {
            string baseFhirUrl = "https://fhir.epic.com/interconnect-fhir-oauth/api/FHIR/R4/";
            string scope = "patient/*.read launch/patient online_access openid profile";
            string redirect_uri = "https://localhost:5001/token";

            FhirClient client = new(baseFhirUrl, DefaultSettings.FhirClientSettings);
            CapabilityStatement capability = client.CapabilityStatement();
            Extension security = capability.Rest[0].Security.GetExtension("http://fhir-registry.smarthealthit.org/StructureDefinition/oauth-uris");
            var authorizeEndpoint = security.GetExtensionValue<FhirUri>("authorize");

            Guid state = Guid.NewGuid();
            cache.Set($"state:{state}", baseFhirUrl);

            StringBuilder launchUrl = new(authorizeEndpoint.Value);
            launchUrl.Append("?client_id=e186ac67-81c3-489c-90c1-fb33df5b6f22");
            launchUrl.Append("&response_type=code");
            launchUrl.Append($"&scope={HttpUtility.UrlEncode(scope)}");
            launchUrl.Append($"&redirect_uri={HttpUtility.UrlEncode(redirect_uri)}");
            launchUrl.Append($"&state={state}");
            launchUrl.Append($"&aud={HttpUtility.UrlEncode(baseFhirUrl)}");

            this.LaunchUrl = launchUrl.ToString();
        }
    }
}
