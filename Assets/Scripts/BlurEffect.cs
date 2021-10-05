using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BlurEffect : MonoBehaviour
{
    public Shader shader;
    [Range(0, 0.1f)]
    public float Blur = 0.05f;

    private Material mat;

    private void Awake()
    {
        if (shader)
        {
            mat = new Material(shader);
            mat.SetFloat("_Blur", Blur);
        }
        else
            Debug.LogWarning("No shader assigned");
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (shader)
            Graphics.Blit(source, destination, mat);
    }
}