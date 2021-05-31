using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ManagermentCommunication
{
    public class ExchangeData
    {
       
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
    }
}
