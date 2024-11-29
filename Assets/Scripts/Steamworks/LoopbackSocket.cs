using Steamworks;

public class LoopbackSocket
{
    private static LoopbackSocket instance;
    public static LoopbackSocket Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LoopbackSocket();
            }
            return instance;
        }
    }

    /// <summary>
    /// Returns the connection handle to what would be the host if using P2P sockets. <br/>
    /// </summary>
    public HSteamNetConnection connection1 { get; private set; }

    /// <summary>
    /// Returns the connection handle to what would be the client if using P2P sockets. <br/>
    /// </summary>
    public HSteamNetConnection connection2 { get; private set; }

    // These are the same as connection1 and connection2, but are used as fields for the sake of the loopback socket.
    HSteamNetConnection connectionAsField1;
    HSteamNetConnection connectionAsField2;

    public LoopbackSocket()
    {

    }

    /// <summary>
    /// Creates a loopback socket pair. <br/>
    /// Uses local host identities and ephemeral ports. <br/>
    /// CPU time will be used to encrypt and decrypt the messages. <br/>
    /// Supports fake delay and packet loss.
    /// </summary>
    public void CreateLoopbackSocket()
    {
        SteamNetworkingIdentity identity1 = new SteamNetworkingIdentity();
        SteamNetworkingIdentity identity2 = new SteamNetworkingIdentity();

        identity1.SetLocalHost();
        identity2.SetLocalHost();

        SteamNetworkingSockets.CreateSocketPair(out connectionAsField1, out connectionAsField2, true, ref identity1, ref identity2);

        connection1 = connectionAsField1;
        connection2 = connectionAsField2;
    }
}
