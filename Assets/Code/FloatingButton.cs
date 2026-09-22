using UnityEngine;

public class FloatingButton : MonoBehaviour
{
    public float moveSpeed = 50f;          // ความเร็วในการบิน
    public Vector2 moveDirection = Vector2.right; // ทิศทางการบินเริ่มต้น
    public bool randomDirection = true;    // สุ่มทิศทางตอนเริ่ม
    public bool bounceOnCollision = true;  // เด้งเมื่อชน
    public RectTransform boundaryArea;     // ขอบเขตพื้นที่บิน (เช่น Canvas หรือ Panel)

    private RectTransform rect;
    private Vector2 currentDir;

    void Start()
    {
        rect = GetComponent<RectTransform>();

        if (randomDirection)
        {
            currentDir = Random.insideUnitCircle.normalized; // ทิศทางสุ่ม
        }
        else
        {
            currentDir = moveDirection.normalized;
        }
    }

    void Update()
    {
        if (boundaryArea == null) return;

        // เคลื่อนที่ตลอดเวลา
        rect.anchoredPosition += currentDir * moveSpeed * Time.deltaTime;

        // ตรวจขอบเขต
        Vector2 pos = rect.anchoredPosition;
        Vector2 size = boundaryArea.rect.size;
        float halfWidth = size.x / 2f;
        float halfHeight = size.y / 2f;

        // เด้งเมื่อชนขอบ
        if (pos.x > halfWidth || pos.x < -halfWidth)
        {
            currentDir.x = -currentDir.x;
            pos.x = Mathf.Clamp(pos.x, -halfWidth, halfWidth);
        }

        if (pos.y > halfHeight || pos.y < -halfHeight)
        {
            currentDir.y = -currentDir.y;
            pos.y = Mathf.Clamp(pos.y, -halfHeight, halfHeight);
        }

        rect.anchoredPosition = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!bounceOnCollision) return;

        // ถ้าชนปุ่มอื่น — เด้งกลับ
        if (other.CompareTag("AnswerButton"))
        {
            currentDir = -currentDir;
        }
    }
}
