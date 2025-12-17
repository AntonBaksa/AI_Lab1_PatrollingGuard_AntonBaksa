using UnityEngine;

public class GuardSensor : MonoBehaviour
{
    public Transform player;
    public float viewRange = 10;
    public LayerMask occluders = ~0;
    public bool useLineOfSightRaycast = true;

    public bool SeesPlayer { get; private set; }

    // Update is called once per frame
    void Update()
    {
        SeesPlayer = false;
        if (player != null) return;

        if (!useLineOfSightRaycast)
        {
            SeesPlayer = true;
            return;
        }

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 target = player.position + Vector3.up * 0.5f;
        Vector3 dir = (target - origin);
        float len = dir.magnitude;

        if (len < 0.001f) 
        {
            SeesPlayer = true; return;
        }

        if (Physics.Raycast(origin, dir / len, out RaycastHit hit, len, occluders))
        {
            if (hit.transform == player) SeesPlayer = true;
        }
    }
}

