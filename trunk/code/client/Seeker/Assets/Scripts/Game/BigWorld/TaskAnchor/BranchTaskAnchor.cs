using UnityEngine;
using System.Collections.Generic;
using EngineCore;
using System;

namespace SeekerGame
{
    public class BranchTaskAnchor : TaskAnchor
    {
        private List<long> m_tasks;
        public BranchTaskAnchor( GameObject root_, string name_, List<long> task_ids_) : base( root_,name_)
        {
            m_tasks = task_ids_;
        }
    }
}
