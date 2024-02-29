using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RacingImport
{
    
    internal class Producto
    {
        
        public string IdA3 { get; set; }
        public string Name { get; set; }
        public double RegularPrice { get; set; }
        public double SalePrice { get; set; }
        public int StockActual { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public int Family { get; set; }
        public string Tax { get; set; }
        public string IdW { get; set; }

        public override string ToString()
        {
            return $"IdA3: {IdA3}, Name: {Name}, RegularPrice: {RegularPrice}, SalePrice: {SalePrice}, StockActual: {StockActual}";
        }


        public String stockStatus(int stockActual)
        {
            if (stockActual == 0)
            {
                return "outofstock";
            }
            return "instock";
        }

        public String jsonProduct()
        {
            String json = "{" +
                    "\"name\": \"" + Name + "\"," +
                    "\"regular_price\": \"" + RegularPrice + "\"," +
                    "\"sale_price\": \"" + SalePrice + "\"," +
                    "\"stock_status\": \"" + stockStatus(StockActual) + "\"}";
            return json;
        }

        public Producto()
        {
        }

        public Producto(string id, string name, double regularPrice, double salePrice, int stockActual)
        {
            IdA3 = id;
            Name = name;
            RegularPrice = regularPrice;
            SalePrice = salePrice;
            StockActual = stockActual;
        }

        public void ejecucionProducto()
        {
            string file_dat = "idProductosMap";
            //Map, ConnectorW
            Dictionary<string, string> idMap = MapManager.LoadIdMap(file_dat);
            ConectorWooCommerce connW = new ConectorWooCommerce();

            //Procesamos producto
            ProcesarProducto(connW, idMap, file_dat);

            //Ver Map
            MapManager.VerMap(idMap);
            MapManager.DirectorioMap("idMap.dat");

            Console.WriteLine("Presiona Enter para salir...");
            Console.ReadLine();
        }
        public void ProcesarProducto(ConectorWooCommerce connW, Dictionary<string, string> idMap, string file_dat)
        {
            String id = IdA3;
            string apiUrl = "https://fuentecarrantona.com/wp-json/wc/v3/products";
            string json = jsonProduct();

            if (idMap.ContainsKey(id))
            {
                string idW = MapManager.GetValueFromId(idMap, id);
                String urlId = apiUrl + "/" + idW;
                HttpResponseMessage responsePut = connW.Put(json, urlId);
                Console.WriteLine(responsePut.ToString());
                Console.WriteLine("idA3=" + id + " " + Name + " idW=" + idW + "  A3 --- Actualizado ---> W");
            }
            else
            {
                HttpResponseMessage responsePost = connW.Post(json, apiUrl);
                string idW = connW.GetIdWFromResponse(responsePost);
                idMap.Add(id, idW);
                MapManager.SaveIdMap(idMap, file_dat);
                Console.WriteLine(responsePost.ToString());
                Console.WriteLine("idA3=" + id + " " + Name + " idW=" + idW + "  A3 --- NUEVO ---> W");
            }

        }
    }

}
