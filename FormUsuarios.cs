using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace GestionUsuario
{
    public class FormUsuarios : Form
    {
        TextBox txtNombre, txtCorreo;
        ComboBox comboBoxEstado, comboBoxRol;
        Button btnGuardar, btnEditar, btnEliminar;
        DataGridView dataGridViewUsuarios;

        int usuarioSeleccionadoId = -1;

        public FormUsuarios()
        {
            InicializarComponentes();
            CargarEstados();
            CargarRoles();
            CargarUsuarios();
        }

        private void InicializarComponentes()
        {
            this.Text = "Gestión de Usuarios";
            this.Size = new System.Drawing.Size(800, 600);

            Label lblNombre = new Label { Text = "Nombre:", Top = 20, Left = 20, Width = 100 };
            txtNombre = new TextBox { Top = 20, Left = 130, Width = 200 };

            Label lblCorreo = new Label { Text = "Correo:", Top = 60, Left = 20, Width = 100 };
            txtCorreo = new TextBox { Top = 60, Left = 130, Width = 200 };

            Label lblEstado = new Label { Text = "Estado:", Top = 100, Left = 20, Width = 100 };
            comboBoxEstado = new ComboBox { Top = 100, Left = 130, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblRol = new Label { Text = "Rol:", Top = 140, Left = 20, Width = 100 };
            comboBoxRol = new ComboBox { Top = 140, Left = 130, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

            btnGuardar = new Button { Text = "Guardar", Top = 180, Left = 130, Width = 90 };
            btnGuardar.Click += BtnGuardar_Click;

            btnEditar = new Button { Text = "Editar", Top = 180, Left = 230, Width = 90 };
            btnEditar.Click += BtnEditar_Click;

            btnEliminar = new Button { Text = "Eliminar", Top = 180, Left = 330, Width = 90 };
            btnEliminar.Click += BtnEliminar_Click;

            dataGridViewUsuarios = new DataGridView
            {
                Top = 230,
                Left = 20,
                Width = 740,
                Height = 300,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dataGridViewUsuarios.CellClick += DataGridViewUsuarios_CellClick;

            this.Controls.AddRange(new Control[] {
                lblNombre, txtNombre,
                lblCorreo, txtCorreo,
                lblEstado, comboBoxEstado,
                lblRol, comboBoxRol,
                btnGuardar, btnEditar, btnEliminar,
                dataGridViewUsuarios
            });
        }

        private void CargarEstados()
        {
            comboBoxEstado.Items.Clear();
            comboBoxEstado.Items.Add(new ComboBoxItem(1, "Activo"));
            comboBoxEstado.Items.Add(new ComboBoxItem(0, "Inactivo"));
        }

        private void CargarRoles()
        {
            comboBoxRol.Items.Clear();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id, Nombre FROM Roles", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    comboBoxRol.Items.Add(new ComboBoxItem(reader.GetInt32(0), reader.GetString(1)));
                }
                con.Close();
            }
        }

        private void CargarUsuarios()
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                SqlDataAdapter da = new SqlDataAdapter(@"
                    SELECT u.Id, u.Nombre, u.CorreoElectronico, 
                           CASE u.Estado WHEN 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado, 
                           r.Nombre AS Rol 
                    FROM Usuarios u
                    INNER JOIN Roles r ON u.RolId = r.Id", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridViewUsuarios.DataSource = dt;
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text;
            string correo = txtCorreo.Text;
            int estado = ((ComboBoxItem)comboBoxEstado.SelectedItem).Id;
            int rolId = ((ComboBoxItem)comboBoxRol.SelectedItem).Id;

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd;
                if (usuarioSeleccionadoId == -1)
                {
                    cmd = new SqlCommand("INSERT INTO Usuarios (Nombre, CorreoElectronico, Estado, RolId) VALUES (@Nombre, @Correo, @Estado, @RolId)", con);
                }
                else
                {
                    cmd = new SqlCommand("UPDATE Usuarios SET Nombre = @Nombre, CorreoElectronico = @Correo, Estado = @Estado, RolId = @RolId WHERE Id = @Id", con);
                    cmd.Parameters.AddWithValue("@Id", usuarioSeleccionadoId);
                }

                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Correo", correo);
                cmd.Parameters.AddWithValue("@Estado", estado);
                cmd.Parameters.AddWithValue("@RolId", rolId);
                cmd.ExecuteNonQuery();
                con.Close();
            }

            LimpiarCampos();
            CargarUsuarios();
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsuarios.SelectedRows.Count == 0) return;

            DataGridViewRow row = dataGridViewUsuarios.SelectedRows[0];
            usuarioSeleccionadoId = Convert.ToInt32(row.Cells["Id"].Value);
            txtNombre.Text = row.Cells["Nombre"].Value.ToString();
            txtCorreo.Text = row.Cells["CorreoElectronico"].Value.ToString();

            comboBoxEstado.SelectedIndex = comboBoxEstado.FindString(row.Cells["Estado"].Value.ToString());
            comboBoxRol.SelectedIndex = comboBoxRol.FindString(row.Cells["Rol"].Value.ToString());
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsuarios.SelectedRows.Count == 0) return;

            int id = Convert.ToInt32(dataGridViewUsuarios.SelectedRows[0].Cells["Id"].Value);
            DialogResult result = MessageBox.Show("¿Deseas eliminar este usuario?", "Confirmar", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Usuarios WHERE Id = @Id", con);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                CargarUsuarios();
                LimpiarCampos();
            }
        }

        private void DataGridViewUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridViewUsuarios.Rows[e.RowIndex].Selected = true;
            }
        }

        private void LimpiarCampos()
        {
            usuarioSeleccionadoId = -1;
            txtNombre.Text = "";
            txtCorreo.Text = "";
            comboBoxEstado.SelectedIndex = -1;
            comboBoxRol.SelectedIndex = -1;
        }
    }
}
