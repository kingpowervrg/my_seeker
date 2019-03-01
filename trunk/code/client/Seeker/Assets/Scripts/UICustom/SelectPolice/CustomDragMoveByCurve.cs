using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EngineCore;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

namespace SeekerGame
{
    class CustomDragMoveByCurve : GameUIComponent
    {
        private const long C_FAST_DRAG_TIME = 1200000;
        private const float C_FAST_DRAG_DIS = 220.0f;
        private const float dragging_s = 200.0f;
        private const float dragging_d = 0.05f;

        private Tweener m_tween = null;
        private long m_drag_time;
        private float m_drag_pos_x;

        GameUIContainer m_location_item_pool;

        private int m_cur_selectd_police_idx;
        public int Cur_selectd_police_idx
        {
            get { return m_cur_selectd_police_idx; }
        }

        private Action<int> MoveStopped;



        public void RegisterStopAction(Action<int> act_)
        {
            MoveStopped = act_;
        }

        private Action Moving;

        public void RegisterMovingAction(Action act_)
        {
            Moving = act_;
        }

        protected override void OnInit()
        {
            base.OnInit();
            this.InitPrefab();
            this.InitParam();
            this.FindUIComponent();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.Refresh();
            if (m_location_item_pool.ChildCount > 0)
            {
                this.AddDragCallBack(OnDragging);
                this.AddDragStartCallBack(OnDragStart);
                this.AddDragEndCallBack(OnDragEnd);

                GameEvents.UIEvents.UI_Select_Police_Event.OnIndexChosen += OnCardClicked;
            }
        }


        public override void OnHide()
        {
            base.OnHide();

            this.RemoveDragCallBack(OnDragging);
            this.RemoveDragStartCallBack(OnDragStart);
            this.RemoveDragEndCallBack(OnDragEnd);

            GameEvents.UIEvents.UI_Select_Police_Event.OnIndexChosen -= OnCardClicked;

            m_tween = null;
        }

        private void FindUIComponent()
        {
            this.m_location_item_pool = this.Make<GameUIContainer>("Turntable/Location_Pool");

        }

        private void PreInitLogic()
        {

        }

        private void AddUILogic()
        {

        }

        private void RemoveUILogic()
        {

        }



        private void OnDragging(GameObject go, Vector2 delta, Vector2 pos)
        {
            float delta_time = 0.0f;

            float s = dragging_s;
            float d = dragging_d;
            if (delta.x < -2.0f)
            {
                if (this.m_location_itmes[this.m_location_itmes.Count - 1].transform.localPosition.x <= this.m_middle_local_pos_x)
                    return;

                float slide_factor = Mathf.Abs(delta.x) / s + d;
                //左滑动
                delta_time = Time.deltaTime * slide_factor;
            }
            else if (delta.x > 2.0f)
            {
                //右滑动
                if (this.m_location_itmes[0].transform.localPosition.x >= this.m_middle_local_pos_x)
                    return;

                float slide_factor = Mathf.Abs(delta.x) / s + d;

                delta_time = -Time.deltaTime * slide_factor;
            }
            else
            {
                return;
            }

            this.m_cur_time += delta_time;

            this.UpdateAllTransform();
        }


        private void OnDragStart(GameObject go, Vector2 delta)
        {
            m_drag_pos_x = delta.x;

            Debug.Log("OnDragStart");

            if (null != m_tween)
            {
                m_tween.Kill();
            }
            m_drag_time = System.DateTime.Now.Ticks;

            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.file_page.ToString());


            if (null != Moving)
                Moving();

            if (null != this.m_change_time_coroutine)
            {
                EngineCoreEvents.BridgeEvent.GetGameRootBehaviour().StopCoroutine(this.m_change_time_coroutine);
            }

        }



        private void OnDragEnd(GameObject go, Vector2 delta)
        {

            float delta_dis = m_drag_pos_x - delta.x;
            Debug.Log(string.Format("delta dis {0}", delta_dis));
            long delta_time = System.DateTime.Now.Ticks - m_drag_time;
            Debug.Log(string.Format("delta time {0}", delta_time));



            float min_delta_x = -1.0f;
            int min_index = 0;
            int total_card_count = this.m_location_itmes.Count;
            for (int i = 0; i < total_card_count; ++i)
            {
                float cur_delta_x = this.m_location_itmes[i].transform.localPosition.x - this.m_middle_local_pos_x;
                float cur_delta_x_abs = Mathf.Abs(cur_delta_x);

                if (min_delta_x < 0.0f || cur_delta_x_abs < min_delta_x)
                {
                    min_delta_x = cur_delta_x_abs;
                    min_index = i;
                }
            }
            float speed_move = 1.0f;
            float abs_delta_x = Mathf.Abs(delta_dis);

            long standard_time = C_FAST_DRAG_TIME;

            float time_speed = (float)standard_time / (float)delta_time;

            Debug.Log(string.Format("time factor {0}", time_speed));

            float standard_dis = C_FAST_DRAG_DIS;

            float dis_speed = abs_delta_x / standard_dis;
            Debug.Log(string.Format("dis factor {0}", dis_speed));

            //快速滑动

            speed_move = dis_speed * time_speed;

            int delta_i = (int)(speed_move);
            Debug.Log(string.Format("add select {0}", delta_i));
            if (delta_dis > 0.0f)
            {
                //手指左滑动
                min_index += delta_i;
                min_index = min_index < total_card_count ? min_index : total_card_count - 1;
            }
            else
            {
                min_index -= delta_i;
                min_index = min_index >= 0 ? min_index : 0;
            }


            Debug.Log(string.Format("aim select {0}", min_index));

            this.m_cur_selectd_police_idx = min_index;

            float min_index_item_time = this.m_cur_time - min_index * this.m_dt_between_2_card;
            float dt = this.m_middle_time - min_index_item_time;
            float aim_time = this.m_cur_time + dt;

            Debug.Log(string.Format("recover speed {0}", speed_move));

            speed_move = Mathf.Sqrt(speed_move);

            Debug.Log(string.Format("recover speed sqrt {0}", speed_move));

            this.ChangeTime(this.m_cur_time, aim_time, speed_move);

            //if (null != this.m_change_time_coroutine)
            //{
            //    EngineCoreEvents.BridgeEvent.GetGameRootBehaviour().StopCoroutine(this.m_change_time_coroutine);
            //}
            //m_change_time_dt = 0.0f;
            //this.m_change_time_coroutine = EngineCoreEvents.BridgeEvent.GetGameRootBehaviour().StartCoroutine(CoroutineChangeTime(this.m_cur_time, aim_time));
        }

        private void OnCardClicked(int index_)
        {


            if (null != m_tween)
            {
                m_tween.Kill();
            }

            if (index_ < 0)
                return;

            int min_index = index_;
            this.m_cur_selectd_police_idx = min_index;

            float min_index_item_time = this.m_cur_time - min_index * this.m_dt_between_2_card;
            float dt = this.m_middle_time - min_index_item_time;
            float aim_time = this.m_cur_time + dt;

            //Debug.Log(string.Format("recover speed {0}", speed_move));

            //speed_move = Mathf.Sqrt(speed_move);

            //Debug.Log(string.Format("recover speed sqrt {0}", speed_move));

            this.ChangeTime(this.m_cur_time, aim_time, 1.0f);// speed_move);
        }


        #region 动画曲线实现轨迹
        public RectTransform canvasRectTrans;
        public Transform m_begin_move_tf;
        //public Transform m_middle_tf;
        public Transform m_end_move_tf;
        public AnimationCurve m_H_curve;
        public AnimationCurve m_V_curve;
        public float m_min_scale = 0.3f;
        public float m_max_scale = 1.0f;
        public AnimationCurve m_scale_curve;
        public float m_dt_between_2_card;
        public float m_middle_time_factor;
        public AnimationCurve m_rotate_curve;
        public Transform m_begin_rotate_tf;
        public Transform m_end_rotate_tf;
        public int m_first_selected_idx;
        public float first_delta_pos = 0.0f;

        private float m_h_total_time = -1.0f;
        private float m_v_total_time = -1.0f;
        private float m_cur_time;
        private float m_middle_time;
        private float m_middle_local_pos_x;

        private float m_change_time_dt;

        private List<GameObject> m_location_itmes = new List<GameObject>();
        //private List<BlurGrayTexture> m_avatar_items = new List<BlurGrayTexture>();
        private List<GameUIComponent> m_avatar_items = new List<GameUIComponent>();

        private Coroutine m_change_time_coroutine = null;

        private float m_r_total_time = -1.0f;
        private float m_s_total_time = -1.0f;
        private float m_blur_total_time = -1.0f;

        private float m_dt_between_2_card_half;
        #endregion






        private RectTransform transTurnTable;
        private Transform transLocation;
        private Transform transLocationItem;
        private Transform transAvatarManager;
        private Transform transAvatarItem;

        private Dictionary<GameObject, GameObject> dictAvater = new Dictionary<GameObject, GameObject>();

        public float minScale = 0.5f;       //最小缩放系数
        public int avaterCount = 2;         //个数

        private void InitPrefab()
        {
            UI_MoveByCurve inner_comp = this.gameObject.GetComponent<UI_MoveByCurve>();

            canvasRectTrans = inner_comp.canvasRectTrans;
            m_begin_move_tf = inner_comp.m_begin_move_tf;
            m_end_move_tf = inner_comp.m_end_move_tf;
            m_H_curve = inner_comp.m_H_curve;
            m_V_curve = inner_comp.m_V_curve;
            m_scale_curve = inner_comp.m_scale_curve;
            m_dt_between_2_card = inner_comp.m_dt_between_2_card;
            m_middle_time_factor = inner_comp.m_middle_time_factor;
            m_rotate_curve = inner_comp.m_rotate_curve;
            m_begin_rotate_tf = inner_comp.m_begin_rotate_tf;
            m_end_rotate_tf = inner_comp.m_end_rotate_tf;
            m_first_selected_idx = inner_comp.first_selected_idx;
            //m_first_selected_idx = NewGuid.GuidNewNodeManager.Instance.first_select_police;
        }

        private void InitParam()
        {
            this.first_delta_pos = SeekerGame.NewGuid.GuidNewNodeManager.Instance.first_delta_pos;

            this.m_location_itmes.Clear();
            this.m_avatar_items.Clear();
            this.m_h_total_time = this.m_H_curve[this.m_H_curve.length - 1].time;
            this.m_v_total_time = this.m_V_curve[this.m_V_curve.length - 1].time;
            this.m_cur_time = 0.0f;


            this.m_middle_time = this.m_h_total_time * this.m_middle_time_factor;
            this.m_middle_local_pos_x = this.GetCurveLocalPos(this.m_middle_time).x;

            this.m_change_time_dt = 0.0f;

            this.m_r_total_time = this.m_rotate_curve[this.m_rotate_curve.length - 1].time;
            this.m_s_total_time = this.m_scale_curve[this.m_scale_curve.length - 1].time;

            this.m_dt_between_2_card_half = this.m_dt_between_2_card * 0.5f;
        }


        private void Refresh()
        {
            this.m_cur_time = this.m_middle_time + m_first_selected_idx * m_dt_between_2_card + first_delta_pos;
            m_cur_selectd_police_idx = 0;
            this.UpdateAllTransform();
        }

        public int Selected_Item_Index
        {
            get { return this.m_first_selected_idx; }
            set
            {
                if (value != this.m_first_selected_idx)
                {
                    this.m_first_selected_idx = value;
                    Refresh();
                }
            }
        }

        //public void InitCustomItems(List<BlurGrayTexture> custom_items_)
        //{

        //    this.m_location_item_pool.EnsureSize<GameUIComponent>(custom_items_.Count);

        //    for (int i = 0; i < custom_items_.Count; ++i)
        //    {
        //        //创建location
        //        var locationItem = this.m_location_item_pool.GetChild<GameUIComponent>(i);
        //        locationItem.SetActive(true);
        //        locationItem.SetScale(Vector3.one);
        //        locationItem.gameObject.transform.rotation = Quaternion.identity;
        //        locationItem.gameObject.transform.localPosition = this.m_begin_move_tf.localPosition;

        //        this.m_location_itmes.Add(locationItem.gameObject);

        //        //创建avater
        //        var avatarItem = custom_items_[i];
        //        avatarItem.SetActive(true);
        //        avatarItem.gameObject.transform.localPosition = Vector3.one;
        //        avatarItem.gameObject.transform.localScale = Vector3.one;

        //        //设置属性
        //        //var tex = avatarItem.transform.Find("Text").GetComponent<Text>();
        //        //tex.text = i.ToString();

        //        this.m_avatar_items.Add(avatarItem);

        //        dictAvater.Add(locationItem.gameObject, avatarItem.gameObject);
        //    }

        //    this.UpdateAllTransform();
        //}


        public void InitCustomItems<T>(List<T> custom_items_) where T : GameUIComponent
        {

            this.m_location_item_pool.EnsureSize<GameImage>(custom_items_.Count);

            for (int i = 0; i < custom_items_.Count; ++i)
            {
                //创建location
                var locationItem = this.m_location_item_pool.GetChild<GameImage>(i);

#if !UNITY_EDITOR
                //locationItem.LazyLoader.enabled = false;
#endif
                locationItem.SetActive(true);
                locationItem.SetScale(Vector3.one);
                locationItem.gameObject.transform.rotation = Quaternion.identity;
                locationItem.gameObject.transform.localPosition = this.m_begin_move_tf.localPosition;

                this.m_location_itmes.Add(locationItem.gameObject);

                //创建avater
                var avatarItem = custom_items_[i];
                avatarItem.SetActive(true);
                avatarItem.gameObject.transform.localPosition = Vector3.one;
                avatarItem.gameObject.transform.localScale = Vector3.one;

                //设置属性
                //var tex = avatarItem.transform.Find("Text").GetComponent<Text>();
                //tex.text = i.ToString();

                this.m_avatar_items.Add(avatarItem);

                dictAvater.Add(locationItem.gameObject, avatarItem.gameObject);
            }

            this.UpdateAllTransform();
        }


        //更新卡牌位置
        public void UpdateAvaterTransform()
        {
            for (int i = 0; i < this.m_location_itmes.Count; ++i)
            {
                var pos = GetPositionInCanvas(this.m_location_itmes[i]);
                var avatarRT = this.m_avatar_items[i].Widget;
                avatarRT.anchoredPosition = pos;
                avatarRT.rotation = this.m_location_itmes[i].transform.rotation;
                //avatarRT.SetAsFirstSibling();
            }
        }

        //获得canvas下坐标
        public Vector2 GetPositionInCanvas(GameObject obj)
        {
            Vector2 localPoint;
            Vector2 localPoint2 = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, obj.transform.position);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTrans, RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, obj.transform.position), CameraManager.Instance.UICamera, out localPoint))
            {
                return localPoint;
            }

            //localPoint = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, obj.transform.position);
            //return localPoint;

            return Vector2.zero;
        }

        //调整视图上的显示顺序，保证近处遮挡远处
        public void AdjustShowOrder()
        {

            m_avatar_items.ForEach(p =>
            {
                p.Widget.SetAsFirstSibling();
            });

            //List<GameObject> avaters = new List<GameObject>(dictAvater.Values);
            //avaters.Sort(delegate (GameObject a, GameObject b)
            //{
            //    //if (a.transform.localPosition.y > b.transform.localPosition.y)
            //    //    return -1;

            //    if (a.transform.localPosition.x > b.transform.localPosition.x)
            //        return -1;

            //    return 0;
            //});

            //avaters.ForEach(p =>
            //{
            //    p.transform.SetAsLastSibling();
            //});
        }

        private void UpdateAvatarScale()
        {
            for (int i = 0; i < this.m_avatar_items.Count; ++i)
            {
                float cur_s_time;
                cur_s_time = this.m_cur_time - i * this.m_dt_between_2_card;
                cur_s_time = Mathf.Clamp(cur_s_time, 0.0f, this.m_s_total_time);
                float factor_s = this.m_scale_curve.Evaluate(cur_s_time);

                float cur_scale = Mathf.Lerp(this.m_min_scale, this.m_max_scale, factor_s);

                var avatarRT = this.m_avatar_items[i];

                avatarRT.Widget.localScale = new Vector3(cur_scale, cur_scale, 1.0f);
            }
        }

        private void UpdateAvatarBlur()
        {
            for (int i = 0; i < this.m_avatar_items.Count; ++i)
            {
                var avatar = this.m_avatar_items[i];

                if (!(avatar is BlurGrayTexture))
                {
                    continue;
                }

                var avatarRT = avatar as BlurGrayTexture;

                float cur_blur_time;
                cur_blur_time = this.m_cur_time - i * this.m_dt_between_2_card;
                cur_blur_time = Mathf.Clamp(cur_blur_time, 0.0f, this.m_h_total_time);
                //float factor_s = this.m_scale_curve.Evaluate(cur_blur_time);

                //float cur_scale = Mathf.Lerp(this.m_min_scale, this.m_max_scale, factor_s);



                //Debug.Log("当前模糊时间" + cur_blur_time);
                if (this.m_middle_time - m_dt_between_2_card_half < cur_blur_time && cur_blur_time < this.m_middle_time + m_dt_between_2_card_half)
                {
                    avatarRT.SetBlur(0.01f);
                }
                else
                {
                    avatarRT.SetBlur(4.5f);
                    //avatarRT.SetBlur(0.01f);
                }
            }
        }


        //旋转结束限制卡牌到每个分割点位置
        //private void OnTurntableEndDrag(GameObject target, PointerEventData eventData)
        //{   

        //    float min_delta_x = -1.0f;
        //    int min_index = 0;
        //    for (int i = 0; i < this.m_location_itmes.Count; ++i)
        //    {
        //        float cur_delta_x = this.m_location_itmes[i].transform.localPosition.x - this.m_middle_local_pos_x;
        //        float cur_delta_x_abs = Mathf.Abs(cur_delta_x);

        //        if (min_delta_x < 0.0f || cur_delta_x_abs < min_delta_x)
        //        {
        //            min_delta_x = cur_delta_x_abs;
        //            min_index = i;
        //        }
        //    }

        //    float min_index_item_time = this.m_cur_time - min_index * this.m_dt_between_2_card;
        //    float dt = this.m_middle_time - min_index_item_time;
        //    float aim_time = this.m_cur_time + dt;

        //    if (null != this.m_change_time_coroutine)
        //    {
        //        this.StopCoroutine(this.m_change_time_coroutine);
        //    }
        //    m_change_time_dt = 0.0f;
        //    this.m_change_time_coroutine = this.StartCoroutine(CoroutineChangeTime(this.m_cur_time, aim_time));
        //}

        private IEnumerator CoroutineChangeTime(float begin_time_, float end_time_)
        {
            //Debug.Log("时间差 " + (end_time_ - begin_time_));
            do
            {
                m_change_time_dt += Time.deltaTime * 10;
                float dt_factor = Mathf.Clamp(m_change_time_dt, 0.0f, 1.0f);
                this.m_cur_time = Mathf.Lerp(begin_time_, end_time_, dt_factor);
                this.UpdateAllTransform();

                yield return new WaitForEndOfFrame();
            }
            while (m_change_time_dt < 1.0f);

            if (null != MoveStopped)
            {
                MoveStopped(this.m_cur_selectd_police_idx);
            }

        }



        private void ChangeTime(float begin_time_, float end_time_, float speed_)
        {
            m_tween = DOTween.To(x => { this.m_cur_time = x; this.UpdateAllTransform(); }, begin_time_, end_time_, 0.4f * speed_).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                if (null != MoveStopped)
                {
                    MoveStopped(this.m_cur_selectd_police_idx);
                }
            });

        }



        //private void OnTurntableBeginDrag(GameObject target, PointerEventData eventData)
        //{
        //    if (null != this.m_change_time_coroutine)
        //    {
        //        this.StopCoroutine(this.m_change_time_coroutine);
        //    }

        //    beginPointerX = eventData.position.x;
        //}

        //private void OnTurntableDrag(GameObject target, PointerEventData eventData)
        //{

        //    float delta_time = 0.0f;

        //    //Debug.Log("滑动速度 " + eventData.delta.x);

        //    if (eventData.delta.x < -2.0f)
        //    {
        //        if (this.m_location_itmes[this.m_location_itmes.Count - 1].transform.localPosition.x <= this.m_middle_local_pos_x)
        //            return;

        //        float slide_factor = Mathf.Abs(eventData.delta.x) / 15.0f + 1.0f;
        //        //左滑动
        //        delta_time = Time.deltaTime * slide_factor;
        //    }
        //    else if (eventData.delta.x > 2.0f)
        //    {
        //        //右滑动
        //        if (this.m_location_itmes[0].transform.localPosition.x >= this.m_middle_local_pos_x)
        //            return;

        //        float slide_factor = Mathf.Abs(eventData.delta.x) / 15.0f + 1.0f;

        //        delta_time = -Time.deltaTime * slide_factor;
        //    }
        //    else
        //    {
        //        return;
        //    }

        //    this.m_cur_time += delta_time;

        //    this.UpdateAllTransform();
        //}

        private void UpdateAllTransform()
        {
            this.UpdateLocateRotation();
            this.UpdateLocateTransform();
            this.UpdateAvatarScale();
            this.UpdateAvaterTransform();
            this.UpdateAvatarBlur();
            this.AdjustShowOrder();
        }

        private void UpdateLocateRotation()
        {
            for (int i = 0; i < this.m_location_itmes.Count; ++i)
            {
                float cur_r_time;
                cur_r_time = this.m_cur_time - i * this.m_dt_between_2_card;
                cur_r_time = Mathf.Clamp(cur_r_time, 0.0f, this.m_r_total_time);
                float factor_r = this.m_rotate_curve.Evaluate(cur_r_time);
                Quaternion cur_r = Quaternion.Slerp(m_begin_rotate_tf.localRotation, m_end_rotate_tf.localRotation, factor_r);
                this.m_location_itmes[i].transform.localRotation = cur_r;
            }
        }

        private void UpdateLocateTransform()
        {
            for (int i = 0; i < this.m_location_itmes.Count; ++i)
            {
                float cur_h_time, cur_v_time;
                cur_h_time = cur_v_time = this.m_cur_time - i * this.m_dt_between_2_card;
                cur_h_time = Mathf.Clamp(cur_h_time, 0.0f, this.m_h_total_time);
                float factor_x = this.m_H_curve.Evaluate(cur_h_time);
                float _x = m_begin_move_tf.localPosition.x * (1f - factor_x) + m_end_move_tf.localPosition.x * factor_x;

                cur_v_time = Mathf.Clamp(cur_v_time, 0.0f, this.m_v_total_time);
                float factor_y = this.m_V_curve.Evaluate(cur_v_time);
                float _y = m_begin_move_tf.localPosition.y * (1f - factor_y) + m_end_move_tf.localPosition.y * factor_y;

                this.m_location_itmes[i].transform.localPosition = new Vector3(_x, _y, m_begin_move_tf.localPosition.z);
            }
        }


        private void GetCurveXY(int index_, float cur_t_, out float x_, out float y_)
        {
            float cur_h_time, cur_v_time;
            cur_h_time = cur_v_time = cur_t_ - index_ * this.m_dt_between_2_card;
            cur_h_time = Mathf.Clamp(cur_h_time, 0.0f, this.m_h_total_time);
            float factor_x = this.m_H_curve.Evaluate(cur_h_time);
            x_ = m_begin_move_tf.localPosition.x * (1f - factor_x) + m_end_move_tf.localPosition.x * factor_x;

            cur_v_time = Mathf.Clamp(cur_v_time, 0.0f, this.m_v_total_time);
            float factor_y = this.m_V_curve.Evaluate(cur_v_time);
            y_ = m_begin_move_tf.localPosition.y * (1f - factor_y) + m_end_move_tf.localPosition.y * factor_y;
        }

        private Vector3 GetCurveWorldPos(float cur_t_)
        {
            float cur_h_time, cur_v_time;
            cur_h_time = cur_v_time = cur_t_;
            cur_h_time = Mathf.Clamp(cur_h_time, 0.0f, this.m_h_total_time);
            float factor_x = this.m_H_curve.Evaluate(cur_h_time);
            float _x = m_begin_move_tf.position.x * (1f - factor_x) + m_end_move_tf.position.x * factor_x;

            cur_v_time = Mathf.Clamp(cur_v_time, 0.0f, this.m_v_total_time);
            float factor_y = this.m_V_curve.Evaluate(cur_v_time);
            float _y = m_begin_move_tf.position.y * (1f - factor_y) + m_end_move_tf.position.y * factor_y;

            return new Vector3(_x, _y, m_begin_move_tf.position.z);
        }

        private Vector3 GetCurveLocalPos(float cur_t_)
        {
            float cur_h_time, cur_v_time;
            cur_h_time = cur_v_time = cur_t_;
            cur_h_time = Mathf.Clamp(cur_h_time, 0.0f, this.m_h_total_time);
            float factor_x = this.m_H_curve.Evaluate(cur_h_time);
            float _x = m_begin_move_tf.localPosition.x * (1f - factor_x) + m_end_move_tf.localPosition.x * factor_x;

            cur_v_time = Mathf.Clamp(cur_v_time, 0.0f, this.m_v_total_time);
            float factor_y = this.m_V_curve.Evaluate(cur_v_time);
            float _y = m_begin_move_tf.localPosition.y * (1f - factor_y) + m_end_move_tf.localPosition.y * factor_y;

            return new Vector3(_x, _y, m_begin_move_tf.localPosition.z);
        }

        //void OnDrawGizmos()
        //{


        //    if (this.m_h_total_time < 0.0f || this.m_v_total_time < 0.0f)
        //    {
        //        this.m_h_total_time = 1.5f;
        //        this.m_v_total_time = 1.5f;
        //    }


        //    Vector3 line_begin = this.m_begin_move_tf.position;
        //    Color color = Color.red;


        //    for (float t = 0.0f; t <= 1.5f; t += 0.1f)
        //    {
        //        Vector3 vec = GetCurveWorldPos(t);
        //        Gizmos.color = color;
        //        Gizmos.DrawLine(line_begin, vec);

        //        line_begin = vec;
        //    }

        //    Gizmos.color = Color.green;
        //    Gizmos.DrawSphere(this.GetCurveWorldPos(this.m_middle_time), 0.3f);
        //}


    }
}
