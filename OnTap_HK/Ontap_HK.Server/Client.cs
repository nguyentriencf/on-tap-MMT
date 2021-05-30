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
        Socket client;
        public Client()
        {
            InitializeComponent();
        }
        public void Connect()
        {
            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            IP = new IPEndPoint(IPAddress.Parse(myIP), 9999);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                client.Connect(IP);
            }
            catch
            {
                MessageBox.Show("Khong the ket noi toi server");
                return;
            }
            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();
        }

        public void Send(string message)
        {
            if (message != String.Empty)
            {
                client.Send(Serialize(message));
            }
        }

        public void Receive()
        {
            try
            {
                while (true)
                {

                    byte[] data = new byte[1024 * 5000];
                    // return number byte rêcived
                    client.Receive(data);
                    ServerReponse serverReponse = new ServerReponse();
                    serverReponse = (ServerReponse)Deserialize(data);


                    switch (serverReponse.Type)
                    {
                        case ServerResponseType.SendFile:
                            byte[] receiveBylength = (byte[])serverReponse.DataResponse;
                            string nameLink = SaveFile(receiveBylength, receiveBylength.Length);
                            SetText(nameLink);
                            break;
                        //case ServerResponseType.BeginExam:
                        //    object timeExam = serverReponse.DataResponse;
                        //    int minute = int.Parse(timeExam.ToString());

                        //    SetTime(timeExam, minute);
                            /* lblThoiGian.Text = timeExam.ToString() + " Phút";
                             int minute = Int32.Parse(timeExam.ToString());
                             counter = minute * 60;
                             countdown.Enabled = true;*/
                        //    break;
                        //case ServerResponseType.SendStudent:
                        //    var userList = (List<Students>)serverReponse.DataResponse;
                        //    SetData(userList);
                        //    break;
                        case ServerResponseType.sendDe:
                            var subject = (string)serverReponse.DataResponse;
                            SetSubject(subject);
                            break;
                      
                        default:
                            break;
                    }
                }
            }
            catch
            {
                //  throw er;
                Close();
            }
        }

        delegate void SetTextCallback(string text);

        private void SetSubject(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.lblMonThi.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetSubject);
                lblMonThi.Invoke(d, new object[] { text });
            }
            else
            {
                this.lblMonThi.Text = text;
            }
        }
        private string SaveFile(byte[] data, int dataLength)
        {
            string pathSave = "D:/";
            int fileNameLength = BitConverter.ToInt32(data, 0);
            string nameFile = Encoding.ASCII.GetString(data, 4, fileNameLength);
            string name = pathSave + Path.GetFileName(nameFile);

            BinaryWriter writer = new BinaryWriter(File.Open(name, FileMode.Append));
            int count = dataLength - 4 - fileNameLength;
            writer.Write(data, 4 + fileNameLength, count);
            return name;
        }
        public byte[] Serialize(object data)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);
            return stream.ToArray();
        }
        public object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);

        }
        private void SetText(string text)
        {
           
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.lblDeThi.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }

            else
            {
                this.lblDeThi.Text = text;
            }
        }

        private void cmdKetNoi_Click(object sender, EventArgs e)
        {
            try
            {
                Connect();
                MessageBox.Show("Kết nối Server thành công");
            }
            catch
            {

                MessageBox.Show("Kết nối Server thất bại");
            }
        }
    
        private void lblDeThi_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.lblDeThi.LinkVisited = true;
            System.Diagnostics.Process.Start(this.lblDeThi.Text);
        }
    }
}
