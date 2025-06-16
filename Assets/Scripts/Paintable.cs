using UnityEngine;

public class Paintable : MonoBehaviour {
    [SerializeField] private int TEXTURE_SIZE = 1024;
    [SerializeField] private bool isWider = false;

    public float extendsIslandOffset = 1f;

    RenderTexture extendIslandsRenderTexture;
    RenderTexture uvIslandsRenderTexture;
    RenderTexture maskRenderTexture;
    RenderTexture maskRenderTextureSuperior;
    RenderTexture supportTexture;

    Renderer rend;

    static readonly int MaskTexID = Shader.PropertyToID("_MaskTexture");
    static readonly int MaskSuperiorID = Shader.PropertyToID("_MaskTextureSuperior");
    static readonly int ExtendTexID = Shader.PropertyToID("_ExtendTexture");

    public RenderTexture getMask() => maskRenderTexture;
    public RenderTexture getMaskSuperior() => maskRenderTextureSuperior;
    public RenderTexture getUVIslands() => uvIslandsRenderTexture;
    public RenderTexture getExtend() => extendIslandsRenderTexture;
    public RenderTexture getSupport() => supportTexture;
    public Renderer getRenderer() => rend;

    void Start() {
        rend = GetComponent<Renderer>();
        InitializeTextures();

        var block = new MaterialPropertyBlock();
        rend.GetPropertyBlock(block);
        block.SetTexture(MaskTexID, maskRenderTexture);
        block.SetTexture(MaskSuperiorID, maskRenderTextureSuperior);
        block.SetTexture(ExtendTexID, extendIslandsRenderTexture);
        rend.SetPropertyBlock(block);

        PaintManager.instance.initTextures(this);
    }

    void InitializeTextures() {
        int width = TEXTURE_SIZE;
        int height = isWider ? TEXTURE_SIZE / 2 : TEXTURE_SIZE;
        var format = RenderTextureFormat.ARGB32;

        maskRenderTexture = CreateTexture(width, height, format);
        maskRenderTextureSuperior = CreateTexture(width, height, format);
        extendIslandsRenderTexture = CreateTexture(width, height, format);
        uvIslandsRenderTexture = CreateTexture(width, height, format);
        supportTexture = CreateTexture(width, height, format);
    }

    RenderTexture CreateTexture(int width, int height, RenderTextureFormat format) {
        return new RenderTexture(width, height, 0, format) {
            filterMode = FilterMode.Bilinear
        };
    }

    void OnDisable() {
        Release(ref maskRenderTexture);
        Release(ref maskRenderTextureSuperior);
        Release(ref uvIslandsRenderTexture);
        Release(ref extendIslandsRenderTexture);
        Release(ref supportTexture);
    }

    void OnDestroy() => OnDisable();

    void Release(ref RenderTexture tex) {
        if (tex != null) {
            tex.Release();
            tex = null;
        }
    }
}
