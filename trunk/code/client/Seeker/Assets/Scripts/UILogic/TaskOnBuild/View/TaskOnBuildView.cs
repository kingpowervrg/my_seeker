//#define TEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using UnityEngine;
using UnityEngine.UI;


namespace SeekerGame
{
    class TaskOnBuildView : BaseViewComponet<TaskOnBuildUILogic>
    {
        protected GameUIContainer m_task_grid;
        protected int m_task_type;
        protected override void OnInit()
        {
            base.OnInit();

            m_task_grid = Make<GameUIContainer>("Grid");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public virtual void Refresh(List<TaskBase> tasks_, int task_type_)
        {
            m_task_type = task_type_;

            m_task_grid.EnsureSize<TaksOnBuildItemView>(tasks_.Count);

            for (int i = 0; i < m_task_grid.ChildCount; ++i)
            {
                var item = m_task_grid.GetChild<TaksOnBuildItemView>(i);
                item.SetTaskInfo(tasks_[i] as NormalTask, CurViewLogic().GetCanvas());
                item.Visible = true;
            }
        }

        public void RefreshPos()
        {
            for (int i = 0; i < m_task_grid.ChildCount; ++i)
            {
                var item = m_task_grid.GetChild<TaksOnBuildItemView>(i);
                item.RefreshPos();
            }
        }

    }
}
