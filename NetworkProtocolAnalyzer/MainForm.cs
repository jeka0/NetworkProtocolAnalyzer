using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpPcap.LibPcap;

namespace NetworkProtocolAnalyzer
{
    public partial class MainForm : Form
    {
        private LibPcapLiveDevice interf;
        public MainForm(LibPcapLiveDevice interf)
        {
            this.interf = interf;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
