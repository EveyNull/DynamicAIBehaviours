using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [System.NonSerialized]
    public static GameController Instance;
    public NeedData needData;
    public SourceData sourceData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
    }
}
