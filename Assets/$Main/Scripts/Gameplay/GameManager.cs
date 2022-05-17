using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameSettings gameSettings;
    public UIManager uiManager;
    public NetworkManager networkManager;
    public GameObject xrOrigin;
    int unmutedCounter = 1;

    public int UnmutedCounter
    {
        get { return unmutedCounter; }
        set
        {

            unmutedCounter = value;
            if (unmutedCounter >= gameSettings.maxPlayersUnmutedBeforeWarning)
            {
                uiManager.ShowWarningForUnmutingMaxUsers(); //Show Warning Popup if max number of users are unmuted
            }
        }
    }
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    /// <summary>
    /// Level of Details Operation
    /// </summary>
    public void StartLoDOperations()
    {
        Debug.Log("Starting LoD operations");
        StartCoroutine(LoDOperations());
    }

    /// <summary>
    /// This function checks the distance from all the other players and sorts them in the list and renders them based on distance and various other parameters
    /// </summary>
    /// <returns></returns>
    IEnumerator LoDOperations()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            networkManager.allPlayers.RemoveAll(x => x == null); //Remove all players that have left the scene
            networkManager.allPlayers = networkManager.allPlayers.OrderBy(x => Vector3.SqrMagnitude(networkManager.GetClientPlayer().transform.position - x.transform.position)).ToList(); //Sort players based on distance
            int professorCount = 0;
            int myCounter = 0;
            for (int i = 0; i < networkManager.allPlayers.Count; i++)
            {
                if (networkManager.allPlayers[i] != networkManager.GetClientPlayer() && networkManager.allPlayers[i].role != ERole.Professor)
                {
                    if (i <= gameSettings.maxPlayersBeforeLoDLevel1 + professorCount - myCounter)
                    {
                        networkManager.allPlayers[i].lodHandler.CurrentLodLevel = 0;
                    }
                    else if (i <= gameSettings.maxPlayersBeforeLoDLevel2 + professorCount - myCounter)
                    {

                        networkManager.allPlayers[i].lodHandler.CurrentLodLevel = 1;
                    }
                    else
                    {
                        networkManager.allPlayers[i].lodHandler.CurrentLodLevel = 2;
                    }

                }
                else if (networkManager.allPlayers[i] == networkManager.GetClientPlayer())
                {
                    myCounter++;
                }
                else if (networkManager.allPlayers[i].role == ERole.Professor)
                {
                    if (networkManager.allPlayers[i] != networkManager.GetClientPlayer())
                        professorCount++;
                }
            }
        }
    }
}
