using Steamworks;

public static class SteamLobbyUtils
{
    /// <summary>
    /// Creates a new lobby and join it.
    /// </summary>
    /// <param name="lobbyType">Friends-only, public, etc...</param>
    /// <param name="maxMembers">Max players allowed in the lobby.</param>
    public static void CreateLobby(ELobbyType lobbyType, int maxMembers)
    {
        if (SteamManager.Initialized)
        {
            SteamMatchmaking.CreateLobby(lobbyType, maxMembers);
            return;
        }
    }

    /// <summary>
    /// Joins a lobby by its Steam ID.
    /// </summary>
    /// <param name="lobbyID"></param>
    public static void JoinLobby(CSteamID lobbyID)
    {
        if (SteamManager.Initialized)
        {
            SteamMatchmaking.JoinLobby(lobbyID);
            return;
        }
    }

    /// <summary>
    /// Leaves the current lobby.  <br/>
    /// Will do nothing if not in a lobby (DualAttorneysLobby.Instance.lobbyID is CSteamID.Nil).
    /// </summary>
    public static void LeaveLobby()
    {
        if (SteamManager.Initialized)
        {
            if (DualAttorneysLobby.Instance.lobbyID != CSteamID.Nil)
            {
                SteamMatchmaking.LeaveLobby(DualAttorneysLobby.Instance.lobbyID);
                return;
            }
        }
    }

    /// <summary>
    /// Disconnects from the current remote host. Will attempt to send any reliable messages queued before disconnecting (if the application runs long enough). <br/>
    /// nReason in this case will be 0. <br/>
    /// Will do nothing if not connected (DualAttorneysLobby.Instance.connection is HSteamNetConnection.Invalid). <br/>
    /// If the connection is already closed, this will only release resources, ignoring nReason, pszDebug, and bEnableLinger.
    /// </summary>
    public static void CloseConnection()
    {
        if (SteamManager.Initialized)
        {
            if (DualAttorneysLobby.Instance.connection != HSteamNetConnection.Invalid)
            {
                SteamNetworkingSockets.CloseConnection(DualAttorneysLobby.Instance.connection, 0, "Called CloseConnection()", true);
                return;
            }
        }
    }

    /// <summary>
    /// Destroys the current listen socket and forcibly closes all accepted connections. <br/>
    /// Will do nothing if not hosting (DualAttorneysLobby.Instance.listenSocket is HSteamListenSocket.Invalid).
    /// </summary>
    public static void CloseListenSocket()
    {
        if (SteamManager.Initialized)
        {
            if (DualAttorneysLobby.Instance.listenSocket != HSteamListenSocket.Invalid)
            {
                SteamNetworkingSockets.CloseListenSocket(DualAttorneysLobby.Instance.listenSocket);
                return;
            }
        }
    }
}
