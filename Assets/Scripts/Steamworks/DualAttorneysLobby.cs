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
    /// Returns the Steam ID of the lobby.
    /// </summary>
    public CSteamID lobbyID { get; private set; } = CSteamID.Nil;

    Callback<LobbyCreated_t> lobbyCreated;
    Callback<LobbyEnter_t> lobbyEntered;

    public DualAttorneysLobby()
    {
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.Log("[Steamworks.NET] Could not create lobby.");
            return;
        }

        Debug.Log("[Steamworks.NET] Lobby created successfully.");
    }

    void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (callback.m_EChatRoomEnterResponse != 1)
        {
            Debug.Log("[Steamworks.NET] Could not enter lobby.");
            return;
        }

        Debug.Log("[Steamworks.NET] Lobby entered successfully.");
    }

}
