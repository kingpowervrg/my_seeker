using System.Collections.Generic;

namespace SeekerGame
{
	public class ConfFact
	{
		static bool reloadStarted = false;
		public static void Register()
		{
				 ConfAchievement.Init();
				 ConfActiveDrop.Init();
				 ConfActivityBase.Init();
				 ConfAssetManifest.Init();
				 Confbranchtask.Init();
				 ConfBuilding.Init();
				 ConfCartoonScene.Init();
				 ConfChapter.Init();
				 ConfCharge.Init();
				 ConfChat.Init();
				 ConfChatItem.Init();
				 ConfCheckIn.Init();
				 ConfCombineFormula.Init();
				 ConfDemoScene.Init();
				 ConfDropOut.Init();
				 ConfPropGiftItem0.Init();
				 ConfPropGiftItem1.Init();
				 ConfPropGiftItem2.Init();
				 ConfDropOut2.Init();
				 Confetl.Init();
				 ConfEvent.Init();
				 ConfEventAttribute.Init();
				 ConfEventPhase.Init();
				 Confexhibit.Init();
				 ConfExpToLevel.Init();
				 Conffailtips.Init();
				 ConfFeedback.Init();
				 ConfFind.Init();
				 ConfFindClue.Init();
				 ConfFindTypeIcon.Init();
				 ConfGMCMD.Init();
				 ConfGuid.Init();
				 ConfGuidArt.Init();
				 ConfGuidNew.Init();
				 ConfGuidNewFunction.Init();
				 Confinapppurchase.Init();
				 ConfJigsawScene.Init();
				 ConfKeyWords.Init();
				 ConfLanguage.Init();
				 ConfMsgCode.Init();
				 ConfNode.Init();
				 ConfNpc.Init();
				 ConfOfficer.Init();
				 ConfPath.Init();
				 ConfPoliceRankIcon.Init();
				 ConfProp.Init();
				 ConfPropGift.Init();
				 ConfPropGiftItem0.Init();
				 ConfPropGiftItem1.Init();
				 ConfPropGiftItem2.Init();
				 ConfPush.Init();
				 ConfReasoning.Init();
				 ConfScene.Init();
				 ConfSceneDifficulty.Init();
				 ConfSceneSpecial.Init();
				 ConfServiceConfig.Init();
				 ConfSkill.Init();
				 ConfSkyEye.Init();
				 ConfSound.Init();
				 ConfTask.Init();
				 ConfTitle.Init();
			
		}
		public static bool ResLoaded()
		{
		 if(!ConfAchievement.resLoaded)
				return false;
		 if(!ConfActiveDrop.resLoaded)
				return false;
		 if(!ConfActivityBase.resLoaded)
				return false;
		 if(!ConfAssetManifest.resLoaded)
				return false;
		 if(!Confbranchtask.resLoaded)
				return false;
		 if(!ConfBuilding.resLoaded)
				return false;
		 if(!ConfCartoonScene.resLoaded)
				return false;
		 if(!ConfChapter.resLoaded)
				return false;
		 if(!ConfCharge.resLoaded)
				return false;
		 if(!ConfChat.resLoaded)
				return false;
		 if(!ConfChatItem.resLoaded)
				return false;
		 if(!ConfCheckIn.resLoaded)
				return false;
		 if(!ConfCombineFormula.resLoaded)
				return false;
		 if(!ConfDemoScene.resLoaded)
				return false;
		 if(!ConfDropOut.resLoaded)
				return false;
		 if(!ConfPropGiftItem0.resLoaded)
				return false;
		 if(!ConfPropGiftItem1.resLoaded)
				return false;
		 if(!ConfPropGiftItem2.resLoaded)
				return false;
		 if(!ConfDropOut2.resLoaded)
				return false;
		 if(!Confetl.resLoaded)
				return false;
		 if(!ConfEvent.resLoaded)
				return false;
		 if(!ConfEventAttribute.resLoaded)
				return false;
		 if(!ConfEventPhase.resLoaded)
				return false;
		 if(!Confexhibit.resLoaded)
				return false;
		 if(!ConfExpToLevel.resLoaded)
				return false;
		 if(!Conffailtips.resLoaded)
				return false;
		 if(!ConfFeedback.resLoaded)
				return false;
		 if(!ConfFind.resLoaded)
				return false;
		 if(!ConfFindClue.resLoaded)
				return false;
		 if(!ConfFindTypeIcon.resLoaded)
				return false;
		 if(!ConfGMCMD.resLoaded)
				return false;
		 if(!ConfGuid.resLoaded)
				return false;
		 if(!ConfGuidArt.resLoaded)
				return false;
		 if(!ConfGuidNew.resLoaded)
				return false;
		 if(!ConfGuidNewFunction.resLoaded)
				return false;
		 if(!Confinapppurchase.resLoaded)
				return false;
		 if(!ConfJigsawScene.resLoaded)
				return false;
		 if(!ConfKeyWords.resLoaded)
				return false;
		 if(!ConfLanguage.resLoaded)
				return false;
		 if(!ConfMsgCode.resLoaded)
				return false;
		 if(!ConfNode.resLoaded)
				return false;
		 if(!ConfNpc.resLoaded)
				return false;
		 if(!ConfOfficer.resLoaded)
				return false;
		 if(!ConfPath.resLoaded)
				return false;
		 if(!ConfPoliceRankIcon.resLoaded)
				return false;
		 if(!ConfProp.resLoaded)
				return false;
		 if(!ConfPropGift.resLoaded)
				return false;
		 if(!ConfPropGiftItem0.resLoaded)
				return false;
		 if(!ConfPropGiftItem1.resLoaded)
				return false;
		 if(!ConfPropGiftItem2.resLoaded)
				return false;
		 if(!ConfPush.resLoaded)
				return false;
		 if(!ConfReasoning.resLoaded)
				return false;
		 if(!ConfScene.resLoaded)
				return false;
		 if(!ConfSceneDifficulty.resLoaded)
				return false;
		 if(!ConfSceneSpecial.resLoaded)
				return false;
		 if(!ConfServiceConfig.resLoaded)
				return false;
		 if(!ConfSkill.resLoaded)
				return false;
		 if(!ConfSkyEye.resLoaded)
				return false;
		 if(!ConfSound.resLoaded)
				return false;
		 if(!ConfTask.resLoaded)
				return false;
		 if(!ConfTitle.resLoaded)
				return false;
		return true;
		}
		public static void ReloadConfig()
		{
				 ConfAchievement.Clear();
				 ConfAchievement.Init();
				 ConfActiveDrop.Clear();
				 ConfActiveDrop.Init();
				 ConfActivityBase.Clear();
				 ConfActivityBase.Init();
				 ConfAssetManifest.Clear();
				 ConfAssetManifest.Init();
				 Confbranchtask.Clear();
				 Confbranchtask.Init();
				 ConfBuilding.Clear();
				 ConfBuilding.Init();
				 ConfCartoonScene.Clear();
				 ConfCartoonScene.Init();
				 ConfChapter.Clear();
				 ConfChapter.Init();
				 ConfCharge.Clear();
				 ConfCharge.Init();
				 ConfChat.Clear();
				 ConfChat.Init();
				 ConfChatItem.Clear();
				 ConfChatItem.Init();
				 ConfCheckIn.Clear();
				 ConfCheckIn.Init();
				 ConfCombineFormula.Clear();
				 ConfCombineFormula.Init();
				 ConfDemoScene.Clear();
				 ConfDemoScene.Init();
				 ConfDropOut.Clear();
				 ConfDropOut.Init();
				 ConfPropGiftItem0.Clear();
				 ConfPropGiftItem0.Init();
				 ConfPropGiftItem1.Clear();
				 ConfPropGiftItem1.Init();
				 ConfPropGiftItem2.Clear();
				 ConfPropGiftItem2.Init();
				 ConfDropOut2.Clear();
				 ConfDropOut2.Init();
				 Confetl.Clear();
				 Confetl.Init();
				 ConfEvent.Clear();
				 ConfEvent.Init();
				 ConfEventAttribute.Clear();
				 ConfEventAttribute.Init();
				 ConfEventPhase.Clear();
				 ConfEventPhase.Init();
				 Confexhibit.Clear();
				 Confexhibit.Init();
				 ConfExpToLevel.Clear();
				 ConfExpToLevel.Init();
				 Conffailtips.Clear();
				 Conffailtips.Init();
				 ConfFeedback.Clear();
				 ConfFeedback.Init();
				 ConfFind.Clear();
				 ConfFind.Init();
				 ConfFindClue.Clear();
				 ConfFindClue.Init();
				 ConfFindTypeIcon.Clear();
				 ConfFindTypeIcon.Init();
				 ConfGMCMD.Clear();
				 ConfGMCMD.Init();
				 ConfGuid.Clear();
				 ConfGuid.Init();
				 ConfGuidArt.Clear();
				 ConfGuidArt.Init();
				 ConfGuidNew.Clear();
				 ConfGuidNew.Init();
				 ConfGuidNewFunction.Clear();
				 ConfGuidNewFunction.Init();
				 Confinapppurchase.Clear();
				 Confinapppurchase.Init();
				 ConfJigsawScene.Clear();
				 ConfJigsawScene.Init();
				 ConfKeyWords.Clear();
				 ConfKeyWords.Init();
				 ConfLanguage.Clear();
				 ConfLanguage.Init();
				 ConfMsgCode.Clear();
				 ConfMsgCode.Init();
				 ConfNode.Clear();
				 ConfNode.Init();
				 ConfNpc.Clear();
				 ConfNpc.Init();
				 ConfOfficer.Clear();
				 ConfOfficer.Init();
				 ConfPath.Clear();
				 ConfPath.Init();
				 ConfPoliceRankIcon.Clear();
				 ConfPoliceRankIcon.Init();
				 ConfProp.Clear();
				 ConfProp.Init();
				 ConfPropGift.Clear();
				 ConfPropGift.Init();
				 ConfPropGiftItem0.Clear();
				 ConfPropGiftItem0.Init();
				 ConfPropGiftItem1.Clear();
				 ConfPropGiftItem1.Init();
				 ConfPropGiftItem2.Clear();
				 ConfPropGiftItem2.Init();
				 ConfPush.Clear();
				 ConfPush.Init();
				 ConfReasoning.Clear();
				 ConfReasoning.Init();
				 ConfScene.Clear();
				 ConfScene.Init();
				 ConfSceneDifficulty.Clear();
				 ConfSceneDifficulty.Init();
				 ConfSceneSpecial.Clear();
				 ConfSceneSpecial.Init();
				 ConfServiceConfig.Clear();
				 ConfServiceConfig.Init();
				 ConfSkill.Clear();
				 ConfSkill.Init();
				 ConfSkyEye.Clear();
				 ConfSkyEye.Init();
				 ConfSound.Clear();
				 ConfSound.Init();
				 ConfTask.Clear();
				 ConfTask.Init();
				 ConfTitle.Clear();
				 ConfTitle.Init();
			reloadStarted = true;
		}
		public static bool IsReloadCompleted()
		{
			if(reloadStarted)
			{
				if(ResLoaded())
				{
					reloadStarted = false;
					return true;
				}
			}
			return false;
		}
	}
}