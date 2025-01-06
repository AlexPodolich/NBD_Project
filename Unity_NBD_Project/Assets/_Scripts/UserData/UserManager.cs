using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance { get; private set; }
    public string userEmail;
    public string UserId;

    private void Awake()
    {
        // Implement Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
        LoadData();
    }

    public void LoadData()
    {
        UserData.LoadUserData();

        userEmail = PlayerPrefs.GetString("UserEmail");
        UserId = PlayerPrefs.GetString("UserId");
    }
}
