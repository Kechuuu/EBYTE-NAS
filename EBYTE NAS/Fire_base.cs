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
        private string MacID;
        private string NumeroIdentificacion;
        public Fire_base()
        {
            InitializeComponent();

        }

        public string NumeroIdentificacion1 { get => NumeroIdentificacion; set => NumeroIdentificacion = value; }
        public string MacID1 { get => this.MacID; set => this.MacID = value; }

        private void Fire_base_Load(object sender, EventArgs e)
        {
            
            try
            {
                string Json_data = MacID1;
                Dictionary<string, JObject> data = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(Json_data);

                // Convertir el Dictionary a una lista de objetos para trabajar más fácilmente con LINQ
                var dataList = data.Select(item => new
                {
                    ID = item.Key,
                    Fecha = Convert.ToDateTime(item.Value["FECHA"].ToString()),
                    Type = item.Value["Type"].ToString()
                }).ToList();

                // Ordenar la lista por la Fecha de manera descendente (puedes usar OrderBy para ascendente)
                dataList = dataList.OrderByDescending(item => item.Fecha).ToList();

                // Asignar la lista ordenada como fuente de datos del DataGridView
                Table_Mac.DataSource = dataList;
               
            }
            catch (Exception ex)
            {
                // Manejar cualquier otra excepción que pueda ocurrir durante la conexión o el proceso
                MessageBox.Show("Error general: " + ex.Message + " Error al conectar a Firebase Realtime Database");
                
            }
            
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(NumeroIdentificacion + " " + MacID);
        }

        private void Table_Mac_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
        }

        private void Table_Mac_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            
        }

        private void Table_Mac_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
