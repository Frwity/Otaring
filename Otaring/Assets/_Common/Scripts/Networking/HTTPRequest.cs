using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Com.RandomDudes.Networking
{
    public static class HTTPRequest
    {
        private static readonly GameObject requestParent;

        static HTTPRequest()
        {
            requestParent = new GameObject("HTTP Requests");

            UnityEngine.Object.DontDestroyOnLoad(requestParent);
        }

        #region Get Requests

        public static void GetRequest(string url, Action<string> callback = null)
        {
            GetRequest(url, new Dictionary<string, string>(), callback);
        }

        public static void GetRequest(string url, Dictionary<string, string> headers, Action<string> callback = null)
        {
            HTTPRequestInstance instance = new GameObject(url).AddComponent<HTTPRequestInstance>();
            instance.transform.SetParent(requestParent.transform);

            instance.GetRequest(url, headers, callback);
        }

        public static void GetRequest(string url, Action<byte[]> callback = null)
        {
            GetRequest(url, new Dictionary<string, string>(), callback);
        }

        public static void GetRequest(string url, Dictionary<string, string> headers, Action<byte[]> callback = null)
        {
            HTTPRequestInstance instance = new GameObject(url).AddComponent<HTTPRequestInstance>();
            instance.transform.SetParent(requestParent.transform);

            instance.GetRequest(url, headers, callback);
        }

        #endregion Get Requests

        #region Post Requests

        public static void PostRequest(string url, string body, Action<string> callback = null)
        {
            PostRequest(url, body, new Dictionary<string, string>(), callback);
        }

        public static void PostRequest(string url, string body, Dictionary<string, string> headers, Action<string> callback = null)
        {
            HTTPRequestInstance instance = new GameObject(url).AddComponent<HTTPRequestInstance>();
            instance.transform.SetParent(requestParent.transform);

            instance.PostRequest(url, body, headers, callback);
        }

        public static void PostRequest(string url, WWWForm form, Action<string> callback = null)
        {
            PostRequest(url, form, new Dictionary<string, string>(), callback);
        }

        public static void PostRequest(string url, WWWForm form, Dictionary<string, string> headers, Action<string> callback = null)
        {
            HTTPRequestInstance instance = new GameObject(url).AddComponent<HTTPRequestInstance>();
            instance.transform.SetParent(requestParent.transform);

            instance.PostRequest(url, form, headers, callback);
        }

        public static void PostRequest(string url, string body, Action<byte[]> callback = null)
        {
            PostRequest(url, body, new Dictionary<string, string>(), callback);
        }

        public static void PostRequest(string url, string body, Dictionary<string, string> headers, Action<byte[]> callback = null)
        {
            HTTPRequestInstance instance = new GameObject(url).AddComponent<HTTPRequestInstance>();
            instance.transform.SetParent(requestParent.transform);

            instance.PostRequest(url, body, headers, callback);
        }

        public static void PostRequest(string url, WWWForm form, Action<byte[]> callback = null)
        {
            PostRequest(url, form, new Dictionary<string, string>(), callback);
        }

        public static void PostRequest(string url, WWWForm form, Dictionary<string, string> headers, Action<byte[]> callback = null)
        {
            HTTPRequestInstance instance = new GameObject(url).AddComponent<HTTPRequestInstance>();
            instance.transform.SetParent(requestParent.transform);

            instance.PostRequest(url, form, headers, callback);
        }

        #endregion Post Requests

        private class HTTPRequestInstance : MonoBehaviour
        {
            private void Awake()
            {
                DontDestroyOnLoad(gameObject);
            }

            #region Get Requests

            public void GetRequest(string url, Dictionary<string, string> headers, Action<string> callback = null)
            {
                StartCoroutine(GetRequestCoroutine(url, headers, callback));
            }

            private IEnumerator GetRequestCoroutine(string url, Dictionary<string, string> headers, Action<string> callback = null)
            {
                using (var request = UnityWebRequest.Get(url))
                {
                    foreach (KeyValuePair<string, string> header in headers)
                        request.SetRequestHeader(header.Key, header.Value);

                    yield return request.SendWebRequest();

                    callback?.Invoke(request.downloadHandler.text);

                    Destroy(gameObject);
                }
            }

            public void GetRequest(string url, Dictionary<string, string> headers, Action<byte[]> callback = null)
            {
                StartCoroutine(GetRequestCoroutine(url, headers, callback));
            }

            private IEnumerator GetRequestCoroutine(string url, Dictionary<string, string> headers, Action<byte[]> callback = null)
            {
                using (var request = UnityWebRequest.Get(url))
                {
                    foreach (KeyValuePair<string, string> header in headers)
                        request.SetRequestHeader(header.Key, header.Value);

                    yield return request.SendWebRequest();

                    callback?.Invoke(request.downloadHandler.data);

                    Destroy(gameObject);
                }
            }

            #endregion Get Requests

            #region Post Requests

            public void PostRequest(string url, string body, Dictionary<string, string> headers, Action<string> callback = null)
            {
                StartCoroutine(PostRequestCoroutine(url, body, headers, callback));
            }

            private IEnumerator PostRequestCoroutine(string url, string body, Dictionary<string, string> headers, Action<string> callback = null)
            {
                using (var request = UnityWebRequest.Post(url, ""))
                {
                    foreach (KeyValuePair<string, string> header in headers)
                        request.SetRequestHeader(header.Key, header.Value);

                    if (body != "")
                    {
                        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(body);
                        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
                    }

                    yield return request.SendWebRequest();

                    callback?.Invoke(request.downloadHandler.text);

                    Destroy(gameObject);
                }
            }

            public void PostRequest(string url, WWWForm form, Dictionary<string, string> headers, Action<string> callback = null)
            {
                StartCoroutine(PostRequestCoroutine(url, form, headers, callback));
            }

            private IEnumerator PostRequestCoroutine(string url, WWWForm form, Dictionary<string, string> headers, Action<string> callback = null)
            {
                using (var request = UnityWebRequest.Post(url, form))
                {
                    foreach (KeyValuePair<string, string> header in headers)
                        request.SetRequestHeader(header.Key, header.Value);

                    yield return request.SendWebRequest();

                    callback?.Invoke(request.downloadHandler.text);

                    Destroy(gameObject);
                }
            }

            public void PostRequest(string url, string body, Dictionary<string, string> headers, Action<byte[]> callback = null)
            {
                StartCoroutine(PostRequestCoroutine(url, body, headers, callback));
            }

            private IEnumerator PostRequestCoroutine(string url, string body, Dictionary<string, string> headers, Action<byte[]> callback = null)
            {
                using (var request = UnityWebRequest.Post(url, ""))
                {
                    foreach (KeyValuePair<string, string> header in headers)
                        request.SetRequestHeader(header.Key, header.Value);

                    if (body != "")
                    {
                        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(body);
                        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
                    }

                    yield return request.SendWebRequest();

                    callback?.Invoke(request.downloadHandler.data);

                    Destroy(gameObject);
                }
            }

            public void PostRequest(string url, WWWForm form, Dictionary<string, string> headers, Action<byte[]> callback = null)
            {
                StartCoroutine(PostRequestCoroutine(url, form, headers, callback));
            }

            private IEnumerator PostRequestCoroutine(string url, WWWForm form, Dictionary<string, string> headers, Action<byte[]> callback = null)
            {
                using (var request = UnityWebRequest.Post(url, form))
                {
                    foreach (KeyValuePair<string, string> header in headers)
                        request.SetRequestHeader(header.Key, header.Value);

                    yield return request.SendWebRequest();

                    callback?.Invoke(request.downloadHandler.data);

                    Destroy(gameObject);
                }
            }

            #endregion Post Requests
        }
    }
}