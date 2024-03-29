﻿using ADODB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Net.Http;
using static System.Net.WebRequestMethods;
using a3ERPActiveX;
using System.Xml.Linq;
using System.Text.Json.Nodes;


namespace RacingImport
{
    internal class Pedido
    {
        string IdW { get; set; }
        string IdA3 { get; set; }
        string DateCreated { get; set; }
        string ShippingTotal { get; set; }
        string Total { get; set; }
        int CustomerId { get; set; }
        Billing billing { get; set; }
        List<LineItem> LineItems { get; set; }

        public Pedido()
        {
        }
        public void PedidoFromJson(JsonElement jsonElement)
        {
            IdW = jsonElement.GetProperty("id").GetString();
            DateCreated = jsonElement.GetProperty("date_created").GetString();
            ShippingTotal = jsonElement.GetProperty("shipping_total").GetString();
            Total = jsonElement.GetProperty("total").GetString();
            CustomerId = jsonElement.GetProperty("customer_id").GetInt32();

            billing = new Billing(jsonElement.GetProperty("billing"));

            LineItems = jsonElement.GetProperty("line_items")
                                       .EnumerateArray()
                                       .Select(item => new LineItem(item))
                                       .ToList();
        }


        class Billing
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Company { get; set; }
            public string Address1 { get; set; }
            public string City { get; set; }
            public string Postcode { get; set; }
            public string Country { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }

            public Billing(JsonElement jsonElement)
            {
                FirstName = jsonElement.GetProperty("first_name").GetString();
                LastName = jsonElement.GetProperty("last_name").GetString();
                Company = jsonElement.GetProperty("company").GetString();
                Address1 = jsonElement.GetProperty("address_1").GetString();
                City = jsonElement.GetProperty("city").GetString();
                Postcode = jsonElement.GetProperty("postcode").GetString();
                Country = jsonElement.GetProperty("country").GetString();
                Email = jsonElement.GetProperty("email").GetString();
                Phone = jsonElement.GetProperty("phone").GetString();
            }
        }

        class LineItem
        {
            public int LineItemId { get; set; }
            public string Name { get; set; }
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public string Total { get; set; }

            public LineItem(JsonElement jsonElement)
            {
                LineItemId = jsonElement.GetProperty("id").GetInt32();
                Name = jsonElement.GetProperty("name").GetString();
                ProductId = jsonElement.GetProperty("product_id").GetInt32();
                Quantity = jsonElement.GetProperty("quantity").GetInt32();
                Total = jsonElement.GetProperty("total").GetString();
            }
        }

        public async void PedidosWooToDB()
        {   
            //Map
            string file_dat = "idPedidosMap";
            Dictionary<string, string> idMap = MapManager.LoadIdMap(file_dat);

            // Conexión a la API
            HttpClient client = new HttpClient();
            string apiBaseUrl = "https://fuentecarrantona.com/wp-json/wc/v3";
            string ordersEndpoint = "/orders";

            for (int i = 1; ; i++) //paginacion woocommerce
            {
                HttpResponseMessage response = await client.GetAsync($"{apiBaseUrl}{ordersEndpoint}?page={i}");
                response.EnsureSuccessStatusCode();

                // Procesar la respuesta JSON
                System.IO.Stream responseStream = await response.Content.ReadAsStreamAsync();
                {
                    using (JsonDocument jsonDocument = await JsonDocument.ParseAsync(responseStream))
                    {
                        // Verifica si hay más páginas
                        if (!jsonDocument.RootElement.EnumerateArray().Any())
                        {
                            break;  // No hay más páginas, sale del bucle
                        }

                        // Obtiene el JsonArray como lista de JsonElement
                        List<JsonElement> jsonArray = jsonDocument.RootElement.EnumerateArray().ToList();
                        foreach (JsonElement jsonElement in jsonArray)
                        {
                            //Actualiza propiedades con los datos del JsonElement
                            PedidoFromJson(jsonElement);

                            //Sólo hay que crear nuevo pedido si no existe (no modificar)
                            //Gestion IdA3: tengo IdW de Woo
                            string IdA3 = idMap.FirstOrDefault(x => x.Value == IdW).Key;
                            if (IdA3 == null)
                            {
                                InsertarNuevoPedido();
                            }
                        }
                    }
                }
            }
        }
        

        //Crear nuevo pedido - Curso a3ERP
        private void InsertarNuevoPedido()
        {
            IPedido Documento = new a3ERPActiveX.Pedido();
            Documento.Iniciar();
            try
            {
                Documento.Nuevo(DateTime.Now.ToString(DateCreated), "CustomerId", true); // Nuevo pedido de compra para el proveedor 2--> CustomerId
                try
                {
                   
                    Documento.AsStringCab["NUMDOC"] = IdA3;
                    //LineItems: 
                    foreach (LineItem lineItem in LineItems)
                    {
                        Documento.NuevaLineaArt("lineItem.ProductId", lineItem.Quantity);
                        Documento.AnadirLinea();
                    }
                    decimal DocId = Documento.Anade();
                    Console.WriteLine("Creado documento con identificador { 0}", DocId);

                    /*
                      Documento.AsStringCab["FECENTREGA"] = DateTime.Now.ToString("dd / MM / yyyy");
                      Documento.AsStringCab[ "CAMPO" ] = ShippingTotal;
                      Documento.AsStringCab[ "CAMPO" ] = Total;
                      Documento.AsIntegerCab[ "CAMPO" ] = pedido.CustomerId;

                    //Billing:
                      Documento.AsStringCab["CAMPO"] = billing.FirstName;
                      Documento.AsStringCab["CAMPO"] = billing.LastName;
                      Documento.AsStringCab["CAMPO"] = billing.Company;
                      Documento.AsStringCab["CAMPO"] = billing.Address1;
                      Documento.AsStringCab["CAMPO"] = billing.City;
                      Documento.AsStringCab["CAMPO"] = billing.Postcode;
                      Documento.AsStringCab["CAMPO"] = billing.Country;
                      Documento.AsStringCab["CAMPO"] = billing.Email;
                      Documento.AsStringCab["CAMPO"] = billing.Phone;
                    
                       Documento.AsStringCab[ "CAMPO" ] = LineItemId;
                       Documento.AsStringCab[ "CAMPO" ] = Name;
                       Documento.AsStringCab[ "CAMPO" ] = ProductId;
                       Documento.AsStringCab[ "CAMPO" ] = Quantity;
                       Documento.AsStringCab[ "CAMPO" ] = Total; 
                    
                      Documento.AsStringCab[''SERIE''] = ''2020'';
                      Documento.AsIntegerCab[''NUMDOC''] = 999999;
                      Documento.AsStringCab[''REFERENCIA''] = ''Documento creado desde a3ErpActiveX'';
                      Documento.AsStringCab[''FECENTREGA''] = DateTime.Now.ToString(''dd / MM / yyyy''); // Fecha de entrega
                      Documento.AsCurrencyCab[''PORPRONTO''] = 13; // Descuento por pronto pago

                      Documento.NuevaLineaArt(''2'', 10); // Artículo 2 x 10 unidades.
                      Documento.AsFloatLin[''DESC1''] = 5; // Sobreescribimos el % dto.por defecto.
                      Documento.AsFloatLin[''PRCMONEDA''] = Documento.AsFloatLin[''PRCMONEDA''] * 1.2;
                      // Operación de lectura y escritura del valor de un campo.Por ejemplo ''Incrementamos el precio por defecto un 20%''.
                      // Para calcular el precio hay ciertos campos que afectan al mismo(artículo, descuentos, etc).
                      // Si queremos forzar un precio concreto, se debe aplazar la asignación del campo PRCMONEDA al final de todo.}
                      Documento.AnadirLinea();
                      decimal DocId = Documento.Anade();
                      MessageBox.Show(String.Format(''Creado documento con identificador { 0}
                      '', DocId));
                    */
                }
                catch (Exception err)
                {
                    Documento.Cancela();
                    Console.WriteLine("Se ha producido el error { 0}", err.Message);
                }
            }
            finally
            {
                Documento.Acabar(); // Liberar recursos.
            }
        }

        /* public static String asignarIdA3(Pedido pedido, Dictionary<string, string> idMap)
         { 
             string IdA3 = idMap.FirstOrDefault(x => x.Value == pedido.IdW).Key;
             if (IdA3 == null){
                 //*aquí
                 idF = FileMapManager.generateNewIdFOrderIds(map);
                 IdFVsIdWOrder idsOrder = new IdFVsIdWOrder(idF, order.getIdW());
                 FileMapManager.addOrderIdsMap(idsOrder, map);
                 FileMapManager.saveOrderIds(Files.IDF_VS_IDW_ORDERS, map);
                 System.out.println("idF=" + idF + " " + order.getBilling().getEmail() + " idW=" + order.getIdW() + "   W --- ACUALIZADO ---> F");
                 logger.info("Nuevo pedido. Detalles: idF={}, idW={}, email={}", idF, order.getIdW(), order.getBilling().getEmail());
             }
         return idA3;
         }*/
        /*
        public Pedido(JsonElement jsonElement)
        {
            Pedido pedido = new Pedido();
            pedido.IdW = jsonElement.GetProperty("id").GetString();

            return pedido;
        }
        */

        public async void EjecutarPedido2()
        {
            int pages;
            int i = 1;

            do
            {
                string file_dat = "idPedidosMap";

                // Map, ConnectorW
                Dictionary<string, string> idMap = MapManager.LoadIdMap(file_dat);
                ConectorWooCommerce connW = new ConectorWooCommerce();

                // Llamada al método Get
                HttpResponseMessage response = GetOrdersPage(i);

                // Procesar la respuesta JSON
                JsonArray jsonArray = await response.Content.ReadAsAsync<JsonArray>();

                // Insertar pedidos en la base de datos
                //MatchOrder.OrdersToBBDD(jsonArray);

                // Obtener el número total de páginas
                pages = connW.Pagination(response);
                i = i + 1;

            } while (i <= pages);
        }

        private HttpResponseMessage GetOrdersPage(int page)
        {
            // Conexión a la API utilizando el método Get
            string apiBaseUrl = "https://fuentecarrantona.com/wp-json/wc/v3";
            string ordersEndpoint = "/orders";

            return Get($"{apiBaseUrl}{ordersEndpoint}?page={page}");
        }

        private HttpResponseMessage Get(string apiUrl)
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



    }
}
    

    


