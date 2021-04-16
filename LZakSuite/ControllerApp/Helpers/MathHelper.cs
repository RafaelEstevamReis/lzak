using ControllerApp.MathResources;
using System;

namespace ControllerApp.Helpers
{
    public static class MathHelper
    {
        public static float Map(float OriginMax, float DestinationMax, float ValueOnOriginToMap)
        {
            return Map(0, OriginMax, 0, DestinationMax, ValueOnOriginToMap);
        }
        public static float Map(float OriginMin, float OriginMax, float DestinationMin, float DestinationMax, float ValueOnOriginToMap)
        {
            // barra pequena
            float originLen = OriginMax - OriginMin;
            float originToValue = ValueOnOriginToMap - OriginMin;

            float percentile = originToValue / originLen;
            //

            // barra pesada
            float destLen = DestinationMax - DestinationMin;
            float cocó = destLen * percentile;
            //

            return cocó + DestinationMin;
        }
        public static float Distance(PointF P1, PointF P2)
        {
            // h = sqrt(c1^2 + c2^2)
            // o cateto vertical é a diferença do Y_P1 e o Y_P2
            // o cateto horizontal é a diferença do X_P1 e o X_P2

            float cv = P1.Y - P2.Y;
            float ch = P1.X - P2.X;

            return (float)Math.Sqrt(cv * cv + ch * ch);
        }
        public static int MillimitersToSteps(float mm, int mmPerStep)
        {
            return (int)Math.Floor(mm * mmPerStep);
        }
        public static float StepsToMillimiters(int steps, int mmPerStep)
        {
            return (float)steps / mmPerStep;
        }

        public static float StringToFloat(this string value)
        {
            try
            {
                var nValue = Convert.ToSingle(value);
                return (float)Math.Round(nValue, 2);
            }
            catch
            {
                return 0F;
            }
        }
    }
}
