using UnityEngine;

public class LightRecoverSystem : LightSystem
{
    public LightRecoverSystem(LightingData m_data):base(m_data)
    {
        
    }

    protected override GameObject LoadGameObject(string modelPath, ItemPosInfoJson posInfo)
    {
        GameObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/" + modelPath + ".prefab");
        obj = GameObject.Instantiate(obj) as GameObject;
        MeshRenderer render = obj.GetComponent<MeshRenderer>();
        if (render != null)
        {
            render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            render.receiveShadows = false;
            render.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            render.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            string shaderName = render.sharedMaterial.shader.name;
            if (shaderName.Contains("Mobile/Diffuse"))
            {
                render.sharedMaterial.shader = Shader.Find("Seeker/Diffuse");
                
            }
            render.sharedMaterial.SetTextureOffset("_LightTex", new Vector2(posInfo.offsetX, posInfo.offsetY));
            render.sharedMaterial.SetTextureScale("_LightTex", new Vector2(posInfo.tilingX, posInfo.tilingY));
        }
        obj.transform.position = new Vector3(posInfo.pos.x, posInfo.pos.y, posInfo.pos.z);
        obj.transform.eulerAngles = new Vector3(posInfo.rotate.x, posInfo.rotate.y, posInfo.rotate.z);
        obj.transform.localScale = new Vector3(posInfo.scale.x, posInfo.scale.y, posInfo.scale.z);
        return obj;
    }

    public void SetLightMap()
    {
        if (m_data.m_allExhibitData.Count == 0)
        {
            LoadConfigData();
        }
        ClearGameObject();
        LoadModel();
    }
}