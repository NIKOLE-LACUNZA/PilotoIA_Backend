using PilotoIA_Backend.Models;
using System.Data;
using System.Data.SqlClient;


namespace PilotoIA_Backend.DataAccess
{
    public class ArchivoPilotoDAO
    {
        private ConexionDAO vgBDConeccion;
        private SqlConnection oConn = new SqlConnection();
        private SqlTransaction oTran = null;
        private readonly IConfiguration _configuration;

        public ArchivoPilotoDAO(string peDbConection)
        {
            vgBDConeccion = new ConexionDAO(peDbConection);
        }

        public async Task<MensajeRespuesta> RegistrarArchivoPiloto(
            string Titulo,
            string Tema,
            List<string> Rutas,
            int Estado
            )
        {
            var result = new MensajeRespuesta();
            int indice = 0;

            try
            {
                DataTable lstRutas = new DataTable();
                lstRutas.Columns.Add("Indice", typeof(int));
                lstRutas.Columns.Add("Ruta", typeof(string));

                foreach(string item in Rutas)
                {
                    lstRutas.Rows.Add(indice, item);
                    indice++;
                }
                using(SqlCommand oCmC = new SqlCommand())
                {
                    oCmC.CommandType = CommandType.StoredProcedure;
                    oCmC.CommandText = "Piloto_INS";
                    SqlParameter lstRutasParam= new SqlParameter("@tblRutas", SqlDbType.Structured);
                    lstRutasParam.Value = lstRutas;
                    lstRutasParam.TypeName = "dbo.RutaArchivoLST";

                    oCmC.Parameters.AddWithValue("@vchTitulo", Titulo);
                    oCmC.Parameters.AddWithValue("@vchTemas", Tema);
                    oCmC.Parameters.AddWithValue("@intEstado", Estado);
                    oCmC.Parameters.Add(lstRutasParam);

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
                                IdMensaje = oSqlR["IdMensaje"] != DBNull.Value ? Convert.ToInt32(oSqlR["IdMensaje"]) : 0,
                                IdTipoMensaje = oSqlR["TipoMensaje"] != DBNull.Value ? Convert.ToInt32(oSqlR["TipoMensaje"]) : 0,
                            };
                        }
                    }
                    oTran.Commit();
                }
            }
            catch (Exception ex)
            {
                // Contrucción de Salida
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

        public async Task<ListaArchivoPilotoRespuesta> ListarArchivoPiloto(
            int TamanioPagina,
            int NumeroPagina
            )
        {
            ListaArchivoPilotoRespuesta result = new ListaArchivoPilotoRespuesta();
            List<ListaArchivoPiloto> Lista = new List<ListaArchivoPiloto>();
            int TotalCuenta = 0;

            try
            {
                using (SqlCommand oCmC = new SqlCommand())
                {
                    oCmC.CommandType = CommandType.StoredProcedure;
                    oCmC.CommandText = "Piloto_LST";
                    oCmC.Parameters.AddWithValue("@intTamanioPagina", TamanioPagina);
                    oCmC.Parameters.AddWithValue("@intPagina", NumeroPagina);

                    oConn = await vgBDConeccion.AbrirModoLecturaAsync();
                    oTran = await Task.Run<SqlTransaction>(() => oConn.BeginTransaction());
                    oCmC.Connection = oTran.Connection;
                    oCmC.Transaction = oTran;

                    using (SqlDataReader oSqlR = await oCmC.ExecuteReaderAsync())
                    {
                        while (await oSqlR.ReadAsync())
                        {
                            var respuesta = new ListaArchivoPiloto()
                            {
                                IdPiloto = oSqlR["IdPiloto"] != DBNull.Value ? Convert.ToInt32(oSqlR["IdPiloto"]) : 0,
                                Titulo = oSqlR["Titulo"] != DBNull.Value ? Convert.ToString(oSqlR["Titulo"]) : string.Empty,
                                Temas = oSqlR["Temas"] != DBNull.Value ? Convert.ToString(oSqlR["Temas"]) : string.Empty,
                                Archivos = oSqlR["Archivos"] != DBNull.Value ? Convert.ToString(oSqlR["Archivos"]) : string.Empty,
                                Estado = oSqlR["Estado"] != DBNull.Value ? Convert.ToString(oSqlR["Estado"]) : string.Empty,
                            };
                            Lista.Add(respuesta);
                        }
                        if (await oSqlR.NextResultAsync())
                        {
                            if (await oSqlR.ReadAsync())
                            {
                                TotalCuenta = oSqlR["TotalFilas"] != DBNull.Value ? Convert.ToInt32(oSqlR["TotalFilas"]) : 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Contrucción de Salida
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
            result.Lista = Lista;
            result.TotalFilas = TotalCuenta;
            return result;
        }
    }
}