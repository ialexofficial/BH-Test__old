using UnityEngine;

namespace Components
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;

        private static Spawner _instance;

        public static Spawner Instance => _instance;

        public Transform GetPoint() =>
            spawnPoints[Random.Range(0, spawnPoints.Length)];
        
        private void Awake()
        {
            _instance = this;
        }
    }
}