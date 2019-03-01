using EngineCore;
using GOGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SeekerGame
{
    public class DragDropMoveParentTexture : GameUIComponent
    {
        private GameImage m_texture;

        public EngineCore.GameImage Texture
        {
            get { return m_texture; }
            set { m_texture = value; }
        }
        protected override void OnInit()
        {
            base.OnInit();
            this.FindUIComponent();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            CustomDragEventTriggerListener lis = CustomDragEventTriggerListener.Get(gameObject);
            lis.onDrag += OnDrag;
            lis.onDragStart += OnBeginDrag;
            lis.onDragEnd += OnEndDrag;
        }


        public override void OnHide()
        {
            base.OnHide();
            CustomDragEventTriggerListener lis = CustomDragEventTriggerListener.Get(gameObject);
            lis.onDrag -= OnDrag;
            lis.onDragStart -= OnBeginDrag;
            lis.onDragEnd -= OnEndDrag;
        }


        private void FindUIComponent()
        {
            Texture = this.Make<GameImage>(this.gameObject);

        }


        private SafeAction<GameObject> DragAbleMoveEnd;
        private SafeAction<GameObject> DragAbleMoveBegin;
        /// <summary>
        /// <要移动的距离，可以移动的距离>
        /// </summary>
        private SafeFunc<Vector3, Vector3> DragAbleMoving;

        private RectTransform m_canvas;
        private Vector3 m_begin_global_mouse_pos;
        private Vector3 m_begin_pos;

        public void InitTexture(string tex_name_, Rect rect_)
        {
            Texture.X = rect_.x;
            Texture.Y = rect_.y;
            Texture.Widget.sizeDelta = rect_.size;
            Texture.Sprite = tex_name_;
            Texture.GetSprite().alphaHitTestMinimumThreshold = 0.5f;
        }

        public void RegisterMoveBegin(Action<GameObject> act_)
        {
            DragAbleMoveBegin = act_;
        }

        public void RegisterMoveEnd(Action<GameObject> act_)
        {
            this.DragAbleMoveEnd = act_;
        }

        public void RegisterMoving(Func<Vector3, Vector3> fun_)
        {
            this.DragAbleMoving = fun_;
        }

        float limit_f = CommonData.C_JIGSAW_LEFT_LIMIT_SCREEN_X;

        private Vector2 m_last_drag_screen_pos;

        public void OnBeginDrag(PointerEventData eventData)
        {
            Canvas canvas = LogicHandler.Canvas;

            if (canvas == null)
                return;

            m_last_drag_screen_pos = new Vector2(eventData.position.x, eventData.position.y);
            this.gameObject.transform.parent.transform.SetAsLastSibling();
            //this.gameObject.transform.parent.transform.localScale = new Vector3(1.05f, 1.05f, 1.0f);
            this.gameObject.transform.localScale = new Vector3(1.05f, 1.05f, 1.0f);
            m_canvas = canvas.transform as RectTransform;

            float limited_x = (eventData.position.x < limit_f ? limit_f : eventData.position.x);
            limited_x = (limited_x > Screen.width - limit_f ? Screen.width - limit_f : limited_x);

            float limited_y = (eventData.position.y < limit_f ? limit_f : eventData.position.y);
            limited_y = (limited_y > Screen.height - limit_f ? Screen.height - limit_f : limited_y);

            Vector2 limited_pos = new Vector2(limited_x, limited_y);

            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_canvas, limited_pos, eventData.pressEventCamera, out globalMousePos))
            {
                m_begin_global_mouse_pos = globalMousePos;
                m_begin_pos = this.gameObject.transform.parent.transform.position;

                if (null != DragAbleMoveBegin)
                {
                    DragAbleMoveBegin.SafeInvoke(this.gameObject.transform.parent.gameObject);
                }
            }


        }

        public void OnDrag(PointerEventData eventData)
        {
            SetDraggedPosition(eventData);
        }



        public void OnEndDrag(PointerEventData eventData)
        {
            this.gameObject.transform.localScale = Vector3.one;
            //this.gameObject.transform.parent.transform.localScale = Vector3.one;

            if (null != this.DragAbleMoveEnd)
                this.DragAbleMoveEnd.SafeInvoke(this.gameObject.transform.parent.gameObject);


        }



        private void SetDraggedPosition(PointerEventData eventData)
        {
            float limited_y = (eventData.position.y < limit_f ? limit_f : eventData.position.y);
            limited_y = (limited_y > Screen.height - limit_f ? Screen.height - limit_f : limited_y);

            float limited_x = (eventData.position.x < limit_f ? limit_f : eventData.position.x);
            //limited_x = (limited_x > Screen.width - CommonData.S_JIGSAW_RIGHT_LIMIT_X ? Screen.width - CommonData.S_JIGSAW_RIGHT_LIMIT_X : limited_x);
            //limited_x = (limited_x > CommonData.S_JIGSAW_RIGHT_LIMIT_WORLD_X ? CommonData.S_JIGSAW_RIGHT_LIMIT_WORLD_X : limited_x);

            Vector2 cur_drag_screen_pos = new Vector2(limited_x, limited_y);

            Vector3 cur_globalMousePos;

            Vector3 could_offset = Vector3.zero;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_canvas, cur_drag_screen_pos, eventData.pressEventCamera, out cur_globalMousePos))
            {
                Vector3 last_globalMousePos;
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_canvas, m_last_drag_screen_pos, eventData.pressEventCamera, out last_globalMousePos))
                {
                    Vector3 offset_ = cur_globalMousePos - last_globalMousePos;

                    could_offset = DragAbleMoving.SafeInvoke(offset_);

                    var rt = this.gameObject.transform.parent;
                    cur_globalMousePos = last_globalMousePos + could_offset;

                    Vector3 offset = cur_globalMousePos - m_begin_global_mouse_pos;

                    rt.position = m_begin_pos + offset;
                    rt.rotation = m_canvas.rotation;

                    cur_drag_screen_pos = RectTransformUtility.WorldToScreenPoint(FrameMgr.Instance.UICamera, cur_globalMousePos);
                    m_last_drag_screen_pos = cur_drag_screen_pos;

                }
            }



        }
    }
}
