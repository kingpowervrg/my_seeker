using EngineCore;
using UnityEngine;
using UnityEngine.UI;

namespace SeekerGame
{
    public class GuidUIClick : GuidUIOpen
    {
        private Transform m_srcTran;
        private Transform m_destTran;
        private TweenScale m_TweenScale;
        private GameToggleButton m_tog;
        private GameButton m_btn;
        
        protected override void OnOpenUI(GUIFrame uiLogic)
        {
            
            m_tog = uiLogic.LogicHandler.Make<GameToggleButton>(m_CurConf.btnName[0]);
            if (m_tog != null)
            {
                m_srcTran = m_tog.gameObject.transform;
                m_tog.AddChangeCallBack(OnToggle);
            }
            else
            {
                m_btn = uiLogic.LogicHandler.Make<GameButton>(m_CurConf.btnName[0]);
                if (m_btn != null)
                {
                    m_srcTran = m_btn.gameObject.transform;
                    m_btn.AddClickCallBack(OnBtn);
                }
            }
            m_TweenScale = m_srcTran.gameObject.AddComponent<TweenScale>();
            m_TweenScale.To = Vector3.one * 1.1f;
            m_TweenScale.m_tweenStyle = UITweenerBase.TweenStyle.Loop;
            m_TweenScale.Duration = 2f;
            base.OnOpenUI(uiLogic);
        }

        protected override void OnShowMask(UILogicBase logic)
        {
            base.OnShowMask(logic);
            GameObject destTran = GameObject.Instantiate(m_srcTran.gameObject) as GameObject;
            m_destTran = destTran.transform;
            ChangeTran(m_uiLogic, m_srcTran, m_destTran);
        }

        private void ChangeTran(UILogicBase logic,Transform srcTran,Transform destTran)
        {
            Transform parent = srcTran.parent;
            int siblingIndex = srcTran.GetSiblingIndex();
            Vector3 localScale = srcTran.transform.localScale;
            Quaternion rotation = srcTran.transform.rotation;
            Vector3 pos = srcTran.transform.localPosition;

            GuidUILogic guidLogic = (GuidUILogic)logic;
            guidLogic.SetData(srcTran);

            destTran.transform.SetParent(parent);
            destTran.transform.localPosition = pos;
            destTran.transform.SetSiblingIndex(siblingIndex);
            destTran.transform.localScale = localScale;
            destTran.transform.rotation = rotation;
        }

        private void OnToggle(bool b)
        {
            m_tog.RemoveChangeCallBack(OnToggle);
            Reset();
        }

        private void OnBtn(GameObject obj)
        {
            m_btn.RemoveClickCallBack(OnBtn);
            Reset();
        }

        protected override void EndGuid()
        {
            if (m_uiLogic != null)
            {
                ChangeTran(m_uiLogic, m_destTran, m_srcTran);

                if (m_TweenScale != null)
                {
                    GameObject.DestroyImmediate(m_TweenScale);
                    m_srcTran.localScale = Vector3.one;
                }
                GameObject.DestroyImmediate(m_destTran.gameObject);
            }
            base.EndGuid();
        }

        private void Reset()
        {
            EndGuid();
        }
    }
}
