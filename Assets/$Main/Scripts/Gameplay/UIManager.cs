using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI mutedText;
    public GameObject warningPopupForUnmutingMaxUsers;

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

    public void ShowWarningForUnmutingMaxUsers()
    {
        warningPopupForUnmutingMaxUsers.SetActive(true);
    }


}
