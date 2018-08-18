using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_CLIENT
{
    public class user
    {
        private string name, mail, admin;
        public user(string data)
        {
            int count = 0;
            char[] cdata = data.ToCharArray();
            for (int i = 0; i < data.Length; i++)
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
            //MessageBox.Show(data);
            switch (int.Parse(admin))
            {
                case 0: admin = "User"; break;
                case 1: admin = "Support"; break;
                case 2: admin = "Moderator"; break;
                case 3: admin = "Admin"; break;
                case 4: admin = "Projektleiter"; break;
            }
        }
        public string Name()
        {
            return name;
        }
        public string Mail()
        {
            return mail;
        }
        public string Admin()
        {
            return admin;
        }
            
    }
}