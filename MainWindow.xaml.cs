using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace lab04
{
    public partial class MainWindow : Window
    {
        private readonly string connectionString;
        private readonly SqlConnection sqlConnection;
        private bool esNuevoProducto = true;
        private Producto productoSeleccionado;

        public class Producto
        {
            public int idproducto { get; set; }
            public string nombreProducto { get; set; }
            public int? idProveedor { get; set; }
            public int? idCategoria { get; set; }
            public string cantidadPorUnidad { get; set; }
            public decimal? precioUnidad { get; set; }
            public short? unidadesEnExistencia { get; set; }
            public short? unidadesEnPedido { get; set; }
            public short? nivelNuevoPedido { get; set; }
            public short? suspendido { get; set; }
            public string categoriaProducto { get; set; }
            public bool estado { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["NeptunoConnection"].ConnectionString;
                sqlConnection = new SqlConnection(connectionString);
                CargarProductosActivos(); // Cargar productos activos al iniciar
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message);
            }
        }

        private void CargarProductosActivos()
        {
            CargarProductos("ListadoDeProductosActivos");
            ActualizarBotones();
        }

        private void CargarProductos(string procedimiento)
        {
            try
            {
                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();

                SqlCommand comando = new SqlCommand(procedimiento, sqlConnection);
                comando.CommandType = CommandType.StoredProcedure;

                SqlDataReader lector = comando.ExecuteReader();
                ObservableCollection<Producto> listaProductos = new ObservableCollection<Producto>();

                while (lector.Read())
                {
                    listaProductos.Add(new Producto
                    {
                        idproducto = lector.GetInt32(lector.GetOrdinal("idproducto")),
                        nombreProducto = lector.IsDBNull(lector.GetOrdinal("nombreProducto")) ? null : lector.GetString(lector.GetOrdinal("nombreProducto")),
                        precioUnidad = lector.IsDBNull(lector.GetOrdinal("precioUnidad")) ? (decimal?)null : lector.GetDecimal(lector.GetOrdinal("precioUnidad")),
                        unidadesEnExistencia = lector.IsDBNull(lector.GetOrdinal("unidadesEnExistencia")) ? (short?)null : lector.GetInt16(lector.GetOrdinal("unidadesEnExistencia")),
                        cantidadPorUnidad = lector.IsDBNull(lector.GetOrdinal("cantidadPorUnidad")) ? null : lector.GetString(lector.GetOrdinal("cantidadPorUnidad")),
                        nivelNuevoPedido = lector.IsDBNull(lector.GetOrdinal("nivelNuevoPedido")) ? (short?)null : lector.GetInt16(lector.GetOrdinal("nivelNuevoPedido")),
                        estado = lector.GetBoolean(lector.GetOrdinal("estado"))
                    });
                }

                ProductosDataGrid.ItemsSource = listaProductos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message);
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                    sqlConnection.Close();
            }
        }

        private void ActualizarBotones()
        {
            btnEditarProducto.IsEnabled = true;
            btnNuevoProducto.IsEnabled = true;
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var producto = ProductosDataGrid.SelectedItem as Producto;
            if (producto == null)
            {
                MessageBox.Show("Por favor, seleccione un producto para eliminar.");
                return;
            }

            if (MessageBox.Show("¿Está seguro que desea eliminar este producto?", 
                "Confirmar eliminación", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                EliminarProducto(producto.idproducto);
            }
        }

        private void EliminarProducto(int idProducto)
        {
            try
            {
                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();

                SqlCommand comando = new SqlCommand("USP_EliminarProducto", sqlConnection);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@idproducto", idProducto);

                comando.ExecuteNonQuery();
                MessageBox.Show("Producto eliminado correctamente");
                CargarProductos("ListadoDeProductosActivos");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el producto: " + ex.Message);
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                    sqlConnection.Close();
            }
        }

        private void btnNuevoProducto_Click(object sender, RoutedEventArgs e)
        {
            esNuevoProducto = true;
            LimpiarFormulario();
            MostrarFormulario(true);
        }

        private void btnEditarProducto_Click(object sender, RoutedEventArgs e)
        {
            var producto = ProductosDataGrid.SelectedItem as Producto;
            if (producto == null)
            {
                MessageBox.Show("Por favor, seleccione un producto para editar.");
                return;
            }

            esNuevoProducto = false;
            productoSeleccionado = producto;
            CargarProductoEnFormulario(producto);
            MostrarFormulario(true);
        }

        private void LimpiarFormulario()
        {
            txtNombreProducto.Text = string.Empty;
            txtPrecioUnitario.Text = string.Empty;
            txtUnidadesExistencia.Text = string.Empty;
            txtCantidadPorUnidad.Text = string.Empty;
            txtNivelReorden.Text = string.Empty;
        }

        private void MostrarFormulario(bool mostrar)
        {
            panelEdicion.Visibility = mostrar ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CargarProductoEnFormulario(Producto producto)
        {
            txtNombreProducto.Text = producto.nombreProducto;
            txtPrecioUnitario.Text = producto.precioUnidad?.ToString();
            txtUnidadesExistencia.Text = producto.unidadesEnExistencia?.ToString();
            txtCantidadPorUnidad.Text = producto.cantidadPorUnidad;
            txtNivelReorden.Text = producto.nivelNuevoPedido?.ToString();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreProducto.Text))
            {
                MessageBox.Show("El nombre del producto es obligatorio.");
                return;
            }

            try
            {
                var producto = new Producto
                {
                    nombreProducto = txtNombreProducto.Text,
                    precioUnidad = !string.IsNullOrEmpty(txtPrecioUnitario.Text) ? decimal.Parse(txtPrecioUnitario.Text) : (decimal?)null,
                    unidadesEnExistencia = !string.IsNullOrEmpty(txtUnidadesExistencia.Text) ? short.Parse(txtUnidadesExistencia.Text) : (short?)null,
                    cantidadPorUnidad = txtCantidadPorUnidad.Text,
                    nivelNuevoPedido = !string.IsNullOrEmpty(txtNivelReorden.Text) ? short.Parse(txtNivelReorden.Text) : (short?)null
                };

                if (esNuevoProducto)
                    InsertarProducto(producto);
                else
                {
                    producto.idproducto = productoSeleccionado.idproducto;
                    ActualizarProducto(producto);
                }

                MostrarFormulario(false);
                CargarProductosActivos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el producto: {ex.Message}");
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            MostrarFormulario(false);
        }

        private void InsertarProducto(Producto producto)
        {
            try
            {
                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();

                SqlCommand comando = new SqlCommand("USP_InsertarProducto", sqlConnection);
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@nombreProducto", producto.nombreProducto);
                comando.Parameters.AddWithValue("@cantidadPorUnidad", (object)producto.cantidadPorUnidad ?? DBNull.Value);
                comando.Parameters.AddWithValue("@precioUnidad", (object)producto.precioUnidad ?? DBNull.Value);
                comando.Parameters.AddWithValue("@unidadesEnExistencia", (object)producto.unidadesEnExistencia ?? DBNull.Value);
                comando.Parameters.AddWithValue("@nivelNuevoPedido", (object)producto.nivelNuevoPedido ?? DBNull.Value);

                comando.ExecuteNonQuery();
                MessageBox.Show("Producto insertado correctamente");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                    sqlConnection.Close();
            }
        }

        private void ActualizarProducto(Producto producto)
        {
            try
            {
                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();

                SqlCommand comando = new SqlCommand("USP_ActualizarProducto", sqlConnection);
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@idproducto", producto.idproducto);
                comando.Parameters.AddWithValue("@nombreProducto", producto.nombreProducto);
                comando.Parameters.AddWithValue("@cantidadPorUnidad", (object)producto.cantidadPorUnidad ?? DBNull.Value);
                comando.Parameters.AddWithValue("@precioUnidad", (object)producto.precioUnidad ?? DBNull.Value);
                comando.Parameters.AddWithValue("@unidadesEnExistencia", (object)producto.unidadesEnExistencia ?? DBNull.Value);
                comando.Parameters.AddWithValue("@nivelNuevoPedido", (object)producto.nivelNuevoPedido ?? DBNull.Value);

                comando.ExecuteNonQuery();
                MessageBox.Show("Producto actualizado correctamente");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                    sqlConnection.Close();
            }
        }
    }
}