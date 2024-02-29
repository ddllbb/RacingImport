using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using a3ERPActiveX;
using ADODB;

namespace RacingImport
{
    internal class Probatinas
    {
        static void Main(string[] args)
        {
            Enlace enlace = new a3ERPActiveX.Enlace();
            Object[] LstMaestros = enlace.ListaMaestros();
            int nItems = Convert.ToInt32(LstMaestros[0]);
            string s = String.Empty;
            Object[] obj;
            for (int i = 1; i <= nItems; i++)
            {
                obj = (LstMaestros[i] as Object[]);
                String item = obj[0].ToString();
                if (!string.IsNullOrEmpty(item))
                    s += "\n" + item;
            };
            Console.WriteLine("Lista de maestros:");
            Console.WriteLine(s);
        }
    }
    
   /* internal class Test
    {
        static void Main(string[] args)
        {
            Producto producto = new Producto();
            Cliente cliente = new Cliente();
            ConectorWooCommerce connW = new ConectorWooCommerce();
             
            IEnlace enlace = new a3ERPActiveX.Enlace();
            enlace.Iniciar("LocalRacingImport", "");
            Console.WriteLine(enlace.EmpresaActiva);

            //Maestros: Ejecutar

            //MetodosEjecutar.ArticulosDBToWoo(producto);
            MetodosEjecutar.ClientesDBToWoo(cliente);

            enlace.Acabar();
            Console.WriteLine("Presione enter para salir...");
            Console.ReadLine();
        }
    }*/

    internal class MetodosEjecutar
    {
        public static void ClientesDBToWoo(Cliente cliente)
        {
            //Maestro: CLIENTES
            IMaestro maestro = new a3ERPActiveX.Maestro();
            maestro.Iniciar("CLIENTES");

            try
            {
                maestro.Primero();
                while (!maestro.EOF)
                {
                    //Mapeo Tabla - Objeto
                    string idA3 = maestro.AsString["CODCLI"];
                    string name = maestro.AsString["USUARIO"];
                    string email = maestro.AsString["E_MAIL_DOCS"];
                    // Ejecutar el cliente
                    cliente.IdA3 = idA3;
                    cliente.Name = name;
                    cliente.Email = email;
                    //cliente.ejecucionCliente();
                    Console.WriteLine(cliente.ToString());

                    maestro.Siguiente();
                }
            }
            finally
            {
                //Se liberan los recursos al finalizar
                maestro.Acabar();
            }
        }

        public static void ClientesWooToDB(Cliente cliente)
        {

        }
        public static void ArticulosDBToWoo(Producto producto)
        {
            //Maestro: ARTICULO
            IMaestro maestro = new a3ERPActiveX.Maestro();
            maestro.Iniciar("ARTICULO");

            try
            {
                maestro.Primero();
                while (!maestro.EOF)
                {
                    //Mapeo Tabla - Objeto
                    string idA3 = maestro.AsString["CODART"];
                    string name = maestro.AsString["DESCART"];
                    double regularPrice = maestro.AsFloat["PRCVENTA"];
                    double salePrice = maestro.AsFloat["PRCSTANDARD"];
                    int stockAcutal = maestro.AsInteger["UNIDADESSTOCK"];

                    // Ejecutar el producto
                    producto.IdA3 = idA3;
                    producto.Name = name;
                    producto.RegularPrice = regularPrice;
                    producto.SalePrice = salePrice;
                    producto.StockActual = stockAcutal;
                    //producto.ejecucionProducto();
                    Console.WriteLine(producto.ToString());

                    maestro.Siguiente();
                }
            }
            finally
            {
                //Se liberan los recursos al finalizar
                maestro.Acabar();
            }

         
        }
    }
    internal class EjemplosTest
    {
        /*ConectorWooCommerce connW = new ConectorWooCommerce();
        Cliente cliente = new Cliente("5", "David", "68613", "david@gmail.com");
        Producto producto = new Producto("7", "CSharp", 50, 35, 2);
        //cliente.ejecucionCliente();
        string json = cliente.JsonCliente();
        string jsonP = producto.jsonProduct();

        string apiUrl = "https://fuentecarrantona.com/wp-json/wc/v3/customers";
        string apiUrlP = "https://fuentecarrantona.com/wp-json/wc/v3/products";
        //String idW = "3";
        //String urlId = apiUrl + "/" + idW;
        //HttpResponseMessage responsePut = connW.Put(json, urlId);
        HttpResponseMessage responsePost = connW.Post(json, apiUrl);
        HttpResponseMessage responsePostP = connW.Post(jsonP, apiUrlP);
        string idW = connW.GetIdWFromResponse(responsePost);
        string idWP = connW.GetIdWFromResponse(responsePostP);
        Console.WriteLine(responsePost.ToString());
            Console.WriteLine(responsePostP.ToString());
            
            Console.WriteLine("Presione enter para salir...");
            Console.ReadLine();*/
       /* Producto producto = new Producto("7", "CSharpmodif", 50, 35, 2);
        producto.ejecucionProducto(); */

       // Conexion conn = new Conexion("LocalRacingImport");

       /* dynamic parametros = enlace.ParamConexion("LocalRacingImport");
        string descripcionEmpresa = parametros[0];
        string tipoDatos = parametros[2];
        string servidor = parametros[3];
        string nombreServidor = parametros[4];
        string nombreBaseDatos = parametros[5];*/

        /*IMaestro maestro = new a3ERPActiveX.Maestro();
            maestro.Iniciar("ARTICULO");
            maestro.Buscar(6);
            Console.WriteLine(maestro.AsString["DESCART"]);*/
        //Console.WriteLine(enlace.Conexion);

    }
}
