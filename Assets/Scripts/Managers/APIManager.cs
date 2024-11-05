using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;

public class APIManager : MonoBehaviour
{
    public static APIManager Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    string serverURL = "";
    string localServerURL = "http://localhost:6969/";
    string mobileServerURL = "http://13.213.139.143:6969/";

    private void Start()
    {
        if (ADM.I.isViewOnlyMode)
            serverURL = mobileServerURL;
        else
            serverURL = localServerURL;
    }

    public void Get<T>(string url, ResponseAction<T> successAction, ResponseAction<T> failAction = null)
    {
        StartCoroutine(GetWorker<T>(serverURL + url, successAction, failAction));
    }

    IEnumerator GetWorker<T>(string url, ResponseAction<T> successAction, ResponseAction<T> failAction = null)
    {
        Debug.Log(url);
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        GenerateResponse<T>(request, successAction, failAction);
    }

    public void Post<T>(string url, T body, ResponseAction<T> successAction, ResponseAction<T> failAction = null)
    {
        if (ADM.I.isViewOnlyMode && !url.Contains("login"))
        {
            Response<T> response = new Response<T>();
            response.status = ResponseStatus.FAIL;
            response.message = new Message(Constants.NotEnoughPermissions);
            GUIManager.Instance.ShowToast(Constants.Error, Constants.NotEnoughPermissions, false);
            failAction(response);
            return;
        }
        StartCoroutine(PostWorker<T>(serverURL + url, body, successAction, failAction));
    }

    IEnumerator PostWorker<T>(string url, T body, ResponseAction<T> successAction, ResponseAction<T> failAction = null)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        GenerateResponse<T>(request, successAction, failAction);
    }

    public void Patch<T>(string url, T body, ResponseAction<T> successAction, ResponseAction<T> failAction = null)
    {
        if (ADM.I.isViewOnlyMode)
        {
            Response<T> response = new Response<T>();
            response.status = ResponseStatus.FAIL;
            response.message = new Message(Constants.NotEnoughPermissions);
            GUIManager.Instance.ShowToast(Constants.Error, Constants.NotEnoughPermissions, false);
            failAction(response);
            return;
        }
        StartCoroutine(PatchWorker<T>(serverURL + url, body, successAction, failAction));
    }

    IEnumerator PatchWorker<T>(string url, T body, ResponseAction<T> successAction, ResponseAction<T> failAction = null)
    {
        var request = new UnityWebRequest(url, "PATCH");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        GenerateResponse<T>(request, successAction, failAction);
    }

    public void Delete<T>(string url, ResponseAction<T> successAction, ResponseAction<T> failAction = null)
    {
        if (ADM.I.isViewOnlyMode)
        {
            Response<T> response = new Response<T>();
            response.status = ResponseStatus.FAIL;
            response.message = new Message(Constants.NotEnoughPermissions);
            GUIManager.Instance.ShowToast(Constants.Error, Constants.NotEnoughPermissions, false);
            failAction(response);
            return;
        }
        StartCoroutine(DeleteWorker<T>(serverURL + url, successAction, failAction));
    }

    IEnumerator DeleteWorker<T>(string url, ResponseAction<T> successAction, ResponseAction<T> failAction = null)
    {
        UnityWebRequest request = UnityWebRequest.Delete(url);
        yield return request.SendWebRequest();
        GenerateResponse<T>(request, successAction, failAction);
    }

    void GenerateResponse<T>(UnityWebRequest request, ResponseAction<T> successAction, ResponseAction<T> failAction = null)
    {
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        Response<T> response = new Response<T>();
        if (request.responseCode == 200)
        {
            response.status = ResponseStatus.SUCCESS;
            if (request.downloadHandler != null)
            {
                Debug.Log("<color=\"green\">" + request.downloadHandler.text + "</color>");
                response.message = new Message(Constants.Done);
                response.data = JsonConvert.DeserializeObject<T>(request.downloadHandler.text, settings);
            }
            else
            {
                Debug.Log("<color=\"green\">DONE</color>");
                response.message = new Message(Constants.Done);
            }

            if (successAction != null) successAction(response);
        }
        else if (request.responseCode == 406)
        {
            Debug.Log("<color=\"orange\">" + request.downloadHandler.text + "</color>");
            response.status = ResponseStatus.FAIL;
            response.message = JsonConvert.DeserializeObject<Message>(request.downloadHandler.text, settings);
            if (failAction != null) failAction(response);
        }
        else
        {
            Debug.Log("<color=\"red\">" + request.error + "</color>");
            response.status = ResponseStatus.FAIL;
            response.message = new Message(request.error);
            if (failAction != null) failAction(response);
        }
    }
}
