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
}
