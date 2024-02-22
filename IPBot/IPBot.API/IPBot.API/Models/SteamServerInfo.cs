using System.Net;
using System.Net.Sockets;
using IPBot.API.Domain;
using IPBot.API.Domain.Enums.ValveServerQueries;
using IPBot.API.Domain.ExtensionMethods;

namespace IPBot.API;

public class SteamServerInfo
{
    private IPEndPoint _endPoint;
    private UdpClient _client;
    public byte Header { get; set; }
    public byte Protocol { get; set; }
    public string Name { get; set; }
    public string Map { get; set; }
    public string Folder { get; set; }
    public string Game { get; set; }
    public short ID { get; set; }
    public byte Players { get; set; }
    public byte MaxPlayers { get; set; }
    public string[] PlayerNames { get; set; }
    public byte Bots { get; set; }
    public ServerTypeFlags ServerType { get; set; }
    public EnvironmentFlags Environment { get; set; }
    public VisibilityFlags Visibility { get; set; }
    public VACFlags VAC { get; set; }
    public string Version { get; set; }
    public ExtraDataFlags ExtraDataFlag { get; set; }

    #region Extra Data Flag Members
    public ulong GameID { get; set; }
    public ulong SteamID { get; set; }
    public string Keywords { get; set; }
    public short Port { get; set; }
    #endregion

    #region Custom Data
    public string IP { get; set; }
    public bool Online { get; set; }
    #endregion

    public SteamServerInfo(string address)
    {
        _endPoint = IPEndpointHelper.GetIPEndPoint(address);

        using (_client = new UdpClient())
        {
            _client.Client.SendTimeout = 500;
            _client.Client.ReceiveTimeout = 500;
            _client.Connect(_endPoint);

            Refresh();
        }

        _client = null;
    }

    public void Refresh()
    {
        _client.Send(A2S_INFO, A2S_INFO.Length);
        byte[] response;
        try
        {
            response = _client.Receive(ref _endPoint);

            if (response[4] == 65)
            {
                var foo = A2S_INFO.Concat(response[5..]).ToArray();
                _client.Send(foo, foo.Length);
                response = _client.Receive(ref _endPoint);
            }
        }
        catch
        {
            Online = false;
            return;
        }

        using (BinaryReader br = new(new MemoryStream(response)))
        {
            IP = _endPoint.Address.ToString();
            Header = br.ReadByte();
            Protocol = br.ReadByte();
            Name = br.ReadAnsiString()[4..];
            Map = br.ReadAnsiString();
            Folder = br.ReadAnsiString();
            Game = br.ReadAnsiString();
            ID = br.ReadInt16();
            Players = br.ReadByte();
            MaxPlayers = br.ReadByte();
            Bots = br.ReadByte();
            ServerType = (ServerTypeFlags)br.ReadByte();
            Environment = (EnvironmentFlags)br.ReadByte();
            Visibility = (VisibilityFlags)br.ReadByte();
            VAC = (VACFlags)br.ReadByte();
            Version = br.ReadAnsiString();
            ExtraDataFlag = (ExtraDataFlags)br.ReadByte();

            if (ExtraDataFlag.HasFlag(ExtraDataFlags.Port))
                Port = br.ReadInt16();
            if (ExtraDataFlag.HasFlag(ExtraDataFlags.SteamID))
                SteamID = br.ReadUInt64();
            if (ExtraDataFlag.HasFlag(ExtraDataFlags.Keywords))
                Keywords = br.ReadAnsiString();
            if (ExtraDataFlag.HasFlag(ExtraDataFlags.GameID))
                GameID = br.ReadUInt64();
        }

        _client.Send(A2S_PLAYER, A2S_PLAYER.Length);

        try
        {
            response = _client.Receive(ref _endPoint);

            if (response[4] == 65)
            {
                var foo = A2S_PLAYER[..5].Concat(response[5..]).ToArray();
                _client.Send(foo, foo.Length);
                response = _client.Receive(ref _endPoint);
                var bar = response[5..];

                using BinaryReader br = new(new MemoryStream(response));
                br.ReadBytes(4);
                var header = br.ReadByte(); // D
                var players = br.ReadByte();
                for (int i = 0; i < players; i++)
                {
                    var index = br.ReadByte();
                    var name = br.ReadAnsiString();
                    var score = br.ReadInt32();
                    var duration = br.ReadSingle();
                }



            }
        }
        catch
        {
            return;
        }
    }





    public static readonly byte[] A2S_INFO = [0xFF, 0xFF, 0xFF, 0xFF, 0x54, 0x53, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x20, 0x45, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x20, 0x51, 0x75, 0x65, 0x72, 0x79, 0x00];
    public static readonly byte[] A2S_PLAYER = [0xFF, 0xFF, 0xFF, 0xFF, 0x55, 0xFF, 0xFF, 0xFF, 0xFF];
}
