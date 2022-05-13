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
    public partial class Form1 : Form
    {
        private MainForm mainForm;
        private List<LibPcapLiveDevice> interfaceList = new List<LibPcapLiveDevice>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            LibPcapLiveDeviceList devices = LibPcapLiveDeviceList.Instance;
            
            foreach (LibPcapLiveDevice device in devices)
            {
                if (!device.Interface.Addresses.Exists(a => a != null && a.Addr != null && a.Addr.ipAddress != null)) continue;
                interfaceList.Add(device);
                comboBox1.Items.Add(device.Interface.FriendlyName);
            }
            if(comboBox1.Items.Count>0)comboBox1.SelectedIndex = 0;
        }

        private void choose_Click(object sender, EventArgs e)
        {
            mainForm = new MainForm(interfaceList[comboBox1.SelectedIndex]);
            mainForm.Show();
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mainForm == null) Application.Exit();
        }
    }
}
