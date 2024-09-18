using Godot;
using GodotUtils;
using System;
using System.Threading.Tasks;
using Template.Netcode;
using Template.Netcode.Client;
using Template.Netcode.Server;

namespace Template.TopDown2D;

public class Net
{
    public event Action<ENetServer> OnServerCreated;
    public event Action<ENetClient> OnClientCreated;

    public static int HeartbeatPosition { get; } = 20;
    public static Vector2 PlayerSpawnPosition { get; } = new Vector2(100, 100);

    public ENetServer Server { get; private set; }
    public ENetClient Client { get; private set; }

    private IGameServerFactory _serverFactory;
    private IGameClientFactory _clientFactory;

    public Net()
    {
        Global.Services.Add(this);
        Global.Services.Get<Global>().OnQuit += StopThreads;
    }

    public void Initialize(IGameServerFactory serverFactory, IGameClientFactory clientFactory)
    {
        _serverFactory = serverFactory;
        _clientFactory = clientFactory;

        Server = serverFactory.CreateServer();
        Client = clientFactory.CreateClient();
    }

    public void StopServer()
    {
        Server.Stop();
    }

    public void StartServer()
    {
        if (Server.IsRunning)
        {
            Server.Log("Server is running already");
            return;
        }

        Server = _serverFactory.CreateServer();
        OnServerCreated?.Invoke(Server);
        Server.Start(25565, 100, new ENetOptions
        {
            PrintPacketByteSize = false,
            PrintPacketData = false,
            PrintPacketReceived = false,
            PrintPacketSent = false
        });
    }

    public void StartClient(string ip, ushort port, string username)
    {
        if (Client.IsRunning)
        {
            Client.Log("Client is running already");
            return;
        }

        Client = _clientFactory.CreateClient();
        OnClientCreated?.Invoke(Client);
        Client.Connect(ip, port, new ENetOptions
        {
            PrintPacketByteSize = false,
            PrintPacketData = false,
            PrintPacketReceived = false,
            PrintPacketSent = false
        });

        Client.OnConnected += () =>
        {
            Client.Send(new CPacketJoin
            {
                Username = username,
                Position = PlayerSpawnPosition
            });
        };
    }

    public void StopClient()
    {
        if (!Client.IsRunning)
        {
            Client.Log("Client was stopped already");
            return;
        }

        Client.Stop();
    }

    private async Task StopThreads()
    {
        // Stop the server and client
        if (ENetLow.ENetInitialized)
        {
            if (Server.IsRunning)
            {
                Server.Stop();

                while (Server.IsRunning)
                    await Task.Delay(1);
            }

            if (Client.IsRunning)
            {
                Client.Stop();

                while (Client.IsRunning)
                    await Task.Delay(1);
            }

            ENet.Library.Deinitialize();
        }

        // Wait for the logger to finish enqueing the remaining logs
        while (Global.Services.Get<Logger>().StillWorking())
            await Task.Delay(1);
    }
}

