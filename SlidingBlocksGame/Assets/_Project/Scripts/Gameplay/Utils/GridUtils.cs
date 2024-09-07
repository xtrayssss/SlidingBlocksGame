using _Project.Scripts.Gameplay.Features.AudioFeature.Components;
using _Project.Scripts.Gameplay.Features.AudioFeature.Systems;
using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.CooldownFeature.Components;
using _Project.Scripts.Gameplay.Features.GameWorldCreationFeature.Components;
using DCFApixels.DragonECS;
using Unity.Mathematics;
using UnityEditor;

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
            [Opt] public EcsTagPool<AudioLoopMarker> AudioLoopMarker;
            [Opt] public EcsTagPool<RefreshCooldownRequest> Refresh;
        }

        public int Create(ScriptableEntityTemplate audioCfg)
        {
            AudioAspect audioAspect = _world.GetAspect<AudioAspect>();

            int audio = _world.NewEntity(audioCfg);

            audioAspect.PlayAudio.Add(audio);
            audioAspect.AudioSource.Add(audio).Value = GameAudio.Instance.SfxSource;

            if (audioAspect.AudioLoopMarker.Has(audio))
            {
                audioAspect.Refresh.Add(audio);
            }
            else
            {
                audioAspect.DeleteEntity.Add(audio);
            }

            return audio;
        }
    }

    public static class GridUtils
    {
        public static readonly int2 Up = new int2(0, 1);
        public static readonly int2 Down = new int2(0, -1);
        public static readonly int2 Left = new int2(-1, 0);
        public static readonly int2 Right = new int2(1, 0);

        public static bool IsWithinCenter(float2 position, in GameField gameField)
        {
            return position.x >= gameField.EdgeSize &&
                   position.x < gameField.EdgeSize + gameField.CenterSize &&
                   position.y >= gameField.EdgeSize &&
                   position.y < gameField.EdgeSize + gameField.CenterSize;
        }

        public static int2 GetCenter(in GameField gameField) =>
            new int2(gameField.EdgeSize, gameField.CenterSize + gameField.EdgeSize - 1);

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

        public static int2 GetCellPosition(float3 worldPosition, in GameField gameField)
        {
            int x = (int)math.floor(
                (worldPosition.x - gameField.OriginPosition.x + gameField.CellSize * 0.5f + gameField.Offset * 0.5f) /
                (gameField.CellSize + gameField.Offset));

            int z = (int)math.floor(
                (worldPosition.z - gameField.OriginPosition.z + gameField.CellSize * 0.5f + gameField.Offset * 0.5f) /
                (gameField.CellSize + gameField.Offset));

            return new int2(x, z);
        }
        
        public static float3 GetWorldPosition(int2 coordinates, in GameField gameField) =>
            new float3(coordinates.x * (gameField.CellSize + gameField.Offset) + gameField.OriginPosition.x, 0,
                coordinates.y * (gameField.CellSize + gameField.Offset) + gameField.OriginPosition.z);
    }
}