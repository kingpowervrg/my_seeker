//#define TEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using UnityEngine;
using GOEngine;
using Google.Protobuf;
using DG.Tweening;
using DG.Tweening.Plugins.Options;

namespace SeekerGame
{
    public class WinLvlUpDetailView : GameUIComponent
    {
        GameImage m_lvl_icon;
        GameLabel m_lvl_name;
        GameProgressBar m_lvl_exp_slider;
        GameLabel m_lvl_exp_txt;


        protected override void OnInit()
        {
            base.OnInit();

            m_lvl_icon = Make<GameImage>("Panel_level:Image");
            m_lvl_name = Make<GameLabel>("Panel_level:Text");
            m_lvl_exp_slider = Make<GameProgressBar>("Panel_level:Slider");
            m_lvl_exp_txt = Make<GameLabel>("Panel_level:Text (1)");
        }


        public override void OnShow(object param)
        {
            base.OnShow(param);

        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public void Refresh(WinFailData data_)
        {

            if (ENUM_SEARCH_MODE.E_SEARCH_ROOM == data_.m_mode)
            {
                SCSceneRewardResponse msg = data_.m_msg as SCSceneRewardResponse;
                long m_scene_group_id = FindObjSceneDataManager.ConvertSceneIdToSceneGroupId(msg.SceneId);
                var m_group_data = new FindObjSceneData(m_scene_group_id, 1, 0);

                var temp_data = FindObjSceneDataManager.Instance.GetDataBySceneGroupID(m_scene_group_id);
                if (null != temp_data)
                    m_group_data = temp_data;

                long scene_difficult_id = FindObjSceneDataManager.GetSceneDifficultID(m_scene_group_id, m_group_data.m_lvl);
                ConfSceneDifficulty difficult_data = ConfSceneDifficulty.Get(scene_difficult_id);
                m_lvl_icon.Sprite = ConfPoliceRankIcon.Get(m_group_data.m_lvl).icon;
                m_lvl_name.Text = LocalizeModule.Instance.GetString(difficult_data.name);
                m_lvl_exp_slider.Value = (float)m_group_data.m_exp / (float)m_group_data.m_full_exp;
                m_lvl_exp_txt.Text = $"{m_group_data.m_exp}/{m_group_data.m_full_exp}";
            }
        }



    }





}