using UnityEngine;
using System.Collections;

public class SeekerTarget : MonoBehaviour
{
    private void Start()
    {

    }

    private void OnDrawGizmosSelected()
    {
        if (!enabled)
            return;

        Gizmos.color = ColorUtils.CreateWithAlpha(Color.blue, 0.5f);

        Vector3 pos = transform.position;
        pos.y += 0.5f;
        Gizmos.DrawSphere(pos, 0.3f);
    }
}
