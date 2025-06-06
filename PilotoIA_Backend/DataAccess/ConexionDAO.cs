using System.Data.SqlClient;
using System.Threading.Tasks;
using System;

namespace PilotoIA_Backend.DataAccess
{
    internal class ConexionDAO : System.ComponentModel.Component
    {
        private string xSqlRead = "";
        private string xSqlWrite = "";
        private string xSrvApps = "";
        private SqlConnection oCn = new SqlConnection();
        private System.ComponentModel.Container components = null;
        public ConexionDAO(System.ComponentModel.IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            xSrvApps = string.Empty;
        }
        public ConexionDAO()
        {
            InitializeComponent();
            xSrvApps = string.Empty;
        }
        public ConexionDAO(String pSC)
        {
            InitializeComponent();
            xSrvApps = pSC;
        }

        public async Task<SqlConnection> AbrirModoLecturaAsync()
        {
            oCn = new SqlConnection();
            xSqlRead = xSrvApps;

            oCn.ConnectionString = xSqlRead;
            try
            {
                await oCn.OpenAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return (oCn);
        }
        public async Task<SqlConnection> AbrirModoEscrituraAsync()
        {
            oCn = new SqlConnection();
            xSqlRead = xSrvApps;

            oCn.ConnectionString = xSqlWrite;
            try
            {
                await oCn.OpenAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return (oCn);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
