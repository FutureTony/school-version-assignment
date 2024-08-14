using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.EventSystems;



public class PlayerNetwork : NetworkBehaviour
{
  public GameObject SpawnpointP1;
  public GameObject SpawnpointP2;
  public GameObject bulletPrefab;
  public GameObject gun;
    float playerHP = 3f;
    float moveSpeed = 3f;
    Vector2 moveDir = new Vector2(0, 0);

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;
        
        if (IsHost)
        {
            
            transform.position = SpawnpointP1.transform.position;
            return;
        }

        
        transform.position = SpawnpointP2.transform.position;
        transform.rotation = new Quaternion(0,180, 0,1);
    }


    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        moveDir = Vector2.zero;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            instantiateBulletServerRpc();
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.localScale = new Vector3(1, 0.5f, 1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir.x = -1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveDir.x = 1;
        }

        transform.position += new Vector3(moveDir.x, moveDir.y, 0) * moveSpeed * Time.deltaTime;
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            TakeDamageServerRpc(1);
        }
    }

   
    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc(float damage, ServerRpcParams serverRpcParams = default)
    {
        if(!IsOwner)
            return;
        playerHP -= damage;
        if (playerHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    [ServerRpc]
    private void instantiateBulletServerRpc(ServerRpcParams serverRpcParams = default)
    {
        //NetworkManager.Singleton.GetComponent<>()
        //serverRpcParams.Receive.SenderClientId;
        
        Vector2 direction = gun.transform.up;
        GameObject bullet = Instantiate(bulletPrefab, gun.transform.position, gun.transform.rotation);
        NetworkObject bulletNetworkObject = bullet.GetComponent<NetworkObject>();
        bulletNetworkObject.Spawn(true);
        
        
        bullet.GetComponent<Rigidbody2D>().velocity = direction * 10;
        bullet.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    }
    

}
