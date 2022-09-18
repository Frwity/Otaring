using UnityEngine;

namespace Com.RandomDudes.Datas
{
    public class UIButtonData : MonoBehaviour
    {
        [SerializeField] private UIButtonSide uiButtonSide = UIButtonSide.Left;

        public UIButtonSide UIButtonSide
        {
            get => uiButtonSide;
        }
    }
}