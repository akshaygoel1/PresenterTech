using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "GameSettings", fileName = "New Game Settings")]
public class GameSettings : ScriptableObject
{
    public int maxPlayersUnmutedBeforeWarning = 7;

    public int maxPlayersBeforeLoDLevel1 = 5;
    public int maxPlayersBeforeLoDLevel2 = 10;

    public bool blockUnmutingAfterMaxUsers = false;
}
