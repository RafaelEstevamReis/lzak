using ImageProcessorApp.Interfaces;
using System;
using System.Drawing;

namespace ImageProcessorApp.Effects
{
    public static class EffectExtensions
    {
        public static Image Apply(this Image img, IEffect effect)
        {
            return effect.Process(img);
        }
        public static Image Apply<T>(this Image image) where T : IEffect
        {
            IEffect eff = (IEffect)Activator.CreateInstance(typeof(T));
            return Apply(image, eff);
        }
    }
}
