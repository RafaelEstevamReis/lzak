namespace FTS.Core
{
    public class Cunfiguration
    {
        public static Cunfiguration Instance { get; } = new Cunfiguration();

        public PointF WorkspaceMM { get; }
        // [0,0] is at Lower Left Corner!

        private Cunfiguration()
        {
            WorkspaceMM = new PointF()
            {
                X = 100,
                Y = 100
            };
        }
    }
}
