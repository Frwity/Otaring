using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.Otaring
{
    public class ButtonMakingSound : Button
    {
        protected override void Awake()
        {
            onClick.AddListener(PlayClickSound);
        }

        private void PlayClickSound()
        {
            
        }
    }
}
