using Unity.Netcode;
using UnityEngine;

public class DefenceSpawner : NetworkBehaviour
{
    [SerializeField] DefenceAsset defenceAsset;

    [SerializeField] Transform leftParent;
    [SerializeField] Transform rightParent;

    public bool canSpawn = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsHost)
            return;
    }


    public int DefenceCost(int indx)
    {
        return defenceAsset.defence[indx].GetComponent<Defence>().cost;
    }
    public void NewDefence(int indx, bool isLeftTeam, ulong playerId)
    {
        SpawnDefenceRPC(indx, isLeftTeam, playerId);
    }

    [Rpc(SendTo.Server)]
    void SpawnDefenceRPC(int indx, bool isLeftTeam, ulong playerId)
    {
        Vector3 newPos;
        NetworkObject newDefence = NetworkManager.SpawnManager.InstantiateAndSpawn(defenceAsset.defence[indx], NetworkManager.LocalClientId);
        if (isLeftTeam)
        {
            newDefence.TrySetParent(leftParent);
            newPos = leftParent.position;
        }
        else
        {
            newDefence.TrySetParent(rightParent);
            newPos = rightParent.position;
        }


        newDefence.transform.position = newPos;
        newDefence.GetComponent<Defence>().ownerId = playerId;
    }
}
