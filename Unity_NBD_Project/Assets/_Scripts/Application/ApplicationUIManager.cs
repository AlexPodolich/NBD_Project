using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ApplicationUIManager : MonoBehaviour
{
    public TextMeshProUGUI currentUserText;
    public GameObject mainPage;
    public GameObject historyPage;

    public TMP_InputField givenText;
    public TextMeshProUGUI resultValueField;

    void Awake()
    {
        currentUserText.text = "Current User: " + UserManager.Instance.userEmail;
    }

    private void Start()
    {
        BackToMainPage();
    }

    public void BackToMainPage()
    {
        mainPage.SetActive(true);
        historyPage.SetActive(false);

        givenText.text = "";
        resultValueField.text = "";
    }

    public void OpenHistoryPage()
    {
        mainPage.SetActive(false);
        historyPage.SetActive(true);
    }

    public void CloseApp()
    {
        Application.Quit();
    }
}
