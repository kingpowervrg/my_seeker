using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;
using System.Linq;
namespace SeekerGame
{
    /// <summary>
    /// 待寻找的物品
    /// </summary>
    public class FindingItem : GameUIComponent
    {
        private const float C_NORMAL_HEIGHT = 46.0f;
        private const float C_SHADOW_HEIGHT = 76.0f;

        private static Material m_shadowMat = new Material(ShaderModule.Instance.GetShader("Seeker/UI_Silhouette"));

        private GameImage m_imgItemIcon;
        private GameLabel m_lbItemName;
        private string m_itemEntityId = string.Empty;
        private GameUIEffect m_itemPickedEffect = null;
        private GameUIEffect m_itemPickedEffectFanci = null;
        private GameImage m_ImgItemBackground = null;
        private GameImage m_shadow_icon = null;
        string findingItemOriginName = string.Empty;
        private GameUIEffect m_fanciEffect = null;
        private bool m_isFanci = false;
        private long m_exhibitID;

        private bool m_lock = false;

        private GameImage m_LockImg = null;
        protected override void OnInit()
        {
            this.m_imgItemIcon = Make<GameImage>(gameObject);
            this.m_lbItemName = Make<GameLabel>("Text");
            this.m_itemPickedEffect = Make<GameUIEffect>("UI_feizhi_wupinlan");
            this.m_ImgItemBackground = Make<GameImage>("Image");
            this.m_shadow_icon = Make<GameImage>("Image_shadow");
            this.m_fanciEffect = Make<GameUIEffect>("UI_fanci_bianzheng");
            this.m_itemPickedEffectFanci = Make<GameUIEffect>("UI_feizhi_wupinlan_jianying");
            this.m_LockImg = Make<GameImage>("Lock");
            this.m_shadow_icon.GetSprite().material = m_shadowMat;

        }

        public void InitUIEffect(FindingMode mode_)
        {
            if (FindingMode.SHADOW == mode_)
            {
                this.m_itemPickedEffectFanci.EffectPrefabName = "UI_feizhi_wupinlan_jianying.prefab";
                this.m_itemPickedEffectFanci.Visible = false;

            }
            else
            {
                this.m_itemPickedEffect.EffectPrefabName = "UI_feizhi_wupinlan.prefab";
                this.m_itemPickedEffect.Visible = false;
            }


        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            this.m_itemPickedEffect.Visible = false;
            this.m_itemPickedEffectFanci.Visible = false;
            if (UILogicHandler.CurrentGameScene.ItemFindingMode == SceneItemFindingMode.OneItemMultipleTime)
                UILogicHandler.OnRefreshFindingProgress += OnOneItemMultipleTimeProgressChanged;
            //Vector3 screenPos = FrameMgr.Instance.UICamera.WorldToScreenPoint(Widget.position);
            //screenPos.z = 30f;
            //Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            //Debug.Log("worldPos : " + worldPos);
        }

        public void SetFindingItemData(string iconName, string itemKeyName, long exhibitID, bool isSpecialItem)
        {
            this.m_exhibitID = exhibitID;
            string itemName = LocalizeModule.Instance.GetString(itemKeyName);
            //Debug.Log("itemKeyName:" + itemKeyName + "  itemName:" + itemName);
            this.m_lbItemName.Visible = true;
            this.m_shadow_icon.Visible = false;
            this.m_ImgItemBackground.Widget.sizeDelta = new Vector2(this.m_ImgItemBackground.Widget.sizeDelta.x, C_NORMAL_HEIGHT);
            if (FindMode == FindingMode.REVERSE_NAME)
            {
                this.m_lbItemName.Text = new string(itemName.Reverse().ToArray());
                this.m_isFanci = true;
            }
            else if (FindMode == FindingMode.NORMAL)
                this.m_lbItemName.Text = itemName;
            else if (FindMode == FindingMode.SHADOW)
            {
                this.m_ImgItemBackground.Widget.sizeDelta = new Vector2(this.m_ImgItemBackground.Widget.sizeDelta.x, C_SHADOW_HEIGHT);
                this.m_lbItemName.Visible = false;
                this.m_shadow_icon.Sprite = iconName;
                this.m_shadow_icon.Visible = true;
                this.m_lbItemName.Text = itemName;
            }
            else
            {
                this.m_lbItemName.Text = itemName;
            }

            this.findingItemOriginName = this.m_lbItemName.Text;

            this.m_ImgItemBackground.Sprite = isSpecialItem ? "btn_common_2.png" : "btn_common_1.png";

            this.m_itemPickedEffect.Visible = false;
            this.m_itemPickedEffectFanci.Visible = false;
            //TimeModule.Instance.SetTimeout(()=> this.m_itemPickedEffect.Visible = false,0.2f);
        }

        public void LockFindItem(bool isLock)
        {
            this.m_LockImg.Visible = isLock;
            if (!isLock)
            {
                this.m_itemPickedEffect.ReplayEffect();
                this.m_itemPickedEffect.SetEffectHideTime(1f);
                //TimeModule.Instance.SetTimeout(()=>{ });
            }
            //this.m_ImgItemBackground.SetGray(isLock);
            //this.m_lbItemName.Visible = !isLock;
        }


        private void OnOneItemMultipleTimeProgressChanged(int foundNum, int totalNum)
        {
            this.m_lbItemName.Text = $"{this.findingItemOriginName }  {foundNum} / {totalNum}";
        }

        public void ReverseToNormal()
        {
            if (FindMode == FindingMode.REVERSE_NAME)
            {
                this.m_isFanci = false;
                this.m_fanciEffect.EffectPrefabName = "UI_fanci_bianzheng.prefab";
                this.m_fanciEffect.SetEffectHideTime(1f);
                this.m_fanciEffect.Visible = true;
                this.m_lbItemName.Text = new string(this.m_lbItemName.Text.Reverse().ToArray());
            }
        }

        public override void OnHide()
        {
            base.OnHide();
            this.m_fanciEffect.Visible = false;
            this.m_lock = false;
            if (UILogicHandler.CurrentGameScene.ItemFindingMode == SceneItemFindingMode.OneItemMultipleTime)
                UILogicHandler.OnRefreshFindingProgress -= OnOneItemMultipleTimeProgressChanged;
        }


        public string EntityId
        {
            get { return this.m_itemEntityId; }
            set { this.m_itemEntityId = value; }
        }

        public long ExhibitID
        {
            get
            {
                return m_exhibitID;
            }
        }

        public GameUIEffect PickedEffect
        {
            get
            {
                if (FindMode == FindingMode.SHADOW)
                {
                    return this.m_itemPickedEffectFanci;
                }
                else
                {
                    return this.m_itemPickedEffect;
                }
            }
        }


        public GameMainUILogic UILogicHandler => LogicHandler as GameMainUILogic;

        public FindingMode FindMode { get; set; }

        public bool IsFanci
        {
            get { return m_isFanci; }
            set { m_isFanci = value; }
        }
    }
}
