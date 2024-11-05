using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
	public List<GameObject> panels;
	public Stack<GameObject> navPanel = new Stack<GameObject> ();

	public static GUIManager Instance;

	void Awake ()
	{
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

		panels = new List<GameObject> ();
		foreach (MRScreen screen in Resources.FindObjectsOfTypeAll(typeof(MRScreen)) as MRScreen[])
			panels.Add (screen.gameObject);

		Application.targetFrameRate = 60;
	}

    void Start ()
	{
		StartGUI ();
	}

	public void Back ()
	{
		if (navPanel != null && navPanel.Count > 0) {
			GameObject toHidePanel = navPanel.Pop ();
			toHidePanel.SetActive (false);
			showCurrentPanel ();
		}
	}

	public GameObject CURRENTPANEL {
		get {
			GameObject currentPanel;
			currentPanel = navPanel.Peek ();
			return currentPanel;
		}
		set {
		}
	}

	public MRScreenName CURRENTSCREENNAME {
		get {
			GameObject currentPanel;
			currentPanel = navPanel.Peek ();
			return currentPanel.GetComponent<MRScreen> ().screenName;
		}
	}

	public void hideCurrentPanel ()
	{
		GameObject currentPanel;
		currentPanel = navPanel.Peek ();
		currentPanel.SetActive (false);
	}

	private void clearStack ()
	{
		foreach (GameObject panel in navPanel)
			panel.SetActive (false);
		navPanel.Clear ();
	}

	private void showCurrentPanel ()
	{
	
		if (navPanel != null && navPanel.Count > 0) {
			GameObject currentPanel;
			currentPanel = navPanel.Peek ();
			currentPanel.SetActive (true);
		} else {
			//Debug.Log ("MRERROR - nav panel is null or zero count in ShowCurrentPanel");
		}

	}

	public void OpenScreenExplicitly (MRScreenName screenName)
	{
		GameObject screenToOpen = panels.Find (asd => asd.GetComponent<MRScreen> ().screenName == screenName);
		if (navPanel != null && navPanel.Count > 0) {
			if (navPanel.Peek ().name != screenToOpen.name)
				OpenScreen (screenToOpen);
		} else {
			//Debug.Log ("MRERROR - nav panel is null or zero count in OpenScreenExplicitly");
		}
	}

	public void OpenScreen (GameObject screenToOpen)
	{
		bool fullScreen = screenToOpen.GetComponent<MRScreen> ().fullScreen;

		int index = panels.FindIndex (p => p.GetComponent<MRScreen> ().screenName == screenToOpen.GetComponent<MRScreen> ().screenName);
		if (fullScreen) {
			CURRENTPANEL.SetActive (false);
		}

		navPanel.Push (panels [index]);
		showCurrentPanel ();

	}

	private void StartGUI ()
	{
		foreach (GameObject panel in panels)
			panel.SetActive (false);
		navPanel.Clear ();

			GameObject firstScreen = panels.Find(asd => asd.GetComponent<MRScreen>().firstScreen);
			navPanel.Push(firstScreen);
		

		showCurrentPanel ();
	}

	public GameObject toastPrefab;
	public void ShowToast(string title, string message, bool isPositive = true)
    {
		GameObject toast = GameObject.Instantiate(toastPrefab);
		toast.GetComponent<Toast>().ShowToast(title, message, isPositive);
    }
}
// end of class
