﻿#if UNITY_WSA && !UNITY_EDITOR && ENABLE_WINMD_SUPPORT //|| CT_DEVELOP
using UnityEngine;
using System;

namespace Crosstales.FB.Wrapper
{
   /// <summary>File browser implementation for WSA (UWP).</summary>
   public class FileBrowserWSA : BaseFileBrowser
   {
      #region Variables

      private static FileBrowserWSAImpl fbWsa;

      private string allFilesText;

      #endregion


      #region Constructor

      /// <summary>
      /// Constructor for a WSA file browser.
      /// </summary>
      public FileBrowserWSA()
      {
         allFilesText = FileBrowser.Instance.TextAllFiles;

         if (fbWsa == null)
         {
            if (Util.Constants.DEV_DEBUG)
               Debug.Log("Initializing WSA file browser...");

            fbWsa = new FileBrowserWSAImpl();

            fbWsa.DEBUG = Util.Config.DEBUG;
         }
      }

      #endregion


      #region Implemented methods

      public override bool canOpenFile => true;
      public override bool canOpenFolder => true;
      public override bool canSaveFile => true;

      public override bool canOpenMultipleFiles => FileBrowserWSAImpl.canOpenMultipleFiles;

      public override bool canOpenMultipleFolders => FileBrowserWSAImpl.canOpenMultipleFolders;

      public override bool isPlatformSupported => Util.Helper.isWSABasedPlatform; // || Util.Helper.isWindowsEditor;

      public override bool isWorkingInEditor => false;

      public override string CurrentOpenSingleFile { get; set; }
      public override string[] CurrentOpenFiles { get; set; }
      public override string CurrentOpenSingleFolder { get; set; }
      public override string[] CurrentOpenFolders { get; set; }
      public override string CurrentSaveFile { get; set; }

      public override byte[] CurrentOpenSingleFileData
      {
         get
         {
            Debug.LogWarning("'CurrentOpenSingleFileData' is currently not supported under WSA (UWP).");

            return null;
         }
      }

      public override byte[] CurrentSaveFileData
      {
         get
         {
            Debug.LogWarning("'CurrentSaveFileData' is currently not supported under WSA (UWP).");

            return null;
         }

         set => Debug.LogWarning("'CurrentSaveFileData' is currently not supported under WSA (UWP).");
      }

      public override string[] OpenFiles(string title, string directory, string defaultName, bool multiselect, params ExtensionFilter[] extensions)
      {
         if (Util.Config.DEBUG && !string.IsNullOrEmpty(title))
            Debug.LogWarning("'title' is not supported under WSA (UWP).");

         if (!string.IsNullOrEmpty(directory))
            Debug.LogWarning("'directory' is not supported under WSA (UWP).");

         if (!string.IsNullOrEmpty(defaultName))
            Debug.LogWarning("'defaultName' is not supported under WSA (UWP).");

         fbWsa.isBusy = true;
         UnityEngine.WSA.Application.InvokeOnUIThread(() => { fbWsa.OpenFiles(getExtensionsFromExtensionFilters(extensions), multiselect); }, false);

         do
         {
            //wait
         } while (fbWsa.isBusy);

         string[] paths = fbWsa.Selection.ToArray();

         if (paths != null)
         {
            CurrentOpenFiles = paths;
            CurrentOpenSingleFile = paths[0];
         }

         return paths;
      }

      public override string[] OpenFolders(string title, string directory, bool multiselect)
      {
         if (Util.Config.DEBUG && !string.IsNullOrEmpty(title))
            Debug.LogWarning("'title' is not supported under WSA (UWP).");

         if (!string.IsNullOrEmpty(directory))
            Debug.LogWarning("'directory' is not supported under WSA (UWP).");

         if (multiselect)
            Debug.LogWarning("'multiselect' for folders is not supported under WSA (UWP).");

         fbWsa.isBusy = true;
         UnityEngine.WSA.Application.InvokeOnUIThread(() => { fbWsa.OpenSingleFolder(); }, false);

         do
         {
            //wait
         } while (fbWsa.isBusy);

         string[] paths = fbWsa.Selection.ToArray();

         if (paths != null)
         {
            CurrentOpenFolders = paths;
            CurrentOpenSingleFolder = paths[0];
         }

         return paths;
      }

      public override string SaveFile(string title, string directory, string defaultName, params ExtensionFilter[] extensions)
      {
         if (Util.Config.DEBUG && !string.IsNullOrEmpty(title))
            Debug.LogWarning("'title' is not supported under WSA (UWP).");

         if (!string.IsNullOrEmpty(directory))
            Debug.LogWarning("'directory' is not supported under WSA (UWP).");

         fbWsa.isBusy = true;
         UnityEngine.WSA.Application.InvokeOnUIThread(() => { fbWsa.SaveFile(defaultName, getExtensionsFromExtensionFilters(extensions)); }, false);

         do
         {
            //wait
         } while (fbWsa.isBusy);

         if (fbWsa.Selection.Count > 0)
         {
            CurrentSaveFile = fbWsa.Selection[0];

            return CurrentSaveFile;
         }

         return null;
      }

      public override void OpenFilesAsync(string title, string directory, string defaultName, bool multiselect, ExtensionFilter[] extensions, Action<string[]> cb)
      {
         cb?.Invoke(OpenFiles(title, directory, defaultName, multiselect, extensions));
      }

      public override void OpenFoldersAsync(string title, string directory, bool multiselect, Action<string[]> cb)
      {
         cb?.Invoke(OpenFolders(title, directory, multiselect));
      }

      public override void SaveFileAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions, Action<string> cb)
      {
         cb?.Invoke(SaveFile(title, directory, defaultName, extensions));
      }

      #endregion


      #region Private methods

      private System.Collections.Generic.List<Extension> getExtensionsFromExtensionFilters(ExtensionFilter[] extensions)
      {
         System.Collections.Generic.List<Extension> list = new System.Collections.Generic.List<Extension>();

         if (extensions != null && extensions.Length > 0)
         {
            foreach (ExtensionFilter filter in extensions)
            {
               list.Add(new Extension(filter.Name, filter.Extensions));

               //Debug.Log($"filter.Extensions: {filter.Extensions.CTDump()}");
            }
         }
         else
         {
            list.Add(new Extension(allFilesText, "*"));
         }

         if (Util.Config.DEBUG)
            Debug.Log($"getExtensionsFromExtensionFilters: {list.CTDump()}");

         return list;
      }

      #endregion
   }
}
#endif
// © 2018-2021 crosstales LLC (https://www.crosstales.com)