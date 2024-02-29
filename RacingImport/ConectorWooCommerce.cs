using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RacingImport
{
    internal class ConectorWooCommerce
    {
        private const String consumerKey = "ck_d6445233c3510c084209e0347549999285d3eb4a";
        private const String consumerSecret= "cs_8ba2e34d5426adab00e357467140c1d2f6fd8501";
        public HttpResponseMessage Put(string jsonBody, string apiUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://fuentecarrantona.com");

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, apiUrl))
                {
                    request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Accept", "application/json");

                    string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{consumerKey}:{consumerSecret}"));
                    request.Headers.Add("Authorization", $"Basic {credentials}");

                    Task<HttpResponseMessage> responseTask = httpClient.SendAsync(request);

                    // Esperar a que la tarea asíncrona se complete (síncrono)
                    responseTask.Wait();

                    return responseTask.Result;
                }
            }
        }

        public HttpResponseMessage Post(string jsonBody, string apiUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://fuentecarrantona.com"); 

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiUrl))
                {
                    request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Accept", "application/json");

                    string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{consumerKey}:{consumerSecret}"));
                    request.Headers.Add("Authorization", $"Basic {credentials}");

                    Task<HttpResponseMessage> responseTask = httpClient.SendAsync(request);

                    // Esperar a que la tarea asíncrona se complete (síncrono)
                    responseTask.Wait();

                    return responseTask.Result;
                }
            }
        }

        public string GetIdWFromResponse(HttpResponseMessage response)
        {
            // Verificar si la respuesta es exitosa (código 2xx)
            if (response.IsSuccessStatusCode)
            {
                // Obtener el contenido de la respuesta
                string responseContent = response.Content.ReadAsStringAsync().Result;

                // Parsear la respuesta JSON usando System.Text.Json
                var jsonDocument = JsonDocument.Parse(responseContent);

                // Obtener el root del JSON
                var root = jsonDocument.RootElement;

                // Verificar si la respuesta contiene el ID del producto
                if (root.TryGetProperty("id", out var idElement) && idElement.ValueKind == JsonValueKind.Number)
                {
                    return idElement.GetInt32().ToString();
                }
            }

            // Si no se encontró el ID o la respuesta no fue exitosa, puedes manejarlo de acuerdo a tus necesidades
            return null;
        }



        public HttpResponseMessage Get(string apiUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://fuentecarrantona.com");

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiUrl))
                {
                    request.Headers.Add("Accept", "application/json");

                    string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{consumerKey}:{consumerSecret}"));
                    request.Headers.Add("Authorization", $"Basic {credentials}");

                    Task<HttpResponseMessage> responseTask = httpClient.SendAsync(request);

                    // Esperar a que la tarea asíncrona se complete (síncrono)
                    responseTask.Wait();

                    return responseTask.Result;
                }
            }
        }

        public int Pagination(HttpResponseMessage response)
        {
            IEnumerable<string> totalElementsHeader;
            if (response.Headers.TryGetValues("X-WP-Total", out totalElementsHeader))
            {
                int totalElements = int.Parse(totalElementsHeader.First());
                int pages = (totalElements / 100) + 1;
                return pages;
            }

            // En caso de que no se encuentre el encabezado "X-WP-Total" o no pueda ser parseado, puedes manejarlo de acuerdo a tus necesidades.
            return 0;
        }





    }
}
