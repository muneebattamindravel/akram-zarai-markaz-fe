﻿#if UNITY_STANDALONE_OSX || CT_DEVELOP
using System;
using UnityEngine;

namespace Crosstales.FB.Wrapper
{
   /// <summary>File browser implementation for macOS.</summary>
   public class FileBrowserMac : BaseFileBrowser
   {
      #region Variables

      private static FileBrowserMac instance;

      private static Action<string[]> _openFileCb;
      private static Action<string[]> _openFolderCb;
      private static Action<string> _saveFileCb;

      private const char splitChar = (char)28;

      #endregion


      #region Constructor

      public FileBrowserMac()
      {
         instance = this;
      }

      #endregion


      #region Implemented methods

      public override bool canOpenFile => true;
      public override bool canOpenFolder => true;
      public override bool canSaveFile => true;

      public override bool canOpenMultipleFiles => true;

      public override bool canOpenMultipleFolders => true;

      public override bool isPlatformSupported => Util.Helper.isMacOSPlatform;

      public override bool isWorkingInEditor => false;

      public override string CurrentOpenSingleFile { get; set; }
      public override string[] CurrentOpenFiles { get; set; }
      public override string CurrentOpenSingleFolder { get; set; }
      public override string[] CurrentOpenFolders { get; set; }
      public override string CurrentSaveFile { get; set; }

      public override string[] OpenFiles(string title, string directory, string defaultName, bool multiselect, params ExtensionFilter[] extensions)
      {
         if (!string.IsNullOrEmpty(defaultName))
            Debug.LogWarning("'defaultName' is not supported under macOS.");

         //directory += System.IO.Path.DirectorySeparatorChar + defaultName; //TODO works?

         string paths = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(Mac.NativeMethods.DialogOpenFilePanel(title, directory, getFilterFromFileExtensionList(extensions), multiselect));

         if (string.IsNullOrEmpty(paths))
            return null;

         string[] pathArray = paths.Split(splitChar);

         CurrentOpenFiles = pathArray;
         CurrentOpenSingleFile = pathArray[0];

         return CurrentOpenFiles;
      }

      public override string[] OpenFolders(string title, string directory, bool multiselect)
      {
         string paths = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(Mac.NativeMethods.DialogOpenFolderPanel(title, directory, multiselect));

         if (string.IsNullOrEmpty(paths))
            return null;

         string[] pathArray = paths.Split(splitChar);

         CurrentOpenFolders = pathArray;
         CurrentOpenSingleFolder = pathArray[0];

         return CurrentOpenFolders;
      }

      public override string SaveFile(string title, string directory, string defaultName, params ExtensionFilter[] extensions)
      {
         string path = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(Mac.NativeMethods.DialogSaveFilePanel(title, directory, defaultName, getFilterFromFileExtensionList(extensions)));

         if (string.IsNullOrEmpty(path))
            return null;

         CurrentSaveFile = path;

         return CurrentSaveFile;
      }

      public override void OpenFilesAsync(string title, string directory, string defaultName, bool multiselect, ExtensionFilter[] extensions, Action<string[]> cb)
      {
         if (!string.IsNullOrEmpty(defaultName))
            Debug.LogWarning("'defaultName' is not supported under macOS.");

         //directory += System.IO.Path.DirectorySeparatorChar + defaultName; //TODO works?

         _openFileCb = cb;
         Mac.NativeMethods.DialogOpenFilePanelAsync(
            title,
            directory,
            getFilterFromFileExtensionList(extensions),
            multiselect,
            openFileCb);
         //(string result) => { _openFileCb.Invoke(result.Split(splitChar)); });
      }

      public override void OpenFoldersAsync(string title, string directory, bool multiselect, Action<string[]> cb)
      {
         _openFolderCb = cb;
         Mac.NativeMethods.DialogOpenFolderPanelAsync(
            title,
            directory,
            multiselect,
            openFolderCb);
         //(string result) => { _openFolderCb.Invoke(result.Split(splitChar)); });
      }

      public override void SaveFileAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions, Action<string> cb)
      {
         _saveFileCb = cb;
         Mac.NativeMethods.DialogSaveFilePanelAsync(
            title,
            directory,
            defaultName,
            getFilterFromFileExtensionList(extensions),
            saveFileCb);
         //(string result) => { _saveFileCb.Invoke(result); });
      }

      #endregion


      #region Private methods

      [AOT.MonoPInvokeCallback(typeof(AsyncCallback))]
      private static void openFileCb(string result)
      {
         if (string.IsNullOrEmpty(result))
         {
            _openFileCb?.Invoke(null);
         }
         else
         {
            string[] pathArray = result.Split(splitChar);

            instance.CurrentOpenFiles = pathArray;
            instance.CurrentOpenSingleFile = pathArray[0];

            _openFileCb?.Invoke(pathArray);
         }
      }

      [AOT.MonoPInvokeCallback(typeof(AsyncCallback))]
      private static void openFolderCb(string result)
      {
         if (string.IsNullOrEmpty(result))
         {
            _openFolderCb?.Invoke(null);
         }
         else
         {
            string[] pathArray = result.Split(splitChar);

            instance.CurrentOpenFolders = pathArray;
            instance.CurrentOpenSingleFolder = pathArray[0];

            _openFolderCb?.Invoke(pathArray);
         }
      }

      [AOT.MonoPInvokeCallback(typeof(AsyncCallback))]
      private static void saveFileCb(string result)
      {
         if (string.IsNullOrEmpty(result))
         {
            _saveFileCb?.Invoke(null);
         }
         else
         {
            instance.CurrentSaveFile = result;

            _saveFileCb?.Invoke(result);
         }
      }

      private static string getFilterFromFileExtensionList(ExtensionFilter[] extensions)
      {
         if (extensions?.Length > 0)
         {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int xx = 0; xx < extensions.Length; xx++)
            {
               ExtensionFilter filter = extensions[xx];

               sb.Append(filter.Name);
               sb.Append(";");

               for (int ii = 0; ii < filter.Extensions.Length; ii++)
               {
                  sb.Append(filter.Extensions[ii]);

                  if (ii + 1 < filter.Extensions.Length)
                     sb.Append(",");
               }

               if (xx + 1 < extensions.Length)
                  sb.Append("|");
            }

            if (Util.Config.DEBUG)
               Debug.Log($"getFilterFromFileExtensionList: {sb}");

            return sb.ToString();
         }

         return string.Empty;
      }

      #endregion
   }
}

namespace Crosstales.FB.Wrapper.Mac
{
   /// <summary>Native methods (bridge to macOS).</summary>
   internal static class NativeMethods
   {
      [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
      public delegate void AsyncCallback(string path);

      [System.Runtime.InteropServices.DllImport("FileBrowser")]
      internal static extern IntPtr DialogOpenFilePanel(string title, string directory, string extension, bool multiselect);

      [System.Runtime.InteropServices.DllImport("FileBrowser")]
      internal static extern IntPtr DialogOpenFolderPanel(string title, string directory, bool multiselect);

      [System.Runtime.InteropServices.DllImport("FileBrowser")]
      internal static extern IntPtr DialogSaveFilePanel(string title, string directory, string defaultName, string extension);

      [System.Runtime.InteropServices.DllImport("FileBrowser")]
      internal static extern void DialogOpenFilePanelAsync(string title, string directory, string extension, bool multiselect, AsyncCallback callback);

      [System.Runtime.InteropServices.DllImport("FileBrowser")]
      internal static extern void DialogOpenFolderPanelAsync(string title, string directory, bool multiselect, AsyncCallback callback);

      [System.Runtime.InteropServices.DllImport("FileBrowser")]
      internal static extern void DialogSaveFilePanelAsync(string title, string directory, string defaultName, string extension, AsyncCallback callback);
   }
}
#endif
// © 2017-2021 crosstales LLC (https://www.crosstales.com)