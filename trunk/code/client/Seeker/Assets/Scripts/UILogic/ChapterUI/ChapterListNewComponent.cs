using DG.Tweening;
using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace SeekerGame
{
    public class ChapterListNewComponent : GameUIComponent
    {
        private GameUIContainer m_grid = null;
        private GameImage m_gridView = null;
        //private int count = 5;
        private GameScrollView m_scrollView = null;
        private GameImage m_BoundImg = null;

        private ChapterScrollItem[] m_scrollItem = null;

        private Transform m_centerTran = null;//中心吸附点

        private ChapterChooseItem m_buttonRoot = null;

        private float m_itemHei = 324f;

        private float m_spaceHei = -80f;

        private float m_xShift = -32f; //偏移角度

        private int displayChapterIndex = 0;

        public bool IsDirty = true;
        #region 数据
        private List<ChapterInfo> m_allChapterInfoList = null;
        #endregion
        protected override void OnInit()
        {
            base.OnInit();
            this.m_grid = Make<GameUIContainer>("Panel_animation:clueScrollView:grid");
            this.m_gridView = Make<GameImage>("Panel_animation:clueScrollView:grid");
            this.m_scrollView = Make<GameScrollView>("Panel_animation:clueScrollView");

            this.m_buttonRoot = Make<ChapterChooseItem>("ChooseItem");
            this.m_BoundImg = Make<GameImage>("Panel_animation:ChooseItem:Panel:Image_bound");
            this.m_centerTran = this.m_scrollView.Widget.Find("center");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_allChapterInfoList = GlobalInfo.MY_PLAYER_INFO.PlayerChapterSystem.PlayerChapterSet;
            this.m_buttonRoot.m_chooseAction = ChooseItem;
            m_gridView.AddDragCallBack(OnDrag);
            m_gridView.AddDragStartCallBack(OnDragStart);
            m_gridView.AddDragEndCallBack(OnDragEnd);

            if (param != null)
            {
                if (param is ChapterInfo)
                    displayChapterIndex = (int)((ChapterInfo)param).ChapterID - 1;
                else
                    displayChapterIndex = Convert.ToInt32(param) - 1;
            }
            else
            {
                displayChapterIndex = this.m_allChapterInfoList.FindIndex(chapterInfo => chapterInfo.ChapterStatus == ChapterStatus.UNLOCK);
                if (displayChapterIndex == -1)
                    displayChapterIndex = this.m_allChapterInfoList.FindIndex(chapterInfo => chapterInfo.ChapterStatus == ChapterStatus.DONE);
            }
            lastDisplayChapterIndex = displayChapterIndex;
            RefreshChapterList();
        }

        public override void OnHide()
        {
            base.OnHide();
            m_gridView.RemoveDragCallBack(OnDrag);
            m_gridView.RemoveDragStartCallBack(OnDragStart);
            m_gridView.RemoveDragEndCallBack(OnDragEnd);
            IsDirty = true;
        }

        private void ChooseItem()
        {
            Visible = false;
        }

        private void RefreshChapterList()
        {
            if (IsDirty)
            {
                IsDirty = false;
                int count = m_allChapterInfoList.Count;
                float scrollHei = m_itemHei * count + m_spaceHei * (count - 1);
                m_scrollView.Widget.sizeDelta = m_scrollView.Widget.sizeDelta.x * Vector2.right + scrollHei * Vector2.up;
                float bottomPos = -scrollHei / 2f;
                this.m_scrollItem = new ChapterScrollItem[count];
                m_grid.EnsureSize<ChapterScrollItem>(count);
                for (int i = 0; i < count; i++)
                {
                    ChapterScrollItem scrollItem = m_grid.GetChild<ChapterScrollItem>(i);
                    this.m_scrollItem[i] = scrollItem;
                    if (i == 0)
                    {
                        scrollItem.Widget.anchoredPosition = Vector2.up * (m_itemHei * i + m_itemHei / 2f);
                    }
                    else
                    {
                        scrollItem.Widget.anchoredPosition = Vector2.up * ((m_itemHei + m_spaceHei) * i + m_itemHei / 2f);
                    }
                    scrollItem.SetChapterID(m_allChapterInfoList[i].ChapterID, m_allChapterInfoList[i].ChapterStatus != ChapterStatus.LOCK);
                    scrollItem.Visible = true;
                }

                Vector3 startPos = GetCenterPosition(displayChapterIndex);
                this.m_gridView.Widget.localPosition = startPos;
                this.m_buttonRoot.SetData(m_allChapterInfoList[displayChapterIndex]);
                this.m_buttonRoot.Visible = true;
            }

        }

        private void OnDrag(GameObject go, Vector2 delta, Vector2 pos)
        {
            if (rotateTweener != null)
            {
                rotateTweener.Kill();
            }
            m_gridView.Widget.localPosition += delta.y * Vector3.Normalize(m_gridView.Widget.up);
            //ReflashItem();
            //for (int i = 0; i < this.m_scrollItem.Length; i++)
            //{
            //    float Yfactor = Mathf.Abs(m_scrollItem[i].Widget.localPosition.y - boundLocalPos.y);
            //    Yfactor = Mathf.Clamp01(Yfactor / m_itemHei);
            //    m_scrollItem[i].Widget.localScale = Vector3.Lerp(Vector3.one * 1.1f, Vector3.one, Yfactor);

            //    m_scrollItem[i].Widget.anchoredPosition = Mathf.Lerp(m_xShift, 0f, Yfactor) * Vector2.right + m_scrollItem[i].Widget.anchoredPosition.y * Vector2.up;
            //}

        }

        private void ReflashItem()
        {
            Vector3 boundLocalPos = m_gridView.Widget.InverseTransformPoint(this.m_centerTran.position);
            for (int i = 0; i < this.m_scrollItem.Length; i++)
            {
                float Yfactor = Mathf.Abs(m_scrollItem[i].Widget.localPosition.y - boundLocalPos.y);
                Yfactor = Mathf.Clamp01(Yfactor / m_itemHei);
                if (Yfactor <= 0.5f)
                {
                    displayChapterIndex = i;
                    break;
                }
                //m_scrollItem[i].Widget.localScale = Vector3.Lerp(Vector3.one * 1.1f, Vector3.one, Yfactor);
                //m_scrollItem[i].Widget.anchoredPosition = Mathf.Lerp(m_xShift, 0f, Yfactor) * Vector2.right + m_scrollItem[i].Widget.anchoredPosition.y * Vector2.up;

            }
        }

        private void OnDragStart(GameObject go, Vector2 delta)
        {
            //this.m_buttonRoot.SetData(m_allChapterInfoList[displayChapterIndex]);
            this.m_buttonRoot.Visible = false;
        }
        private Tween rotateTweener = null;
        private int lastDisplayChapterIndex = 0;
        private void OnDragEnd(GameObject go, Vector2 delta)
        {
            Vector3 boundLocalPos = m_gridView.Widget.InverseTransformPoint(this.m_centerTran.position);
            float minFactor = 1000f;
            int minIndex = 0;
            for (int i = 0; i < this.m_scrollItem.Length; i++)
            {
                float Yfactor = Mathf.Abs(m_scrollItem[i].Widget.localPosition.y - boundLocalPos.y);
                if (Yfactor < minFactor)
                {
                    minFactor = Yfactor;
                    minIndex = i;
                }
            }
            displayChapterIndex = minIndex;
            if (lastDisplayChapterIndex != displayChapterIndex)
            {
                lastDisplayChapterIndex = displayChapterIndex;
                this.m_buttonRoot.SetData(m_allChapterInfoList[displayChapterIndex]);
            }
            Vector3 centerPos = GetCenterPosition(minIndex);
            if (rotateTweener != null)
            {
                rotateTweener.Kill();
            }
            rotateTweener = m_gridView.Widget.DOLocalMove(centerPos, 0.5f).OnComplete(OnRotateTweenerTweening);
        }

        private Vector3 GetCenterPosition(int itemIndex)
        {
            Vector3 itemPos = m_scrollView.Widget.InverseTransformPoint(this.m_scrollItem[itemIndex].Position);
            itemPos.z = 0f;
            Vector3 boundPos = m_scrollView.Widget.InverseTransformPoint(this.m_centerTran.position);
            boundPos.z = 0f;

            float distance = Vector3.Distance(itemPos, boundPos);
            if (itemPos.y > boundPos.y)
            {
                distance = -distance;
            }
            Vector3 centerPos = m_gridView.Widget.anchoredPosition3D + Vector3.Normalize(m_gridView.Widget.up) * distance;
            return centerPos;
        }

        private void OnRotateTweenerTweening()
        {
            this.m_buttonRoot.Visible = true;
            //ReflashItem();
        }

    }

    public class ChapterScrollItem : GameUIComponent
    {
        private long m_chapterID = 0;
        private GameTexture m_chapterCover = null;

        protected override void OnInit()
        {
            this.m_chapterCover = Make<GameTexture>("Image_bg:RawImage");
        }

        public void SetChapterID(long chapterID, bool status)
        {
            this.m_chapterID = chapterID;
            ConfChapter chapterData = ConfChapter.Get(chapterID);
            if (chapterData != null)
            {
                this.m_chapterCover.TextureName = chapterData.cover;
                this.m_chapterCover.SetGray(!status);
            }
        }
    }

    public class ChapterChooseItem : GameUIComponent
    {
        private GameTexture m_texture = null;
        private GameLabel m_chapterName;
        private GameLabel m_chapterID;
        private GameImage m_btnChoose;
        private ChapterInfo m_currentSelectChapterInfo;
        private Material m_specialGraymaterial = null;
        private GameImage m_btnDownChapter = null;

        public Action m_chooseAction = null;
        private bool m_isDownloading = false;

        protected override void OnInit()
        {
            base.OnInit();
            this.m_texture = Make<GameTexture>("Panel:Image_33:RawImage");
            this.m_chapterName = Make<GameLabel>("Image_bg_3:chapterName");
            this.m_chapterID = Make<GameLabel>("Image_bg_3:chapterID");
            this.m_btnChoose = Make<GameImage>("Image_bg_3:Button");
            this.m_btnDownChapter = Make<GameImage>("Image_bg_3:Button_download");
            this.m_specialGraymaterial = new Material(ShaderModule.Instance.GetShader("UI/Gray"));

        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            this.m_btnChoose.AddClickCallBack(OnChooseItem);
            this.m_btnDownChapter.AddClickCallBack(OnBtnDownloadChapterClick);
        }

        public override void OnHide()
        {
            base.OnHide();
            this.m_btnChoose.RemoveClickCallBack(OnChooseItem);
            this.m_btnDownChapter.RemoveClickCallBack(OnBtnDownloadChapterClick);

        }

        public override void Dispose()
        {
            base.Dispose();

            GameObject.Destroy(this.m_specialGraymaterial);
        }

        private void OnChooseItem(GameObject obj)
        {
            if (this.m_currentSelectChapterInfo.ChapterStatus == ChapterStatus.LOCK)
            {
                return;
            }
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

            GUIFrame uiChapterFrame = EngineCoreEvents.UIEvent.GetFrameEvent.SafeInvoke(UIDefine.UI_CHAPTER);
            if (uiChapterFrame != null && uiChapterFrame.Visible)
                (uiChapterFrame.LogicHandler as ChapterUILogic).SetChapterID(this.m_currentSelectChapterInfo.ChapterID);
            else
                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(new FrameMgr.OpenUIParams(UIDefine.UI_CHAPTER) { Param = this.m_currentSelectChapterInfo.ChapterID });

            Dictionary<UBSParamKeyName, object> selectChapterDict = new Dictionary<UBSParamKeyName, object>();
            selectChapterDict.Add(UBSParamKeyName.Select_ChapterID, this.m_currentSelectChapterInfo.ChapterID);
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.file_choice, null, selectChapterDict);
            m_chooseAction();
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_ChapterMap);
        }

        private void OnBtnDownloadChapterClick(GameObject btnDownloadChapter)
        {
            if (m_isDownloading)
                return;

            if (GlobalInfo.MY_PLAYER_INFO.PlayerChapterSystem.IsChapterAssetExist(this.m_currentSelectChapterInfo.ChapterID))
                ChapterDownloadFinish();
            else
            {
                List<string> chapterAssetList = GlobalInfo.MY_PLAYER_INFO.PlayerChapterSystem.GetChapterDynamicAssetList(this.m_currentSelectChapterInfo.ChapterID);

                ResourceMgr.Instance.RequestDynamicAssets(chapterAssetList.ToArray(), SetDownloadProgress, finish =>
                {
                    m_isDownloading = true;
                    ChapterDownloadFinish();
                });

            }
        }


        public void SetData(ChapterInfo chapterInfo)
        {
            this.m_currentSelectChapterInfo = chapterInfo;
            this.m_texture.TextureName = chapterInfo.ChapterConfData.cover;
            this.m_texture.SetMaterial(this.m_specialGraymaterial);

            this.m_chapterName.Text = LocalizeModule.Instance.GetString(chapterInfo.ChapterConfData.name);
            this.m_chapterID.Text = "Case NO." + chapterInfo.ChapterConfData.id;
            bool status = GlobalInfo.MY_PLAYER_INFO.PlayerChapterSystem.IsChapterAssetExist(chapterInfo.ChapterID);
            bool isPreviousChapterDone = true;

            if (chapterInfo.ChapterID >= 2)
                isPreviousChapterDone = GlobalInfo.MY_PLAYER_INFO.PlayerChapterSystem.ChapterSet.GetChapterInfoByID(chapterInfo.ChapterID - 1).ChapterStatus == ChapterStatus.DONE;


            this.m_btnDownChapter.Visible = !status;
            this.m_btnDownChapter.SetGray(!isPreviousChapterDone);
            this.m_btnDownChapter.EnableClick = isPreviousChapterDone;

            this.m_btnChoose.Visible = status;
            this.m_btnChoose.SetGray(!isPreviousChapterDone);
            this.m_btnChoose.EnableClick = status && isPreviousChapterDone;

            this.m_specialGraymaterial.SetFloat("_GrayFactor", status ? 1 : 0);
            this.m_texture.Visible = false;         //todo:不知道为啥对于RawImage 修改材质时，不实时刷新,应该是MaskableGraphic的问题 后续需要看一下
            this.m_texture.Visible = true;
        }

        private void SetDownloadProgress(AssetUpdateInfo chapterDownloadInfo)
        {
            this.m_texture.RawImage.material.SetFloat("_GrayFactor", chapterDownloadInfo.DownloadProgress);
            this.m_texture.Visible = false;
            this.m_texture.Visible = true;
        }

        private void ChapterDownloadFinish()
        {
            SetData(this.m_currentSelectChapterInfo);
            this.m_texture.RawImage.material.SetFloat("_GrayFactor", 1.0f);
            this.m_texture.Visible = false;
            this.m_texture.Visible = true;

            m_isDownloading = false;
            GameEvents.ChapterEvents.OnChapterDownloadFinish.SafeInvoke(this.m_currentSelectChapterInfo);
        }
    }
}
