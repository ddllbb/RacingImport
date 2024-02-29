using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace RacingImport
{
    internal class MapManager
    {
        
        public static void VerMap(Dictionary<string, string> idMap)
        {
            foreach (var kvp in idMap)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
        }

        public static void DirectorioMap(string file_dat)
        {
            string directorioActual = Directory.GetCurrentDirectory();
            string rutaArchivo = Path.Combine(directorioActual, file_dat);
            Console.WriteLine("Ruta del archivo: " + rutaArchivo);
        }
        public static string GetValueFromId (Dictionary<string, string> idMap, string id)
        {
            if (idMap.TryGetValue(id, out string idW))
            {
                //Console.WriteLine($"El valor asociado a la clave '{id}' es: {idW}");
            }
            else
            {
                Console.WriteLine($"No se encontró ningún valor para la clave '{id}'");
            }
            return idW;
        }
        public static void SaveIdMap(Dictionary<string, string> idMap, string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, idMap);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar el mapa de IDs: {ex.Message}");
            }
        }

        public static Dictionary<string, string> LoadIdMap(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open))
                    {
                        IFormatter formatter = new BinaryFormatter();
                        return (Dictionary<string, string>)formatter.Deserialize(fs);
                    }
                }
                else
                {
                    return new Dictionary<string, string>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar el mapa de IDs: {ex.Message}");
                return new Dictionary<string, string>();
            }
        }
    }

    class Program
    {
        static void EjemploMain()
        {
            // Uso del diccionario y almacenamiento en un archivo
            Dictionary<string, string> idMap = MapManager.LoadIdMap("idMap.dat");

            // Agregar o actualizar valores en el diccionario
            idMap["id1"] = "efp1";
            idMap["id2"] = "efp2";

            // Guardar el diccionario actualizado
            MapManager.SaveIdMap(idMap, "idMap.dat");

            // Imprimir el contenido del diccionario
            foreach (var entry in idMap)
            {
                Console.WriteLine($"ID: {entry.Key}, EFP ID: {entry.Value}");
            }
        }
    }
}
