using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;



public class RadarImage : UnityEngine.UI.RawImage
{
    public const int MAX_PROP_COUNT = 4;
    private const float MAX_PROP_VALUE = 1000.0f;

    [SerializeField]
    public float prop0 = 1000;
    public float prop1 = 1000;
    public float prop2 = 1000;
    public float prop3 = 1000;

    public float percent0;
    public float percent1;
    public float percent2;
    public float percent3;


    public Vector2[] _maxPropVector = new Vector2[MAX_PROP_COUNT];
    private Vector2[] _propList = new Vector2[MAX_PROP_COUNT];
    private Vector2[] _currentList = new Vector2[MAX_PROP_COUNT];

    private bool _isSetValue = false;



    protected override void Awake()
    {
        base.Awake();

        _maxPropVector = new Vector2[MAX_PROP_COUNT];
        _propList = new Vector2[MAX_PROP_COUNT];
        _currentList = new Vector2[MAX_PROP_COUNT];

        //_maxPropVector[0] = new Vector2(0, 100);
        //_maxPropVector[1] = new Vector2(100, 0);
        //_maxPropVector[2] = new Vector2(0, -100);
        //_maxPropVector[3] = new Vector2(-100, 0);

        _maxPropVector[0] = new Vector2(1, 1);
        _maxPropVector[1] = new Vector2(1, 1);
        _maxPropVector[2] = new Vector2(1, 1);
        _maxPropVector[3] = new Vector2(1, 1);

        _isSetValue = false;
    }

    protected override void Start()
    {
        base.Start();
        this.SetPropList(new List<float> { prop0, prop1, prop2, prop3 });
    }

    // 设置五个属性值  
    public void SetPropList(List<float> list)
    {
        if (list.Count < MAX_PROP_COUNT)
        {
            return;
        }

        var r = GetPixelAdjustedRect();
        var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);
        // v.x,v.y为左下角的点   v.z,v.w为右上角的点  

        _propList[0] = new Vector3(v.x * _maxPropVector[0].x, v.y * _maxPropVector[0].y) * list[0] / MAX_PROP_VALUE; // 1  
        _propList[1] = new Vector3(v.z * _maxPropVector[1].x, v.y * _maxPropVector[1].y) * list[1] / MAX_PROP_VALUE; // 2  
        _propList[2] = new Vector3(v.z * _maxPropVector[2].x, v.w * _maxPropVector[2].y) * list[2] / MAX_PROP_VALUE; // 3  
        _propList[3] = new Vector3(v.x * _maxPropVector[3].x, v.w * _maxPropVector[3].y) * list[3] / MAX_PROP_VALUE; // 4  


        percent0 = list[0] / MAX_PROP_VALUE;
        percent1 = list[1] / MAX_PROP_VALUE;
        percent2 = list[2] / MAX_PROP_VALUE;
        percent3 = list[3] / MAX_PROP_VALUE;


        for (int i = 0; i < MAX_PROP_COUNT; ++i)
        {
            _currentList[i] = _propList[i];
        }

        _isSetValue = true;

        SetVerticesDirty();
    }



    void Update()
    {
        if (!_isSetValue)
        {
            return;
        }

        this.SetPropList(new List<float> { prop0, prop1, prop2, prop3 });

        //SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // 尚未赋值，不用绘制  
        if (!_isSetValue)
        {
            return;
        }

        Color32 color32 = color;
        vh.Clear();
        //vh.AddVert(new Vector3(0, 0), color32, new Vector2(0f, 0f)); // 0  
        vh.AddVert(_currentList[0], color32, new Vector2(0f, 0f)); // 1  0
        vh.AddVert(_currentList[1], color32, new Vector2(1f, 0f)); // 2  1
        vh.AddVert(_currentList[2], color32, new Vector2(1f, 1f)); // 3  2
        vh.AddVert(_currentList[3], color32, new Vector2(0f, 1f)); // 4  3
                                                                   //vh.AddVert(_currentList[4], color32, new Vector2(1f, 0f)); // 5  

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(0, 2, 3);
        //vh.AddTriangle(0, 3, 4);
        //vh.AddTriangle(0, 4, 5);
        //vh.AddTriangle(0, 5, 1);
    }
}

