using a3ERPActiveX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADODB;
using System.Security.Cryptography;


namespace RacingImport
{
    internal class Conexion
    {
        IEnlace enlace;
        Connection connection;

        public Conexion(string nombre)
        {
            this.enlace = selecEmpresaActiva();
            this.connection = getAdoConnection(enlace, nombre);
        }

        public Connection getAdoConnection(IEnlace enlace, String nombre)
        {
            Connection connection = enlace.GetConexionDB(nombre);
            return connection;
        }

        public IEnlace selecEmpresaActiva()
        {
            Boolean lConectado = false;
            IEnlace e = new a3ERPActiveX.Enlace();
            lConectado = e.SelecEmpresa();

            try
            {
                if (!lConectado)
                {
                    Console.WriteLine("Error al conectar con la base de datos de A3");
                }
            }
            catch (Exception) { }

            return e;
        }
    }

    

    internal class Ejemplos
    {
        //lista de empresas
        /* Enlace enlace = new a3ERPActiveX.Enlace();
         Object[] Lst = enlace.Empresas();
         int nItems = Convert.ToInt32(Lst[0]);
         string s = String.Empty;
         Object obj;
         for (int i = 1; i <= nItems; i++)
         {
             obj = (Lst[i] as Object);
             String item = obj.ToString();
             s += "\n" + item;
         }
         // Mostrar la lista en la consola
         Console.WriteLine("Lista de empresas:");
         Console.WriteLine(s);
         Console.WriteLine(Lst[1]);*/

        //Lista Maestros
        /*Enlace enlace = new a3ERPActiveX.Enlace();
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
        Console.WriteLine("Lista de empresas:");
        Console.WriteLine(s);*/

        //Buscar Artículo
        /*Enlace enlace = new a3ERPActiveX.Enlace();

        enlace.Iniciar("LocalRacingImport", "");
        Connection conn = enlace.GetConexionDB("LocalRacingImport");*/

        /* IMaestro maestro = new a3ERPActiveX.Maestro();
        maestro.Iniciar("ARTICULO");
       Console.WriteLine(maestro.AsString["DESCART"]);
       Console.WriteLine(enlace.EmpresaActiva);
       Console.WriteLine(maestro.Estado.ToString());*/

    }
    
}

    
