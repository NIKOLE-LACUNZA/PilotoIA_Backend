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
            List<RutaArchivo> RutasArchivos,
            List<string> RutasVectores
            )
        {
            var result = new MensajeRespuesta();
            int indice = 0;

            try
            {
                DataTable lstRutasArchivos = new DataTable();
                lstRutasArchivos.Columns.Add("Indice", typeof(int));
                lstRutasArchivos.Columns.Add("Nombre", typeof(string));
                lstRutasArchivos.Columns.Add("Ruta", typeof(string));

                foreach (var archivo in RutasArchivos)
                {
                    lstRutasArchivos.Rows.Add(indice, archivo.Nombre, archivo.Ruta);
                    indice++;
                }

                indice = 0;

                DataTable lstRutasVectores = new DataTable();
                lstRutasVectores.Columns.Add("Indice", typeof(int));
                lstRutasVectores.Columns.Add("Ruta", typeof(string));

                foreach (string item in RutasVectores)
                {
                    lstRutasVectores.Rows.Add(indice, item);
                    indice++;
                }

                using (SqlCommand oCmC = new SqlCommand())
                {
                    oCmC.CommandType = CommandType.StoredProcedure;
                    oCmC.CommandText = "Piloto_INS";
                    SqlParameter lstRutaArchivosParam = new SqlParameter("@tblRutasArchivo", SqlDbType.Structured);
                    lstRutaArchivosParam.Value = lstRutasArchivos;
                    lstRutaArchivosParam.TypeName = "dbo.RutaArchivoLST";
                    SqlParameter lstRutaVectoresParam = new SqlParameter("@tblRutasVector", SqlDbType.Structured);
                    lstRutaVectoresParam.Value = lstRutasVectores;
                    lstRutaVectoresParam.TypeName = "dbo.RutaVectorLST";

                    oCmC.Parameters.AddWithValue("@vchTitulo", Titulo);
                    oCmC.Parameters.AddWithValue("@vchTemas", Tema);
                    oCmC.Parameters.Add(lstRutaArchivosParam);
                    oCmC.Parameters.Add(lstRutaVectoresParam);

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
            string Filtro,
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
                    oCmC.CommandText = "Piloto_FLT";
                    oCmC.Parameters.AddWithValue("@vchfiltro", Filtro);
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
                                Vectores = oSqlR["Vectores"] != DBNull.Value ? Convert.ToString(oSqlR["Vectores"]) : string.Empty,
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

        public async Task<MensajeRespuesta> EditarArchivoPiloto(
            int IdPiloto,
            string Usuario,
            string Titulo,
            string Tema,
            List<ListaArchivosEdit> RutasArchivos,
            List<string> RutasVectores,
            List<int> IdsEliminados
        )
        {
            var result = new MensajeRespuesta();
            int indice = 0;

            try
            {
                var lstRutasArchivos = new DataTable();
                lstRutasArchivos.Columns.Add("Indice", typeof(int));
                lstRutasArchivos.Columns.Add("Nombre", typeof(string));
                lstRutasArchivos.Columns.Add("Ruta", typeof(string));

                foreach (var a in RutasArchivos)
                {
                    lstRutasArchivos.Rows.Add(a.Id, a.Nombre, a.Ruta);
                }

                var lstRutasVectores = new DataTable();
                lstRutasVectores.Columns.Add("Indice", typeof(int));
                lstRutasVectores.Columns.Add("Ruta", typeof(string));

                foreach (var ruta in RutasVectores)
                {
                    lstRutasVectores.Rows.Add(indice++, ruta);
                }

                var lstIdsEliminados = new DataTable();
                lstIdsEliminados.Columns.Add("IdArchivo", typeof(int));
                foreach (var id in IdsEliminados)
                {
                    lstIdsEliminados.Rows.Add(id);
                }

                using var oCmC = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Piloto_UPD"
                };

                oCmC.Parameters.AddWithValue("@vchUsuMod", Usuario);
                oCmC.Parameters.AddWithValue("@intIdPiloto", IdPiloto);
                oCmC.Parameters.AddWithValue("@vchTitulo", Titulo);
                oCmC.Parameters.AddWithValue("@vchTemas", Tema);

                oCmC.Parameters.Add(new SqlParameter("@tblRutasArchivos", SqlDbType.Structured)
                {
                    TypeName = "dbo.RutaArchivoLST",
                    Value = lstRutasArchivos
                });

                oCmC.Parameters.Add(new SqlParameter("@tblRutasVectores", SqlDbType.Structured)
                {
                    TypeName = "dbo.RutaVectorLST",
                    Value = lstRutasVectores
                });

                oCmC.Parameters.Add(new SqlParameter("@tblIdsEliminados", SqlDbType.Structured)
                {
                    TypeName = "dbo.IdsEliminadosLST",
                    Value = lstIdsEliminados
                });

                oConn = await vgBDConeccion.AbrirModoLecturaAsync();
                oTran = oConn.BeginTransaction();
                oCmC.Connection = oConn;
                oCmC.Transaction = oTran;

                using var reader = await oCmC.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Mensaje = reader["Mensaje"]?.ToString() ?? "";
                    result.IdMensaje = reader["IdMensaje"] != DBNull.Value ? Convert.ToInt32(reader["IdMensaje"]) : 0;
                    result.IdTipoMensaje = reader["TipoMensaje"] != DBNull.Value ? Convert.ToInt32(reader["TipoMensaje"]) : 0;
                }

                oTran.Commit();
            }
            catch (Exception ex)
            {
                if (oTran != null) await Task.Run(() => oTran.Rollback());
                throw new Exception($"Error en DAO: {ex.Message}");
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