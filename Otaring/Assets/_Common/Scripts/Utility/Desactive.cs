using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.Otaring
{
    public class Desactive : MonoBehaviour
    {
        public void DesactiveGameObject()
        {
            gameObject.SetActive(false);
        }
    }
}
