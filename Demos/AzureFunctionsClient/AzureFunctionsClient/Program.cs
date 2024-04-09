using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace AzureFunctionsClient
{
    internal class Program
    {
        private static string apiBaseUrl = "https://nacs-2024-vs-csharp-prep.azurewebsites.net";
        private static string scopesUri = "api://8e86ae2f-a516-474f-ba0c-fb9101332a0e";

        static void Main(string[] args)
        {
            AuthenticatedGetRequest("GetCustomers").Wait();
        }

        static async Task GetRequest(string route)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var endpoint = $"{apiBaseUrl}/api/{route}";                
                using (var response = await client.GetAsync(endpoint))
                {
                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(jsonResponse);
                }
            }
        }

        static async Task NorthwindGetRequest()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var endpoint = $"https://services.odata.org/northwind/northwind.svc/Customers";
                using (var response = await client.GetAsync(endpoint))
                {
                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(jsonResponse);
                }
            }
        }

        private async static Task<string> GetAccessToken()
        {
            var tenantName = "robwindsor2";
            var clientId = "8e86ae2f-a516-474f-ba0c-fb9101332a0e";
            var clientSecret = "r2H8Q~57o4e_MEqpMP-ayEbV61SXU7QgMJbFrcUF";
            var authority = $"https://login.microsoftonline.com/{tenantName}.onmicrosoft.com/";
            var azureApp = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithAuthority(authority)
                .WithClientSecret(clientSecret)
                .Build();

            var scopes = new string[] { $"{scopesUri}/.default" };
            var authResult = await azureApp.AcquireTokenForClient(scopes).ExecuteAsync();
            return authResult.AccessToken;
        }

        static async Task AuthenticatedGetRequest(string functionName)
        {
            var token = await GetAccessToken();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var endpoint = $"{apiBaseUrl}/api/{functionName}";
                using (var response = await client.GetAsync(endpoint))
                {
                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(jsonResponse);
                }
            }
        }
    }
}
