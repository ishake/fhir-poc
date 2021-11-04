using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using FhirQuestionnairePoc.Settings;
using System;

namespace FhirQuestionnairePoc.Pages
{
    public class TokenModel : PageModel
    {
        private static readonly HttpClient httpClient = new();
        private readonly IMemoryCache cache;

        public TokenModel(IMemoryCache cache)
        {
            this.cache = cache;
        }

        [BindProperty(SupportsGet = true)]
        public string Code { get; set; }

        [BindProperty(SupportsGet = true)]
        public string State { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (Request.Query.ContainsKey("error"))
            {
                Console.WriteLine("Error from authorization server:");
                Console.WriteLine(Request.QueryString);
                return new OkResult();
            }

            FormUrlEncodedContent content = new(new[]
            {
                new KeyValuePair<string, string>("code", this.Code),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("redirect_uri", "https://localhost:5001/token"),
                new KeyValuePair<string, string>("client_id", "e186ac67-81c3-489c-90c1-fb33df5b6f22")
            });

            string iss = cache.Get<string>($"state:{this.State}");

            FhirClient fhirClient = new(iss, DefaultFhirClientSettings.Settings);
            CapabilityStatement capability = fhirClient.CapabilityStatement();
            Extension security = capability.Rest[0].Security.GetExtension("http://fhir-registry.smarthealthit.org/StructureDefinition/oauth-uris");
            var tokenEndpoint = security.GetExtensionValue<FhirUri>("token");

            HttpResponseMessage response = await httpClient.PostAsync(tokenEndpoint.Value, content);

            string payload = await response.Content.ReadAsStringAsync();
            AccessTokenContext accessTokenContext = JsonSerializer.Deserialize<AccessTokenContext>(payload);

            return RedirectToPage("Questionnaire", new
            {
                AccessToken = accessTokenContext.access_token,
                Patient = accessTokenContext.patient,
                State = this.State
            });
        }
    }
}
