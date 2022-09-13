using System.Collections;
using Components;
using Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float restartDelay = 5;
    [SerializeField] private UIManager uiManager;
    
    private static GameManager _instance;

    public static GameManager Instance => _instance;
    
    public void EndGame(uint winnerId)
    {
        string winnerName = "";

        foreach (var player in FindObjectsOfType<PlayerController>())
        {
            if (player.NetId == winnerId)
            {
                winnerName = player.Nickname;
                break;
            }
        }
        
        uiManager.ShowWinnerMenu(winnerName);
        StartCoroutine(DelayRestart());
    }

    private void Restart()
    {
        uiManager.HideAllMenus();
        
        foreach(var player in FindObjectsOfType<PlayerController>())
            player.Spawn();
    }

    private void Awake()
    {
        _instance = this;
    }

    private IEnumerator DelayRestart()
    {
        yield return new WaitForSeconds(restartDelay);
        
        Restart();
    }
}
