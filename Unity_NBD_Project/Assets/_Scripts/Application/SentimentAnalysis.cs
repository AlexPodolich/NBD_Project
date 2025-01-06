using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using Firebase.Database;
using System;
using Firebase.Extensions;
using Debug = UnityEngine.Debug;

public class SentimentAnalysis : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI resultText;
    private string userId;
    private DatabaseReference dbReference;

    private void Awake()
    {
        userId = UserManager.Instance.UserId;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void AnalyzeComment()
    {
        string comment = inputField.text;
        StartCoroutine(SendRequest(comment));
    }

    //public void AnalyzeComment()
    //{
    //    string comment = inputField.text;
    //    string inputPath = "path_to_your_python_script_folder/input.txt";
    //    string outputPath = "path_to_your_python_script_folder/output.txt";

    //    // Записать комментарий в файл
    //    File.WriteAllText(inputPath, comment);

    //    // Запустить Python-скрипт
    //    ProcessStartInfo psi = new ProcessStartInfo
    //    {
    //        FileName = "python",
    //        Arguments = "path_to_your_python_script_folder/script.py",
    //        RedirectStandardOutput = true,
    //        UseShellExecute = false,
    //        CreateNoWindow = true
    //    };

    //    Process process = Process.Start(psi);
    //    process.WaitForExit();

    //    // Считать результат из output.txt
    //    if (File.Exists(outputPath))
    //    {
    //        string sentiment = File.ReadAllText(outputPath);
    //        resultText.text = $"Result: {sentiment}";
    //    }
    //    else
    //    {
    //        resultText.text = "Error: Output not found!";
    //    }
    //}

    private IEnumerator SendRequest(string comment)
    {
        string url = "http://127.0.0.1:8000/analyze";

        // Serialize the comment using Newtonsoft.Json
        string jsonData = JsonConvert.SerializeObject(new { text = comment });

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<Response>(request.downloadHandler.text);
            resultText.text = $"{response.result}";
            HistoryRow historyRow = new HistoryRow()
            {
                comment = comment,
                prediction = response.result
            };
            SaveToHistory(historyRow);
        }
        else
        {
            resultText.text = $"Error: {request.error}";
        }
    }

    [System.Serializable]
    public class Response
    {
        public string result;
    }

    public void SaveToHistory(HistoryRow historyRow)
    {
        string historyPath = $"users/{userId}/history";

        // Use a safe date format for Firebase keys
        string dataNameInFirebase = DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss") + " record";

        // Save data to Firebase
        dbReference.Child(historyPath).Child(dataNameInFirebase).SetRawJsonValueAsync(JsonUtility.ToJson(historyRow)).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("New history record saved successfully!");
            }
            else
            {
                Debug.LogError("Failed to save new history record: " + task.Exception);
            }
        });
    }
}
