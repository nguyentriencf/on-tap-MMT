﻿using System;
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
using ManagermentCommunication;
namespace client
{
    public partial class Form1 : Form
    {
        IPEndPoint iPEnd;
        Socket client;
        public Form1()
        {
            InitializeComponent();
        }

        public void Connect()
        {
            string hostName = Dns.GetHostName();
            string ip = Dns.GetHostByName(hostName).AddressList[0].ToString();
            iPEnd = new IPEndPoint(IPAddress.Parse(ip), 9999);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                client.Connect(iPEnd);
            }
            catch
            {
                MessageBox.Show("Kết nối không thành công");
                return;
            }
            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();
        }
        public void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    ServerReponse serverReponse = new ServerReponse();
                    serverReponse = (ServerReponse)Deserialize(data);
                    switch (serverReponse.Type)
                    {             
                        case ServerReponType.sendStudentDetail:

                            break;
                        default:
                            break;
                    }
                }
            }
            catch 
            {

                throw;
            }
        }
        public byte[] Serialize(object data)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, data);
            return stream.ToArray();
        }
        public object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }
        public void Send(string data)
        {
            if (data != string.Empty)
            {
                client.Send(Serialize(data));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Connect();
            }
            catch
            {
                MessageBox.Show("Kết nối server thất bại");
            }
            
        }

        private void btnSendSV_Click(object sender, EventArgs e)
        {
            List<string> infoStuden = new List<string>();
            infoStuden.Add(txtMSSV.Text);
            infoStuden.Add(txtHoVaTen.Text);
            infoStuden.Add(txtCountry.Text);
            foreach (var item in infoStuden)
            {
                MessageBox.Show(item);
            }
           
        }
    }
}
