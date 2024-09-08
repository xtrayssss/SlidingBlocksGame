using DCFApixels.DragonECS;

namespace _Project.Scripts.Gameplay.Utils
{
    public static class TemplateNodeExtensions
    {
        public static int NewEntity<TTemplate>(this EcsWorld world, TTemplate template) where TTemplate : ITemplateNode
        {
            int e = world.NewEntity();
            template.Apply(world.id, e);
            return e;
        }

        public static entlong NewEntityLong<TTemplate>(this EcsWorld world, ITemplateNode template)
            where TTemplate : ITemplateNode
        {
            entlong e = world.NewEntityLong();
            template.Apply(world.id, e.ID);
            return e;
        }
    }
}