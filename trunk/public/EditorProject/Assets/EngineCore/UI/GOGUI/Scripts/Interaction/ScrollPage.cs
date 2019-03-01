using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

class ScrollPage : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    //页面：0，1，2  索引从0开始
    //每夜占的比列：0/2=0  1/2=0.5  2/2=1
    private List<float> pageArray;

    //public List<float> PageArray
    //{
    //    // get { return pageArray; }
    //    set { pageArray = value; }
    //}
    private List<Toggle> toggleArray;

    public List<Toggle> ToggleArray
    {
        //get { return toggleArray; }
        set { toggleArray = value; }
    }
    private int pageCount;//多少页

    private int pageIndex = 0;//:当前页码
    private ScrollRect rect;
    //滑动速度
    public float smooting=4;

    public float Smooting
    {
        get { return smooting; }
        set { smooting = value; }
    }

    private int pageSize;

    /// <summary>
    /// 一页显示多少条
    /// </summary>
    public int setPageSize
    {
        get { return pageSize; }
        set { pageSize = value; }
    }

    private int sumRecord;

    /// <summary>
    /// 总记录数
    /// </summary>
    public int setSumRecord
    {
        get { return sumRecord; }
        set { sumRecord = value; }
    }

    private List<GameObject> pageList = new List<GameObject>();

    //滑动的起始坐标
    private float targethorizontal = 0;

    //是否拖拽结束
    private bool isDrag = false;


    public GameObject page;

    private int updateInfo;

    public int UpdateInfo
    {
        //get { return updateInof; }
        set { Init(); }
    }

    //private static UIScrollItem instance;
    //public static UIScrollItem Instance
    //{
    //    get { return instance; }
    //    //set { ObjectPools.instance = value; }
    //}

    void Init()
    {
        UnInit();
        //pageSize = 8;
        //setSumRecord = 54;
        //smooting = 4;
        rect = transform.GetComponent<ScrollRect>();
        pageArray = new List<float>();
        toggleArray = new List<Toggle>();
        targethorizontal = 0;
        rect.horizontalNormalizedPosition = 0;
        RectTransform rect_tran=rect.content.GetComponent<RectTransform>();
        GridLayoutGroup grid = rect.content.GetComponent<GridLayoutGroup>();
        // 1ページの幅を取得.
        float pageWidth = grid.cellSize.x + grid.spacing.x;

        //instance = this; //单列

        pageCount = (int)(rect_tran.sizeDelta.x / pageWidth); //sumRecord为总记录数

        //Debug.LogError(pageCount);

        //求出每页的临界角，页索引从0开始
        int num = pageCount - 1;
        if (num == 0) num = 1;
        for (int i = 0; i < pageCount; i++)
        {
            //Debug.LogError(i + "/" + num + "==" + (float)i / (float)num);
            pageArray.Add((float)i / (float)num);
        }

        if (page != null)
        {

            ToggleGroup group = page.transform.parent.GetComponent<ToggleGroup>();
            for (int i = 0; i < pageCount; i++)
            {
                
                ////获取页码预设体
                GameObject pageItem = GameObject.Instantiate(page);
                pageList.Add(pageItem);
                pageItem.transform.SetParent(page.transform.parent);
                pageItem.transform.localScale = new Vector3(1, 1, 1);
                pageItem.transform.localRotation = new Quaternion();
                pageItem.transform.localPosition = Vector3.one;
                pageItem.SetActive(true);
                Toggle toogle = pageItem.GetComponent<Toggle>();
                toogle.group = group;
                toggleArray.Add(toogle);
            }


        }

        //Debug.LogError(pageSize);

        //rect.horizontalNormalizedPosition = 0;

        //不管怎样默认都是显示最后一页，同理，页码页码也是。已在生成页码时处理
        //初始化不需要显示滑动效果。直接赋值即可
        //targethorizontal = pageArray[pageArray.Count - 1];
        //rect.horizontalNormalizedPosition = pageArray[pageArray.Count - 1];

        foreach (var item in toggleArray)
        {
            item.isOn = false;
        }

        if (toggleArray.Count > 0)
        {
            //默认第一个选中
            toggleArray[0].isOn = true;

        }
    }

    void Awake()
    {
    }
    void Start()
    {
        Init();
        rect.horizontalNormalizedPosition = 0;
    }

    void Update()
    {
        //如果不判断。当在拖拽的时候要也会执行插值，所以会出现闪烁的效果
        ////这里只要在拖动结束的时候。在进行插值
        if (!isDrag)
        {
            rect.horizontalNormalizedPosition = Mathf.Lerp(rect.horizontalNormalizedPosition, targethorizontal, Time.deltaTime * smooting);

            //rect.horizontalNormalizedPosition = targethorizontal;
        }
    }
    /// <summary>
    /// 拖动开始
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
    }

    /// <summary>
    /// 拖拽结束
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;

        float posX = rect.horizontalNormalizedPosition;
        int index = 0;
        //假设离第一位最近
        float offset = Mathf.Abs(pageArray[index] - posX);
        for (int i = 1; i < pageArray.Count; i++)
        {
            float temp = Mathf.Abs(pageArray[i] - posX);
            if (temp < offset)
            {
                index = i;
                //保存当前的偏移量
                offset = temp;
            }
        }
        try
        {
            targethorizontal = pageArray[index];
            //说明页码大于1 toggle有值
            if (pageCount > 1 && toggleArray.Count > 0)
                toggleArray[index].isOn = true;
        }
        catch (Exception)
        {
            return;
        }
    }

    void UnInit()
    {
        foreach (var item in pageList)
        {
            GameObject.Destroy(item);
        }
    }
}