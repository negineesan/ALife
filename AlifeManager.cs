using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.IO;

public class AlifeManager : MonoBehaviour
{
    [DllImport("alife", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr createAlife(float x, float y);

    [DllImport("alife", CallingConvention = CallingConvention.Cdecl)]
    private static extern void updateAlife(IntPtr alife, float foodX, float foodY);

    [DllImport("alife", CallingConvention = CallingConvention.Cdecl)]
    private static extern void destroyAlife(IntPtr alife);

   [DllImport("alife", CallingConvention = CallingConvention.Cdecl)]
    private static extern void debugLog(string message);

    [DllImport("alife", CallingConvention = CallingConvention.Cdecl)]
    private static extern float getAlifeX(IntPtr alife);

    [DllImport("alife", CallingConvention = CallingConvention.Cdecl)]
    private static extern float getAlifeY(IntPtr alife);

    [DllImport("alife", CallingConvention = CallingConvention.Cdecl)]
    private static extern float getAlifeRadius(IntPtr alife);

    [DllImport("alife", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool isDead(IntPtr alife);

    private IntPtr alife;
    private GameObject alifeObject;
    private bool isAlifeDead = false;
    private StreamWriter logFile;

    void Start()
    {
        // Initialize log file
        string logFilePath = "G:/VSCode_src/c++/alife/ALife_log.txt"; // Absolute path
        logFile = new StreamWriter(logFilePath, false); // Overwrite existing file
        logFile.WriteLine("Log started at " + DateTime.Now);
        WriteLog("Start called");

        alife = createAlife(0, 0); // 初期位置を(0, 0)に設定
        WriteLog("alife: " + alife);

        // Create a sphere primitive to represent the Alife
        alifeObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        alifeObject.name = "Alife";

        // Set the initial position and scale of the sphere
        alifeObject.transform.position = new Vector3(0, 0, 0); // 初期位置を(0, 0, 0)に設定
        alifeObject.transform.localScale = new Vector3(getAlifeRadius(alife) * 2, getAlifeRadius(alife) * 2, getAlifeRadius(alife) * 2);

        // Set the main camera's position and rotation to match the scene view
#if UNITY_EDITOR
        if (UnityEditor.SceneView.lastActiveSceneView != null)
        {
            Camera.main.transform.position = UnityEditor.SceneView.lastActiveSceneView.camera.transform.position;
            Camera.main.transform.rotation = UnityEditor.SceneView.lastActiveSceneView.camera.transform.rotation;
        }
#endif
    }

    void Update()
    {
        WriteLog("Update called");

        if (!isAlifeDead)
        {
            if (isDead(alife))
            {
                // Change the color of the sphere to black when Alife is dead
                alifeObject.GetComponent<Renderer>().material.color = Color.black;
                isAlifeDead = true;
            }
            else
            {
                // Generate random food position (replace with your actual food generation logic)
                float foodX = UnityEngine.Random.Range(-2.0f, 2.0f);
                float foodY = UnityEngine.Random.Range(-2.0f, 2.0f);

                Debug.Log("foodX: " + foodX + ", foodY: " + foodY + ", deltaTime: " + Time.deltaTime + ", alife.x: " + getAlifeX(alife) + ", alife.y: " + getAlifeY(alife)); // 変更

                updateAlife(alife, foodX, foodY);

                // Update the position and scale of the sphere
                alifeObject.transform.position = new Vector3(getAlifeX(alife), 0, getAlifeY(alife));
                alifeObject.transform.localScale = new Vector3(getAlifeRadius(alife) * 2, getAlifeRadius(alife) * 2, getAlifeRadius(alife) * 2);
            }
        }
    }

    void OnDestroy()
    {
        WriteLog("OnDestroy called");
        destroyAlife(alife);
        logFile.Close();
    }

    private void WriteLog(string message)
    {
        //Debug.Log(message); // Unityのコンソールへの出力を削除
        logFile.WriteLine(DateTime.Now + " - " + message);
        debugLog(message); // alife.dllのdebugLog関数を呼び出す
    }
}
