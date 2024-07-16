using System;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNet : NetworkBehaviour
{
    [SerializeField]
    NetworkVariable<Color> color = new NetworkVariable<Color>(Color.white,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
        );
    public NetworkVariable<bool> isLeftTeam = new NetworkVariable<bool>(true,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
        );

    //public bool isLeftTeam;

    [SerializeField] Transform defenceBtnsWrapper;
    [SerializeField] Transform unitsBtnsWrapper;
    [SerializeField] EnemySpawner enemySpawner;
    [SerializeField] DefenceSpawner defenceSpawner;
    [SerializeField] GameObject cam;
    [SerializeField] GameObject canvas;
    [SerializeField] string message;
    [SerializeField] int money = 0;
    [SerializeField] int income = 1;
    [SerializeField]
    RoundManager roundManager;
    [SerializeField]
    PlayerAttack playerAttack;

    [SerializeField]
    TextMeshProUGUI incomeText;
    [SerializeField]
    TextMeshProUGUI moneyText;

    [SerializeField]
    GameObject deafeatScreen;
    [SerializeField]
    GameObject winScreen;

    public bool[] defenceIsPlaced = new bool[2];

    bool canPlay = false;

    [SerializeField]
    Button cooldown;

    private void Start()
    {
        roundManager = FindAnyObjectByType<RoundManager>();
        enemySpawner = FindAnyObjectByType<EnemySpawner>();
        defenceSpawner = FindAnyObjectByType<DefenceSpawner>();
        if (!IsLocalPlayer)
        {
            cam.SetActive(false);
            canvas.SetActive(false);
            return;
        }
        roundManager.OnCanBePlayedChanged += ChangeCanPlay;
        roundManager.OnRoundEnded += RoundIncome;

        GetMoneyRpc(10, isLeftTeam.Value);
        GetIncomeRpc(1);
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.M))
        {
            SendMessageRPC(message);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SendUnit(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SendUnit(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SendUnit(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            SendUnit(3);
        if (Input.GetKeyDown(KeyCode.Z))
            PlaceNewDefence(0);
        else if (Input.GetKeyDown(KeyCode.X))
            PlaceNewDefence(1);
    }
    void ChangeCanPlay(bool canBe)
    {
        canPlay = canBe;
        playerAttack.canAttack = canBe;
        UpdateUnitBtns(canBe);
        UpdateDefenceBtns(canBe);
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;

        //NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

    }
    public void UpdateUnitBtns(bool canInteractWith)
    {
        if (canInteractWith)
            for (int i=0; i < unitsBtnsWrapper.childCount; i++)
                unitsBtnsWrapper.GetChild(i).GetComponent<Button>().
                    interactable = enemySpawner.UnitCost(i) <= money;
        else
            for (int i = 0; i < unitsBtnsWrapper.childCount; i++)
                unitsBtnsWrapper.GetChild(i).GetComponent<Button>().
                    interactable = false;
    }
    public void UpdateDefenceBtns(bool canInteractWith)
    {
        if (canInteractWith)
            for (int i = 0; i < defenceBtnsWrapper.childCount; i++)
                defenceBtnsWrapper.GetChild(i).GetComponent<Button>().
                    interactable = !(defenceIsPlaced[i]) && defenceSpawner.DefenceCost(i) <= money;
        else
            for (int i = 0; i < defenceBtnsWrapper.childCount; i++)
                defenceBtnsWrapper.GetChild(i).GetComponent<Button>().
                    interactable = false;
    }

    [Rpc(SendTo.ClientsAndHost)]
    void SendMessageRPC(string msg)
    {
        Debug.Log(msg);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetupRPC(Vector3 position, bool isLeftTeam)
    {
        transform.position = position;
        print("setup "+ gameObject.name);
        if (IsOwner)
        {
            this.isLeftTeam.Value = isLeftTeam;
            color.Value = FindObjectOfType<ColorChange>().color;
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void GetMoneyRpc(int quantity, bool isInLeftTeam)
    {
        if (isLeftTeam.Value != isInLeftTeam) return;
        money += quantity;
            
        UpdateUnitBtns(true);
        UpdateDefenceBtns(true);
        moneyText.text = "money: "+money;
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void SpendMoneyRpc(int quantity)
    {
        money -= quantity;

        UpdateUnitBtns(true);
        UpdateDefenceBtns(true);
        moneyText.text = "money: "+money;
        incomeText.text = "income: " + income;
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void GetIncomeRpc(int quantity)
    {
        this.income += quantity;
        incomeText.text = "income: " + income;
    }
    void RoundIncome()
    {
        GetMoneyRpc(this.income, this.isLeftTeam.Value);
    }



    public void SendUnit(int indx)
    {
        if (!canPlay) return;
        int cost = enemySpawner.UnitCost(indx);
        int newIncome = enemySpawner.UnitIncome(indx);

        if (money < cost) return;
        SpendMoneyRpc(cost);
        GetIncomeRpc(newIncome);

        if (isLeftTeam.Value)
            enemySpawner.NewUnit(indx, isLeftTeam.Value, OwnerClientId, color.Value);
        else
            enemySpawner.NewUnit(indx, isLeftTeam.Value, OwnerClientId, color.Value);

    }
    public void PlaceNewDefence(int indx)
    {
        if (!canPlay) return;
        int cost = defenceSpawner.DefenceCost(indx);


        if (money < cost || defenceIsPlaced[indx]) return;
        SpendMoneyRpc(cost);

        if (isLeftTeam.Value)
            defenceSpawner.NewDefence(indx, isLeftTeam.Value, OwnerClientId);
        else
            defenceSpawner.NewDefence(indx, isLeftTeam.Value, OwnerClientId);

        defenceIsPlaced[indx] = true;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ReduceAttackCooldownRpc()
    {
        if (money < 20) return;

        cooldown.interactable = false;
        SpendMoneyRpc(20);
        playerAttack.attackSpeed -= 0.5f;
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }

    public void WinLabel()
    {
        winScreen.SetActive(true);
        //BG - disconect
    }
    public void DeafeatLabel()
    {
        deafeatScreen.SetActive(true);
        //BG - disconect
    }


































}
