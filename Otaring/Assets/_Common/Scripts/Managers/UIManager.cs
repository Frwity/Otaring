using Com.RandomDudes.Debug;
using Com.RandomDudes.Managers;
using Com.RandomDudes.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.RandomDudes
{
    public class UIManager : Manager
    {
        private static readonly Dictionary<Type, UIScreen> typeToScreensDictionnary = new Dictionary<Type, UIScreen>();
        private static readonly List<UIScreen> activeScreens = new List<UIScreen>();

        protected override void Awake()
        {
            base.Awake();

            UIScreen UIScreen;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (!transform.GetChild(i).TryGetComponent(out UIScreen))
                    continue;

                UIScreen.Init(this);
                UIScreen.gameObject.SetActive(false);
            }
        }

        #region Get Screens

        public static T GetScreen<T>() where T : UIScreen
        {
            Type type = typeof(T);

            if (!typeToScreensDictionnary.ContainsKey(type))
                DevLog.Warning(type.ToString() + " is missing");

            return (T)(typeToScreensDictionnary[type]);
        }

        public static void AddScreenToDictionnary(UIScreen pScreen) => typeToScreensDictionnary.Add(pScreen.GetType(), pScreen);

        public static void RemoveScreenFromDictionnary(UIScreen pScreen) => typeToScreensDictionnary.Remove(pScreen.GetType());

        #endregion Get Screens

        #region Screens

        #region Add Screen

        public T AddScreen<T>() where T : UIScreen
        {
            return AddScreen<T>(AnimationScreenType.Immediate, "");
        }

        public T AddScreen<T>(AnimationScreenType animationScreenType, string nameAnimation = "") where T : UIScreen
        {
            return AddScreen<T>(animationScreenType, null, nameAnimation);
        }

        public T AddScreen<T>(AnimationScreenType animationScreenType, Action callback, string nameAnimation = "") where T : UIScreen
        {
            StopInteractionEveryting();

            T screen = GetScreen<T>();
            activeScreens.Add(screen);

            switch (animationScreenType)
            {
                case AnimationScreenType.Animator:
                    screen.OpenImmediate();
                    screen.StartAnimation(nameAnimation, callback);
                    break;
                case AnimationScreenType.Tween:
                    screen.OpenScreenTween(callback);
                    break;
                case AnimationScreenType.Coroutine:
                    screen.OpenCoroutine(callback);
                    break;
                case AnimationScreenType.Immediate:
                    screen.OpenImmediate();
                    callback?.Invoke();
                    break;
            }

            return screen;
        }

        #endregion Add Screen

        #region Remove Screen

        public T RemoveScreen<T>() where T : UIScreen
        {
            return RemoveScreen<T>(AnimationScreenType.Immediate, null, "");
        }

        public T RemoveScreen<T>(AnimationScreenType animationScreenType, string nameAnimation = "") where T : UIScreen
        {
            return RemoveScreen<T>(animationScreenType, null, nameAnimation);
        }

        public T RemoveScreen<T>(AnimationScreenType animationScreenType, Action callback, string nameAnimation = "") where T : UIScreen
        {
            T screen = GetScreen<T>();
            activeScreens.Remove(screen);

            switch (animationScreenType)
            {
                case AnimationScreenType.Animator:
                    callback += screen.CloseImmediate;
                    screen.StartAnimation(nameAnimation, callback);
                    break;
                case AnimationScreenType.Tween:
                    screen.CloseScreenTween(callback);
                    break;
                case AnimationScreenType.Coroutine:
                    callback += screen.CloseImmediate;
                    screen.CloseCoroutine(callback);
                    break;
                case AnimationScreenType.Immediate:
                    screen.CloseImmediate();
                    callback?.Invoke();
                    break;
            }

            return screen;
        }

        #endregion Remove Screen

        #region Remove Screens Tween
        public void RemoveAllScreensTween(Action callback)
        {
            if (activeScreens.Count > 0)
            {
                for (int i = activeScreens.Count - 1; i > 0; i--)
                    activeScreens[i].CloseScreenTween();

                activeScreens[0].CloseScreenTween(callback);
                activeScreens.Clear();
            }
            else callback();
        }

        public void RemoveAllScreensTween()
        {
            if (activeScreens.Count == 0)
                return;

            for (int i = activeScreens.Count - 1; i > -1; i--)
            {
                activeScreens[i].CloseScreenTween();
            }
            activeScreens.Clear();
        }

        public void RemoveAllScreensImmediate()
        {
            if (activeScreens.Count == 0)
                return;

            for (int i = activeScreens.Count - 1; i > -1; i--)
                activeScreens[i].CloseImmediate();

            activeScreens.Clear();
        }

        #endregion Remove Screen Tween

        public void StopInteractionScreens()
        {
            foreach (UIScreen lScreen in activeScreens)
                lScreen.StopInteraction();
        }

        #endregion Screens

        public void StopInteractionEveryting()
        {
            StopInteractionScreens();
        }
    }
}