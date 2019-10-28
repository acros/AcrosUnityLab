using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ScreenSpaceSnow : MonoBehaviour
{
    public Texture2D SnowTexture;

    public Color SnowColor = Color.white;

    public float SnowTextureScale = 0.1f;

    [Range(0, 1)]
    public float BottomThreshold = 0f;
    [Range(0, 1)]
    public float TopThreshold = 1f;

    private Material _material;
    private Camera _Cam;

    void OnEnable()
    {
        _Cam = GetComponent<Camera>();
        // dynamically create a material that will use our shader
        _material = new Material(Shader.Find("TKoU/ScreenSpaceSnow"));

        // tell the camera to render depth and normals
        // [Snow fall on upper plane]
        _Cam.depthTextureMode |= DepthTextureMode.DepthNormals;


        //TODO: Why must deferred?
        Debug.Assert(_Cam.renderingPath == RenderingPath.DeferredShading);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // set shader properties
        _material.SetMatrix("_CamToWorld", _Cam.cameraToWorldMatrix);
        _material.SetColor("_SnowColor", SnowColor);
        _material.SetFloat("_BottomThreshold", BottomThreshold);
        _material.SetFloat("_TopThreshold", TopThreshold);
        _material.SetTexture("_SnowTex", SnowTexture);
        _material.SetFloat("_SnowTexScale", SnowTextureScale);

        // execute the shader on input texture (src) and write to output (dest)
        Graphics.Blit(src, dest, _material);
    }
}