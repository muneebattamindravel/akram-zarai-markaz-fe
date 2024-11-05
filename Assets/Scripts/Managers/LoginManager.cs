using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public delegate void LoginEvent();
    public static LoginEvent onLoggedIn;

    string LOGIN_ROUTE = "login";

    public void ValidateCredentials(Login login, ResponseAction<Login> successAction, ResponseAction<Login> failAction = null)
    {
        APIManager.Instance.Post<Login>(LOGIN_ROUTE, login, (response) =>
        {
            successAction(response);
            if (onLoggedIn != null)
                onLoggedIn.Invoke();

        }, (response) => {
            if (failAction != null)
                failAction(response);
        });
    }
}
