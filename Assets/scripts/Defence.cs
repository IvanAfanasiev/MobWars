using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Defence : NetworkBehaviour
{
    public int cost;
    [SerializeField]
    int damage = 10;
    [SerializeField]
    int maxHP = 100;
    int _currHP;
    public int currHP
    {
        get { return _currHP; }
        set
        {
            if (currHP > maxHP)
                currHP = maxHP;
            else if (value < 0)
                currHP = 0;
            if (value == 0)
                Destroy(gameObject);
            _currHP = value;
        }
    }
    public enum State
    {
        None,
        Idle,
        Attack,
        Destroy
    }
    public State state;
    [Range(0f, 3f)]
    public float attackSpeed;

    [SerializeField]
    Unit target;


    [SerializeField]
    Image hpAmount; 
    [SerializeField]
    Image hpAmount2;
    [SerializeField]
    Image hpAmount3;
    [SerializeField]
    TextMeshProUGUI HpText;

    [SerializeField]
    Transform rotatedPart;
    [SerializeField]
    Transform rotatedPart1;
    [SerializeField]
    Transform rotatedPart2;


    public ulong ownerId;

    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        _currHP = maxHP;
        HpText.text = currHP + "/" + maxHP;
        hpAmount.fillAmount = (float)currHP / maxHP;
        hpAmount2.fillAmount = (float)currHP / maxHP;
        hpAmount3.fillAmount = (float)currHP / maxHP;
    }
    void Update()
    {
        if (target == null) state = State.Idle;
        else state = State.Attack;
        switch (state)
        {
            case State.None:
                break;
            case State.Idle:
                break;
            case State.Attack:
                Attack();
                break;
            case State.Destroy:
                break;
            default:
                break;
        }
    }

    void Attack()
    {
        anim.Play("CanonFire");
        anim.speed = attackSpeed;

        if (rotatedPart != null)
        {
            rotatedPart.forward = (target.transform.position - transform.position);
            rotatedPart1.forward = (target.transform.position - transform.position);
            rotatedPart2.forward = (target.transform.position - transform.position);
        }
        //timer += Time.deltaTime;
        //if (timer >= attackSpeed)
        //{
        //    //anim.Play("Canon");
        //    timer = 0;
        //    target.GetDamageRpc(damage);
        //}
    }

    public void DealDamage()
    {
        if (!IsHost) return;
        if (target != null)
            target.GetDamageRpc(damage, ownerId);
    }


    [Rpc(SendTo.ClientsAndHost)]
    public void GetDamageRpc(int damage)
    {
        currHP -= damage;
        HpText.text = currHP + "/" + maxHP;
        hpAmount.fillAmount = (float)currHP / maxHP;
        hpAmount2.fillAmount = (float)currHP / maxHP;
        hpAmount3.fillAmount = (float)currHP / maxHP;
        if (currHP <= 0)
            Destroyed();
    }

    void Destroyed() 
    {
        NetworkManager.ConnectedClientsList[(int)ownerId].PlayerObject.
            GetComponent<PlayerNet>().defenceIsPlaced[0] = false;
        NetworkBehaviour.Destroy(gameObject);
    }



    public void Heal(int value)
    {
        currHP += value;
        HpText.text = currHP + "/" + maxHP;
        hpAmount.fillAmount = (float)currHP / maxHP;
        hpAmount2.fillAmount = (float)currHP / maxHP;
        hpAmount3.fillAmount = (float)currHP / maxHP;
    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 6 || target != null) return;

        target = other.GetComponent<Unit>();
        state = State.Attack;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != 6 || target != null) return;
        target = other.GetComponent<Unit>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Unit>() == target)
            target = null;
    }
}
