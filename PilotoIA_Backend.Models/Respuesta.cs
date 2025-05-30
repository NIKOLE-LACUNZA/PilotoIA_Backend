using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PilotoIA_Backend.Models
{
    public class MensajeRespuesta
    {
        public int IdTipoMensaje {  get; set; }
        public int IdMensaje { get; set; }
        public string Mensaje { get; set; }
    }
}
