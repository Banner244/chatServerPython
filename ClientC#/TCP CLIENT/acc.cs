using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCP_CLIENT
{
    public partial class acc : Form
    {
        public acc(user me)
        {
            InitializeComponent();
            this.FormClosing += fileTypeDialog_FormClosing;
            label1.Text += " " + me.Name();
            label2.Text += " " + me.Mail();
            label3.Text += " " + me.Admin();
        }

        private void fileTypeDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
/*     int count = 0;
     char[] cdata = data.ToCharArray();
     for(int i = 0; i < data.Length; i++)
     {
         if (cdata[i].ToString() == ",")
         {
             i++;
             count++;
         }
         switch (count)
         {
             case 0: name += cdata[i].ToString(); break;
             case 1: mail += cdata[i].ToString(); break;
             case 2: admin += cdata[i].ToString(); break;
         }
     }
     label1.Text += " " + name;
     label2.Text += " " + mail;
     //MessageBox.Show(data);
     switch (int.Parse(admin))
     {
         case 0: label3.Text += "User"; break;
         case 1: label3.Text += "Support"; break;
         case 2: label3.Text += "Moderator"; break;
         case 3: label3.Text += "Admin"; break;
         case 4: label3.Text += "Projektleiter"; break;
     }*/
