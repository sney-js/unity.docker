using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackholeShader : MonoBehaviour
{

    public Shader shader;

    public float ratio = 1;   // The ratio of the height to the length of the screen to display properly shader
    public float radius = 0;  // The radius of the black hole measured in the same units as the other objects in the scene

    public GameObject BH;  // The object whose position is taken as the position of the black hole
    private Material _material; // Material which is located shader
    protected Material material
    {
        get
        {
            if (_material == null)
            {
                _material = new Material(shader);
                _material.hideFlags = HideFlags.HideAndDontSave;
            }
            return _material;
        }
    }

    protected virtual void OnDisable()
    {
        if (_material)
        {
            DestroyImmediate(_material);
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (shader && material)
        {
            // Find the position of the black hole in screen coordinates

            Camera camera1 = GetComponent<Camera>();
            Vector2 pos = new Vector2(
                camera1.WorldToViewportPoint(BH.transform.position).x / camera1.pixelWidth,
                (camera1.WorldToViewportPoint(BH.transform.position).z / camera1.pixelHeight));

            Vector3 location = GetComponent<Camera>().WorldToViewportPoint(BH.transform.position);
            //print(pos);
            //print(screenPoint);
            // Install all the required parameters for the shader
            material.SetVector("_Position", new Vector2(location.x, location.y));
            material.SetFloat("_Ratio", ratio);
            material.SetFloat("_Rad", radius);
            material.SetFloat("_Distance", Vector3.Distance(BH.transform.position, this.transform.position));
            // And is applied to the resulting image.
            Graphics.Blit(source, destination, material);
        }
    }
}
