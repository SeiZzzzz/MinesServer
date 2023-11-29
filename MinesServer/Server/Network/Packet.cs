using MinesServer.Network.Auth;
using MinesServer.Network.BotInfo;
using MinesServer.Network.Chat;
using MinesServer.Network.ConnectionStatus;
using MinesServer.Network.Constraints;
using MinesServer.Network.GUI;
using MinesServer.Network.Movement;
using MinesServer.Network.Programmator;
using MinesServer.Network.Tutorial;
using MinesServer.Network.World;
using System.Runtime.InteropServices;
using System.Text;

namespace MinesServer.Network
{
    public delegate IDataPartBase PacketDecoder(ReadOnlySpan<byte> decodeFrom);
    public readonly record struct Packet(string dataType, ITopLevelPacket data) : IDataPart<Packet>
    {
        const int dataTypeLength = sizeof(byte);
        const int eventTypeLength = sizeof(byte) * 2;
        const int lengthLength = sizeof(int);

        public string PacketName => throw new NotImplementedException();

        public string EventType => data.PacketName;

        private static PacketDecoder? GetDecoder(string packetName) => packetName switch
        {
            TYPacket.packetName => x => TYPacket.Decode(x), // TY
            AUPacket.packetName => x => AUPacket.Decode(x), // AU
            AEPacket.packetName => x => AEPacket.Decode(x), // AE
            AHPacket.packetName => x => AHPacket.Decode(x), // AH
            BotInfoPacket.packetName => x => BotInfoPacket.Decode(x), // BI
            TPPacket.packetName => x => TPPacket.Decode(x), // @T
            SmoothTPPacket.packetName => x => SmoothTPPacket.Decode(x), // @t
            BasketPacket.packetName => x => BasketPacket.Decode(x), // @B
            GuPacket.packetName => x => GuPacket.Decode(x), // Gu
            LivePacket.packetName => x => LivePacket.Decode(x), // @L
            SpeedPacket.packetName => x => SpeedPacket.Decode(x), // sp
            NickListPacket.packetName => x => NickListPacket.Decode(x), // NL
            OnlinePacket.packetName => x => OnlinePacket.Decode(x), // ON
            LevelPacket.packetName => x => LevelPacket.Decode(x), // LV
            MoneyPacket.packetName => x => MoneyPacket.Decode(x), // P$
            OKPacket.packetName => x => OKPacket.Decode(x), // OK
            AutoDiggPacket.packetName => x => AutoDiggPacket.Decode(x), // BD
            GeoPacket.packetName => x => GeoPacket.Decode(x), // GE
            SettingsPacket.packetName => x => SettingsPacket.Decode(x), // #S
            WorldInfoPacket.packetName => x => WorldInfoPacket.Decode(x), // cf
            PongPacket.packetName => x => PongPacket.Decode(x), // PO
            SkillsPacket.packetName => x => SkillsPacket.Decode(x), // @S
            OpenURLPacket.packetName => x => OpenURLPacket.Decode(x), // GR
            ClanShowPacket.packetName => x => ClanShowPacket.Decode(x), // cS
            ClanHidePacket.packetName => x => ClanHidePacket.Decode(x), // cH
            AgressionPacket.packetName => x => AgressionPacket.Decode(x), // BA
            BanHammerPacket.packetName => x => BanHammerPacket.Decode(x), // SU
            MissionProgressPacket.packetName => x => MissionProgressPacket.Decode(x), // MP
            BibikaPacket.packetName => x => BibikaPacket.Decode(x), // BB
            RespPacket.packetName => x => RespPacket.Decode(x), // @R
            NaviArrowPacket.packetName => x => NaviArrowPacket.Decode(x), // GO
            DailyRewardPacket.packetName => x => DailyRewardPacket.Decode(x), // DR
            ChatNotificationPacket.packetName => x => ChatNotificationPacket.Decode(x), // mN
            ChatColorPacket.packetName => x => ChatColorPacket.Decode(x), // mC
            StatusPacket.packetName => x => StatusPacket.Decode(x), // ST
            ReconnectPacket.packetName => x => ReconnectPacket.Decode(x), // RC
            PurchasePacket.packetName => x => PurchasePacket.Decode(x), // $$
            StatePanelPacket.packetName => x => StatePanelPacket.Decode(x), // SP
            BadCellsPacket.packetName => x => BadCellsPacket.Decode(x), // BC
            ProgrammatorPacket.packetName => x => ProgrammatorPacket.Decode(x), // @P
            ChatMessagesPacket.packetName => x => ChatMessagesPacket.Decode(x), // mU
            MissionPanelPacket.packetName => x => MissionPanelPacket.Decode(x), // MM
            CurrentChatPacket.packetName => x => CurrentChatPacket.Decode(x), // mO
            ModulesPacket.packetName => x => ModulesPacket.Decode(x), // PM
            ChatListPacket.packetName => x => ChatListPacket.Decode(x), // mL
            OpenProgrammatorPacket.packetName => x => OpenProgrammatorPacket.Decode(x), // #P
            GUIPacket.packetName => x => GUIPacket.Decode(x), // GU
            PingPacket.packetName => x => PingPacket.Decode(x), // PI
            HBPacket.packetName => x => HBPacket.Decode(x), // HB
            InventoryPacket.packetName => x => InventoryPacket.Decode(x), // IN
            _ => null
        };

        public int Encode(Span<byte> output)
        {
            if (EventType.Length != eventTypeLength) throw new InvalidPayloadException($"Invalid event type length: Expected {eventTypeLength} but got {EventType.Length}");
            if (GetDecoder(EventType) is null) throw new NotImplementedException($"Event type {EventType} is not implemented");
            var length = Length;
            MemoryMarshal.Write(output, in length);
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
            return new(dataType, (ITopLevelPacket)decoder(input[caret..packetLength]));
        }

        public int Length => lengthLength + dataTypeLength + eventTypeLength + data.Length;
    }
}
