using UnityEngine;

public class PlayerAutoMove : MonoBehaviour
{
    public float speed = 3f;

    void Update()
    {
        // เดินไปทางขวาอัตโนมัติ
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}
