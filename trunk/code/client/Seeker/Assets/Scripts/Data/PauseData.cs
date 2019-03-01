using System;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using Google.Protobuf;

namespace SeekerGame
{
    class PauseData
    {
        public ENUM_SEARCH_MODE m_mode;
        public long m_id;
        public bool m_hidePause = false;
    }
}
