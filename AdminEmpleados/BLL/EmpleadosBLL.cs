using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminEmpleados.BLL
{
    class EmpleadosBLL
    {
        public int Id { get; set; }
        public string NombreEmpleado { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public int Departamento { get; set; }
        public string Correo { get; set; }
        public byte[] FotoEmpleado { get; set; }
    }
}
