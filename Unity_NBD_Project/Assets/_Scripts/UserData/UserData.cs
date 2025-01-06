using Firebase.Auth;
using UnityEngine;

public static class UserData
{
    public static void SaveUserData(FirebaseUser user)
    {
        PlayerPrefs.SetString("UserId", user.UserId);
        PlayerPrefs.SetString("UserEmail", user.Email);


        //PlayerPrefs.Save();

        Debug.Log("Data Saved Ended");
    }

    public static void LoadUserData()
    {
        if (PlayerPrefs.HasKey("UserId"))
        {
            string userId = PlayerPrefs.GetString("UserId");
            string userEmail = PlayerPrefs.GetString("UserEmail");

            Debug.Log("Loaded User: " + userId + ", " + userEmail);
        }
        else
        {
            Debug.Log("No user data found.");
        }
    }

    public static void ClearUserData()
    {
        PlayerPrefs.DeleteKey("UserId");
        PlayerPrefs.DeleteKey("UserEmail");

        PlayerPrefs.Save();
    }
}