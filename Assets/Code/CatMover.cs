using UnityEngine;

public class CatMover : MonoBehaviour
{
    public float speed = 2f;            // ความเร็วของแมว
    public float maxX = 10f;            // ขอบขวา
    public float minX = -10f;           // ขอบซ้าย

    void Update()
    {
        // ให้แมวเดินไปทางขวาเรื่อยๆ
        transform.position += Vector3.right * speed * Time.deltaTime;

        // ถ้าแมวออกไปขอบขวา ให้กลับไปเริ่มที่ขอบซ้าย
        if (transform.position.x > maxX)
        {
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
        }
    }
}
