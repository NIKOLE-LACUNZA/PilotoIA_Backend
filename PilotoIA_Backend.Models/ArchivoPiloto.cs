using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PilotoIA_Backend.Models
{
    public class ArchivoPiloto
    {
        public string Titulo {  get; set; }
        public string Temas { get; set; }
        public List<string> Archivos { get; set; }
        public int Estado {  get; set; }
    }

    public class EditarArchivoPiloto
    {        
        public string Usuario { get; set; }
        public int IdPiloto { get; set; }
        public string? Titulo { get; set; }
        public string? Temas { get; set; }
        public List<string>? Archivos { get; set; }
        public List<string>? Vectores { get; set; }
    }
    public class ListarArchivoPilotoDTO
    {
        public int TamanioPagina { get; set; }
        public int NumeroPagina { get; set; }
    }
    public class ListaArchivoPiloto
    {
        public int IdPiloto {  get; set; }
        public string Titulo { get; set; }
        public string Temas { get; set; }
        public string Archivos { get; set; }

        public string Vectores { get; set; }
        public string Estado { get; set; }
    }
    public class ListaArchivoPilotoRespuesta
    {
        public List<ListaArchivoPiloto> Lista { get; set; }
        public int TotalFilas { get; set; }
    }
}
