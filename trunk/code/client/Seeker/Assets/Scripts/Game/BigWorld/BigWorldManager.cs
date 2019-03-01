using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame
{
    public class BigWorldManager : Singleton<BigWorldManager>
    {
        BigWorldSystem m_bigWorldSystem;

        public BigWorldManager()
        {

        }

        public void LoadBigWorld(string openUIName = "", bool needShowMainUI = true,long buildID = 1)
        {
            if (m_bigWorldSystem != null)
            {
                GameEvents.BigWorld_Event.OnCameraMove.SafeInvoke(buildID);
                return;
            }
            SeekerGame.NewGuid.GuidNewManager.Instance.ReLoadGuid(10006);
            UnityEngine.Debug.Log("Load BigWorld");
            HedgehogTeam.EasyTouch.EasyTouch.SetEnableAutoSelect(false);
            m_bigWorldSystem = new BigWorldSystem();
            if (m_bigWorldSystem.IsNeedProgress())
            {
                LoadingProgressManager.Instance.LoadBigWorldScene(() =>
                {
                    m_bigWorldSystem.OnEnterBigWorld(openUIName,needShowMainUI, buildID);
                });
            }
            else
            {
                m_bigWorldSystem.OnEnterBigWorld("",true,buildID);
            }
        }

        public void EnterBigWorld()
        {
            if (m_bigWorldSystem == null)
            {
                LoadBigWorld();
                return;
            }
            m_bigWorldSystem.OnEnterBigWorld();
        }

        public void ClearBigWorld()
        {
            if (m_bigWorldSystem != null)
            {
                m_bigWorldSystem.OnDestory();
                GameEvents.UIEvents.UI_Loading_Event.OnStartLoading = null;
                m_bigWorldSystem = null;
            }
        }

        public void OnBigWorldCanUnLock(long taskId)
        {
            List<ConfBuilding> building = ConfBuilding.array;
            for (int i = 0; i < building.Count; i++)
            {
                if (building[i].unlockTask == taskId)
                {
                    GameEvents.BigWorld_Event.OnReflashBigWorld.SafeInvoke();
                    return;
                }
            }
        }
    }
}
