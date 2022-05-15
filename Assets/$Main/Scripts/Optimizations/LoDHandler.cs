using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoDHandler : MonoBehaviour
{
    public List<GameObject> lodLevels = new List<GameObject>();
    int currentLodLevel;
    public int CurrentLodLevel
    {
        get { return currentLodLevel; }
        set
        {
            currentLodLevel = value;
            SetLodLevel();
        }
    }

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
    }
}
