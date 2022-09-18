using Com.IsartDigital.Otaring;
using Com.RandomDudes.Utility;
using System;
using System.Collections.Generic;

namespace Com.RandomDudes.Managers
{
    public abstract class Manager : StateMachine
    {
        private static Dictionary<Type, Manager> managers = new Dictionary<Type, Manager>();

        protected static GameManager gameManager;

        #region Get Managers

        public static T GetManager<T>() where T : Manager
        {
            Type lType = typeof(T);
            if (!managers.ContainsKey(lType))
                return null;

            return (T)(managers[lType]);
        }

        public static void AddManager(Manager pManager)
        {
            if (managers.ContainsKey(pManager.GetType()))
                return;

            managers.Add(pManager.GetType(), pManager);
        }

        public static void RemoveManager(Manager pManager)
        {
            if (!managers.ContainsKey(pManager.GetType()))
                return;

            managers.Remove(pManager.GetType());
        }

        #endregion Get Managers

        public static GameManager GameManager
        {
            get
            {
                if (gameManager == null)
                    throw new Exception("Game Manager is null");

                return gameManager;
            }

            protected set
            {
                gameManager = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            AddManager(this);
        }

        virtual protected void OnDestroy()
        {
            RemoveManager(this);
        }
    }
}