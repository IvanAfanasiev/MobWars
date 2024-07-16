using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Unit : NetworkBehaviour
{
    public Color clr;
    [SerializeField]
    GameObject[] ownMaterial;
    [SerializeField]
    PLayerGlobalSettings playerSettings;

    public int maxHP;

    public bool isInLeftTeam;
    [SerializeField]
    int currHP;
    public int damage;
    [Range(0f, 10f)]
    public float speed;
    [Range(0f, 10f)]
    public float attackSpeed;
    float timer = 0;
    public int cost;
    public int deathReward;
    public int income;
    public ulong ownerId;


    [SerializeField]
    GameObject Hps;
    [SerializeField]
    Image hpAmount;
    [SerializeField]
    Image hpAmount2;
    [SerializeField]
    Image hpAmount3;
    [SerializeField]
    NetworkObject deathParticles;

    public Defence target;
    public enum State
    {
        None,
        Idle,
        Walk,
        Attack,
        Death
    }
    public State state;


    void Start()
    {
        currHP = maxHP;
        hpAmount.fillAmount = (float)currHP / maxHP;
        hpAmount2.fillAmount = (float)currHP / maxHP;
        hpAmount3.fillAmount = (float)currHP / maxHP;

        for(int i=0; i< ownMaterial.Length; i++) {
            ownMaterial[i].GetComponent<MeshRenderer>().materials[0].color = clr;
        }
    }
    void Update()
    {
        if (target == null)
            state = State.Walk;
        switch (state)
        {
            case State.None:
                break;
            case State.Idle:
                //Standing on place
                break;
            case State.Walk:
                timer = 0;
                Walk();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Death:
                break;
            default:
                break;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetupRpc(int indx, bool isLeftTeam, ulong playerId, Color clr)
    {
        isInLeftTeam = isLeftTeam;
        ownerId = playerId;
        this.clr = clr;
    }

    void Walk()
    {
        transform.position += Vector3.forward * speed * Time.deltaTime;
    }
    void Attack()
    {
        timer += Time.deltaTime;
        if (timer >= attackSpeed)
        {
            //anim.Play("Attack");
            DealDamage();
            timer = 0;
        }
    }

    public void DealDamage()
    {
        if (!IsHost) return;
        if (target != null)
            target.GetDamageRpc(damage);
    }


    [Rpc(SendTo.ClientsAndHost)]
    public void GetDamageRpc(int damage, ulong playerId)
    {
        Hps.SetActive(true);
        currHP -= damage;
        hpAmount.fillAmount = (float)currHP / maxHP;
        hpAmount2.fillAmount = (float)currHP / maxHP;
        hpAmount3.fillAmount = (float)currHP / maxHP;
        if (currHP <= 0)
        {
            GetKilledRpc(playerId);
        }
    }
    public void Heal(int value)
    {
        currHP += value;
    }


    void Death()
    {
        if (!IsHost) return;
        state = State.Death;
        speed = 0f;

        NetworkObject newParticles = NetworkManager.SpawnManager.InstantiateAndSpawn(deathParticles, NetworkManager.LocalClientId);
        NetworkObject newParticles2 = NetworkManager.SpawnManager.InstantiateAndSpawn(deathParticles, NetworkManager.LocalClientId);
        NetworkObject newParticles3 = NetworkManager.SpawnManager.InstantiateAndSpawn(deathParticles, NetworkManager.LocalClientId);

        newParticles.transform.position = transform.position;
        newParticles2.transform.position = new Vector3(transform.position.x - 100, transform.position.y, transform.position.z);
        newParticles3.transform.position = new Vector3(transform.position.x + 70, transform.position.y, transform.position.z);

        newParticles.TrySetParent(transform.parent.parent.parent);
        newParticles2.TrySetParent(transform.parent.parent.parent);
        newParticles3.TrySetParent(transform.parent.parent.parent);

        Destroy(newParticles.gameObject, 3);
        Destroy(newParticles2.gameObject, 3);
        Destroy(newParticles3.gameObject, 3);
        NetworkManager.Destroy(gameObject);
    }


    [Rpc(SendTo.Server)]
    public void GetKilledRpc(ulong playerId)//get killed by player
    {
        //player, who killed unit, gets money

        //NetworkManager.ConnectedClientsList[(int)playerId].PlayerObject.
        //    GetComponent<PlayerNet>().
        //        GetMoneyRpc(1, isInLeftTeam);

        Death();
    }

    public void Finished() //reqchd the target
    {
        Death();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 7) return; //defence

        state = State.Attack;
        target = other.GetComponent<Defence>();

    }
}
