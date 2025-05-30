using UnityEngine;
using UnityEngine.Rendering;

public class PaintManager : Singleton<PaintManager> {
    [Header("Shader References")]
    public Shader texturePaint;
    public Shader extendIslands;


    // Shader Property IDs
    int prepareUVID = Shader.PropertyToID("_PrepareUV");
    int positionID = Shader.PropertyToID("_PainterPosition");
    int hardnessID = Shader.PropertyToID("_Hardness");
    int strengthID = Shader.PropertyToID("_Strength");
    int radiusID = Shader.PropertyToID("_Radius");
    int blendOpID = Shader.PropertyToID("_BlendOp");
    int colorID = Shader.PropertyToID("_PainterColor");
    int textureID = Shader.PropertyToID("_MainTex");
    int uvOffsetID = Shader.PropertyToID("_OffsetUV");
    int uvIslandsID = Shader.PropertyToID("_UVIslands");
    int maskTypeID = Shader.PropertyToID("_MaskType");

    Material paintMaterial;
    Material extendMaterial;
    CommandBuffer command;

    public override void Awake() {
        base.Awake();

        paintMaterial = new Material(texturePaint);
        extendMaterial = new Material(extendIslands);
        command = new CommandBuffer();
        command.name = "CommandBuffer - " + gameObject.name;

    }

    public void initTextures(Paintable paintable) {
        RenderTexture mask = paintable.getMask();
        RenderTexture maskSuperior = paintable.getMaskSuperior();
        RenderTexture uvIslands = paintable.getUVIslands();
        RenderTexture extend = paintable.getExtend();
        RenderTexture support = paintable.getSupport();
        Renderer rend = paintable.getRenderer();

        // Clear all textures
        command.SetRenderTarget(mask);
        command.ClearRenderTarget(false, true, Color.clear);

        command.SetRenderTarget(maskSuperior);
        command.ClearRenderTarget(false, true, Color.clear);

        command.SetRenderTarget(extend);
        command.ClearRenderTarget(false, true, Color.clear);

        command.SetRenderTarget(support);
        command.ClearRenderTarget(false, true, Color.clear);

        // Prepare UV islands
        paintMaterial.SetFloat(prepareUVID, 1);
        command.SetRenderTarget(uvIslands);
        command.DrawRenderer(rend, paintMaterial, 0);

        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }

    public void paint(Paintable paintable, Vector3 pos, float radius = 1f,
                     float hardness = .5f, float strength = .5f,
                     Color? color = null, bool paintOnSuperior = false) {

        // Update paint position visualization


        RenderTexture targetMask = paintOnSuperior ? paintable.getMaskSuperior() : paintable.getMask();
        RenderTexture uvIslands = paintable.getUVIslands();
        RenderTexture extend = paintable.getExtend();
        RenderTexture support = paintable.getSupport();
        Renderer rend = paintable.getRenderer();

        // Set up paint material
        paintMaterial.SetFloat(prepareUVID, 0);
        paintMaterial.SetVector(positionID, pos);
        paintMaterial.SetFloat(hardnessID, hardness);
        paintMaterial.SetFloat(strengthID, strength);
        paintMaterial.SetFloat(radiusID, radius);
        paintMaterial.SetTexture(textureID, support);
        paintMaterial.SetColor(colorID, color ?? Color.red);
        paintMaterial.SetFloat(maskTypeID, paintOnSuperior ? 1 : 0); // Tell shader which mask we're painting on

        // Set up extend material
        extendMaterial.SetFloat(uvOffsetID, paintable.extendsIslandOffset);
        extendMaterial.SetTexture(uvIslandsID, uvIslands);

        // Paint operations
        command.SetRenderTarget(targetMask);
        command.DrawRenderer(rend, paintMaterial, 0);

        command.SetRenderTarget(support);
        command.Blit(targetMask, support);

        command.SetRenderTarget(extend);
        command.Blit(targetMask, extend, extendMaterial);

        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }





    void OnDestroy() {
        if (command != null) {
            command.Release();
        }


    }
}