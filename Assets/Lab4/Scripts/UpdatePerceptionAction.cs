using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.UIElements;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Update Perception", story: "updates perception stuff", category: "Action/Sensing", id: "2261dc07e47fef25822b10083e6301c1")]
public partial class UpdatePerceptionAction : Action
{

    protected override Status OnStart()
    {
        return Status.Running;
    }
    

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

