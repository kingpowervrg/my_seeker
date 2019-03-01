using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame.NewGuid
{
    public class GuidNewNodeManager : Singleton<GuidNewNodeManager>
    {
        private bool m_canCartoon = false;

        public void SetCanCartoon(bool can)
        {
            m_canCartoon = can;
        }

        public long taskConfId = 0;
        public int m_eventState = 0;

        public bool getCanCartoon()
        {
            return m_canCartoon;
        }

        public float first_delta_pos = 0;
        public bool needPause = false;
        public bool sceneShowResult = true;
        //public float 

        public const string CoinComplete = "CoinComplete";
        public const string FindItemByID = "FindItemByID";
        public const string ForbidErrorSecond = "ForbidErrorSecond"; //禁用扣时间
        public const string ForbidScenePropBuy = "ForbidScenePropBuy"; //禁用道具内购
        public const string ForbidScenePause = "ForbidScenePause"; //禁用暂停按钮
        public const string ForbidPropUse = "ForbidPropUse";
        public const string ChangeStateToLogin = "ChangeStateToLogin";
        public const string ForbidBtnBack = "ForbidBtnBack";

        public const string SceneTips = "SceneTips";
        public const string NeedFindID = "NeedFindID";
        public const string PropFree = "PropFree";


        public Dictionary<string, NodeStatus> m_nodeStatus = new Dictionary<string, NodeStatus>();

        public void SetNodeStatus(string node,NodeStatus status)
        {
            if (m_nodeStatus.ContainsKey(node))
            {
                m_nodeStatus[node] = status;
            }
            else
            {
                m_nodeStatus.Add(node,status);
            }
        }

        public NodeStatus GetNodeStatus(string node)
        {
            if (m_nodeStatus.ContainsKey(node))
            {
                return m_nodeStatus[node];
            }
            return NodeStatus.Complete;
        }

        public bool m_sceneTips = true;

        #region 1123新增添加
        public const string sceneExhibit = "sceneExhibit"; //场景物件
        //公共变量
        private Dictionary<string, string> m_commonParams = new Dictionary<string, string>();

        public void SetCommonParams(string key,string value)
        {
            if (m_commonParams.ContainsKey(key))
            {
                m_commonParams[key] = value;
            }
            else
            {
                m_commonParams.Add(key,value);
            }
        }

        public string GetCommonParams(string key)
        {
            if (m_commonParams.ContainsKey(key))
            {
                return m_commonParams[key];
            }
            return string.Empty;
        }

        public void RemoveCommonParams(string key)
        {
            if (m_commonParams.ContainsKey(key))
            {
                m_commonParams.Remove(key);
            }
        }

        /// <summary>
        /// 公共对象
        /// </summary>
        public const string O_MASK__57 = "mats/o_mask__57"; 

        private Dictionary<string, Object> m_commonObjs = new Dictionary<string, object>();

        public void SetCommonObj(string key, Object value)
        {
            if (m_commonObjs.ContainsKey(key))
            {
                m_commonObjs[key] = value;
            }
            else
            {
                m_commonObjs.Add(key, value);
            }
        }

        public Object GetCommonObj(string key)
        {
            if (m_commonObjs.ContainsKey(key))
            {
                return m_commonObjs[key];
            }
            return null;
        }

        public void RemoveCommonObj(string key)
        {
            if (m_commonObjs.ContainsKey(key))
            {
                m_commonObjs.Remove(key);
            }
        }
        public override void Destroy()
        {
            base.Destroy();
            this.m_commonParams.Clear();
            this.m_nodeStatus.Clear();
            this.m_commonObjs.Clear();
        }
        #endregion

    }

}
