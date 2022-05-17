using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum ERole
{
    None = 0,
    Professor,
    Student
}
public class PlayerSetup : MonoBehaviour
{
    public static PlayerSetup instance = null;

    ERole role = ERole.None;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// Button Press selects the role of the player
    /// </summary>
    /// <param name="roleID">0 -> None, 1 -> Professor, 2 -> Student </param>
    public void SelectRole(int roleID)
    {
        role = (ERole)roleID;
        EnterGameScene();
    }


    public void EnterGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    public ERole GetRole()
    {
        return role;
    }
}
