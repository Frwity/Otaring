using UnityEngine;

namespace Com.RandomDudes.Utility
{
    public static class ColorUtility
    {
        public static Color HexToColor(int hexInt)
        {
            return new Color((hexInt & 0xFF0000) / 255f, (hexInt & 0x00FF00) / 255f, (hexInt & 0x0000FF) / 255f);
        }
    }
}