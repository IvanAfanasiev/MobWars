using System.Net;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Target : NetworkBehaviour
{
    [SerializeField]
    int maxHP = 100;
    [SerializeField]
    public int currHP;

    [SerializeField]
    Image hpAmount;
    [SerializeField]
    Image hpAmount1;
    [SerializeField]
    TextMeshProUGUI HpText;
    [SerializeField]
    TextMeshProUGUI HpText1;

    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    bool isRightTeam;

    private void Start()
    {
        currHP = maxHP;
        HpText.text = currHP + "/" + maxHP;
        HpText1.text = HpText.text;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 6) return;
        other.GetComponent<Unit>().Finished();
        GetDamageRpc(other.GetComponent<Unit>().damage);
    }

    [Rpc(SendTo.Server)]
    public void GetDamageRpc(int damage)
    {
        currHP -= damage;

        if (currHP <= 0)
        {
            gameManager.GameOverRpc(isRightTeam);
            currHP = 0;
        }

        HpText.text = currHP + "/" + maxHP;
        HpText1.text = HpText.text;
        hpAmount.fillAmount = (float)currHP / maxHP;
        hpAmount1.fillAmount = hpAmount.fillAmount;
    }

    public void Heal(int value)
    {
        currHP += value;
        if (currHP > maxHP)
            currHP = maxHP;
        HpText.text = currHP + "/" + maxHP;
        HpText1.text = HpText.text;
        hpAmount.fillAmount = (float)currHP / maxHP;
        hpAmount1.fillAmount = hpAmount.fillAmount;


    }
}
