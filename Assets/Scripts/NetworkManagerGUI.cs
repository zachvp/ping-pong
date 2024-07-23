using UnityEngine;
using Unity.Netcode;

public class NetworkManagerGUI : MonoBehaviour
{
    private NetworkManager manager;
    
    private GUIStyle defaultLabelStyle;
    public Color textColor = Color.black;
    public int fontSize = 24;
    public int padding = 16;
    public int margin = 16;
    public FontStyle fontStyle = FontStyle.Bold;

    private GUIStyle defaultButtonStyle;

    private void Awake()
    {
        manager = GetComponent<NetworkManager>();

        defaultLabelStyle = new GUIStyle();
        defaultButtonStyle = new GUIStyle();
    }

    private void OnGUI()
    {
        // update styles

        // label style
        defaultLabelStyle.normal.textColor = textColor;
        defaultLabelStyle.fontSize = fontSize;
        defaultLabelStyle.fontStyle = fontStyle;
        defaultLabelStyle.margin = new RectOffset(margin, margin, margin, margin);

        // button style
        GUI.skin.button.normal.textColor = textColor;
        GUI.skin.button.fontSize = fontSize;
        GUI.skin.button.fontStyle = fontStyle;
        GUI.skin.button.stretchWidth = true;
        GUI.skin.button.fixedHeight = 48;
        GUI.skin.button.padding = new RectOffset(padding, padding, padding, padding);
        GUI.skin.button.margin = new RectOffset(margin, margin, margin, margin);

        GUILayout.BeginArea(new Rect(margin, margin, 300, 300));

        GUILayout.BeginHorizontal();
        if (!manager.IsHost && !manager.IsClient)
        {
            if (GUILayout.Button("Start Host"))
            {
                manager.StartHost();
            }
        }

        if (!manager.IsClient)
        {
            if (GUILayout.Button("Start Client"))
            {
                manager.StartClient();
            }
        }
        GUILayout.EndHorizontal();

        // status labels
        var mode = "NONE";
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

        //GUILayout.Label($"Transport: {manager.NetworkConfig.NetworkTransport.GetType().Name}");
        GUILayout.Label($"Mode: {mode}", defaultLabelStyle);

        GUILayout.EndArea();
    }
}
