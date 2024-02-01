using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct GraveyardAspect : IAspect
{
    private const float BRAIN_SAFETY_RADIUS_SQ = 100f;

    public readonly Entity Entity;

    private readonly RefRO<LocalTransform> _transformAspect;

    private readonly RefRO<GraveyardProperties> _graveyardProperties;

    private readonly RefRW<GraveyardRandom> _graveyardRandom;

    private readonly RefRW<ZombieSpawnPoints> _zombieSpawnPoints;

    private readonly RefRW<ZombieSpawnTimer> _zombieSpawnTimer;

    private float3 MinCorner => _transformAspect.ValueRO.Position - HalfDimensions;
    private float3 MaxCorner => _transformAspect.ValueRO.Position + HalfDimensions;

    private float3 HalfDimensions => new()
    {
        x = _graveyardProperties.ValueRO.FieldDimentions.x * 0.5f,
        y = 0f,
        z = _graveyardProperties.ValueRO.FieldDimentions.y * 0.5f
    };

    public int NumberOfThumbstones => _graveyardProperties.ValueRO.NumberOfThumbstones;

    public Entity TombstonePrefab => _graveyardProperties.ValueRO.TombstonePrefab;

    public bool ZombieSpawnPointsInitialized()
    {
        return _zombieSpawnPoints.ValueRO.Value.IsCreated && ZombieSpawnPointCount > 0;
    }

    private int ZombieSpawnPointCount => _zombieSpawnPoints.ValueRO.Value.Value.Value.Length;

    public LocalTransform GetRandomThumbstoneTransform()
    {
        return new LocalTransform
        {
            Position = GetRandomPosition(),
            Rotation = GetRandomRotation(),
            Scale = GetRandomScale(0.5f)
        };
    }

    private float3 GetRandomPosition()
    {
        float3 randomPosition;

        do
        {
            randomPosition = _graveyardRandom.ValueRW.Value.NextFloat3(MinCorner, MaxCorner);
        } while (math.distancesq(_transformAspect.ValueRO.Position, randomPosition) <= BRAIN_SAFETY_RADIUS_SQ);

        return randomPosition;
    }

    private quaternion GetRandomRotation()
    {
        return quaternion.RotateY(_graveyardRandom.ValueRW.Value.NextFloat(-0.25f, 0.25f));
    }

    private float GetRandomScale(float min)
    {
        return _graveyardRandom.ValueRW.Value.NextFloat(min, 1f);
    }

    public float ZombieSpawnTimer
    {
        get => _zombieSpawnTimer.ValueRO.Value;
        set => _zombieSpawnTimer.ValueRW.Value = value;
    }

    public bool TimeToSpawnZombie => ZombieSpawnTimer <= 0f;

    public float ZombieSpawnRate => _graveyardProperties.ValueRO.ZombieSpawnRate;

    public Entity ZombiePrefab => _graveyardProperties.ValueRO.ZombiePrefab;

    public LocalTransform GetZombieSpawnPoint()
    {
        var position = GetRandomZombieSpawnPoint();
        return new LocalTransform
        {
            Position = position,
            Rotation = quaternion.RotateY(MathHelpers.GetHeading(position, _transformAspect.ValueRO.Position)),
            Scale = 1f
        };
    }

    private float3 GetRandomZombieSpawnPoint()
    {
        return GetZombieSpawnPoint(_graveyardRandom.ValueRW.Value.NextInt(ZombieSpawnPointCount));
    }

    private float3 GetZombieSpawnPoint(int i) => _zombieSpawnPoints.ValueRO.Value.Value.Value[i];

    public float3 Position => _transformAspect.ValueRO.Position;
}