using ComponentFactory.Krypton.Toolkit;
using System;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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
            cb_module.Text = "E22";
            tx_addres.PlaceholderText = "Enter HEX format 0 - FF FF";
            tx_channel.PlaceholderText = "Enter HEX format 0 - 50";
            tx_net_id.PlaceholderText = "Enter HEX format 0 - FF FF";
            tx_key.PlaceholderText = "Enter HEX format 0 - FF FF";
            btn_conectar.Image = global::EBYTE_NAS.Properties.Resources.port_close;
            puerto.DataReceived += Puerto_DataReceived;



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
        int m, mx, my;

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
            if (cb_module.SelectedItem == "E32")
            {
                terminalLb.Text = "";
                if (puerto.IsOpen)
                {
                    timer_module.Start();
                    byte[] get = { 0x41, 0x54, 0x2B, 0x44, 0x45, 0x56, 0x54, 0x59, 0x50, 0x45, 0x3D, 0x3F };
                    puerto.Write(get,0,get.Length);
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
                        byte[] hexMessage2 = { 0xC0, 0x00, 0x09, resultado_01, resultado_01_1, resultado_02, resultado_03, resultado_04H, resultado_05H, resultado_06H, 0x00, 0x00 }; //-C1-00-09-00-F8-0A-60-C0-1E-80-00-00
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

            else
            {
                MessageBox.Show("Port closed", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            // timer1.Start();
        }

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

        }
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (cb_module.SelectedItem == "E22")
            {
                String mensaje = terminalLb.Text;
                if (mensaje.Contains("C1") && mensaje.Length > 10)
                {
                    String[] partes = mensaje.Split('-');
                    //tx_NI.Text = partes[1] + partes[2] + partes[3] + partes[4] + partes[5] + partes[6] + partes[7] + partes[8] + partes[9] + partes[10] + partes[11] + partes[12];
                    timer1.Stop();

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
                    int channel = Convert.ToUInt16(partes[9], 16);
                    tx_channel.Text = Convert.ToString(channel);
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

                    timer1.Stop();

                }
            } 
                    
            
        timer1.Stop();
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

                maclb.Text = Frame[0];
                lb_frame_1.Text = Frame[1];

                puerto.Write(mensaje_byte_tamaño, 0, mensaje_byte_tamaño.Length);
                MessageBox.Show(BitConverter.ToString(mensaje_byte_tamaño));
            }
            else
            {
                MessageBox.Show("Port closed","Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        string checj_addres_set;
        private void timer_set_Tick(object sender, EventArgs e)
        {
            if (cb_module.SelectedItem == "E22")
            {
                String mensaje = terminalLb.Text;
                if (mensaje.Contains("C1") && mensaje.Length > 10)
                {
                    String[] partes = mensaje.Split('-');
                    //tx_NI.Text = partes[1] + partes[2] + partes[3] + partes[4] + partes[5] + partes[6] + partes[7] + partes[8] + partes[9] + partes[10] + partes[11] + partes[12];
                    timer1.Stop();

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
                    int channel = Convert.ToUInt16(partes[9], 16);
                    tx_channel.Text = Convert.ToString(channel);
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

        private void timer2_get_Tick(object sender, EventArgs e)
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
                
                timer1.Start();
        }

        private void timer_module_Tick(object sender, EventArgs e)
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

            }

        private void CB_PARITY_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click_2(object sender, EventArgs e)
        {

        }

        private void cb_module_SelectedIndexChanged(object sender, EventArgs e)
        {
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





            }
        }

        private void pictureBox2_Click_3(object sender, EventArgs e)
        {
            
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

        private bool IsHexDigit(char c)
        {
            return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
        }


    }
}
