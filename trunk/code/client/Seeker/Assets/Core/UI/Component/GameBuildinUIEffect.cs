/********************************************************************
	created:  2019-1-25 15:43:40
	filename: GameBuildinUIEffect.cs
	author:	  songguangze@outlook.com
	
	purpose:  UI内置的特效对象, 用于直接挂到UI节点上特效的生命周期维护
*********************************************************************/
using EngineCore;
using SeekerGame;
using UnityEngine;


public class GameBuildinUIEffect : MonoBehaviour
{
    private GameUIEffect m_buildInEffectComponent = null;
    private UIEffectCanvas m_buildinEffectCavnasInfo = null;
    private UILogicBase m_ownerUILogic = null;

    public void InitBuildinUIEffect(string effectName, UILogicBase ownerUILogic)
    {
        this.m_ownerUILogic = ownerUILogic;

        this.m_buildInEffectComponent = ownerUILogic.Make<GameUIEffect>(gameObject);

        this.m_buildInEffectComponent.EffectPrefabName = effectName;

        this.m_buildInEffectComponent.Visible = gameObject.activeInHierarchy && m_ownerUILogic.UIFrame.Visible;
    }
}
