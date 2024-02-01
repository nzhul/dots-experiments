using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GraveyardMono : MonoBehaviour
{
    public float2 FieldDimentions;
    public int NumberOfThumbstones;
    public GameObject TombstonePrefab;
    public uint Seed;
    public GameObject ZombiePrefab;
    public float ZombieSpawnRate;
}

public class GraveyardBaker : Baker<GraveyardMono>
{
    public override void Bake(GraveyardMono authoring)
    {
        var graveyardEntity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(graveyardEntity, new GraveyardProperties
        {
            FieldDimentions = authoring.FieldDimentions,
            NumberOfThumbstones = authoring.NumberOfThumbstones,
            TombstonePrefab = GetEntity(authoring.TombstonePrefab, TransformUsageFlags.Dynamic),
            ZombiePrefab = GetEntity(authoring.ZombiePrefab, TransformUsageFlags.Dynamic),
            ZombieSpawnRate = authoring.ZombieSpawnRate
        });

        AddComponent(graveyardEntity, new GraveyardRandom
        {
            Value = Unity.Mathematics.Random.CreateFromIndex(authoring.Seed)
        });

        AddComponent<ZombieSpawnPoints>(graveyardEntity);
        AddComponent<ZombieSpawnTimer>(graveyardEntity);
    }
}