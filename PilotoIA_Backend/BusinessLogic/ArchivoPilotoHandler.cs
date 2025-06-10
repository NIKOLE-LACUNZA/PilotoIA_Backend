using System.Text.Json;
using PilotoIA_Backend.DataAccess;
using PilotoIA_Backend.Models;
namespace PilotoIA_Backend.BusinessLogic
{
    public class ArchivoPilotoHandler
    {
        private ArchivoPilotoDAO vgDataAccess;
        private beMySettings vgSettings;
        private readonly IWebHostEnvironment _env;
        public ArchivoPilotoHandler(beMySettings peSettings, IWebHostEnvironment env)
        {
            this.vgSettings = peSettings;
            vgDataAccess = new ArchivoPilotoDAO(vgSettings.DbConnection);
            _env = env;
        }

        public async Task<MensajeRespuesta> RegistrarArchivoPilotoAsync(
            string Titulo,
            string Tema,
            List<ListaArchivos> ArchivosBase64
            )
        {
            try
            {
                var archivosLista = new List<RutaArchivo>();
                var rutasVectores = new List<string>();

                MensajeRespuesta Respuesta = null;

                using var httpClient = new HttpClient();

                for (int i = 0; i < ArchivosBase64.Count; i++)
                {
                    var base64 = ArchivosBase64[i];
                    var nombreArchivo = base64.Nombre;

                    var payload = new
                    {
                        nombreArchivo = nombreArchivo,
                        base64Contenido = base64.Base64
                    };

                    var response = await httpClient.PostAsJsonAsync("https://pilotoianuevobackend-bbb7fqc0hbd4ccaf.canadacentral-01.azurewebsites.net/api/document/subir-base64", payload);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Error al subir el archivo {nombreArchivo}: {response.StatusCode}");
                    }

                    using var responseStream = await response.Content.ReadAsStreamAsync();
                    using var doc = await JsonDocument.ParseAsync(responseStream);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("documento", out var rutaArchivo))
                        archivosLista.Add(new RutaArchivo
                        {
                            Nombre = nombreArchivo,
                            Ruta = rutaArchivo.GetString()
                        });

                    if (root.TryGetProperty("vector", out var rutaVector))
                        rutasVectores.Add(rutaVector.GetString());
                }


                Respuesta = await vgDataAccess.RegistrarArchivoPiloto(
                    Titulo,
                    Tema,
                    archivosLista,
                    rutasVectores
                    );

                return Respuesta;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ListaArchivoPilotoRespuesta> ListarArchivoPilotoAsync(
           string Filtro, 
           int TamanioPagina,
           int NumeroPagina
           )
        {
            try
            {
                ListaArchivoPilotoRespuesta Respuesta = null;

                Respuesta = await vgDataAccess.ListarArchivoPiloto(
                    Filtro,
                    TamanioPagina,
                    NumeroPagina);

                return Respuesta;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<MensajeRespuesta> EditarArchivoPilotoAsync(EditarArchivoPiloto archivo)
        {
            try
            {
                var rutasArchivos = new List<ListaArchivosEdit>();
                var rutasVectores = new List<string>();
                int indice = 0;

                using var httpClient = new HttpClient();

                foreach (var nuevo in archivo.NuevosArchivos)
                {
                    var payload = new
                    {
                        nombreArchivo = nuevo.Nombre,
                        base64Contenido = nuevo.Base64
                    };

                    var response = await httpClient.PostAsJsonAsync("https://pilotoianuevobackend-bbb7fqc0hbd4ccaf.canadacentral-01.azurewebsites.net/api/document/subir-base64", payload);
                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Error al subir el archivo {nuevo.Nombre}");

                    var contenido = await response.Content.ReadAsStringAsync();
                    var json = JsonDocument.Parse(contenido);
                    var root = json.RootElement;

                    var rutaArchivo = root.GetProperty("documento").GetString();
                    var rutaVector = root.GetProperty("vector").GetString();

                    rutasArchivos.Add(new ListaArchivosEdit
                    {
                        Id = indice,
                        Nombre = nuevo.Nombre,
                        Ruta = rutaArchivo
                    });

                    rutasVectores.Add(rutaVector ?? "");
                    indice++;
                }

                return await vgDataAccess.EditarArchivoPiloto(
                    archivo.IdPiloto,
                    archivo.Usuario,
                    archivo.Titulo ?? "",
                    archivo.Temas ?? "",
                    rutasArchivos,
                    rutasVectores,
                    archivo.IdsArchivosEliminados
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en handler: {ex.Message}", ex);
            }
        }

    }
}
