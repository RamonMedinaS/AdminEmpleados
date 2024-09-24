using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdminEmpleados.BLL;
using AdminEmpleados.DAL;

namespace AdminEmpleados.PL
{
    public partial class frmDepartamentos : Form
    {
        DepartamentosDAL oDepartmentosDAL;
        public frmDepartamentos()
        {
            oDepartmentosDAL = new DepartamentosDAL();
            InitializeComponent();
            LLegarGrid();
            LimpiarEntradas();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            oDepartmentosDAL.Agregar(RecuperarInformacion());
            LLegarGrid();
            LimpiarEntradas();
        }

        private DepartamentoBLL RecuperarInformacion()
        {
            DepartamentoBLL oDepartamentoBLL = new DepartamentoBLL();
            int Id = 0; int.TryParse(txtID.Text, out Id);
            oDepartamentoBLL.Id = Id;
            oDepartamentoBLL.Departamento = txtNombreDep.Text;
            return oDepartamentoBLL;
        }

        private void Seleccionar(object sender, DataGridViewCellMouseEventArgs e)
        {
            int indice = e.RowIndex;
            dgvDepartamento.ClearSelection();
            if (indice >= 0)
            {
                txtID.Text = dgvDepartamento.Rows[indice].Cells[0].Value.ToString();
                txtNombreDep.Text = dgvDepartamento.Rows[indice].Cells[1].Value.ToString();

                btnAgregar.Enabled = false;
                btnModificar.Enabled = true;
                btnEliminar.Enabled = true;
                btnCancelar.Enabled = true;
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            oDepartmentosDAL.Eliminar(RecuperarInformacion());
            LLegarGrid();
            LimpiarEntradas();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            oDepartmentosDAL.Modificar(RecuperarInformacion());
            LLegarGrid();
            LimpiarEntradas();
        }

        public void LLegarGrid()
        {
            dgvDepartamento.DataSource = oDepartmentosDAL.MostrarDepartamentos().Tables[0];
            dgvDepartamento.Columns[0].HeaderText = "ID";
            dgvDepartamento.Columns[1].HeaderText = "Nombre Departamento";
        }

        public void LimpiarEntradas()
        {
            txtID.Text = "";
            txtNombreDep.Text = "";

            btnAgregar.Enabled = true;
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;
            btnCancelar.Enabled = false;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarEntradas();
        }
    }
}
