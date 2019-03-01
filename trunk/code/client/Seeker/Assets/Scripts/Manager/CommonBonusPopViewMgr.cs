/// <summary>
/// 管理所有弹出类，奖励页面的展示排序
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class CommonBonusPopViewMgr : Singleton<CommonBonusPopViewMgr>
    {
        Queue<EUNM_BONUS_POP_VIEW_TYPE> m_view_queue;
        bool m_is_one_bonus_showing = false;

        public CommonBonusPopViewMgr()
        {
            m_view_queue = new Queue<EUNM_BONUS_POP_VIEW_TYPE>();
            SortBonus();

            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnCache += EnqueueBonus;
            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnShow += ShowBonus;
            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnBlock += BlockBonus;
        }

        public void ClearBonus()
        {
            m_view_queue.Clear();
            m_is_one_bonus_showing = false;
        }

        private void EnqueueBonus(EUNM_BONUS_POP_VIEW_TYPE bonus_)
        {
            if (!m_view_queue.Contains(bonus_))
                m_view_queue.Enqueue(bonus_);
        }

        private void ShowBonus()
        {
            if (m_is_one_bonus_showing)
            {
                return;
            }

            if (0 == m_view_queue.Count)
                return;

            //BlockBonus(true);

            SortBonus();
            var view = m_view_queue.Dequeue();

            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Listen_OnShow.SafeInvoke(view);
        }

        private void BlockBonus(bool b_)
        {
            m_is_one_bonus_showing = b_;
        }

        private void SortBonus()
        {
            if (m_view_queue.Count > 0)
            {
                var datas = m_view_queue.OrderBy(item => (int)item).ToList();

                m_view_queue.Clear();

                foreach (var item in datas)
                {
                    m_view_queue.Enqueue(item);
                }

            }
        }



    }
}
