using UnityEngine;

public class Paintable : MonoBehaviour {
    [SerializeField] private int TEXTURE_SIZE = 1024;
    [SerializeField] private bool isWwider;

    public float extendsIslandOffset = 1;

    // Render Textures
    RenderTexture extendIslandsRenderTexture;
    RenderTexture uvIslandsRenderTexture;  // Fixed typo in variable name (was uvIslandsRenderTexture)
    RenderTexture maskRenderTexture;
    RenderTexture maskRenderTextureSuperior;
    RenderTexture supportTexture;

    Renderer rend;

    // Correct property IDs
    int maskTextureID = Shader.PropertyToID("_MaskTexture");
    int maskTextureSuperiorID = Shader.PropertyToID("_MaskTextureSuperior");  // Consistent naming
    int extendTextureID = Shader.PropertyToID("_ExtendTexture");  // Added for clarity

    public RenderTexture getMask() => maskRenderTexture;
    public RenderTexture getMaskSuperior() => maskRenderTextureSuperior;
    public RenderTexture getUVIslands() => uvIslandsRenderTexture;  // Fixed to return correct texture
    public RenderTexture getExtend() => extendIslandsRenderTexture;
    public RenderTexture getSupport() => supportTexture;
    public Renderer getRenderer() => rend;

    void Start() {
        InitializeTextures();
        rend = GetComponent<Renderer>();

        // Assign all necessary textures to the material
        rend.material.SetTexture(maskTextureID, maskRenderTexture);
        rend.material.SetTexture(maskTextureSuperiorID, maskRenderTextureSuperior);
        rend.material.SetTexture(extendTextureID, extendIslandsRenderTexture);

        PaintManager.instance.initTextures(this);
    }

    void InitializeTextures() {
        int width = TEXTURE_SIZE;
        int height = isWwider ? TEXTURE_SIZE / 2 : TEXTURE_SIZE;
        RenderTextureFormat format = RenderTextureFormat.ARGB32;

        // Create textures with consistent settings
        maskRenderTexture = CreateTexture(width, height, format);
        
        maskRenderTextureSuperior = CreateTexture(width, height, format);
        //Debug.Log(maskRenderTexture.name + "wacawaca");
        extendIslandsRenderTexture = CreateTexture(width, height, format);
        uvIslandsRenderTexture = CreateTexture(width, height, format);
        supportTexture = CreateTexture(width, height, format);
    }

    RenderTexture CreateTexture(int width, int height, RenderTextureFormat format) {
        var rt = new RenderTexture(width, height, 0, format);
        rt.filterMode = FilterMode.Bilinear;
        return rt;
    }

    void OnDisable() {
        // Safely release all textures
        ReleaseTexture(ref maskRenderTexture);
        ReleaseTexture(ref maskRenderTextureSuperior);
        ReleaseTexture(ref uvIslandsRenderTexture);
        ReleaseTexture(ref extendIslandsRenderTexture);
        ReleaseTexture(ref supportTexture);
    }

    void ReleaseTexture(ref RenderTexture texture) {
        if (texture != null) {
            texture.Release();
            texture = null;  // Prevents access to released texture
        }
    }

#if UNITY_EDITOR
    void OnDestroy() {
        OnDisable();  // Extra safety in editor
    }
#endif
}