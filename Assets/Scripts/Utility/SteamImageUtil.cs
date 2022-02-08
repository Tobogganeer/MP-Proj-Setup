using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Steamworks.Data;

public static class SteamImageUtil
{
    public static async System.Threading.Tasks.Task<Texture2D> GetSmallPFP(SteamId friend)
    {
        Image? image = await SteamFriends.GetSmallAvatarAsync(friend);
        if (!image.HasValue)
            return null;
        else
            return GetTextureFromImage(image.Value);
    }

    public static async System.Threading.Tasks.Task<Texture2D> GetMediumPFP(SteamId friend)
    {
        Image? image = await SteamFriends.GetMediumAvatarAsync(friend);
        if (!image.HasValue)
            return null;
        else
            return GetTextureFromImage(image.Value);
    }

    public static async System.Threading.Tasks.Task<Texture2D> GetLargePFP(SteamId friend)
    {
        Image? image = await SteamFriends.GetLargeAvatarAsync(friend);
        if (!image.HasValue)
            return null;
        else
            return GetTextureFromImage(image.Value);
    }

    public static Texture2D GetTextureFromImage(Image image)
    {
        Texture2D tex = new Texture2D((int)image.Width, (int)image.Height);

        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                Steamworks.Data.Color col = image.GetPixel(x, y);

                tex.SetPixel(x, (int)image.Height - y, new UnityEngine.Color(col.r / 255f, col.g / 255f, col.b / 255f, col.a / 255f));
            }
        }

        tex.Apply();
        return tex;
    }
}