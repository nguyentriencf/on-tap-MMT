using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagerCommunication;
namespace Ontap_HK.Server
{
    public partial class Client : Form
    {
        IPEndPoint IP;
        Socket server;
        List<Socket> clientList;
        List<string> clientIP;
        ServerReponse serverReponse = new ServerReponse();
        public Client()
        {
            InitializeComponent();
        }
   
    }
}
