using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RacingImport
{
    
    internal class Cliente
    {
        
        public string IdA3 { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string IdW { get; set; }

        public override string ToString()
        {
            return $"IdA3: {IdA3}, Name: {Name}, Phone: {Phone}, Email: {Email}";
        }

       
        public String JsonCliente()
        {
            String json = "{\"first_name\": \"" + Name + "\", \"billing\": {\"phone\": \"" + Phone + "\"}, \"email\": \"" + Email + "\"}";
            return json;
        }

        public Cliente()
        {
        }

        public Cliente(string idA3, string name, string phone, string email)
        {
            IdA3 = idA3;
            Name = name;
            Phone = phone;
            Email = email;
        }

        public void ejecucionCliente()
        {
            string file_dat = "idClientesMap";
            //Map, ConnectorW
            Dictionary<string, string> idMap = MapManager.LoadIdMap(file_dat);
            ConectorWooCommerce connW = new ConectorWooCommerce();

            //Procesamos cliente
            ProcesarCliente(connW, idMap, file_dat);

            //Ver Map
            MapManager.VerMap(idMap);
            MapManager.DirectorioMap(file_dat);

            Console.WriteLine("Presiona Enter para salir...");
            Console.ReadLine();
        }
        public void ProcesarCliente(ConectorWooCommerce connW, Dictionary<string, string> idMap, string file_dat)
        {
            String id = IdA3;
            string apiUrl = "https://fuentecarrantona.com/wp-json/wc/v3/customers";
            string json = JsonCliente();

            if (idMap.ContainsKey(id))
            {
                string idW = MapManager.GetValueFromId(idMap, id);
                String urlId = apiUrl + "/" + idW;
                HttpResponseMessage responsePut = connW.Put(json, urlId);
                Console.WriteLine(responsePut.ToString());
                Console.WriteLine("idA3=" + id + " " + Email + " idW=" + idW + "  A3 --- Actualizado ---> W");
            }
            else
            {
                HttpResponseMessage responsePost = connW.Post(json, apiUrl);
                string idW = connW.GetIdWFromResponse(responsePost);
                idMap.Add(id, idW);
                MapManager.SaveIdMap(idMap, file_dat);
                Console.WriteLine(responsePost.ToString());
                Console.WriteLine("idA3=" + id + " " + Email + " idW=" + idW + "  A3 --- NUEVO ---> W");
            }

        }
    }

}
