using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.Otaring
{
    public class Wall : MonoBehaviour
    {
        private static List<Wall> walls = new List<Wall>();

        [SerializeField] private float timeForSign = 0.5f;
        [SerializeField] private float timeBetweenParticleAndWall = 0.5f;
        [SerializeField] private float timeToGoUp = 0.5f;
        [SerializeField] private float timeToStayUp = 2f;
        [SerializeField] private float timeToGoDown = 0.5f;
        [SerializeField] private float distanceToTravel = 10f;
        [SerializeField] private float distanceForSign = 5f;

        [SerializeField] private Transform child = default;
        [SerializeField] private ParticleSystem[] signs = new ParticleSystem[0];

        private float elapsedTime;
        private Coroutine currentCoroutine;

        private Vector3 originalPosition;

        public static void StopWallParticles()
        {
            for (int i = walls.Count - 1; i >= 0; i--)
                for (int j = walls[i].signs.Length - 1; j >= 0; j--)
                {
                    walls[i].signs[j].Stop();
                    walls[i].signs[j].SetParticles(new ParticleSystem.Particle[0]);
                }
        }

        private void Awake()
        {
            originalPosition = child.position;
            walls.Add(this);
        }

        public void Elevate()
        {
            currentCoroutine = StartCoroutine(GoUpThenDown());
        }

        public void Reset()
        {
            child.position = originalPosition;

            if (currentCoroutine != null) 
                StopCoroutine(currentCoroutine);
        }

        private IEnumerator GoUpThenDown()
        {
            for (int i = signs.Length - 1; i >= 0; i--)
                signs[i].Play();

            yield return new WaitForSeconds(timeBetweenParticleAndWall);

            Vector3 startPosition;
            Vector3 currentPosition;
            Vector3 endPosition;

            startPosition = child.position;
            endPosition = child.position;
            endPosition.y -= distanceForSign;

            elapsedTime = 0f;

            while (elapsedTime < timeForSign)
            {
                elapsedTime += Time.deltaTime;
                child.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / timeToGoUp);

                yield return null;
            }
            elapsedTime = 0f;

            for (int i = signs.Length - 1; i >= 0; i--)
                signs[i].Stop();

            currentPosition = child.position;
            endPosition = child.position;
            endPosition.y += distanceToTravel;

            while (elapsedTime < timeToGoUp)
            {
                elapsedTime += Time.deltaTime;
                child.position = Vector3.Lerp(currentPosition, endPosition, elapsedTime / timeToGoUp);
                
                yield return null;
            }
            elapsedTime = 0f;

            while (elapsedTime < timeToStayUp)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            elapsedTime = 0f;

            while (elapsedTime < timeToGoDown)
            {
                elapsedTime += Time.deltaTime;
                child.position = Vector3.Lerp(endPosition, startPosition, elapsedTime / timeToGoDown);

                yield return null;
            }
        }

        private void OnDestroy()
        {
            walls.Remove(this);
        }
    }
}
