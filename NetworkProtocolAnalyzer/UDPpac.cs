using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;

namespace NetworkProtocolAnalyzer
{
    public class UDPpac
    {
        private int srcPort;
        private int dstPort;
        private ushort checksum;
        private int key;
        private UdpPacket udpPacket;
        public UDPpac(int key, UdpPacket udpPacket)
        {
            this.key = key;
            this.udpPacket = udpPacket;
            this.srcPort = udpPacket.SourcePort;
            this.dstPort = udpPacket.DestinationPort;
            this.checksum = udpPacket.Checksum;
        }
        public override string ToString()
        {
            return "Packet number: " + key +
                                            " Type: UDP" +
                                            "\r\nSource port:" + srcPort +
                                            "\r\nDestination port: " + dstPort +
                                            "\r\nChecksum:" + checksum.ToString() + " valid: " + udpPacket.ValidChecksum +
                                            "\r\nValid UDP checksum: " + udpPacket.ValidUDPChecksum; ;
        }
    }
}
