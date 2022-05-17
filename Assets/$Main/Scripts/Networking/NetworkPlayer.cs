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
        if (!photonView.IsMine) //If this network player client is not mine
        {
            if (PlayerSetup.instance.GetRole() == ERole.Professor) //If my player is a professor
            {
                muteButton.SetActive(true); //Enable the mute button on the other user
            }

            if (role == ERole.Professor) //If this network player client's role is of a professor
            {
                lodHandler.CurrentLodLevel = 0; //Set them to LOD Level 0, which renders them in the highest possible quality
            }

            GameManager.instance.networkManager.AddPlayer(this); // Add the player to the list

            //Destroy all components/objects which we don't want to access on the other user
            Destroy(userHandleCanvas);
            Destroy(xrOrigin);
            Destroy(cameraOffset);
            Destroy(locomotion);
            Destroy(xrInteractionManager);
        }
        else
        {
            GameManager.instance.networkManager.AddMyPlayer(this);
            GameManager.instance.networkManager.AddPlayer(this);
            if (role == ERole.Student) //If this client's role is of a student
            {
                GameManager.instance.uiManager.SetMutedText(true); //Set them as muted (default)
                photonVoiceView.RecorderInUse.TransmitEnabled = false;
            }
            else
            {
                GameManager.instance.uiManager.SetMutedText(false);//Set them as unmuted (default)
            }
            lodHandler.CurrentLodLevel = 0;
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
    /// <summary>
    /// Toggle the mic on this client user
    /// </summary>
    public void ToggleMic()
    {
        if (GameManager.instance.gameSettings.blockUnmutingAfterMaxUsers && GameManager.instance.UnmutedCounter >= GameManager.instance.gameSettings.maxPlayersUnmutedBeforeWarning && !isMicUnmuted)
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

    #region Raise Hand Functionality
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

    #endregion

}
