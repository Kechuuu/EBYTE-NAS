using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using Newtonsoft.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace EBYTE_NAS
{

    public partial class Fire_base : KryptonForm
    {
        
        public Fire_base()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(10, 10);
            tx_SearchMac.Text = "11 24 B3 11 52 ";

        }
        private string MacID;
        private string NumeroIdentificacion;
        public string NumeroIdentificacion1 { get => NumeroIdentificacion; set => NumeroIdentificacion = value; }
        public string MacID1 { get => this.MacID; set => this.MacID = value; }
        public Fire_base Fire_BaseInstance { get => fire_BaseInstance; set => fire_BaseInstance = value; }

        private Fire_base fire_BaseInstance;
        IFirebaseClient client;
        private void Conexion()
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "bMGJw8A2tp0MCkH5mcKWCe90QsX5OKq3qVHWwsEv",
                BasePath = "https://fir-ebyte-default-rtdb.firebaseio.com/"

            };
            client = new FireSharp.FirebaseClient(config);
        }

        private void Fire_base_Load(object sender, EventArgs e)
        {
            Form1 call_Form1 = new Form1();
            call_Form1.Update_table = true;
            string Json_data = MacID1;
            Dictionary<string, JObject> data = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(Json_data);
            UpdateDataGridView(data);
        }
        public void UpdateDataGridView(Dictionary<string, JObject> data)
        {
            // Convertir el Dictionary a una lista de objetos para trabajar más fácilmente con LINQ
            var dataList = data.Select(item => new
            {
                ID = item.Key,
                Fecha = Convert.ToDateTime(item.Value["FECHA"].ToString()),
                Type = item.Value["Type"].ToString()
            }).ToList();

            // Ordenar la lista por la Fecha de manera descendente (puedes usar OrderBy para ascendente)
            dataList = dataList.OrderByDescending(item => item.ID).ToList();

            // Asignar la lista ordenada como fuente de datos del DataGridView
            Table_Mac.DataSource = dataList;
        }


        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            Actualizar();
            this.Show();
        }


        private void Actualizar()
        {
            try
            {
                Conexion();
                if (client != null)
                {
                    FirebaseResponse response = client.Get("Mac_address");
                    if (response.Body != "null")
                    {
                        MacID1 = response.Body;
                    }
                }
            }
            catch (FireSharp.Exceptions.FirebaseException ex)
            {
                // Manejar el error específico de Firebase
                MessageBox.Show("Error de Firebase: " + ex.Message);

            }

            string Json_data = MacID1;
            Dictionary<string, JObject> data = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(Json_data);
            UpdateDataGridView(data);
        }

        private void Clear_Table()
        {
            Form1 call_Form1 = new Form1();
            if (Table_Mac.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = Table_Mac.SelectedRows[0];
                // Aquí puedes trabajar con la fila seleccionada, por ejemplo, obtener sus datos:
                string cellValue = selectedRow.Cells["ID"].Value.ToString();
                IFirebaseConfig config = new FirebaseConfig
                {
                    AuthSecret = "bMGJw8A2tp0MCkH5mcKWCe90QsX5OKq3qVHWwsEv",
                    BasePath = "https://fir-ebyte-default-rtdb.firebaseio.com/"
                };

                IFirebaseClient client = new FireSharp.FirebaseClient(config);

                if (client != null)
                {
                    // Realizar la operación de eliminación en la base de datos
                    FirebaseResponse response = client.Delete("Mac_address/" + cellValue);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Nodo eliminado correctamente " + "*** "+cellValue+" ***");
                    }
                    else
                    {
                        MessageBox.Show("Error al eliminar el nodo");
                    }
                }
                else
                {
                    MessageBox.Show("Error al conectar a Firebase Realtime Database");
                }

                // Realizar otras acciones según la fila seleccionada...
            }
            
            // timer_getData.Start();

            //   call_Form1.btn_Show_table_Click(btn_clear, EventArgs.Empty);
        }
        private void btn_clear_Click(object sender, EventArgs e)
        {
            Clear_Table();
            Actualizar();
            this.Show();

        }

        private void Table_Mac_SelectionChanged(object sender, EventArgs e)
        {
            
        }

        private void Fire_base_FormClosed(object sender, FormClosedEventArgs e)
        {
        }


        private void tx_SearchMac_TextChange(object sender, EventArgs e)
        {
            try
            {
                string searchText = tx_SearchMac.Text;
                if (!string.IsNullOrEmpty(searchText))
                {
                    DataGridViewRow targetRow = null;

                    // Itera a través de todas las filas del DataGridView
                    foreach (DataGridViewRow row in Table_Mac.Rows)
                    {
                        // Verificar si el valor de la celda en la columna "Nombre" coincide con el texto de búsqueda
                        if (row.Cells["ID"].Value != null && row.Cells["ID"].Value.ToString().Equals(searchText, StringComparison.OrdinalIgnoreCase))
                        {
                            targetRow = row;
                            break; // Rompe el bucle una vez que se encuentra la primera coincidencia
                        }
                    }

                    if (targetRow != null)
                    {
                        lb_search.Text = "✓";
                        // Se encontró la fila, seleccionarla
                        Table_Mac.ClearSelection();
                        targetRow.Selected = true;

                        // Ajustar el scroll para mostrar la fila seleccionada en el centro
                        int targetRowIndex = Table_Mac.Rows.IndexOf(targetRow);
                        int visibleRowCount = Table_Mac.DisplayedRowCount(false);
                        int firstVisibleRowIndex = Table_Mac.FirstDisplayedScrollingRowIndex;

                        if (targetRowIndex < firstVisibleRowIndex || targetRowIndex >= firstVisibleRowIndex + visibleRowCount)
                        {
                            // La fila está fuera de la vista actual, centrarla en la vista
                            int scrollToIndex = Math.Max(0, targetRowIndex - (visibleRowCount / 2));
                            Table_Mac.FirstDisplayedScrollingRowIndex = scrollToIndex;
                        }
                    }
                    else
                    {
                        // No se encontró la fila, mostrar mensaje de error o realizar otra acción deseada
                        if (tx_SearchMac.Text.Length >= 23)
                        {
                            lb_search.Text = "Not found";
                        }
                      //  MessageBox.Show("No se encontró ninguna coincidencia.");
                    }
                }
                else
                {
                    // Si el cuadro de búsqueda está vacío, deseleccionar cualquier fila seleccionada
                    Table_Mac.ClearSelection();
                }
            }
            catch (Exception EX)
            {

                MessageBox.Show(EX.Message);
            }
            
        }
    }
}
