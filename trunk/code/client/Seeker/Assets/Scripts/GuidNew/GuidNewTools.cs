using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SeekerGame.NewGuid
{
    public class GuidNewTools
    {
        public static Vector2[] GetEmptyPos(Vector2[] pos,Transform parent)
        {
            Vector2[] localPos = new Vector2[4];
            if (pos.Length != 4)
            {
                return null;
            }
            localPos[0] = WordToLocalPos(parent, pos[0]);
            localPos[1] = WordToLocalPos(parent, pos[1]);
            localPos[2] = WordToLocalPos(parent, pos[2]);
            localPos[3] = WordToLocalPos(parent, pos[3]);

            Vector2 center = new Vector2((localPos[0].x + localPos[3].x)/2f, (localPos[0].y + localPos[1].y) / 2f);
            Vector2 hw = new Vector2(localPos[3].x - localPos[0].x, localPos[1].y - localPos[0].y);
            Vector2[] emptyPos = new Vector2[2] { center, hw };
            return emptyPos;
        }

        public static Vector3 GetWorldCirclePos(Vector2[] pos)
        {
            Vector2 center = new Vector2((pos[0].x + pos[3].x) / 2f, (pos[0].y + pos[1].y) / 2f);
            float radius = Mathf.Max(pos[3].x - pos[0].x, pos[1].y - pos[0].y) / 2f;
            Vector3 circle = new Vector3 (center.x, center.y,radius);
            return circle;
        }

        //public static Vector2[] GetWorldRectPos(Vector2[] pos)
        //{
        //}

        public static Vector2 WordToLocalPos(Transform m_RectRoot, Vector2 world)
        {
            Vector2 position = Vector2.zero;
            position = m_RectRoot.InverseTransformPoint(world);
            return position;
        }

        public static bool isInnerRect(Vector2[] corn, Vector2 pos)
        {
            if (pos.x >= corn[0].x && pos.x <= corn[2].x
                && pos.y >= corn[0].y && pos.y <= corn[2].y)
            {
                return true;
            }
            return false;
        }

        public static bool isInnerCircle(Vector3 circle, Vector2 pos)
        {
            float dis = Vector2.Distance(pos, Vector3.right * circle.x + Vector3.up * circle.y);
            if (dis <= circle.z)
            {
                return true;
            }
            return false;
        }

        public static bool InnerEmpty(List<Vector3> circleEmpty,List<Vector2[]> rectEmpty,Vector2 pos)
        {
            for (int i = 0; i < circleEmpty.Count; i++)
            {
                if (isInnerCircle(circleEmpty[i],pos))
                {
                    return true;
                }
            }
            for (int i = 0; i < rectEmpty.Count; i++)
            {
                if (isInnerRect(rectEmpty[i],pos))
                {
                    return true;
                }
            }
            return false;
        }

        public static void PassEvent<T>(string btnName, PointerEventData data, ExecuteEvents.EventFunction<T> function, bool isClick,int clickType = 0) where T : IEventSystemHandler
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);

            GameObject current = data.pointerCurrentRaycast.gameObject;
            for (int i = 0; i < results.Count; i++)
            {
                GameObject resultGameObject = results[i].gameObject;
                if (current != resultGameObject)
                {
                    if (isClick)
                    {
                        if (resultGameObject.name.Equals(btnName))
                        {
                            GameObject resultObj = results[i].gameObject;
                            if (clickType == 0)
                            {
                                if (resultObj.GetComponent<UnityEngine.UI.Toggle>() != null)
                                {
                                    ExecuteEvents.Execute(resultObj, data, function);
                                    return;
                                }
                                GOGUI.EventTriggerListener eventListener = GOGUI.EventTriggerListener.Get(resultObj);
                                if (eventListener == null)
                                {
                                    eventListener = GOGUI.EventTriggerListener.Get(resultObj.transform.parent);
                                }
                                eventListener.OnClick();
                                return;
                            }
                            else if (clickType == 1)
                            {
                                ExecuteEvents.Execute(resultObj, data, function);
                                GOGUI.EventTriggerListener eventListener = GOGUI.EventTriggerListener.Get(resultObj);
                                if (eventListener == null)
                                {
                                    eventListener = GOGUI.EventTriggerListener.Get(resultObj.transform.parent);
                                }
                                eventListener.OnLongPressBegin();
                            }
                            else if (clickType == 2)
                            {
                                GOGUI.EventTriggerListener eventListener = GOGUI.EventTriggerListener.Get(resultObj);
                                if (eventListener == null)
                                {
                                    eventListener = GOGUI.EventTriggerListener.Get(resultObj.transform.parent);
                                }
                                eventListener.OnMyLongPressEnd();
                                //ExecuteEvents.Execute(resultObj, data, ExecuteEvents.pointerUpHandler);
                            }

                        }
                    }
                    
                }
            }
        }
    }
}
