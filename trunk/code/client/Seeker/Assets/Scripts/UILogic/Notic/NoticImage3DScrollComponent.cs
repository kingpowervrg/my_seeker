using EngineCore;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
namespace SeekerGame
{
    public class NoticImage3DScrollComponent : Scroll3DComponent
    {

        public void SetData(List<NoticeInfo> noticeInfo)
        {
            ResetItemCom();
            count = noticeInfo.Count;
            itemArray = new NoticImage3DItem[count];
            this.m_container.EnsureSize<NoticImage3DItem>(count);
            for (int i = 0; i < count; i++)
            {
                NoticImage3DItem noticItem = this.m_container.GetChild<NoticImage3DItem>(i);
                noticItem.SetData(noticeInfo[i]);
                noticItem.SetData(count, i, m_centerPos, this.m_leftParent);
                noticItem.Visible = true;
                itemArray[i] = noticItem;
            }
        }

        public override void OnHide()
        {
            base.OnHide();
        }


        public class NoticImage3DItem : Scroll3DItem
        {
            private GameTexture m_texture = null;

            protected override void OnInit()
            {
                base.OnInit();
                this.m_texture = Make<GameTexture>(gameObject);
            }

            public void SetData(NoticeInfo noticInfo)
            {
                m_texture.TextureName = noticInfo.Picture;
            }

        }
    }
}
