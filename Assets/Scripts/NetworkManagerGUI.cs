using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class NetworkManagerGUI : MonoBehaviour
{
    private NetworkManager manager;

    public Button startHost;
    public Button startClient;
    public TextMeshProUGUI mode;

    private void Awake()
    {
        manager = GetComponent<NetworkManager>();

        startHost.onClick.AddListener(() =>
        {
            manager.StartHost();
            UpdateUI();
        });

        startClient.onClick.AddListener(() =>
        {
            manager.StartClient();
            UpdateUI();
        });
    }


    private void UpdateUI()
    {
        var modeValue = "NONE";
        if (manager.IsHost)
        {
            modeValue = "Host";
            startHost.enabled = false;
            startClient.enabled = false;
        }
        else if (manager.IsServer)
        {
            modeValue = "Server";
            startHost.enabled = false;
            startClient.enabled = false;
        }
        else if (manager.IsClient)
        {
            modeValue = "Client";
            startHost.enabled = false;
            startClient.enabled = false;
        }

        mode.text = $"Mode: {modeValue}";
    }
}
