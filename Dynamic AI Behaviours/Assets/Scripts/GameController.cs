using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [System.NonSerialized]
    public static GameController Instance;

    [SerializeField]
    private Canvas backPropagateCanvas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PauseGame()
    {
        Time.timeScale = 1.0f - Time.timeScale;
        backPropagateCanvas.enabled = Time.timeScale == 0.0f;
    }
}
