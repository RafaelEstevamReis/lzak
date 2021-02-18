namespace FTS.Core
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

    }
}
