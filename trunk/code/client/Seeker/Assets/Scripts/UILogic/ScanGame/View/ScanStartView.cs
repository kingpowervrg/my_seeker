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
    public class ScanStartView : BaseViewComponet<ScanGameUILogic>
    {

        GameLabel m_time_txt;
        GameLabel m_desc_txt;
        GameUIContainer m_examine_grid;

        protected override void OnInit()
        {
            base.OnInit();
            m_time_txt = Make<GameLabel>("Text");
            m_desc_txt = Make<GameLabel>("Text (1)");
            m_examine_grid = Make<GameUIContainer>("ScrollView:Viewport");
        }



        public override void OnShow(object param)
        {
            base.OnShow(param);

            long scan_id = 0;

            if (null != param)
            {
                List<long> my_param = param as List<long>;
                scan_id = my_param[0];

            }

            ConfFind scan_data = ConfFind.Get(scan_id);

            m_time_txt.Text = CommonTools.SecondToStringMMSS((double)scan_data.time);
            m_desc_txt.Text = LocalizeModule.Instance.GetString(scan_data.descs);

            Dictionary<int, HashSet<long>> scan_datas = ScanDataManager.Instance.Examin_clue_datas(scan_id);

            m_examine_grid.EnsureSize<ExamineItemView>(scan_datas.Keys.Count);

            int i = 0;
            foreach (var kvp in scan_datas)
            {
                int scan_type = kvp.Key;
                var item = m_examine_grid.GetChild<ExamineItemView>(i);

                item.Refresh(ConfFindTypeIcon.Get(scan_type).icon, $"{ConfFindTypeIcon.Get(scan_type).name} {kvp.Value.Count}");
                item.Visible = true;
                ++i;
            }
        }

        public override void OnHide()
        {
            base.OnHide();
        }


        public void SetTimeVisible(bool v_)
        {
            m_time_txt.Visible = v_;
        }



    }


}