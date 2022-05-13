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
            Camera.main.GetComponent<PlayerCam>().SetPlayer(this.transform);
        }

    }

    private void Update()
    {
        speakingGO.SetActive(this.photonVoiceView.IsRecording);
    }
}
