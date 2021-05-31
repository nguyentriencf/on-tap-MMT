using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagermentCommunication;
namespace Server
{
    public partial class Server : Form
    {
        ExchangeData exchange = new ExchangeData();
        List<Student> listStudent = new List<Student>();
        ServerReponse serverReponse = new ServerReponse();
        IPEndPoint IPEnd;
        Socket server;
        List<Socket> listClient;
        List<Student> students;
        public Server()
        {
            InitializeComponent();

            Connect();
        }
    
        private void Server_Load(object sender, EventArgs e)
        {
        
        }
        public void Connect()
        {
            listClient = new List<Socket>();
            IPEnd = new IPEndPoint(IPAddress.Any,9999);
            server = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            server.Bind(IPEnd);

            Thread listen = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        server.Listen(100);
                        Socket client = server.Accept();
                        listClient.Add(client);
                        Thread receive = new Thread(Receive);
                        receive.IsBackground = true;
                        receive.Start(client);
                    }
                }
                catch
                {
                    IPEnd = new IPEndPoint(IPAddress.Any, 9999);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }

            });
            listen.IsBackground = true;
            listen.Start();

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
                    serverReponse = (ServerReponse)exchange.Deserialize(data);
                    switch (serverReponse.Type)
                    {                  
                        case ServerReponType.sendInfoStudent:
                            //Student student = new Student();
                            //student = (Student)serverReponse.DataReponse;
                            //listStudent.Add(student);
                            //this.dgvDSSV.DataSource = listStudent;

                            break;
                      
                        default:
                            break;
                    }
                }
            }
            catch
            {
                        

            }
        }
        public void close()
        {
            server.Close();
        }
    }
}
