using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum UGUISide
{
    Bottom,
    BottomLeft,
    BottomRight,
    Left,
    Right,
    Top,
    TopLeft,
    TopRight,
}

public class MathEx
{
    /// <summary>
    /// src用描点对齐tar的描点，tar确定src的位置
    /// </summary>
    /// <param name="src"></param>
    /// <param name="srcSide"></param>
    /// <param name="tar"></param>
    /// <param name="tarSide"></param>
    /// <param name="area"></param>
    public static void AnchorTo(RectTransform src, UGUISide srcSide, RectTransform tar, UGUISide tarSide, Transform canvas)
    {
        if (null == tar || tar == src)
        {
            return;
        }
        Bounds srcBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(canvas, src);//计算src的包围盒
        Bounds tarBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(canvas, tar);//计算tar的包围盒

        Vector2 srcOffset = GetBoundsOffset(srcBounds, srcSide);//计算描点的偏移量
        Vector2 tarOffset = GetBoundsOffset(tarBounds, tarSide);//计算描点的偏移量
        Vector2 tarCenter = tarBounds.center;

        //计算src相对于tar的位置
        src.anchoredPosition = (tarCenter - tarOffset + srcOffset);
        D.log("src.anchoredPosition="+ src.anchoredPosition);
    }

    public static bool SetUIArea(RectTransform target, Canvas canvas)
    {
        CanvasScaler canvaScaler = canvas.GetComponent<CanvasScaler>();
        Rect rect = new Rect(-Screen.width / 2, -Screen.height / 2, Screen.width, Screen.height);
        float scale = canvaScaler.matchWidthOrHeight == 1 ? canvaScaler.referenceResolution.y / (float)Screen.height : canvaScaler.referenceResolution.x / (float)Screen.width;
        rect = new Rect(rect.x * scale, rect.y * scale, rect.width * scale, rect.height * scale);
        return SetUIArea(target, rect, canvas.transform);
    }

    /// <summary>
    /// 注意rect中心点在中间
    /// </summary>
    /// <param name="target"></param>
    /// <param name="area"></param>
    /// <returns></returns>
    private static bool SetUIArea(RectTransform target, Rect area, Transform canvas)
    {
        Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(canvas, target);

        if (null == area)
        {
            return false;
        }

        Vector2 delta = default(Vector2);
        if (bounds.center.x - bounds.extents.x < area.x)//target超出area的左边框
        {
            delta.x += Mathf.Abs(bounds.center.x - bounds.extents.x - area.x);
        }
        else if (bounds.center.x + bounds.extents.x > area.width / 2)//target超出area的右边框
        {
            delta.x -= Mathf.Abs(bounds.center.x + bounds.extents.x - area.width / 2);
        }

        if (bounds.center.y - bounds.extents.y < area.y)//target超出area上边框
        {
            delta.y += Mathf.Abs(bounds.center.y - bounds.extents.y - area.y);
        }
        else if (bounds.center.y + bounds.extents.y > area.height / 2)//target超出area的下边框
        {
            delta.y -= Mathf.Abs(bounds.center.y + bounds.extents.y - area.height / 2);
        }

        //加上偏移位置算出在屏幕内的坐标
        target.anchoredPosition += delta;

        return delta != default(Vector2);
    }

    public static Vector2 GetBoundsOffset(Bounds bounds, UGUISide side)
    {
        Vector2 offset = Vector2.zero;

        switch (side)
        {
            case UGUISide.Bottom:
                offset.y = bounds.extents.y;
                break;
            case UGUISide.BottomLeft:
                offset.x = bounds.extents.x;
                offset.y = bounds.extents.y;
                break;
            case UGUISide.BottomRight:
                offset.x = -bounds.extents.x;
                offset.y = bounds.extents.y;
                break;
            case UGUISide.Left:
                offset.x = bounds.extents.x;
                break;
            case UGUISide.Right:
                offset.x = -bounds.extents.x;
                break;
            case UGUISide.Top:
                offset.y = -bounds.extents.y;
                break;
            case UGUISide.TopLeft:
                offset.x = bounds.extents.x;
                offset.y = -bounds.extents.y;
                break;
            case UGUISide.TopRight:
                offset.x = -bounds.extents.x;
                offset.y = -bounds.extents.y;
                break;
        }

        return offset;
    }
}