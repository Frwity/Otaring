using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.Otaring
{
    public class HUD2 : HUD
    {
        protected override void LevelManager_OnScoreUpdated(uint leftTeamLife, uint rightTeamLife)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i > leftTeamLife - 1)
                    lifeList[i].SetActive(false);
                else
                    lifeList[i].SetActive(true);
            }
        }
    }
}
