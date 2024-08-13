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
    float moveSpeed = 3f;
    Vector2 moveDir = new Vector2(0, 0);

    public override void OnNetworkSpawn()
    {
        if (IsHost && IsOwner)
        {
            transform.position = SpawnpointP1.transform.position;
        }

        if (IsClient && !IsHost && IsOwner)
        {
            transform.position = SpawnpointP2.transform.position;
            transform.rotation = new Quaternion(0,180, 0,1);
        }
    }


    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        moveDir = Vector2.zero;

        if (Input.GetKey(KeyCode.Space))
        {
            instantiateBullet();
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

    private void instantiateBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, gun.transform.position, gun.transform.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = gun.transform.forward * 15;
    }
    

}
