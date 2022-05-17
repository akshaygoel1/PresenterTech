using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class LoDHandler : MonoBehaviour
{
    public List<GameObject> lodLevels = new List<GameObject>();
    int currentLodLevel;
    public Transform head, leftHand, rightHand;
    public PhotonView photonView;

    public PhotonTransformView leftHandPTV, rightHandPTV;

    /// <summary>
    /// CurrentLodLevel property can be used to get or set the LoD level. If the LoD level is set, it calls the SetLodLevel() function
    /// </summary>
    public int CurrentLodLevel
    {
        get { return currentLodLevel; }
        set
        {
            currentLodLevel = value;
            SetLodLevel();
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            head.gameObject.SetActive(false);
            leftHand.gameObject.SetActive(false);
            rightHand.gameObject.SetActive(false);
            MapPosition(head, XRNode.Head);
            MapPosition(leftHand, XRNode.LeftHand);
            MapPosition(rightHand, XRNode.RightHand);
        }
    }

    /// <summary>
    /// Maps the position from the device to the target
    /// </summary>
    /// <param name="target">The transform that needs to be affected</param>
    /// <param name="node">The XR node from the device</param>
    void MapPosition(Transform target, XRNode node)
    {
        InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos);
        InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rot);

        target.position = pos;
        target.rotation = rot;
    }

    /// <summary>
    /// Enable and disable the objects which need to be rendered based on the LoD level
    /// </summary>
    void SetLodLevel()
    {
        for (int i = 0; i < lodLevels.Count; i++)
        {
            if (i != currentLodLevel)
            {
                if (lodLevels[i].activeSelf)
                    lodLevels[i].SetActive(false);
            }
            else
            {
                lodLevels[i].SetActive(true);
            }
        }

        //Hide the hands if the user is not close to the player to save on the processing
        if (currentLodLevel > 0 && !photonView.IsMine)
        {
            leftHand.gameObject.SetActive(false);
            rightHand.gameObject.SetActive(false);
            photonView.ObservedComponents.Remove(leftHandPTV);
            photonView.ObservedComponents.Remove(rightHandPTV);
            leftHandPTV.enabled = false;
            rightHandPTV.enabled = false;
        }
        else if (currentLodLevel == 0 && !photonView.IsMine)
        {
            leftHand.gameObject.SetActive(true);
            rightHand.gameObject.SetActive(true);
            leftHandPTV.enabled = true;
            rightHandPTV.enabled = true;
            photonView.ObservedComponents.Add(leftHandPTV);
            photonView.ObservedComponents.Add(rightHandPTV);

        }
    }
}
