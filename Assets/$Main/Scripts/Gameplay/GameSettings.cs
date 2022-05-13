using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "GameSettings", fileName = "New Game Settings")]
public class GameSettings : ScriptableObject
{
    public int maxPlayersUnmutedBeforeWarning = 7;

    public int maxPlayersBeforeLoD = 5;
}
