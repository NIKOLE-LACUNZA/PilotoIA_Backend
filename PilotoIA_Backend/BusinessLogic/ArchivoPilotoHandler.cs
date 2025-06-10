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
        public async Task<MensajeRespuesta> EditarArchivoPilotoAsync(
            int IdPiloto,
            string Usuario,
            string Titulo,
            string Tema,
            List<string> RutasArchivos,
            List<string> RutasVectores
            )
        {
            try
            {
                MensajeRespuesta Respuesta = null;

                Respuesta = await vgDataAccess.EditarArchivoPiloto(
                    IdPiloto,
                    Usuario,
                    Titulo,
                    Tema,
                    RutasArchivos,
                    RutasVectores);

                return Respuesta;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
