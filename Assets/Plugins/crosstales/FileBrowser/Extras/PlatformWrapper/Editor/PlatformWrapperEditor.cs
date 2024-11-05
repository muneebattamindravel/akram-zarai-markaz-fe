#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Crosstales.FB.EditorExtension
{
   /// <summary>Custom editor for the 'PlatformWrapper'-class.</summary>
   [CustomEditor(typeof(Tool.PlatformWrapper))]
   [CanEditMultipleObjects]
   public class PlatformWrapperEditor : Editor
   {
      #region Variables

      private Tool.PlatformWrapper script;

      #endregion


      #region Properties

      public static bool isPrefabInScene => GameObject.Find("PlatformWrapper") != null;

      #endregion


      #region Editor methods

      private void OnEnable()
      {
         script = (Tool.PlatformWrapper)target;
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         if (script.isActiveAndEnabled)
         {
            //do something
         }
         else
         {
            EditorUtil.EditorHelper.SeparatorUI();
            EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
         }
      }

      #endregion
   }
}
#endif
// © 2021 crosstales LLC (https://www.crosstales.com)