using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RpcTest : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
       if (!IsServer && !IsOwner)
       {
           ServerOnlyRpc(0, NetworkObjectId);
       }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void ClientAndHostRpc(int value, ulong sourceNetworkObjectId)
    {
        Debug.Log("ClientAndHostRpc called with value: " + value + " from sourceNetworkObjectId: " + sourceNetworkObjectId);
        if (IsOwner)
        {
            ServerOnlyRpc(value + 1, sourceNetworkObjectId);
        }
    }
    [Rpc(SendTo.Server)]
    void ServerOnlyRpc(int value, ulong sourceNetworkObjectId)
    {
        Debug.Log("ServerOnlyRpc called with value: " + value + " from sourceNetworkObjectId: " + sourceNetworkObjectId);
        if (IsServer)
        {
            ClientAndHostRpc(value + 1, sourceNetworkObjectId);
        }
    }
}
