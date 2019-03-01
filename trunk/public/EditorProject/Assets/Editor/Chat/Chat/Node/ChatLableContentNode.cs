using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chat
{
    public class ChatLableContentNode : ChatBaseNode
    {
        ChatLink m_chatlink;
        public override void Construct()
        {
            m_chatlink = MakeLet<ChatLink>(true);
        }
    }
}
