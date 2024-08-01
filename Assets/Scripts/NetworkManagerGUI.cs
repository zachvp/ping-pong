using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using System.Net;
using System.Net.Sockets;
using Unity.Netcode.Transports.UTP;
using BeaconLib;
using UnityEditor.Search;
using UnityEditor.Animations;

public class NetworkManagerGUI : MonoBehaviour
{
    private NetworkManager manager;
    private UnityTransport transport;

    public Button startHost;
    public Button startClient;
    public Button search;
    public TextMeshProUGUI mode;
    public TMP_InputField ipInput;

    public SavedSettings settings;
    
    private Beacon beacon;
    private Probe probe;

    private void Awake()
    {
        manager = GetComponent<NetworkManager>();
        transport = GetComponent<UnityTransport>();

        var appName = "PingPong";
        beacon = new(appName, transport.ConnectionData.Port);
        probe = new(appName);

        if (settings.ipAddress.Trim().Length > 0 )
            ipInput.text = settings.ipAddress;

        // register UI button listeners to start host and client
        startHost.onClick.AddListener(() =>
        {
            manager.StartHost();
            UpdateUI();

            // start beacon server
            beacon.BeaconData = $"PingPong Discover Server on {Dns.GetHostName()}";
            beacon.Start();
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

        // search button
        search.onClick.AddListener(() =>
        {
            Debug.Log($"start search");

            probe.BeaconsUpdated += (beacons) =>
            {
                foreach (var beacon in beacons)
                {
                    Debug.LogFormat($"{beacon.Address}: {beacon.Data}");
                }
            };

            probe.Start();
        });

        // client connected event
        manager.OnClientConnectedCallback += (clientId) =>
        {
            if (manager.IsHost)
                Debug.Log($"server started, i am the host");

            // display the client's LAN IP address
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
                Debug.LogError("Unable to find local IP address");
        };
    }

    private void OnDestroy()
    {
        beacon.Stop();
        probe.Stop();
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
