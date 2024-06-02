using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    // Singleton instance for easy access
    public static GameManager Instance;

    void Awake()
    {
        // Implement singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Call this method to start the game
    public void StartGame()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            PlayerManager playerManager = PlayerManager.Find(player);
            if (playerManager != null)
            {
                playerManager.ResetStats();
            }
        }

        // Other code to start the game, e.g., spawning players
    }
}