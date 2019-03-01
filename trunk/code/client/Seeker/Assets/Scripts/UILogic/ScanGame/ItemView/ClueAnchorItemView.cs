using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class ClueAnchorItemView : GameUIComponent
    {
        long m_clue_id;
        public long Clue_id
        {
            get { return m_clue_id; }
        }


        public override void OnShow(object param)
        {
            base.OnShow(param);

            this.AddPressDownCallBack(OnPressDown);
            this.AddPressUpCallBack(OnPressUp);
        }

        public override void OnHide()
        {
            base.OnHide();

            this.RemovePressDownCallBack(OnPressDown);
            this.RemovePressDownCallBack(OnPressUp);
        }


        public void Refresh(long id_, float x, float y)
        {

            m_clue_id = id_;

            this.Widget.anchoredPosition = new Vector2(x, y);

        }

        void OnPressDown(GameObject obj)
        {

        }

        void OnPressUp(GameObject obj)
        {

        }
    }

}
