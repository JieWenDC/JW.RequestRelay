using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JW.RequestRelay
{
    public partial class MD : Form
    {
        public MD()
        {
            InitializeComponent();
        }

        private void clientMenu_create_Click(object sender, EventArgs e)
        {
            var form = new Client.Create();
            form.MdiParent = this;
            form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        private void clientMenu_Manage_Click(object sender, EventArgs e)
        {
            var form = new Client.Manage();
            form.MdiParent = this;
            form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        private void toolMenuClient_Click(object sender, EventArgs e)
        {
            var form = new Tools.Client();
            form.MdiParent = this;
            form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        private void toolMenuServer_Click(object sender, EventArgs e)
        {
            var form = new Tools.Server();
            form.MdiParent = this;
            form.WindowState = FormWindowState.Maximized;
            form.Show();

        }
    }
}
