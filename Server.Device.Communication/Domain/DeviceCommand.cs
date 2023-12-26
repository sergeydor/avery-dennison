using Common.Domain.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Device.Communication.Domain
{  
    public class DeviceOutgoingCommand : DeviceCommandBase
    {
        public CommandData Data { get; set; }

        public int ComputeHash() // to bind input and output data
        {
            List<byte> bytes = new List<byte>(64);
            bytes.Add(this.Data.CMD);
            bytes.AddRange(base.GetBytes(base.DeviceIdentity.MacAddress));
            return ComputeHash(bytes.ToArray());
        }

        public override string ToString()
        {
            //byte[] data = GetBytes();
            //string hex = BitConverter.ToString(data).Replace("-", string.Empty);

            StringBuilder bld = new StringBuilder();
            bld.Append(BitConverter.ToString(ToArr(Data.SOF)));
            bld.Append('-');
            bld.Append(BitConverter.ToString(ToArr(Data.LEN)));
            bld.Append('-');
            bld.Append(BitConverter.ToString(ToArr(Data.CMD)));

            bld.Append('-');
            if (Data.DATA.Length > 0)
                bld.Append('-');
            bld.Append(BitConverter.ToString(Data.DATA));
            if (Data.DATA.Length > 0)
                bld.Append('-');
            bld.Append('-');

            bld.Append(BitConverter.ToString(ToArr(Data.FCS)));
            return bld.ToString();
        }

        public byte[] GetBytes()
        {
	        var length = 4 + Data.DATA.Length;
			byte[] data = new byte[length];
            data[0] = Data.SOF;
            data[1] = Data.LEN;
	        data[2] = Data.CMD;
	        if (Data.DATA.Length > 0)
	        {
		        Array.Copy(Data.DATA, 0, data, 3, Data.DATA.Length);
	        }
	        data[length - 1] = Data.FCS;
	        return data;
        }

        public static DeviceOutgoingCommand FromBytes(byte[] data)
        {
			var result = new DeviceOutgoingCommand();
			var commandData = new CommandData();

			byte len = data[1];

			commandData.LEN = len;
			commandData.CMD = data[2];
			commandData.DATA = new byte[len];

			for (int i = 0; i < len; i++)
			{
				commandData.DATA[i] = data[i + 3];
			}
			commandData.FCS = data[len + 3];
			result.Data = commandData;

			return result;
        }
    }

    public class DeviceIncommingCommand : DeviceCommandBase
    {
        public ResponseData Data { get; set; }  
              
        public int ComputeHash()
        {
            List<byte> bytes = new List<byte>(64);
            bytes.Add(this.Data.CMD);
            bytes.AddRange(base.GetBytes(base.DeviceIdentity.MacAddress));
            return ComputeHash(bytes.ToArray());
        }

        public override string ToString()
        {
            //byte[] data = GetBytes();
            //string hexstr = BitConverter.ToString(data).Replace("-", string.Empty);

            StringBuilder bld = new StringBuilder();
            bld.Append(BitConverter.ToString(ToArr(Data.SOF)));
            bld.Append('-');
            bld.Append(BitConverter.ToString(ToArr(Data.LEN)));
            bld.Append('-');
            bld.Append(BitConverter.ToString(ToArr(Data.CMD)));
            bld.Append('-');
            bld.Append(BitConverter.ToString(ToArr(Data.STATUS)));
            bld.Append('-');
            bld.Append(BitConverter.ToString(ToArr(Data.TMR_HI)));
            bld.Append('-');
            bld.Append(BitConverter.ToString(ToArr(Data.TMR_LO)));
            bld.Append('-');
            if (Data.DATA.Length > 0)
                bld.Append('-');
            bld.Append(BitConverter.ToString(Data.DATA));
            if (Data.DATA.Length > 0)
                bld.Append('-');
            bld.Append('-');
            bld.Append(BitConverter.ToString(ToArr(Data.FCS)));

            return bld.ToString();
        }
        
        public byte[] GetBytes()
        {
            byte[] data = new byte[6 + Data.DATA.Length];
            data[0] = Data.SOF;
            data[1] = Data.LEN;
            data[2] = Data.CMD;
            data[3] = Data.STATUS;
            data[4] = Data.TMR_HI;
            data[5] = Data.TMR_LO;
	        if (Data.DATA.Length > 0)
	        {
		        Array.Copy(Data.DATA, 0, data, 6, Data.DATA.Length);
		        data[Data.DATA.Length - 1] = Data.LEN;
	        }
	        return data;
        }

        public static DeviceIncommingCommand FromBytes(byte[] data)
        {
			var result = new DeviceIncommingCommand();
			var responseData = new ResponseData();

			byte len = data[1];

			responseData.LEN = len;
			responseData.CMD = data[2];
	        responseData.STATUS = data[3];
	        responseData.TMR_HI = data[4];
	        responseData.TMR_LO = data[5];
			responseData.DATA = new byte[len];

			for (int i = 0; i < len; i++)
			{
				responseData.DATA[i] = data[i + 6];
			}
			responseData.FCS = data[len + 6];
			result.Data = responseData;

			return result;
        }
    }

    public abstract class DeviceCommandBase
    {
        public DeviceIdentity DeviceIdentity { get; set; }

        public DateTime ReceiveDt { get; set; }

        protected byte[] ToArr(byte b)
        {
            return new byte[] { b };
        }

        protected byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        protected int ComputeHash(params byte[] data)
        {
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;

                for (int i = 0; i < data.Length; i++)
                    hash = (hash ^ data[i]) * p;

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }
    }
}
