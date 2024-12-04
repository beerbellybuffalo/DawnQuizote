using UnityEngine;

[ExecuteInEditMode]
public class ColliderOutline : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Collider collider = GetComponent<Collider>();
        if (collider)
        {
            Gizmos.color = Color.green;
            if (collider is BoxCollider box)
            {
                Gizmos.matrix = Matrix4x4.TRS(box.transform.position, box.transform.rotation, transform.lossyScale);
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (collider is SphereCollider sphere)
            {
                Gizmos.matrix = Matrix4x4.TRS(sphere.transform.position, sphere.transform.rotation, Vector3.one);
                // Account for lossyScale for accurate radius
                float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
                Gizmos.DrawWireSphere(sphere.center, sphere.radius * maxScale);
            }
            else if (collider is CapsuleCollider capsule)
            {
                Gizmos.matrix = Matrix4x4.TRS(capsule.transform.position, capsule.transform.rotation, Vector3.one);
                // Calculate the scaled radius and height
                float radius = capsule.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);
                float height = Mathf.Max(0, capsule.height * transform.lossyScale.y - 2 * radius);

                // Draw the capsule wireframe
                Vector3 up = transform.up * (height / 2);
                Gizmos.DrawWireSphere(capsule.center + up, radius);
                Gizmos.DrawWireSphere(capsule.center - up, radius);
            }
        }
    }
}
