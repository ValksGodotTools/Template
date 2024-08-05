namespace Template;

using GodotUtils.Netcode;
using GodotUtils.Netcode.Client;
using GodotUtils.Netcode.Server;

public class Net
{
    public event Action<GameClient> OnClientCreated;
    public event Action<GameServer> OnServerCreated;

    public GameClient Client { get; private set; } = new();
    public GameServer Server { get; private set; } = new();

    public Net()
    {
        Global.Services.Add(this);
        Global.Services.Get<Global>().OnQuit += async () =>
        {
            await StopThreads();
        };
    }

    public void StartServer()
    {
        if (Server.IsRunning)
        {
            Server.Log("Server is running already");
            return;
        }

        Server = new();
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

        Client = new();
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
                Username = username
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
            if (Server != null && Server.IsRunning)
            {
                Server.Stop();

                while (Server.IsRunning)
                    await Task.Delay(1);
            }

            if (Client != null && Client.IsRunning)
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
