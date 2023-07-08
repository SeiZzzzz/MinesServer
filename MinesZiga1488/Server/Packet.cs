using System.Text;

namespace MinesServer.Server
{
    public struct Packet
    {
        public const int dataTypeLength = 1;
        public const int eventTypeLength = 2;
        public const int lengthLength = 4;

        public readonly string dataType;
        public readonly string eventType;
        public readonly byte[] data;
        public Packet(byte[] input)
        {
            byte[] dt = new byte[dataTypeLength], et = new byte[eventTypeLength], l = new byte[lengthLength];
            Buffer.BlockCopy(input, 0, l, 0, l.Length);
            Buffer.BlockCopy(input, l.Length, dt, 0, dt.Length);
            Buffer.BlockCopy(input, l.Length + dt.Length, et, 0, et.Length);
            var headerLength = l.Length + dt.Length + et.Length;
            var packetLength = BitConverter.ToUInt32(l);
            packetLength = packetLength > input.Length ? (uint)(input.Length - headerLength) : packetLength >= headerLength ? (uint)(packetLength - headerLength) : packetLength;
            data = new byte[packetLength];
            Buffer.BlockCopy(input, headerLength, data, 0, data.Length);
            dataType = Encoding.UTF8.GetString(dt);
            eventType = Encoding.UTF8.GetString(et);
        }
        public Packet(string dataType, string eventType, byte[] data)
        {
            this.dataType = dataType;
            this.eventType = eventType;
            this.data = data;
        }

        public Packet(string dataType, string eventType, string data) : this(dataType, eventType, Encoding.UTF8.GetBytes(data)) { }
        public byte[] Compile
        {
            get
            {
                var ret = new byte[Length];
                var len = BitConverter.GetBytes(Length);
                Buffer.BlockCopy(len, 0, ret, 0, len.Length);
                Buffer.BlockCopy(Encoding.UTF8.GetBytes(dataType), 0, ret, len.Length, dataType.Length);
                Buffer.BlockCopy(Encoding.UTF8.GetBytes(eventType), 0, ret, len.Length + dataType.Length, eventType.Length);
                Buffer.BlockCopy(data, 0, ret, len.Length + dataType.Length + eventType.Length, data.Length);
                return ret;
            }
        }
        public uint Length => (uint)(4U + dataType.Length + eventType.Length + data.Length);
    }
}
