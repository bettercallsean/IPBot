using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using IPBot.API.Domain.Enums.ValveServerQueries;
using IPBot.API.Domain.ExtensionMethods;

namespace IPBot.API.Domain.Utilities;

public static partial class A2SHelper
{
    private const int ChallengeResponse = 65;

    public static readonly byte[] A2S_INFO = [0xFF, 0xFF, 0xFF, 0xFF, 0x54, 0x53, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x20, 0x45, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x20, 0x51, 0x75, 0x65, 0x72, 0x79, 0x00];
    public static readonly byte[] A2S_PLAYER = [0xFF, 0xFF, 0xFF, 0xFF, 0x55, 0xFF, 0xFF, 0xFF, 0xFF];

    public static async Task<SteamServerInfo> SendA2SRequestsAsync(string address)
    {
        var endPoint = IPEndpointHelper.GetIPEndPoint(address);

        using var client = new UdpClient();
        client.Client.SendTimeout = 500;
        client.Client.ReceiveTimeout = 500;
        client.Connect(endPoint);

        var steamServerInfo = await SendA2SInfoRequestAsync(client, endPoint);

        if (steamServerInfo.Online)
        {
            var connectedPlayerNames = await SendA2SPlayerRequestAsync(client, endPoint);
            steamServerInfo.PlayerCount = (byte)connectedPlayerNames.Count;
            steamServerInfo.PlayerNames = connectedPlayerNames;
        }

        return steamServerInfo;
    }

    private static async Task<SteamServerInfo> SendA2SInfoRequestAsync(UdpClient client, IPEndPoint endPoint)
    {
        SteamServerInfo steamServerInfo;

        await client.SendAsync(A2S_INFO, A2S_INFO.Length);

        byte[] response;
        try
        {
            response = client.Receive(ref endPoint);

            if (response[4] == ChallengeResponse)
            {
                var foo = A2S_INFO.Concat(response[5..]).ToArray();
                client.Send(foo, foo.Length);
                response = client.Receive(ref endPoint);
            }

            using BinaryReader br = new(new MemoryStream(response));

            steamServerInfo = new SteamServerInfo
            {
                Online = true,
                IP = endPoint.Address.ToString(),
                Header = br.ReadByte(),
                Protocol = br.ReadByte(),
                Name = br.ReadAnsiString()[4..],
                Map = br.ReadAnsiString(),
                Folder = br.ReadAnsiString(),
                Game = br.ReadAnsiString(),
                ID = br.ReadInt16(),
                PlayerCount = br.ReadByte(),
                MaxPlayers = br.ReadByte(),
                Bots = br.ReadByte(),
                ServerType = (ServerTypeFlags)br.ReadByte(),
                Environment = (EnvironmentFlags)br.ReadByte(),
                Visibility = (VisibilityFlags)br.ReadByte(),
                VAC = (VACFlags)br.ReadByte(),
                Version = br.ReadAnsiString(),
                ExtraDataFlag = (ExtraDataFlags)br.ReadByte()
            };

            if (!string.IsNullOrWhiteSpace(steamServerInfo.Map))
                steamServerInfo.Map = string.Join(" ", CapitalLetterRegex().Split(steamServerInfo.Map));

            if (steamServerInfo.ExtraDataFlag.HasFlag(ExtraDataFlags.Port))
                steamServerInfo.Port = br.ReadInt16();
            if (steamServerInfo.ExtraDataFlag.HasFlag(ExtraDataFlags.SteamID))
                steamServerInfo.SteamID = br.ReadUInt64();
            if (steamServerInfo.ExtraDataFlag.HasFlag(ExtraDataFlags.Keywords))
                steamServerInfo.Keywords = br.ReadAnsiString();
            if (steamServerInfo.ExtraDataFlag.HasFlag(ExtraDataFlags.GameID))
                steamServerInfo.GameID = br.ReadUInt64();

            return steamServerInfo;
        }
        catch
        {
            return new SteamServerInfo
            {
                Online = false
            };
        }
    }

    private static async Task<List<string>> SendA2SPlayerRequestAsync(UdpClient client, IPEndPoint endPoint)
    {
        var playerNames = new List<string>();
        await client.SendAsync(A2S_PLAYER, A2S_PLAYER.Length);

        byte[] response;
        try
        {
            response = client.Receive(ref endPoint);

            if (response[4] == ChallengeResponse)
            {
                var concatenatedResponse = A2S_PLAYER[..5].Concat(response[5..]).ToArray();
                client.Send(concatenatedResponse, concatenatedResponse.Length);
                response = client.Receive(ref endPoint);
            }

            using BinaryReader br = new(new MemoryStream(response));
            br.ReadBytes(4);
            var header = br.ReadByte();
            var players = br.ReadByte();

            for (int i = 0; i < players; i++)
            {
                var index = br.ReadByte();
                var name = br.ReadAnsiString();
                var score = br.ReadInt32();
                var duration = br.ReadSingle();

                if (!string.IsNullOrEmpty(name))
                {
                    playerNames.Add(name);
                }
            }

            return playerNames;
        }
        catch
        {
            return playerNames;
        }
    }


    [GeneratedRegex("(?<!^)(?=[A-Z])")]
    private static partial Regex CapitalLetterRegex();
}
