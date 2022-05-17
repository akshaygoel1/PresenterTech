using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI mutedText;
    public GameObject warningPopupForUnmutingMaxUsers;

    /// <summary>
    /// Set the muted text on the HUD
    /// </summary>
    /// <param name="isMuted"></param>
    public void SetMutedText(bool isMuted)
    {
        if (isMuted)
        {
            mutedText.text = "You are muted!";
        }
        else
        {
            mutedText.text = "You are unmuted! You are audible to others.";
        }
    }

    /// <summary>
    /// Enable the warning popup
    /// </summary>
    public void ShowWarningForUnmutingMaxUsers()
    {
        warningPopupForUnmutingMaxUsers.SetActive(true);
    }


}
