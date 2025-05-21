using UnityEngine;

public class Paintable : MonoBehaviour {
    [SerializeField] private int TEXTURE_SIZE = 1024;
    [SerializeField] private bool isWwider;

    public float extendsIslandOffset = 1;

    RenderTexture extendIslandsRenderTexture;
    RenderTexture uvIslandsRenderTexture;
    RenderTexture maskRenderTexture;
    RenderTexture supportTexture;

    Renderer rend;

    int maskTextureID = Shader.PropertyToID("_MaskTexture");

    public RenderTexture getMask() => maskRenderTexture;
    public RenderTexture getUVIslands() => uvIslandsRenderTexture;
    public RenderTexture getExtend() => extendIslandsRenderTexture;
    public RenderTexture getSupport() => supportTexture;
    public Renderer getRenderer() => rend;

    void Start() {
        int width = TEXTURE_SIZE;
        int height = isWwider ? TEXTURE_SIZE / 2 : TEXTURE_SIZE;

        RenderTextureFormat format = RenderTextureFormat.ARGB32;

        maskRenderTexture = new RenderTexture(width, height, 0, format);
        maskRenderTexture.filterMode = FilterMode.Bilinear;

        extendIslandsRenderTexture = new RenderTexture(width, height, 0, format);
        extendIslandsRenderTexture.filterMode = FilterMode.Bilinear;

        uvIslandsRenderTexture = new RenderTexture(width, height, 0, format);
        uvIslandsRenderTexture.filterMode = FilterMode.Bilinear;

        supportTexture = new RenderTexture(width, height, 0, format);
        supportTexture.filterMode = FilterMode.Bilinear;

        rend = GetComponent<Renderer>();
        rend.material.SetTexture(maskTextureID, extendIslandsRenderTexture);

        PaintManager.instance.initTextures(this);
    }

    void OnDisable() {
        maskRenderTexture.Release();
        uvIslandsRenderTexture.Release();
        extendIslandsRenderTexture.Release();
        supportTexture.Release();
    }
}
