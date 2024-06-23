using UnityEngine;
using Unity.Netcode;

public class NetworkManagerGUI : MonoBehaviour
{
    private NetworkManager manager;

    private void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!manager.IsClient && !manager.IsServer)
        {
            if (GUILayout.Button("Start Host"))
            {
                manager.StartHost();
            }
            if (GUILayout.Button("Start Client"))
            {
                manager.StartClient();
            }
            if (GUILayout.Button("Start Server"))
            {
                manager.StartServer();
            }
        }
        else
        {
            // status labels
            string mode = "NONE";
            if (manager.IsHost)
            {
                mode = "Host";
            }
            else if (manager.IsServer)
            {
                mode = "Server";
            }
            else if (manager.IsClient)
            {
                mode = "Client";
            }

            GUILayout.Label($"Transport: {manager.NetworkConfig.NetworkTransport.GetType().Name}");
            GUILayout.Label($"Mode: {mode}");

            // submit new position
            if (GUILayout.Button(manager.IsServer ? "Move" : "Request Move"))
            {


            }
        }

        GUILayout.EndArea();
    }
}
