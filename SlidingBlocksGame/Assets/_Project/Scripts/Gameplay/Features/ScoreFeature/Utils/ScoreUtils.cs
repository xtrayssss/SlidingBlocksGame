using _Project.Scripts.Gameplay.Features.CommonFeature.Components;
using _Project.Scripts.Gameplay.Features.ScoreFeature.Components;
using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Features.ScoreFeature.Utils
{
    public static class ScoreUtils
    {
        private class Aspect : EcsAspectAuto
        {
            [Opt] public readonly EcsPool<UpdateScoreRequest> UpdateScore;
            [Opt] public readonly EcsPool<UpdateBestScoreRequest> UpdateBestScore;
            [Opt] public readonly EcsPool<TargetEntity> Scorable;
        }

        public static void UpdateBestScore(int scorable, int score, bool overwrite = false)
        {
            EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();

            Aspect aspect = world.GetAspect<Aspect>();

            int request = world.NewEntity();

            ref UpdateBestScoreRequest updateRequest = ref aspect.UpdateBestScore.Add(request);
            updateRequest.Value = score;
            updateRequest.Overwrite = overwrite;
            aspect.Scorable.Add(request).Value = scorable.ToEntityLong(world);
        }

        public static void UpdateScore(int scorable, int score, bool overwrite = false)
        {
            EcsDefaultWorld world = EcsDefaultWorldSingletonProvider.Instance.Get();

            Aspect aspect = world.GetAspect<Aspect>();

            int request = world.NewEntity();

            ref UpdateScoreRequest updateRequest = ref aspect.UpdateScore.Add(request);
            updateRequest.Value = score;
            updateRequest.Overwrite = overwrite;
            aspect.Scorable.Add(request).Value = scorable.ToEntityLong(world);
        }
    }
}