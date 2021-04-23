using System.ComponentModel;

namespace ImageProcessorApp
{
    public enum ImageTypes
    {
        [Description("Bitmap (.bmp) image")]
        BMP,
        
        [Description("Portable Grapics Format (.png) image")]
        PNG,

        [Description("Joint Photographic Group (.jpg) image")]
        JPG,

        [Description("Unsupported image file type")]
        Unsupported,

        [Description("Image file type not allowed")]
        NotAllowed
    }
}
