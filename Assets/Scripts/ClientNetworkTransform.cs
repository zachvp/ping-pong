using Unity.Netcode.Components;
using UnityEngine;

// original file source:
//      https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop/blob/main/Packages/com.unity.multiplayer.samples.coop/Utilities/Net/ClientAuthority/ClientNetworkTransform.cs

/// <summary>
/// Used for syncing a transform with client side changes. This includes host. Pure server as owner isn't supported by this. Please use NetworkTransform
/// for transforms that'll always be owned by the server.
/// </summary>
[DisallowMultipleComponent]
public class ClientNetworkTransform : NetworkTransform
{
    public bool isAuthoritative;

    /// <summary>
    /// Used to determine who can write to this transform. Owner client only.
    /// This imposes state to the server. This is putting trust on your clients. Make sure no security-sensitive features use this transform.
    /// </summary>
    protected override bool OnIsServerAuthoritative()
    {
        return isAuthoritative;
    }
}
