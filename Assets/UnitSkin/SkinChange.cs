using UnityEngine;
using System.Runtime.ConstrainedExecution;
using UnityEditor;
using System;
using Unity.VisualScripting;
using Unity.Mathematics;
using System.Linq;
using Unity.Netcode;

public class SkinChange : NetworkBehaviour
{
    //[SerializeField]
    //PLayerGlobalSettings PlayerSet;
    //public Material bodyMat;
    //public GameObject body;
    //[SyncVar]
    //public Color clr = new Color();


    //void Start()
    //{
    //    //bodyMat = PlayerSet.GetMaterial();

    //    if (!isLocalPlayer) return;
    //    //bodyMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
    //    print(gameObject.name);
    //    SetSelectedButton();
    //}

    //[Client]
    //void ChangeIndx(Color newClr)
    //{
    //    bodyMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
    //    bodyMat.name = gameObject.name;
    //    bodyMat.color = clr;
    //    PlayerSet.materials.Add(bodyMat);
    //    bodyMat = PlayerSet.materials.Last();
    //    //bodyMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

    //    //bodyMat = PlayerSet.GetMaterial();

    //    //bodyMat.color = PlayerSet.GetColor();
    //    //bodyMat.SetColor("_Color", PlayerSet.GetColor());

    //    clr = PlayerSet.GetColor();// bodyMat.color;
    //    body.GetComponent<MeshRenderer>().material = bodyMat;
    //}
    //[Command]
    //private void SetSelectedButton()
    //{
    //    print("SetMaterial " + gameObject.name);
    //    print("color " + PlayerSet.GetColor());
    //    ChangeIndx(clr);
    //    RpcChangeIndx(clr);
    //}
    //[ClientRpc]
    //private void RpcChangeIndx(Color newClr)
    //{
    //    //bodyMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
    //    bodyMat = PlayerSet.GetMaterial();
    //    //bodyMat.color = PlayerSet.GetColor();
    //    //bodyMat.SetColor("_Color", PlayerSet.GetColor());

    //    clr = PlayerSet.GetColor();// bodyMat.color;
    //    body.GetComponent<MeshRenderer>().material = bodyMat;
    //}



    //indx - номер префаба в списке заранее подготовленных. можно сохранять через плеерПреф. например индекс скина в магазине.
    //[Command]
    //void SendMaterial(int indx)
    //{
    //    print("SendMaterial " + gameObject.name);
    //    GetMaterial(indx);
    //}

    //[ClientRpc]
    //void GetMaterial(int index)
    //{
    //    print("GetMaterial " + gameObject.name);
    //    body = PlayerSet.GetMaterial();
    //}

    //[Client]
    //void SendMatLocal()
    //{
    //    print("SendMatLocal " + gameObject.name);
    //    if (!isLocalPlayer) return;
    //    print("SendMatLocal2 " + gameObject.name);
    //    SendMaterial(0);
    //}
}
