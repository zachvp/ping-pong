using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using System.Net;
using System.Net.Sockets;
using Unity.Netcode.Transports.UTP;

public class NetworkManagerGUI : MonoBehaviour
{
    private NetworkManager manager;
    private UnityTransport transport;

    public Button startHost;
    public Button startClient;
    public TextMeshProUGUI mode;
    public TMP_InputField ipInput;

    private void Awake()
    {
        manager = GetComponent<NetworkManager>();
        transport = GetComponent<UnityTransport>();

        startHost.onClick.AddListener(() =>
        {
            manager.StartHost();
            UpdateUI();
        });

        startClient.onClick.AddListener(() =>
        {
            transport.ConnectionData.Address = ipInput.text;
            var success = manager.StartClient();
            if (success)
                UpdateUI();
            else
                Debug.LogError($"Client connection failed, how to debug?");
        });

        manager.OnServerStarted += () =>
        {
            if (!manager.IsHost)
            {
                return;
            }
            Debug.Log($"server started, i am the host");

            var host = Dns.GetHostEntry(Dns.GetHostName());
            var foundIP = false;
            foreach (var address in host.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    UIDebug.Instance.Register("IP Address", () => address.ToString());
                    foundIP = true;
                }
            }
            if (!foundIP)
            {
                Debug.LogError("Unable to find local IP address");
            }
        };
    }

    private void UpdateUI()
    {
        var modeValue = "NONE";
        if (manager.IsHost)
        {
            modeValue = "Host";
            startHost.gameObject.SetActive(false);
            startClient.gameObject.SetActive(false);
            ipInput.gameObject.SetActive(false);
        }
        else if (manager.IsServer)
        {
            modeValue = "Server";
            startHost.gameObject.SetActive(false);
            startClient.gameObject.SetActive(false);
        }
        else if (manager.IsClient)
        {
            modeValue = "Client";
            startHost.gameObject.SetActive(false);
            startClient.gameObject.SetActive(false);
        }

        mode.text = $"Mode: {modeValue}";
    }
}
