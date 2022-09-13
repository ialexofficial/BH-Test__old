using Controllers;
using Mirror;
using UnityEngine;

namespace Components
{
    public class ScoreManager : NetworkBehaviour
    {
        [SerializeField] private int winScore = 3;

        private readonly SyncDictionary<uint, int> _scores = new SyncDictionary<uint, int>();
        private static ScoreManager _instance;

        public static ScoreManager Instance => _instance;

        public int GetScore(uint netId) =>
            _scores.ContainsKey(netId) ? _scores[netId] : 0;

        [Command(requiresAuthority = false)]
        public void Connect(uint netId)
        {
            _scores[netId] = 0;
        }

        [Command(requiresAuthority = false)]
        public void IncreaseScore(uint netId)
        {
            if (++_scores[netId] == winScore)
            {
                EndGame(netId);
            }
        }
        
        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            _scores.Callback += UpdateScores;
        }

        [ClientRpc]
        private void EndGame(uint netId)
        {
            GameManager.Instance.EndGame(netId);
        }

        private void UpdateScores(SyncDictionary<uint, int>.Operation operation, uint key, int value)
        {
            foreach (var player in FindObjectsOfType<PlayerController>())
            {
                player.UpdateScore(_scores[player.NetId]);
            }
        }
    }
}