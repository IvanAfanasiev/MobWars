using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class RoundManager : NetworkBehaviour
{
    [SerializeField]
    int maxTime = 10;
    [SerializeField]
    float timer;

    [SerializeField]
    float factor = 1.2f;
    [SerializeField]
    TextMeshProUGUI timerText;

    [SerializeField]
    Transform leftUnits;
    [SerializeField]
    Transform rightUnits;

    public bool canBePlayed = false;

    public delegate void canBePlayedChanged(bool canBe);
    public delegate void RoundEnded();
    public event canBePlayedChanged OnCanBePlayedChanged;
    public event RoundEnded OnRoundEnded;

    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if (!canBePlayed) return;
        CountTimerRpc();
    }
    void CountTimerRpc()
    {
        timer -= Time.deltaTime;

        timerText.text = "Time left: " + (int)(timer / 60) + ":" + (((int)timer) % 60).ToString("0#");// ((int)timer).ToString();
        if (timer <= 0)
            EndRoundRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void StartRoundRpc()
    {
        canBePlayed = true;
        OnCanBePlayedChanged.Invoke(canBePlayed);
    }
    [Rpc(SendTo.ClientsAndHost)]
    void EndRoundRpc()
    {
        canBePlayed = false;
        OnCanBePlayedChanged.Invoke(canBePlayed);
        DestroyAllUnitsRpc();
        OnRoundEnded.Invoke();
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    public void ReloadRoundRpc()
    {
        maxTime = (int)(maxTime * factor);
        timer = maxTime;
        timerText.text = "Time left: " + (int)(timer / 60) + ":" + ((int)timer) % 60;
        anim.Play("StartRound");
    }
    [Rpc(SendTo.Server)]
    void DestroyAllUnitsRpc()
    {
        for (int i = leftUnits.childCount - 1; i >= 0; i--)
            leftUnits.GetChild(i).GetComponent<Unit>().Finished();
        for (int i = rightUnits.childCount - 1; i >= 0; i--)
            rightUnits.GetChild(i).GetComponent<Unit>().Finished();
        anim.Play("RestartRound");
    }
}
