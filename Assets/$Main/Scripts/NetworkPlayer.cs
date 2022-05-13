using Photon.Pun;
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
    public GameObject speakingGO, muteButton;
    public PhotonVoiceView photonVoiceView;
    bool isMicUnmuted = false;
    public Sprite muted, unmuted;
    public Image micIcon;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            if (PlayerSetup.instance.GetRole() == ERole.Professor)
            {
                muteButton.SetActive(true);
            }

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
            if (role == ERole.Student)
            {
                photonVoiceView.RecorderInUse.TransmitEnabled = false;
            }
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

        if (Input.GetKeyDown(KeyCode.Space) && photonView.IsMine)
        {
            if (this.role != ERole.Professor)
            {
                ToggleMic();
            }
        }
    }

    public void ToggleMic()
    {
        Debug.Log("Mic toggle");
        photonView.RPC("RPC_ToggleMic", RpcTarget.All, photonView.ViewID);
    }

    [PunRPC]
    public void RPC_SpeakerIcon(int photonViewId, bool enabled)
    {
        NetworkManager.instance.otherPlayers.Find(x => x.photonView.ViewID == photonViewId).speakingGO.SetActive(enabled);
    }


    [PunRPC]
    public void RPC_ToggleMic(int photonViewId)
    {
        NetworkManager.instance.otherPlayers.Find(x => x.photonView.ViewID == photonViewId).photonVoiceView.RecorderInUse.TransmitEnabled = !isMicUnmuted;
        isMicUnmuted = !isMicUnmuted;
        if (isMicUnmuted)
        {
            micIcon.sprite = unmuted;
        }
        else
        {
            micIcon.sprite = muted;
        }

    }
}
