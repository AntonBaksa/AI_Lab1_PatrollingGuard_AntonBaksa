using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SteeringAgent : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 5f;
    public float maxForce = 10f; // Limit how "fast" we can change direction(turning radius)

    [Header("Arrive")]
    public float slowingRadius = 3f;

    [Header("Separation")]
    public float separationRadius = 1.5f;
    public float separationStrength = 5f;
    
    [Header("Weights")]
    public float arriveWeight = 1f;
    public float separationWeight = 1f;
    
    [Header("Debug")]
    public bool drawDebug = true;

    private Vector3 velocity = Vector3.zero;
    
    // Optional target for Seek / Arrive
    public GameObject target;
    
    // Static list so agents can find each other
    public static List<SteeringAgent> allAgents = new List<SteeringAgent>();

    private void OnEnable()
    {
        allAgents.Add(this);
    }
    private void OnDisable()
    {
        allAgents.Remove(this);
    }

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Target");
    }

    void Update()
    {
        // Calculate Steering Force
        Vector3 totalSteering = Vector3.zero;
        if (target != null)
        {
            totalSteering += Arrive(target.transform.position, slowingRadius) * arriveWeight;
        }

        if(allAgents.Count > 1)
        {
            totalSteering += Separation(separationRadius, separationStrength) * separationWeight;
        }

        totalSteering = Vector3.ClampMagnitude(totalSteering, maxForce);
        velocity += totalSteering * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        transform.position += velocity * Time.deltaTime;

        // Face Movement Direction
        if (velocity.sqrMagnitude > 0.0001f)
        {
            transform.forward = velocity.normalized;
        }



    }

    // -- BEHAVIOUR STUBS --
    public Vector3 Seek(Vector3 targetPos) 
    {
        Vector3 toTarget = targetPos - transform.position;

        if(toTarget.sqrMagnitude < 0.001f)
        {
            return Vector3.zero;
        }

        Vector3 desired = toTarget.normalized * maxSpeed;

        return desired - velocity;  
    }

    public Vector3 Arrive(Vector3 targetPos, float slowingRadius)
    {
        Vector3 toTarget = targetPos - transform.position;
        float dist = toTarget.magnitude;

        if (dist < 0.001f)
        {
            return Vector3.zero;
        }

        float desiredSpeed = maxSpeed;

        if(dist < slowingRadius)
        {
            desiredSpeed = maxSpeed * (dist / slowingRadius);
        }

        Vector3 desired = toTarget.normalized * desiredSpeed;

        return desired - velocity;
    }

    public Vector3 Separation(float separationRadius, float separationStrength)
    {
        Vector3 force = Vector3.zero;
        int neighbourCount = 0;

        foreach(SteeringAgent other in allAgents)
        {
            if (other == this) continue;
            Vector3 toMe = transform.position - other.transform.position;
            float dist = toMe.magnitude;

            if (dist > 0f && dist < separationRadius)
            {
                force += toMe.normalized / dist;
                neighbourCount++;
            }
        }
        if (neighbourCount > 0)
        {
            force/= neighbourCount;
            force = force.normalized * maxSpeed;
            force = force - velocity;
            force *= separationStrength;
        }
        return force;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawDebug) return;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + velocity);
    }
}
