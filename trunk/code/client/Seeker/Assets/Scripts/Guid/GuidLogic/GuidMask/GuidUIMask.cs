using EngineCore;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SeekerGame
{
    public class GuidUIMask : GuidUI
    {
        private List<TweenScale> tweens = new List<TweenScale>();
        public override void StartGuid()
        {
            base.StartGuid();
            GameEvents.UI_Guid_Event.OnNextGuid += OnNextGuid;
        }

        protected override void OnOpenUI(GUIFrame uiLogic)
        {
            base.OnOpenUI(uiLogic);
            TimeModule.Instance.SetTimeout(delegate ()
            {
                tweens.Clear();
                GuidMaskCommonData maskCommonDatas = new GuidMaskCommonData();
                List<GuidMaskData> maskDatas = new List<GuidMaskData>();
                for (int i = 0; i < m_CurConf.maskName.Length; i++)
                {
                    string btnStr = m_CurConf.maskName[i].Replace(":", "/");
                    Transform srcTran = uiLogic.FrameRootTransform.Find(btnStr);
                    if (srcTran == null)
                    {
                        continue;
                    }
                    //if (srcTran.GetComponent<Image>() == null && srcTran.GetComponent<RawImage>() == null)
                    //{
                    //    Image srcRay = srcTran.gameObject.AddComponent<Image>();
                    //    srcRay.raycastTarget = true;
                    //    srcRay.color = new Color(1,1,1,0.01f);
                    //}

                    //TweenScale srcTween = srcTran.gameObject.AddComponent<TweenScale>();
                    //srcTween.to = Vector3.one * 1.2f;
                    //srcTween.duration = 0.5f;
                    //srcTween.style = UITweenerBase.Style.PingPong;
                    //srcTween.Play();
                    //tweens.Add(srcTween);
                    RectTransform srcRect = srcTran.GetComponent<RectTransform>();
                    if (srcRect != null)
                    {
                        Vector2[] cornPos = GuidTools.getCornPos(srcRect);
                        GuidMaskData maskData = new GuidMaskData();
                        maskData.leftBottom = cornPos[0];
                        maskData.leftTop = cornPos[1];
                        maskData.rightTop = cornPos[2];
                        maskData.rightBottom = cornPos[3];
                        maskData.maskType = (GuidMaskType)m_CurConf.maskType[i];
                        if (m_CurConf.btnName.Length > i)
                        {
                            maskData.btnName = m_CurConf.btnName[i];
                        }
                        else
                        {
                            maskData.btnName = string.Empty;
                        }
                        maskDatas.Add(maskData);
                    }
                }
                List<Vector2> artAnchor = GuidTools.getArtPos(uiLogic.FrameRootTransform, m_CurConf);
                maskCommonDatas.m_confGuid = m_CurConf;
                maskCommonDatas.m_maskdata = maskDatas;
                maskCommonDatas.hasEvent = true;
                maskCommonDatas.m_artPos = artAnchor;
                maskCommonDatas.eventPassType = (EventPassType)m_CurConf.eventPassType;
                //maskCommonDatas.m_artPath = new List<string>(m_CurConf.artPath);
                if (m_CurConf.btnName == null || m_CurConf.btnName.Length == 0)
                {
                    maskCommonDatas.hasEvent = false;
                }
                maskCommonDatas.m_operaType = (GuidEnum)m_CurConf.type;

                float.TryParse(m_CurConf.typeValue, out maskCommonDatas.m_TypeValue);
                FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_GUID);
                param.Param = maskCommonDatas;
                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);

            }, 0.1f);
        }

        private void OnNextGuid()
        {
            EndGuid();
        }

        protected override void EndGuid()
        {
            base.EndGuid();
            for (int i = 0; i < tweens.Count; i++)
            {
                if (tweens[i] != null)
                {
                    tweens[i].transform.localScale = Vector3.one;
                    GameObject.DestroyImmediate(tweens[i]);
                }
            }
        }

        protected override void Destory()
        {
            base.Destory();
            GameEvents.UI_Guid_Event.OnNextGuid -= OnNextGuid;
        }
    }
}
