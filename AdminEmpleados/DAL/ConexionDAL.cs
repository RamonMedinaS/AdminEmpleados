using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace AdminEmpleados.DAL
{
    class ConexionDAL
    {
        private string CadenaConexion = "Data Source=VMDEV01;Initial Catalog=TEST;User ID=sa;Password=adf3q9;Persist Security Info=False;";
        SqlConnection Conexion;

        public SqlConnection EstablecerConexion()
        {
            this.Conexion = new SqlConnection(this.CadenaConexion);
            return this.Conexion;
        }
        public bool EjecutarComandosSinRetorno(string strComando)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = strComando;
                cmd.Connection = this.EstablecerConexion();
                Conexion.Open();
                cmd.ExecuteNonQuery();
                Conexion.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool EjecutarComandosSinRetorno(SqlCommand SQLcomando)
        {
            try
            {
                SqlCommand Comando = SQLcomando;
                Comando.Connection = this.EstablecerConexion();
                Conexion.Open();
                Comando.ExecuteNonQuery();
                Conexion.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public DataSet EjecutarSentencia(SqlCommand sqlComando)
        {
            DataSet DS = new DataSet();
            SqlDataAdapter Adaptador = new SqlDataAdapter();

            try
            {
                SqlCommand Comando = new SqlCommand();
                Comando = sqlComando;
                Comando.Connection = EstablecerConexion();
                Adaptador.SelectCommand = Comando;
                Conexion.Open();
                Adaptador.Fill(DS);
                Conexion.Close();
                return DS;
            }
            catch
            {
                return DS;
            }
        }
    }
}
