using Com.RandomDudes.Managers;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.RandomDudes.SceneManagement
{
    public class SceneLoader : Manager
    {
        [SerializeField] private SceneField loadingScene = default;

        private AsyncOperation loadOperation;

        override protected void Awake()
        {
            base.Awake();

            Application.backgroundLoadingPriority = ThreadPriority.Low;

            DontDestroyOnLoad(gameObject);
        }

        #region Level Loading

        public void LoadScene(string sceneName)
        {
            LoadScene(sceneName, null);
        }

        public void LoadScene(string sceneName, Action callBack)
        {
            if (loadOperation != null)
                return;

            StartCoroutine(GoToLoadingScreen(() => { StartCoroutine(LoadSceneCoroutine(sceneName, callBack)); }));
        }

        private IEnumerator GoToLoadingScreen(Action callBack)
        {
            loadOperation = SceneManager.LoadSceneAsync(loadingScene, LoadSceneMode.Single);

            while (!loadOperation.isDone)
                yield return null;

            loadOperation = null;

            callBack?.Invoke();
        }

        private IEnumerator LoadSceneCoroutine(string sceneName, Action callBack)
        {
            loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            loadOperation.completed += (AsyncOperation operation) => { SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName)); };

            loadOperation.allowSceneActivation = false;

            while (loadOperation.progress < 0.9f)
                yield return null;

            loadOperation.allowSceneActivation = true;

            while (!loadOperation.isDone)
                yield return null;

            loadOperation = null;

            SceneManager.UnloadSceneAsync(loadingScene);

            callBack?.Invoke();
        }

        #endregion
    }
}