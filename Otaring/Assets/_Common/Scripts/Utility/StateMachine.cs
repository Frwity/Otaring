using System;
using UnityEngine;

namespace Com.RandomDudes.Utility
{
    abstract public class StateMachine : MonoBehaviour
    {
        public Action DoAction { get; protected set; }

        virtual protected void Awake()
        {
            SetModeVoid();
        }

        virtual public void NormalMode()
        {
            SetModeNormal();
        }

        virtual public void VoidMode()
        {
            SetModeVoid();
        }

        private void SetModeVoid()
        {
            DoAction = DoActionVoid;
        }

        private void SetModeNormal()
        {
            DoAction = DoActionNormal;
        }

        virtual protected void DoActionVoid() { }

        virtual protected void DoActionNormal() { }
    }
}