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

            GameManager.instance.networkManager.AddPlayer(this);
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
                else if (scripts[i] is EventCamera) continue;
                else if (scripts[i] is CanvasScaler) continue;
                else if (scripts[i] is GraphicRaycaster) continue;

                Destroy(scripts[i]);
            }
        }
        else
        {
            GameManager.instance.networkManager.AddMyPlayer(this);
            GameManager.instance.networkManager.AddPlayer(this);
            if (role == ERole.Student)
            {
                GameManager.instance.uiManager.SetMutedText(true);
                photonVoiceView.RecorderInUse.TransmitEnabled = false;
            }
            else
            {
                GameManager.instance.uiManager.SetMutedText(false);
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

    }

    public void ToggleMic()
    {
        Debug.Log("Mic toggle");
        photonView.RPC("RPC_ToggleMic", RpcTarget.All, photonView.ViewID);
    }

    [PunRPC]
    public void RPC_SpeakerIcon(int photonViewId, bool enabled)
    {
        GameManager.instance.networkManager.allPlayers.Find(x => x.photonView.ViewID == photonViewId).speakingGO.SetActive(enabled);
    }


    [PunRPC]
    public void RPC_ToggleMic(int photonViewId)
    {

        NetworkPlayer np = GameManager.instance.networkManager.allPlayers.Find(x => x.photonView.ViewID == photonViewId);
        np.isMicUnmuted = !np.isMicUnmuted;
        if (np.photonView.IsMine)
        {
            np.photonVoiceView.RecorderInUse.TransmitEnabled = np.isMicUnmuted;
        }



        if (np.isMicUnmuted)
        {
            np.micIcon.sprite = unmuted;
            if (np.photonView.IsMine)
            {
                GameManager.instance.uiManager.SetMutedText(false);
            }
        }
        else
        {
            np.micIcon.sprite = muted;
            np.speakingGO.SetActive(false);
            if (np.photonView.IsMine)
            {
                GameManager.instance.uiManager.SetMutedText(true);
            }

        }



    }
}
