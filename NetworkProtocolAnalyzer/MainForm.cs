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
using SharpPcap;
using PacketDotNet;
using System.Threading;
using System.Diagnostics;

namespace NetworkProtocolAnalyzer
{
    public partial class MainForm : Form
    {
        private LibPcapLiveDevice interf;
        private Thread sniffing;
        private CaptureFileWriterDevice captureFileWriter;
        private List<Packet> packets = new List<Packet>();
        private Stopwatch stopwatch = new Stopwatch();
        private int sumLength = 0, count = 0, count2=0;
        string time_str = "";
        private bool run = false;
        public MainForm(LibPcapLiveDevice interf)
        {
            this.interf = interf;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!run)
            {
                run = true;
                System.IO.File.Delete(Environment.CurrentDirectory + "capture.pcap");
                interf.OnPacketArrival += new PacketArrivalEventHandler(Device_OnPacketArrival);
                sniffing = new Thread(new ThreadStart(sniffing_Proccess));
                stopwatch.Start();
                sniffing.Start();
            }
        }
        private void sniffing_Proccess()
        {
            // Open the device for capturing
            int readTimeoutMilliseconds = 1000;
            interf.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            // Start the capturing process
            if (interf.Opened)
            {
                captureFileWriter = new CaptureFileWriterDevice(interf, Environment.CurrentDirectory + "capture.pcap");
                interf.Capture();
            }
        }
        public void Device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            // dump to a file
            captureFileWriter.Write(e.Packet);

            // start extracting properties for the listview 
            DateTime time = e.Packet.Timeval.Date;
            time_str = (time.Hour + 3) + ":" + time.Minute + ":" + time.Second + ":" + time.Millisecond;
            var packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            
            // add to the list
            packets.Add(packet);
            var ipPacket = (IpPacket)packet.Extract(typeof(IpPacket));
            if (ipPacket != null)
            {
                ListViewItem item = new ListViewItem((packets.Count-1).ToString());
                item.SubItems.Add(time_str);
                String DestinationAddress= ipPacket.DestinationAddress.ToString();
                String protocol = ipPacket.Protocol.ToString();
                item.SubItems.Add(ipPacket.SourceAddress.ToString());
                item.SubItems.Add(DestinationAddress.ToString());
                item.SubItems.Add(protocol);
                int length = e.Packet.Data.Length;
                item.SubItems.Add(length.ToString());
                sumLength += length;
                String[] strs = DestinationAddress.Split('.');
                if (strs[strs.Length - 1] == "0" || strs[strs.Length - 1] == "255") count++;
                if (protocol != "TCP" && protocol != "UDP" && protocol != "IP" && protocol != "ICMP" && protocol != "ICMPV6") count2++;
                Action action = () => listView1.Items.Add(item), action2 = () => AveragePacketLength.Text = (sumLength / packets.Count).ToString(),
                    action3 = () => { double min = stopwatch.ElapsedMilliseconds / 60000.0 + 1; NumberOfPacketsPerMinute.Text = (packets.Count / min).ToString("0.00"); }
                ,action4 = () => NumberOfBroadcastRequests.Text = count.ToString(), action5 = () => NumberOfNonTCPIPPackets.Text = count2.ToString();
                listView1.Invoke(action);
                AveragePacketLength.Invoke(action2);
                NumberOfPacketsPerMinute.Invoke(action3);
                NumberOfBroadcastRequests.Invoke(action4);
                NumberOfNonTCPIPPackets.Invoke(action5);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            run = false;
            sniffing.Abort();
            interf.StopCapture();
            interf.Close();
            stopwatch.Stop();
            captureFileWriter.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            button2_Click(sender, e);
            Application.Exit();
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            string protocol = e.Item.SubItems[4].Text;
            int key = Int32.Parse(e.Item.SubItems[0].Text);
            Packet packet = packets[key];
            switch (protocol)
            {
                case "TCP":
                    var tcpPacket = (TcpPacket)packet.Extract(typeof(TcpPacket));
                    if (tcpPacket != null)
                    {
                        TCPpac tcpPac = new TCPpac(key, tcpPacket);
                        textBox2.Text = "";
                        textBox2.Text = tcpPac.ToString();
                    }
                    break;
                case "UDP":
                    var udpPacket = (UdpPacket)packet.Extract(typeof(UdpPacket));
                    if (udpPacket != null)
                    {
                        UDPpac udpPac = new UDPpac(key, udpPacket);

                        textBox2.Text = "";
                        textBox2.Text = udpPac.ToString();
                    }
                    break;
                case "ARP":
                        var arpPacket = (ARPPacket)packet.Extract(typeof(ARPPacket));
                        if (arpPacket != null)
                        {
                            System.Net.IPAddress senderAddress = arpPacket.SenderProtocolAddress;
                            System.Net.IPAddress targerAddress = arpPacket.TargetProtocolAddress;
                            System.Net.NetworkInformation.PhysicalAddress senderHardwareAddress = arpPacket.SenderHardwareAddress;
                            System.Net.NetworkInformation.PhysicalAddress targerHardwareAddress = arpPacket.TargetHardwareAddress;

                            textBox2.Text = "";
                            textBox2.Text = "Packet number: " + key +
                                            " Type: ARP" +
                                            "\r\nHardware address length:" + arpPacket.HardwareAddressLength +
                                            "\r\nProtocol address length: " + arpPacket.ProtocolAddressLength +
                                            "\r\nOperation: " + arpPacket.Operation.ToString() + // ARP request or ARP reply ARP_OP_REQ_CODE, ARP_OP_REP_CODE
                                            "\r\nSender protocol address: " + senderAddress +
                                            "\r\nTarget protocol address: " + targerAddress +
                                            "\r\nSender hardware address: " + senderHardwareAddress +
                                            "\r\nTarget hardware address: " + targerHardwareAddress;
                        }
                    break;
                case "ICMP":
                        var icmpPacket = (ICMPv4Packet)packet.Extract(typeof(ICMPv4Packet));
                        if (icmpPacket != null)
                        {
                            textBox2.Text = "";
                            textBox2.Text = "Packet number: " + key +
                                            " Type: ICMP v4" +
                                            "\r\nType Code: 0x" + icmpPacket.TypeCode.ToString("x") +
                                            "\r\nChecksum: " + icmpPacket.Checksum.ToString("x") +
                                            "\r\nID: 0x" + icmpPacket.ID.ToString("x") +
                                            "\r\nSequence number: " + icmpPacket.Sequence.ToString("x");
                        }
                    break;
                case "IGMP":
                        var igmpPacket = (IGMPv2Packet)packet.Extract(typeof(IGMPv2Packet));
                        if (igmpPacket != null)
                        {
                            textBox2.Text = "";
                            textBox2.Text = "Packet number: " + key +
                                            " Type: IGMP v2" +
                                            "\r\nType: " + igmpPacket.Type +
                                            "\r\nGroup address: " + igmpPacket.GroupAddress +
                                            "\r\nMax response time" + igmpPacket.MaxResponseTime;
                        }
                    
                    break;
                default:
                    textBox2.Text = "";
                    break;
            }
        }
    }
    
}
