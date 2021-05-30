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
namespace Client
{
    public partial class Server : Form
    {
        IPEndPoint IP;
        Socket server;
        List<Socket> clientList;
        List<string> clientIP;
        ServerReponse serverReponse = new ServerReponse();
        public Server()
        {
            InitializeComponent();
        }


        private OpenFileDialog openFileDialog1;
        string PathNameSubjectExam;
        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    PathNameSubjectExam = openFileDialog1.FileName;
                    string fileName = Path.GetFileName(PathNameSubjectExam);
                    lstDeThi.Items.Add(fileName);
                    //_server.Send(PathName);
                }
                catch
                {
                    MessageBox.Show("Loi mo file");
                }
            }
        }
        public void Connect()
        {
            clientList = new List<Socket>();
            IP = new IPEndPoint(IPAddress.Any, 9999);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(IP);

            Thread Listen = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        server.Listen(100);
                        Socket client = server.Accept();
                        clientList.Add(client);
                        Thread receive = new Thread(Receive);
                        receive.IsBackground = true;
                        receive.Start(client);
                    }
                }
                catch
                {
                    IP = new IPEndPoint(IPAddress.Any, 9999);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                }

            });
            Listen.IsBackground = true;
            Listen.Start();
        }
        public void Receive(object socket)
        {
            var client = socket as Socket;
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    client.Receive(data);
                    ServerReponse serverReponse = new ServerReponse();
                    serverReponse = (ServerReponse)Deserialize(data);
                    switch (serverReponse.Type)
                    {
                        case ServerResponseType.SendFile:
                            byte[] receiveBylength = (byte[])serverReponse.DataResponse;
                            SaveFile(receiveBylength, receiveBylength.Length);
                            break;
                        //case ServerResponseType.SendAcceptUser:
                        //    string ipUser = client.RemoteEndPoint.ToString().Split(':')[0];
                        //    var mssv = (string)serverReponse.DataResponse;
                        //    Updates(mssv);
                        //    break;
                        default:
                            break;
                    }
                }

            }
            catch (Exception er)
            {
                //throw er;

                Close();
            }
        }
        public void Send(string filePath)
        {

            foreach (Socket client in clientList)
            {
                if (filePath != String.Empty)
                {
                    serverReponse.Type = ServerResponseType.SendFile;
                    serverReponse.DataResponse = GetFilePath(filePath);
                    client.Send(Serialize(serverReponse));
                    //   client.Send(GetFilePath(filePath));

                }
            }

        }
        public void SaveFile(byte[] data, int dataLength)
        {
            string pathSave = "D:/receive/";
            int fileNameLength = BitConverter.ToInt32(data, 0);
            string nameFile = Encoding.ASCII.GetString(data, 4, fileNameLength);
            string nameFolder = Path.GetFileName(nameFile);
            string root = pathSave + nameFolder;
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
            foreach (string Files in Directory.EnumerateFiles(nameFile))
            {
                string name = root + "/" + Path.GetFileName(Files);
                BinaryWriter writer = new BinaryWriter(File.Open(name, FileMode.Append));
                int count = dataLength - 4 - fileNameLength;
                writer.Write(data, 4 + fileNameLength, count);
            }

        }
        public byte[] GetFilePath(string filePath)
        {
            //  var name = Path.GetFileName(filePath);
            byte[] fNameByte = Encoding.ASCII.GetBytes(filePath);
            byte[] fileData = File.ReadAllBytes(filePath);
            byte[] serverData = new byte[4 + fNameByte.Length + fileData.Length];
            byte[] fNameLength = BitConverter.GetBytes(fNameByte.Length);
            fNameLength.CopyTo(serverData, 0);
            fNameByte.CopyTo(serverData, 4);
            fileData.CopyTo(serverData, 4 + fNameByte.Length);
            return serverData;
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
        public delegate void UpdateHandler(object sender, UpdateEventArgs args);
        public event UpdateHandler EventUpdateHandler;
        public class UpdateEventArgs : EventArgs
        {
            public string mssv { get; set; }

        }
        public void Updates(string MSSV)
        {
            UpdateEventArgs args = new UpdateEventArgs();

            args.mssv = MSSV;
            EventUpdateHandler.Invoke(this, args);

        }

        public void sendDe(string namede)
        {
            foreach (Socket client in clientList)
            {
                serverReponse.Type = ServerResponseType.sendDe;
                serverReponse.DataResponse = namede;
                client.Send(Serialize(serverReponse));
            }
        }

        private void cmdBatDauLamBai_Click(object sender, EventArgs e)
        {
           sendDe(cbChonMonThi.Text);
            string pathSubjectExam = PathNameSubjectExam;
           Send(pathSubjectExam);
        }

        private void Server_Load(object sender, EventArgs e)
        {
            Connect();
        }

    }
}
