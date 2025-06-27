using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;
using System;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }
    private FirebaseAuth auth;
    private FirebaseFirestore db;
    private string userId;
    private PlayerStats localPlayerStats;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                db = FirebaseFirestore.DefaultInstance;
                SignInAnonymously();
            }
        });
    }

    private void SignInAnonymously()
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task => {
            if (!task.IsCanceled && !task.IsFaulted)
            {
                userId = task.Result.User.UserId;
                LoadPlayerStats();
            }
        });
    }

    public async void SubmitScore(int newScore, int longestCombo)
    {
        Debug.Log("LEVEL ENDED! LM: SubmitScore: Final Score: before check" + newScore);
        if (string.IsNullOrEmpty(userId) || localPlayerStats == null) return;
        localPlayerStats.longestCombo = Mathf.Max(localPlayerStats.longestCombo, longestCombo);
        if (newScore > localPlayerStats.highestScore) localPlayerStats.highestScore = newScore;
        localPlayerStats.totalPlantsBloomed++;
        Debug.Log("LEVEL ENDED! LM: SubmitScore: Final Score: " + newScore);

        try
        {
            localPlayerStats.playerName = ProfileManager.Instance.PlayerName;
            Debug.Log("fetching local player stats: " + localPlayerStats.playerName);
            DocumentReference docRef = db.Collection("leaderboard").Document(userId);
            await docRef.SetAsync(localPlayerStats);
            Debug.Log("fetching local player stats after async");
        }
        catch(Exception ex)
        {
            Debug.Log("Error fetching local player stats: " + ex.Message);
        }
    }

    private async void LoadPlayerStats()
    {
        Debug.Log("LoadPlayerStats started");

        Debug.Log("UserId: " + userId);
        if (string.IsNullOrEmpty(userId))
        {
            Debug.Log("UserId is null or empty, returning early.");
            return;
        }

        string playerName = null;
        try
        {
            Debug.Log("Fetching PlayerPrefs");
            playerName = PlayerPrefs.GetString("PlayerName", null);
            if (string.IsNullOrEmpty(playerName))
            {
                Debug.Log("Generating random player name");
                System.Random rng = new System.Random();
                playerName = "Player" + rng.Next(1000, 9999);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error fetching PlayerPrefs or generating name: " + e.Message);
        }

        try
        {
            Debug.Log("Starting snapshot retrieval...");
            DocumentReference docRef = db.Collection("leaderboard").Document(userId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            Debug.Log("Snapshot retrieval completed.");

            if (snapshot.Exists)
            {
                localPlayerStats = snapshot.ConvertTo<PlayerStats>();
                Debug.Log("Loaded player stats from snapshot.");
            }
            else
            {
                localPlayerStats = new PlayerStats(playerName);
                Debug.Log("Created new player stats.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading player stats from Firestore: " + e.Message);
        }
    }






    public async Task<QuerySnapshot> FetchLeaderboard(string sortByField)
    {
        Query leaderboardQuery = db.Collection("leaderboard").OrderByDescending(sortByField).Limit(10);
        return await leaderboardQuery.GetSnapshotAsync();
    }
}
