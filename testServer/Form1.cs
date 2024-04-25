using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace testServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HttpServer.start();
        }
    }
}
