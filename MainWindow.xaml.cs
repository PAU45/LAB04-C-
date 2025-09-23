using System;
using System.Collections.ObjectModel;

using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace lab04
{
    public partial class MainWindow : Window
    {
        // Variable de conexión global
        SqlConnection sqlConnection = new SqlConnection("Data Source=LAB1502-022\\SQLEXPRESS;Initial Catalog=Neptuno;User Id=paulo; Pwd=123456; TrustServerCertificate=true");

        // Clases para almacenar los datos de las tablas
        public class Producto
        {
            public int idproducto { get; set; }
            public string nombreProducto { get; set; }
            public decimal? precioUnidad { get; set; }
        }

        public class Categoria
        {
            public int idcategoria { get; set; }
            public string nombrecategoria { get; set; }
            public string descripcion { get; set; }
        }

        public class Proveedor
        {
            public int idProveedor { get; set; }
            public string nombreCompania { get; set; }
            public string nombrecontacto { get; set; }
            public string cargocontacto { get; set; }
            public string direccion { get; set; }
            public string ciudad { get; set; }
            public string region { get; set; }
            public string codPostal { get; set; }
            public string pais { get; set; }
            public string telefono { get; set; }
            public string fax { get; set; }
            public string paginaprincipal { get; set; }
        }

        // Clase con la corrección en 'descuento'
        public class DetallePedido
        {
            public int idpedido { get; set; }
            public int idproducto { get; set; }
            public decimal preciounidad { get; set; }
            public int cantidad { get; set; }
            public decimal descuento { get; set; } // <-- Corregido a 'decimal'
            public DateTime FechaPedido { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCargarProductos_Click(object sender, RoutedEventArgs e)
        {
            CargarProductos();
        }

        private void CargarProductos()
        {
            ObservableCollection<Producto> listaProductos = new ObservableCollection<Producto>();
            try
            {
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }

                SqlCommand comando = new SqlCommand("ListadoDeProductosSinParametros", sqlConnection);
                comando.CommandType = CommandType.StoredProcedure;

                SqlDataReader lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    listaProductos.Add(new Producto
                    {
                        idproducto = lector.GetInt32(lector.GetOrdinal("idproducto")),
                        nombreProducto = lector.IsDBNull(lector.GetOrdinal("nombreProducto")) ? null : lector.GetString(lector.GetOrdinal("nombreProducto")),
                        precioUnidad = lector.IsDBNull(lector.GetOrdinal("precioUnidad")) ? (decimal?)null : lector.GetDecimal(lector.GetOrdinal("precioUnidad"))
                    });
                }

                ProductosDataGrid.ItemsSource = listaProductos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los productos: " + ex.Message);
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        private void btnCargarCategorias_Click(object sender, RoutedEventArgs e)
        {
            CargarCategorias();
        }

        private void CargarCategorias()
        {
            ObservableCollection<Categoria> listaCategorias = new ObservableCollection<Categoria>();
            try
            {
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }

                SqlCommand comando = new SqlCommand("ListadoDeCategoriasSinParametros", sqlConnection);
                comando.CommandType = CommandType.StoredProcedure;

                SqlDataReader lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    listaCategorias.Add(new Categoria
                    {
                        idcategoria = lector.GetInt32(lector.GetOrdinal("idcategoria")),
                        nombrecategoria = lector.IsDBNull(lector.GetOrdinal("nombrecategoria")) ? null : lector.GetString(lector.GetOrdinal("nombrecategoria")),
                        descripcion = lector.IsDBNull(lector.GetOrdinal("descripcion")) ? null : lector.GetString(lector.GetOrdinal("descripcion"))
                    });
                }

                CategoriasDataGrid.ItemsSource = listaCategorias;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las categorías: " + ex.Message);
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        private void btnBuscarProveedores_Click(object sender, RoutedEventArgs e)
        {
            CargarProveedores();
        }

        private void CargarProveedores()
        {
            ObservableCollection<Proveedor> listaProveedores = new ObservableCollection<Proveedor>();
            try
            {
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }

                SqlCommand comando = new SqlCommand("ListadoDeProveedoresPorNombreyCiudad", sqlConnection);
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@nombrecontacto", txtNombreContacto.Text);
                comando.Parameters.AddWithValue("@ciudad", txtCiudad.Text);

                SqlDataReader lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    listaProveedores.Add(new Proveedor
                    {
                        idProveedor = lector.GetInt32(lector.GetOrdinal("idProveedor")),
                        nombreCompania = lector.GetString(lector.GetOrdinal("nombreCompañia")),
                        nombrecontacto = lector.GetString(lector.GetOrdinal("nombrecontacto")),
                        cargocontacto = lector.GetString(lector.GetOrdinal("cargocontacto")),
                        direccion = lector.GetString(lector.GetOrdinal("direccion")),
                        ciudad = lector.GetString(lector.GetOrdinal("ciudad")),
                        region = lector.IsDBNull(lector.GetOrdinal("region")) ? null : lector.GetString(lector.GetOrdinal("region")),
                        codPostal = lector.IsDBNull(lector.GetOrdinal("codPostal")) ? null : lector.GetString(lector.GetOrdinal("codPostal")),
                        pais = lector.GetString(lector.GetOrdinal("pais")),
                        telefono = lector.GetString(lector.GetOrdinal("telefono")),
                        fax = lector.IsDBNull(lector.GetOrdinal("fax")) ? null : lector.GetString(lector.GetOrdinal("fax")),
                        paginaprincipal = lector.IsDBNull(lector.GetOrdinal("paginaprincipal")) ? null : lector.GetString(lector.GetOrdinal("paginaprincipal"))
                    });
                }

                ProveedoresDataGrid.ItemsSource = listaProveedores;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar proveedores: " + ex.Message);
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        private void btnBuscarPedidos_Click(object sender, RoutedEventArgs e)
        {
            CargarDetallesPedidos();
        }

        private void CargarDetallesPedidos()
        {
            ObservableCollection<DetallePedido> listaPedidos = new ObservableCollection<DetallePedido>();
            try
            {
                if (dpFechaInicio.SelectedDate == null || dpFechaFin.SelectedDate == null)
                {
                    MessageBox.Show("Por favor, seleccione un rango de fechas.");
                    return;
                }

                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }

                SqlCommand comando = new SqlCommand("ListadoDeDetallesDePedidosPorFecha", sqlConnection);
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@fechaInicio", dpFechaInicio.SelectedDate);
                comando.Parameters.AddWithValue("@fechaFin", dpFechaFin.SelectedDate);

                SqlDataReader lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    listaPedidos.Add(new DetallePedido
                    {
                        idpedido = lector.GetInt32(lector.GetOrdinal("idpedido")),
                        idproducto = lector.GetInt32(lector.GetOrdinal("idproducto")),
                        preciounidad = lector.GetDecimal(lector.GetOrdinal("preciounidad")),
                        cantidad = lector.GetInt32(lector.GetOrdinal("cantidad")),
                        descuento = lector.GetDecimal(lector.GetOrdinal("descuento")), // <-- Corregido para usar GetDecimal
                        FechaPedido = lector.GetDateTime(lector.GetOrdinal("FechaPedido"))
                    });
                }

                PedidosDataGrid.ItemsSource = listaPedidos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar detalles de pedidos: " + ex.Message);
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }
    }
}