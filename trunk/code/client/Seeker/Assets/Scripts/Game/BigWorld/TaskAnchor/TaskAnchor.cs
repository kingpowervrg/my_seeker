using UnityEngine;
using System.Collections.Generic;
using EngineCore;
using System;

namespace SeekerGame
{
    public class TaskAnchor
    {
        public GameObject m_root;
        public string m_name;

        public TaskAnchor( GameObject root_, string name_)
        {
            m_root = root_;
            m_name = name_;
        }
    }
}
