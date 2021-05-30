using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagermentCommunication
{
    public enum ServerReponType
    {
        SearchStudent,
        sendStudentDetail
    }
    [Serializable]
    public class ServerReponse
    {
        public ServerReponType Type { get; set; }
        public object DataReponse { get; set; }
    }
}
