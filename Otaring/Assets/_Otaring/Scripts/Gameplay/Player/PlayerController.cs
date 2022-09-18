using Com.RandomDudes.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Com.IsartDigital.Otaring.Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        public ActionEvent OnButtonPress = new ActionEvent();
        public ActionEvent OnStartPress = new ActionEvent();
        public ActionEvent OnButtonRealeased = new ActionEvent();

        public Vector2 moveInput { private set; get; } = Vector2.zero;

        public void OnMove(InputAction.CallbackContext ctx)
        {
            Vector2 ctxValue = ctx.ReadValue<Vector2>();

            moveInput = ctx.ReadValue<Vector2>();
        }

        public void OnButton(InputAction.CallbackContext ctx)
        {
            if (ctx.ReadValueAsButton())
                OnButtonPress?.Invoke();
            else
                OnButtonRealeased?.Invoke();
        }
        public void OnStart(InputAction.CallbackContext ctx)
        {
            if (ctx.ReadValueAsButton())
                OnStartPress?.Invoke();
        }
    }
}