using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using TMPro;



public class PlayerNetwork : NetworkBehaviour
{
    public GameObject SpawnpointP1;
    public GameObject SpawnpointP2;
    public GameObject bulletPrefab;
    public GameObject gun;
    public TextMeshProUGUI WinText;
    float playerHP = 3f;
    float moveSpeed = 3f;
    Vector2 moveDir = new Vector2(0, 0);
    [SerializeField] public List<string>Emotes;
    public TextMeshProUGUI HostEmote;
    public TextMeshProUGUI ClientEmote;
    
    private Coroutine _emoteCoroutine;

    // generates the spawnpoint for the player
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
    // Awake function to find the emote text locations
    public void Awake()
    {
        HostEmote = GameObject.Find("HostEmote").GetComponent<TextMeshProUGUI>();
        ClientEmote = GameObject.Find("ClientEmote").GetComponent<TextMeshProUGUI>();
    }
    // calls to the server to show the emote for all players
    [ServerRpc]
    private void ClientEmotedServerRpc(int emoteIndex, ServerRpcParams serverRpcParams = default)
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            SetEmoteTextClientRpc((int)serverRpcParams.Receive.SenderClientId, emoteIndex);
        }
        
    }
    // makes sure the emote is beeing overriden if another emote was pressed for each player
    [ClientRpc]
    private void SetEmoteTextClientRpc(int playerIndex, int emoteIndex, ClientRpcParams clientRpcParams = default)
    {
        if (_emoteCoroutine != null)
        {
            StopCoroutine(_emoteCoroutine);
        }
        
        _emoteCoroutine = StartCoroutine(EmoteCoroutine(playerIndex, emoteIndex));
    }
    // coroutine for emotes where it takes in the player index and the emote index & will display it for 3 seconds on coresponding players emote location
    private IEnumerator EmoteCoroutine(int playerIndex, int emoteIndex)
    {
        if (playerIndex == 0)
            HostEmote.text = Emotes[emoteIndex];
        else
            ClientEmote.text = Emotes[emoteIndex];
        
        yield return new WaitForSeconds(3);
        
        if (playerIndex == 0)
            HostEmote.text = "";
        else
            ClientEmote.text = "";
    }
    
    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        
        // movement
        moveDir = Vector2.zero;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            instantiateBulletServerRpc();
        }
        // if (Input.GetKey(KeyCode.LeftShift))
        // {
        //     transform.localScale = new Vector3(1, 0.5f, 1);
        // }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir.x = -1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveDir.x = 1;
        }
        transform.position += new Vector3(moveDir.x, moveDir.y, 0) * moveSpeed * Time.deltaTime;

        // emotes
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ClientEmotedServerRpc(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ClientEmotedServerRpc(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ClientEmotedServerRpc(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ClientEmotedServerRpc(3);
        }
        
        
    }

    // a collision check for bullets hitting the player & if it hits the owner of the bullets he wont take damage
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsOwner)
        {
            return;
        }
        if (other.gameObject.tag == "Bullet" && other.gameObject.GetComponent<NetworkObject>().OwnerClientId != NetworkManager.Singleton.LocalClientId)
        {
            TakeDamageServerRpc(1);
        }
    }

   // server rpc for taking damage which either player can call
    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc(float damage, ServerRpcParams serverRpcParams = default)
    {

        playerHP -= damage;
        if (playerHP <= 0)
        {
            // if (IsClient && !IsHost)
            // {
            //     ShowEndscreenClientRpc(1);
            //     Destroy(gameObject);
            // }
            // else if (IsHost)
            // {
            //     ShowEndscreenClientRpc(2);
            //     Destroy(gameObject);
            // }

            // if the sender (the one that dies) is the host
            if (serverRpcParams.Receive.SenderClientId == NetworkManager.Singleton.ConnectedClientsList[0].ClientId)
            {
                ShowEndscreenClientRpc(2);
            }
            //otherwise
            else
            {
                ShowEndscreenClientRpc(1);
            }
            
            Destroy(gameObject);

        }
    }
    // client rpc for showing the endscreen to each player so they are synced up
    [ClientRpc]
    private void ShowEndscreenClientRpc(int winningPlayer)
    {
        WinText = GameObject.Find("WinText").GetComponent<TextMeshProUGUI>();
        WinText.text = "Player " + winningPlayer + " wins!";
    }
    // server rpc for instantiating the bullet  taking in the bullet prefab and the direction of the gun
    [ServerRpc]
    private void instantiateBulletServerRpc(ServerRpcParams serverRpcParams = default)
    {
        //NetworkManager.Singleton.GetComponent<>()
        //serverRpcParams.Receive.SenderClientId;
        
        Vector2 direction = gun.transform.up;
        GameObject bullet = Instantiate(bulletPrefab, gun.transform.position, gun.transform.rotation);
        NetworkObject bulletNetworkObject = bullet.GetComponent<NetworkObject>();
        bulletNetworkObject.SpawnWithOwnership(serverRpcParams.Receive.SenderClientId,true);
        
        
        bullet.GetComponent<Rigidbody2D>().velocity = direction * 10;
        bullet.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    }
    

}
