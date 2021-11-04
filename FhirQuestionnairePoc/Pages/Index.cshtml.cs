﻿using System;
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

        public void OnGet()
        {
            string baseFhirUrl = "https://fhir.epic.com/interconnect-fhir-oauth/api/FHIR/R4/";
            string scope = "patient/Patient.read patient/Observation.read launch/patient online_access openid profile";
            string redirect_uri = "https://localhost:5001/token";

            FhirClient client = new(baseFhirUrl, DefaultFhirClientSettings.Settings);
            CapabilityStatement capability = client.CapabilityStatement();
            Extension security = capability.Rest[0].Security.GetExtension("http://fhir-registry.smarthealthit.org/StructureDefinition/oauth-uris");
            var authorizeEndpoint = security.GetExtensionValue<FhirUri>("authorize");

            Guid state = Guid.NewGuid();
            cache.Set($"state:{state}", baseFhirUrl);

            StringBuilder redirectUri = new(authorizeEndpoint.Value);
            redirectUri.Append("?client_id=e186ac67-81c3-489c-90c1-fb33df5b6f22");
            redirectUri.Append("&response_type=code");
            redirectUri.Append($"&scope={HttpUtility.UrlEncode(scope)}");
            redirectUri.Append($"&redirect_uri={HttpUtility.UrlEncode(redirect_uri)}");
            redirectUri.Append($"&state={state}");
            redirectUri.Append($"&aud={HttpUtility.UrlEncode(baseFhirUrl)}");

            this.LaunchUrl = redirectUri.ToString();
        }
    }
}
