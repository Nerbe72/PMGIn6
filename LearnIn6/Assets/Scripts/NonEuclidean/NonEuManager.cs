using UnityEngine;

public class NonEuManager : MonoBehaviour
{
    public Camera cameraB;
    public Material cameraMatB;

    public Camera cameraA;
    public Material cameraMatA;

    private void Awake()
    {
        Application.targetFrameRate = 120;
    }

    void Start()
    {
        if (cameraB.targetTexture != null)
        {
            cameraB.targetTexture.Release();
        }
        cameraB.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        cameraMatB.mainTexture = cameraB.targetTexture;

        if (cameraA.targetTexture != null)
        {
            cameraA.targetTexture.Release();
        }
        cameraA.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        cameraMatA.mainTexture = cameraA.targetTexture;
    }
}
