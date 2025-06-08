    using PilotoIA_Backend.BusinessLogic;
    using PilotoIA_Backend.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Routing;
    namespace PilotoIA_Backend.Controllers
    { 
        [Route("api/[controller]")]
        [ApiController]
        public class ArchivoPilotoController : ControllerBase
        {

            private ArchivoPilotoHandler vgDataAccess;
            private beMySettings vgSettings;
            private readonly IConfiguration _configuration;
            private readonly ILogger<ArchivoPilotoController> _logger;

            public ArchivoPilotoController(IOptions<beMySettings> peSettings, IConfiguration configuration, ILogger<ArchivoPilotoController> logger, IWebHostEnvironment env)
            {
                vgSettings = peSettings.Value;
                _logger = logger;
                vgDataAccess = new ArchivoPilotoHandler(vgSettings, env);
                _configuration = configuration;
            }

        [HttpPost("registrar")]
        public IActionResult RegistrarArchivoPiloto([FromBody] ArchivoPiloto archivo)
            {
                try
                {
                    var resultado = vgDataAccess.RegistrarArchivoPilotoAsync(
                        archivo.Titulo,
                        archivo.Temas,
                        archivo.Archivos
                        ).Result;

                    if (resultado != null)
                    {
                        _logger.LogInformation("Archivo Registrado");
                        return Ok(new { resultado.Mensaje, IdTipoMensaje = resultado.IdTipoMensaje });
                    }

                    return BadRequest("No se pudo registrar el archivo.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return StatusCode(500, new { IdTipoMensaje = 1, Message = ex.Message });
                }
            }

        [HttpGet("listar")]
        public IActionResult ListarArchivoPiloto([FromQuery] string? Filtro, [FromQuery] int TamanioPagina, [FromQuery] int NumeroPagina)
        {
            try
            {
                ListaArchivoPilotoRespuesta Result = vgDataAccess.ListarArchivoPilotoAsync(
                    Filtro,
                    TamanioPagina,
                    NumeroPagina).Result;

                if (Result == null)
                {
                    return Ok(new { mensaje = "No se encontraron registros", IdTipoMensaje = 1 });
                }


                return Ok(new { Result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { IdTipoMensaje = 1, Message = ex.Message });
            }
        }

        [HttpPost("editar")]
        public IActionResult EditarArchivoPiloto([FromBody] EditarArchivoPiloto archivo)
        {
            try
            {
                var resultado = vgDataAccess.EditarArchivoPilotoAsync(
                    archivo.IdPiloto,
                    archivo.Usuario,
                    archivo.Titulo,
                    archivo.Temas,
                    archivo.Archivos,
                    archivo.Vectores).Result;

                if (resultado != null)
                {
                    _logger.LogInformation("Registro Editado");
                    return Ok(new { resultado.Mensaje, IdTipoMensaje = resultado.IdTipoMensaje });
                }

                return BadRequest("No se pudo editar el archivo.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { IdTipoMensaje = 1, Message = ex.Message });
            }
        }

    }
}
