using Photon.Pun;
using Photon.Voice.PUN;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.XR.CoreUtils;
public class NetworkPlayer : MonoBehaviourPunCallbacks
{
    public PhotonView photonView;
    public ERole role = ERole.None;
    public GameObject speakingGO, muteButton;
    public PhotonVoiceView photonVoiceView;
    bool isMicUnmuted = false;
    public Sprite muted, unmuted;
    public Image micIcon;
    public LoDHandler lodHandler;
    bool isHandRaised = false;
    public GameObject raiseHandIcon;
    public Image raiseHandImage;
    public Sprite raiseHandSprite, lowerHandSprite;
    public TextMeshProUGUI raiseHandText;

    public GameObject userHandleCanvas;
    public XROrigin xrOrigin;
    public GameObject cameraOffset;
    public GameObject locomotion;
    public GameObject xrInteractionManager;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            if (PlayerSetup.instance.GetRole() == ERole.Professor)
            {
                muteButton.SetActive(true);

            }

            if (role == ERole.Professor)
            {
                lodHandler.CurrentLodLevel = 0;
            }

            GameManager.instance.networkManager.AddPlayer(this);
            Destroy(userHandleCanvas);
            Destroy(xrOrigin);
            Destroy(cameraOffset);
            Destroy(locomotion);
            Destroy(xrInteractionManager);

            //MonoBehaviour[] scripts = GetComponentsInChildren<MonoBehaviour>();
            //for (int i = 0; i < scripts.Length; i++)
            //{
            //    if (scripts[i] is NetworkPlayer) continue;
            //    else if (scripts[i] is PhotonView) continue;
            //    else if (scripts[i] is PhotonTransformView) continue;
            //    else if (scripts[i] is PhotonVoiceView) continue;
            //    else if (scripts[i] is Speaker) continue;
            //    else if (scripts[i] is Button) continue;
            //    else if (scripts[i] is Image) continue;
            //    else if (scripts[i] is EventCamera) continue;
            //    else if (scripts[i] is CanvasScaler) continue;
            //    else if (scripts[i] is GraphicRaycaster) continue;
            //    else if (scripts[i] is LoDHandler) continue;

            //    Destroy(scripts[i]);
            //}
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
            lodHandler.CurrentLodLevel = 2;
            Camera.main.GetComponent<PlayerCam>().SetPlayer(this.transform);
            GameManager.instance.StartLoDOperations();
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

    #region Voice Functions

    public void ToggleMic()
    {
        if (GameManager.instance.gameSettings.blockUnmutingAfterMaxUsers && GameManager.instance.UnmutedCounter >= GameManager.instance.gameSettings.maxPlayersUnmutedBeforeWarning)
            return;

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


            GameManager.instance.UnmutedCounter += 1;

            if (GameManager.instance.UnmutedCounter >= GameManager.instance.gameSettings.maxPlayersUnmutedBeforeWarning)
            {
                GameManager.instance.uiManager.ShowWarningForUnmutingMaxUsers();
            }

            if (np.photonView.IsMine)
            {
                GameManager.instance.uiManager.SetMutedText(false);
                if (np.isHandRaised)
                {
                    np.ToggleRaiseHand();
                }
            }
        }
        else
        {
            np.micIcon.sprite = muted;
            np.speakingGO.SetActive(false);

            GameManager.instance.UnmutedCounter -= 1;


            if (np.photonView.IsMine)
            {
                GameManager.instance.uiManager.SetMutedText(true);

            }

        }
    }
    #endregion


    public void ToggleRaiseHand()
    {
        photonView.RPC("RPC_ToggleRaiseHand", RpcTarget.All, photonView.ViewID, !isHandRaised);
    }

    [PunRPC]
    public void RPC_ToggleRaiseHand(int photonViewID, bool enabled)
    {
        NetworkPlayer networkPlayer = GameManager.instance.networkManager.allPlayers.Find(x => x.photonView.ViewID == photonViewID);
        networkPlayer.raiseHandIcon.SetActive(enabled);
        networkPlayer.isHandRaised = enabled;
        if (networkPlayer.photonView.IsMine)
            networkPlayer.UpdateRaiseHandControls();
    }


    public void UpdateRaiseHandControls()
    {
        if (isHandRaised)
        {
            raiseHandImage.sprite = lowerHandSprite;
            raiseHandText.text = "Lower Hand";
        }
        else
        {
            raiseHandImage.sprite = raiseHandSprite;
            raiseHandText.text = "Raise Hand";
        }
    }

}
