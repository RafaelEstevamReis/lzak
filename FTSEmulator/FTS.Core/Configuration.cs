namespace FTS.Core
{
    public class Configuration
    {
        public static Configuration Instance { get; } = new Configuration();
        
        public PointF WorkspaceMM { get; }
        // [0,0] is at Lower Left Corner!

        public float StepSizeMM { get; set; }

        private Configuration()
        {
            StepSizeMM = 2;
            WorkspaceMM = new PointF()
            {
                X = 100,
                Y = 100
            };
        }
    }
}
