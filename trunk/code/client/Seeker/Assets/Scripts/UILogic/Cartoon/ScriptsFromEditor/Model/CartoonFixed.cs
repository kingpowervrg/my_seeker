using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

public class CartoonFixed : MonoBehaviour
{



    public const int C_DEFAULT_ENTER_DOOR = 4;
    public const int C_WIN_DOOR = 9;
    public const int C_FAIL_DOOR = 0;


    public enum ENUM_CARTOON_PATH_TYPE
    {
        E_BEGIN,
        E_TRIP,
        E_END,
    }

    public class VideoItem
    {
        public int m_entrance;
        public int m_exit;
        public VideoClip m_video;
    }

    //[HideInInspector]
    public int m_item_id; //在CartoonTemplate m_cartoon_items数组中的位置
    public int m_width_unit = 1;
    public int m_height_unit = 1;


    [HideInInspector]
    public RectTransform m_rect;

    public List<VideoClip> m_videos;


    public ENUM_CARTOON_ITEM_TYPE m_item_type;




    public List<int> DoorIndexCopy
    {
        get { List<int> ret = new List<int>(); ret.AddRange(m_doors_idx); return ret; }

    }
    protected List<int> m_doors_idx;


    //protected Dictionary<int, int> m_path_2_videoName_dict = new Dictionary<int, int>()
    //{
    //    { 0, System.Convert.ToInt32( "00000000") },
    //    { 1, System.Convert.ToInt32("10000000") }, { 2, System.Convert.ToInt32("01000000") }, { 3, System.Convert.ToInt32("00100000") },
    //    { 4, System.Convert.ToInt32("00010000") }, { 5, System.Convert.ToInt32("00001000") }, { 6, System.Convert.ToInt32("00000100") },
    //    { 7, System.Convert.ToInt32("00000010") }, { 8, System.Convert.ToInt32("00000001") },
    //    { 9, System.Convert.ToInt32("11111111") }
    //};
    protected Dictionary<int, int> m_pre_exit_2_cur_entrance_dict = new Dictionary<int, int>()
    { { 1, 6 }, { 2, 5 }, { 3, 8 }, { 4, 7 },
        { 5, 2 }, { 6, 1 }, { 7, 4 }, { 8, 3 } };

    protected Dictionary<int, int> m_entrance_2_exit_dict = new Dictionary<int, int>()
    { { 6, 1 }, { 5, 2 }, { 8, 3 }, { 7, 4 },
        { 2, 5 }, { 1, 6 }, { 4, 7 }, { 3, 8 } };

    private Dictionary<int, VideoItem> m_entrance_2_video_dict;


    private Dictionary<int, int> m_video_entrance_2_exit;
    public System.Collections.Generic.Dictionary<int, int> Video_entrance_2_exit
    {
        get { return m_video_entrance_2_exit; }
    }

    public bool GetVideo(int pre_exit_, out VideoClip clip_, out int cur_exit_, bool is_first = false)
    {
        clip_ = null;
        cur_exit_ = 0;

        int cur_entrance;
        if (!is_first)
        {
            cur_entrance = this.GetEnterDoor(pre_exit_);
        }
        else
        {
            cur_entrance = this.GetEnterDoor(C_DEFAULT_ENTER_DOOR);
        }

        if (!m_entrance_2_video_dict.ContainsKey(cur_entrance))
            return false;

        clip_ = m_entrance_2_video_dict[cur_entrance].m_video;
        cur_exit_ = GetExitDoor(m_entrance_2_video_dict[cur_entrance].m_exit);

        return true;
    }

    private int GetExitDoor(int video_exit_)
    {
        if (C_FAIL_DOOR == video_exit_ || C_WIN_DOOR == video_exit_)
            return video_exit_;

        int exit_idx = m_doors_idx.IndexOf(video_exit_);
        return exit_idx + 1;
    }

    public VideoClip GetFirstClip()
    {
        return this.m_videos.First<VideoClip>();
    }

    private int GetEnterDoor(int pre_exit_)
    {
        int cur_entrance = m_pre_exit_2_cur_entrance_dict[pre_exit_];

        int cur_door_index = this.GetDoorIndexByEntrance(cur_entrance);

        return m_doors_idx[cur_door_index];
    }


    public virtual void Init()
    {


        m_entrance_2_video_dict = new Dictionary<int, VideoItem>();
        m_doors_idx = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };

        foreach (VideoClip clip in m_videos)
        {
            VideoItem item = new VideoItem();
            int en, ex;
            if (!this.GetEntranceAndExit(clip.name, out en, out ex))
                continue;

            item.m_entrance = en;
            item.m_exit = ex;
            item.m_video = clip;

            if (!m_entrance_2_video_dict.ContainsKey(en))
                m_entrance_2_video_dict.Add(en, item);
            else
            {
                Debug.LogError(string.Format("{0}号卡通，含有多个相同的入口{1}视频", this.m_item_id, en));
            }
        }


        this.InitAllVideoIdxDict();
    }

    void Awake()
    {
        this.m_rect = this.GetComponent<RectTransform>();
    }
    // Use this for initialization
    void Start()
    {
        this.Init();
    }

    protected bool GetEntranceAndExit(string videoName_, out int entrance_, out int exit_)
    {
        entrance_ = 0;
        exit_ = 0;
        if (videoName_.EndsWith(".mp4"))
        {
            Debug.LogError("视频文件不是以mp4结尾");
            return false;
        }

        string[] split_str = videoName_.Split('.');

        string num_str = split_str[0];

        string[] split_num = num_str.Split('-');

        if (4 != split_num.Length)
        {
            Debug.LogError("视频文件名字 不是以 页-块-出发-到达 的格式命名");
            return false;
        }

        if (string.IsNullOrEmpty(split_num[2]) || string.IsNullOrEmpty(split_num[3]))
        {
            Debug.LogError("视频文件 出发到达 数字 命名错误");
            return false;
        }

        string entrance_str = split_num[2];
        string exit_str = split_num[3];

        entrance_ = int.Parse(entrance_str);
        exit_ = int.Parse(exit_str);

        return true;
    }

    /// <summary>
    /// 
    ///    1  2
    ///  8      3
    ///  7      4
    ///    6  5
    /// 
    /// </summary>
    /// <param name="entrance_"></param>
    /// <returns></returns>
    private int GetDoorIndexByEntrance(int entrance_)
    {
        return entrance_ - 1;
    }

    //public virtual List<int> SetAnchorPosAndReturnOccupyIndex(Vector2 anchor_pos_, int index_, int w_, int h_)
    //{
    //    if( 0 <= index_ && index_ < w_ * h_)
    //    {
    //        this.m_rect.anchoredPosition = anchor_pos_;
    //        return new List<int> { index_ };
    //    }

    //    return null;

    //}


    private void InitAllVideoIdxDict()
    {
        m_video_entrance_2_exit = new Dictionary<int, int>();

        foreach (KeyValuePair<int, VideoItem> item in m_entrance_2_video_dict)
        {
            Video_entrance_2_exit.Add(item.Value.m_entrance, item.Value.m_exit);
        }

    }
}
