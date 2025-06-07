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
            List<string> RutasArchivos,
            List<string> RutasVectores,
            int Estado
            )
        {
            try
            {
                MensajeRespuesta Respuesta = null;

                Respuesta = await vgDataAccess.RegistrarArchivoPiloto(
                    Titulo,
                    Tema,
                    RutasArchivos,
                    RutasVectores,
                    Estado);

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
