using System.Collections;
using UnityEngine;

namespace Com.RandomDudes.Utility
{
    public class CardinalPointsAroundVector2 : IEnumerable
    {
        private Vector2 baseVector;
        private float offsetLength;

        public CardinalPointsAroundVector2(Vector2 baseVector, float offsetLength)
        {
            this.baseVector = baseVector;
            this.offsetLength = offsetLength;
        }

        public IEnumerator GetEnumerator()
        {
            return new Enumerator(baseVector, offsetLength);
        }

        private class Enumerator : IEnumerator
        {
            private static readonly Vector2[] offsets = new Vector2[] { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };

            public object Current { get => baseVector + offsets[position] * offsetLength; }

            private int position = -1;
            private float offsetLength;

            private Vector2 baseVector;

            public Enumerator(Vector2 pBaseVector, float pOffsetLength)
            {
                baseVector = pBaseVector;
                offsetLength = pOffsetLength;
            }

            public bool MoveNext()
            {
                return (++position < offsets.Length);
            }

            public void Reset()
            {
                position = -1;
            }
        }
    }
}