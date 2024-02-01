using Unity.Entities;
using UnityEngine;

public class BrainMono : MonoBehaviour
{
    public float Health;
}

public class BrainBaker : Baker<BrainMono>
{
    public override void Bake(BrainMono authoring)
    {
        var brainEntity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<BrainTag>(brainEntity);
        AddComponent(brainEntity, new BrainHealth
        {
            Value = authoring.Health,
            Max = authoring.Health
        });
        AddBuffer<BrainDamageBufferElement>(brainEntity);
    }
}