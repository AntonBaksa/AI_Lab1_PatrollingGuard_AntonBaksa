using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Status = Unity.Behavior.Node.Status;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Clear Target", story: "Forgets [Target] and reset flags", category: "Action/Sensing", id: "c7c9aa224f73eb0ee3e006cf2ec27b21")]
public partial class ClearTarget : Action
{
    [SerializeReference]
    public BlackboardVariable<GameObject> Target;

    [SerializeReference]
    public BlackboardVariable<bool> HasLineOfSight;

    [SerializeReference]
    public BlackboardVariable<float> TimeSinceLastSeen;

    protected override Status OnUpdate()
    {
        if (Target != null) Target.Value = null;
        if (HasLineOfSight != null) HasLineOfSight.Value = false;

        if (TimeSinceLastSeen != null) TimeSinceLastSeen.Value = 9999f;

        return Status.Success;
    }
}

