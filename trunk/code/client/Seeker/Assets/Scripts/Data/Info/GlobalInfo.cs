#define WAV
using EngineCore;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class GlobalInfo : Singleton<GlobalInfo>
    {
        public static long MY_PLAYER_ID;
        public static string SERVER_ADDRESS;            //服务器地址
        public static string ACCOUNT_TOKEN = string.Empty;

        public static GameNetworkMode GAME_NETMODE
        {
            get { return m_gameNetMode; }
            set { m_gameNetMode = value; }
        }
        private static GameNetworkMode m_gameNetMode = GameNetworkMode.Network;

        //当前音乐
        private static Audio m_currentMusic = null;

        public static Dictionary<string, string> ConstAudioKeyDict = new Dictionary<string, string>();

        public void Init()
        {
            EngineCore.EngineCoreEvents.BridgeEvent.GetServerAddress = () =>
            {
                return SERVER_ADDRESS;
            };

            EngineCore.EngineCoreEvents.SystemEvents.FetchUserIdentification = () =>
            {
                return ACCOUNT_TOKEN;
            };

            Enable_Music = PlayerPrefTool.GetBool(ENUM_PREF_KEY.E_ENABLE_MUSIC.ToString(), true);
            Enable_Sound = PlayerPrefTool.GetBool(ENUM_PREF_KEY.E_ENABLE_SOUND.ToString(), true);

            EngineCore.EngineCoreEvents.AudioEvents.FetchAudioPathByKey = FetchAudioPathByKey;
            GameEvents.System_Events.PlayMainBGM = PlayMainAudio;


            InitAudioBridge();

            InitDefaultAudioKey();
        }


        private void InitDefaultAudioKey()
        {
            ConstAudioKeyDict.Add("Button_Click_Common", "tongyong_dianjianniu.mp3");
            ConstAudioKeyDict.Add("Close_Window", "tongyong_guanbijiemian.mp3");
        }

        #region 单机模式下个人信息
        private static PlayerInfo Standalone_PLAYER_INFO = null;

        public void CreatePlayerInfo()
        {
            Standalone_PLAYER_INFO = new PlayerInfo(-1);

            Standalone_PLAYER_INFO.SetCash(0).SetCoin(0).SetExp(0).SetExpMultiple(0)
                .SetIcon("").SetLaborUnionn(0)
                .SetLevel(1).SetUpgradeExp(0).SetVit(105);
            if (NewGuid.GuidNewManager.Instance.GetProgressByIndex(3))
            {
                Standalone_PLAYER_INFO.SetLevel(2);
            }
            #region 背包数据
            //Standalone_PLAYER_INFO.AddBagInfo(1);
            //Standalone_PLAYER_INFO.AddBagInfo(41);
            #endregion
#if OFFICER_SYS
            Standalone_PLAYER_INFO.AddOfficerInfo(121);
#endif

            Standalone_PLAYER_INFO.PlayerNickName = "ME";
            Standalone_PLAYER_INFO.HasRenamed = false;
            PlayerInfoManager.Instance.AddPlayerInfo(-1, Standalone_PLAYER_INFO);
        }
        #endregion

        public static PlayerInfo MY_PLAYER_INFO
        {
            get
            {
                if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Standalone)
                {
                    return Standalone_PLAYER_INFO;
                }
                return PlayerInfoManager.Instance.GetPlayerInfo(MY_PLAYER_ID);
            }
        }

        /// <summary>
        /// 玩家Token
        /// </summary>
        /// <param name="accountToken"></param>
        public static void SetAccountToken(string accountToken)
        {
            ACCOUNT_TOKEN = accountToken;
        }

        /// <summary>
        /// 开启音效
        /// </summary>
        public static bool Enable_Sound
        {
            get
            {
                return AudioModule.Instance.GlobalUISoundsVolume > 0;
            }
            set
            {
                PlayerPrefTool.SetBool(ENUM_PREF_KEY.E_ENABLE_SOUND.ToString(), value);

                AudioModule.Instance.GlobalUISoundsVolume = value ? 1 : 0;
                AudioModule.Instance.GlobalSoundsVolume = value ? 1 : 0;

                if (UserBehaviorStatisticsModules.Instance != null)
                {
                    Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();
                    _param.Add(UBSParamKeyName.Success, value ? 1 : 0);
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.user_soundeffect, null, _param);
                }
            }
        }

        /// <summary>
        /// 开启音乐 
        /// </summary>
        public static bool Enable_Music
        {
            get
            {
                return AudioModule.Instance.GlobalMusicVolume > 0;
            }
            set
            {
                PlayerPrefTool.SetBool(ENUM_PREF_KEY.E_ENABLE_MUSIC.ToString(), value);
                GameEvents.System_Events.EnableMusicEvent.SafeInvoke(value);

                AudioModule.Instance.GlobalMusicVolume = value ? 1 : 0;


                if (UserBehaviorStatisticsModules.Instance != null)
                {
                    Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();
                    _param.Add(UBSParamKeyName.Success, value ? 1 : 0);
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.user_music, null, _param);
                }
            }
        }

        /// <summary>
        /// 开启购买
        /// </summary>
        public static bool Enable_Purchase
        {
            get
            {
                return PlayerPrefTool.GetBool(ENUM_PREF_KEY.E_ENABLE_PURCHASE.ToString(), false);
            }
            set
            {
                PlayerPrefTool.SetBool(ENUM_PREF_KEY.E_ENABLE_PURCHASE.ToString(), value);
                GameEvents.System_Events.EnablePurchaseEvent.SafeInvoke(value);

                if (UserBehaviorStatisticsModules.Instance != null)
                {
                    Dictionary<UBSParamKeyName, object> _param = new Dictionary<UBSParamKeyName, object>();
                    _param.Add(UBSParamKeyName.Success, value ? 1 : 0);
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.user_purchaselimit, null, _param);
                }
            }
        }

        /// <summary>
        /// 通过Audio Key获取Audio文件路径
        /// </summary>
        /// <param name="audioKey"></param>
        /// <returns></returns>
        public static string FetchAudioPathByKey(string audioKey)
        {
            ConfSound audioConf = ConfSound.Get(audioKey);
#if MP3
            return audioConf != null ? audioConf.SoundPath : string.Empty;
#elif WAV
            return audioConf != null ? audioConf.WavPath : string.Empty;
#endif
        }

        /// <summary>
        /// 播放主背景音乐
        /// </summary>
        /// <param name="isPlay"></param>
        private void PlayMainAudio(bool isPlay)
        {
            if (isPlay)
                PlayMusic("Main_UI");
        }



        private string GetAudioPath(string sn_)
        {
            if (ConstAudioKeyDict.ContainsKey(sn_))
                return ConstAudioKeyDict[sn_];

            ConfSound s = ConfSound.Get(sn_);

#if MP3
           return null != s ? s.SoundPath : null;
#elif WAV
            return null != s ? s.WavPath : null;
#endif
        }

        public void InitAudioBridge()
        {
            if (EngineCoreEvents.AudioEvents.Usr_GetAudioPathBySn.IsNull)
                EngineCoreEvents.AudioEvents.Usr_GetAudioPathBySn += GetAudioPath;
        }


        public void PlayMusic(string music_sn_)
        {
            EngineCoreEvents.AudioEvents.PlayAndGetAudio.SafeInvoke(Audio.AudioType.Music, music_sn_, (audio) =>
            {
                m_currentMusic = audio;
            });
        }

        /// <summary>
        /// 游戏版本
        /// </summary>
        public static string GAME_VERSION
        {
            get; set;
        }
    }


}

