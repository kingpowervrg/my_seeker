using System;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using Google.Protobuf;

namespace SeekerGame
{

    public class WinFailData
    {

        public ENUM_SEARCH_MODE m_mode;
        public IMessage m_msg;

        public WinFailData( ENUM_SEARCH_MODE mode_, IMessage msg_)
        {
            m_mode = mode_;
            m_msg = msg_;
        }



    }


}

