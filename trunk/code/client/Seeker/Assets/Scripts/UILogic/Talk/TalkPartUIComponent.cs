using EngineCore;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace SeekerGame
{

    public class TalkPartUIComponent : GameUIComponent
    {
        private GameSpine m_icon_tex;
        private GameTexture m_icon_texture;
        private GameUIComponent m_chooseRoot_obj; //带选择对话框根节点
        private GameLabel m_chooseContent_lab;
        private GameObject m_textPanel_obj;
        private GameObject m_imgPanel_obj;

        private GameToggleButton[] m_text_toggle;
        private GameToggleButton[] m_img_toggle;
        private GameImage[] m_ask_img;
        private GameLabel[] m_ask_lab;

        private GameButton m_textRoot_obj; //文字对话框根节点
        private GameLabel m_noContent_lab;
        private TextFader m_contentFader = null;

        private ConfChatItem m_partdata;

        private int MaxChooseNum = 4;

        private long m_chooseID = -1;

        private int m_curType = -1;
        private float fontSize = 0f;
        private float maxLabelWidth = 740f;
        protected override void OnInit()
        {
            base.OnInit();
            InitController();
            InitListener();

        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            GameEvents.UIEvents.UI_Talk_Event.OnTalkNextPart += OnTalkNextPart;
            this.maxLabelWidth = m_noContent_lab.Widget.sizeDelta.x;
        }

        public void SetIcon(GameSpine icon, GameTexture tex)
        {
            m_icon_tex = icon;
            this.m_icon_texture = tex;
        }

        public override void OnHide()
        {
            base.OnHide();
            this.m_icon_tex.Visible = false;
            this.m_icon_texture.Visible = false;
            this.m_isSpine = false;
            this.isTalk = false;
            GameEvents.UIEvents.UI_Talk_Event.OnTalkNextPart -= OnTalkNextPart;
        }

        private void InitController()
        {
            //m_icon_tex = Make<GameTexture>("icon");
            m_chooseRoot_obj = Make<GameUIComponent>("Image");
            m_chooseContent_lab = m_chooseRoot_obj.Make<GameLabel>("content");
            
            m_textPanel_obj = m_chooseRoot_obj.gameObject.transform.Find("Panel_text").gameObject;
            m_imgPanel_obj = m_chooseRoot_obj.gameObject.transform.Find("Panel_image").gameObject;

            m_text_toggle = new GameToggleButton[MaxChooseNum];
            m_img_toggle = new GameToggleButton[MaxChooseNum];
            m_ask_img = new GameImage[MaxChooseNum];
            m_ask_lab = new GameLabel[MaxChooseNum];
            for (int i = 0; i < MaxChooseNum; i++)
            {
                m_text_toggle[i] = m_chooseRoot_obj.Make<GameToggleButton>(string.Format("Panel_text:Toggle_{0}", i));
                m_img_toggle[i] = m_chooseRoot_obj.Make<GameToggleButton>(string.Format("Panel_image:Toggle_{0}", i));
                m_ask_img[i] = m_img_toggle[i].Make<GameImage>("Image");
                m_img_toggle[i].Visible = false;
                m_ask_lab[i] = m_text_toggle[i].Make<GameLabel>("Label");
                m_text_toggle[i].Visible = false;
            }

            m_textRoot_obj = Make<GameButton>("Text");
            m_noContent_lab = m_textRoot_obj.Make<GameLabel>("content");
            this.m_contentFader = m_noContent_lab.GetComponent<TextFader>();
            fontSize = m_noContent_lab.Label.fontSize;
            //m_next_btn = Make<GameButton>("Button");
        }

        private void InitListener()
        {
            for (int i = 0; i < MaxChooseNum; i++)
            {
                setChooseListener(i);
            }
            //m_next_btn.AddClickCallBack(btnNextPart);
            m_chooseRoot_obj.AddClickCallBack(btnNextPart);
            m_textRoot_obj.AddClickCallBack(btnNextPart);
        }

        private int GetLineForText(string str)
        {
            int strLen = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (isChinese(str[i]))
                {
                    strLen += 2;
                }
                else
                {
                    strLen++;
                }
            }
            return Mathf.CeilToInt(strLen * fontSize / 2f / this.maxLabelWidth);
            //byte[] strByte = System.Text.Encoding.Unicode.GetBytes(str);
            //Debug.Log(strByte.Length);
            //return Mathf.CeilToInt(((strByte.Length / 3) * 30 / maxText));
        }

        private bool isChinese(char c)
        {
            return c >= 0x4e00 && c <= 0x9fbb || c == '，' || c == '。';
        }

        private bool m_isSpine = false;
        public void setData(ConfChatItem partdata)
        {
            if (partdata == null)
            {
                return;
            }
            m_partdata = partdata;
            if (m_partdata.icon.Contains("${player_icon}"))
            {
                //if (!GlobalInfo.MY_PLAYER_INFO.PlayerIcon.Contains("http") && !GlobalInfo.MY_PLAYER_INFO.PlayerIcon.Contains("https"))
                //    m_icon_tex.TextureName = CommonData.GetBigPortrait(GlobalInfo.MY_PLAYER_INFO.PlayerIcon);
                //else
                //    m_icon_tex.TextureName = "image_player_size4_1.png";
                m_icon_tex.SpineName = CommonData.GetSpineHead(GlobalInfo.MY_PLAYER_INFO.PlayerIcon);
                m_isSpine = true;
                //this.m_icon_tex.Visible = true;
                //m_icon_texture.Visible = false;
            }
            else
            {
                if (m_partdata.icon.Contains(".png"))
                {
                    m_icon_texture.TextureName = m_partdata.icon;
                    m_isSpine = false;
                    //m_icon_texture.Visible = true;
                    //this.m_icon_tex.Visible = false;
                }
                else
                {
                    m_icon_tex.SpineName = m_partdata.icon.Trim();
                    m_isSpine = true;
                    //m_icon_tex.Visible = true;
                    //this.m_icon_texture.Visible = false;
                }
            }
            //m_icon_tex.PlayAnimation(string.Empty,true);
            setPartType(m_partdata.jumptype);
            m_curType = m_partdata.jumptype;
            if (m_partdata.jumptype == 0)
            {
                //文字
                string temp = LocalizeModule.Instance.GetString(m_partdata.content);
                TalkContent = temp;
                //m_noContent_lab.Text = temp;
                int line = GetLineForText(temp);
                float fonts_height = (line + 2) * fontSize;
                m_textRoot_obj.Widget.sizeDelta = new Vector2(m_textRoot_obj.Widget.sizeDelta.x, fonts_height);
            }
            else
            {
                m_chooseContent_lab.Text = LocalizeModule.Instance.GetString(m_partdata.content);
                if (m_partdata.jumptype == 2)
                {
                    setTextChoose();
                }
                else if (m_partdata.jumptype == 1)
                {
                    setImgChoose();
                }
            }
        }

        private void OnTalkNextPart()
        {
            if (this.isTalk)
            {
                btnNextPart(null);
            }
        }

        private void setPartType(int type)
        {
            m_textPanel_obj.SetActive(type == 2);
            m_imgPanel_obj.SetActive(type == 1);
            m_chooseRoot_obj.SetActive(1 == type || 2 == type);
            m_textRoot_obj.SetActive(0 == type);
        }

        private void setTextChoose()
        {
            if (m_partdata.jumpcontens == null || m_ask_lab.Length < m_partdata.jumpcontens.Length)
            {
                return;
            }
            for (int i = 0; i < m_partdata.jumpcontens.Length; i++)
            {
                m_ask_lab[i].Text = LocalizeModule.Instance.GetString(m_partdata.jumpcontens[i]);
                m_text_toggle[i].SetActive(true);
            }
        }

        private void setImgChoose()
        {
            if (m_partdata.jumpcontens == null || m_ask_img.Length < m_partdata.jumpcontens.Length)
            {
                return;
            }
            for (int i = 0; i < m_partdata.jumpcontens.Length; i++)
            {
                m_ask_img[i].Sprite = m_partdata.jumpcontens[i];
                m_img_toggle[i].SetActive(true);
            }
        }

        private void setChooseListener(int i)
        {
            m_text_toggle[i].AddChangeCallBack(delegate (bool flag)
            {
                setAnswer(flag, i);
            });

            m_img_toggle[i].AddChangeCallBack(delegate (bool flag)
            {
                setAnswer(flag, i);
            });
        }

        private void setAnswer(bool flag, int index)
        {
            if (!flag)
            {
                return;
            }
            if (m_partdata.jumpids != null && m_partdata.jumpids.Length > index)
            {
                if (m_curType <= 0)
                {
                    return;
                }
                resetToggle();
                GameEvents.UIEvents.UI_Talk_Event.OnTalkChoose.SafeInvoke(m_partdata.jumpids[index]);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            m_chooseRoot_obj.RemoveClickCallBack(btnNextPart);
            m_textRoot_obj.RemoveClickCallBack(btnNextPart);
        }

        private void btnNextPart(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.task_next.ToString());
            if (m_curType != 0 || !ContentFaderComplete)
            {
                return;
            }
            GameEvents.UIEvents.UI_Talk_Event.OnTalkChoose.SafeInvoke(-1);
            //m_chooseID = -1;
        }

        private void resetToggle()
        {
            if (m_curType == 2)
            {
                //m_textPanel_TogGroup.SetAllTogglesOff();
                for (int i = 0; i < m_text_toggle.Length; i++)
                {
                    m_text_toggle[i].Checked = false;
                }
            }
            else if (m_curType == 1)
            {
                //m_imgPanel_TogGroup.SetAllTogglesOff();
                for (int i = 0; i < m_img_toggle.Length; i++)
                {
                    m_img_toggle[i].Checked = false;
                }
            }
        }

        private bool isTalk = false;
        public void IsTalk(bool isTalk)
        {
            if (this.isTalk == isTalk)
            {
                return;
            }
            this.isTalk = isTalk;
            //this.m_chooseRoot_obj.Visible = isTalk;
            this.m_textRoot_obj.Visible = isTalk;
            if (!isTalk)
            {
                if (m_isSpine)
                {
                    IconTweener(m_icon_tex.Spine, true);
                    this.m_icon_tex.StopAnimation();
                }
                else
                {
                    IconTweener(m_icon_texture.RawImage, true);
                }
            }
            else
            {
                if (m_isSpine)
                {
                    if (!m_icon_tex.CachedVisible)
                    {
                        m_icon_tex.Visible = true;
                    }
                    IconTweener(m_icon_tex.Spine, false);
                    this.m_icon_tex.PlayAnimation(string.Empty, true);
                }
                else
                {
                    if (!m_icon_texture.CachedVisible)
                    {
                        m_icon_texture.Visible = true;
                    }
                    IconTweener(m_icon_texture.RawImage, false);
                }
            }
        }

        private void IconTweener(Graphic tran, bool isForward = true)
        {
            if (isForward)
            {
                tran.rectTransform.localScale = Vector3.one;
                tran.color = Color.white;
            }

            Vector3 destScale = Vector3.one * 0.8f;
            Color destColor = Color.white * 0.5f;
            if (!isForward)
            {
                destScale = Vector3.one;
                destColor = Color.white;
            }
            tran.rectTransform.DOScale(destScale, 0.4f);
            tran.CrossFadeColor(destColor, 0.4f, true, false);
        }

        private void ReplayTextFader()
        {
            m_contentFader.enabled = false;
            m_contentFader.enabled = true;
        }

        private string TalkContent
        {
            set {
                m_noContent_lab.Text = value;
                ReplayTextFader();
            }
        }

        public bool ContentFaderComplete
        {
            get
            {
                bool faderComplete = m_contentFader.enabled;
                m_contentFader.enabled = false;
                return !faderComplete;
            }
        }

    }
}
