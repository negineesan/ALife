using UnityEngine;
using System.Runtime.InteropServices;
using System;

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

    void Start()
    {
        Debug.Log("Start called");
        alife = createAlife(0, 0); // 初期位置を(0, 0)に設定
        Debug.Log("alife: " + alife);

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
        Debug.Log("Update called");

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

                updateAlife(alife, foodX, foodY);

                // Update the position and scale of the sphere
                alifeObject.transform.position = new Vector3(getAlifeX(alife), 0, getAlifeY(alife));
                alifeObject.transform.localScale = new Vector3(getAlifeRadius(alife) * 2, getAlifeRadius(alife) * 2, getAlifeRadius(alife) * 2);
            }
        }
    }

    void OnDestroy()
    {
        destroyAlife(alife);
    }
}
