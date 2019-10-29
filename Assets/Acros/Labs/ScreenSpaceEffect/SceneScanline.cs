using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class SceneScanline : MonoBehaviour {

    public Color LineColor;
    public Texture2D scanTexture;

    private Material _material;
    private Camera _Cam;

    public Vector2 depthRange = new Vector2(0, 0.95f);
    public float lineNormalWidth = 0.05f;
    public float scanDuration = 2;
    public Vector2 texUvScale = new Vector2(1,1);

    private float currPos = 0;

    void OnEnable()
    {
        _Cam = GetComponent<Camera>();
        // dynamically create a material that will use our shader
        _material = new Material(Shader.Find("Acros/SceneDepthScanline"));

        // tell the camera to render depth and normals
        _Cam.depthTextureMode |= DepthTextureMode.DepthNormals;

        _material.SetTexture("_scanTex", scanTexture);
        _material.SetColor("_lineColor", LineColor);
        scanDuration = Mathf.Clamp(scanDuration, 0.1f, 100);
    }

    private void Update()
    {
        currPos += ( ( scanDuration) * Time.deltaTime);
        if (currPos + lineNormalWidth >= depthRange.y) 
            currPos = depthRange.x;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // set shader properties
        _material.SetFloat("_lineDepthMin", currPos);
        _material.SetFloat("_lineDepthMax", currPos + lineNormalWidth);
        _material.SetVector("_scale", texUvScale);

        // execute the shader on input texture (src) and write to output (dest)
        Graphics.Blit(src, dest, _material);
    }
}
