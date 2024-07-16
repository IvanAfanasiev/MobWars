using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class GameStart : MonoBehaviour
{
    public UnityEngine.UI.Button hostBtn;
    public UnityEngine.UI.Button clientBtn;
    public UnityEngine.UI.Button serverBtn;

    private void Awake()
    {
        serverBtn.onClick.AddListener(() => { NetworkManager.Singleton.StartServer(); });
        hostBtn.onClick.AddListener(() => { NetworkManager.Singleton.StartHost(); });
        clientBtn.onClick.AddListener(() => { NetworkManager.Singleton.StartClient(); });
    }
}
