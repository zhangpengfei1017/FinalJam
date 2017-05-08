using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Photon.PunBehaviour
{
    HeroController[] allPlayers;
    // Use this for initialization
    void Start()
    {
        //photonView.RPC("OnPlayerJoinedGameplay", PhotonTargets.AllViaServer, null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [PunRPC]
    void OnPlayerJoinedGameplay()
    {
        //allPlayers = FindObjectsOfType<HeroController>();
        //sort by some way       
    }
}
