using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerSetup : MonoBehaviour
{
    public static PlayerSetup instance = null;

    public enum ERole
    {
        None,
        Professor,
        Student
    }

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

    public void SelectStudentRole()
    {
        role = ERole.Student;
        EnterGameScene();
    }

    public void SelectProfessorRole()
    {
        role = ERole.Professor;
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
