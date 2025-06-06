using PilotoIA_Backend.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace PilotoIA_Backend.DataAccess
{
    public class CronJobDAO
    {
        private ConexionDAO vgBDConeccion;
        private SqlConnection oConn = new SqlConnection();
        private SqlTransaction oTran = null;
        private readonly IConfiguration _configuration;
        public CronJobDAO(string peDbConnection)
        {
            vgBDConeccion = new ConexionDAO(peDbConnection);
        }

        public async Task<MensajeRespuesta> ActualizarEstadosAsync()
        {
            var result = new MensajeRespuesta();

            try
            {
                using (SqlCommand oCmC = new SqlCommand())
                {
                    oCmC.CommandType = CommandType.StoredProcedure;
                    oCmC.CommandText = "ActualizarEstados";

                    oConn = await vgBDConeccion.AbrirModoLecturaAsync();
                    oTran = await Task.Run<SqlTransaction>(() => oConn.BeginTransaction());
                    oCmC.Connection = oTran.Connection;
                    oCmC.Transaction = oTran;
                    using (SqlDataReader oSqlR = await oCmC.ExecuteReaderAsync())
                    {
                        while (await oSqlR.ReadAsync())
                        {
                            result = new MensajeRespuesta()
                            {
                                Mensaje = oSqlR["Mensaje"] != DBNull.Value ? Convert.ToString(oSqlR["Mensaje"]) : string.Empty,
                                IdTipoMensaje = oSqlR["TipoMensaje"] != DBNull.Value ? Convert.ToInt32(oSqlR["TipoMensaje"]) : 0,
                            };
                        }
                    }
                    oTran.Commit();
                }
            }
            catch (Exception ex)
            {
                //Contrucción de Salida
                if (oTran != null)
                {
                    await Task.Run(() => oTran.Rollback());
                }
                throw new Exception(ex.Message);
            }
            finally
            {
                if (oTran != null)
                {
                    await oTran.DisposeAsync();
                    await oConn.DisposeAsync();
                    vgBDConeccion.Dispose();
                }
            }
            return result;
        }
    }
}
