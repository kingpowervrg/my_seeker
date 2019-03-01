#if UNITY_DEBUG
using EngineCore;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class GMModule : UILogicBase
    {
        public static List<GMCommandWrap> GMCommandWrapList = new List<GMCommandWrap>();
        public static Dictionary<GMCommandWrap, Action> GMCommandInjectorDict = new Dictionary<GMCommandWrap, Action>();

        public static void InitAllGMCommand()
        {
            if (GMCommandWrapList.Count > 0)
                return;

            List<ConfGMCMD> gmCmdList = ConfGMCMD.array;

            for (int i = 0; i < gmCmdList.Count; ++i)
            {
                string strCmdType = gmCmdList[i].messageName;
                Type messageRawType = Type.GetType(strCmdType, true, true);
                if (messageRawType == null)
                    Debug.LogError($"no message define:{strCmdType}");

                ConfGMCMD confCMD = gmCmdList[i];

                GMCommandWrap cmdWrap = new GMCommandWrap()
                {
                    CommandName = confCMD.messageName,
                    conf = confCMD,
                    MessageType = messageRawType
                };

                GMCommandWrapList.Add(cmdWrap);
            }
        }

        public override FrameDisplayMode UIFrameDisplayMode => FrameDisplayMode.WINDOWED;




        public class GMCommandWrap
        {
            public string CommandName;
            public Type MessageType;
            public ConfGMCMD conf;
            public Func<Dictionary<string, string>, List<IMessage>> GMCommandInjector = null;
        }
    }
}
#endif