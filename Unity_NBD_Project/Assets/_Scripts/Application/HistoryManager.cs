using Firebase.Database;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoryManager : MonoBehaviour
{
    private DatabaseReference dbReference; // Firebase database reference
    public string userId; // User ID for the current user

    public GameObject historyListContainer; // Parent container for history entries
    public GameObject historyPrefab; // Prefab for a history entry item

    private void Awake()
    {
        userId = UserManager.Instance.UserId;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Load and display history data
    public void LoadHistory()
    {
        StartCoroutine(LoadHistoryData());
    }

    private IEnumerator LoadHistoryData()
    {
        // Firebase operation to fetch history data
        var serverData = dbReference.Child("users").Child(userId).Child("history").GetValueAsync();

        // Wait until the data is fetched
        yield return new WaitUntil(() => serverData.IsCompleted);

        if (serverData.IsFaulted)
        {
            Debug.LogError("Failed to load history data.");
            yield break;
        }

        DataSnapshot snapshot = serverData.Result;

        // Check if the snapshot contains any data
        if (snapshot.ChildrenCount == 0)
        {
            Debug.LogWarning("No history records found for this user.");
            yield break;
        }

        // Clear existing history entries
        foreach (Transform child in historyListContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Loop through history records and create UI entries
        foreach (DataSnapshot childSnapshot in snapshot.Children)
        {
            string key = childSnapshot.Key; // The record key (e.g., "2024-12-22_15-30-00 record")
            string date = ExtractDateFromKey(key);

            // Deserialize the history record from JSON
            HistoryRow historyRow = JsonUtility.FromJson<HistoryRow>(childSnapshot.GetRawJsonValue());

            // Create a new history entry UI
            CreateHistoryUI(historyRow.comment, historyRow.prediction, date);
        }
    }

    // Extract the date in "dd MMM yyyy" format from the Firebase key
    private string ExtractDateFromKey(string key)
    {
        // Key format: "yyyy-MM-dd_HH-mm-ss record"
        if (key.Contains(" "))
        {
            string[] parts = key.Split(' ');
            string datePart = parts[0]; // yyyy-MM-dd_HH-mm-ss

            // Convert to DateTime
            if (DateTime.TryParseExact(datePart, "yyyy-MM-dd_HH-mm-ss", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
            {
                // Format the date as "dd MMM yyyy"
                return dateTime.ToString("dd MMM yyyy");
            }
        }
        return "Unknown Date";
    }


    // Create a new history UI element
    private void CreateHistoryUI(string comment, string prediction, string date)
    {
        GameObject newHistoryEntry = Instantiate(historyPrefab, historyListContainer.transform);

        // Set the text fields in the prefab
        TextMeshProUGUI commentText = newHistoryEntry.transform.Find("CommentValue").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI predictionText = newHistoryEntry.transform.Find("PredictionValue").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI dateText = newHistoryEntry.transform.Find("DateValue").GetComponent<TextMeshProUGUI>();

        commentText.text = comment;
        predictionText.text = prediction;
        dateText.text = date;

        // Optional: Force layout rebuild to ensure proper display
        LayoutRebuilder.ForceRebuildLayoutImmediate(historyListContainer.GetComponent<RectTransform>());
    }
}
