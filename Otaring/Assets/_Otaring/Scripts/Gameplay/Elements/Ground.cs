using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.Otaring
{
    public class Ground : MonoBehaviour
    {
        public IEnumerator Sparkles(float duration)
        {
            float elapsedTime = 0f;
            float elapsedTime2 = 0f;
            bool bSwitchTransition = false;
            Color newColor = Color.white;
            Material material = GetComponent<Renderer>().material;
            Color originalColor = material.color;


            while (elapsedTime < duration)
            {

                if (elapsedTime2 > 0.25f)
                {
                    elapsedTime2 = 0f;
                    bSwitchTransition = !bSwitchTransition;
                }
                
                if (bSwitchTransition)
                    material.color = Color.Lerp(originalColor, newColor, elapsedTime2 / 0.25f);
                else
                    material.color = Color.Lerp(newColor, originalColor, elapsedTime2 / 0.25f);
                
                elapsedTime2 += Time.deltaTime;
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            GetComponent<Renderer>().material.color = originalColor;
        }
    }
}
