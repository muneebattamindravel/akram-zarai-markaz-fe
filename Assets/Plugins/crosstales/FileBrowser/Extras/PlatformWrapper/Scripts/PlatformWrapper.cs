using UnityEngine;
using System;

namespace Crosstales.FB.Tool
{
   /// <summary>Allows to configure wrappers per platform.</summary>
   //[ExecuteInEditMode]
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_tool_1_1_paralanguage.html")] //TODO update URL
   public class PlatformWrapper : MonoBehaviour
   {
      [Header("Configuration Settings"), Tooltip("Platform specific wrapper for the app (empty wrapper = default of the OS).")]
      public PlatformWrapperTuple[] Configuration;

      [Header("Default"), Tooltip("Default wrapper of the app (empty = default of the OS).")] public Wrapper.BaseCustomFileBrowser DefaultWrapper;

      [Header("Editor"), Tooltip("Use the default wrapper inside the Editor (default: false).")] public bool UseDefault;

      private void Start()
      {
         bool found = false;

         if (!Crosstales.FB.Util.Helper.isEditor && !UseDefault)
         {
            Crosstales.Common.Model.Enum.Platform currentPlatform = Util.Helper.CurrentPlatform;

            foreach (PlatformWrapperTuple config in Configuration)
            {
               if (config.Platform == currentPlatform)
               {
                  if (config.CustomWrapper == null)
                  {
                     FileBrowser.Instance.CustomMode = false;
                  }
                  else
                  {
                     FileBrowser.Instance.CustomWrapper = config.CustomWrapper;
                     FileBrowser.Instance.CustomMode = true;
                  }

                  found = true;
                  break;
               }
            }
         }

         if (!found)
         {
            if (DefaultWrapper == null)
            {
               FileBrowser.Instance.CustomMode = false;
            }
            else
            {
               FileBrowser.Instance.CustomWrapper = DefaultWrapper;
               FileBrowser.Instance.CustomMode = true;
            }
         }
      }
   }

   [Serializable]
   public class PlatformWrapperTuple
   {
      public Crosstales.Common.Model.Enum.Platform Platform;
      public Wrapper.BaseCustomFileBrowser CustomWrapper;
   }
}
// © 2021 crosstales LLC (https://www.crosstales.com)