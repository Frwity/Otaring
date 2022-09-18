using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.Otaring.Gameplay.Elements
{
    public class TeamObject : MonoBehaviour
    {
        public TeamColor Team { get; private set; } = TeamColor.NONE;

        [Header("Team")]
        [SerializeField] private List<RendererToUpdate> renderersToUpdate = new List<RendererToUpdate>();

        virtual public void SetTeam(TeamColor team)
        {
            Team = team;

            Color color = team == TeamColor.BLUE ? Color.blue : Color.red;
            Material[] materials;
            for (int i = renderersToUpdate.Count - 1; i >= 0; i--)
            {
                materials = renderersToUpdate[i].renderer.materials;

                for (int j = 0; j < renderersToUpdate[i].materialsToUpdate.Count; j++)
                    materials[renderersToUpdate[i].materialsToUpdate[j]].color = color;
            }
        }

        [System.Serializable]
        public class RendererToUpdate
        {
            public Renderer renderer = default;
            public List<int> materialsToUpdate = new List<int>();
        }
    }
}