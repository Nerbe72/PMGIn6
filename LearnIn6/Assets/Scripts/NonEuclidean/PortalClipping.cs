using UnityEngine;

public class PortalClipping : MonoBehaviour
{
    public Camera playerCamera;

    public Transform player;
    public Transform portalPlane;

    public LayerMask outsideLayerMask;
    public LayerMask insideLayerMask;

    public Camera cameraOutside;
    public Camera cameraInside;
    public RenderTexture renderTextureOutside;
    public RenderTexture renderTextureInside;

    private void Start()
    {
        cameraOutside.targetTexture = renderTextureOutside;
        cameraInside.targetTexture = renderTextureInside;
    }

    private void Update()
    {
        float distance = Vector3.Dot(player.position - portalPlane.position, portalPlane.forward);

        if (distance > 0)
        {
            playerCamera.cullingMask = insideLayerMask;
        } else if (distance < 0)
        {
            playerCamera.cullingMask = outsideLayerMask;
        } else
        {
            playerCamera.cullingMask = outsideLayerMask | insideLayerMask;
        }

        Material portalMaterial = playerCamera.GetComponent<Renderer>().material;
        portalMaterial.SetTexture("_OutsideTex", renderTextureOutside);
        portalMaterial.SetTexture("_InsideTex", renderTextureInside);

        //portalPlane.position = player.position;
        //portalPlane.forward = distance > 0 ? portalPlane.forward : -portalPlane.forward;

        //SetClippingPlane(playerCamera, portalPlane);
    }

    private void SetClippingPlane(Camera _camera, Transform _portalPlane)
    {
        Vector4 clipPlaneWorldSpace = new Vector4(
            _portalPlane.forward.x,
            _portalPlane.forward.y,
            _portalPlane.forward.z,
            Vector3.Dot(_portalPlane.forward, _portalPlane.position));

        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(_camera.worldToCameraMatrix) * clipPlaneWorldSpace;

        _camera.projectionMatrix = _camera.CalculateObliqueMatrix(clipPlaneCameraSpace);
    }
}
