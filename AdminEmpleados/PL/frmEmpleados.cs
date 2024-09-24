using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdminEmpleados.DAL;
using AdminEmpleados.BLL;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace AdminEmpleados.PL
{
    public partial class frmEmpleados : Form
    {
        public frmEmpleados()
        {
            InitializeComponent();
        }
        private void CargarDatos()
        {
            string connectionString = "Data Source=VMDEV01;Initial Catalog=TEST;User ID=sa;Password=adf3q9;Persist Security Info=False;";
            string query = "SELECT E.Id, E.Nombre, E.ApellidoPaterno, E.ApellidoMaterno, E.Correo, D.Departamento " +
                   "FROM Empleados E " +
                   "LEFT JOIN EmpleadoDepartamento ED ON E.Id = ED.IdEmpleado " +
                   "LEFT JOIN Departamentos D ON ED.IdDepartamento = D.Id";

            using(SqlConnection con=new SqlConnection(connectionString))
            {
                SqlDataAdapter dta = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                dta.Fill(dt);

                //Asignar los datos al DataGridView
                dgvEmpleados.DataSource = dt;
            }
        }

        private void frmEmpleados_Load(object sender, EventArgs e)
        {
            DepartamentosDAL objDepartamentos = new DepartamentosDAL();
            cbxDepartamento.DataSource = objDepartamentos.MostrarDepartamentos().Tables[0];
            cbxDepartamento.DisplayMember = "Departamento";
            cbxDepartamento.ValueMember = "Id";

            CargarDatos();
        }

        private void btnExaminar_Click(object sender, EventArgs e)
        {
            OpenFileDialog imagen = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (imagen.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //Cargar imagen en el picturebox usando Bitmap
                    picFoto.Image = new Bitmap(imagen.FileName);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error al cargar la imagen: " + ex.Message);
                }
            }
        }

        private byte[] ImageToByteArray(Image image)
        {
            using(MemoryStream ms=new MemoryStream())
            {
                //Crear una copia de la imagen para evitar problemas con archivos bloqueados
                using(Bitmap bmp=new Bitmap(image))
                {
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                }
                return ms.ToArray();
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            //Validar que los campos no esten vacios
            if(string.IsNullOrEmpty(txtNombre.Text)||
                string.IsNullOrEmpty(txtApellidoP.Text)||
                string.IsNullOrEmpty(txtApellidoM.Text)||
                string.IsNullOrEmpty(txtCorreo.Text))
            {
                MessageBox.Show("Por favor de llenar todos los capos!");
            }

            //Verifica si el campo de la foto esta vacio
            if (picFoto.Image == null)
            {
                //Muestra mensaje de advertencia
                MessageBox.Show("Debe seleccionar una foto para el empleado");
                return;
            }

            string connectionString = "Data Source=VMDEV01;Initial Catalog=TEST;User ID=sa;Password=adf3q9;Persist Security Info=False;";
            string queryEmpleado = "INSERT INTO Empleados (Nombre, ApellidoPaterno, ApellidoMaterno, Correo, Foto) " +
                       "VALUES (@Nombre, @ApellidoPaterno, @ApellidoMaterno, @Correo, @Foto); " +
                       "SELECT SCOPE_IDENTITY();";

            string queryDepartamento = "INSERT INTO EmpleadoDepartamento (IdEmpleado, IdDepartamento) " +
                           "VALUES (@EmpleadoID, @DepartamentoID)";

            using(SqlConnection con=new SqlConnection(connectionString))
            {
                con.Open();

                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    //Insertar en la tabla Empleados
                    SqlCommand comd = new SqlCommand(queryEmpleado, con, transaction);
                    comd.Parameters.AddWithValue("@Nombre", txtNombre.Text);
                    comd.Parameters.AddWithValue("@ApellidoPaterno", txtApellidoP.Text);
                    comd.Parameters.AddWithValue("@ApellidoMaterno", txtApellidoM.Text);
                    comd.Parameters.AddWithValue("@Correo", txtCorreo.Text);

                    //Convertir la imagen en el PictureBox a un arreglo de bytes
                    byte[] fotoBytes = ImageToByteArray(picFoto.Image);
                    comd.Parameters.AddWithValue("@Foto", fotoBytes);

                    //Ejecutar la consulta y obtener el ID del empleado recien insertado
                    int empleadoID = Convert.ToInt32(comd.ExecuteScalar());

                    //Insertar en la tabla EmpleadoDepartamento
                    SqlCommand comdDepartamento = new SqlCommand(queryDepartamento, con, transaction);
                    comdDepartamento.Parameters.AddWithValue("@EmpleadoID", empleadoID);

                    //Obtener el DepartamentoID del ComboBox
                    int departamentoID = Convert.ToInt32(cbxDepartamento.SelectedValue);
                    comdDepartamento.Parameters.AddWithValue("@DepartamentoID", departamentoID);

                    //Ejecutar la consulta de insercion de EmpleadoDepartamento
                    comdDepartamento.ExecuteNonQuery();

                    //Confirmar la transaccion
                    transaction.Commit();

                    MessageBox.Show("Empleado registrado correctamente");
                }
                catch(Exception ex)
                {
                    //Si se llega a tener un error se revierte la transaccion
                    transaction.Rollback();
                    MessageBox.Show("Error al registrar el empleado: " + ex.Message);
                }
            }

            CargarDatos();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=VMDEV01;Initial Catalog=TEST;User ID=sa;Password=adf3q9;Persist Security Info=False;";
            string queryEmpleado = "UPDATE Empleados " +
                       "SET Nombre = @Nombre, ApellidoPaterno = @ApellidoPaterno, ApellidoMaterno = @ApellidoMaterno, " +
                       "Correo = @Correo, Foto = @Foto " +
                       "WHERE Id = @EmpleadoID";

            string queryDepartamento = "UPDATE EmpleadoDepartamento " +
                                       "SET IdDepartamento = @DepartamentoID " +
                                       "WHERE IdEmpleado = @EmpleadoID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Iniciar una transacción para asegurar que ambas consultas se ejecuten juntas
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Actualizar la tabla Empleados
                    SqlCommand commandEmpleado = new SqlCommand(queryEmpleado, connection, transaction);
                    commandEmpleado.Parameters.AddWithValue("@Nombre", txtNombre.Text);
                    commandEmpleado.Parameters.AddWithValue("@ApellidoPaterno", txtApellidoP.Text);
                    commandEmpleado.Parameters.AddWithValue("@ApellidoMaterno", txtApellidoM.Text);
                    commandEmpleado.Parameters.AddWithValue("@Correo", txtCorreo.Text);

                    // Convertir la imagen en el PictureBox a un arreglo de bytes
                    byte[] fotoBytes = ImageToByteArray(picFoto.Image);
                    commandEmpleado.Parameters.AddWithValue("@Foto", fotoBytes);

                    int empleadoID = Convert.ToInt32(dgvEmpleados.CurrentRow.Cells["Id"].Value);
                    commandEmpleado.Parameters.AddWithValue("@EmpleadoID", empleadoID);

                    commandEmpleado.ExecuteNonQuery();

                    // Actualizar la tabla EmpleadoDepartamento
                    SqlCommand commandDepartamento = new SqlCommand(queryDepartamento, connection, transaction);
                    int departamentoID = Convert.ToInt32(cbxDepartamento.SelectedValue);
                    commandDepartamento.Parameters.AddWithValue("@DepartamentoID", departamentoID);
                    commandDepartamento.Parameters.AddWithValue("@EmpleadoID", empleadoID);

                    commandDepartamento.ExecuteNonQuery();

                    // Confirmar la transacción
                    transaction.Commit();

                    MessageBox.Show("Datos actualizados correctamente");
                }
                catch (Exception ex)
                {
                    // En caso de error, deshacer la transacción
                    transaction.Rollback();
                    MessageBox.Show("Error al actualizar: " + ex.Message);
                }
            }

            // Refrescar el DataGridView para mostrar los cambios
            CargarDatos();
        }

        private void dgvEmpleados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvEmpleados.Rows[e.RowIndex];

                //Aqui se llenan los Textbox con los datos del empleado seleccionado
                txtNombre.Text = row.Cells["Nombre"].Value.ToString();
                txtApellidoP.Text = row.Cells["ApellidoPaterno"].Value.ToString();
                txtApellidoM.Text = row.Cells["ApellidoMaterno"].Value.ToString();
                txtCorreo.Text = row.Cells["Correo"].Value.ToString();

                //Seleccionar el departamento en el combobox
                string nombreDepartamento = row.Cells["Departamento"].Value.ToString();
                cbxDepartamento.SelectedIndex = cbxDepartamento.FindStringExact(nombreDepartamento);

                //Mostrar la imagen
                int empleadoID = Convert.ToInt32(row.Cells["Id"].Value);
                CargarFoto(empleadoID);
            }
        }

        private void CargarFoto(int empleadoID)
        {
            string connectionString = "Data Source=VMDEV01;Initial Catalog=TEST;User ID=sa;Password=adf3q9;Persist Security Info=False;";
            string query = "SELECT Foto FROM Empleados WHERE Id=@EmpleadoID";

            using(SqlConnection con=new SqlConnection(connectionString))
            {
                SqlCommand comd = new SqlCommand(query, con);
                comd.Parameters.AddWithValue("@EmpleadoID", empleadoID);
                con.Open();

                byte[] fotoBytes = comd.ExecuteScalar() as byte[];
                if (fotoBytes != null)
                {
                    using(MemoryStream ms=new MemoryStream(fotoBytes))
                    {
                        picFoto.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    picFoto.Image = null;
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            //Verifica si hay un empleado seleccionado en el DataGridView
            if (dgvEmpleados.SelectedRows.Count > 0)
            {
                //Obtiene el ID seleccionado
                int empleadoID = Convert.ToInt32(dgvEmpleados.SelectedRows[0].Cells["Id"].Value);
                //Confirmar con el usuario si realmente quiere eliminar al empleado
                DialogResult result = MessageBox.Show("¿Esta seguro de que desea eliminar este empleado?", "Confirmar eliminacion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    //Si el usuario confirma, procede eliminacion
                    try
                    {
                        string connectionString = "Data Source=VMDEV01;Initial Catalog=TEST;User ID=sa;Password=adf3q9;Persist Security Info=False;";
                        using(SqlConnection con=new SqlConnection(connectionString))
                        {
                            con.Open();

                            //Crear una transaccion para asegurar la consistencia
                            SqlTransaction transaction = con.BeginTransaction();
                            try
                            {
                                ////Eliminar primero la relacion en la tabla EmpleadoDepartamento
                                string queryEmpleadoDepartamento = "DELETE FROM EmpleadoDepartamento WHERE IdEmpleado = @EDepartamentoID";
                                SqlCommand comdDepartamento = new SqlCommand(queryEmpleadoDepartamento, con, transaction);
                                comdDepartamento.Parameters.AddWithValue("@EDepartamentoID", empleadoID);
                                comdDepartamento.ExecuteNonQuery();

                                //Eliminar el empleado de la tabla Empleados
                                string queryEmpleado = "DELETE FROM Empleados WHERE Id=@EmpleadoID";
                                SqlCommand comdEmpleado = new SqlCommand(queryEmpleado, con, transaction);
                                comdEmpleado.Parameters.AddWithValue("@EmpleadoID", empleadoID);
                                int filasAfectadas = comdEmpleado.ExecuteNonQuery();

                                //Si se elimino correctamente, confirmar la transaccion
                                if (filasAfectadas > 0)
                                {
                                    transaction.Commit();
                                    MessageBox.Show("Empleado eliminado correctamente");

                                    //Refrescar el DataGrid
                                    CargarDatos();
                                }
                                else
                                {
                                    //Si no se elimina se revierte la transaccion
                                    transaction.Rollback();
                                    MessageBox.Show("Error: No se pudo eliminar el empleado");
                                }
                            }
                            catch(Exception ex)
                            {
                                //Se revierte transaccion en caso de error
                                transaction.Rollback();
                                MessageBox.Show("Error al eliminar el empleado: " + ex.Message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al conectar a la base de datos: " + ex.Message);
                    }
                }
                else
                {
                    //Si no hay un empleado seleccionado
                    MessageBox.Show("Seleccione un empleado para eliminar");
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            //Cierra el formulario actual
            this.Close();
            //Crea una instancia de la pantalla principal
            Form1 principal = new Form1();
            //Muestra la pantalla principal
            principal.Show();
        }
    }
}
