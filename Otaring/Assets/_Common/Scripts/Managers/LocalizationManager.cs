using Com.RandomDudes.Datas;
using Com.RandomDudes.Debug;
using Com.RandomDudes.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Com.RandomDudes.Managers
{
    public class LocalizationManager : Manager
    {
        [SerializeField] private LanguagesData languagesData = null;

        public LanguagesData LanguagesData { get => languagesData; }

        private TranslateText[] texts = null;
        private Dictionary<string, Dictionary<Languages, string>> idToLanguageToText = new Dictionary<string, Dictionary<Languages, string>>();
        private Dictionary<Languages, Dictionary<string, string>> translatedTextToId = new Dictionary<Languages, Dictionary<string, string>>();
        private Languages[] languagesUsed = null;

        protected override void Awake()
        {
            if (GetManager<LocalizationManager>() != null)
            {
                Destroy(gameObject);
                return;
            }

            base.Awake();

            texts = languagesData.Texts;
            TranslateText translateText;
            TranslateTextInLanguage translateTextInLanguage;

            for (int i = texts.Length - 1; i >= 0; i--)
            {
                Dictionary<Languages, string> languageToText = new Dictionary<Languages, string>();
                translateText = texts[i];
                idToLanguageToText.Add(translateText.text_id, languageToText);

                for (int x = translateText.translatedText.Length - 1; x >= 0; x--)
                {
                    translateTextInLanguage = translateText.translatedText[x];
                    languageToText.Add(translateTextInLanguage.language, translateTextInLanguage.text);

                    if (!translatedTextToId.ContainsKey(translateTextInLanguage.language))
                        translatedTextToId.Add(translateTextInLanguage.language, new Dictionary<string, string>());

                    if (!translatedTextToId[translateTextInLanguage.language].ContainsKey(translateTextInLanguage.text))
                        translatedTextToId[translateTextInLanguage.language].Add(translateTextInLanguage.text, translateText.text_id);
                }
            }

            languagesUsed = new Languages[translatedTextToId.Keys.Count];
            translatedTextToId.Keys.CopyTo(languagesUsed, 0);
        }

        public string GetTranslatedLanguage(string pID) => GetTranslatedLanguage(pID, languagesData.Language);

        private string GetTranslatedLanguage(string pID, Languages pLanguage)
        {
            string lStringToReturn;
            if (idToLanguageToText.ContainsKey(pID) && idToLanguageToText[pID].ContainsKey(pLanguage))
                lStringToReturn = idToLanguageToText[pID][pLanguage];
            else
                lStringToReturn = GetTranslatedLanguageUsingPreviouslyTranslatedText(pID, pLanguage);


            lStringToReturn = lStringToReturn.Replace('é', 'e');
            lStringToReturn = lStringToReturn.Replace('è', 'e');
            return lStringToReturn;
        }

        private string GetTranslatedLanguageUsingPreviouslyTranslatedText(string pID, Languages pLanguage)
        {
            for (int i = languagesUsed.Length - 1; i >= 0; i--)
            {
                if (translatedTextToId[languagesUsed[i]].ContainsKey(pID))
                    return GetTranslatedLanguage(translatedTextToId[languagesUsed[i]][pID], pLanguage);
            }

            DevLog.Warning("pas de traduction possible pour " + pID + " pour la langue " + pLanguage);
            return "";
        }
    }
}