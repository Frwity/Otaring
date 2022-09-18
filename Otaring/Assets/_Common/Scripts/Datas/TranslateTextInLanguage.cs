using System;
using UnityEngine;

namespace Com.RandomDudes.Datas
{
    [Serializable]
    public struct TranslateTextInLanguage
    {
        public Languages language;
        [TextArea] public string text;
    }
}