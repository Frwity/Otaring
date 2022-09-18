using Com.RandomDudes.Events;
using UnityEngine;

namespace Com.RandomDudes.Managers
{
    public class TouchController : Manager
    {
        public ActionEvent onTouchBegin = new ActionEvent();
        public ActionEvent onTouchEnd = new ActionEvent();
        public ActionEvent onButtonBack = new ActionEvent();

        private bool hasReleasedBack = true;
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR

    private Touch mainTouch;
    private bool hasTouched = false;
    private int fingersWhenReleased = 0;

    void Update()
    {
         CheckForBackButton();

        if (Input.touchCount > 0)
        {
            if (!hasTouched)
            {
                mainTouch = Input.GetTouch(0);
                onTouchBegin.Invoke();
                hasTouched = true;
            }
            else if (mainTouch.fingerId == Input.GetTouch(0).fingerId)
            {
                mainTouch = Input.GetTouch(0);
            }
            else if (fingersWhenReleased == 0)
            {
                fingersWhenReleased = Input.touchCount;
                onTouchEnd.Invoke();
            }

            if (fingersWhenReleased != 0 && fingersWhenReleased < Input.touchCount)
            {
                hasTouched = false;
                fingersWhenReleased = 0;
            }

        }
        else if (fingersWhenReleased == 0 && hasTouched && mainTouch.phase == TouchPhase.Ended)
        {
            onTouchEnd.Invoke();
            hasTouched = false;
        }
        else
        {
            fingersWhenReleased = 0;
            hasTouched = false;
        }
    }

    public Vector3 GetTouchPosition()
    {
        if (mainTouch.phase == TouchPhase.Ended)
            return Vector3.zero;

        return mainTouch.position;
    }

#elif UNITY_EDITOR

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
                onTouchBegin.Invoke();
            else if (Input.GetMouseButtonUp(0))
                onTouchEnd.Invoke();

            CheckForBackButton();
        }

        public Vector3 GetTouchPosition()
        {
            return Input.mousePosition;
        }

#endif

        private void CheckForBackButton()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (hasReleasedBack)
                {
                    onButtonBack.Invoke();
                    hasReleasedBack = false;
                }
            }
            else
                hasReleasedBack = true;
        }
    }
}