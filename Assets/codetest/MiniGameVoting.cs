using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MiniGameVote : MonoBehaviour
{
    public GameObject voteCanvas;
    public GameObject[] miniGameCanvases;
    public Button startVoteButton;
    public float voteTime = 5f;

    private int[] votes;
    private float timer;
    private bool voting = false;

    void Start()
    {
        startVoteButton.onClick.AddListener(StartVote);
        votes = new int[miniGameCanvases.Length];
    }

    void Update()
    {
        if (voting)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                EndVote();
            }

            foreach (Touch t in Input.touches)
            {
                if (t.phase == TouchPhase.Began)
                {
                    int player = GetTouchPlayer(t.position);
                    if (player != -1)
                    {
                        int gameChoice = GetMiniGameChoice(player);
                        votes[gameChoice]++;
                    }
                }
            }
        }
    }

    void StartVote()
    {
        voteCanvas.SetActive(true);
        foreach (var canvas in miniGameCanvases) canvas.SetActive(false);

        votes = new int[miniGameCanvases.Length];
        timer = voteTime;
        voting = true;
    }

    void EndVote()
    {
        voting = false;
        voteCanvas.SetActive(false);

        int maxVotesIndex = System.Array.IndexOf(votes, votes.Max());
        miniGameCanvases[maxVotesIndex].SetActive(true);
    }

    int GetTouchPlayer(Vector2 pos)
    {
        float w = Screen.width;
        float h = Screen.height;

        if (pos.x < w / 2 && pos.y > h / 2) return 0;
        if (pos.x > w / 2 && pos.y > h / 2) return 1;
        if (pos.x < w / 2 && pos.y < h / 2) return 2;
        if (pos.x > w / 2 && pos.y < h / 2) return 3;

        return -1;
    }

    int GetMiniGameChoice(int player)
    {
        // ตอนนี้ให้ผู้เล่น 1 และ 2 โหวตมินิเกม 1, ผู้เล่น 3 และ 4 โหวตมินิเกม 2 เป็นตัวอย่าง
        if (player == 0 || player == 1) return 0;
        else return 1;
    }
}
