using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MinesServer.Network.Tutorial;
using MinesServer.Network.TypicalEvents;

namespace MinesServer.Network
{
    public readonly struct TYPacket : IDataPart<TYPacket>
    {
        const int eventTypeLength = 4;
        const int eventTimeLength = sizeof(int);
        const int eventLocationLength = sizeof(uint);

        public readonly uint eventTime;
        public readonly uint x;
        public readonly uint y;
        public readonly IDataPartBase data;

        public const string packetName = "TY";

        public string PacketName => packetName;

        public string EventType => data.PacketName;

        private static PacketDecoder GetDecoder(string packetName) => packetName switch
        {
            XmovPacket.packetName => x => XmovPacket.Decode(x),
            XdigPacket.packetName => x => XdigPacket.Decode(x),
            XgeoPacket.packetName => x => XgeoPacket.Decode(x),
            XheaPacket.packetName => x => XheaPacket.Decode(x),
            XbldPacket.packetName => x => XbldPacket.Decode(x),
            LoclPacket.packetName => x => LoclPacket.Decode(x),
            WhoiPacket.packetName => x => WhoiPacket.Decode(x),
            TADGPacket.packetName => x => TADGPacket.Decode(x),
            GUI_Packet.packetName => x => GUI_Packet.Decode(x),
            CpriPacket.packetName => x => CpriPacket.Decode(x),
            ChooPacket.packetName => x => ChooPacket.Decode(x),
            CmenPacket.packetName => x => CmenPacket.Decode(x),
            CsetPacket.packetName => x => CsetPacket.Decode(x),
            ChatPacket.packetName => x => ChatPacket.Decode(x),
            XhurPacket.packetName => x => XhurPacket.Decode(x),
            TAGRPacket.packetName => x => TAGRPacket.Decode(x),
            FINVPacket.packetName => x => FINVPacket.Decode(x),
            INVNPacket.packetName => x => INVNPacket.Decode(x),
            INUSPacket.packetName => x => INUSPacket.Decode(x),
            RESPPacket.packetName => x => RESPPacket.Decode(x),
            GDonPacket.packetName => x => GDonPacket.Decode(x),
            DPBXPacket.packetName => x => DPBXPacket.Decode(x),
            HelpPacket.packetName => x => HelpPacket.Decode(x),
            INCLPacket.packetName => x => INCLPacket.Decode(x),
            SettPacket.packetName => x => SettPacket.Decode(x),
            pRSTPacket.packetName => x => pRSTPacket.Decode(x),
            BldsPacket.packetName => x => BldsPacket.Decode(x),
            ClanPacket.packetName => x => ClanPacket.Decode(x),
            MisoPacket.packetName => x => MisoPacket.Decode(x),
            PDELPacket.packetName => x => PDELPacket.Decode(x),
            PCOPPacket.packetName => x => PCOPPacket.Decode(x),
            PRENPacket.packetName => x => PRENPacket.Decode(x),
            THIDPacket.packetName => x => THIDPacket.Decode(x),
            RndmPacket.packetName => x => RndmPacket.Decode(x),
            PopePacket.packetName => x => PopePacket.Decode(x),
            PROGPacket.packetName => x => PROGPacket.Decode(x),
            MissPacket.packetName => x => MissPacket.Decode(x),
            ChinPacket.packetName => x => ChinPacket.Decode(x),
            _ => null
        };

        public static TYPacket Decode(ReadOnlySpan<byte> input)
        {
            var caret = eventTypeLength;
            var eventType = Encoding.UTF8.GetString(input[..caret]);
            var decoder = GetDecoder(eventType);
            if (decoder is null) throw new InvalidPayloadException($"Invalid event type: {eventType}");
            var eventTime = MemoryMarshal.Read<uint>(input[caret..(caret += eventTimeLength)]);
            var x = MemoryMarshal.Read<uint>(input[caret..(caret += eventLocationLength)]);
            var y = MemoryMarshal.Read<uint>(input[caret..(caret += eventLocationLength)]);
            return new(eventTime, x, y, decoder(input[caret..]));
        }

        public TYPacket(uint eventTime, uint x, uint y, IDataPartBase data)
        {
            this.eventTime = eventTime;
            this.x = x;
            this.y = y;
            this.data = data;
        }

        public int Encode(Span<byte> output)
        {
            if (EventType.Length != eventTypeLength) throw new InvalidPayloadException($"Invalid event type length: Excepted {eventTypeLength} but got {EventType.Length}");
            if (GetDecoder(EventType) is null) throw new InvalidPayloadException($"Invalid event type: {EventType}");
            var caret = eventTypeLength;
            var bytesWritten = Encoding.UTF8.GetBytes(EventType, output);
            var et = eventTime;
            MemoryMarshal.Write(output[caret..(caret += eventTimeLength)], ref et);
            bytesWritten += eventTimeLength;
            var tmpx = x;
            MemoryMarshal.Write(output[caret..(caret += eventLocationLength)], ref tmpx);
            bytesWritten += eventLocationLength;
            var tmpy = y;
            MemoryMarshal.Write(output[caret..(caret += eventLocationLength)], ref tmpy);
            bytesWritten += eventLocationLength;
            bytesWritten += data.Encode(output[caret..(caret + data.Length)]);
            return bytesWritten;
        }

        public int Length => eventTypeLength + eventTimeLength + eventLocationLength * 2 + data.Length;
    }
}
