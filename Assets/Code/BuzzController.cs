using UnityEngine;
using System;

public class BuzzController : MonoBehaviour
{
    public GameObject buzzPanel;
    public KeyCode[] playerKeys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R };

    private bool isBuzzActive = false;
    private float buzzEndTime;
    private int totalPlayers;

    public event Action<int> OnPlayerSelected;

    public void StartBuzz(int playerCount, float timeLimit)
    {
        totalPlayers = playerCount;
        buzzPanel.SetActive(true);
        isBuzzActive = true;
        buzzEndTime = Time.time + timeLimit;
    }

    void Update()
    {
        if (!isBuzzActive) return;

        for (int i = 0; i < totalPlayers && i < playerKeys.Length; i++)
        {
            if (Input.GetKeyDown(playerKeys[i]))
            {
                SelectPlayer(i);
                return;
            }
        }

        if (Time.time >= buzzEndTime)
        {
            int randomPlayer = UnityEngine.Random.Range(0, totalPlayers);
            SelectPlayer(randomPlayer);
        }
    }

    void SelectPlayer(int playerIndex)
    {
        isBuzzActive = false;
        buzzPanel.SetActive(false);
        OnPlayerSelected?.Invoke(playerIndex);
    }
}
