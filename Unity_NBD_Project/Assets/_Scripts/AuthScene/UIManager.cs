using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject loginUI;
    public GameObject regUI;

    private void Awake()
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
    }

    private void Start()
    {
        OpenLoginScreen();
    }

    public void OpenLoginScreen()
    {
        loginUI.SetActive(true);
        regUI.SetActive(false);

        AuthManager.Instance.emailRegisterField.text = "";
        AuthManager.Instance.passwordRegisterField.text = "";
        AuthManager.Instance.passwordRegisterVerifyField.text = "";
        AuthManager.Instance.warningRegisterText.text = "";

    }

    public void OpenRegisterScreen()
    {
        loginUI.SetActive(false);
        regUI.SetActive(true);

        AuthManager.Instance.emailLoginField.text = "";
        AuthManager.Instance.passwordLoginField.text = "";
        AuthManager.Instance.warningLoginText.text = "";
        AuthManager.Instance.confirmLoginText.text = "";

    }
}
