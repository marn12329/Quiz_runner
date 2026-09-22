using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FloatingArea : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider2D>().size);
    }
}
