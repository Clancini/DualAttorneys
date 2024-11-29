using Steamworks;
using UnityEngine;

public class DualAttorneysLobby
{
    private static DualAttorneysLobby lobbyInstance;
    public static DualAttorneysLobby Instance
    {
        get
        {
            if (lobbyInstance == null)
            {
                lobbyInstance = new DualAttorneysLobby();
            }
            return lobbyInstance;
        }
    }

    /// <summary>
    /// Returns the Steam ID of the lobby. <br/>
    /// Will be CSteamID.Nil if not in a lobby.
    /// </summary>
    public CSteamID lobbyID { get; private set; } = CSteamID.Nil;

    /// <summary>
    /// Returns the Steam ID of the lobby owner. <br/>
    /// Will be CSteamID.Nil if not in a lobby.
    /// </summary>
    public CSteamID ownerID { get; private set; } = CSteamID.Nil;

    /// <summary>
    /// Returns the listen socket handle. <br/>
    /// Returns HSteamListenSocket.Invalid if not hosting.
    /// </summary>
    public HSteamListenSocket listenSocket { get; private set; }  = HSteamListenSocket.Invalid;

    /// <summary>
    /// Returns the connection handle to the other player from the local user's perspective. <br/>
    /// Will be HSteamNetConnection.Invalid if not connected.
    /// </summary>
    public HSteamNetConnection connection { get; private set; } = HSteamNetConnection.Invalid;

    /// <summary>
    /// Returns the Steam ID of the other player from the local user's perspective. <br/>
    /// Will be CSteamID.Nil if not connected.
    /// </summary>
    public CSteamID otherSteamID { get; private set; } = CSteamID.Nil;

    /// <summary>
    /// Returns the loopback socket instance. <br/>
    /// It is not guaranteed that the loopback socket is being used. <br/>
    /// Does NOT handle connection status changes and should only be used for testing network code.  <br/>
    /// Does NOT need a lobby to work.
    /// </summary>
    public LoopbackSocket loopbackSocket { get; private set; } = LoopbackSocket.Instance;

    /// <summary>
    /// Returns whether the loopback socket should be used over P2P sockets. <br/>
    /// Always false if not in the editor or development build.
    /// </summary>
    public bool useLoopbackSocket { get; private set; } = false;

    Callback<LobbyCreated_t> lobbyCreated;
    Callback<LobbyEnter_t> lobbyEntered;
    Callback<SteamNetConnectionStatusChangedCallback_t> connectionStatusChange;

    public DualAttorneysLobby()
    {
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        connectionStatusChange = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(OnConnectionStatusChange);
    }

    void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.Log("[Steamworks.NET] Could not create lobby.");
            return;
        }

        Debug.Log("[Steamworks.NET] Lobby created successfully.");

        listenSocket = SteamNetworkingSockets.CreateListenSocketP2P(0, 0, null);    // The host created the lobby, and now they need to create a P2P listen socket.
    }

    void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (callback.m_EChatRoomEnterResponse != 1)
        {
            Debug.Log("[Steamworks.NET] Could not enter lobby.");
            return;
        }

        Debug.Log("[Steamworks.NET] Lobby entered successfully.");

        lobbyID = new CSteamID(callback.m_ulSteamIDLobby);  // Store the lobby ID here since both players run this code.
        ownerID = SteamMatchmaking.GetLobbyOwner(lobbyID);

        // At this point, the host has created the listen socket, and the client has joined the lobby.
        // The client now need to connect to the host's listen socket.
        // This obviously doesn't have to run on the host's side, so we check if the local user is the host (lobby owner), and if not, carry on.
        if (ownerID == SteamLocal.LocalSteamID)
            return;

        SteamNetworkingIdentity remoteIdentity = new SteamNetworkingIdentity();
        remoteIdentity.SetSteamID(ownerID);
        connection = SteamNetworkingSockets.ConnectP2P(ref remoteIdentity, 0, 0, null);
    }

    void OnConnectionStatusChange(SteamNetConnectionStatusChangedCallback_t callback)
    {
        switch(callback.m_info.m_eState)
        {
            case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting:  // The host (and only host) receives this callback when the client is connecting.

                Debug.Log("[Steamworks.NET] Other peer is connecting...");

                SteamNetworkingSockets.AcceptConnection(callback.m_hConn);
                connection = callback.m_hConn;  // We now know the connection handle to the client and store it here.
                break;

            case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected:   // The client and host receive this callback when the connection is established.

                Debug.Log("[Steamworks.NET] Connected to peer.");

                otherSteamID = callback.m_info.m_identityRemote.GetSteamID();  // We now know the other player's Steam ID from both perspectives and store it here.
                break;

            case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer:    // The client receives this callback when the connection is closed or refused by the host.

                Debug.LogFormat("[Steamworks.NET] Connection closed by peer." +
                    "\nm_eEndReason: {0}" +
                    "\nm_szEndDebug: {1}", callback.m_info.m_eEndReason, callback.m_info.m_szEndDebug);

                SteamLobbyUtils.CloseConnection();

                break;

            case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally:    // Both receive this callback when an error occurs locally (e.g. timeout, auth, etc...).

                Debug.LogFormat("[Steamworks.NET] Problem detected locally." +
                    "\nm_eEndReason: {0}" +
                    "\nm_szEndDebug: {1}", callback.m_info.m_eEndReason, callback.m_info.m_szEndDebug);

                SteamLobbyUtils.CloseConnection();

                break;
        }
    }

    /// <summary>
    /// Toggles whether the loopback socket should be used over P2P sockets. <br/>
    /// Can only be used in the editor and development builds.
    /// </summary>
    /// <param name="enabled"></param>
    public void UseLoopbackSocket(bool enabled)
    {
        if (!Application.isEditor && !Debug.isDebugBuild)
            return;
           
        useLoopbackSocket = enabled;
    }

    public void Destory()
    {
        SteamLobbyUtils.CloseConnection();
        SteamLobbyUtils.LeaveLobby();
    }
}
