using EngineCore;
using GOEngine;
using System.Collections.Generic;
using System.Linq;
namespace SeekerGame
{
    public class PlayerInfo : BasePlayerInfo
    {
        #region 角色基本信息
        public string PlayerNickName
        {
            get; set;
        }

        private string m_playerIcon;
        public string PlayerIcon
        {
            get { return m_playerIcon; }
        }

        private string m_netWorkIcon;
        public string NetWorkIcon
        {
            get { return m_netWorkIcon; }
        }

        private int m_coin;
        public int Coin
        {
            get { return m_coin; }
        }
        private int m_cash;
        public int Cash
        {
            get { return m_cash; }
        }
        private int m_vit;
        public int Vit
        {
            get { return m_vit; }
        }
        private int m_level;
        public int Level
        {
            get { return m_level; }
        }
        private int m_exp;
        public int Exp
        {
            get { return m_exp; }
        }
        private int m_laborUnion;
        public int LaborUnion
        {
            get { return m_laborUnion; }
        }
        private int m_upgradeExp;
        public int UpgradeExp
        {
            get { return m_upgradeExp; }
        }
        float m_expMultiple;
        public float ExpMultiple
        {
            get { return m_expMultiple; }
        }

        float m_achieve;
        public float Achieve
        {
            get { return m_achieve; }
        }

        public string PlayerTitle
        {
            get; set;
        }

        public long TitleID
        {
            get; set;
        } = 0L;

        /// <summary>
        /// 是否换过昵称
        /// </summary>
        public bool HasRenamed { get; set; } = false;


        public PlayerInfo(long player_id_)
        {
            this.m_player_id = player_id_;
        }

        public PlayerInfo SetIcon(string playerIcon_)
        {
            this.m_playerIcon = playerIcon_;
            if (PlayerPrefTool.GetUsername(ENUM_LOGIN_TYPE.E_THIRD) != null)
            {
                SetNetWorkIcon(playerIcon_);
            }
            return this;
        }

        public void SetNetWorkIcon(string netWorkIcon)
        {
            this.m_netWorkIcon = netWorkIcon;
        }

        public PlayerInfo SetCoin(int coin_)
        {
            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashCoin.SafeInvoke(m_coin, coin_);
            //GameEvents.UIEvents.UI_Achievement_Event.OnReflashAchievement.SafeInvoke();
            this.m_coin = coin_;
            return this;
        }

        public PlayerInfo ChangeCoin(int coin)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.coin_cost.ToString());

            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashCoin.SafeInvoke(m_coin, m_coin + coin);
            GameEvents.UIEvents.UI_Achievement_Event.OnReflashAchievement.SafeInvoke();
            this.m_coin += coin;
            return this;
        }

        public PlayerInfo SetCash(int cash_)
        {
            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashCash.SafeInvoke(m_cash, cash_);
            //GameEvents.UIEvents.UI_Achievement_Event.OnReflashAchievement.SafeInvoke();
            this.m_cash = cash_;
            return this;
        }

        public PlayerInfo ChangeCash(int cash)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.cash_cost.ToString());

            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashCash.SafeInvoke(m_cash, m_cash + cash);
            GameEvents.UIEvents.UI_Achievement_Event.OnReflashAchievement.SafeInvoke();
            this.m_cash += cash;
            return this;
        }

        public PlayerInfo SetVit(int vit_)
        {
            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashVit.SafeInvoke(m_vit, vit_);
            this.m_vit = vit_;
            return this;
        }

        public PlayerInfo ChangeVit(int vit)
        {
            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashVit.SafeInvoke(m_vit, m_vit + vit);
            this.m_vit += vit;
            return this;
        }

        public PlayerInfo SetLevel(int level_)
        {
            this.m_level = level_;
            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashLevel.SafeInvoke();
            return this;
        }

        public PlayerInfo SetExp(int exp_)
        {
            this.m_exp = exp_;
            GameEvents.UIEvents.UI_GameEntry_Event.OnReflashLevel.SafeInvoke();
            //TimeModule.Instance.SetTimeout(() => GameEvents.PlayerEvents.OnExpChanged.SafeInvoke(null), 5.0f);
            return this;
        }
        public PlayerInfo SetLaborUnionn(int laborUnion_)
        {
            this.m_laborUnion = laborUnion_;
            return this;
        }

        public PlayerInfo SetUpgradeExp(int upgradeExp_)
        {
            this.m_upgradeExp = upgradeExp_;
            return this;
        }

        public PlayerInfo SetExpMultiple(float expMultiple_)
        {
            this.m_expMultiple = expMultiple_;
            return this;
        }

        public PlayerInfo AddAchieve(float achieve)
        {
            this.m_achieve += achieve;
            return this;
        }
        #endregion

#if OFFICER_SYS
        #region 警员信息

        private long m_timestamp = 0L;
        public long OfficerTimestamp
        {
            get { return m_timestamp; }

        }
        private Dictionary<long, OfficerInfo> m_officer_dict = new Dictionary<long, OfficerInfo>();
        public System.Collections.Generic.Dictionary<long, OfficerInfo> Officer_dict
        {
            get { return m_officer_dict; }
        }

        public List<OfficerInfo> Officer_infos
        {
            get
            {
                List<OfficerInfo> temp = (m_officer_dict.Values).ToList<OfficerInfo>();

                temp.Sort(PoliceUILogicAssist.OfficerCompare);

                return temp;
            }
        }

public void ClearOfficerInfo()
        {
            //this.m_officer_infos.Clear();
            this.m_officer_dict.Clear();
        }

        public void AddOfficerInfo(OfficerInfo info_)
        {
            m_timestamp = System.DateTime.Now.Millisecond;
            if (m_officer_dict.ContainsKey(info_.OfficerId))
            {
                m_officer_dict[info_.OfficerId] = info_;
                return;
            }

            m_officer_dict.Add(info_.OfficerId, info_);

            //this.m_officer_infos.Add(info_);
        }

        public void AddOfficerInfo(long officerId)
        {
            OfficerInfo officerInfo = new OfficerInfo();
            officerInfo.PlayerOfficerId = officerId * 100;
            officerInfo.OfficerId = officerId;
            officerInfo.Level = 1;
            ConfOfficer confOfficer = ConfOfficer.Get(officerId);
            officerInfo.Outsight = confOfficer.outsight;
            officerInfo.WillPower = confOfficer.willpower;
            officerInfo.Attention = confOfficer.attention;
            officerInfo.Memory = confOfficer.memory;
            officerInfo.VitConsume = confOfficer.vitConsume;
            officerInfo.SecondGain = confOfficer.secondGain;
            AddOfficerInfo(officerInfo);
            //officerInfo.Outsight
        }

        public void SetOfficerInfos(IEnumerable<OfficerInfo> collection)
        {
            m_timestamp = System.DateTime.Now.Millisecond;

            this.ClearOfficerInfo();
            //this.m_officer_infos.AddRange(collection);

            foreach (OfficerInfo info in collection)
            {
                m_officer_dict.Add(info.OfficerId, info);
            }

        }

        public OfficerInfo GetOfficerInfo(long officer_id_)
        {
            OfficerInfo info;
            if (this.m_officer_dict.TryGetValue(officer_id_, out info))
            {
                return info;
            }

            return null;
        }

        public OfficerInfo GetOfficerInfoByPlayId(long playerOfficeId)
        {
            foreach (var kv in m_officer_dict)
            {
                if (kv.Value.PlayerOfficerId == playerOfficeId)
                {
                    return kv.Value;
                }
            }
            return null;
        }



        #endregion
#endif

        #region 章节信息
        private ChapterSystem m_playerChapterSystem = null;

        public void InitPlayerChapterSystem()
        {
            this.m_playerChapterSystem = new ChapterSystem(this);
            this.m_playerChapterSystem.InitPlayerChapterSystem();
        }

        public void ClearPlayerChapterSystem()
        {
            if (this.m_playerChapterSystem != null)
            {
                this.m_playerChapterSystem.Dispose();
                this.m_playerChapterSystem = null;
            }
        }

        public ChapterSystem PlayerChapterSystem => this.m_playerChapterSystem;

        #endregion

        #region 背包信息
        private Dictionary<long, PlayerPropMsg> m_bag_infos = new Dictionary<long, PlayerPropMsg>();
        public Dictionary<long, PlayerPropMsg> Bag_infos
        {
            get { return m_bag_infos; }
        }


        private Dictionary<long, PlayerPropMsg> m_recent_prop_infos = new Dictionary<long, PlayerPropMsg>();
        public Dictionary<long, PlayerPropMsg> Recent_Prop_infos
        {
            get { return m_recent_prop_infos; }
        }

        public void SyncPlayerBag()
        {
            CSPlayerPropRequest msg_prop = new CSPlayerPropRequest();
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(msg_prop);
        }

        public void ClearBagInfo()
        {
            this.m_bag_infos.Clear();
        }



        private void AddBagInfo(long id, int count)
        {
            GameEvents.UIEvents.UI_Bag_Event.Tell_OnPropIn.SafeInvoke(id);

            ConfProp conf_prop = ConfProp.Get(id);
            if ((int)PROP_TYPE.E_FUNC == conf_prop.type || (int)PROP_TYPE.E_CHIP == conf_prop.type
                || (int)PROP_TYPE.E_NROMAL == conf_prop.type || (int)PROP_TYPE.E_ENERGE == conf_prop.type
                || (int)PROP_TYPE.E_OFFICER == conf_prop.type || (int)PROP_TYPE.E_EXHABIT == conf_prop.type
                ||(int)PROP_TYPE.E_GIFT == conf_prop.type)
            {

                if (m_bag_infos.ContainsKey(id))
                {
                    m_bag_infos[id].Count += count;
                }
                else
                {
                    PlayerPropMsg prop = new PlayerPropMsg();
                    prop.PropId = id;
                    prop.Count = count;
                    m_bag_infos.Add(id, prop);
                }

                if (m_recent_prop_infos.ContainsKey(id))
                {
                    m_recent_prop_infos[id].Count += count;
                }
                else
                {
                    PlayerPropMsg prop = new PlayerPropMsg();
                    prop.PropId = id;
                    prop.Count = count;
                    m_recent_prop_infos.Add(id, prop);
                }
            }
        }



        public void AddSingleBagInfo(long id, int count)
        {


            AddBagInfo(id, count);

        }


        public void SetBagInfos(IEnumerable<PlayerPropMsg> collection)
        {
            this.ClearBagInfo();

            foreach (PlayerPropMsg prop in collection)
            {
                if (m_bag_infos.ContainsKey(prop.PropId))
                {
                    m_bag_infos[prop.PropId].Count += prop.Count;
                }
                else
                {
                    this.m_bag_infos.Add(prop.PropId, prop);
                }
            }
        }

        public PlayerPropMsg GetBagInfosByID(long id)
        {
            PlayerPropMsg prop = null;
            m_bag_infos.TryGetValue(id, out prop);
            return prop;
        }

        public void ReducePropForBag(long id)
        {
            ReducePropForBag(id, 1);
        }

        public void ReducePropForBag(long id, int count_)
        {
            if (m_bag_infos.ContainsKey(id))
            {
                m_bag_infos[id].Count -= count_;

                if (m_bag_infos[id].Count < 1)
                {
                    m_bag_infos.Remove(id);
                }

            }
        }



        public void ClearRecentPropInfo()
        {
            m_recent_prop_infos.Clear();
        }

        public void SetRecentPropInfos(IEnumerable<PlayerPropMsg> collection)
        {
            this.ClearRecentPropInfo();

            foreach (PlayerPropMsg prop in collection)
            {
                if (m_recent_prop_infos.ContainsKey(prop.PropId))
                {
                    m_recent_prop_infos[prop.PropId].Count += prop.Count;
                }
                else
                {
                    this.m_recent_prop_infos.Add(prop.PropId, prop);
                }
            }
        }

        #endregion

        #region 任务系统
        private TaskSystem m_playerTaskSystem = null;
        public void InitPlayerTaskSystem()
        {
            this.m_playerTaskSystem = new TaskSystem(this);

            CSTaskIdListRequest reqSyncTask = new CSTaskIdListRequest();
            //TODO : 没有任务，游戏就没法进行，所以初始化任务，要用同步。
            //GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(reqSyncTask);
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(reqSyncTask);
        }

        public void ClearPlayerTaskSystem()
        {
            if (this.m_playerTaskSystem != null)
            {
                this.m_playerTaskSystem.Dispose();
                this.m_playerTaskSystem = null;
            }
        }

        public TaskSystem PlayerTaskSystem => this.m_playerTaskSystem;
        #endregion

        #region 邮件系统
        private MailSystem m_playerMailSystem = null;

        public void InitPlayerMailSystem()
        {
            this.m_playerMailSystem = new MailSystem(this);
            this.m_playerMailSystem.SyncPlayerMails();
        }

        public MailSystem PlayerMailSystem => this.m_playerMailSystem;
        #endregion

        #region 天眼系统数据
        public bool m_skyEyehasCache = false;
        private List<long> m_skyHasReward = new List<long>();

        public void AddSkyEyeHasRewardById(long skyEyeId)
        {
            if (m_skyHasReward.Contains(skyEyeId))
            {
                UnityEngine.Debug.LogError("skyEyeId has exsit !!!  " + skyEyeId);
                return;
            }
            m_skyHasReward.Add(skyEyeId);
        }

        public bool IsSkyEyeRewardContainId(long skyEyeId)
        {
            return m_skyHasReward.Contains(skyEyeId);
        }

        public void ClearSkyEye()
        {
            this.m_skyHasReward.Clear();
            this.m_skyEyehasCache = false;
        }

        public List<long> SkyHasReward{ get { return m_skyHasReward;}}
        #endregion

        /// <summary>
        /// 玩家释放
        /// </summary>
        public void Dispose()
        {
            this.m_playerChapterSystem.Dispose();
            this.m_playerMailSystem.Dispose();
            this.m_playerTaskSystem.Dispose();
        }
    }
}
