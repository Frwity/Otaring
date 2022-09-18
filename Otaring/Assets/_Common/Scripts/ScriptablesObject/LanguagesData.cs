using Com.RandomDudes.Datas;
using Com.RandomDudes.Events;
using UnityEngine;

namespace Com.RandomDudes.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LanguagesDataObject", menuName = "ScriptableObjects/Common/LanguagesDataObject", order = 1)]
    public class LanguagesData : ScriptableObject
    {
        [SerializeField] private TranslateText[] texts = null;
        [SerializeField] private Languages language = Languages.Francais;

        public ActionEvent OnLanguageChanged = new ActionEvent();

        public TranslateText[] Texts { get => texts; }

        public Languages Language { get => language; }

        public void ChangeLanguage(Languages language)
        {
            this.language = language;

            OnLanguageChanged.Invoke();
        }
    }
}