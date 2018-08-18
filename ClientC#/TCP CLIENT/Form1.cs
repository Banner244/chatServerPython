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
using System.Threading;

namespace TCP_CLIENT
{
    public partial class Form1 : Form
    {
        loginForm gg;
        TcpClient cl;
        NetworkStream stream;
        private user me;

        public Form1(TcpClient client, loginForm l)
        {
            gg = l;
            InitializeComponent();
            this.FormClosing += fileTypeDialog_FormClosing;


            cl = client;
            stream = cl.GetStream();
            Thread ctThread = new Thread(getMessage);
            ctThread.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sendMessage();
        }
        void sendMessage()
        {
            if (textBox1.Text != "")
            {
                string msg = (textBox1.Text);
                int action = 0;
                string package = msg.Length + "," + action + ":" + msg;
                Byte[] data = System.Text.Encoding.UTF8.GetBytes(package);
                stream.Write(data, 0, data.Length);
                textBox1.Text = "";
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            acc main = new acc(me);
            main.Show();
        }
        public void AppendTextBox2(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox2), new object[] { value });
                return;
            }
        }
        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }

               string length = "";
               string action = "";
               string msg="";
               int counter = 0;
               char[] cmsg = value.ToCharArray();
               bool change = true;

               for(int i = 0; i<value.Length; i++)
               {
                   if (change)
                   {
                       if (cmsg[i].ToString() == "," )
                       {
                           counter++;
                           i++;
                       }
                       if (cmsg[i].ToString() == ":")
                       {
                           counter++;
                           i++;
                           change = false;
                       }
                   }

                switch (counter)
                   {
                       case 0: length += cmsg[i].ToString(); break;
                       case 1: action += cmsg[i].ToString(); break;
                       case 2: msg += cmsg[i].ToString(); break;
                   }
               }

               switch (int.Parse(action))
               {
                   case 0: richTextBox1.Text += "\n"+msg; break;
                   case 1: setData(msg); break;
                   case 2: onlineList(msg); break;
                   case 3: kicked(msg); break;
               }
            this.Text = "User: " + me.Name() + " | Online: " + listBox1.Items.Count;
        }
        List<string> MyList = new List<string>();

        void kicked(string message)
        {
            MessageBox.Show("Du wurdest von: "+ message + " gekickt !");
            Environment.Exit(1);
        }
        void onlineList(string users)
        {
            // listBox1.Items.Clear();
            MyList.Clear();
            int count = 0;
            char[] cdata = users.ToCharArray();
            string user="";


            for (int i = 0; i < users.Length; i++)
            {
                if (cdata[i].ToString() == ",")
                {
                    i++;
                    count++;
                    MyList.Add(user);
                    //listBox1.Items.Add(user);
                    user = "";
                }
                user += cdata[i].ToString();
            }
            //listBox1.Items.Add(user);
            MyList.Add(user);
            refreshOnlinelList();

        }
        void refreshOnlinelList()
        {
            if(MyList.Count != listBox1.Items.Count)
            {
                listBox1.Items.Clear();
                listBox1.Items.AddRange(MyList.ToArray());
            }
        }
        void setData(string dataString)
        {
            //data = dataString;
            me = new user(dataString);
            //label1.Text += " " + me.Name();
            
            //MessageBox.Show(dataString);
        }
        void getMessage()
        {
            while (true)
            {
                
                int l = 1;
                while (true)
                {
                    Byte[] data = new Byte[l];

                    String responseData = String.Empty;

                    stream.Read(data, 0, data.Length);
                    responseData = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
                    //MessageBox.Show(responseData);
                    string t2 = "";
                    bool change = false;
                    while (true)
                    {
                        foreach (char test in responseData)
                        {
                            if (test.ToString() == ":")
                            {
                                //MessageBox.Show(responseData);
                                change = true;
                                break;
                            }
                        }
                        if (change) break;

                        data = new Byte[l];
                        stream.Read(data, 0, data.Length);
                        responseData = responseData + System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
                    }
                    char[] cmsg = responseData.ToCharArray();
                    for (int i = 0; i < responseData.Length; i++)
                    {
                        if (cmsg[i].ToString() != ",")
                            t2 += cmsg[i].ToString();
                        else
                            break;
                        
                    }

                    //MessageBox.Show(t2);
                    data = new Byte[int.Parse(t2)];
                    stream.Read(data, 0, data.Length);
                    responseData = responseData + System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
                    AppendTextBox(responseData);
                    break;
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            // set the current caret position to the end
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            // scroll it automatically
            richTextBox1.ScrollToCaret();
        }

        bool checkString(string check, string fullString)
        {
            char[] charsToCheck = check.ToCharArray();
            char[] charsFrfullSt = fullString.ToCharArray();
            int count = 0;
            for (int i = 0; i < charsFrfullSt.Length; i++)
            {

                if (charsFrfullSt[i].ToString() == charsToCheck[count].ToString()){
                    count++;
                    if (count == charsToCheck.Length) return true;
                }else{
                    count = 0;
                }
            }
            return false;
        }

        private void fileTypeDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(1);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                sendMessage();
        }

        private void kickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string kickName  = listBox1.GetItemText(listBox1.SelectedItem);
            
            string msg = (kickName);
            int action = 1;
            string package = msg.Length + "," + action + ":" + msg;
            Byte[] data = System.Text.Encoding.UTF8.GetBytes(package);
            stream.Write(data, 0, data.Length);
            textBox1.Text = "";
            MessageBox.Show("du hast "+ kickName + " gekickt !");
        }

        private void banToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = listBox1.GetItemText(listBox1.SelectedItem);

            string msg = (text);
            int action = 2;
            string package = msg.Length + "," + action + ":" + msg;
            Byte[] data = System.Text.Encoding.UTF8.GetBytes(package);
            stream.Write(data, 0, data.Length);
            textBox1.Text = "";
            MessageBox.Show("du hast " + text + " bebannt !");
        }
    }
}
