using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using TMPro;

public class Screen_Login : MonoBehaviour
{
    public TMP_InputField input_username, input_password;
    public MRButton button_login;
    public GameObject preLoader;

    private void Awake()
    {
        preLoader.SetActive(false);
        input_username.text = "akram";
        input_password.text = "akram";
    }

    private void OnEnable()
    {
        KeyboardManager.enterPressed += OnEnterKeyPressed;
        Debug.Log(Application.persistentDataPath);
    }

    private void OnDisable()
    {
        KeyboardManager.enterPressed -= OnEnterKeyPressed;
    }

    public void OnEnterKeyPressed()
    {
        MRSoundManager.Instance.Play(SoundType.BUTTON_CLICK);
        Button_LoginClicked();
    }

    public void Button_LoginClicked()
    {
        if (string.IsNullOrEmpty(input_username.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.UsernameEmpty, false);
            return;
        }
        else if (string.IsNullOrEmpty(input_password.text))
        {
            GUIManager.Instance.ShowToast(Constants.Error, Constants.PasswordEmpty, false);
            return;
        }

        button_login.interactible = false;
        preLoader.SetActive(true);

        LoginManager.Instance.ValidateCredentials(new Login(input_username.text, input_password.text),
            (response) =>
            {
                StartCoroutine(LoginToApp());
            },
            (response) =>
            {
                GUIManager.Instance.ShowToast(Constants.Failed, response.message.message, false);
                button_login.interactible = true;
                preLoader.SetActive(false);
            });
    }

    IEnumerator LoginToApp()
    {
        yield return new WaitForSecondsRealtime(1f);
        GUIManager.Instance.ShowToast(Constants.Success, Constants.loggedIn);
        preLoader.SetActive(false);
        NavigationPanel.Instance.SlideIn();
        NavigationPanel.Instance.NavigateTo(MRScreenName.MainDashboard);
        TopBar.Instance.Show();
    }
}
