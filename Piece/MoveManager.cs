using System.Collections;
using UnityEngine;

namespace Piece
{
    public class MoveManager : MonoBehaviour
    {
        public static MoveManager Instance { get; private set; }
        [SerializeField] private float moveSpeed = 2.0f;
        [SerializeField] private float heightOffset = 1.0f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void MoveToPosition(GameObject targetObject, Vector3 targetPosition, float delay)
        {
            StartCoroutine(MoveMeeple(targetObject, targetPosition, delay));
        }

        private IEnumerator MoveMeeple(GameObject targetObject, Vector3 targetPosition, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            var position = targetObject.transform.position;
            var timeToReachTarget = Vector3.Distance(position, targetPosition) / moveSpeed;
            var elapsedTime = 0f;

            while (elapsedTime < timeToReachTarget)
            {
                var normalizedTime = elapsedTime / timeToReachTarget; // Normalized time between 0 and 1
                var yOffset = heightOffset * Mathf.Sin(normalizedTime * Mathf.PI); // Upward offset based on a sine wave.

                var nextPosition = Vector3.Lerp(position, targetPosition, normalizedTime);
                nextPosition.y += yOffset; // Add the upward offset to the next position.

                targetObject.transform.position = nextPosition;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            targetObject.transform.position = targetPosition;
        }
    }
}