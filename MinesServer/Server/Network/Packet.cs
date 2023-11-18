using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MinesServer.Network.Auth;
using MinesServer.Network.BotInfo;
using MinesServer.Network.Chat;
using MinesServer.Network.ConnectionStatus;
using MinesServer.Network.GUI;
using MinesServer.Network.Movement;
using MinesServer.Network.Programmator;
using MinesServer.Network.Tutorial;
using MinesServer.Network.TypicalEvents;
using MinesServer.Network.World;

namespace MinesServer.Network
{
    public delegate IDataPartBase PacketDecoder(ReadOnlySpan<byte> decodeFrom);
    public readonly struct Packet : IDataPart<Packet>
    {
        const int dataTypeLength = sizeof(byte);
        const int eventTypeLength = sizeof(byte) * 2;
        const int lengthLength = sizeof(int);

        public readonly string dataType;
        public readonly IDataPartBase data;

        public string PacketName => throw new NotImplementedException();

        public string EventType => data.PacketName;

        private static PacketDecoder GetDecoder(string packetName) => packetName switch
        {
            TYPacket.packetName => x => TYPacket.Decode(x),
            AUPacket.packetName => x => AUPacket.Decode(x),
            AEPacket.packetName => x => AEPacket.Decode(x),
            AHPacket.packetName => x => AHPacket.Decode(x),
            BotInfoPacket.packetName => x => BotInfoPacket.Decode(x),
            TPPacket.packetName => x => TPPacket.Decode(x),
            SmoothTPPacket.packetName => x => SmoothTPPacket.Decode(x),
            BasketPacket.packetName => x => BasketPacket.Decode(x),
            GuPacket.packetName => x => GuPacket.Decode(x),
            LivePacket.packetName => x => LivePacket.Decode(x),
            SpeedPacket.packetName => x => SpeedPacket.Decode(x),
            NickListPacket.packetName => x => NickListPacket.Decode(x),
            OnlinePacket.packetName => x => OnlinePacket.Decode(x),
            LevelPacket.packetName => x => LevelPacket.Decode(x),
            MoneyPacket.packetName => x => MoneyPacket.Decode(x),
            OKPacket.packetName => x => OKPacket.Decode(x),
            AutoDiggPacket.packetName => x => AutoDiggPacket.Decode(x),
            GeoPacket.packetName => x => GeoPacket.Decode(x),
            SettingsPacket.packetName => x => SettingsPacket.Decode(x),
            WorldInfoPacket.packetName => x => WorldInfoPacket.Decode(x),
            PongPacket.packetName => x => PongPacket.Decode(x),
            SkillsPacket.packetName => x => SkillsPacket.Decode(x),
            OpenURLPacket.packetName => x => OpenURLPacket.Decode(x),
            ClanShowPacket.packetName => x => ClanShowPacket.Decode(x),
            ClanHidePacket.packetName => x => ClanHidePacket.Decode(x),
            AgressionPacket.packetName => x => AgressionPacket.Decode(x),
            BanHammerPacket.packetName => x => BanHammerPacket.Decode(x),
            MissionProgressPacket.packetName => x => MissionProgressPacket.Decode(x),
            BibikaPacket.packetName => x => BibikaPacket.Decode(x),
            RespPacket.packetName => x => RespPacket.Decode(x),
            NaviArrowPacket.packetName => x => NaviArrowPacket.Decode(x),
            DailyRewardPacket.packetName => x => DailyRewardPacket.Decode(x),
            ChatNotificationPacket.packetName => x => ChatNotificationPacket.Decode(x),
            ChatColorPacket.packetName => x => ChatColorPacket.Decode(x),
            StatusPacket.packetName => x => StatusPacket.Decode(x),
            ReconnectPacket.packetName => x => ReconnectPacket.Decode(x),
            PurchasePacket.packetName => x => PurchasePacket.Decode(x),
            StatePanelPacket.packetName => x => StatePanelPacket.Decode(x),
            BadCellsPacket.packetName => x => BadCellsPacket.Decode(x),
            ProgrammatorPacket.packetName => x => ProgrammatorPacket.Decode(x),
            ChatMessagesPacket.packetName => x => ChatMessagesPacket.Decode(x),
            MissionPanelPacket.packetName => x => MissionPanelPacket.Decode(x),
            CurrentChatPacket.packetName => x => CurrentChatPacket.Decode(x),
            ModulesPacket.packetName => x => ModulesPacket.Decode(x),
            ChatListPacket.packetName => x => ChatListPacket.Decode(x),
            OpenProgrammatorPacket.packetName => x => OpenProgrammatorPacket.Decode(x),
            GUIPacket.packetName => x => GUIPacket.Decode(x),
            PingPacket.packetName => x => PingPacket.Decode(x),
            HBPacket.packetName => x => HBPacket.Decode(x),
            InventoryPacket.packetName => x => InventoryPacket.Decode(x),
            _ => null
        };
        
        public Packet(string dataType, IDataPartBase data)
        {
            this.dataType = dataType;
            this.data = data;
        }
        
        public int Encode(Span<byte> output)
        {
            if (EventType.Length != eventTypeLength) throw new InvalidPayloadException($"Invalid event type length: Expected {eventTypeLength} but got {EventType.Length}");
            if (GetDecoder(EventType) is null) throw new InvalidPayloadException($"Invalid event type: {EventType}");
            var length = Length;
            MemoryMarshal.Write(output, ref length);
            var bytesWritten = lengthLength;
            var caret = 0;
            bytesWritten += Encoding.UTF8.GetBytes(dataType, output[(caret += lengthLength)..]);
            bytesWritten += Encoding.UTF8.GetBytes(EventType, output[(caret += dataTypeLength)..]);
            bytesWritten += data.Encode(output[(caret += EventType.Length)..]);
            return bytesWritten;
        }

        public static Packet Decode(ReadOnlySpan<byte> input)
        {
            int packetLength = MemoryMarshal.Read<int>(input);
            //if (packetLength != input.Length) throw new InvalidPayloadException($"Invalid packet length: Expected {packetLength} but got {input.Length}");
            var caret = lengthLength;
            var dataType = Encoding.UTF8.GetString(input[caret..(caret += dataTypeLength)]);
            var eventType = Encoding.UTF8.GetString(input[caret..(caret += eventTypeLength)]);
            var decoder = GetDecoder(eventType) ?? throw new InvalidPayloadException($"Invalid event type: {eventType}");
            return new(dataType, decoder(input[caret..packetLength]));
        }

        public int Length => lengthLength + dataTypeLength + eventTypeLength + data.Length;
    }
}
