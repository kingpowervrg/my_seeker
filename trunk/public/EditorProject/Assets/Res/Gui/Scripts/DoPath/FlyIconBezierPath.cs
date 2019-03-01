using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using DG.Tweening;


public class FlyIconBezierPath : MonoBehaviour
{
    public Color drawColor = Color.white;

    [FormerlySerializedAs("anchor_from")]
    [SerializeField]
    public Transform anchor_from;
    [FormerlySerializedAs("anchor_middle1")]
    [SerializeField]
    public Transform anchor_middle1;
    [FormerlySerializedAs("anchor_middle2")]
    [SerializeField]
    public Transform anchor_middle2;
    [FormerlySerializedAs("anchor_to")]
    [SerializeField]
    public Transform anchor_to;

    //public Transform real_from;
    public Transform real_to;

    public int resolution;

    Transform m_move_trf;

    /// <summary>
    /// middle距离出发点的X距离
    /// </summary>
    float anchor_dis_x_from2to;
    float anchor_dis_x_from2middle1;
    float anchor_dis_x_from2middle2;

    /// <summary>
    /// middle距离出发点的距离，占from到to总距离的百分比
    /// </summary>
    float anchor_dis_x_middle1_f;
    float anchor_dis_x_middle2_f;

    Vector3 real_from_pos;
    Vector3 real_middle1_pos;
    Vector3 real_middle2_pos;
    Vector3 real_to_pos;

    private bool m_dirty;
    public bool Dirty
    {
        get { return m_dirty; }
        set { m_dirty = value; }
    }
#if UNITY_EDITOR
    // Use this for initialization
    void Start()
    {

        m_move_trf = this.transform.Find("Move").transform;
        InitLerp();

    }
#endif

    public void RefreshMove(Vector3 from_, Vector3 to_)
    {
        m_move_trf.position = from_;
        real_from_pos = from_;
        real_to_pos = to_;

        Lerping();
    }

#if UNITY_EDITOR
    public void RefreshMoveInEditor()
    {
        real_from_pos = m_move_trf.position;
        real_to_pos = real_to.position;

        Lerping();
    }


    public void ResetMoveInEditor()
    {
        m_move_trf.localPosition = Vector3.zero;
    }

#endif

    void InitLerp()
    {
        real_from_pos = anchor_from.position;
        real_to_pos = anchor_to.position;

        anchor_dis_x_from2to = Mathf.Abs(anchor_to.transform.position.x - anchor_from.transform.position.x);
        anchor_dis_x_from2middle1 = Mathf.Abs(anchor_middle1.transform.position.x - anchor_from.transform.position.x);
        anchor_dis_x_from2middle2 = Mathf.Abs(anchor_middle2.transform.position.x - anchor_from.transform.position.x);

        anchor_dis_x_middle1_f = anchor_dis_x_from2middle1 / anchor_dis_x_from2to;
        anchor_dis_x_middle2_f = anchor_dis_x_from2middle2 / anchor_dis_x_from2to;
    }


    void Lerping()
    {
        LerpMiddlePoint();
        LerpPath();
    }
    void LerpMiddlePoint()
    {
        float real_dis_x_from2to = Mathf.Abs(real_from_pos.x - real_to_pos.x);
        float dis_compare_new2old = real_dis_x_from2to / anchor_dis_x_from2to;
        float delta_x1, delta_x2;

        if (real_from_pos.x < real_to_pos.x)
        { delta_x1 = anchor_dis_x_from2middle1; delta_x2 = anchor_dis_x_from2middle2; }
        else
        { delta_x1 = -anchor_dis_x_from2middle1; delta_x2 = -anchor_dis_x_from2middle2; }

        float middle1_x = real_from_pos.x + delta_x1 * dis_compare_new2old;
        float middle1_y = real_from_pos.y + (anchor_middle1.transform.position.y - anchor_from.position.y);


        float middle2_x = real_from_pos.x + delta_x2 * dis_compare_new2old;
        float middle2_y = real_from_pos.y + (anchor_middle2.transform.position.y - anchor_from.position.y);

        real_middle1_pos = new Vector3(middle1_x, middle1_y, 0.0f);
        real_middle2_pos = new Vector3(middle2_x, middle2_y, 0.0f);
    }

    void LerpPath()
    {
        var path = GetMovePath();
        m_move_trf.DOPath(path.ToArray(), 1.0f, pathMode: PathMode.TopDown2D, resolution: resolution, gizmoColor: Color.yellow);
    }



    List<Vector3> GetMovePath()
    {
        List<Vector3> ret = new List<Vector3>();

        for (int i = 0; i < resolution + 1; i++)
        {
            Vector3 currentPoint = CalculateBezier(real_from_pos, real_middle1_pos, real_middle2_pos, real_to_pos, (float)i / (float)resolution);
            ret.Add(currentPoint);
        }

        return ret;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = drawColor;

        Vector3 lastPoint = anchor_from.position;
        Vector3 currentPoint = Vector3.zero;

        for (int i = 0; i < resolution + 1; i++)
        {
            currentPoint = CalculateBezier(anchor_from.position, anchor_middle1.position, anchor_middle2.position, anchor_to.position, (float)i / (float)resolution);
            Gizmos.DrawLine(lastPoint, currentPoint);
            lastPoint = currentPoint;
        }
    }



    public static Vector3 CalculateBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return (Mathf.Pow(1 - t, 3) * p0) + (3 * Mathf.Pow(1 - t, 2) * t * p1) + (3 * (1 - t) * t * t * p2) + (t * t * t * p3);
    }
}
