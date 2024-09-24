using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminEmpleados.BLL;
using System.Data;
using System.Data.SqlClient;

namespace AdminEmpleados.DAL
{
    class EmpleadosDAL
    {
        ConexionDAL conexion;

        public EmpleadosDAL()
        {
            conexion = new ConexionDAL();
        }
    }
}
