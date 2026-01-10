using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Runord.Client.Shared.ValueObjects
{
    public class NodeAddress
    {
        public string Ip { get; set; }
        public int Port { get; set; }

        public NodeAddress(string ip, int port)
        {
            if (!IPAddress.TryParse(ip, out _))
                throw new ArgumentException("Invalid IP address", nameof(ip));

            if (port < 0 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port), "Port must be between 0 and 65535");

            Ip = ip;
            Port = port;
        }

        public override string ToString() => $"{Ip}:{Port}";
    }
}
