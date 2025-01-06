using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Extensions;
using Firebase.Database;
public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance;
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    private DatabaseReference databaseReference;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;
    public Button loginBtn;

    //Register variables
    [Header("Register")]
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;
    public TMP_Text confirmRegisterText;
    public Button registerBtn;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Debug.Log("Instance already exists!");
            Destroy(this);
        }

        // Initialize Firebase Database reference
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }

            // Ensure Firebase is initialized before navigating
            if (task.IsCompleted && auth.CurrentUser != null)
            {
                Debug.Log("SaveUserData");
                // Save user data in PlayerPrefs
                UserData.SaveUserData(User);

            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
    }

    private void OnDestroy()
    {
        auth = null;
    }

    private void OnApplicationQuit()
    {
        if (auth.CurrentUser != null)
        {
            auth.SignOut();

            UserData.ClearUserData();
            Debug.Log("User signed out.");
        }
        else
        {
            Debug.Log("No user is currently signed in.");
        }
    }

    private void Start()
    {
        loginBtn.onClick.AddListener(OnLoginButtonClick);
        registerBtn.onClick.AddListener(OnRegisterButtonClick);
    }

    //Function for the login button
    private async void OnLoginButtonClick()
    {
        string email = emailLoginField.text;
        string password = passwordLoginField.text;

        confirmLoginText.text = "Logging in...";
        var result = await LoginUserAsync(email, password);

        confirmLoginText.text = result ? "Login successful!" : "Login failed";
        warningLoginText.text = result ? "" : "Invalid email or password";
    }

    //Function for the register button
    public void OnRegisterButtonClick()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text));
    }

    public async Task<bool> LoginUserAsync(string email, string password)
    {
        try
        {
            var userCredential = await auth.SignInWithEmailAndPasswordAsync(email, password);

            Debug.Log("Login successful. Welcome, " + userCredential.User.DisplayName);

            UserData.SaveUserData(userCredential.User);

            // Start the coroutine to wait and navigate
            StartCoroutine(NavigateToApp(1f)); // 1 second delay
            return true;
        }
        catch (FirebaseException e)
        {
            Debug.LogError("Login failed: " + e.Message);
            return false;
        }
    }

    public IEnumerator NavigateToApp(float delay)
    {
        Debug.Log("NavigateToApp");
        UserManager.Instance.LoadData();
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        SceneManager.LoadScene("Application"); // Navigate to Application Scene
    }

    private IEnumerator Register(string email, string password)
    {
        if (email == "")
        {
            //If the email field is blank show a warning
            warningRegisterText.text = "Missing Email";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            AddUserDataToDatabase(RegisterTask.Result.User.UserId, email);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result.User;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = email };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    Task ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        confirmRegisterText.text = "Registration successful!";
                        warningRegisterText.text = "";

                        // Wait a moment to let the user see the confirmation text
                        yield return new WaitForSeconds(1.0f); // Adjust the delay as needed
                        confirmRegisterText.text = "";
                        UIManager.Instance.OpenLoginScreen();
                        warningRegisterText.text = "";
                    }
                }
            }
        }
    }

    private void AddUserDataToDatabase(string userId, string email)
    {
        var userProfileInfo = new UserProfileInfo
        {
            email = email
        };

        string jsonProfile = JsonUtility.ToJson(userProfileInfo);

        // Define path to store data
        string userProfilePath = $"users/{userId}/profileInfo";

        // Push data to Firebase
        databaseReference.Child(userProfilePath).SetRawJsonValueAsync(jsonProfile).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("User Profile data added to database successfully.");
            }
            else
            {
                Debug.LogError("Failed to save user data to database: " + task.Exception);
            }
        });

        UserManager.Instance.userEmail = email;
        Debug.Log("User profile data added to database.");
    }
}
