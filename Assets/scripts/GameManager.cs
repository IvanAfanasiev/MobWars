using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public NetworkVariable<int> maxPlayers = new NetworkVariable<int>(2,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
        );
    [SerializeField]
    PlayerSpawner playerSpawner;
    [SerializeField]
    TextMeshProUGUI waitingText;
    [SerializeField]
    RoundManager roundManager;
    [SerializeField]
    GameObject teamButtons;

    //[SerializeField]
    //List<Image> for player colors

    [SerializeField]
    Animator anim;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsHost)
        {
            //gameObject.SetActive(false);
            return;
        }
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    void OnClientConnected(ulong id)
    {
        Debug.Log("dołączył się "+id);
        if (NetworkManager.ConnectedClientsIds.Count > maxPlayers.Value)
        {
            NetworkManager.Singleton.DisconnectClient(id);
            NetworkManager.ConnectedClientsList[NetworkManager.ConnectedClientsIds.Count-1].PlayerObject.GetComponent<PlayerNet>().Disconnect();
            return;
        }
        else if (NetworkManager.ConnectedClientsIds.Count <= maxPlayers.Value)
            PlayAnimationRpc();
        
        for (int i = 0; i < NetworkManager.ConnectedClients.Count; i++)
        {
            SetTeamRpc(
                NetworkManager.ConnectedClientsIds[i],
                (NetworkManager.ConnectedClientsIds[i] % 2 == 0)? false : true
            );
        }
    }
    void OnClientDisconnected(ulong id)
    {
        byte inLeftTeam = 0;
        byte inRightTeam = 0;
        for (int i = 0; i < NetworkManager.ConnectedClients.Count; i++)
        {
            if (NetworkManager.ConnectedClientsList[i].PlayerObject.GetComponent<PlayerNet>().isLeftTeam.Value)
                inLeftTeam++;
            else 
                inRightTeam++;
        }
        if (inLeftTeam != 0 && inRightTeam != 0)
            return;

        //если в какой-то команде 0 игроков 
    
        //противоположная команда вин
    }


    [Rpc(SendTo.Server)]
    public void SetTeamRpc(ulong id, bool isLeft)
    {
        if (!IsHost) return;
        if (maxPlayers.Value == 2)
        {
            playerSpawner.SpawnPlayer(id, isLeft);
            return;
        }
        byte inTeam = 0;
        for (int i = 0; i < NetworkManager.ConnectedClients.Count; i++)
        {
            if (NetworkManager.ConnectedClientsList[i].PlayerObject.GetComponent<PlayerNet>().isLeftTeam.Value == isLeft)
                inTeam++;
        }
        if (inTeam == 2) return;

        playerSpawner.SpawnPlayer(id, isLeft);
    }


    [Rpc(SendTo.ClientsAndHost)]
    public void GameOverRpc(bool isLeftTeamWin)
    {
        for (int i = 0; i < NetworkManager.ConnectedClients.Count; i++)
        {
            if (isLeftTeamWin == NetworkManager.ConnectedClientsList[i].PlayerObject.GetComponent<PlayerNet>().isLeftTeam.Value)
                NetworkManager.ConnectedClientsList[i].PlayerObject.GetComponent<PlayerNet>().WinLabel();
            else
                NetworkManager.ConnectedClientsList[i].PlayerObject.GetComponent<PlayerNet>().DeafeatLabel();
        }
    }


    [Rpc(SendTo.ClientsAndHost)]
    public void PlayAnimationRpc()
    {
        waitingText.text = "waiting for other "+ (maxPlayers.Value - NetworkManager.ConnectedClientsIds.Count) + " players";
        if (maxPlayers.Value == NetworkManager.ConnectedClientsIds.Count)
            waitingText.text = "";

        if (maxPlayers.Value == 4)
        {
            anim.Play("ClientConnect" + NetworkManager.ConnectedClientsIds.Count);
            teamButtons.SetActive(true);
        }
        else
        {
            teamButtons.SetActive(false);
            anim.Play("ClientConnect" + (NetworkManager.ConnectedClientsIds.Count+2));
        }
    }


    public void Start1vs1()
    {
        maxPlayers.Value = 2;
    }

    public void Start2vs2()
    {
        maxPlayers.Value = 4;
    }

}
