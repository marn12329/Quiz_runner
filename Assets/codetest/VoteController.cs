using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class VoteController : MonoBehaviour
{
    public GameObject voteCanvas;          // หน้าจอโหวต
    public GameObject[] miniGameCanvases;  // มินิเกมทั้งหมด
    public Button startVoteButton;         // ปุ่มเริ่มโหวต
    public TextMeshProUGUI timerText;      // แสดงเวลาที่เหลือ
    public float voteTime = 5f;            // เวลาโหวต

    private int[] votes;
    private float timer;
    private bool voting = false;

    void Start()
    {
        startVoteButton.onClick.AddListener(StartVote);
        votes = new int[miniGameCanvases.Length];
        voteCanvas.SetActive(false);
        foreach (var c in miniGameCanvases) c.SetActive(false);
    }

    void Update()
    {
        if (voting)
        {
            timer -= Time.deltaTime;
            timerText.text = "เวลาที่เหลือ: " + Mathf.Ceil(timer).ToString();
            if (timer <= 0)
            {
                EndVote();
            }

            // รับโหวตจาก 4 ผู้เล่น
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

            // รองรับ Mouse click สำหรับทดสอบบน PC
            if (Input.GetMouseButtonDown(0))
            {
                int player = GetTouchPlayer(Input.mousePosition);
                if (player != -1)
                {
                    int gameChoice = GetMiniGameChoice(player);
                    votes[gameChoice]++;
                }
            }
        }
    }

    void StartVote()
    {
        voteCanvas.SetActive(true);
        foreach (var c in miniGameCanvases) c.SetActive(false);

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

        if (pos.x < w / 2 && pos.y > h / 2) return 0;  // ผู้เล่น 1
        if (pos.x > w / 2 && pos.y > h / 2) return 1;  // ผู้เล่น 2
        if (pos.x < w / 2 && pos.y < h / 2) return 2;  // ผู้เล่น 3
        if (pos.x > w / 2 && pos.y < h / 2) return 3;  // ผู้เล่น 4

        return -1;
    }

    int GetMiniGameChoice(int player)
    {
        // ผู้เล่น 1,2 → เกม 1 | ผู้เล่น 3,4 → เกม 2
        if (player == 0 || player == 1) return 0;
        else return 1;
    }
}
