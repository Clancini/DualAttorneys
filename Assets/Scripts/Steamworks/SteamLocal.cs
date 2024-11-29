using Steamworks;
using UnityEngine;

public static class SteamLocal
{
    /// <summary>
    /// Returns the Steam ID of the local user. <br/>
    /// Will return CSteamID.Nil if Steam is not initialized.
    /// </summary>
    public static CSteamID LocalSteamID
    {
        get
        {
            if (SteamManager.Initialized)
            {
                return SteamUser.GetSteamID();
            }
            return CSteamID.Nil;
        }
    }

    /// <summary>
    /// Returns the Steam name of the local user as appears in the community profile. <br/>
    /// Will return "Not Initialized" if Steam is not initialized.
    /// </summary>
    public static string LocalPersonaName
    {
        get
        {
            if (SteamManager.Initialized)
            {
                return SteamFriends.GetPersonaName();
            }
            return "Not Initialized";
        }
    }

    /// <summary>
    /// Returns the state of the local user as appears in the friends list (online, away, etc). <br/>
    /// Will return k_EPersonaStateOffline if Steam is not initialized.
    /// </summary>
    public static EPersonaState LocalFriendState
    {
        get
        {
            if (SteamManager.Initialized)
            {
                return SteamFriends.GetPersonaState();
            }
            return EPersonaState.k_EPersonaStateOffline;
        }
    }

    /// <summary>
    /// Returns the avatar handle of the local user as medium (64x64) size. <br/>
    /// To be used with GetImageSize and GetImageRGBA. <br/>
    /// Will return 0 if Steam is not initialized or if the user has no avatar.
    /// </summary>
    public static int LocalMediumAvatarHandle
    {
        get
        {
            if (SteamManager.Initialized)
            {
                return SteamFriends.GetMediumFriendAvatar(LocalSteamID);
            }
            return 0;
        }
    }

    /// <summary>
    /// Returns the local user's medium (64x64) avatar as a Texture2D. <br/>
    /// Will return null if Steam is not initialized or if the user has no avatar.
    /// </summary>
    public static Texture2D GetLocalMediumAvatar()
    {
        if (SteamManager.Initialized)
        {
            int avatarHandle = LocalMediumAvatarHandle;
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
}
