using System;

namespace Com.RandomDudes.Datas
{
    [Serializable]
    public struct TranslateText
    {
        public string text_id;
        public TranslateTextInLanguage[] translatedText;
    }
}