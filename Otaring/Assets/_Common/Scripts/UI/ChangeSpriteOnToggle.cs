using UnityEngine;
using UnityEngine.UI;

namespace Com.RandomDudes.UI
{
    public class ChangeSpriteOnToggle : MonoBehaviour
    {
        [SerializeField] private readonly Sprite enabledState = default;
        [SerializeField] private readonly Sprite disabledState = default;

        private Toggle toggle;
        private Image image;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
            image = GetComponent<Image>();
        }

        private void Start() => toggle.onValueChanged.AddListener(Toggle_OnValueChanged);

        private void Toggle_OnValueChanged(bool value) => image.sprite = value ? enabledState : disabledState;

        private void OnDestroy() => toggle.onValueChanged.RemoveListener(Toggle_OnValueChanged);
    }
}