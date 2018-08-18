using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;

namespace TCP_CLIENT
{
    public partial class loginForm : Form
    {
        public loginForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            login();
        }
        void login()
        {
            try
            {
                string IP = "127.0.0.1"; 
                Int32 port = 8217;
                TcpClient client = new TcpClient(IP, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                string msg = "ServerKeyN: " + textBox1.Text + "P: " + textBox2.Text;
                Byte[] data = System.Text.Encoding.UTF8.GetBytes(msg);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: " + msg);
                
                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                MessageBox.Show("123");
                responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
                if (responseData == "success")
                {
                    Form1 main = new Form1(client, this);
                    main.Show();
                    this.Hide();
                }
                else if (responseData == " ")
                {
                    MessageBox.Show("Dieser Account existiert nicht !");
                    CloeAll(client, stream);
                }
                else if (responseData == "b")
                {
                    MessageBox.Show("Dein Account ist gesperrt !");
                    CloeAll(client, stream);
                }
                else if (responseData == "d")
                {
                    MessageBox.Show("Du bist bereits eingeloggt !");
                    CloeAll(client, stream);
                }
                else
                {
                    CloeAll(client, stream);
                }
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("ArgumentNullException: ");
            }
            catch (SocketException)
            {
                MessageBox.Show("SocketException: Es konnte keine Verbindung hergestellt werden...");
            }
        }
        void CloeAll(TcpClient client, NetworkStream stream)
        {
            stream.Close();
            client.Close();
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://registerpage");//Page to register
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                login();
        }
    }
}
