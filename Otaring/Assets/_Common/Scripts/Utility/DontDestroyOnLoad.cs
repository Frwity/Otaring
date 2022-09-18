using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.Otaring
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
