//#define TEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using UnityEngine;
using GOEngine;
using Google.Protobuf;
using DG.Tweening;
using DG.Tweening.Plugins.Options;

namespace SeekerGame
{
    public class ScanRewardView : BaseViewComponet<ScanGameUILogic>
    {
        class FlyItem
        {
            public int group_id = 0;
            public int fly_count = 0;
        }


        const float C_FLY_INTERVAL_TIME = 0.05f;
        const float C_FLY_INTERVAL_HALF_TIME = 0.025f;

        long m_scan_id;
        GameRecycleContainer m_normal_effect_grid;
        GameRecycleContainer m_special_effect_grid;
        GameRecycleContainer m_num_grid;
#if MULTI_SHAKE
        GameRecycleContainer m_normal_shake_effect_grid;
        GameUIEffect m_special_shake_effect;
#else
        ShakeVitEffectItemView2 m_normal_shake;
        ShakeVitEffectItemView2 m_special_shake;
#endif




        GameImage m_vit_icon;

        Queue<List<FlyItem>> m_prepare_fly_group_queue = new Queue<List<FlyItem>>();

        List<Shooter> m_shooters = new List<Shooter>();
        protected override void OnInit()
        {
            base.OnInit();
            m_normal_effect_grid = Make<GameRecycleContainer>("Grid");
            m_special_effect_grid = Make<GameRecycleContainer>("Grid (1)");
            m_num_grid = Make<GameRecycleContainer>("Grid_Txt");
            m_vit_icon = Make<GameImage>("Image (1)");
#if MULTI_SHAKE
            m_normal_shake_effect_grid = m_vit_icon.Make<GameRecycleContainer>("Grid_Normal");
            m_special_shake_effect = m_vit_icon.Make<GameUIEffect>("UI_tili_jiesuan_doudong_02");
            m_special_shake_effect.EffectPrefabName = "UI_tili_jiesuan_doudong_02.prefab";
#else
            m_normal_shake = Make<ShakeVitEffectItemView2>("Shake");
            m_normal_shake.InitPrefabNameAndDelay("UI_tili_jiesuan_doudong", 0.2f);
            m_special_shake = Make<ShakeVitEffectItemView2>("Shake02");
            m_special_shake.InitPrefabNameAndDelay("UI_tili_jiesuan_doudong_02", 0.5f);
#endif
        }



        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_prepare_fly_group_queue.Clear();


            if (null != param)
            {
                List<long> my_param = param as List<long>;
                this.m_scan_id = my_param[0];
            }
            else
            {
                m_scan_id = CurViewLogic().Scan_id;
            }


            int max = 0;
            List<FlyItem> one_wave = new List<FlyItem>();
            foreach (var kvp in CurViewLogic().Cur_game_finded_clues)
            {
                FlyItem fi = new FlyItem()
                {
                    group_id = kvp.Key,
                    fly_count = kvp.Value.Count,
                };

                one_wave.Add(fi);

                max = kvp.Value.Count > max ? kvp.Value.Count : max;
            }

            if (0 == one_wave.Count)
            {
                FlyVitIcon();
                return;
            }

            max *= CurViewLogic().Cur_game_finded_clues.Keys.Count;

            m_prepare_fly_group_queue.Enqueue(one_wave);


#if TEST
            max += 10;
#endif

            if (CurViewLogic().Win)
            {
                one_wave = new List<FlyItem>();

                FlyItem fi = new FlyItem()
                {
                    group_id = 999,
                    fly_count = 1,
                };

                one_wave.Add(fi);

                m_prepare_fly_group_queue.Enqueue(one_wave);

                m_special_effect_grid.EnsureSize<FlySpecialVitEffectItemView>(1);
            }


            m_normal_effect_grid.EnsureSize<FlyNromalVitEffectItemView>(max + 3);
#if MULTI_SHAKE
            m_normal_shake_effect_grid.EnsureSize<ShakeVitEffectItemView>(max + 3);
#endif
            m_num_grid.EnsureSize<FlyNumItemView>(max);
            m_shooters.Clear();

            TimeModule.Instance.SetTimeInterval(UpdateFlyEffect, C_FLY_INTERVAL_TIME);

            FlyEffect();

        }

        public override void OnHide()
        {
            base.OnHide();

            TimeModule.Instance.RemoveTimeaction(UpdateFlyEffect);
        }


        public void RecycleFlyVitEffect(FlyVitEffectItemView view_)
        {

            if (view_ is FlyNromalVitEffectItemView)
                m_normal_effect_grid.RecycleElement<FlyNromalVitEffectItemView>(view_ as FlyNromalVitEffectItemView);
            else if (view_ is FlySpecialVitEffectItemView)
                m_special_effect_grid.RecycleElement<FlySpecialVitEffectItemView>(view_ as FlySpecialVitEffectItemView);
        }




        public void RecycleFlyNum(FlyNumItemView view_)
        {
            m_num_grid.RecycleElement<FlyNumItemView>(view_);
        }

        public void FlyVitNum(int num_)
        {
            var fly_num = m_num_grid.GetAvaliableContainerElement<FlyNumItemView>();
            fly_num.Refresh(num_);
            fly_num.Visible = true;
        }

        public void ShakeVit(FlyVitEffectItemView view_)
        {
            m_vit_icon.Visible = false;

            if (view_ is FlyNromalVitEffectItemView)
            {
#if MULTI_SHAKE
                var shake = m_normal_shake_effect_grid.GetAvaliableContainerElement<ShakeVitEffectItemView>();
                shake.RegisterRecycleParent(m_normal_shake_effect_grid);
                shake.Visible = true;
#else
                m_normal_shake.Shake();
#endif
            }
            else if (view_ is FlySpecialVitEffectItemView)
            {
#if MULTI_SHAKE
                m_special_shake_effect.Visible = true;
#else
                m_special_shake.Shake();
#endif
            }
        }

        public void ShowVitIcon()
        {
            m_vit_icon.Visible = true;
        }



        class Shooter
        {
            Queue<float> m_threhold_time = new Queue<float>();
            public long group_id;

            GameRecycleContainer m_gun;
            Vector3 m_cur_wave_from_pos;
            Vector3 m_cur_wave_to_pos;
            FlyItem m_cur_wave = null;
            float m_time_sum;
            bool m_is_finished;
            public bool Is_finished
            {
                get { return m_is_finished; }
            }

            bool m_is_wait_for_first_fire = true;

            public Shooter()
            {
                m_threhold_time.Enqueue(0.5f);
                m_threhold_time.Enqueue(0.3f);
                m_threhold_time.Enqueue(0.2f);
            }

            public void Load(long group_id_, GameRecycleContainer gun_, FlyItem fi_, Vector3 from_pos_, Vector3 to_pos_, float delay_)
            {
                group_id = group_id_;
                m_gun = gun_;
                m_cur_wave = fi_;
                m_cur_wave_from_pos = from_pos_;
                m_cur_wave_to_pos = to_pos_;
                m_time_sum = 0.0f - delay_;
                m_is_finished = false;
                m_is_wait_for_first_fire = true;
#if TEST
                if (999 != group_id_)
                    m_cur_wave.fly_count += 10;
#endif
            }

            public void Unload()
            {
                m_cur_wave = null;
            }

            public void UpdateShooting()
            {
                if (null == m_cur_wave || 0 == m_cur_wave.fly_count)
                    return;

                float sum_before_add = m_time_sum;

                m_time_sum += C_FLY_INTERVAL_TIME;

                int count = 999 == m_cur_wave.group_id ? 5 : 1;

                if (m_is_wait_for_first_fire)
                {
                    if (m_time_sum > 0.0f)
                    {
                        m_time_sum = 0;
                        m_is_wait_for_first_fire = false;
                        Fire(count);
                    }
                    return;
                }


                float threhold_time; // = m_cur_wave.fly_count * C_FLY_INTERVAL_HALF_TIME;

                if (0 == m_threhold_time.Count)
                {
                    threhold_time = 0.1f;
                }
                else
                {
                    threhold_time = m_threhold_time.Peek();
                }

                if (m_time_sum > threhold_time)
                {
                    if (0 != m_threhold_time.Count)
                    {
                        m_threhold_time.Dequeue();
                    }

                    Fire(count);
                }
            }
            void Fire(int count_)
            {
                var vit_effect = m_gun.GetAvaliableContainerElement<FlyVitEffectItemView>();
                vit_effect.Refresh(count_, m_cur_wave_from_pos, m_cur_wave_to_pos);
                vit_effect.Visible = true;

                m_time_sum = 0;
                --m_cur_wave.fly_count;

                if (0 == m_cur_wave.fly_count)
                {
                    m_is_finished = true;
                }
            }
        }




        void UpdateFlyEffect()
        {

            if (0 == m_shooters.Count || 0 == m_shooters.FindAll((item) => false == item.Is_finished).Count)
                return;

            m_shooters.ForEach((item) => item.UpdateShooting());


            if (0 == m_shooters.FindAll((item) => false == item.Is_finished).Count)
            {
                m_shooters.Clear();
                TimeModule.Instance.SetTimeout(FlyEffect, 1.0f);
            }
        }

        void FlyEffect()
        {
            if (0 == m_prepare_fly_group_queue.Count)
            {
                FlyVitIcon();
                return;
            }


            List<FlyItem> cur_wave = m_prepare_fly_group_queue.Dequeue();

            for (int i = 0; i < cur_wave.Count; ++i)
            {
                Shooter shooter = new Shooter();

                var item = cur_wave[i];

                GameRecycleContainer gun = 999 == item.group_id ? m_special_effect_grid : m_normal_effect_grid;

                shooter.Load(item.group_id, gun, item, CurViewLogic().GetClueProgressItemViewWorldPos(item.group_id), m_vit_icon.Position, 0.3f * i);
                m_shooters.Add(shooter);
            }

        }

        void FlyingVitIcon()
        {
            m_vit_icon.Visible = false;
#if MULTI_SHAKE
            m_special_shake_effect.Visible = false;
#endif
            CurViewLogic().FlyVitIcon();
            this.Visible = false;

            Debug.Log("FlyVitIcon");
        }

        void FlyVitIcon()
        {
            TimeModule.Instance.SetTimeout(FlyingVitIcon, 1.0f);
        }

    }


}