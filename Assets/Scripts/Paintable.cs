using UnityEngine;
using System.IO;
using UnityEditor;
[RequireComponent(typeof(Renderer))]
public class Paintable : MonoBehaviour {
    [Header("Texture Settings")]
    [SerializeField] private int TEXTURE_SIZE = 1024;
    [SerializeField] private bool isWider = false;
    [SerializeField] private bool autoSaveTexture = false; // Toggle in Inspector

    [Header("Saved Data")]
    //[SerializeField] private Texture2D _savedMaskTexture; // Drag saved texture here

    public float extendsIslandOffset = 1f;

    // Render Textures
    private RenderTexture extendIslandsRenderTexture;
    private RenderTexture uvIslandsRenderTexture;
    private RenderTexture maskRenderTexture;
    private RenderTexture maskRenderTextureSuperior;
    private RenderTexture supportTexture;

    private Renderer rend;
    private bool hasBeenPainted = false;
    private bool _texturesInitialized = false;

    // Shader Property IDs
    private static readonly int MaskTexID = Shader.PropertyToID("_MaskTexture");
    private static readonly int MaskSuperiorID = Shader.PropertyToID("_MaskTextureSuperior");
    private static readonly int ExtendTexID = Shader.PropertyToID("_ExtendTexture");

    void Start() {
        //InitializeSystem();
    }
    public void EnsureInitialized() {
        if (_texturesInitialized) return;

        rend = GetComponent<Renderer>();
        InitializeTextures();
        ApplyTexturesToMaterial();
        _texturesInitialized = true;
    }


    void InitializeSystem() {
        rend = GetComponent<Renderer>();
        InitializeTextures();
        ApplyTexturesToMaterial();
    }

    void InitializeTextures() {
        if (_texturesInitialized) return;

        int width = TEXTURE_SIZE;
        int height = isWider ? TEXTURE_SIZE * 2 : TEXTURE_SIZE;
        var format = RenderTextureFormat.ARGB32;

        maskRenderTexture = CreateRenderTexture(width, height, format);
        maskRenderTextureSuperior = CreateRenderTexture(width, height, format);
        extendIslandsRenderTexture = CreateRenderTexture(width, height, format);
        uvIslandsRenderTexture = CreateRenderTexture(width, height, format);
        supportTexture = CreateRenderTexture(width, height, format);

        // Load saved texture if available
    //    if (_savedMaskTexture != null) {
    //        LoadMaskFromTexture(_savedMaskTexture);
    //    }

    //    _texturesInitialized = true;
    }

    void OnEnable() {
        if (!_texturesInitialized) return;

        if (hasBeenPainted || maskRenderTexture != null) {
            ApplyTexturesToMaterial();
        }
    }

    void OnDisable() {
        if (autoSaveTexture && hasBeenPainted) {
            SaveMaskToTexture();
        }

        if (!hasBeenPainted) {
            ReleaseUnpaintedTextures();
        }
    }

    void OnDestroy() {
        ReleaseAllTextures();
    }

    #region Texture Management
    //ok esta parte guarda la mascara por si activo y desactivo el objeto
    //esto solo es mejor para el juego si en algun momento los objetos pintables son desactivados si no seguro es peor.
    public void SaveMaskToTexture() {
        if (maskRenderTexture == null) return;

        Texture2D tex = new Texture2D(
            maskRenderTexture.width,
            maskRenderTexture.height,
            TextureFormat.ARGB32,
            false
        );

        RenderTexture.active = maskRenderTexture;
        tex.ReadPixels(new Rect(0, 0, maskRenderTexture.width, maskRenderTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        //_savedMaskTexture = tex;
        hasBeenPainted = true;
   

        }

    public void LoadMaskFromTexture(Texture2D sourceTexture) {
        if (maskRenderTexture == null || sourceTexture == null) return;

        Graphics.Blit(sourceTexture, maskRenderTexture);
        MarkAsPainted();
        ApplyTexturesToMaterial();
    }

    private RenderTexture CreateRenderTexture(int width, int height, RenderTextureFormat format) {
        return new RenderTexture(width, height, 0, format) {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
    }
    #endregion

    #region Utility Methods
    private void ApplyTexturesToMaterial() {
        var block = new MaterialPropertyBlock();
        rend.GetPropertyBlock(block);
        block.SetTexture(MaskTexID, maskRenderTexture);
        block.SetTexture(MaskSuperiorID, maskRenderTextureSuperior);
        block.SetTexture(ExtendTexID, extendIslandsRenderTexture);
        rend.SetPropertyBlock(block);
    }

    public void MarkAsPainted() {
        hasBeenPainted = true;
    }

    private void ReleaseUnpaintedTextures() {
        if (!hasBeenPainted) {
            ReleaseTexture(ref maskRenderTexture);
            ReleaseTexture(ref maskRenderTextureSuperior);
            ReleaseTexture(ref uvIslandsRenderTexture);
            ReleaseTexture(ref extendIslandsRenderTexture);
            ReleaseTexture(ref supportTexture);
        }
    }

    private void ReleaseAllTextures() {
        ReleaseTexture(ref maskRenderTexture);
        ReleaseTexture(ref maskRenderTextureSuperior);
        ReleaseTexture(ref uvIslandsRenderTexture);
        ReleaseTexture(ref extendIslandsRenderTexture);
        ReleaseTexture(ref supportTexture);
    }

    private void ReleaseTexture(ref RenderTexture tex) {
        if (tex != null) {
            tex.Release();
            tex = null;
        }
    }
    #endregion

    #region Public Accessors
    public RenderTexture getMask() => maskRenderTexture;
    public RenderTexture getMaskSuperior() => maskRenderTextureSuperior;
    public RenderTexture getUVIslands() => uvIslandsRenderTexture;
    public RenderTexture getExtend() => extendIslandsRenderTexture;
    public RenderTexture getSupport() => supportTexture;
    public Renderer getRenderer() => rend;

    public bool HasPaintData => hasBeenPainted;
    #endregion
}