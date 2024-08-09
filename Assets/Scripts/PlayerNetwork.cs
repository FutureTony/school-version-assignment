using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;


public class PlayerNetwork : NetworkBehaviour
{
    public NetworkVariable<Vector2> Position = new NetworkVariable<Vector2>();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Move();
        }
    }

    public void Move()
    {
        SubmitPositionRequestRpc();
    }

    [Rpc(SendTo.Server)]
    void SubmitPositionRequestRpc(RpcParams rpcParams = default)
    {
        var randomPosition = GetRandomPositionInGame();
        transform.position = randomPosition;
        Position.Value = randomPosition;
    }
    static Vector2 GetRandomPositionInGame()
    {
        return new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
    }

    private void Update()
    {
        transform.position = Position.Value;
    }
}
