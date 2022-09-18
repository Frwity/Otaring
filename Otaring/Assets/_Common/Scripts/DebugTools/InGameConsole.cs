using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Com.RandomDudes.Debug
{
    public class InGameConsole : MonoBehaviour
    {
        struct Log
        {
            public string message;
            public string stackTrace;
            public LogType type;
        }

        static readonly Dictionary<LogType, string> logTypeColors = new Dictionary<LogType, string>()
        {
            { LogType.Assert, "white" },
            { LogType.Error, "red" },
            { LogType.Exception, "red" },
            { LogType.Log, "white" },
            { LogType.Warning, "yellow" },
        };

        private const int TIME_TO_TEST_CLICK = 2;

        /// <summary>
        /// The hotkey to show and hide the console window.
        /// </summary>
        public KeyCode toggleKey = KeyCode.BackQuote;

        /// <summary>
        /// Is the console visible at the start.
        /// </summary>
        public bool activeAtStart = false;
        public bool dontDestroyOnLoad = true;

        List<Log> logs = new List<Log>();
        public void ClearLogs() { logs.Clear(); }

        uint duplicates = 0;
        bool isAtBottom = true;

        public int scrollbarWidth = 10;
        public int margins = 20;
        public int textSize = 14;
        public int ClickCounter = 0;

        GameObject consoleCanvas;
        GameObject scrollview;
        GameObject scrollBar;
        GameObject handle;
        GameObject consoleText;

        Text m_Text;
        Scrollbar m_Scrollbar;
        ScrollRect m_ScrollRect;

        [SerializeField] private AudioSource errorSound = null;
        [SerializeField] private AudioClip[] errorSoundsList = null;
        private List<AudioClip> copiedErrorSoundsList = null;
        private float timeCounter = 0;
        private float timeCounterCoroutine = 0;
        private bool isActivated = false;

        // Start is called before the first frame update
        void Awake()
        {
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            copiedErrorSoundsList = errorSoundsList.ToList();
            RectTransform m_RectTransform;

            consoleCanvas = new GameObject("console", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            consoleCanvas.transform.SetParent(this.transform);
            if (this.gameObject.GetComponent<RectTransform>() == null)
                consoleCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            else
            {
                Destroy(consoleCanvas.GetComponent<CanvasScaler>());

                m_RectTransform = consoleCanvas.GetComponent<RectTransform>();
                m_RectTransform.sizeDelta = new Vector2(0, 0);
                m_RectTransform.anchorMin = new Vector2(0, 0);
                m_RectTransform.anchorMax = new Vector2(1, 1);
                m_RectTransform.anchoredPosition = new Vector2(0, 0);
            }

            scrollview = new GameObject("scrollView", typeof(CanvasRenderer), typeof(ScrollRect), typeof(Image), typeof(Mask));
            scrollview.transform.SetParent(consoleCanvas.transform);

            m_RectTransform = scrollview.GetComponent<RectTransform>();
            m_RectTransform.sizeDelta = new Vector2(-(margins * 2), -(margins * 2));
            m_RectTransform.anchorMin = new Vector2(0, 0);
            m_RectTransform.anchorMax = new Vector2(1, 1);
            m_RectTransform.anchoredPosition = new Vector2(0, 0);

            m_ScrollRect = scrollview.GetComponent<ScrollRect>();
            m_ScrollRect.horizontal = false;
            m_ScrollRect.inertia = false;
            m_ScrollRect.movementType = ScrollRect.MovementType.Clamped;

            scrollview.GetComponent<Image>().color = Color.black;


            scrollBar = new GameObject("scrollBar", typeof(CanvasRenderer), typeof(Scrollbar));
            scrollBar.transform.SetParent(consoleCanvas.transform);

            m_RectTransform = scrollBar.GetComponent<RectTransform>();
            m_RectTransform.sizeDelta = new Vector2(scrollbarWidth, -(margins * 2));
            m_RectTransform.anchorMin = new Vector2(1, 0);
            m_RectTransform.anchorMax = new Vector2(1, 1);
            m_RectTransform.anchoredPosition = new Vector2(-(margins + scrollbarWidth), 0);
            m_RectTransform.pivot = new Vector2(0.5f, 0.5f);


            handle = new GameObject("handle", typeof(CanvasRenderer), typeof(Image));
            handle.transform.SetParent(scrollBar.transform);

            m_RectTransform = handle.GetComponent<RectTransform>();
            m_RectTransform.sizeDelta = new Vector2(scrollbarWidth, 0);
            m_RectTransform.anchoredPosition = new Vector2(0, 0);
            m_RectTransform.pivot = new Vector2(0.5f, 0.5f);

            m_Scrollbar = scrollBar.GetComponent<Scrollbar>();
            m_Scrollbar.targetGraphic = handle.GetComponent<Image>();
            m_Scrollbar.handleRect = handle.GetComponent<RectTransform>();
            m_Scrollbar.direction = Scrollbar.Direction.BottomToTop;

            m_ScrollRect.verticalScrollbar = m_Scrollbar;
            m_ScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;


            consoleText = new GameObject("text", typeof(CanvasRenderer), typeof(Text), typeof(ContentSizeFitter));
            consoleText.transform.SetParent(scrollview.transform);
            scrollview.GetComponent<ScrollRect>().content = m_RectTransform;

            consoleText.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            m_RectTransform = consoleText.GetComponent<RectTransform>();
            m_RectTransform.sizeDelta = new Vector2(-50, 0);
            m_RectTransform.anchorMin = new Vector2(0, 0);
            m_RectTransform.anchorMax = new Vector2(1, 1);
            m_RectTransform.anchoredPosition = new Vector2(-15, 0);
            m_RectTransform.pivot = new Vector2(0.5f, 1.0f);

            scrollview.GetComponent<ScrollRect>().content = m_RectTransform;

            m_Text = consoleText.GetComponent<Text>();
            m_Text.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            m_Text.fontSize = textSize;

            m_Text.text = "";

            if (!activeAtStart)
                consoleCanvas.SetActive(false);
        }


        /// <summary>
        /// Records a log from the log callback.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="stackTrace">Trace of where the message came from.</param>
        /// <param name="type">Type of message (error, exception, warning, assert).</param>
        void HandleLog(string message, string stackTrace, LogType type)
        {
            logs.Add(new Log()
            {
                message = message,
                stackTrace = stackTrace,
                type = type,

            });


            if ((type == LogType.Exception || type == LogType.Warning || type == LogType.Error) && !errorSound.isPlaying)
            {
                if (copiedErrorSoundsList.Count == 0) copiedErrorSoundsList = errorSoundsList.ToList();
                int lIndex = UnityEngine.Random.Range(0, copiedErrorSoundsList.Count - 1);
                AudioClip lCurrentSound = copiedErrorSoundsList[lIndex];
                copiedErrorSoundsList.RemoveAt(lIndex);
                errorSound.clip = lCurrentSound;



                errorSound.Play();

            }
        }

        void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
            isActivated = true;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
            isActivated = false;
        }


        public void ActivateDebug()
        {
            consoleCanvas.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {
            if (!isActivated) return;

            if (Input.GetMouseButton(0))
            {
                timeCounter += Time.deltaTime;

                if (timeCounter > 1.5)
                {
                    consoleCanvas.SetActive(false);
                    return;
                }

            }

            else timeCounter = 0;

            if (consoleCanvas.activeInHierarchy)
            {
                isAtBottom = (m_Scrollbar.value == 0);

                m_Text.text = "";
                duplicates = 0;

                for (int index = 0; index < logs.Count; index++)
                {
                    var log = logs[index];

                    if (index + 1 < logs.Count && log.message == logs[index + 1].message)
                    {

                        duplicates += 1;
                        continue;
                    }
                    else
                    {
                        if (duplicates > 0)
                        {
                            m_Text.text += "<color=" + logTypeColors[log.type] + ">" + log.message + "</color>" + " <color=green>(" + (duplicates + 1).ToString() + ")</color>" + "\n";
                            duplicates = 0;
                            continue;
                        }
                    }
                    m_Text.text += "<color=" + logTypeColors[log.type] + ">" + log.message + "</color> \n";

                    if (log.type == LogType.Exception || log.type == LogType.Error)
                        m_Text.text += "<color=blue>" + log.stackTrace + "</color>" + "\n";
                }
                if (isAtBottom)
                    m_ScrollRect.verticalNormalizedPosition = 0;
            }
        }

        public void OnClick()
        {
            ClickCounter++;
            timeCounterCoroutine = 0;

            if (ClickCounter >= 3)
            {
                ClickCounter = 0;
                consoleCanvas.SetActive(!consoleCanvas.activeSelf);
            }

            StartCoroutine(CheckClick());
        }

        private IEnumerator CheckClick()
        {
            while (timeCounterCoroutine < TIME_TO_TEST_CLICK)
            {
                timeCounterCoroutine += Time.deltaTime;
                yield return null;
            }

            ClickCounter = 0;
        }
    }
}