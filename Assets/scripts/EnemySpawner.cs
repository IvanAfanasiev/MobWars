using System.Runtime.ConstrainedExecution;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] EnemiesAsset enemiesAsset;
    [SerializeField] float rangeSpawn;

    [SerializeField] Transform clientParent;
    [SerializeField] Transform hostParent;


    public bool canSpawn = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsHost)
            return;
    }
    public int UnitCost(int indx)
    {
        return enemiesAsset.untis[indx].GetComponent<Unit>().cost;
    }
    public int UnitIncome(int indx)
    {
        return enemiesAsset.untis[indx].GetComponent<Unit>().income;
    }
    public void NewUnit(int indx, bool isLeftTeam, ulong playerId, Color clr)
    {
        SpawnUnitRPC(indx, isLeftTeam, playerId, clr);
    }

    [Rpc(SendTo.Server)]
    void SpawnUnitRPC(int indx, bool isLeftTeam, ulong playerId, Color clr)
    {
        Vector3 newPos;
        float randX = Random.Range(-rangeSpawn, rangeSpawn);
        NetworkObject newUnit = NetworkManager.SpawnManager.InstantiateAndSpawn(enemiesAsset.untis[indx], NetworkManager.LocalClientId);
        if (isLeftTeam)
        {
            newUnit.TrySetParent(hostParent);
            newPos = new Vector3(hostParent.position.x + randX, hostParent.position.y, hostParent.position.z);
        }
        else
        {
            newUnit.TrySetParent(hostParent);
            newPos = new Vector3(clientParent.position.x + randX, clientParent.position.y, clientParent.position.z);
        }
        newUnit.transform.position = newPos;


        newUnit.GetComponent<Unit>().SetupRpc(indx, isLeftTeam, playerId, clr);
    }
}
