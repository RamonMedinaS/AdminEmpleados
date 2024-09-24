using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using AdminEmpleados.BLL;

namespace AdminEmpleados.DAL
{
    class DepartamentosDAL
    {
        ConexionDAL Conexion;

        public DepartamentosDAL()
        {
            Conexion = new ConexionDAL();
        }

        public bool Agregar(DepartamentoBLL oDepartamentosBLL)
        {
            SqlCommand SQLComando = new SqlCommand("INSERT INTO Departamentos VALUES(@Departamento)");
            SQLComando.Parameters.Add("@Departamento", SqlDbType.VarChar).Value = oDepartamentosBLL.Departamento;
            return Conexion.EjecutarComandosSinRetorno(SQLComando);

            //return Conexion.EjecutarComandosSinRetorno("INSERT INTO Departamentos (Departamento) VALUES ('"+oDepartamentosBLL.Departamento+"')");
        }
        public bool Eliminar(DepartamentoBLL oDepartamentosBLL)
        {
            SqlCommand SQLComando = new SqlCommand("DELETE FROM Departamentos WHERE Id=@Id");
            SQLComando.Parameters.Add("@Id", SqlDbType.Int).Value = oDepartamentosBLL.Id;
            return Conexion.EjecutarComandosSinRetorno(SQLComando);

            //Conexion.EjecutarComandosSinRetorno("DELETE FROM Departamentos WHERE Id='" + oDepartamentosBLL.Id + "'");
        }

        public bool Modificar(DepartamentoBLL oDepartamentosBLL)
        {
            SqlCommand SQLComando = new SqlCommand("UPDATE Departamentos SET Departamento=@Departamento WHERE Id=@Id");
            SQLComando.Parameters.Add("@Departamento", SqlDbType.VarChar).Value = oDepartamentosBLL.Departamento;
            SQLComando.Parameters.Add("@Id", SqlDbType.Int).Value = oDepartamentosBLL.Id;
            return Conexion.EjecutarComandosSinRetorno(SQLComando);
        }
        public DataSet MostrarDepartamentos()
        {
            SqlCommand sentencia = new SqlCommand("SELECT * FROM Departamentos");
            return Conexion.EjecutarSentencia(sentencia);
        }
    }
}
