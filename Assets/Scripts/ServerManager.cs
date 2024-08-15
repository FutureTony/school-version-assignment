using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public NetworkManager networkManager;
    // Start is called before the first frame update
    private void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
        
    }
    // setting up GUI area size for buttons
    private void OnGUI()
    {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!networkManager.IsServer && !networkManager.IsClient)
        {
            StartButtons();
        }
        else
        {
            GUILabels();

            SubmitAction();
        }
        GUILayout.EndArea();
    }
    // buttons to start the server, host, or client
    private void StartButtons()
    {
        if (GUILayout.Button("Host"))
        {
            networkManager.StartHost();
        }
        if (GUILayout.Button("Client"))
        {
            networkManager.StartClient();
        }
        if (GUILayout.Button("Server"))
        {
            networkManager.StartServer();
        }
    }
    // labels for buttons
    private void GUILabels()
    {
        var mode = networkManager.IsHost ? "Host" : networkManager.IsServer ? "Server" : "Client";
        GUILayout.Label("Transport: " + networkManager.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }
    //old unity setup recomendation to start using server rpcs aswell as settijng up a network
    private void SubmitAction()
    {
       if (GUILayout.Button(networkManager.IsServer ? "Move" : "Submit"))
       {
           if(networkManager.IsServer && !networkManager.IsClient)
           {
                foreach (ulong clientId in networkManager.ConnectedClientsIds)
                {
                    // networkManager.SpawnManager.GetPlayerNetworkObject(clientId).GetComponent<PlayerNetwork>().Move();
                }
           }
           else
           {
               // old move function from unity documentation
                var player = networkManager.SpawnManager.GetLocalPlayerObject();
                var PlayerObject = player.GetComponent<PlayerNetwork>();
                // PlayerObject.Move();
           }
       }
    }
}
