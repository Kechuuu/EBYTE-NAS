using ComponentFactory.Krypton.Toolkit;
using System;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections.Generic;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System.Net;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;


namespace EBYTE_NAS
{
    public partial class Form1 : KryptonForm
    {
        string[] arregloPuertos; //Arreglo para los puertos
        SerialPort puerto;
        public Form1()
        {
            InitializeComponent();
            puerto = new SerialPort();
            //cb_module.Text = "E220";
            tx_addres.PlaceholderText = "Enter HEX format 0 - FF FF";
            tx_channel.PlaceholderText = "Enter HEX format 0 - 50";
            tx_net_id.PlaceholderText = "Enter HEX format 0 - FF FF";
            tx_key.PlaceholderText = "Enter HEX format 0 - FF FF";
            btn_conectar.Image = global::EBYTE_NAS.Properties.Resources.port_close;
            puerto.DataReceived += Puerto_DataReceived;
            

        }
        
        private void CheckVersion()
        {

            string new_Version = "1.0.1";
            String My_Version = "1.0.1";
            Conexion();
            // Verificar si la conexión fue exitosa
            if (client != null)
            {
                try
                {
                    FirebaseResponse response = client.Get("Version/New_version");

                    // Verificar si el nodo existe y tiene datos
                    if (response.Body != "null")
                    {
                        //  MessageBox.Show(response.Body);
                        new_Version = response.Body.Replace("\"", "");

                        if (My_Version != new_Version) //compara versiones para ver si hay una version nueva disponible
                        {
                            DialogResult result = MessageBox.Show("Nueva Actualizacion disponible ¿deseas descargar nueva version?", "Elegir Opción", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (result == DialogResult.Yes)
                            {
                                
                                
                                string link = "https://cdn.glitch.me/bca95e37-ff12-44c9-92a8-19fd3a3b892d/V2.0_Setup%20xbee_LoRa.zip?v=1691425217484";

                                try
                                {
                                    Process.Start(new ProcessStartInfo
                                    {
                                        FileName = link,
                                        UseShellExecute = true
                                    });
                                    File.Delete(@".\V2.0_Setup xbee_LoRa (3)");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error al abrir el enlace: " + ex.Message);
                                }
                            }
                            else if (result == DialogResult.No)
                            {
                                MessageBox.Show("Version actual " + My_Version);
                            }

                        }
                        else
                        {
                            MessageBox.Show("Cuentas con la version mas actualizada");
                        }
                    }
                    else
                    {
                        MessageBox.Show("No hay datos en el nodo especificado");
                    }
                }
                catch (FireSharp.Exceptions.FirebaseException ex)
                {
                    // Manejar el error específico de Firebase
                    MessageBox.Show("Error de Firebase: " + ex.Message);

                }
            }
        }

        DateTime fechaActual;
        Fire_base FireBase = new Fire_base(); // llamando a la tabla del segundo formulario

        IFirebaseClient client; //llamado a client de database
        int paro = 1;

        //variables de transferencias entre formularios
        private bool eliminar;
        public bool Eliminar { get => eliminar; set => eliminar = value; }
        private string Linea_clear; //variable para limpiar la linea desde la tabla
        string nodoAEliminar; //variable de nodo para eliminar
        private bool activate_table;
        public bool Update_table { get => update_table; set => update_table = value; }
        bool Actualizar;

        //variables encargadas de verificar en envio de datos
        String check_addres;
        String check_channel;
        String check_net_id;
        String check_Key;
        String check_baud;
        String check_parity;
        String check_air;
        String check_Psize;
        String check_Wrole;
        String check_Wcycle;
        String check_power;
        String check_RSSI;
        String check_crssi;
        String check_fixed;
        String check_relay;
        String check_LBT;
        String check_WControl;


        int m, mx, my; //variables de movimiento del forms




        private string IncrementarMacString(string mac)
        {
            // Convertir la cadena en un arreglo de enteros (bytes)
            string[] macBytes = mac.Split(' ');

            // Convertir cada byte en un valor numérico
            int byte1 = int.Parse(macBytes[0], System.Globalization.NumberStyles.HexNumber);
            int byte2 = int.Parse(macBytes[1], System.Globalization.NumberStyles.HexNumber);
            int byte3 = int.Parse(macBytes[2], System.Globalization.NumberStyles.HexNumber);

            // Incrementar el último byte
            byte3++;

            // Asegurarse de que no se desborde el último byte
            if (byte3 > 255)
            {
                byte3 = 0; // Reiniciar a 00 si el último byte supera 255
                byte2++;   // Incrementar el penúltimo byte

                // Asegurarse de que no se desborde el penúltimo byte
                if (byte2 > 255)
                {
                    byte2 = 0; // Reiniciar a 00 si el penúltimo byte supera 255
                    byte1++;   // Incrementar el primer byte

                    // Asegurarse de que no se desborde el primer byte
                    if (byte1 > 255)
                    {
                        byte1 = 0; // Reiniciar a 00 si el primer byte supera 255
                    }
                }
            }

            // Convertir los bytes de nuevo a la cadena en formato de dirección MAC
            return $"{byte1:X2} {byte2:X2} {byte3:X2}";
        }

        
        private void Conexion()
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "bMGJw8A2tp0MCkH5mcKWCe90QsX5OKq3qVHWwsEv",
                BasePath = "https://fir-ebyte-default-rtdb.firebaseio.com/"

            };
            client = new FireSharp.FirebaseClient(config);
        }


        private void pictureBox2_Click_3(object sender, EventArgs e)
        {

        }
       
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "bMGJw8A2tp0MCkH5mcKWCe90QsX5OKq3qVHWwsEv",
                BasePath = "https://fir-ebyte-default-rtdb.firebaseio.com/"
            };

            IFirebaseClient client = new FireSharp.FirebaseClient(config);

            if (client != null)
            {
              //  MessageBox.Show("Conexión exitosa a Firebase Realtime Database");

                // La ruta del nodo que deseas eliminar (por ejemplo, "usuarios/nodo_a_eliminar")

                if (eliminar == false)
                {
                    nodoAEliminar = TB_MAC_ID.Text;
                }
                else
                {
                    nodoAEliminar = Linea_clear;
                    Eliminar = false;
                }

                // Realizar la operación de eliminación en la base de datos
                FirebaseResponse response = client.Delete("Mac_address/" + nodoAEliminar);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show("Nodo eliminado correctamente " + "*** " + nodoAEliminar + " ***");
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
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "bMGJw8A2tp0MCkH5mcKWCe90QsX5OKq3qVHWwsEv",
                BasePath = "https://fir-ebyte-default-rtdb.firebaseio.com/"
            };

            IFirebaseClient client = new FireSharp.FirebaseClient(config);

            try
            {


                if (client != null)
                {
                    // Obtener el ID que deseas agregar
                    string id = TB_MAC_ID.Text;

                    // Verificar si el ID ya existe en la base de datos
                    FirebaseResponse checkResponse = client.Get("Mac_address/" + id);
                    if (checkResponse.Body != "null")
                    {
                        MessageBox.Show("El ID ya existe en la base de datos. No se puede agregar.");
                        return;
                    }

                    // Si el ID no existe, entonces procedemos a agregar el nuevo nodo
                    DateTime fechaActual = DateTime.Now;
                    string fechaFormateada = fechaActual.ToString("g");

                    // Datos que deseas agregar al nuevo nodo
                    var data = new
                    {
                        ID = id,
                        FECHA = fechaFormateada,
                        Type = lb_module_type.Text
                    };

                    // La ruta donde se agregará el nuevo nodo (por ejemplo, "Mac_address/nuevo_nodo")
                    string nuevaRuta = "Mac_address/" + id;

                    // Realizar la operación de escritura en la base de datos
                    SetResponse response = client.Set(nuevaRuta, data);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Nuevo nodo agregado correctamente");
                    }
                    else
                    {
                        MessageBox.Show("Error al agregar el nuevo nodo");
                    }
                }
                else
                {
                    MessageBox.Show("Error al conectar a Firebase Realtime Database");
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Error al conectar a Firebase Realtime Database");
            }
        }

        private void Puerto_DataReceived(object sender, SerialDataReceivedEventArgs e)//Metodo generado parea recibir datos
        {
            this.Invoke(new Action(ProcessingData));
        }

        private void ProcessingData() //obtencion de datos del puerto
        {

            int bytesToRead = puerto.BytesToRead;
            byte[] buffer = new byte[bytesToRead];
            puerto.Read(buffer, 0, bytesToRead);

            // MessageBox.Show("Mensaje recibido: " + BitConverter.ToString(buffer));

            terminalLb.Text += "-" + BitConverter.ToString(buffer);
            terminalLb.Text = terminalLb.Text.Replace("--", "-");


        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            m = 0;
        }


        private void btn_scroll_Click(object sender, EventArgs e)
        {
            btn_scroll.Image = global::EBYTE_NAS.Properties.Resources.Copia_de_Copia_de_Diseño_sin_título__6_;
            if (panel_Conf.AutoScroll == true)
            {
                panel_Conf.AutoScroll = false;

            }
            else
            {
                panel_Conf.AutoScroll = true;
                btn_scroll.Image = global::EBYTE_NAS.Properties.Resources.Copia_de_Copia_de_Diseño_sin_título__5_;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (m == 1)
            {
                this.SetDesktopLocation(MousePosition.X - mx, MousePosition.Y - my);
            }
        }
        int frecuency;
        private void tx_addres_TextChange(object sender, EventArgs e)
        {
            tx_addres.Text = tx_addres.Text.ToUpper();
            if (tx_addres.Text.Length >= 1)
            {
                tx_addres.Text = tx_addres.Text.ToUpper();
                string valorHexa = tx_addres.Text.Replace(" ", "");


                if (int.TryParse(valorHexa, System.Globalization.NumberStyles.HexNumber, null, out int value))
                {
                    // Si el valor ingresado es un número hexadecimal válido
                    // Verificar que esté en el rango permitido

                    // Actualizar el valor del ComboBox


                    if (value >= 0 && value <= 65535)
                    {
                        frecuency = value;
                        lb_addres.Text = "Addres ->" + Convert.ToString(frecuency);
                        //lb_channel.Text = "Channel";
                        lb_addres.ForeColor = Color.White;
                    }
                    else
                    {
                        // El valor está fuera del rango permitido, mostrar un mensaje de error o tomar alguna acción
                        lb_addres.Text = "Addres -> Numero fuera de rango";
                        lb_addres.ForeColor = Color.Red;
                        // También podrías restaurar un valor válido anterior o establecer un valor predeterminado.
                    }

                }

            }
        }

        private void tx_channel_TextChange(object sender, EventArgs e)
        {
            if (tx_channel.Text.Length >= 1)
            {
                tx_channel.Text = tx_channel.Text.ToUpper();
                if (int.TryParse(tx_channel.Text, System.Globalization.NumberStyles.HexNumber, null, out int value))
                {
                    // Si el valor ingresado es un número hexadecimal válido
                    // Verificar que esté en el rango permitido

                    // Actualizar el valor del ComboBox


                    if (value >= 0 && value <= 80)
                    {

                        frecuency = 850 + value;
                        lb_channel.Text = "Channel " + Convert.ToString(frecuency) + "MHz";
                        tx_channel.Text = value.ToString("X"); // Convertir a formato hexadecimal
                        //lb_channel.Text = "Channel";
                        lb_channel.ForeColor = Color.White;
                    }
                    else
                    {
                        // El valor está fuera del rango permitido, mostrar un mensaje de error o tomar alguna acción
                        lb_channel.Text = "Channel -> Numero fuera de rango";
                        lb_channel.ForeColor = Color.Red;
                        // También podrías restaurar un valor válido anterior o establecer un valor predeterminado.
                    }

                }
            }
        }

        private void tx_net_id_TextChange(object sender, EventArgs e)
        {
            tx_net_id.Text = tx_net_id.Text.ToUpper();
        }

        private void tx_key_TextChange(object sender, EventArgs e)
        {
            tx_key.Text = tx_key.Text.ToUpper();
        }

        private void btn_get_Click(object sender, EventArgs e)
        {
            if (cb_module.SelectedItem == "E22")
            {
                terminalLb.Text = "";
                if (puerto.IsOpen)
                {
                    puerto.WriteLine("AT+DEVTYPE=?");
                    timer_module.Start();
                    //get   
                }
                else
                {
                    MessageBox.Show("Port closed", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            if (cb_module.SelectedItem == "E220")
            {
                terminalLb.Text = "";
                if (puerto.IsOpen)
                {
                    byte[] get = { 0x41, 0x54, 0x2B, 0x44, 0x45, 0x56, 0x54, 0x59, 0x50, 0x45, 0x3D, 0x3F };
                    puerto.Write(get, 0, get.Length);
                    timer_module.Start();
                }
                else
                {
                    MessageBox.Show("Port closed", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            if (cb_module.SelectedItem == "E32")
            {
                terminalLb.Text = "";
                if (puerto.IsOpen)
                {

                    timer_module.Start();
                    byte[] get = { 0x41, 0x54, 0x2B, 0x44, 0x45, 0x56, 0x54, 0x59, 0x50, 0x45, 0x3D, 0x3F };
                    puerto.Write(get, 0, get.Length);
                    //timer_module.Start();
                    //get   
                }
                else
                {
                    MessageBox.Show("Port closed", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
        }
        public static byte[] ConvertHexStringToByteArray(string hexString)
        {

            int length = hexString.Length;
            byte[] byteArray = new byte[length / 2];

            for (int i = 0; i < length; i += 2)
            {
                byteArray[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            // at+cwcode
            // AT+DEVTYPE
            return byteArray;

        }

        private void btn_set_Click(object sender, EventArgs e)
        {
            if (puerto.IsOpen)
            {
                terminalLb.Text = "";
                if (cb_module.SelectedItem == "E22")
                {
                    //%NI=Coordinador,MAC=
                    //25 4E 49 3D - 43 6F 6F 72 64 69 6E 61 64 6F 72 2C - 4D 41 43 3D  - 11 24 B3 11 52 01 25 - 25
                    try
                    {
                        string Binariosbaud = "";
                        string Binarioparity = "";
                        string BinariosAIR = "";
                        terminalLb.Text = "";

                        //direccion 03H
                        //condiciones para el baud
                        if (CB_baudRate.Text == "1200")
                        {
                            Binariosbaud = "000";
                        }
                        if (CB_baudRate.Text == "2400")
                        {
                            Binariosbaud = "001";
                        }
                        if (CB_baudRate.Text == "4800")
                        {
                            Binariosbaud = "010";
                        }
                        if (CB_baudRate.Text == "9600")
                        {
                            Binariosbaud = "011";
                        }
                        if (CB_baudRate.Text == "19200")
                        {
                            Binariosbaud = "100";
                        }
                        if (CB_baudRate.Text == "38400")
                        {
                            Binariosbaud = "101";
                        }
                        if (CB_baudRate.Text == "57600")
                        {
                            Binariosbaud = "110";
                        }
                        if (CB_baudRate.Text == "115200")
                        {
                            Binariosbaud = "111";
                        }

                        if (check_baud == CB_baudRate.Text)
                        {
                            lb_BaudRate.Text = "Baud Rate";
                            lb_BaudRate.ForeColor = Color.White;
                        }
                        else
                        {
                            if (CB_baudRate.Text != "")
                            {
                                lb_BaudRate.Text = "Baud Rate ✔";
                                lb_BaudRate.ForeColor = Color.Green;
                            }
                        }


                        //condiciones para LA PARIDAD
                        if (CB_PARITY.Text == "8N1")
                        {
                            Binarioparity = "00";
                        }
                        if (CB_PARITY.Text == "8O1")
                        {
                            Binarioparity = "01";
                        }
                        if (CB_PARITY.Text == "8E1")
                        {
                            Binarioparity = "10";
                        }

                        if (check_parity == CB_PARITY.Text)
                        {
                            lb_Parity.Text = "Parity";
                            lb_Parity.ForeColor = Color.White;
                        }
                        else
                        {
                            if (CB_PARITY.Text != "")
                            {
                                lb_Parity.Text = "Parity ✔";
                                lb_Parity.ForeColor = Color.Green;
                            }

                        }

                        //condiciones para el air rate
                        if (cb_air_rate.Text == "0.3K")
                        {
                            BinariosAIR = "000";
                        }
                        if (cb_air_rate.Text == "1.2K")
                        {
                            BinariosAIR = "001";
                        }
                        if (cb_air_rate.Text == "2.4K")
                        {
                            BinariosAIR = "010";
                        }
                        if (cb_air_rate.Text == "4.8K")
                        {
                            BinariosAIR = "011";
                        }
                        if (cb_air_rate.Text == "9.6K")
                        {
                            BinariosAIR = "100";
                        }
                        if (cb_air_rate.Text == "19.2K")
                        {
                            BinariosAIR = "101";
                        }
                        if (cb_air_rate.Text == "38.4K")
                        {
                            BinariosAIR = "110";
                        }
                        if (cb_air_rate.Text == "62.5K")
                        {
                            BinariosAIR = "111";
                        }

                        if (check_air == cb_air_rate.Text)
                        {
                            lb_Air.Text = "Air Rate";
                            lb_Air.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_air_rate.Text != "")
                            {
                                lb_Air.Text = "Air Rate ✔";
                                lb_Air.ForeColor = Color.Green;
                            }
                        }




                        //DIRECIION 04H
                        string BinariosPsize = "";
                        string BinariRSSI = "";
                        string Binarioreserve = "000";
                        string BinariosPower = "";

                        //binarios packet size

                        if (cb_Psize.Text == "240 bytes")
                        {
                            BinariosPsize = "00";
                        }
                        if (cb_Psize.Text == "128 bytes")
                        {
                            BinariosPsize = "01";
                        }
                        if (cb_Psize.Text == "64 bytes")
                        {
                            BinariosPsize = "10";
                        }
                        if (cb_Psize.Text == "32 bytes")
                        {
                            BinariosPsize = "11";
                        }
                        if (check_Psize == cb_Psize.Text)
                        {
                            lb_PacketSize.Text = "Packet Size";
                            lb_PacketSize.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_Psize.Text != "")
                            {
                                lb_PacketSize.Text = "Packet Size ✔";
                                lb_PacketSize.ForeColor = Color.Green;
                            }
                        }


                        //binarios para RSSI
                        if (cb_Crssi.Text == "Disable")
                        {
                            BinariRSSI = "0";
                        }
                        if (cb_Crssi.Text == "Enable")
                        {
                            BinariRSSI = "1";
                        }
                        if (check_RSSI == cb_Crssi.Text)
                        {
                            lb_rssi.Text = "Channel RSSI";
                            lb_rssi.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_Crssi.Text != "")
                            {
                                lb_rssi.Text = "Channel RSSI ✔";
                                lb_rssi.ForeColor = Color.Green;
                            }
                        }

                        //los binarios direccion 4 3 2 reservados = 0 0 0

                        //binarios power

                        if (cb_power.Text == "30dBm")
                        {
                            BinariosPower = "00";
                        }
                        if (cb_power.Text == "27dBm")
                        {
                            BinariosPower = "01";
                        }
                        if (cb_power.Text == "24dBm")
                        {
                            BinariosPower = "10";
                        }
                        if (cb_power.Text == "21dBm")
                        {
                            BinariosPower = "11";
                        }
                        if (check_power == cb_power.Text)
                        {
                            lb_Power.Text = "Power";
                            lb_Power.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_power.Text != "")
                            {
                                lb_Power.Text = "Power ✔";
                                lb_Power.ForeColor = Color.Green;
                            }
                        }

                        //DIRECIION 06H
                        string BinariosE_RSSI = "";
                        string Binari_fixed = "";
                        string Binarior_reply = "";
                        string Binarios_LBT = "";
                        string Binarios_Wtrans = "";
                        string Binarios_Wcycle = "";

                        //Packet rssi
                        if (cb_Prssi.Text == "Disable")
                        {
                            BinariosE_RSSI = "0";
                        }
                        if (cb_Prssi.Text == "Enable")
                        {
                            BinariosE_RSSI = "1";
                        }

                        //tran mode
                        if (cb_tranMode.Text == "Normal")
                        {
                            Binari_fixed = "0";
                        }
                        if (cb_tranMode.Text == "Fixed")
                        {
                            Binari_fixed = "1";
                        }
                        if (check_fixed == cb_tranMode.Text)
                        {
                            lb_TranMode.Text = "Tran Mode";
                            lb_TranMode.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_tranMode.Text != "")
                            {
                                lb_TranMode.Text = "Tran Mode ✔";
                                lb_TranMode.ForeColor = Color.Green;
                            }
                        }

                        //relay
                        if (cb_relay.Text == "Disable")
                        {
                            Binarior_reply = "0";
                        }
                        if (cb_relay.Text == "Enable")
                        {
                            Binarior_reply = "1";
                        }
                        if (check_relay == cb_relay.Text)
                        {
                            lb_relay.Text = "Relay";
                            lb_relay.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_relay.Text != "")
                            {
                                lb_relay.Text = "Relay ✔";
                                lb_relay.ForeColor = Color.Green;
                            }
                        }

                        //LBT
                        if (cb_lbt.Text == "Disable")
                        {
                            Binarios_LBT = "0";
                        }
                        if (cb_lbt.Text == "Enable")
                        {
                            Binarios_LBT = "1";
                        }
                        if (check_LBT == cb_lbt.Text)
                        {
                            lb_LBT.Text = "LBT";
                            lb_LBT.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_lbt.Text != "")
                            {
                                lb_LBT.Text = "LBT ✔";
                                lb_LBT.ForeColor = Color.Green;
                            }
                        }


                        //wor_control
                        if (cb_Wrole.Text == "Recieve")
                        {
                            Binarios_Wtrans = "0";
                        }
                        if (cb_Wrole.Text == "Translate")
                        {
                            Binarios_Wtrans = "1";
                        }

                        if (check_Wrole == cb_Wrole.Text)
                        {
                            lb_WorROLE.Text = "Wor Role";
                            lb_WorROLE.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_Wrole.Text != "")
                            {
                                lb_WorROLE.Text = "Wor Role  ✔";
                                lb_WorROLE.ForeColor = Color.Green;
                            }
                        }
                        //wor cycle

                        if (cb_Wcycle.Text == "500ms")
                        {
                            Binarios_Wcycle = "000";
                        }
                        if (cb_Wcycle.Text == "1000ms")
                        {
                            Binarios_Wcycle = "001";
                        }
                        if (cb_Wcycle.Text == "1500ms")
                        {
                            Binarios_Wcycle = "010";
                        }
                        if (cb_Wcycle.Text == "2000ms")
                        {
                            Binarios_Wcycle = "011";
                        }
                        if (cb_Wcycle.Text == "2500ms")
                        {
                            Binarios_Wcycle = "100";
                        }
                        if (cb_Wcycle.Text == "3000ms")
                        {
                            Binarios_Wcycle = "101";
                        }
                        if (cb_Wcycle.Text == "3500ms")
                        {
                            Binarios_Wcycle = "110";
                        }
                        if (cb_Wcycle.Text == "4000ms")
                        {
                            Binarios_Wcycle = "111";
                        }

                        if (check_Wcycle == cb_Wcycle.Text)
                        {
                            lb_WorCycle.Text = "Wor Cycle";
                            lb_WorCycle.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_Wcycle.Text != "")
                            {
                                lb_WorCycle.Text = "Wor cycle ✔";
                                lb_WorCycle.ForeColor = Color.Green;
                            }
                        }

                        //conversion de la direccion 01H
                        if (tx_addres.Text != string.Empty)
                        {


                            string addres = tx_addres.Text;
                            if (addres == check_addres || addres == checj_addres_set)
                            {
                                lb_addres.Text = "Addres";
                                lb_addres.ForeColor = Color.White;
                            }
                            else
                            {

                                lb_addres.Text = "Addres ✔";
                                lb_addres.ForeColor = Color.Green;
                            }

                            string addres_s = addres.Substring(0, 2);
                            string addres_i = addres.Substring(3, 2);
                            // MessageBox.Show(addres_s + addres_i);

                            //conversion net id 02H
                            string net_id = tx_net_id.Text;
                            if (tx_net_id.Text == check_net_id)
                            {
                                lb_netID.Text = "Net ID";
                                lb_netID.ForeColor = Color.White;
                            }
                            else
                            {

                                lb_netID.Text = "Net ID ✔";
                                lb_netID.ForeColor = Color.Green;
                            }


                            //conversion de la direccion 03H a binario
                            string Addres_03H = Binariosbaud + Binarioparity + BinariosAIR;
                            int numeroDecimal = Convert.ToInt32(Addres_03H, 2);
                            string numeroHexadecimal_03 = numeroDecimal.ToString("X");


                            //conversion de la direccion 04H a binarios 
                            string Addres_04H = BinariosPsize + BinariRSSI + Binarioreserve + BinariosPower;
                            int numeroDecimal_04 = Convert.ToInt32(Addres_04H, 2);
                            string numeroHexadecimal_04 = numeroDecimal_04.ToString("X");


                            //conversion de la direccion 05H a binarios 
                            string channel = tx_channel.Text;
                            if (check_channel == tx_channel.Text)
                            {
                                lb_channel.Text = "Channel";
                                lb_channel.ForeColor = Color.White;
                            }
                            else
                            {
                                lb_channel.Text = "Channel ✔";
                                lb_channel.ForeColor = Color.Green;
                            }


                            //conversion de la direccion 06H a binarios 
                            string Addres_06H = BinariosE_RSSI + Binari_fixed + Binarior_reply + Binarios_LBT + Binarios_Wtrans + Binarios_Wcycle;

                            int numeroDecimal_06H = Convert.ToInt32(Addres_06H, 2);
                            string numeroHexadecimal_06 = numeroDecimal_06H.ToString("X");




                            byte resultado_01 = byte.Parse(addres_s, System.Globalization.NumberStyles.HexNumber);
                            byte resultado_01_1 = byte.Parse(addres_i, System.Globalization.NumberStyles.HexNumber);
                            byte resultado_02 = byte.Parse(net_id, System.Globalization.NumberStyles.HexNumber);
                            byte resultado_03 = byte.Parse(numeroHexadecimal_03, System.Globalization.NumberStyles.HexNumber);
                            byte resultado_04H = byte.Parse(numeroHexadecimal_04, System.Globalization.NumberStyles.HexNumber);
                            byte resultado_05H = byte.Parse(channel, System.Globalization.NumberStyles.HexNumber);
                            byte resultado_06H = byte.Parse(numeroHexadecimal_06, System.Globalization.NumberStyles.HexNumber);



                            //byte[] hexMessage = {  };
                            byte[] hexMessage2 = { 0xC0, 0x00, 0x08, resultado_01, resultado_01_1, resultado_02, resultado_03, resultado_04H, resultado_05H, resultado_06H, 0x00, 0x00 }; //-C1-00-09-00-F8-0A-60-C0-1E-80-00-00
                                                                                                                                                                                          // puerto.Write(hexMessage, 0, hexMessage.Length);
                            puerto.Write(hexMessage2, 0, hexMessage2.Length);
                            //puerto.Write(mensaje_byte_tamaño, 0, mensaje_byte_tamaño.Length);
                            timer_set.Start();

                        }
                        else
                        {
                            MessageBox.Show("no hay datos");
                        }

                    }
                    catch (Exception error)
                    {

                        MessageBox.Show(error.Message);
                    }
                }
                if (cb_module.SelectedItem == "E220")
                {
                    //%NI=Coordinador,MAC=
                    //25 4E 49 3D - 43 6F 6F 72 64 69 6E 61 64 6F 72 2C - 4D 41 43 3D  - 11 24 B3 11 52 01 25 - 25
                    try
                    {
                        string Binariosbaud = "";
                        string Binarioparity = "";
                        string BinariosAIR = "";
                        terminalLb.Text = "";

                        //direccion 03H
                        //condiciones para el baud
                        if (CB_baudRate.Text == "1200")
                        {
                            Binariosbaud = "000";
                        }
                        if (CB_baudRate.Text == "2400")
                        {
                            Binariosbaud = "001";
                        }
                        if (CB_baudRate.Text == "4800")
                        {
                            Binariosbaud = "010";
                        }
                        if (CB_baudRate.Text == "9600")
                        {
                            Binariosbaud = "011";
                        }
                        if (CB_baudRate.Text == "19200")
                        {
                            Binariosbaud = "100";
                        }
                        if (CB_baudRate.Text == "38400")
                        {
                            Binariosbaud = "101";
                        }
                        if (CB_baudRate.Text == "57600")
                        {
                            Binariosbaud = "110";
                        }
                        if (CB_baudRate.Text == "115200")
                        {
                            Binariosbaud = "111";
                        }

                        if (check_baud == CB_baudRate.Text)
                        {
                            lb_BaudRate.Text = "Baud Rate";
                            lb_BaudRate.ForeColor = Color.White;
                        }
                        else
                        {
                            if (CB_baudRate.Text != "")
                            {
                                lb_BaudRate.Text = "Baud Rate ✔";
                                lb_BaudRate.ForeColor = Color.Green;
                            }
                        }


                        //condiciones para LA PARIDAD
                        if (CB_PARITY.Text == "8N1")
                        {
                            Binarioparity = "00";
                        }
                        if (CB_PARITY.Text == "8O1")
                        {
                            Binarioparity = "01";
                        }
                        if (CB_PARITY.Text == "8E1")
                        {
                            Binarioparity = "10";
                        }

                        if (check_parity == CB_PARITY.Text)
                        {
                            lb_Parity.Text = "Parity";
                            lb_Parity.ForeColor = Color.White;
                        }
                        else
                        {
                            if (CB_PARITY.Text != "")
                            {
                                lb_Parity.Text = "Parity ✔";
                                lb_Parity.ForeColor = Color.Green;
                            }

                        }

                        //condiciones para el air rate
                        if (cb_air_rate.Text == "0.3K")
                        {
                            BinariosAIR = "000";
                        }
                        if (cb_air_rate.Text == "1.2K")
                        {
                            BinariosAIR = "001";
                        }
                        if (cb_air_rate.Text == "2.4K")
                        {
                            BinariosAIR = "010";
                        }
                        if (cb_air_rate.Text == "4.8K")
                        {
                            BinariosAIR = "011";
                        }
                        if (cb_air_rate.Text == "9.6K")
                        {
                            BinariosAIR = "100";
                        }
                        if (cb_air_rate.Text == "19.2K")
                        {
                            BinariosAIR = "101";
                        }
                        if (cb_air_rate.Text == "38.4K")
                        {
                            BinariosAIR = "110";
                        }
                        if (cb_air_rate.Text == "62.5K")
                        {
                            BinariosAIR = "111";
                        }

                        if (check_air == cb_air_rate.Text)
                        {
                            lb_Air.Text = "Air Rate";
                            lb_Air.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_air_rate.Text != "")
                            {
                                lb_Air.Text = "Air Rate ✔";
                                lb_Air.ForeColor = Color.Green;
                            }
                        }




                        //DIRECIION 04H
                        string BinariosPsize = "";
                        string BinariRSSI = "";
                        string Binarioreserve = "000";
                        string BinariosPower = "";

                        //binarios packet size

                        if (cb_Psize.Text == "240 bytes")
                        {
                            BinariosPsize = "00";
                        }
                        if (cb_Psize.Text == "128 bytes")
                        {
                            BinariosPsize = "01";
                        }
                        if (cb_Psize.Text == "64 bytes")
                        {
                            BinariosPsize = "10";
                        }
                        if (cb_Psize.Text == "32 bytes")
                        {
                            BinariosPsize = "11";
                        }
                        if (check_Psize == cb_Psize.Text)
                        {
                            lb_PacketSize.Text = "Packet Size";
                            lb_PacketSize.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_Psize.Text != "")
                            {
                                lb_PacketSize.Text = "Packet Size ✔";
                                lb_PacketSize.ForeColor = Color.Green;
                            }
                        }


                        //binarios para RSSI
                        if (cb_Crssi.Text == "Disable")
                        {
                            BinariRSSI = "0";
                        }
                        if (cb_Crssi.Text == "Enable")
                        {
                            BinariRSSI = "1";
                        }
                        if (check_RSSI == cb_Crssi.Text)
                        {
                            lb_rssi.Text = "Channel RSSI";
                            lb_rssi.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_Crssi.Text != "")
                            {
                                lb_rssi.Text = "Channel RSSI ✔";
                                lb_rssi.ForeColor = Color.Green;
                            }
                        }

                        //los binarios direccion 4 3 2 reservados = 0 0 0

                        //binarios power

                        if (cb_power.Text == "30dBm")
                        {
                            BinariosPower = "00";
                        }
                        if (cb_power.Text == "27dBm")
                        {
                            BinariosPower = "01";
                        }
                        if (cb_power.Text == "24dBm")
                        {
                            BinariosPower = "10";
                        }
                        if (cb_power.Text == "21dBm")
                        {
                            BinariosPower = "11";
                        }
                        if (check_power == cb_power.Text)
                        {
                            lb_Power.Text = "Power";
                            lb_Power.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_power.Text != "")
                            {
                                lb_Power.Text = "Power ✔";
                                lb_Power.ForeColor = Color.Green;
                            }
                        }

                        //DIRECIION 06H
                        string BinariosE_RSSI = "";
                        string Binari_fixed = "";
                        string Binarior_reply = "";
                        string Binarios_LBT = "";
                        string Binarios_Wtrans = "";
                        string Binarios_Wcycle = "";

                        //Packet rssi
                        if (cb_Prssi.Text == "Disable")
                        {
                            BinariosE_RSSI = "0";
                        }
                        if (cb_Prssi.Text == "Enable")
                        {
                            BinariosE_RSSI = "1";
                        }

                        //tran mode
                        if (cb_tranMode.Text == "Normal")
                        {
                            Binari_fixed = "0";
                        }
                        if (cb_tranMode.Text == "Fixed")
                        {
                            Binari_fixed = "1";
                        }
                        if (check_fixed == cb_tranMode.Text)
                        {
                            lb_TranMode.Text = "Tran Mode";
                            lb_TranMode.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_tranMode.Text != "")
                            {
                                lb_TranMode.Text = "Tran Mode ✔";
                                lb_TranMode.ForeColor = Color.Green;
                            }
                        }

                        //relay
                        if (cb_relay.Text == "Disable")
                        {
                            Binarior_reply = "0";
                        }
                        if (cb_relay.Text == "Enable")
                        {
                            Binarior_reply = "1";
                        }
                        if (check_relay == cb_relay.Text)
                        {
                            lb_relay.Text = "Relay";
                            lb_relay.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_relay.Text != "")
                            {
                                lb_relay.Text = "Relay ✔";
                                lb_relay.ForeColor = Color.Green;
                            }
                        }

                        //LBT
                        if (cb_lbt.Text == "Disable")
                        {
                            Binarios_LBT = "0";
                        }
                        if (cb_lbt.Text == "Enable")
                        {
                            Binarios_LBT = "1";
                        }
                        if (check_LBT == cb_lbt.Text)
                        {
                            lb_LBT.Text = "LBT";
                            lb_LBT.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_lbt.Text != "")
                            {
                                lb_LBT.Text = "LBT ✔";
                                lb_LBT.ForeColor = Color.Green;
                            }
                        }


                        //wor_control
                        if (cb_Wrole.Text == "Recieve")
                        {
                            Binarios_Wtrans = "0";
                        }
                        if (cb_Wrole.Text == "Translate")
                        {
                            Binarios_Wtrans = "1";
                        }

                        if (check_Wrole == cb_Wrole.Text)
                        {
                            lb_WorROLE.Text = "Wor Role";
                            lb_WorROLE.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_Wrole.Text != "")
                            {
                                lb_WorROLE.Text = "Wor Role  ✔";
                                lb_WorROLE.ForeColor = Color.Green;
                            }
                        }
                        //wor cycle

                        if (cb_Wcycle.Text == "500ms")
                        {
                            Binarios_Wcycle = "000";
                        }
                        if (cb_Wcycle.Text == "1000ms")
                        {
                            Binarios_Wcycle = "001";
                        }
                        if (cb_Wcycle.Text == "1500ms")
                        {
                            Binarios_Wcycle = "010";
                        }
                        if (cb_Wcycle.Text == "2000ms")
                        {
                            Binarios_Wcycle = "011";
                        }
                        if (cb_Wcycle.Text == "2500ms")
                        {
                            Binarios_Wcycle = "100";
                        }
                        if (cb_Wcycle.Text == "3000ms")
                        {
                            Binarios_Wcycle = "101";
                        }
                        if (cb_Wcycle.Text == "3500ms")
                        {
                            Binarios_Wcycle = "110";
                        }
                        if (cb_Wcycle.Text == "4000ms")
                        {
                            Binarios_Wcycle = "111";
                        }

                        if (check_Wcycle == cb_Wcycle.Text)
                        {
                            lb_WorCycle.Text = "Wor Cycle";
                            lb_WorCycle.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_Wcycle.Text != "")
                            {
                                lb_WorCycle.Text = "Wor cycle ✔";
                                lb_WorCycle.ForeColor = Color.Green;
                            }
                        }

                        //conversion de la direccion 01H
                        if (tx_addres.Text != string.Empty)
                        {


                            string addres = tx_addres.Text;
                            if (addres == check_addres || addres == checj_addres_set)
                            {
                                lb_addres.Text = "Addres";
                                lb_addres.ForeColor = Color.White;
                            }
                            else
                            {

                                lb_addres.Text = "Addres ✔";
                                lb_addres.ForeColor = Color.Green;
                            }

                            string addres_s = addres.Substring(0, 2);
                            string addres_i = addres.Substring(3, 2);
                            // MessageBox.Show(addres_s + addres_i);

                            //conversion net id 02H
                            string net_id = tx_net_id.Text;
                            if (tx_net_id.Text == check_net_id)
                            {
                                lb_netID.Text = "Net ID";
                                lb_netID.ForeColor = Color.White;
                            }
                            else
                            {

                                lb_netID.Text = "Net ID ✔";
                                lb_netID.ForeColor = Color.Green;
                            }


                            //conversion de la direccion 03H a binario
                            string Addres_03H = Binariosbaud + Binarioparity + BinariosAIR;
                            int numeroDecimal = Convert.ToInt32(Addres_03H, 2);
                            string numeroHexadecimal_03 = numeroDecimal.ToString("X");


                            //conversion de la direccion 04H a binarios 
                            string Addres_04H = BinariosPsize + BinariRSSI + Binarioreserve + BinariosPower;
                            int numeroDecimal_04 = Convert.ToInt32(Addres_04H, 2);
                            string numeroHexadecimal_04 = numeroDecimal_04.ToString("X");


                            //conversion de la direccion 05H a binarios 
                            string channel = tx_channel.Text;
                            if (check_channel == tx_channel.Text)
                            {
                                lb_channel.Text = "Channel";
                                lb_channel.ForeColor = Color.White;
                            }
                            else
                            {
                                lb_channel.Text = "Channel ✔";
                                lb_channel.ForeColor = Color.Green;
                            }


                            //conversion de la direccion 06H a binarios 
                            string Addres_06H = BinariosE_RSSI + Binari_fixed + Binarior_reply + Binarios_LBT + Binarios_Wtrans + Binarios_Wcycle;

                            int numeroDecimal_06H = Convert.ToInt32(Addres_06H, 2);
                            string numeroHexadecimal_06 = numeroDecimal_06H.ToString("X");




                            byte resultado_01 = byte.Parse(addres_s, System.Globalization.NumberStyles.HexNumber);
                            byte resultado_01_1 = byte.Parse(addres_i, System.Globalization.NumberStyles.HexNumber);
                            byte resultado_03 = byte.Parse(numeroHexadecimal_03, System.Globalization.NumberStyles.HexNumber);
                            byte resultado_04H = byte.Parse(numeroHexadecimal_04, System.Globalization.NumberStyles.HexNumber);
                            byte resultado_05H = byte.Parse(channel, System.Globalization.NumberStyles.HexNumber);
                            byte resultado_06H = byte.Parse(numeroHexadecimal_06, System.Globalization.NumberStyles.HexNumber);



                            //byte[] hexMessage = {  };
                            byte[] hexMessage2 = { 0xC0, 0x00, 0x08, resultado_01, resultado_01_1, resultado_03, resultado_04H, resultado_05H, resultado_06H, 0x00, 0x00 }; //-C1-00-09-00-F8-0A-60-C0-1E-80-00-00
                                                                                                                                                                            // puerto.Write(hexMessage, 0, hexMessage.Length);
                            puerto.Write(hexMessage2, 0, hexMessage2.Length);
                            //puerto.Write(mensaje_byte_tamaño, 0, mensaje_byte_tamaño.Length);
                            timer_set.Start();

                        }
                        else
                        {
                            MessageBox.Show("no hay datos");
                        }

                    }
                    catch (Exception error)
                    {

                        MessageBox.Show(error.Message);
                    }
                }
                if (cb_module.SelectedItem == "E32")
                {
                    //conversion de la direccion 01H
                    if (tx_addres.Text != string.Empty)
                    {


                        string addres = tx_addres.Text;
                        if (addres == check_addres || addres == checj_addres_set)
                        {
                            lb_addres.Text = "Addres";
                            lb_addres.ForeColor = Color.White;
                        }
                        else
                        {

                            lb_addres.Text = "Addres ✔";
                            lb_addres.ForeColor = Color.Green;
                        }
                        terminalLb.Text = "";

                        string Binariosbaud = "";
                        string Binarioparity = "";
                        string BinariosAIR = "";

                        //CONVERSION DE LA DIRECCION 03H
                        if (CB_baudRate.Text == "1200")
                        {
                            Binariosbaud = "000";
                        }
                        if (CB_baudRate.Text == "2400")
                        {
                            Binariosbaud = "001";
                        }
                        if (CB_baudRate.Text == "4800")
                        {
                            Binariosbaud = "010";
                        }
                        if (CB_baudRate.Text == "9600")
                        {
                            Binariosbaud = "011";
                        }
                        if (CB_baudRate.Text == "19200")
                        {
                            Binariosbaud = "100";
                        }
                        if (CB_baudRate.Text == "38400")
                        {
                            Binariosbaud = "101";
                        }
                        if (CB_baudRate.Text == "57600")
                        {
                            Binariosbaud = "110";
                        }
                        if (CB_baudRate.Text == "115200")
                        {
                            Binariosbaud = "111";
                        }

                        if (check_baud == CB_baudRate.Text)
                        {
                            lb_BaudRate.Text = "Baud Rate";
                            lb_BaudRate.ForeColor = Color.White;
                        }
                        else
                        {
                            if (CB_baudRate.Text != "")
                            {
                                lb_BaudRate.Text = "Baud Rate ✔";
                                lb_BaudRate.ForeColor = Color.Green;
                            }
                        }


                        //condiciones para LA PARIDAD
                        if (CB_PARITY.Text == "8N1")
                        {
                            Binarioparity = "00";
                        }
                        if (CB_PARITY.Text == "8O1")
                        {
                            Binarioparity = "01";
                        }
                        if (CB_PARITY.Text == "8E1")
                        {
                            Binarioparity = "10";
                        }

                        if (check_parity == CB_PARITY.Text)
                        {
                            lb_Parity.Text = "Parity";
                            lb_Parity.ForeColor = Color.White;
                        }
                        else
                        {
                            if (CB_PARITY.Text != "")
                            {
                                lb_Parity.Text = "Parity ✔";
                                lb_Parity.ForeColor = Color.Green;
                            }

                        }

                        //condiciones para el air rate
                        if (cb_air_rate.Text == "0.3K")
                        {
                            BinariosAIR = "000";
                        }
                        if (cb_air_rate.Text == "1.2K")
                        {
                            BinariosAIR = "001";
                        }
                        if (cb_air_rate.Text == "2.4K")
                        {
                            BinariosAIR = "010";
                        }
                        if (cb_air_rate.Text == "4.8K")
                        {
                            BinariosAIR = "011";
                        }
                        if (cb_air_rate.Text == "9.6K")
                        {
                            BinariosAIR = "100";
                        }
                        if (cb_air_rate.Text == "19.2K")
                        {
                            BinariosAIR = "101";
                        }
                        if (cb_air_rate.Text == "38.4K")
                        {
                            BinariosAIR = "110";
                        }
                        if (cb_air_rate.Text == "62.5K")
                        {
                            BinariosAIR = "111";
                        }

                        if (check_air == cb_air_rate.Text)
                        {
                            lb_Air.Text = "Air Rate";
                            lb_Air.ForeColor = Color.White;
                        }
                        else
                        {
                            if (cb_air_rate.Text != "")
                            {
                                lb_Air.Text = "Air Rate ✔";
                                lb_Air.ForeColor = Color.Green;
                            }
                        }

                        //DIRECCION 06H 
                        string Binarios_fixed = "";
                        string Binario_ioMode = "";
                        string Binarios_WakeUp = "";
                        string Binarios_FEC = "";
                        string Binarios_Power = "";

                        //FIXED MODE
                        if (cb_Wrole.Text == "Disable")
                        {
                            Binarios_fixed = "0";
                        }
                        if (cb_Wrole.Text == "Enable")
                        {
                            Binarios_fixed = "1";
                        }

                        //IO MODE
                        if (cb_tranMode.Text == "OpenDrain")
                        {
                            Binario_ioMode = "0";
                        }
                        if (cb_tranMode.Text == "PushPull")
                        {
                            Binario_ioMode = "1";
                        }
                        //WOR TIMING

                        if (cb_Wcycle.Text == "500ms")
                        {
                            Binarios_WakeUp = "000";
                        }
                        if (cb_Wcycle.Text == "1000ms")
                        {
                            Binarios_WakeUp = "001";
                        }
                        if (cb_Wcycle.Text == "1500ms")
                        {
                            Binarios_WakeUp = "010";
                        }
                        if (cb_Wcycle.Text == "2000ms")
                        {
                            Binarios_WakeUp = "011";
                        }
                        if (cb_Wcycle.Text == "2500ms")
                        {
                            Binarios_WakeUp = "100";
                        }
                        if (cb_Wcycle.Text == "3000ms")
                        {
                            Binarios_WakeUp = "101";
                        }
                        if (cb_Wcycle.Text == "3500ms")
                        {
                            Binarios_WakeUp = "110";
                        }
                        if (cb_Wcycle.Text == "4000ms")
                        {
                            Binarios_WakeUp = "111";
                        }



                        //FEC
                        if (cb_Psize.Text == "Disable")
                        {
                            Binarios_FEC = "0"; //Disable
                        }
                        if (cb_Psize.Text == "Enable")
                        {
                            Binarios_FEC = "1"; //Enable
                        }


                        //configuracion power
                        if (cb_power.Text == "30dBm")
                        {
                            Binarios_Power = "00";
                        }
                        if (cb_power.Text == "27dBm")
                        {
                            Binarios_Power = "01";
                        }
                        if (cb_power.Text == "24dBm")
                        {
                            Binarios_Power = "10";
                        }
                        if (cb_power.Text == "21dBm")
                        {
                            Binarios_Power = "11";
                        }
                        check_power = cb_power.Text;

                        string Addres_03H = Binarioparity + Binariosbaud + BinariosAIR;
                        int numeroDecimal = Convert.ToInt32(Addres_03H, 2);
                        string numeroHexadecimal_03 = numeroDecimal.ToString("X");

                        string Addres_05H = Binarios_fixed + Binario_ioMode + Binarios_WakeUp + Binarios_FEC + Binarios_Power;
                        int Decimal_05H = Convert.ToInt32(Addres_05H, 2);
                        string numeroHexadecimal_05 = Decimal_05H.ToString("X");


                        string addres_s = addres.Substring(0, 2);
                        string addres_i = addres.Substring(3, 2);

                        //direccion 4h
                        string channel = tx_channel.Text;
                        if (check_channel == tx_channel.Text)
                        {
                            lb_channel.Text = "Channel";
                            lb_channel.ForeColor = Color.White;
                        }
                        else
                        {
                            lb_channel.Text = "Channel ✔";
                            lb_channel.ForeColor = Color.Green;
                        }
                        byte resultado_04H = byte.Parse(channel, System.Globalization.NumberStyles.HexNumber);

                        //CONVERSION PARA CADENA HEXA
                        byte resultado_01 = byte.Parse(addres_s, System.Globalization.NumberStyles.HexNumber);
                        byte resultado_01_1 = byte.Parse(addres_i, System.Globalization.NumberStyles.HexNumber);

                        byte resultado_03 = byte.Parse(numeroHexadecimal_03, System.Globalization.NumberStyles.HexNumber);
                        byte resultado_05 = byte.Parse(numeroHexadecimal_05, System.Globalization.NumberStyles.HexNumber);

                        byte[] hexMessage2 = { 0xC0, resultado_01, resultado_01_1, resultado_03, resultado_04H, resultado_05 }; //-C0-09-DD-5B-07-1D
                                                                                                                                // puerto.Write(hexMessage, 0, hexMessage.Length);
                        puerto.Write(hexMessage2, 0, hexMessage2.Length);

                    }

                }
            }

            else
            {
                MessageBox.Show("Port closed", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            // timer1.Start();

        }
        // variable utilizada para verificar la conexion del puerto
        bool puerto_Conectado;
        private void btn_conectar_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (puerto.IsOpen == false)
                {
                    if (cb_puertos != null)
                    {
                        if (cb_puertos.SelectedItem != null)
                        {
                            puerto.PortName = cb_puertos.SelectedItem.ToString();
                            puerto.Open();
                            puerto.DtrEnable = true;

                            // ProcessingData();
                            btn_conectar.Image = global::EBYTE_NAS.Properties.Resources.port_open;
                            puerto_Conectado = true;
                            timer_detect_port.Start();
                            if (cb_module.SelectedItem=="" || cb_module.SelectedItem == null)
                            {

                                MessageBox.Show("Selecciona un modulo para configurar ");
                            }
                            else
                            {
                                puerto.WriteLine("@AT+CONF=" + cb_module.SelectedItem + "@");
                                MessageBox.Show("@AT+CONF=" + cb_module.SelectedItem + "@");
                            }
                           
                        }
                        else
                        {
                            MessageBox.Show("Selecciona un puerto", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se encontraron puertos", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {

                    puerto.Close();
                    btn_conectar.Image = global::EBYTE_NAS.Properties.Resources.port_close;
                    puerto_Conectado = false;
                    MessageBox.Show("Port closed", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }



        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            m = 1;
            mx = e.X;
            my = e.Y;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            timer_checkVersion.Start();
        }

        private void timer1_Tick(object sender, EventArgs e) // extra el mensaje (hexa del ebyte) y lo almacena visualmente en los tx box
        {
            if (cb_module.SelectedItem == "E22")
            {
                String mensaje = terminalLb.Text;
                if (mensaje.Contains("C1") && mensaje.Length > 10)
                {
                    String[] partes = mensaje.Split('-');
                    //tx_NI.Text = partes[1] + partes[2] + partes[3] + partes[4] + partes[5] + partes[6] + partes[7] + partes[8] + partes[9] + partes[10] + partes[11] + partes[12];
                    timer_get_info.Stop();

                    // addres direccion del modulo 
                    string addres = partes[4] + " " + partes[5];

                    // int addres_0 = Convert.ToInt32(addres, 16);
                    tx_addres.Text = Convert.ToString(addres);
                    check_addres = tx_addres.Text; // valor de cambio para visualizacion


                    // extraccion de datos de net id
                    int Net_ID = Convert.ToUInt16(partes[6], 16);
                    tx_net_id.Text = Convert.ToString(Net_ID);
                    check_net_id = tx_net_id.Text;

                    //comienzo de la configuracion de lectura en addres 03H
                    string binary_03H = Convert.ToString(Convert.ToInt32(partes[7], 16), 2); //03H

                    while (binary_03H.Length < 8)
                    {
                        binary_03H = "0" + binary_03H;
                    }

                    binary_03H = "b" + binary_03H;
                    int delimitador;
                    string binary_03H_baudrate = "";
                    string binary_03H_parity = "";
                    string binary_03H_airRate = "";
                    //  MessageBox.Show(binary_03H);
                    try
                    {
                        delimitador = binary_03H.IndexOf('b');
                        binary_03H_baudrate = binary_03H.Substring(delimitador + 1, 3);
                        binary_03H_parity = binary_03H.Substring(delimitador + 4, 2);
                        binary_03H_airRate = binary_03H.Substring(delimitador + 6, 3);
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }

                    // MessageBox.Show(partes[4]);   
                    //-C1-00-09-1D-4D-0A-62-00-02-03-00-00
                    //-C1-00-09-F8-0A-68-02-03-00-00-00-00
                    // 9600 8n1 2.4

                    //configuracion para baud rate
                    if (binary_03H_baudrate == "000")
                    {
                        CB_baudRate.Text = "1200";
                    }
                    if (binary_03H_baudrate == "001")
                    {
                        CB_baudRate.Text = "2400";
                    }
                    if (binary_03H_baudrate == "010")
                    {
                        CB_baudRate.Text = "4800";
                    }
                    if (binary_03H_baudrate == "011")
                    {
                        CB_baudRate.Text = "9600";
                    }
                    if (binary_03H_baudrate == "100")
                    {
                        CB_baudRate.Text = "19200";
                    }
                    if (binary_03H_baudrate == "101")
                    {
                        CB_baudRate.Text = "38400";
                    }
                    if (binary_03H_baudrate == "110")
                    {
                        CB_baudRate.Text = "57600";
                    }
                    if (binary_03H_baudrate == "111")
                    {
                        CB_baudRate.Text = "115200";
                    }
                    check_baud = CB_baudRate.Text;

                    //configuracion 04H
                    string binary_04H = Convert.ToString(Convert.ToInt32(partes[8], 16), 2); //03H

                    while (binary_04H.Length < 8)
                    {
                        binary_04H = "0" + binary_04H;
                    }

                    int delimitador_04H = binary_04H.IndexOf('b');
                    string binary_04H_Psize = binary_04H.Substring(delimitador_04H + 1, 2);
                    string binary_04H_RSSI = binary_04H.Substring(delimitador_04H + 3, 1);
                    string binary_04H_power = binary_04H.Substring(delimitador_04H + 7, 2);

                    //configuracion de packet size 

                    if (binary_04H_Psize == "00")
                    {
                        cb_Psize.Text = "240 bytes";
                    }
                    if (binary_04H_Psize == "01")
                    {
                        cb_Psize.Text = "128 bytes";
                    }
                    if (binary_04H_Psize == "10")
                    {
                        cb_Psize.Text = "64 bytes";
                    }
                    if (binary_04H_Psize == "11")
                    {
                        cb_Psize.Text = "32 bytes";
                    }
                    check_Psize = cb_Psize.Text;

                    //configuracion rssi

                    if (binary_04H_RSSI == "0")
                    {
                        cb_Crssi.Text = "Disable";
                    }
                    if (binary_04H_RSSI == "1")
                    {
                        cb_Crssi.Text = "Enable";
                    }
                    check_crssi = cb_Crssi.Text;

                    //configuracion power
                    if (binary_04H_power == "00")
                    {
                        cb_power.Text = "30dBm";
                    }
                    if (binary_04H_power == "01")
                    {
                        cb_power.Text = "27dBm";
                    }
                    if (binary_04H_power == "10")
                    {
                        cb_power.Text = "24dBm";
                    }
                    if (binary_04H_power == "11")
                    {
                        cb_power.Text = "21dBm";
                    }
                    check_power = cb_power.Text;

                    //configuracion para parity
                    if (binary_03H_parity == "10")
                    {
                        CB_PARITY.Text = "8E1";
                    }
                    if (binary_03H_parity == "01")
                    {
                        CB_PARITY.Text = "8O1";
                    }
                    if (binary_03H_parity == "00")
                    {
                        CB_PARITY.Text = "8N1";
                    }
                    check_parity = CB_PARITY.Text;

                    //configuracion para air rate
                    if (binary_03H_airRate == "000")
                    {
                        cb_air_rate.Text = "0.3K";
                    }
                    if (binary_03H_airRate == "001")
                    {
                        cb_air_rate.Text = "1.2K";
                    }
                    if (binary_03H_airRate == "010")
                    {
                        cb_air_rate.Text = "2.4K";
                    }
                    if (binary_03H_airRate == "011")
                    {
                        cb_air_rate.Text = "4.8K";
                    }
                    if (binary_03H_airRate == "100")
                    {
                        cb_air_rate.Text = "9.6K";
                    }
                    if (binary_03H_airRate == "101")
                    {
                        cb_air_rate.Text = "19.2K";
                    }
                    if (binary_03H_airRate == "110")
                    {
                        cb_air_rate.Text = "38.4K";
                    }
                    if (binary_03H_airRate == "111")
                    {
                        cb_air_rate.Text = "62.5K";
                    }
                    check_air = cb_air_rate.Text;

                    //channel Canal 05H
                    //int channel = Convert.ToUInt16(partes[9], 16);
                    tx_channel.Text = partes[9];
                    check_channel = tx_channel.Text;

                    //CONFIG 06H

                    string binary_06H = Convert.ToString(Convert.ToInt32(partes[10], 16), 2); //03H

                    while (binary_06H.Length < 8)
                    {
                        binary_06H = "0" + binary_06H;
                    }

                    int delimitador_06H = binary_06H.IndexOf('b');
                    string binary_06H_RSSI = binary_06H.Substring(delimitador_06H + 1, 1);
                    string binary_06H_FIXED = binary_06H.Substring(delimitador_06H + 2, 1);
                    string binary_06H_RElay = binary_06H.Substring(delimitador_06H + 3, 1);
                    string binary_06H_LBT = binary_06H.Substring(delimitador_06H + 4, 1);
                    string binary_06H_WORCONTROL = binary_06H.Substring(delimitador_06H + 5, 1);
                    string binary_06H_Wcycle = binary_06H.Substring(delimitador_06H + 6, 3);

                    //Packet rssi
                    if (binary_06H_RSSI == "0")
                    {
                        cb_Prssi.Text = "Disable";
                    }
                    if (binary_06H_RSSI == "1")
                    {
                        cb_Prssi.Text = "Enable";
                    }
                    check_RSSI = cb_Prssi.Text;

                    //tran mode
                    if (binary_06H_FIXED == "0")
                    {
                        cb_tranMode.Text = "Normal";
                    }
                    if (binary_06H_FIXED == "1")
                    {
                        cb_tranMode.Text = "Fixed";
                    }
                    check_fixed = cb_tranMode.Text;

                    //relay
                    if (binary_06H_RElay == "0")
                    {
                        cb_relay.Text = "Disable";
                    }
                    if (binary_06H_RElay == "1")
                    {
                        cb_relay.Text = "Enable";
                    }
                    check_relay = cb_relay.Text;

                    //LBT
                    if (binary_06H_LBT == "0")
                    {
                        cb_lbt.Text = "Disable";
                    }
                    if (binary_06H_LBT == "1")
                    {
                        cb_lbt.Text = "Enable";
                    }
                    check_LBT = cb_lbt.Text;

                    //wor_control
                    if (binary_06H_WORCONTROL == "0")
                    {
                        cb_Wrole.Text = "Recieve";
                    }
                    if (binary_06H_WORCONTROL == "1")
                    {
                        cb_Wrole.Text = "Translate";
                    }
                    check_Wrole = cb_Wrole.Text;

                    //wor cycle

                    if (binary_06H_Wcycle == "000")
                    {
                        cb_Wcycle.Text = "500ms";
                    }
                    if (binary_06H_Wcycle == "001")
                    {
                        cb_Wcycle.Text = "1000ms";
                    }
                    if (binary_06H_Wcycle == "010")
                    {
                        cb_Wcycle.Text = "1500ms";
                    }
                    if (binary_06H_Wcycle == "011")
                    {
                        cb_Wcycle.Text = "2000ms";
                    }
                    if (binary_06H_Wcycle == "100")
                    {
                        cb_Wcycle.Text = "2500ms";
                    }
                    if (binary_06H_Wcycle == "101")
                    {
                        cb_Wcycle.Text = "3000ms";
                    }
                    if (binary_06H_Wcycle == "110")
                    {
                        cb_Wcycle.Text = "3500ms";
                    }
                    if (binary_06H_Wcycle == "111")
                    {
                        cb_Wcycle.Text = "4000ms";
                    }
                    check_Wcycle = cb_Wcycle.Text;

                    // key 
                    string key = partes[11] + partes[12];

                    int key_0 = Convert.ToInt32(key, 16);


                    tx_key.Text = Convert.ToString(key_0);
                    check_Key = tx_key.Text;
                }

            }
            if (cb_module.SelectedItem == "E220")
            {
                String mensaje = terminalLb.Text;
                if (mensaje.Contains("C1") && mensaje.Length > 10)
                {
                    String[] partes = mensaje.Split('-');
                    //tx_NI.Text = partes[1] + partes[2] + partes[3] + partes[4] + partes[5] + partes[6] + partes[7] + partes[8] + partes[9] + partes[10] + partes[11] + partes[12];
                    timer_get_info.Stop();

                    // addres direccion del modulo 
                    string addres = partes[4] + " " + partes[5];



                    // int addres_0 = Convert.ToInt32(addres, 16);
                    tx_addres.Text = Convert.ToString(addres);
                    check_addres = tx_addres.Text; // valor de cambio para visualizacion


                    //comienzo de la configuracion de lectura en addres 03H
                    string binary_03H = Convert.ToString(Convert.ToInt32(partes[6], 16), 2); //03H

                    while (binary_03H.Length < 8)
                    {
                        binary_03H = "0" + binary_03H;
                    }

                    binary_03H = "b" + binary_03H;
                    int delimitador;
                    string binary_03H_baudrate = "";
                    string binary_03H_parity = "";
                    string binary_03H_airRate = "";
                    //  MessageBox.Show(binary_03H);
                    try
                    {
                        delimitador = binary_03H.IndexOf('b');
                        binary_03H_baudrate = binary_03H.Substring(delimitador + 1, 3);
                        binary_03H_parity = binary_03H.Substring(delimitador + 4, 2);
                        binary_03H_airRate = binary_03H.Substring(delimitador + 6, 3);
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }

                    // MessageBox.Show(partes[4]);   
                    //-C1-00-09-1D-4D-0A-62-00-02-03-00-00
                    //-C1-00-09-F8-0A-68-02-03-00-00-00-00
                    // 9600 8n1 2.4

                    //configuracion para baud rate
                    if (binary_03H_baudrate == "000")
                    {
                        CB_baudRate.Text = "1200";
                    }
                    if (binary_03H_baudrate == "001")
                    {
                        CB_baudRate.Text = "2400";
                    }
                    if (binary_03H_baudrate == "010")
                    {
                        CB_baudRate.Text = "4800";
                    }
                    if (binary_03H_baudrate == "011")
                    {
                        CB_baudRate.Text = "9600";
                    }
                    if (binary_03H_baudrate == "100")
                    {
                        CB_baudRate.Text = "19200";
                    }
                    if (binary_03H_baudrate == "101")
                    {
                        CB_baudRate.Text = "38400";
                    }
                    if (binary_03H_baudrate == "110")
                    {
                        CB_baudRate.Text = "57600";
                    }
                    if (binary_03H_baudrate == "111")
                    {
                        CB_baudRate.Text = "115200";
                    }
                    check_baud = CB_baudRate.Text;

                    //configuracion 04H
                    string binary_04H = Convert.ToString(Convert.ToInt32(partes[7], 16), 2); //03H

                    while (binary_04H.Length < 8)
                    {
                        binary_04H = "0" + binary_04H;
                    }

                    int delimitador_04H = binary_04H.IndexOf('b');
                    string binary_04H_Psize = binary_04H.Substring(delimitador_04H + 1, 2);
                    string binary_04H_RSSI = binary_04H.Substring(delimitador_04H + 3, 1);
                    string binary_04H_power = binary_04H.Substring(delimitador_04H + 7, 2);

                    //configuracion de packet size 

                    if (binary_04H_Psize == "00")
                    {
                        cb_Psize.Text = "240 bytes";
                    }
                    if (binary_04H_Psize == "01")
                    {
                        cb_Psize.Text = "128 bytes";
                    }
                    if (binary_04H_Psize == "10")
                    {
                        cb_Psize.Text = "64 bytes";
                    }
                    if (binary_04H_Psize == "11")
                    {
                        cb_Psize.Text = "32 bytes";
                    }
                    check_Psize = cb_Psize.Text;

                    //configuracion rssi

                    if (binary_04H_RSSI == "0")
                    {
                        cb_Crssi.Text = "Disable";
                    }
                    if (binary_04H_RSSI == "1")
                    {
                        cb_Crssi.Text = "Enable";
                    }
                    check_crssi = cb_Crssi.Text;

                    //configuracion power
                    if (binary_04H_power == "00")
                    {
                        cb_power.Text = "30dBm";
                    }
                    if (binary_04H_power == "01")
                    {
                        cb_power.Text = "27dBm";
                    }
                    if (binary_04H_power == "10")
                    {
                        cb_power.Text = "24dBm";
                    }
                    if (binary_04H_power == "11")
                    {
                        cb_power.Text = "21dBm";
                    }
                    check_power = cb_power.Text;

                    //configuracion para parity
                    if (binary_03H_parity == "10")
                    {
                        CB_PARITY.Text = "8E1";
                    }
                    if (binary_03H_parity == "01")
                    {
                        CB_PARITY.Text = "8O1";
                    }
                    if (binary_03H_parity == "00")
                    {
                        CB_PARITY.Text = "8N1";
                    }
                    check_parity = CB_PARITY.Text;

                    //configuracion para air rate
                    if (binary_03H_airRate == "000")
                    {
                        cb_air_rate.Text = "0.3K";
                    }
                    if (binary_03H_airRate == "001")
                    {
                        cb_air_rate.Text = "1.2K";
                    }
                    if (binary_03H_airRate == "010")
                    {
                        cb_air_rate.Text = "2.4K";
                    }
                    if (binary_03H_airRate == "011")
                    {
                        cb_air_rate.Text = "4.8K";
                    }
                    if (binary_03H_airRate == "100")
                    {
                        cb_air_rate.Text = "9.6K";
                    }
                    if (binary_03H_airRate == "101")
                    {
                        cb_air_rate.Text = "19.2K";
                    }
                    if (binary_03H_airRate == "110")
                    {
                        cb_air_rate.Text = "38.4K";
                    }
                    if (binary_03H_airRate == "111")
                    {
                        cb_air_rate.Text = "62.5K";
                    }
                    check_air = cb_air_rate.Text;

                    //channel Canal 05H
                    // int channel = Convert.ToUInt16(partes[8], 16);
                    tx_channel.Text = partes[8];

                    check_channel = tx_channel.Text;

                    //CONFIG 06H

                    string binary_06H = Convert.ToString(Convert.ToInt32(partes[9], 16), 2); //03H

                    while (binary_06H.Length < 8)
                    {
                        binary_06H = "0" + binary_06H;
                    }

                    int delimitador_06H = binary_06H.IndexOf('b');
                    string binary_06H_RSSI = binary_06H.Substring(delimitador_06H + 1, 1);
                    string binary_06H_FIXED = binary_06H.Substring(delimitador_06H + 2, 1);
                    string binary_06H_RElay = binary_06H.Substring(delimitador_06H + 3, 1);
                    string binary_06H_LBT = binary_06H.Substring(delimitador_06H + 4, 1);
                    string binary_06H_WORCONTROL = binary_06H.Substring(delimitador_06H + 5, 1);
                    string binary_06H_Wcycle = binary_06H.Substring(delimitador_06H + 6, 3);

                    //Packet rssi
                    if (binary_06H_RSSI == "0")
                    {
                        cb_Prssi.Text = "Disable";
                    }
                    if (binary_06H_RSSI == "1")
                    {
                        cb_Prssi.Text = "Enable";
                    }
                    check_RSSI = cb_Prssi.Text;

                    //tran mode
                    if (binary_06H_FIXED == "0")
                    {
                        cb_tranMode.Text = "Normal";
                    }
                    if (binary_06H_FIXED == "1")
                    {
                        cb_tranMode.Text = "Fixed";
                    }
                    check_fixed = cb_tranMode.Text;

                    //relay
                    if (binary_06H_RElay == "0")
                    {
                        cb_relay.Text = "Disable";
                    }
                    if (binary_06H_RElay == "1")
                    {
                        cb_relay.Text = "Enable";
                    }
                    check_relay = cb_relay.Text;

                    //LBT
                    if (binary_06H_LBT == "0")
                    {
                        cb_lbt.Text = "Disable";
                    }
                    if (binary_06H_LBT == "1")
                    {
                        cb_lbt.Text = "Enable";
                    }
                    check_LBT = cb_lbt.Text;

                    //wor_control
                    if (binary_06H_WORCONTROL == "0")
                    {
                        cb_Wrole.Text = "Recieve";
                    }
                    if (binary_06H_WORCONTROL == "1")
                    {
                        cb_Wrole.Text = "Translate";
                    }
                    check_Wrole = cb_Wrole.Text;

                    //wor cycle

                    if (binary_06H_Wcycle == "000")
                    {
                        cb_Wcycle.Text = "500ms";
                    }
                    if (binary_06H_Wcycle == "001")
                    {
                        cb_Wcycle.Text = "1000ms";
                    }
                    if (binary_06H_Wcycle == "010")
                    {
                        cb_Wcycle.Text = "1500ms";
                    }
                    if (binary_06H_Wcycle == "011")
                    {
                        cb_Wcycle.Text = "2000ms";
                    }
                    if (binary_06H_Wcycle == "100")
                    {
                        cb_Wcycle.Text = "2500ms";
                    }
                    if (binary_06H_Wcycle == "101")
                    {
                        cb_Wcycle.Text = "3000ms";
                    }
                    if (binary_06H_Wcycle == "110")
                    {
                        cb_Wcycle.Text = "3500ms";
                    }
                    if (binary_06H_Wcycle == "111")
                    {
                        cb_Wcycle.Text = "4000ms";
                    }
                    check_Wcycle = cb_Wcycle.Text;

                    // key 
                    string key = partes[10] + partes[11];

                    int key_0 = Convert.ToInt32(key, 16);



                    tx_key.Text = Convert.ToString(key_0);
                    check_Key = tx_key.Text;
                }

            }
            if (cb_module.SelectedItem == "E32")
            {
                terminalLb.Text = terminalLb.Text.Replace("--", "-");
                String mensaje = terminalLb.Text;
                //MessageBox.Show(mensaje);
                if (mensaje.Contains("C0") && mensaje.Length > 4)
                {
                    String[] partes = mensaje.Split('-');//-C0-00-00-1A-06-40

                    //1A -> baud 03h
                    //06 -> channel
                    //40 -> fixed
                    // 00 00 -> addres

                    // addres direccion del modulo 
                    string addres = partes[2] + " " + partes[3];

                    // int addres_0 = Convert.ToInt32(addres, 16);
                    tx_addres.Text = Convert.ToString(addres);
                    check_addres = tx_addres.Text; // valor de cambio para visualizacion


                    // int channel = Convert.ToUInt16(partes[5], 16);
                    tx_channel.Text = Convert.ToString(partes[5]);
                    check_channel = tx_channel.Text;

                    //Configuracion de la columna 3 en el manual
                    string binary_03H = Convert.ToString(Convert.ToInt32(partes[4], 16), 2); //03H
                    while (binary_03H.Length < 8)
                    {
                        binary_03H = "0" + binary_03H;
                    }

                    binary_03H = "b" + binary_03H;
                    int delimitador;
                    string binary_03H_baudrate = "";
                    string binary_03H_parity = "";
                    string binary_03H_airRate = "";

                    try
                    {
                        delimitador = binary_03H.IndexOf('b');
                        binary_03H_baudrate = binary_03H.Substring(delimitador + 3, 3);
                        binary_03H_parity = binary_03H.Substring(delimitador + 1, 2);
                        binary_03H_airRate = binary_03H.Substring(delimitador + 6, 3);
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }
                    //configuracion para parity
                    if (binary_03H_parity == "10")
                    {
                        CB_PARITY.Text = "8E1";
                    }
                    if (binary_03H_parity == "01")
                    {
                        CB_PARITY.Text = "8O1";
                    }
                    if (binary_03H_parity == "00")
                    {
                        CB_PARITY.Text = "8N1";
                    }
                    check_parity = CB_PARITY.Text;

                    if (binary_03H_baudrate == "000")
                    {
                        CB_baudRate.Text = "1200";
                    }
                    if (binary_03H_baudrate == "001")
                    {
                        CB_baudRate.Text = "2400";
                    }
                    if (binary_03H_baudrate == "010")
                    {
                        CB_baudRate.Text = "4800";
                    }
                    if (binary_03H_baudrate == "011")
                    {
                        CB_baudRate.Text = "9600";
                    }
                    if (binary_03H_baudrate == "100")
                    {
                        CB_baudRate.Text = "19200";
                    }
                    if (binary_03H_baudrate == "101")
                    {
                        CB_baudRate.Text = "38400";
                    }
                    if (binary_03H_baudrate == "110")
                    {
                        CB_baudRate.Text = "57600";
                    }
                    if (binary_03H_baudrate == "111")
                    {
                        CB_baudRate.Text = "115200";
                    }
                    check_baud = CB_baudRate.Text;

                    //configuracion para air rate
                    if (binary_03H_airRate == "000")
                    {
                        cb_air_rate.Text = "0.3K";
                    }
                    if (binary_03H_airRate == "001")
                    {
                        cb_air_rate.Text = "1.2K";
                    }
                    if (binary_03H_airRate == "010")
                    {
                        cb_air_rate.Text = "2.4K";
                    }
                    if (binary_03H_airRate == "011")
                    {
                        cb_air_rate.Text = "4.8K";
                    }
                    if (binary_03H_airRate == "100")
                    {
                        cb_air_rate.Text = "9.6K";
                    }
                    if (binary_03H_airRate == "101")
                    {
                        cb_air_rate.Text = "19.2K";
                    }
                    if (binary_03H_airRate == "110")
                    {
                        cb_air_rate.Text = "38.4K";
                    }
                    if (binary_03H_airRate == "111")
                    {
                        cb_air_rate.Text = "62.5K";
                    }
                    check_air = cb_air_rate.Text;


                    //Configuracion de la columna 5 en el manual

                    string binary_06H = Convert.ToString(Convert.ToInt32(partes[6], 16), 2);


                    while (binary_06H.Length < 8)
                    {
                        binary_06H = "0" + binary_06H;
                    }

                    int delimitador_06H = binary_06H.IndexOf('b');

                    /*
                     * fixed 1
                     * io mode 1 
                     * wake up 3
                     * FEC 1 
                     * POWER 2
                    */
                    string binary_05H_FIXED = binary_06H.Substring(delimitador_06H + 1, 1);

                    string binary_05H_IO_MODE = binary_06H.Substring(delimitador_06H + 2, 1);

                    //string binary_06H_FIXED = binary_06H.Substring(delimitador_06H + 2, 1);

                    string binary_05H_WAKE_UP = binary_06H.Substring(delimitador_06H + 3, 3);

                    string binary_05H_FEC = binary_06H.Substring(delimitador_06H + 6, 1);

                    string binary_05H_POWER = binary_06H.Substring(delimitador_06H + 7, 2);

                    lb_TranMode.Text = "IO Mode";
                    lb_WorCycle.Text = "Wor Timing";
                    lb_WorROLE.Text = "Fixed Mode";
                    lb_PacketSize.Text = "FEC";


                    //FIXED MODE
                    if (binary_05H_FIXED == "0")
                    {
                        cb_Wrole.Text = "Disable";
                    }
                    if (binary_05H_FIXED == "1")
                    {
                        cb_Wrole.Text = "Enable";
                    }
                    check_fixed = cb_Wrole.Text;

                    //IO MODE
                    if (binary_05H_IO_MODE == "0")
                    {
                        cb_tranMode.Text = "OpenDrain";
                    }
                    if (binary_05H_IO_MODE == "1")
                    {
                        cb_tranMode.Text = "PushPull";
                    }
                    check_LBT = cb_tranMode.Text;

                    //WOR TIMING

                    if (binary_05H_WAKE_UP == "000")
                    {
                        cb_Wcycle.Text = "500ms";
                    }
                    if (binary_05H_WAKE_UP == "001")
                    {
                        cb_Wcycle.Text = "1000ms";
                    }
                    if (binary_05H_WAKE_UP == "010")
                    {
                        cb_Wcycle.Text = "1500ms";
                    }
                    if (binary_05H_WAKE_UP == "011")
                    {
                        cb_Wcycle.Text = "2000ms";
                    }
                    if (binary_05H_WAKE_UP == "100")
                    {
                        cb_Wcycle.Text = "2500ms";
                    }
                    if (binary_05H_WAKE_UP == "101")
                    {
                        cb_Wcycle.Text = "3000ms";
                    }
                    if (binary_05H_WAKE_UP == "110")
                    {
                        cb_Wcycle.Text = "3500ms";
                    }
                    if (binary_05H_WAKE_UP == "111")
                    {
                        cb_Wcycle.Text = "4000ms";
                    }
                    check_Wcycle = cb_Wcycle.Text;



                    //FEC
                    if (binary_05H_FEC == "0")
                    {
                        cb_Psize.Text = "Disable"; //Disable
                    }
                    if (binary_05H_FEC == "1")
                    {
                        cb_Psize.Text = "Enable"; //Enable
                    }
                    check_Wrole = cb_Psize.Text;


                    //configuracion power
                    if (binary_05H_POWER == "00")
                    {
                        cb_power.Text = "30dBm";
                    }
                    if (binary_05H_POWER == "01")
                    {
                        cb_power.Text = "27dBm";
                    }
                    if (binary_05H_POWER == "10")
                    {
                        cb_power.Text = "24dBm";
                    }
                    if (binary_05H_POWER == "11")
                    {
                        cb_power.Text = "21dBm";
                    }
                    check_power = cb_power.Text;




                    timer_get_info.Stop();

                }
            }


            timer_get_info.Stop();
        }

        private void tx_addres_KeyPress(object sender, KeyPressEventArgs e)
        {
            tx_addres.MaxLength = 5;
            if (!IsHexDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Ignora el carácter ingresado
            }

            string input = tx_addres.Text.ToUpper();

            // Eliminar todos los caracteres que no sean dígitos hexadecimales
            string cleanedInput = Regex.Replace(input, "[^0-9a-fA-F]", "");

            // Dividir el texto en grupos de dos caracteres
            string formattedOutput = "";
            int groupSize = 2;
            int startIndex = 0;

            while (startIndex + groupSize <= cleanedInput.Length)
            {
                formattedOutput += cleanedInput.Substring(startIndex, groupSize) + " ";
                startIndex += groupSize;
            }

            // Agregar el último grupo sin espacio adicional si hay un número impar de caracteres
            if (startIndex < cleanedInput.Length)
            {
                formattedOutput += cleanedInput.Substring(startIndex);
            }

            tx_addres.Text = formattedOutput.Trim();
            tx_addres.SelectionStart = tx_addres.Text.Length;
        }

        private void tx_channel_KeyPress(object sender, KeyPressEventArgs e)
        {
            tx_channel.MaxLength = 2;
            if (!IsHexDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Ignora el carácter ingresado
            }

            string input = tx_channel.Text.ToUpper();

            // Eliminar todos los caracteres que no sean dígitos hexadecimales
            string cleanedInput = Regex.Replace(input, "[^0-9a-fA-F]", "");

            // Dividir el texto en grupos de dos caracteres
            string formattedOutput = "";
            int groupSize = 2;
            int startIndex = 0;

            while (startIndex + groupSize <= cleanedInput.Length)
            {
                formattedOutput += cleanedInput.Substring(startIndex, groupSize) + " ";
                startIndex += groupSize;
            }

            // Agregar el último grupo sin espacio adicional si hay un número impar de caracteres
            if (startIndex < cleanedInput.Length)
            {
                formattedOutput += cleanedInput.Substring(startIndex);
            }

            tx_channel.Text = formattedOutput.Trim();
            tx_channel.SelectionStart = tx_channel.Text.Length;
        }

        private void tx_net_id_KeyPress(object sender, KeyPressEventArgs e)
        {
            tx_net_id.MaxLength = 5;
            if (!IsHexDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Ignora el carácter ingresado
            }

            string input = tx_net_id.Text.ToUpper();

            // Eliminar todos los caracteres que no sean dígitos hexadecimales
            string cleanedInput = Regex.Replace(input, "[^0-9a-fA-F]", "");

            // Dividir el texto en grupos de dos caracteres
            string formattedOutput = "";
            int groupSize = 2;
            int startIndex = 0;

            while (startIndex + groupSize <= cleanedInput.Length)
            {
                formattedOutput += cleanedInput.Substring(startIndex, groupSize) + " ";
                startIndex += groupSize;
            }

            // Agregar el último grupo sin espacio adicional si hay un número impar de caracteres
            if (startIndex < cleanedInput.Length)
            {
                formattedOutput += cleanedInput.Substring(startIndex);
            }

            tx_net_id.Text = formattedOutput.Trim();
            tx_net_id.SelectionStart = tx_net_id.Text.Length;
        }

        private void tx_key_KeyPress(object sender, KeyPressEventArgs e)
        {
            tx_key.MaxLength = 5;
            if (!IsHexDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Ignora el carácter ingresado
            }

            string input = tx_key.Text.ToUpper();

            // Eliminar todos los caracteres que no sean dígitos hexadecimales
            string cleanedInput = Regex.Replace(input, "[^0-9a-fA-F]", "");

            // Dividir el texto en grupos de dos caracteres
            string formattedOutput = "";
            int groupSize = 2;
            int startIndex = 0;

            while (startIndex + groupSize <= cleanedInput.Length)
            {
                formattedOutput += cleanedInput.Substring(startIndex, groupSize) + " ";
                startIndex += groupSize;
            }

            // Agregar el último grupo sin espacio adicional si hay un número impar de caracteres
            if (startIndex < cleanedInput.Length)
            {
                formattedOutput += cleanedInput.Substring(startIndex);
            }

            tx_key.Text = formattedOutput.Trim();
            tx_key.SelectionStart = tx_key.Text.Length;
        }
        string mac_id_3A = "";
        private void TB_MAC_ID_Leave(object sender, EventArgs e)
        {
            TB_MAC_ID.Text = TB_MAC_ID.Text.Replace(" ", "");
            string texto = TB_MAC_ID.Text; // Tu cadena de entrada
            StringBuilder resultado = new StringBuilder();

            bool esImpar = TB_MAC_ID.Text.Length % 2 != 0;
            //MessageBox.Show(Convert.ToString(TB_MAC_ID.Text.Length));


            if (esImpar == true || TB_MAC_ID.Text.Length != 16 && TB_MAC_ID.Text.Length != 32)
            {
                if (TB_MAC_ID.Text.Length > 16)
                {
                    lb_error_mac_id.Text = "Demasiados caracteres";// + Convert.ToString(TB_MAC_ID.Text.Length); ;
                    lb_error_mac_id.ForeColor = Color.Red;
                    lb_mac_id.Text = "MAC ID";
                }
                else
                {


                    lb_error_mac_id.Text = "caracteres incompletos";//+ Convert.ToString(TB_MAC_ID.Text.Length); ;
                    lb_error_mac_id.ForeColor = Color.Red;
                    lb_mac_id.Text = "MAC ID";
                }
            }

            else
            {
                lb_mac_id.Text = "MAC ID ✔";
                lb_error_mac_id.Text = "";
                //lb_error_mac_id.ForeColor = Color.Green;
                //lb_error_mac_id.Text = Convert.ToString(TB_MAC_ID.Text.Length);


                for (int i = 0; i < texto.Length; i += 2)
                {
                    // Obtén el par de caracteres actual
                    string parCaracteres = texto.Substring(i, Math.Min(2, texto.Length - i));

                    // Agrega el par de caracteres actual y "3A" al resultado
                    resultado.Append(parCaracteres + "3A");
                }

                string resultadoFinal = resultado.ToString();
                TB_MAC_ID.Text = resultadoFinal;
                TB_MAC_ID.Text = TB_MAC_ID.Text.Replace("3A3A3A", " 3A ");
                mac_id_3A = TB_MAC_ID.Text;
                TB_MAC_ID.Text = TB_MAC_ID.Text.Replace(" 3A ", " ");
            }
        }

        private void maclb_TextChanged(object sender, EventArgs e)
        {

        }

        private void pic_baud_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The serial interface baud rate for communication between the serial port of the module and the host.", "Descroption", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pic_parity_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The parity for the UART. Both parities must be the same.", "Description", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pic_air_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The higher the rate, the shorter the distance", "Description", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pic_pack_Click(object sender, EventArgs e)
        {
            MessageBox.Show("When the data is smaller than the sub packet lenght, the serial output of the receiving end is an uninterrupted continuous output. When the data is larger than the sub packet lenght, the receiving end serial port will output the sub packet.", "Description", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pic_Wrole_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Role of sleep mode", "Description", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void picWcycle_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Valid only for mode 1. Cycle to turn on the antenna and listen", "Description", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pic_power_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Transmitting power", "Description", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pic_Tmode_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Fixed point transmission", "Description", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pic_Relay_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Repeater Function", "Description", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pic_LBT_Click(object sender, EventArgs e)
        {
            MessageBox.Show("LBT enable byte (monitor before transmission). Note: may cause data relay", "Description", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pic_Crsi_Click(object sender, EventArgs e)
        {
            MessageBox.Show("RSSI Ambient noise", "Description", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pic_Prsi_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Enable RSSI", "Description", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TB_MAC_ID_TextChange(object sender, EventArgs e)
        {

        }

        private void TB_MAC_ID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!IsHexDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Ignora el carácter ingresado
            }

            string input = TB_MAC_ID.Text.ToUpper();

            // Eliminar todos los caracteres que no sean dígitos hexadecimales
            string cleanedInput = Regex.Replace(input, "[^0-9a-fA-F]", "");

            // Dividir el texto en grupos de dos caracteres
            string formattedOutput = "";
            int groupSize = 2;
            int startIndex = 0;

            while (startIndex + groupSize <= cleanedInput.Length)
            {
                formattedOutput += cleanedInput.Substring(startIndex, groupSize) + " ";
                startIndex += groupSize;
            }

            // Agregar el último grupo sin espacio adicional si hay un número impar de caracteres
            if (startIndex < cleanedInput.Length)
            {
                formattedOutput += cleanedInput.Substring(startIndex);
            }

            TB_MAC_ID.Text = formattedOutput.Trim();
            TB_MAC_ID.SelectionStart = TB_MAC_ID.Text.Length;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            CB_PARITY.Width = panel_Conf.Width - 60;
            CB_baudRate.Width = panel_Conf.Width - 60;
            cb_air_rate.Width = panel_Conf.Width - 60;
            cb_Psize.Width = panel_Conf.Width - 60;
            cb_Wrole.Width = panel_Conf.Width - 60;
            cb_Wcycle.Width = panel_Conf.Width - 60;
            cb_power.Width = panel_Conf.Width - 60;
            cb_tranMode.Width = panel_Conf.Width - 60;
            cb_relay.Width = panel_Conf.Width - 60;
            cb_lbt.Width = panel_Conf.Width - 60;
            cb_Crssi.Width = panel_Conf.Width - 60;
            cb_Prssi.Width = panel_Conf.Width - 60;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            try
            {
                Conexion();
                // Verificar si la conexión fue exitosa
                if (client != null)
                {
                    try
                    {
                        FirebaseResponse response = client.Get("Mac_address");

                        // Verificar si el nodo existe y tiene datos
                        if (response.Body != "null")
                        {
                            FireBase.MacID1 = response.Body;
                            // FireBase.Show();
                            string mensaje = response.Body;

                            string mac = "11 24 B3 11 52 ";
                            string mac_2 = "00 00 00";
                            int input = 0; // Asigna el valor adecuado para tu caso (número de intentos)

                            while (input == 0)
                            {
                                if (mensaje.Contains(mac + mac_2))
                                {
                                    //mac_2 = "00 00 02";
                                    mac_2 = IncrementarMacString(mac_2);

                                    //MessageBox.Show(mac + mac_2 + " sigo");

                                }
                                else
                                {
                                    input = 1; // Terminar el bucle si no se encuentra la mac_2
                                    TB_MAC_ID.Text = mac + mac_2;
                                   // MessageBox.Show("sali");
                                    // Si el ID no existe, entonces procedemos a agregar el nuevo nodo
                                    DateTime fechaActual = DateTime.Now;
                                    string fechaFormateada = fechaActual.ToString("g");

                                    // Datos que deseas agregar al nuevo nodo
                                    var data = new
                                    {
                                        ID = "",
                                        FECHA = fechaFormateada,
                                        Type = lb_module_type.Text
                                    };

                                    // La ruta donde se agregará el nuevo nodo (por ejemplo, "Mac_address/nuevo_nodo")
                                    string nuevaRuta = "Mac_address/" + TB_MAC_ID.Text;

                                    // Realizar la operación de escritura en la base de datos
                                    response = client.Set(nuevaRuta, data);

                                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                    {
                                        MessageBox.Show("Nuevo nodo agregado correctamente");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Error al agregar el nuevo nodo");
                                    }
                                }
                            }

                        }
                        else
                        {
                            MessageBox.Show("No hay datos en el nodo especificado");
                        }
                    }
                    catch (FireSharp.Exceptions.FirebaseException ex)
                    {
                        // Manejar el error específico de Firebase
                        MessageBox.Show("Error de Firebase: " + ex.Message);

                    }
                }
                else
                {
                    MessageBox.Show("Error al conectar a Firebase Realtime Database");

                }

            }
            catch (Exception ex)
            {
                // Manejar cualquier otra excepción que pueda ocurrir durante la conexión o el proceso
                MessageBox.Show("Error general: " + ex.Message + " Error al conectar a Firebase Realtime Database");
                maclb.Text = ex.Message;
            }

            if (puerto.IsOpen)
            {


                byte[] ni_start = { 0x25, 0x4E, 0x49, 0x3D };
                byte[] node_id = Encoding.UTF8.GetBytes(tx_NI.Text);
                //Usuario
                byte[] mac_start = { 0x2c, 0x4D, 0x41, 0x43, 0x3D };
                byte[] mac = ConvertHexStringToByteArray(mac_id_3A.Replace(" ", ""));
                byte[] final = { 0x25 };
                //8F 00 0A 25 4E 49 3D ,2C 4D 41 43 3D 25 A7

                //Limpiar suma

                byte[] mensaje_byte = ni_start.Concat(node_id).Concat(mac_start).Concat(mac).Concat(final).ToArray();
                int checksum = 0;
                foreach (byte b in mensaje_byte)
                {
                    checksum += b;
                }

                checksum = ~checksum;

                byte[] checksumByte = { (byte)(checksum & 0xFF) };
                //here

                int tamaño = mensaje_byte.Length;
                string Tamaño_maclb = tamaño.ToString("X");
                byte[] tamaño_mensaje_byte = { 0x8F, 0x00, byte.Parse(Tamaño_maclb, System.Globalization.NumberStyles.HexNumber) };


                //este byte es el que lleva toda la informacion concatenada
                byte[] mensaje_byte_tamaño = tamaño_mensaje_byte.Concat(ni_start).Concat(node_id).Concat(mac_start).Concat(mac).Concat(final).Concat(checksumByte).ToArray();
                
                string mensaje_byte_tamaño_string = BitConverter.ToString(mensaje_byte_tamaño).Replace("-", " ");
                string frame_0 = mensaje_byte_tamaño_string.Replace("2C", ",2C");

                //tx_NI.Text = "";

                string[] Frame = frame_0.Split(',');

                if (lb_mac_id.Text == "MAC ID ✔" || TB_MAC_ID.Text.Length == 23)
                {
                    maclb.Text = Frame[0];
                    lb_frame_1.Text = Frame[1];
                    puerto.Write(mensaje_byte_tamaño, 0, mensaje_byte_tamaño.Length);////////////////
                    MessageBox.Show(BitConverter.ToString(mensaje_byte_tamaño));
                }
                else
                {
                    MessageBox.Show("Mac ID diferente de 8 bytes" + TB_MAC_ID.Text.Length.ToString());
                }

            }
            else
            {
                MessageBox.Show("Port closed", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
               
            }
        }
        string checj_addres_set;
        private void timer_set_Tick(object sender, EventArgs e) //envia las configuraciones de los modulos (con base a lo indicado en los textbox)
        {
            if (cb_module.SelectedItem == "E22")
            {
                String mensaje = terminalLb.Text;
                if (mensaje.Contains("C1") && mensaje.Length > 10)
                {
                    String[] partes = mensaje.Split('-');
                    //tx_NI.Text = partes[1] + partes[2] + partes[3] + partes[4] + partes[5] + partes[6] + partes[7] + partes[8] + partes[9] + partes[10] + partes[11] + partes[12];
                    timer_get_info.Stop();

                    // addres direccion del modulo 
                    string addres = partes[4] + " " + partes[5];

                    // int addres_0 = Convert.ToInt32(addres, 16);
                    tx_addres.Text = Convert.ToString(addres);
                    check_addres = tx_addres.Text; // valor de cambio para visualizacion


                    // extraccion de datos de net id
                    int Net_ID = Convert.ToUInt16(partes[6], 16);
                    tx_net_id.Text = Convert.ToString(Net_ID);
                    check_net_id = tx_net_id.Text;

                    //comienzo de la configuracion de lectura en addres 03H
                    string binary_03H = Convert.ToString(Convert.ToInt32(partes[7], 16), 2); //03H
                    while (binary_03H.Length < 8)
                    {
                        binary_03H = "0" + binary_03H;
                    }

                    binary_03H = "b" + binary_03H;
                    int delimitador;
                    string binary_03H_baudrate = "";
                    string binary_03H_parity = "";
                    string binary_03H_airRate = "";
                    //  MessageBox.Show(binary_03H);
                    try
                    {
                        delimitador = binary_03H.IndexOf('b');
                        binary_03H_baudrate = binary_03H.Substring(delimitador + 1, 3);
                        binary_03H_parity = binary_03H.Substring(delimitador + 4, 2);
                        binary_03H_airRate = binary_03H.Substring(delimitador + 6, 3);
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }

                    // MessageBox.Show(partes[4]);   
                    //-C1-00-09-1D-4D-0A-62-00-02-03-00-00
                    //-C1-00-09-F8-0A-68-02-03-00-00-00-00
                    // 9600 8n1 2.4

                    //configuracion para baud rate
                    if (binary_03H_baudrate == "000")
                    {
                        CB_baudRate.Text = "1200";
                    }
                    if (binary_03H_baudrate == "001")
                    {
                        CB_baudRate.Text = "2400";
                    }
                    if (binary_03H_baudrate == "010")
                    {
                        CB_baudRate.Text = "4800";
                    }
                    if (binary_03H_baudrate == "011")
                    {
                        CB_baudRate.Text = "9600";
                    }
                    if (binary_03H_baudrate == "100")
                    {
                        CB_baudRate.Text = "19200";
                    }
                    if (binary_03H_baudrate == "101")
                    {
                        CB_baudRate.Text = "38400";
                    }
                    if (binary_03H_baudrate == "110")
                    {
                        CB_baudRate.Text = "57600";
                    }
                    if (binary_03H_baudrate == "111")
                    {
                        CB_baudRate.Text = "115200";
                    }
                    check_baud = CB_baudRate.Text;

                    //configuracion 04H
                    string binary_04H = Convert.ToString(Convert.ToInt32(partes[8], 16), 2); //03H

                    while (binary_04H.Length < 8)
                    {
                        binary_04H = "0" + binary_04H;
                    }

                    int delimitador_04H = binary_04H.IndexOf('b');
                    string binary_04H_Psize = binary_04H.Substring(delimitador_04H + 1, 2);
                    string binary_04H_RSSI = binary_04H.Substring(delimitador_04H + 3, 1);
                    string binary_04H_power = binary_04H.Substring(delimitador_04H + 7, 2);

                    //configuracion de packet size 

                    if (binary_04H_Psize == "00")
                    {
                        cb_Psize.Text = "240 bytes";
                    }
                    if (binary_04H_Psize == "01")
                    {
                        cb_Psize.Text = "128 bytes";
                    }
                    if (binary_04H_Psize == "10")
                    {
                        cb_Psize.Text = "64 bytes";
                    }
                    if (binary_04H_Psize == "11")
                    {
                        cb_Psize.Text = "32 bytes";
                    }
                    check_Psize = cb_Psize.Text;

                    //configuracion rssi

                    if (binary_04H_RSSI == "0")
                    {
                        cb_Crssi.Text = "Disable";
                    }
                    if (binary_04H_RSSI == "1")
                    {
                        cb_Crssi.Text = "Enable";
                    }
                    check_crssi = cb_Crssi.Text;

                    //configuracion power
                    if (binary_04H_power == "00")
                    {
                        cb_power.Text = "30dBm";
                    }
                    if (binary_04H_power == "01")
                    {
                        cb_power.Text = "27dBm";
                    }
                    if (binary_04H_power == "10")
                    {
                        cb_power.Text = "24dBm";
                    }
                    if (binary_04H_power == "11")
                    {
                        cb_power.Text = "21dBm";
                    }
                    check_power = cb_power.Text;

                    //configuracion para parity
                    if (binary_03H_parity == "10")
                    {
                        CB_PARITY.Text = "8E1";
                    }
                    if (binary_03H_parity == "01")
                    {
                        CB_PARITY.Text = "8O1";
                    }
                    if (binary_03H_parity == "00")
                    {
                        CB_PARITY.Text = "8N1";
                    }
                    check_parity = CB_PARITY.Text;

                    //configuracion para air rate
                    if (binary_03H_airRate == "000")
                    {
                        cb_air_rate.Text = "0.3K";
                    }
                    if (binary_03H_airRate == "001")
                    {
                        cb_air_rate.Text = "1.2K";
                    }
                    if (binary_03H_airRate == "010")
                    {
                        cb_air_rate.Text = "2.4K";
                    }
                    if (binary_03H_airRate == "011")
                    {
                        cb_air_rate.Text = "4.8K";
                    }
                    if (binary_03H_airRate == "100")
                    {
                        cb_air_rate.Text = "9.6K";
                    }
                    if (binary_03H_airRate == "101")
                    {
                        cb_air_rate.Text = "19.2K";
                    }
                    if (binary_03H_airRate == "110")
                    {
                        cb_air_rate.Text = "38.4K";
                    }
                    if (binary_03H_airRate == "111")
                    {
                        cb_air_rate.Text = "62.5K";
                    }
                    check_air = cb_air_rate.Text;

                    //channel Canal 05H
                    //  int channel = Convert.ToUInt16(partes[9], 16);
                    tx_channel.Text = partes[9];
                    check_channel = tx_channel.Text;

                    //CONFIG 06H

                    string binary_06H = Convert.ToString(Convert.ToInt32(partes[10], 16), 2); //03H

                    while (binary_06H.Length < 8)
                    {
                        binary_06H = "0" + binary_06H;
                    }

                    int delimitador_06H = binary_06H.IndexOf('b');
                    string binary_06H_RSSI = binary_06H.Substring(delimitador_06H + 1, 1);
                    string binary_06H_FIXED = binary_06H.Substring(delimitador_06H + 2, 1);
                    string binary_06H_RElay = binary_06H.Substring(delimitador_06H + 3, 1);
                    string binary_06H_LBT = binary_06H.Substring(delimitador_06H + 4, 1);
                    string binary_06H_WORCONTROL = binary_06H.Substring(delimitador_06H + 5, 1);
                    string binary_06H_Wcycle = binary_06H.Substring(delimitador_06H + 6, 3);

                    //Packet rssi
                    if (binary_06H_RSSI == "0")
                    {
                        cb_Prssi.Text = "Disable";
                    }
                    if (binary_06H_RSSI == "1")
                    {
                        cb_Prssi.Text = "Enable";
                    }
                    check_RSSI = cb_Prssi.Text;

                    //tran mode
                    if (binary_06H_FIXED == "0")
                    {
                        cb_tranMode.Text = "Normal";
                    }
                    if (binary_06H_FIXED == "1")
                    {
                        cb_tranMode.Text = "Fixed";
                    }
                    check_fixed = cb_tranMode.Text;

                    //relay
                    if (binary_06H_RElay == "0")
                    {
                        cb_relay.Text = "Disable";
                    }
                    if (binary_06H_RElay == "1")
                    {
                        cb_relay.Text = "Enable";
                    }
                    check_relay = cb_relay.Text;

                    //LBT
                    if (binary_06H_LBT == "0")
                    {
                        cb_lbt.Text = "Disable";
                    }
                    if (binary_06H_LBT == "1")
                    {
                        cb_lbt.Text = "Enable";
                    }
                    check_LBT = cb_lbt.Text;

                    //wor_control
                    if (binary_06H_WORCONTROL == "0")
                    {
                        cb_Wrole.Text = "Recieve";
                    }
                    if (binary_06H_WORCONTROL == "1")
                    {
                        cb_Wrole.Text = "Translate";
                    }
                    check_Wrole = cb_Wrole.Text;

                    //wor cycle

                    if (binary_06H_Wcycle == "000")
                    {
                        cb_Wcycle.Text = "500ms";
                    }
                    if (binary_06H_Wcycle == "001")
                    {
                        cb_Wcycle.Text = "1000ms";
                    }
                    if (binary_06H_Wcycle == "010")
                    {
                        cb_Wcycle.Text = "1500ms";
                    }
                    if (binary_06H_Wcycle == "011")
                    {
                        cb_Wcycle.Text = "2000ms";
                    }
                    if (binary_06H_Wcycle == "100")
                    {
                        cb_Wcycle.Text = "2500ms";
                    }
                    if (binary_06H_Wcycle == "101")
                    {
                        cb_Wcycle.Text = "3000ms";
                    }
                    if (binary_06H_Wcycle == "110")
                    {
                        cb_Wcycle.Text = "3500ms";
                    }
                    if (binary_06H_Wcycle == "111")
                    {
                        cb_Wcycle.Text = "4000ms";
                    }
                    check_Wcycle = cb_Wcycle.Text;

                    // key 
                    string key = partes[11] + partes[12];

                    int key_0 = Convert.ToInt32(key, 16);


                    tx_key.Text = Convert.ToString(key_0);
                    check_Key = tx_key.Text;
                }
                timer_set.Stop();
            }
        
        }

        private void CB_baudRate_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {


        }

        private void bunifuLabel4_Click(object sender, EventArgs e)
        {

        }

        private void timer2_get_Tick(object sender, EventArgs e) //envia el mensaje para obtener la caracteristicas del modulo 
        {

            terminalLb.Text = "";
            timer2_get.Stop();
            if (puerto.IsOpen)
            {
                byte[] hexMessage = { 0xC1, 0x00, 0x09 };
                puerto.Write(hexMessage, 0, hexMessage.Length);
            }
            else
            {
                MessageBox.Show("Port closed", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            timer_get_info.Start();
        }

        private void timer_module_Tick(object sender, EventArgs e) //timer.- es utilizado para verificar el tipo del modulo (obtiene el nombre del modulo)
        {
            if (cb_module.SelectedItem == "E22")
            {
                timer_module.Stop();
                if (terminalLb.Text.Length >= 21)
                {
                    string hex = terminalLb.Text;

                    if (terminalLb.Text.Contains("-44-45-56-54-59-50-45-"))
                    {
                        hex = terminalLb.Text;
                        hex = hex.Replace("-", ""); // Elimina los guiones del string hex

                        byte[] bytes = new byte[hex.Length / 2];
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                        }

                        string result = Encoding.ASCII.GetString(bytes);
                        lb_module_type.Text = result;
                        terminalLb.Text = "";
                    }
                }
                timer2_get.Start();
            }
            if (cb_module.SelectedItem == "E32")
            {

                if (terminalLb.Text.Length >= 21)
                {
                    string hex = terminalLb.Text;

                    if (terminalLb.Text.Contains("-44-45-56-54-59-50-45-"))
                    {
                        hex = terminalLb.Text;
                        hex = hex.Replace("-", ""); // Elimina los guiones del string hex

                        byte[] bytes = new byte[hex.Length / 2];
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                        }

                        string result = Encoding.ASCII.GetString(bytes);
                        lb_module_type.Text = result;
                        terminalLb.Text = "";
                        timer_module.Stop();
                    }
                }
                timer2_get.Start();

            }
            if (cb_module.SelectedItem == "E220")
            {

                if (terminalLb.Text.Length >= 21)
                {
                    string hex = terminalLb.Text;

                    if (terminalLb.Text.Contains("-44-45-56-54-59-50-45-"))
                    {
                        hex = terminalLb.Text;
                        hex = hex.Replace("-", ""); // Elimina los guiones del string hex

                        byte[] bytes = new byte[hex.Length / 2];
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                        }

                        string result = Encoding.ASCII.GetString(bytes);
                        lb_module_type.Text = result;
                        terminalLb.Text = "";
                        timer_module.Stop();
                    }
                }
                timer2_get.Start();

            }

        }

        private void CB_PARITY_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void cb_module_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (puerto.IsOpen)
            {
                puerto.WriteLine("@AT+CONF=" + cb_module.SelectedItem + "@");
                MessageBox.Show("@AT+CONF=" + cb_module.SelectedItem + "@");
            }
            
            if (cb_module.SelectedItem == "E22")
            {
                tx_net_id.Show();
                lb_netID.Show();


                tx_net_id.Show();
                tx_key.Show();
                tx_key.Location = new Point(15, 285);
                pic_key.Location = new Point(39, 265);
                pic_key.Show();
                lb_netID.Show();
                bunifuPanel2.Size = new Size(300, 343);

                //panel de configuracion
                cb_Prssi.Show();
                bunifuLabel19.Show();
                pic_Prsi.Show();

                cb_Crssi.Show();
                lb_rssi.Show();
                pic_Crsi.Show();

                lb_LBT.Show();
                cb_lbt.Show();
                pic_LBT.Show();

                cb_relay.Show();
                pic_Relay.Show();
                lb_relay.Show();

                lb_TranMode.Text = "Tran Mode";
                lb_WorCycle.Text = "Wor Cycle";
                lb_WorROLE.Text = "Wor Role";
                lb_PacketSize.Text = "Packet Size";

                bunifuLabel6.Location = new Point(14, 800);

                List<string> Items_IOMode = new List<string>
                    {
                        "Normal",
                        "Fixed"
                    };

                cb_tranMode.Items.Clear(); // Limpiar los elementos existentes 
                cb_tranMode.Items.AddRange(Items_IOMode.ToArray());

                List<string> nuevosItems = new List<string>
                    {
                        "32 bytes",
                        "64 bytes",
                        "128 bytes",
                        "240 bytes"
                    };

                // Asignar la lista de elementos al ComboBox
                cb_Psize.Items.Clear(); // Limpiar los elementos existentes (opcional)
                cb_Psize.Items.AddRange(nuevosItems.ToArray());

                List<string> Items_Wrole = new List<string>
                    {
                        "Recieve",
                        "Translate"
                    };

                // Asignar la lista de elementos al ComboBox
                cb_Wrole.Items.Clear();
                cb_Wrole.Items.AddRange(Items_Wrole.ToArray());
            }
            if (cb_module.SelectedItem == "E220")
            {

                tx_key.Show();
                pic_key.Show();


                //panel de configuracion
                cb_Prssi.Show();
                bunifuLabel19.Show();
                pic_Prsi.Show();

                cb_Crssi.Show();
                lb_rssi.Show();
                pic_Crsi.Show();

                lb_LBT.Show();
                cb_lbt.Show();
                pic_LBT.Show();

                cb_relay.Show();
                pic_Relay.Show();
                lb_relay.Show();

                lb_TranMode.Text = "Tran Mode";
                lb_WorCycle.Text = "Wor Cycle";
                lb_WorROLE.Text = "Wor Role";
                lb_PacketSize.Text = "Packet Size";

                bunifuLabel6.Location = new Point(14, 800);

                tx_net_id.Hide();
                lb_netID.Hide();


                pic_key.Location = new Point(39, 204);
                tx_key.Location = new Point(15, 223);
                bunifuPanel2.Size = new Size(300, 300);

                List<string> Items_IOMode = new List<string>
                    {
                        "Normal",
                        "Fixed"
                    };

                cb_tranMode.Items.Clear(); // Limpiar los elementos existentes 
                cb_tranMode.Items.AddRange(Items_IOMode.ToArray());


                List<string> nuevosItems = new List<string>
                    {
                        "32 bytes",
                        "64 bytes",
                        "128 bytes",
                        "240 bytes"
                    };

                // Asignar la lista de elementos al ComboBox
                cb_Psize.Items.Clear(); // Limpiar los elementos existentes (opcional)
                cb_Psize.Items.AddRange(nuevosItems.ToArray());

                List<string> Items_Wrole = new List<string>
                    {
                        "Recieve",
                        "Translate"
                    };

                // Asignar la lista de elementos al ComboBox
                cb_Wrole.Items.Clear();
                cb_Wrole.Items.AddRange(Items_Wrole.ToArray());


            }
            if (cb_module.SelectedItem == "E32")
            {
                tx_net_id.Hide();
                tx_key.Hide();
                pic_key.Hide();
                lb_netID.Hide();
                bunifuPanel2.Size = new Size(300, 223);

                //panel de configuracion
                cb_Prssi.Hide();
                bunifuLabel19.Hide();
                pic_Prsi.Hide();

                cb_Crssi.Hide();
                lb_rssi.Hide();
                pic_Crsi.Hide();

                lb_LBT.Hide();
                cb_lbt.Hide();
                pic_LBT.Hide();

                cb_relay.Hide();
                pic_Relay.Hide();
                lb_relay.Hide();



                lb_TranMode.Text = "IO Mode";
                lb_WorCycle.Text = "Wor Timing";
                lb_WorROLE.Text = "Fixed Mode";
                lb_PacketSize.Text = "FEC";

                bunifuLabel6.Location = new Point(14, 540);

                List<string> nuevosItems = new List<string>
                    {
                        "Disable",
                        "Enable"
                    };

                // Asignar la lista de elementos al ComboBox
                cb_Psize.Items.Clear(); // Limpiar los elementos existentes (opcional)
                cb_Psize.Items.AddRange(nuevosItems.ToArray());

                List<string> Items_Wrole = new List<string>
                    {
                        "Disable",
                        "Enable"
                    };

                // Asignar la lista de elementos al ComboBox
                cb_Wrole.Items.Clear();
                cb_Wrole.Items.AddRange(Items_Wrole.ToArray());

                List<string> Items_IOMode = new List<string>
                    {
                        "OpenDrain",
                        "PushPull"
                    };

                // Asignar la lista de elementos al ComboBox
                cb_tranMode.Items.Clear(); // Limpiar los elementos existentes 
                cb_tranMode.Items.AddRange(Items_IOMode.ToArray());
            }
        }

        private void timer_detect_port_Tick(object sender, EventArgs e) //se encarga de verificar que la comunicacion serial no se ha interrumpido
        {
            if (puerto.IsOpen == false && puerto_Conectado == true)
            {
                timer_detect_port.Stop();
                MessageBox.Show("Port closed", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cb_puertos.Text = "Closed";
                btn_conectar.Image = global::EBYTE_NAS.Properties.Resources.port_close;
            }

        }
        Fire_base fireBaseForm = new Fire_base();
        public event EventHandler PictureBoxClickEvent;
       
        
        private void timer_catchTable_Tick(object sender, EventArgs e)
        {
            
            if (update_table == true)
            {
                update_table = false;
                try
                {
                    Conexion();
                    if (client != null)
                    {
                        FirebaseResponse response = client.Get("Mac_address");
                        if (response.Body != "null")
                        {
                            fireBaseForm.MacID1 = response.Body;
                           // MessageBox.Show("enviado");
                            
                        }
                    }
                }
                catch (FireSharp.Exceptions.FirebaseException ex)
                {
                    // Manejar el error específico de Firebase
                    MessageBox.Show("Error de Firebase: " + ex.Message);
                }
                
            }
        }
        private bool update_table;
        private bool Table_closed;
        
        private int force_form = 0;

        public void Show_table()
        {
            try
            {

                // fireBaseForm.Close();
                Conexion();
                // Verificar si la conexión fue exitosa
                if (client != null)
                {
                    FirebaseResponse response = client.Get("Mac_address");

                    // Verificar si el nodo existe y tiene datos
                    if (response.Body != "null")
                    {

                        //FireBase.MacID1 = response.Body;

                        // Verificar si el formulario ya está creado y mostrarlo o crear una nueva instancia
                        fireBaseForm.MacID1 = response.Body;

                        if (fireBaseForm != null && !fireBaseForm.IsDisposed)
                        {
                            //fireBaseForm.Show();
                            //MessageBox.Show("1");
                        }
                        if (fireBaseForm.IsDisposed)
                        {
                            fireBaseForm = new Fire_base();
                            fireBaseForm.MacID1 = response.Body;
                            //fireBaseForm.Show();
                            //MessageBox.Show("2");
                            if (!fireBaseForm.Visible)
                            {
                                fireBaseForm.Show(); // Mostrar el formulario si está oculto.
                            }
                        }
                    }
                }
            }
            catch (FireSharp.Exceptions.FirebaseException ex)
            {
                // Manejar el error específico de Firebase
                MessageBox.Show("Error de Firebase: " + ex.Message);

            }
        }
        public void btn_Show_table_Click(object sender, EventArgs table)
        {

            try
            {

                // fireBaseForm.Close();
                Conexion();
                // Verificar si la conexión fue exitosa
                if (client != null)
                {
                    FirebaseResponse response = client.Get("Mac_address");

                    // Verificar si el nodo existe y tiene datos
                    if (response.Body != "null")
                    {

                        //FireBase.MacID1 = response.Body;

                        // Verificar si el formulario ya está creado y mostrarlo o crear una nueva instancia
                        fireBaseForm.MacID1 = response.Body;

                        if (fireBaseForm != null && !fireBaseForm.IsDisposed)
                        {
                            fireBaseForm.Show();
                            //MessageBox.Show("1");
                        }
                        if (fireBaseForm.IsDisposed)
                        {
                            fireBaseForm = new Fire_base();
                            fireBaseForm.MacID1 = response.Body;
                            //fireBaseForm.Show();
                            //MessageBox.Show("2");
                            if (!fireBaseForm.Visible)
                            {
                                fireBaseForm.Show(); // Mostrar el formulario si está oculto.
                            }
                        }

                        else
                        {
                            // Actualizar = false;
                            //fireBaseForm.MacID1 = response.Body;
                            /*
                                Fire_base fireBaseForm = new Fire_base();
                                fireBaseForm.MacID1 = response.Body;
                                fireBaseForm.Show();
                            */
                        }

                    }
                }
            }
            catch (FireSharp.Exceptions.FirebaseException ex)
            {
                // Manejar el error específico de Firebase
                MessageBox.Show("Error de Firebase: " + ex.Message);

            }
        }

        private void timer_eliminar_Ltable_Tick(object sender, EventArgs e)
        {
            timer_eliminar_Ltable.Stop();
            if (Eliminar == true)
            {
                pictureBox5_Click(btn_clear_table, EventArgs.Empty);
            }
        }

        private void cb_puertos_DropDown_1(object sender, EventArgs e)
        {
            cb_puertos.Items.Clear();
            arregloPuertos = SerialPort.GetPortNames(); // Metodo que regtresa un array con los puertos seriales en string
            foreach (string element_nombrePort in arregloPuertos)
            {
                cb_puertos.Items.Add(element_nombrePort);
            }
        }

        private void pictureBox2_Click_2(object sender, EventArgs e)
        {

        }

       

        private void pictureBox2_Click_4(object sender, EventArgs e)
        {
            string new_Version = "1.0.0";
            Conexion();
            // Verificar si la conexión fue exitosa
            if (client != null)
            {
                try
                {
                    FirebaseResponse response = client.Get("Version/New_version");

                    // Verificar si el nodo existe y tiene datos
                    if (response.Body != "null")
                    {
                      //  MessageBox.Show(response.Body);
                        new_Version = response.Body.Replace("\"", "");


                        String My_Version = "1.0.0";
                        MessageBox.Show(My_Version + " " + new_Version);
                        if (My_Version != new_Version)
                        {
                            DialogResult result = MessageBox.Show("Nueva Actualizacion disponible ¿deseas descargar nueva version?", "Elegir Opción", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (result == DialogResult.Yes)
                            {
                                string link = "https://cdn.glitch.me/bca95e37-ff12-44c9-92a8-19fd3a3b892d/V2.0_Setup%20xbee_LoRa.zip?v=1691425217484";

                                try
                                {
                                    Process.Start(new ProcessStartInfo
                                    {
                                        FileName = link,
                                        UseShellExecute = true
                                    });
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error al abrir el enlace: " + ex.Message);
                                }
                            }
                            else if (result == DialogResult.No)
                            {
                                MessageBox.Show("sigue con tu vida");
                            }

                        }
                        else
                        {
                            MessageBox.Show("Cuentas con la version mas actualizada");
                        }
                    }
                    else
                    {
                        MessageBox.Show("No hay datos en el nodo especificado");
                    }
                }
                catch (FireSharp.Exceptions.FirebaseException ex)
                {
                    // Manejar el error específico de Firebase
                    MessageBox.Show("Error de Firebase: " + ex.Message);

                } }
          
            
            
           
        }

        private void timer_checkVersion_Tick(object sender, EventArgs e)
        {
            timer_checkVersion.Stop();
            CheckVersion();
        }

        private void pictureBox2_Click_5(object sender, EventArgs e)
        {
            string filePath = @"C:\Users\Jesus Armando\Downloads\hola"; // Ruta completa del archivo

            try
            {
                File.OpenRead(filePath);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private bool IsHexDigit(char c)
        {
            return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'); //evaluates hexa characters
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

    }
   
}
