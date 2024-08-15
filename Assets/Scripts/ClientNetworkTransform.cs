using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetworkTransform : NetworkTransform
{
    // codemonkey video recomendation instead of using the network transform component so clients get network rights
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
    
    
}
