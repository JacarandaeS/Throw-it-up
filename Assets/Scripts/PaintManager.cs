using UnityEngine;
using UnityEngine.Rendering;

public class PaintManager : Singleton<PaintManager> {
    [Header("Shader References")]
    public Shader texturePaint;
    public Shader extendIslands;

    [Header("Layer Toggle")]
    public bool layer2 = false;

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
        command = new CommandBuffer { name = "CommandBuffer - " + gameObject.name };
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.L)) {
            layer2 = !layer2;
            Debug.Log("Layer2 toggled: " + layer2);
        }
    }

    public void initTextures(Paintable paintable) {
        RenderTexture mask = paintable.getMask();
        RenderTexture maskSuperior = paintable.getMaskSuperior();
        RenderTexture uvIslands = paintable.getUVIslands();
        RenderTexture extend = paintable.getExtend();
        RenderTexture support = paintable.getSupport();
        Renderer rend = paintable.getRenderer();

        command.SetRenderTarget(mask);
        command.ClearRenderTarget(false, true, Color.clear);

        command.SetRenderTarget(maskSuperior);
        command.ClearRenderTarget(false, true, Color.clear);

        command.SetRenderTarget(extend);
        command.ClearRenderTarget(false, true, Color.clear);

        command.SetRenderTarget(support);
        command.ClearRenderTarget(false, true, Color.clear);

        paintMaterial.SetFloat(prepareUVID, 1);
        command.SetRenderTarget(uvIslands);
        command.DrawRenderer(rend, paintMaterial, 0);

        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }

    public void paint(Paintable paintable, Vector3 pos, float radius = 1f,
                     float hardness = .5f, float strength = .5f,
                     Color? color = null) {

        if (paintable == null) {
            Debug.LogWarning("Tried to paint on null Paintable object");
            return;
        }

        try {
            RenderTexture targetMask = layer2 ? paintable.getMaskSuperior() : paintable.getMask();
            RenderTexture uvIslands = paintable.getUVIslands();
            RenderTexture extend = paintable.getExtend();
            RenderTexture support = paintable.getSupport();
            Renderer rend = paintable.getRenderer();

            // Paint material setup
            paintMaterial.SetFloat(prepareUVID, 0);
            paintMaterial.SetVector(positionID, pos);
            paintMaterial.SetFloat(hardnessID, hardness);
            paintMaterial.SetFloat(strengthID, strength);
            paintMaterial.SetFloat(radiusID, radius);
            paintMaterial.SetTexture(textureID, support);
            paintMaterial.SetColor(colorID, color ?? Color.red);

            // Extend material setup
            extendMaterial.SetFloat(uvOffsetID, paintable.extendsIslandOffset);
            extendMaterial.SetTexture(uvIslandsID, uvIslands);

            // Command buffer paint process
            command.Clear();

            command.SetRenderTarget(targetMask);
            command.DrawRenderer(rend, paintMaterial, 0);

            command.SetRenderTarget(support);
            command.Blit(targetMask, support);

            command.SetRenderTarget(extend);
            command.Blit(targetMask, extend, extendMaterial);

            Graphics.ExecuteCommandBuffer(command);
        }
        catch (System.Exception e) {
            return;
        }
    }

    void OnDestroy() {
        if (command != null) {
            command.Release();
        }
    }
}
