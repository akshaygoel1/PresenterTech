﻿using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NetworkPlayer : MonoBehaviourPunCallbacks
{
    public PhotonView photonView;
    public ERole role = ERole.None;
    public GameObject speakingGO;
    public PhotonVoiceView photonVoiceView;
    public AudioSource audioSource;
    private void Start()
    {
        if (!photonView.IsMine)
        {
            NetworkManager.instance.AddPlayer(this);
            MonoBehaviour[] scripts = GetComponentsInChildren<MonoBehaviour>();
            for (int i = 0; i < scripts.Length; i++)
            {
                if (scripts[i] is NetworkPlayer) continue;
                else if (scripts[i] is PhotonView) continue;
                else if (scripts[i] is PhotonTransformView) continue;
                else if (scripts[i] is PhotonVoiceView) continue;
                else if (scripts[i] is Speaker) continue;
                else if (scripts[i] is Button) continue;
                else if (scripts[i] is Image) continue;

                Destroy(scripts[i]);
            }
        }
        else
        {
            NetworkManager.instance.AddMyPlayer(this);
            NetworkManager.instance.AddPlayer(this);
            Camera.main.GetComponent<PlayerCam>().SetPlayer(this.transform);
        }

    }

    private void Update()
    {
        if (this.photonVoiceView.RecorderInUse != null && this.photonVoiceView.RecorderInUse.TransmitEnabled && photonView.IsMine)
        {
            if (this.photonVoiceView.IsRecording != speakingGO.activeInHierarchy)
                photonView.RPC("RPC_SpeakerIcon", RpcTarget.All, photonView.ViewID, this.photonVoiceView.IsRecording);

        }
    }

    [PunRPC]
    public void RPC_SpeakerIcon(int photonViewId, bool enabled)
    {
        NetworkManager.instance.otherPlayers.Find(x => x.photonView.ViewID == photonViewId).speakingGO.SetActive(enabled);
    }
}