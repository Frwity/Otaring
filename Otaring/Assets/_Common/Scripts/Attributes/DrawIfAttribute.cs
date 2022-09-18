using Com.RandomDudes.Debug;
using System;
using UnityEngine;

/// <summary>
/// Based on: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
/// /// Rework by Chakib Benssoum
/// </summary>

namespace Com.RandomDudes.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class DrawIfAttribute : PropertyAttribute
    {
        public string[] ComparedPropertyNames { get; private set; }

        public object[] ComparedValues { get; private set; }

        public DisablingTypes DisablingType { get; private set; }

        public DrawIfAttribute(string[] comparedPropertyNames, object[] comparedValues, DisablingTypes disablingType = DisablingTypes.DontDraw)
        {
            if (comparedPropertyNames.Length != comparedValues.Length)
            {
                DevLog.Error("Not all parameters have been specified");
                return;
            }

            ComparedPropertyNames = comparedPropertyNames;
            ComparedValues = comparedValues;
            DisablingType = disablingType;
        }

        public DrawIfAttribute(string comparedPropertyName, object comparedValue, DisablingTypes disablingType = DisablingTypes.DontDraw)
        {
            ComparedPropertyNames = new string[1] { comparedPropertyName };
            ComparedValues = new object[1] { comparedValue };
            DisablingType = disablingType;
        }

        public enum DisablingTypes
        {
            ReadOnly = 2,
            DontDraw = 3
        }
    }
}