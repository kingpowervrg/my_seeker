using System;
using System.Collections.Generic;
using Google.Protobuf;
using SeekerGame;
using SeekerGame.NewGuid;
namespace EngineCore
{
    public class MessageStandalonHandler
    {
        public static void Call(IMessage message)
        {
            //IMessage response = null;
            int messageId = message.GetMessageId();
            UnityEngine.Debug.Log("send standalon message " + messageId);
            switch (messageId)
            {
                case MessageDefine.CSSceneSuspendRequest: //场景暂停
                    SCSceneSuspendResponse suspendRes = new SCSceneSuspendResponse();
                    suspendRes.Result = 0;
                    MessageHandler.Call(MessageDefine.SCSceneSuspendResponse, suspendRes);
                    break;
                case MessageDefine.SCSuspendResponse: //拼图场景暂停
                    {
                        SCSuspendResponse suspendResponse = new SCSuspendResponse();
                        suspendResponse.Result = 0;
                        MessageHandler.Call(MessageDefine.SCSuspendResponse, suspendResponse);
                    }
                    break;
                case MessageDefine.CSSceneResumeRequest: //场景恢复
                    SCSceneResumeResponse resumeRes = new SCSceneResumeResponse();
                    resumeRes.Result = 0;
                    MessageHandler.Call(MessageDefine.SCSceneResumeResponse, resumeRes);
                    break;
                case MessageDefine.CSResumeRequest://拼图场景恢复
                    {
                        SCResumeResponse resumeResponse = new SCResumeResponse();
                        resumeResponse.Result = 0;
                        MessageHandler.Call(MessageDefine.SCResumeResponse, resumeResponse);
                    }
                    break;
                case MessageDefine.CSSkillEmitRequest: //技能释放
                    CSSkillEmitRequest emitReq = (CSSkillEmitRequest)message;
                    SCSkillEmitResponse emitRes = new SCSkillEmitResponse();
                    long propID = emitReq.PropId;
                    emitRes.PropId = emitReq.PropId;
                    ConfProp confProp = ConfProp.Get(propID);
                    if (confProp == null)
                    {
                        emitRes.SkillId = 0;
                        emitRes.Result = 0;
                        MessageHandler.Call(MessageDefine.SCSkillEmitResponse, emitRes);
                        return;
                    }
                    emitRes.SkillId = confProp.skillId;
                    emitRes.Result = 1;
                    MessageHandler.Call(MessageDefine.SCSkillEmitResponse, emitRes);
                    break;

                case MessageDefine.CSSceneRewardRequest:  //局内结算
                    #region
                    CSSceneRewardRequest rewardRequest = (CSSceneRewardRequest)message;
                    ConfScene confScene = ConfScene.Get(rewardRequest.SceneId);
                    SCSceneRewardResponse rewardRes = new SCSceneRewardResponse();
                    rewardRes.SceneId = rewardRequest.SceneId;
                    rewardRes.OutputCoin = confScene.outputMoney;
                    rewardRes.OutputCash = confScene.outputCash;
                    rewardRes.OutputVit = confScene.outputVit;
                    rewardRes.OutputExp = confScene.outputExp;
                    if (confScene.dropId > 0)
                    {
                        ConfDropOut2 dropOut = ConfDropOut2.Get(confScene.dropId);
                        if (!string.IsNullOrEmpty(dropOut.fixed2))
                        {
                            List<DropOutJsonData> rdm_datas = CommonHelper.ParseDropOut(dropOut.fixed2);
                            for (int i = 0; i < rdm_datas.Count; i++)
                            {
                                GiftItem giftItem = new GiftItem();
                                giftItem.ItemId = rdm_datas[i].value;
                                giftItem.Num = rdm_datas[i].count;
                                rewardRes.GiftItems.Add(giftItem);
                            }
                        }
                    }
                    MessageHandler.Call(MessageDefine.SCSceneRewardResponse, rewardRes);
                    #endregion
                    break;
                case MessageDefine.CSBuildingListReq: //建筑物数据
                    #region
                    SCBuildingListResp buildRes = new SCBuildingListResp();
                    BuildingInfo buildInfo = new BuildingInfo();
                    buildInfo.BuildingId = 1;
                    buildInfo.Status = 1;
                    buildRes.Infos.Add(buildInfo);
                    MessageHandler.Call(MessageDefine.SCBuildingListResp, buildRes);
                    #endregion
                    break;
                case MessageDefine.CSTaskIdListRequest: //获取任务列表
                    #region
                    //SCTaskIdListResponse taskIdListRes = new SCTaskIdListResponse();
                    //TaskIdInfo taskIdInfo = new TaskIdInfo();
                    //taskIdInfo.TaskId = 6001;
                    //taskIdInfo.PlayerTaskId = taskIdInfo.TaskId * 10;
                    //taskIdListRes.TaskIdInfos.Add(taskIdInfo);
                    //MessageHandler.Call(MessageDefine.SCTaskIdListResponse, taskIdListRes);
                    #endregion
                    break;
                case MessageDefine.CSTaskListRequest:
                    #region 任务
                    CSTaskListRequest taskListRequest = (CSTaskListRequest)message;

                    if (taskListRequest.TaskIds.Count > 1)
                    {
                        //SCAcceptTaskNotice taskNotice = new SCAcceptTaskNotice();
                        if (taskListRequest.TaskIds[0] > 0)
                        {
                            SCTaskStatusChangeNotice taskStatusChange = new SCTaskStatusChangeNotice();
                            taskStatusChange.PlayerTaskId = taskListRequest.TaskIds[0] * 10;
                            taskStatusChange.TaskId = taskListRequest.TaskIds[0];
                            taskStatusChange.Status = 3;
                            MessageHandler.Call(MessageDefine.SCTaskStatusChangeNotice, taskStatusChange);
                        }

                        SCAcceptTaskNotice taskNotice = new SCAcceptTaskNotice();

                        AcceptTaskInfo acceptNewTaskInfo = new AcceptTaskInfo();
                        acceptNewTaskInfo.TaskId = taskListRequest.TaskIds[1];
                        acceptNewTaskInfo.PlayerTaskId = acceptNewTaskInfo.TaskId * 10;
                        acceptNewTaskInfo.Status = 1;
                        taskNotice.AcceptTasks.Add(acceptNewTaskInfo);


                        MessageHandler.Call(MessageDefine.SCAcceptTaskNotice, taskNotice);
                    }
                    #endregion
                    //SCTaskListResponse taskListRes = new SCTaskListResponse();
                    //TaskInfo taskInfo = new TaskInfo();
                    //taskInfo.TaskId = 6001;
                    //taskInfo.Status = 1;
                    //taskInfo.PlayerTaskId = taskInfo.TaskId * 10;
                    //taskListRes.TaskInfos.Add(taskInfo);
                    //MessageHandler.Call(MessageDefine.SCTaskListResponse, taskListRes);
                    break;
                case MessageDefine.CSChapterListRequest: //章节
                    #region
                    SCChapterListResponse chapterListRes = new SCChapterListResponse();
                    PlayerChapterInfo chapterInfo = new PlayerChapterInfo();
                    chapterInfo.PlayerChapterId = 1000;
                    chapterInfo.ChapterId = 1;
                    chapterInfo.Status = 1;
                    chapterListRes.Chapters.Add(chapterInfo);
                    MessageHandler.Call(MessageDefine.SCChapterListResponse, chapterListRes);
                    #endregion
                    break;
                case MessageDefine.CSEnterRequest:
                    #region 拼图
                    SCEnterResponse enterRes = new SCEnterResponse();
                    enterRes.Result = 0;
                    CSEnterRequest enterRequest = (CSEnterRequest)message;
                    long sceneId = enterRequest.SceneId;
                    int sceneType = (int)(sceneId / CommonData.C_SCENE_TYPE_ID);

                    int vitConsume = 0;
                    int seconds = 0;
                    for (int i = 0; i < enterRequest.OfficerIds.Count; i++)
                    {
                        long officerId = enterRequest.OfficerIds[i];
                        ConfOfficer officer = ConfOfficer.Get(officerId);
                        vitConsume += officer.vitConsume;
                        seconds += officer.secondGain;
                        enterRes.OfficerIds.Add(officerId);
                    }
                    //if (sceneType == CommonData.C_SEEK_SCENE_START_ID) //寻物
                    //{

                    //}
                    enterRes.Seconds = seconds;
                    enterRes.VitConsume = vitConsume;
                    if (sceneType == CommonData.C_JIGSAW_SCENE_START_ID) //拼图
                    {
                        enterRes.Seconds = 3600;
                    }
                    //else if (sceneType == CommonData.C_CARTOON_SCENE_START_ID) // 事件
                    //{

                    //}
                    MessageHandler.Call(MessageDefine.SCEnterResponse, enterRes);
                    #endregion
                    break;
                case MessageDefine.CSFinishRequest:
                    #region 拼图结束
                    CSFinishRequest finishRequest = (CSFinishRequest)message;

                    SCFinishResponse finishRes = new SCFinishResponse();
                    finishRes.Result = 0;
                    finishRes.SceneId = finishRequest.SceneId;
                    Reward reward = new Reward();
                    reward.Percent = 100;
                    reward.Type = 3;
                    reward.Num = 15;
                    finishRes.Rewards.Add(reward);
                    MessageHandler.Call(MessageDefine.SCFinishResponse, finishRes);
                    #endregion
                    break;
                case MessageDefine.CSSearchSceneSelectOfficerReq:
                    #region  选警员
                    CSSearchSceneSelectOfficerReq selectOfficerReq = (CSSearchSceneSelectOfficerReq)message;
                    int officerVitConsume = 0;
                    int officerCostSeconds = 300;
                    for (int i = 0; i < selectOfficerReq.OfficerIds.Count; i++)
                    {
                        long officerId = selectOfficerReq.OfficerIds[i];
                        ConfOfficer confOfficer = ConfOfficer.Get(officerId / 100);
                        officerVitConsume += confOfficer.vitConsume;
                        //officerCostSeconds += confOfficer.secondGain;
                    }

                    SCSearchSceneSelectOfficerResp selectOfficerReponse = new SCSearchSceneSelectOfficerResp();
                    selectOfficerReponse.VitConsume = officerVitConsume;
                    selectOfficerReponse.CostSeconds = officerCostSeconds;
                    MessageHandler.Call(MessageDefine.SCSearchSceneSelectOfficerResp, selectOfficerReponse);
                    #endregion
                    break;
                case MessageDefine.CSSceneEnterRequest:
                    {
                        #region 进入场景
                        CSSceneEnterRequest enterReq = (CSSceneEnterRequest)message;
                        SCSceneEnterResponse enterResponse = new SCSceneEnterResponse();
                        int enterVitConsume = 0;
                        int enterCostSeconds = 300;
                        for (int i = 0; i < enterReq.OfficerIds.Count; i++)
                        {
                            long officerId = enterReq.OfficerIds[i];
                            ConfOfficer confOfficer = ConfOfficer.Get(officerId / 100);
                            enterVitConsume += confOfficer.vitConsume;
                            //enterCostSeconds += confOfficer.secondGain;
                            enterResponse.OfficerIds.Add(officerId);
                        }
                        enterResponse.IsDropScene = false;

                        string exhibit = GuidNewNodeManager.Instance.GetCommonParams(GuidNewNodeManager.sceneExhibit);
                        string[] exhibitIdStr = exhibit.Split('|');
                        if (exhibitIdStr.Length > 0)
                        {
                            for (int i = 0; i < exhibitIdStr.Length; i++)
                            {
                                long exhibitId = long.Parse(exhibitIdStr[i]);
                                //enterResponse.SceneExhibits.Add(exhibitId);
                                enterResponse.TaskExhibits.Add(exhibitId);
                                enterResponse.VitConsume = enterVitConsume;
                                enterResponse.Seconds = enterCostSeconds;
                            }
                            MessageHandler.Call(MessageDefine.SCSceneEnterResponse, enterResponse);
                        }
                        #endregion
                    }
                    break;
                case MessageDefine.CSMarketItemRequest:
                    {
                        #region 请求时钟商店数据
                        CSMarketItemRequest marketItemReq = (CSMarketItemRequest)message;
                        SCMarketItemResponse marketItemRes = new SCMarketItemResponse();
                        MarketItemMsg marketmsg = new MarketItemMsg();
                        if (marketItemReq.PropId == 4)
                        {

                            marketmsg.Id = 304;
                            marketmsg.Cost = 0;
                            marketmsg.CostType = CostType.CostCash;
                        }
                        marketItemRes.MarketItems = marketmsg;
                        MessageHandler.Call(MessageDefine.SCMarketItemResponse, marketItemRes);
                        #endregion
                    }
                    break;
                case MessageDefine.MarkeBuyRequest:
                    {
                        #region 购买
                        MarkeBuyRequest buyReq = (MarkeBuyRequest)message;

                        MarkeBuyResponse buyRes = new MarkeBuyResponse();
                        PlayerPropMsg propmsg = new PlayerPropMsg();
                        if (buyReq.MarketItemId == 304)
                        {
                            //购买时钟
                            propmsg.PropId = 4;
                            propmsg.Count = 1;
                        }
                        buyRes.Props = propmsg;
                        MessageHandler.Call(MessageDefine.MarkeBuyResponse, buyRes);
                        #endregion
                    }
                    break;
                case MessageDefine.CSSkillTimerEmitReq:
                    {
                        #region 警员技能释放
                        SCSkillTimerEmitResp res = new SCSkillTimerEmitResp();
                        res.Result = 0;
                        MessageHandler.Call(MessageDefine.SCSkillTimerEmitResp, res);
                        #endregion
                    }
                    break;
                case MessageDefine.CSEventEnterRequest:
                    {
                        #region 进入事件
                        SCEventEnterResponse res = new SCEventEnterResponse();
                        res.Result = 0;
                        MessageHandler.Call(MessageDefine.SCEventEnterResponse, res);
                        #endregion
                    }
                    break;
                case MessageDefine.CSEventPhaseFeedbackRequest:
                    {
                        #region 请求事件
                        CSEventPhaseFeedbackRequest request = (CSEventPhaseFeedbackRequest)message;


                        SCEventPhaseFeedbackResponse res = new SCEventPhaseFeedbackResponse();
                        res.Valuation = true;
                        res.TotalScore = 200;
                        PhaseInfo phaseInfo = new PhaseInfo();
                        phaseInfo.PhaseId = (int)ConfEvent.Get(request.EventId).phases[0];
                        phaseInfo.PhaseTemplateId = (int)ConfEvent.Get(request.EventId).phases[0];
                        phaseInfo.OfficerTemplateId = 121;
                        phaseInfo.PhaseScore = 200;
                        res.PhaseInfos.Add(phaseInfo);
                        MessageHandler.Call(MessageDefine.SCEventPhaseFeedbackResponse, res);
                        #endregion
                    }
                    break;
                case MessageDefine.CSEventRewardRequest:
                    {
                        CSEventRewardRequest request = (CSEventRewardRequest)message;

                        SCEventRewardResponse res = new SCEventRewardResponse();
                        res.Score = 200;
                        res.Valuation = 2;
                        SceneRewardComprise sceneReward = new SceneRewardComprise();
                        ConfEvent confEvent = ConfEvent.Get(request.EventId);
                        sceneReward.OutputCash = confEvent.cashGain;
                        //sceneReward.OutputCoin = confEvent.coinGain;
                        //sceneReward.OutputVit = confEvent.vitGain;
                        //sceneReward.OutputExp = confEvent.expGain;
                        sceneReward.Type = 1;
                        res.SceneRewardComprise.Add(sceneReward);
                        MessageHandler.Call(MessageDefine.SCEventRewardResponse, res);
                    }
                    break;
            }
        }
    }
}
