using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using _Project.Scripts.Gameplay.Features.UIFeature.Systems;
using DCFApixels.DragonECS;
using Unity.Mathematics;

namespace _Project.Scripts.Gameplay.Utils
{
    public class AudioUtils : IEcsProcess
    {
        [EcsInject] private EcsDefaultWorld _world;

        private class AudioAspect : EcsAspectAuto
        {
            [Opt] public EcsTagPool<PlayAudioRequest> PlayAudio;
            [Opt] public EcsTagPool<DeleteEntityCommand> DeleteEntity;
            [Opt] public EcsPool<AudioSourceRef> AudioSource;
        }

        public int Create(ScriptableEntityTemplate audioCfg)
        {
            AudioAspect audioAspect = _world.GetAspect<AudioAspect>();

            int request = _world.NewEntity(audioCfg);

            audioAspect.PlayAudio.Add(request);
            audioAspect.DeleteEntity.Add(request);
            audioAspect.AudioSource.Add(request).Value = AudioSingleton.Instance.SfxSource;

            return request;
        }
    }

    public static class GameFieldUtils
    {
        private static readonly int2 Up = new int2(0, 1);
        private static readonly int2 Down = new int2(0, -1);
        private static readonly int2 Left = new int2(-1, 0);
        private static readonly int2 Right = new int2(1, 0);

        public static int2 GetInvertedSide(float2 position, in GameField gameField)
        {
            if (position.x < gameField.EdgeSize)
                return Right;
            if (position.x >= gameField.EdgeSize + gameField.CenterSize)
                return Left;
            if (position.y < gameField.EdgeSize)
                return Up;
            if (position.y >= gameField.EdgeSize + gameField.CenterSize)
                return Down;

            return default;
        }

        public static float2 WorldToGridPosition(float3 worldPosition, in GameField field)
        {
            int x = (int)math.floor(
                (worldPosition.x - field.OriginPosition.x + field.CellSize * 0.5f + field.Offset * 0.5f) /
                (field.CellSize + field.Offset));

            int z = (int)math.floor(
                (worldPosition.z - field.OriginPosition.z + field.CellSize * 0.5f + field.Offset * 0.5f) /
                (field.CellSize + field.Offset));

            return new float2(x, z);
        }

        public static float3 GetWorldPosition(float2 coordinates, in GameField field) =>
            new float3(coordinates.x * (field.CellSize + field.Offset) + field.OriginPosition.x, 0,
                coordinates.y * (field.CellSize + field.Offset) + field.OriginPosition.z);
    }
}