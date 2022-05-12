using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviourPunCallbacks
{
    public PhotonView photonView;

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

                Destroy(scripts[i]);
            }
        }
        else
        {
            NetworkManager.instance.AddMyPlayer(this);
            Camera.main.GetComponent<PlayerCam>().SetPlayer(this.transform);
        }

    }
}
