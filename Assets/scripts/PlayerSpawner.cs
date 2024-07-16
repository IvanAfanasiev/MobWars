using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField]
    Transform[] positions;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsHost)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    public void SpawnPlayer(ulong id, bool isLeftTeam)
    {
        print(id % 2);
        NetworkObject player = NetworkManager.Singleton.ConnectedClients[id].PlayerObject;
        player.GetComponent<PlayerNet>().SetupRPC(
            positions[isLeftTeam? 0 : 1].position,
            isLeftTeam);
    }
}
