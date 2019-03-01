using GOGUI;
using UnityEngine;
using Spine.Unity;
using Spine;
using System;

namespace EngineCore
{
    public class GameSpine : GameUIComponent
    {
        protected SkeletonGraphic spine;
        protected ResStatus m_assetLoadStatus = ResStatus.NONE;     //资源加载状态
        public Action<SkeletonDataAsset> LoadSpineComplete; //spine加载完成

        private string cacheAnim = string.Empty;
        private bool cahcheLoop = false;
        protected override void OnInit()
        {
            base.OnInit();
            this.spine = GetComponent<SkeletonGraphic>();
            if (this.spine.skeletonDataAsset != null)
            {
                this.m_currentAnim = this.spine.startingAnimation;
                m_spineName = spine.skeletonDataAsset.name;
            }
        }

        private string m_spineName = string.Empty;

        public string SpineName
        {
            get
            {
                if (spine.skeletonDataAsset == null)
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(m_spineName))
                {
                    return spine.skeletonDataAsset.name + ".asset";
                }
                return m_spineName;
            }

            set
            {
                if (string.IsNullOrEmpty(value) || m_spineName == value)
                {
                    return;
                }
                spine.CrossFadeAlpha(0f, 0f, true);
                this.m_playAuto = false;
                this.cacheAnim = string.Empty;
                this.cahcheLoop = false;
                this.m_currentAnim = string.Empty;
                m_spineName = value;
                LoadRes();
            }
        }

        private string m_currentAnim = string.Empty;
        public string CurrentAnimationName
        {
            get
            {
                return this.m_currentAnim;
            }
        }

        public bool isLoop
        {
            get { return spine.startingLoop; }
            set
            {
                spine.startingLoop = value;
            }
        }
        private bool m_playAuto = false;

        public void PlayAnimation(string animName = "", bool isLoop = false)
        {
            this.isLoop = isLoop;
            if (spine.SkeletonData == null || m_assetLoadStatus == ResStatus.WAIT || m_assetLoadStatus == ResStatus.ERROR)
            {
                this.cacheAnim = animName;
                this.cahcheLoop = isLoop;
                this.m_playAuto = true;
                return;
            }
            if (string.IsNullOrEmpty(animName))
            {
                if (spine.SkeletonData.Animations.Count > 0)
                {
                    Spine.Animation itemAnimation = spine.SkeletonData.Animations.Items[0];
                    spine.AnimationState.SetAnimation(0, itemAnimation, isLoop);
                    this.m_currentAnim = itemAnimation.Name;
                }
            }
            else
            {
                Spine.Animation animation = spine.SkeletonData.FindAnimation(animName);
                if (animation != null)
                {
                    this.m_currentAnim = animation.Name;
                    spine.AnimationState.SetAnimation(0, animation, isLoop);
                }
            }
        }

        public void Play()
        {
            PlayAnimation();
        }

        public void StopAnimation(bool clear_ = false)
        {
            this.cacheAnim = string.Empty;
            this.cahcheLoop = false;
            this.m_playAuto = false;
            if (spine.SkeletonData == null || m_assetLoadStatus == ResStatus.WAIT || m_assetLoadStatus == ResStatus.ERROR)
            {
                return;
            }
            spine.AnimationState.SetEmptyAnimation(0, 0);

            if (clear_)
                spine.AnimationState.ClearTracks();
        }

        private void LoadRes()
        {
            if (this.m_assetLoadStatus == ResStatus.NONE || this.m_assetLoadStatus == ResStatus.OK || this.m_assetLoadStatus == ResStatus.ERROR)
            {
                LoadRes(this.m_spineName);
            }
        }

        private void LoadRes(string resName)
        {
            ReleaseSpine();
            this.m_assetLoadStatus = ResStatus.WAIT;
            EngineCoreEvents.ResourceEvent.GetAssetEvent.SafeInvoke(resName, LoadResComplete, LoadPriority.Prior);
        }

        private void LoadResComplete(string assetName, UnityEngine.Object assetObj)
        {
            if (assetObj == null)
            {
                this.m_assetLoadStatus = ResStatus.ERROR;
            }
            else
            {
                this.m_assetLoadStatus = ResStatus.OK;
                if (!assetName.Equals(this.m_spineName))
                {
                    GOGUITools.ReleaseAssetAction.SafeInvoke(assetName, assetObj);
                    GOGUITools.GetAssetAction.SafeInvoke(this.m_spineName, LoadResComplete, LoadPriority.Prior);
                    return;
                }
                SkeletonDataAsset skeleton = assetObj as SkeletonDataAsset;
                this.spine.skeletonDataAsset = skeleton;
                //加载上来直接播放
                //this.spine.startingAnimation = string.Empty;
                this.spine.Initialize(true);
                if (!string.IsNullOrEmpty(this.cacheAnim) || this.m_playAuto)
                {
                    PlayAnimation(this.cacheAnim, this.cahcheLoop);
                }
                if (LoadSpineComplete != null)
                {
                    LoadSpineComplete(skeleton);
                }
                spine.CrossFadeAlpha(1f, 0f, true);
                //Debug.Log("load spine complete " + assetName + " anim : " + cacheAnim);
            }
        }

        private void ReleaseSpine()
        {
            if (!string.IsNullOrEmpty(SpineName) && spine && spine.skeletonDataAsset && this.m_assetLoadStatus == ResStatus.OK)
            {
                GOGUITools.ReleaseAssetAction.SafeInvoke(SpineName, spine.skeletonDataAsset);
                this.m_assetLoadStatus = ResStatus.NONE;
                spine.skeletonDataAsset = null;
            }
        }

        void OnDestroy()
        {
            ReleaseSpine();
        }

        public SkeletonGraphic Spine
        {
            get { return spine; }
        }
    }
}
