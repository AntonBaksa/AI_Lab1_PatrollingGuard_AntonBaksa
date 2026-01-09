using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Status = Unity.Behavior.Node.Status;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Update Perception", story: "Updates Perception stuff", category: "Action/Sensing", id: "0cb7492c3775a22f5aa8626527bb3de8")]
public partial class UpdatePerception : Action
{
    [SerializeReference]
    public BlackboardVariable<GameObject> Target;

    [SerializeReference]
    public BlackboardVariable<bool> HasLineOfSight;

    [SerializeReference]
    public BlackboardVariable<Vector3> LastKnownPosition;

    [SerializeReference]
    public BlackboardVariable<float> TimeSinceLastSeen;
    
    protected override Status OnStart()
    {
        // Ensure we have sane defaults.
        if (TimeSinceLastSeen != null && TimeSinceLastSeen.Value < 0f) TimeSinceLastSeen.Value = 9999f;
           
        return Status.Running;
    }
    protected override Status OnUpdate()
    {
        var sensors = GameObject != null ? GameObject.GetComponent<GuardSensors>() : null;

        if (sensors == null)
        {
            // No sensors attached -> treat as "can't see anything"
            
            if (HasLineOfSight != null) HasLineOfSight.Value = false;

            if (TimeSinceLastSeen != null) TimeSinceLastSeen.Value += Time.deltaTime;

            return Status.Success;
        }
            
        bool sensed = sensors.TrySenseTarget(out GameObject sensedTarget,out Vector3 sensedPos,out bool hasLOS);
           
        if (sensed && hasLOS)
        {
            if (Target != null) Target.Value = sensedTarget;
            if (HasLineOfSight != null) HasLineOfSight.Value = true;
            
            if (LastKnownPosition != null) LastKnownPosition.Value = sensedPos;

            if (TimeSinceLastSeen != null) TimeSinceLastSeen.Value = 0f;
        }
        else
        {
            // Keep Target as-is (we "remember" who we were chasing),

            // but mark that we don't currently have LOS.
            if (HasLineOfSight != null) HasLineOfSight.Value = false;

            if (TimeSinceLastSeen != null) TimeSinceLastSeen.Value += Time.deltaTime;

        }
        
        // This node is a fast "service-like" update; finishes immediately each tick.

        return Status.Success;
    }
}

