using UnityEngine;

namespace Utilities
{
    public static class MappingUtil
    {
        public static float Map(
            float value,
            float originalMin,
            float originalMax,
            float newMin,
            float newMax,
            bool clamp
        )
        {
            float newValue = (value - originalMin) / (originalMax - originalMin) * (newMax - newMin) + newMin;
            if(clamp)
                newValue = Mathf.Clamp(newValue, newMin, newMax);
            
            return newValue;
        }

        public static float Remap(
            this float value,
            float from1,
            float to1,
            float from2,
            float to2
            )
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}
