using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using System.IO;

[CustomEditor (typeof(MREditorUtil))]
public class MREditorUtil : Editor
{

    public static List<GameObject> listOfChildren = new List<GameObject>();

    [MenuItem ("MREditorUtilities/Selection/Toggle Selected Objects %#a")]
	static void Toggle ()
	{
		foreach (GameObject go in Selection.gameObjects) {
			if (go.activeSelf)
				go.SetActive (false);
			else
				go.SetActive (true);

            
		}
	}

	[MenuItem ("MREditorUtilities/Delete Preferences %#-")]
	static void DeletePreferences ()
	{
		PlayerPrefs.DeleteAll ();
		PlayerPrefs.Save ();

		Caching.ClearCache ();

	}


	[MenuItem ("MREditorUtilities/GameObject/Reset Local Position %#r")]
	static void ResetPosition ()
	{
		foreach (GameObject go in Selection.gameObjects) {
			go.transform.localPosition = Vector3.zero;
		}
	}

	static void ReadyCanvas()
	{
		EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
	}
}