/********************************************************************
	created:  2018-4-4 14:5:35
	filename: EntityObjectBase.cs
	author:	  songguangze@outlook.com
	
	purpose:  实体渲染对象基类
*********************************************************************/
using EngineCore;
using UnityEngine;


namespace SeekerGame
{
    public class EntityObjectBase
    {
        private GameObject m_entityGameObject = null;               //渲染对象
        private EntityBase m_owner = null;                          //所属Entity
        private string m_objectOriginName = string.Empty;           //资源原始名称
        private int m_entityLayer = 1;
        private bool m_isEnable = true;

        public EntityObjectBase(EntityBase owner, GameObject entityGameObject)
        {
            this.m_owner = owner;
            this.m_entityGameObject = entityGameObject;
            this.m_objectOriginName = entityGameObject.name;
            this.EntityGameObject.name = owner.EntityId;
        }

        public virtual void Update()
        {

        }


        public virtual void Destroy()
        {
            //EngineCoreEvents.ResourceEvent.ReleaseAndRemoveAssetEvent.SafeInvoke(this.Owner.AssetName, EntityGameObject);
            EngineCoreEvents.ResourceEvent.ReleaseAssetEvent.SafeInvoke(this.Owner.AssetName, EntityGameObject);
            this.m_entityGameObject = null;
        }

        /// <summary>
        /// 设置物体位置及旋转信息
        /// </summary>
        /// <param name="positin"></param>
        /// <param name="rotationEularAngle"></param>
        public void SetEntityObjectPositionAndRotation(Vector3 positin, Vector3 rotationEularAngle)
        {
            EntityTransform.SetPositionAndRotation(positin, Quaternion.Euler(rotationEularAngle));
        }

        public void SetLightMap(Vector4 lightInfo, Texture tex)
        {
            if (this.EntityMat == null)
            {
                Debug.LogError("entity lightmap error");
                return;
            }
            this.EntityMat.SetTexture("_LightTex", tex);
            this.EntityMat.SetTextureOffset("_LightTex", new Vector2(lightInfo.x, lightInfo.y));
            this.EntityMat.SetTextureScale("_LightTex", new Vector2(lightInfo.z, lightInfo.w));
        }

        /// <summary>
        /// 设置实体渲染对象父节点
        /// </summary>
        /// <param name="parentTransform"></param>
        public void SetEntityParent(Transform parentTransform)
        {
            EntityTransform.SetParent(parentTransform);
        }


        #region Properties
        public bool Visible
        {
            get { return this.m_entityGameObject.activeInHierarchy; }
            set { this.m_entityGameObject.SetActive(value); }
        }

        public Transform EntityTransform
        {
            get { return this.m_entityGameObject.transform; }
        }


        public Material EntityMat
        {
            get
            {
                Renderer render = this.m_entityGameObject.GetComponentInChildren<Renderer>(true);
                if (render != null || render.material != null)
                {
                    string shaderName = EntityManager.Instance.GetEntityShader(render.material.shader.name);
                    if (!string.IsNullOrEmpty(shaderName))
                    {
                        srcShaderName = shaderName;
                        //render.material.shader = Shader.Find(shaderName);
                        render.material.shader = ShaderModule.Instance.GetShader(shaderName);
                    }
                    return render.material;
                }
                return null;
            }
        }

        string srcShaderName = string.Empty;
        public void ChangeEntityMatToHint()
        {
            string shaderName = EntityManager.Instance.GetHintShader(srcShaderName);
            if (!string.IsNullOrEmpty(shaderName))
            {
                EntityMat.shader = ShaderModule.Instance.GetShader(shaderName);
            }
        }

        public void RecoverEntityMat()
        {
            if (!string.IsNullOrEmpty(srcShaderName))
            {
                EntityMat.shader = ShaderModule.Instance.GetShader(srcShaderName);
            }
        }

        public EntityBase Owner
        {
            get { return this.m_owner; }
        }

        public GameObject EntityGameObject
        {
            get { return this.m_entityGameObject; }
        }

        public int Layer
        {
            get { return this.m_entityLayer; }
            set
            {
                if (this.m_entityLayer != value)
                {
                    GameObjectUtil.SetLayer(EntityGameObject, value);
                    this.m_entityLayer = value;
                }
            }
        }

        public virtual bool IsEnable
        {
            get { return this.m_isEnable; }
            set { this.m_isEnable = value; }
        }


        #endregion

    }
}
