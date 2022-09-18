using UnityEngine;

namespace Com.RandomDudes.Attributes
{
    public class ClampVector2Attribute : PropertyAttribute
    {
        public readonly float minX, maxX, minY, maxY;
        public readonly bool isClamped;

        public ClampVector2Attribute(float minX, float maxX, float minY, float maxY, bool isClamped = true)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
            this.isClamped = isClamped;
        }
    }
}