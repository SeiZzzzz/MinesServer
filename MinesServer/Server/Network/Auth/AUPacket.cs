using MinesServer.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinesServer.Network.Auth
{
    public readonly struct AUPacket : IDataPart<AUPacket>
    {
        public enum AuthType
        {
            NoAuth,
            Vk,
            Debug,
            Regular,
            ServerSide
        }

        public readonly AuthType type;
        public readonly string uniq;
        public readonly int? user_id;
        public readonly string token;

        public const string packetName = "AU";

        public string PacketName => packetName;

        public AUPacket(string uniq, bool serverside = true)
        {
            if (serverside) type = AuthType.ServerSide;
            else type = AuthType.NoAuth;
            this.uniq = uniq;
        }

        public AUPacket(string uniq, int user_id, string token)
        {
            type = AuthType.Regular;
            this.uniq = uniq;
            this.user_id = user_id;
            this.token = token;
        }

        public AUPacket(string uniq, string token, bool debug = false)
        {
            if (debug) type = AuthType.Debug;
            else type = AuthType.Vk;
            this.uniq = uniq;
            this.token = token;
        }

        public int Length => uniq.Length + type switch
        {
            AuthType.NoAuth => 8,
            AuthType.Vk => 4 + token.Length,
            AuthType.Debug => 7 + token.Length,
            AuthType.Regular => user_id.Value.Digits() + 2 + token.Length,
            AuthType.ServerSide => 0,
            _ => 0
        };

        public static AUPacket Decode(ReadOnlySpan<byte> decodeFrom)
        {
            var str = Encoding.UTF8.GetString(decodeFrom).Split('_');
            if (str.Length == 1) return new(str[0], true);
            if (str.Length != 3) throw new InvalidPayloadException($"Expected {3} parts but got {str.Length}");
            return str[1] switch
            {
                "NO" => new(str[0], false),
                "DEBUG" => new(str[0], str[2], true),
                "VK" => new(str[0], str[2], false),
                _ => new(str[0], int.Parse(str[1]), str[2])
            };
        }

        public int Encode(Span<byte> output) => Encoding.UTF8.GetBytes(uniq + type switch
        {
            AuthType.NoAuth => "_NO_AUTH",
            AuthType.Vk => $"_VK_{token}",
            AuthType.Debug => $"_DEBUG_{token}",
            AuthType.Regular => $"_{user_id}_{token}",
            AuthType.ServerSide => "",
            _ => ""
        }, output);
    }
}
