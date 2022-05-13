using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;

namespace NetworkProtocolAnalyzer
{
    public class TCPpac
    {
        private int srcPort;
        private int dstPort;
        private ushort checksum;
        private int key;
        private TcpPacket tcpPacket;
        public TCPpac(int key, TcpPacket tcpPacket)
        {
            this.tcpPacket = tcpPacket;
            this.key = key;
            this.srcPort = tcpPacket.SourcePort;
            this.dstPort = tcpPacket.DestinationPort;
            this.checksum = tcpPacket.Checksum;
        }
        public override string ToString()
        {
            return "Packet number: " + key +
                                            " Type: TCP" +
                                            "\r\nSource port:" + srcPort +
                                            "\r\nDestination port: " + dstPort +
                                            "\r\nTCP header size: " + tcpPacket.DataOffset +
                                            "\r\nWindow size: " + tcpPacket.WindowSize +
                                            "\r\nChecksum:" + checksum.ToString() + (tcpPacket.ValidChecksum ? ",valid" : ",invalid") +
                                            "\r\nTCP checksum: " + (tcpPacket.ValidTCPChecksum ? ",valid" : ",invalid") +
                                            "\r\nSequence number: " + tcpPacket.SequenceNumber.ToString() +
                                            "\r\nAcknowledgment number: " + tcpPacket.AcknowledgmentNumber + (tcpPacket.Ack ? ",valid" : ",invalid") +
                                            "\r\nUrgent pointer: " + (tcpPacket.Urg ? "valid" : "invalid") +
                                            "\r\nACK flag: " + (tcpPacket.Ack ? "1" : "0") +
                                            "\r\nPSH flag: " + (tcpPacket.Psh ? "1" : "0") + 
                                            "\r\nRST flag: " + (tcpPacket.Rst ? "1" : "0") +
                                            "\r\nSYN flag: " + (tcpPacket.Syn ? "1" : "0") +
                                            "\r\nFIN flag: " + (tcpPacket.Fin ? "1" : "0") +
                                            "\r\nECN flag: " + (tcpPacket.ECN ? "1" : "0") +
                                            "\r\nCWR flag: " + (tcpPacket.CWR ? "1" : "0") +
                                            "\r\nNS flag: " + (tcpPacket.NS ? "1" : "0"); ;
        }
    }
}
