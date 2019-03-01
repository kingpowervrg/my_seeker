using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class NormalItemView : GameUIComponent
    {
        protected long m_unique_id;
        public long Unique_id
        {
            get { return m_unique_id; }
        }



        public void Refresh(long unique_id_)
        {
            m_unique_id = unique_id_;
        }

        public void SetPos(Vector2 local_pos_)
        {
            this.Widget.anchoredPosition = local_pos_;
        }

        protected override void OnInit()
        {
            base.OnInit();



        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public override string ToString() => $"Item View {m_unique_id}";
    }

}
