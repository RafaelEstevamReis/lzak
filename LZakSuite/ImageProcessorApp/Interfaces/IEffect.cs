namespace ImageProcessorApp.Interfaces
{
    public interface IEffect
    {
        System.Drawing.Image Process(System.Drawing.Image image);
    }
}
