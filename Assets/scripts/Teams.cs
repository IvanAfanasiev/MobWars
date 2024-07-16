using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Teams : NetworkBehaviour
{
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    PlayerSpawner playerSpawner;

    private void Awake()
    {
        if (!IsHost) return;
        GetComponent<NetworkObject>().Spawn();
    }

    public void ChooseTeamRpc(bool isLeft)
    {
        print("team changed by player with id"+ NetworkManager.LocalClientId);
        gameManager.SetTeamRpc(NetworkManager.LocalClientId, isLeft);
    }


}
