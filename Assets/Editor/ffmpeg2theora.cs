using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEditor;
using System.Diagnostics;
using System;
using System.Runtime.InteropServices;
using System.Threading;

public class ffmpeg2theora : EditorWindow
{
    string filePath = "";
    [MenuItem("Tools/Video2OGV")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ffmpeg2theora), true, "视频转OGV");

    }

    void OnGUI()
    {
        GUILayout.Label(filePath, EditorStyles.boldLabel);
        if (GUILayout.Button("选择视频"))
        {
            OpenFileName ofn = new OpenFileName();

            ofn.structSize = Marshal.SizeOf(ofn);

            ofn.filter = "所有文件\0*.*\0\0";

            ofn.file = new string(new char[256]);

            ofn.maxFile = ofn.file.Length;

            ofn.fileTitle = new string(new char[64]);

            ofn.maxFileTitle = ofn.fileTitle.Length;
            string path = Application.streamingAssetsPath;
            path = path.Replace('/', '\\');
            //默认路径
            ofn.InitialDirectory = path;
            ofn.title = "选择视频";

            ofn.defExt = "mp4";//显示文件的类型
            //注意 一下项目不一定要全选 但是0x00000008项不要缺少
            ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR

            if (WindowDll.GetOpenFileName(ofn))
            {
                UnityEngine.Debug.Log("Selected file with full path: {0}" + ofn.file);
                filePath = ofn.file;
            }

            this.ShowNotification(new GUIContent("莫慌，编辑器会卡住一段时间！"));

            Timer timerClose = new Timer(new TimerCallback(ToOGV), this,3000, 0); 
            //ToOGV();  
        }
    }

    IEnumerator WaitTime(float time,Action action)
    {
        yield return new WaitForSeconds(time);
        action.Invoke();
    }

    void ToOGV( object obj)
    { 
        ProcessStartInfo ps = new ProcessStartInfo(@"D:\Github\unityffmpeg2theora\Assets\Editor\ffmpeg2theora.exe",
           filePath);
        ps.CreateNoWindow = true;
        ps.UseShellExecute = false;
        ps.RedirectStandardOutput = true;
        
        Process p = null;
        string output = "";
        try
        {
            p = Process.Start(ps);
            output = p.StandardOutput.ReadToEnd(); 
            p.WaitForExit();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.ToString());
        }
        finally 
        { 
            if (p != null)
            {
                UnityEngine.Debug.Log(output);
                p.Close(); 
            }  
        }
    }

}


[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]

public class OpenFileName
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String InitialDirectory = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

public class WindowDll
{
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
    public static bool GetOpenFileName1([In, Out] OpenFileName ofn)
    {
        return GetOpenFileName(ofn);
    }
}
