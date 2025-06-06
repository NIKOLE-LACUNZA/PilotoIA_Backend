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
            List<string> Rutas,
            int Estado
            )
        {
            try
            {
                MensajeRespuesta Respuesta = null;

                Respuesta = await vgDataAccess.RegistrarArchivoPiloto(
                    Titulo,
                    Tema,
                    Rutas,
                    Estado);

                return Respuesta;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ListaArchivoPilotoRespuesta> ListarArchivoPilotoAsync(
           int TamanioPagina,
           int NumeroPagina
           )
        {
            try
            {
                ListaArchivoPilotoRespuesta Respuesta = null;

                Respuesta = await vgDataAccess.ListarArchivoPiloto(
                    TamanioPagina,
                    NumeroPagina);

                return Respuesta;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
