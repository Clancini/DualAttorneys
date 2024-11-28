using Steamworks;
using UnityEngine;

public static class SteamFriend
{
    /// <summary>
    /// Returns the alias of the friend as appears in the friends list.
    /// Will return "Not Initialized" if Steam is not initialized.
    /// Will return "" or "[unknown]" if the friend is not found.
    /// </summary>
    public static string GetFriendPersonaName(CSteamID steamID)
    {
        if (SteamManager.Initialized)
        {
            return SteamFriends.GetFriendPersonaName(steamID);
        }
        return "Not Initialized";
    }

    /// <summary>
    /// Returns an handle to the friend's avatar as medium (64x64) size.
    /// Will return 0 if Steam is not initialized or if the friend has no avatar.
    /// </summary>
    public static int GetFriendMediumAvatarHandle(CSteamID steamID)
    {
        if (SteamManager.Initialized)
        {
            return SteamFriends.GetMediumFriendAvatar(steamID);
        }
        return 0;
    }

    /// <summary>
    /// Returns the friend's avatar as (a medium size) 64x64 Texture2D.
    /// Will return null if Steam is not initialized, if the friend is not found, or if the friend has no avatar.
    /// </summary>
    public static Texture2D GetFriendMediumAvatar(CSteamID steamID)
    {
        if (SteamManager.Initialized)
        {
            int avatarHandle = GetFriendMediumAvatarHandle(steamID);
            if (avatarHandle != 0)
            {
                uint width, height;
                if (SteamUtils.GetImageSize(avatarHandle, out width, out height) && width > 0 && height > 0)
                {
                    byte[] avatarRGBA = new byte[4 * width * height];   // The size of the RGBA data is width * height * 4 bytes (1 byte per channel)
                    if (SteamUtils.GetImageRGBA(avatarHandle, avatarRGBA, 4 * (int)width * (int)height))
                    {
                        Texture2D avatarTexture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                        avatarTexture.LoadRawTextureData(avatarRGBA);
                        avatarTexture.Apply();
                        return avatarTexture;
                    }
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Returns the state of the friend as appears in the friends list (online, away, etc).
    /// Will return k_EPersonaStateOffline if Steam is not initialized.
    /// </summary>
    public static EPersonaState GetFriendPersonaState(CSteamID steamID)
    {
        if (SteamManager.Initialized)
        {
            return SteamFriends.GetFriendPersonaState(steamID);
        }
        return EPersonaState.k_EPersonaStateOffline;
    }
}
