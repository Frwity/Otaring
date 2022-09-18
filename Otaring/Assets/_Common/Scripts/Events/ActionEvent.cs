using System;

namespace Com.RandomDudes.Events
{
    [Serializable]
#if UNITY_EDITOR
    public partial class ActionEvent
#else
    public class ActionEvent
#endif
    {
        public static ActionEvent operator +(ActionEvent pA, Action pB) => pA.AddListener(pB);
        public static ActionEvent operator -(ActionEvent pA, Action pB) => pA.RemoveListener(pB);

        public ActionEvent() { }
        public ActionEvent(Action callback) => Action = callback;

        protected event Action Action;

        public ActionEvent AddListener(Action callback)
        {
            if (Action == null)
                Action = callback;

            else
            {
                Action -= callback;
                Action += callback;
            }

            return this;
        }

        public ActionEvent RemoveListener(Action callback)
        {
            if (Action == null) return this;

            Action -= callback;

            return this;
        }

        public void Invoke()
        {
            if (Action == null) return;
            Action();
        }

        public void RemoveAllListeners()
        {
            Action = null;
        }
    }

    public class ActionEvent<T0>
    {
        private event Action<T0> _action;

        public ActionEvent() { }
        public ActionEvent(Action<T0> callbackToAdd) => _action = callbackToAdd;

        public static ActionEvent<T0> operator +(ActionEvent<T0> pA, Action<T0> pB) => pA.AddListener(pB);
        public static ActionEvent<T0> operator -(ActionEvent<T0> pA, Action<T0> pB) => pA.RemoveListener(pB);

        public ActionEvent<T0> AddListener(Action<T0> callback)
        {
            if (_action == null)
                _action = callback;

            else
            {
                _action -= callback;
                _action += callback;
            }

            return this;
        }

        public ActionEvent<T0> RemoveListener(Action<T0> callback)
        {
            if (_action == null) return this;

            _action -= callback;

            return this;
        }

        public void Invoke(T0 p0)
        {
            if (_action == null) return;
            _action(p0);
        }

        public void RemoveAllListeners()
        {
            _action = null;
        }
    }

    public class ActionEvent<T0, T1>
    {
        private event Action<T0, T1> _action;

        public ActionEvent() { }
        public ActionEvent(Action<T0, T1> callbackToAdd) => _action = callbackToAdd;

        public static ActionEvent<T0, T1> operator +(ActionEvent<T0, T1> pA, Action<T0, T1> pB) => pA.AddListener(pB);
        public static ActionEvent<T0, T1> operator -(ActionEvent<T0, T1> pA, Action<T0, T1> pB) => pA.RemoveListener(pB);

        public ActionEvent<T0, T1> AddListener(Action<T0, T1> callback)
        {
            if (_action == null)
                _action = callback;

            else
            {
                _action -= callback;
                _action += callback;
            }

            return this;
        }

        public ActionEvent<T0, T1> RemoveListener(Action<T0, T1> callback)
        {
            if (_action == null) return this;

            _action -= callback;

            return this;
        }

        public void Invoke(T0 p0, T1 p1)
        {
            if (_action == null) return;
            _action(p0, p1);
        }

        public void RemoveAllListeners()
        {
            _action = null;
        }
    }

    public class ActionEvent<T0, T1, T2>
    {
        private event Action<T0, T1, T2> _action;

        public ActionEvent() { }
        public ActionEvent(Action<T0, T1, T2> callbackToAdd) => _action = callbackToAdd;

        public static ActionEvent<T0, T1, T2> operator +(ActionEvent<T0, T1, T2> pA, Action<T0, T1, T2> pB) => pA.AddListener(pB);
        public static ActionEvent<T0, T1, T2> operator -(ActionEvent<T0, T1, T2> pA, Action<T0, T1, T2> pB) => pA.RemoveListener(pB);

        public ActionEvent<T0, T1, T2> AddListener(Action<T0, T1, T2> callback)
        {
            if (_action == null)
                _action = callback;

            else
            {
                _action -= callback;
                _action += callback;
            }

            return this;
        }

        public ActionEvent<T0, T1, T2> RemoveListener(Action<T0, T1, T2> callback)
        {
            if (_action == null) return this;

            _action -= callback;

            return this;
        }

        public void Invoke(T0 p0, T1 p1, T2 p2)
        {
            if (_action == null) return;
            _action(p0, p1, p2);
        }

        public void RemoveAllListeners()
        {
            _action = null;
        }
    }

    public class ActionEvent<T0, T1, T2, T3>
    {
        private event Action<T0, T1, T2, T3> _action;
        public ActionEvent() { }
        public ActionEvent(Action<T0, T1, T2, T3> callbackToAdd) => _action = callbackToAdd;

        public static ActionEvent<T0, T1, T2, T3> operator +(ActionEvent<T0, T1, T2, T3> pA, Action<T0, T1, T2, T3> pB) => pA.AddListener(pB);
        public static ActionEvent<T0, T1, T2, T3> operator -(ActionEvent<T0, T1, T2, T3> pA, Action<T0, T1, T2, T3> pB) => pA.RemoveListener(pB);

        public ActionEvent<T0, T1, T2, T3> AddListener(Action<T0, T1, T2, T3> callback)
        {
            if (_action == null)
                _action = callback;

            else
            {
                _action -= callback;
                _action += callback;
            }

            return this;
        }

        public ActionEvent<T0, T1, T2, T3> RemoveListener(Action<T0, T1, T2, T3> callback)
        {
            if (_action == null) return this;

            _action -= callback;

            return this;
        }

        public void Invoke(T0 p0, T1 p1, T2 p2, T3 p3)
        {
            if (_action == null) return;
            _action(p0, p1, p2, p3);
        }

        public void RemoveAllListeners()
        {
            _action = null;
        }
    }

    public class ActionEvent<T0, T1, T2, T3, T4>
    {
        private event Action<T0, T1, T2, T3, T4> _action;

        public ActionEvent() { }
        public ActionEvent(Action<T0, T1, T2, T3, T4> callbackToAdd) => _action = callbackToAdd;

        public static ActionEvent<T0, T1, T2, T3, T4> operator +(ActionEvent<T0, T1, T2, T3, T4> pA, Action<T0, T1, T2, T3, T4> pB) => pA.AddListener(pB);
        public static ActionEvent<T0, T1, T2, T3, T4> operator -(ActionEvent<T0, T1, T2, T3, T4> pA, Action<T0, T1, T2, T3, T4> pB) => pA.RemoveListener(pB);

        public ActionEvent<T0, T1, T2, T3, T4> AddListener(Action<T0, T1, T2, T3, T4> callback)
        {
            if (_action == null)
                _action = callback;

            else
            {
                _action -= callback;
                _action += callback;
            }

            return this;
        }

        public ActionEvent<T0, T1, T2, T3, T4> RemoveListener(Action<T0, T1, T2, T3, T4> callback)
        {
            if (_action == null) return this;

            _action -= callback;

            return this;
        }

        public void Invoke(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            if (_action == null) return;
            _action(p0, p1, p2, p3, p4);
        }

        public void RemoveAllListeners()
        {
            _action = null;
        }
    }
}