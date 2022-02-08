using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetExtensions
{
    public static string SteamName(this Steamworks.SteamId id) => new Steamworks.Friend(id).Name;
}
