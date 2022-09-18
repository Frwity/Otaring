using UnityEngine;

namespace Com.RandomDudes.Attributes
{
    public class ClampVector3Attribute : PropertyAttribute
    {
        public readonly float minX, maxX, minY, maxY, minZ, maxZ;
        public readonly bool isClamped;

        public ClampVector3Attribute(float minX, float maxX, float minY, float maxY, float minZ, float maxZ, bool isClamped = true)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
            this.minZ = minZ;
            this.maxZ = maxZ;
            this.isClamped = isClamped;
        }
    }
}