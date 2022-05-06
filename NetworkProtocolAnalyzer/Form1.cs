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
        List<LibPcapLiveDevice> interfaceList = new List<LibPcapLiveDevice>();
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
        }

        private void choose_Click(object sender, EventArgs e)
        {
           
        }

    }
}
