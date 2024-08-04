using System;
using Unity.Netcode;

public class NetworkDeactivate : NetworkBehaviour
{
    public Mode mode;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner && mode == Mode.DEACTIVATE_NON_OWNER)
        {
            gameObject.SetActive(false);
        }
    }

    public enum Mode
    {
        NONE = 0,
        DEACTIVATE_NON_OWNER = 1,
    }
}
