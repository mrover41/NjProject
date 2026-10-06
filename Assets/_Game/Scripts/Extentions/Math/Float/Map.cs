using UnityEngine;

namespace Ext.Math {
    public static class FloatExt {
        public static float Map(this float value, float oldMin, float oldMax, float newMin, float newMax)  {
                return Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(oldMin, oldMax, value));
        }
    }
}