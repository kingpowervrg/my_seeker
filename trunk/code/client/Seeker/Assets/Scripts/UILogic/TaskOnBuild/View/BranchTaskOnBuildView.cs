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
     class BranchTaskOnBuildView : TaskOnBuildView
    {
       
        public override void Refresh( List<TaskBase> tasks_, int task_type_ )
        {
            base.Refresh(tasks_, task_type_);

           

            m_task_grid.EnsureSize<TaksOnBuildItemView>(tasks_.Count);

            for(int i = 0; i < m_task_grid.ChildCount; ++i)
            {
                var item = m_task_grid.GetChild<TaksOnBuildItemView>(i);
                item.SetTaskInfo(tasks_[i] as NormalTask, CurViewLogic().GetCanvas());
            }

        }

       
       
    }
}
