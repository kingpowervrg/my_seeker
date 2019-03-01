/********************************************************************
	created:  2018-5-15 13:59:1
	filename: TaskCompleteItems.cs
	author:	  songguangze@outlook.com
	
	purpose:  任务系统-收集物品完成条件
*********************************************************************/
using System.Collections.Generic;

namespace SeekerGame
{
    [TaskComplete(TaskCompleteMode.CompletedByItem)]
    public class TaskCompleteItems : TaskCompleteCondition
    {
        private Dictionary<long, ItemWrapper> m_taskCollectItemDict = new Dictionary<long, ItemWrapper>();

        public TaskCompleteItems(object param) : base(param)
        {
            long[] itemIds = (param as List<long[]>)[0];
            long[] itemNums = (param as List<long[]>)[1];

            this.m_taskCompleteData = new List<ItemWrapper>();
            for (int i = 0; i < itemIds.Length; ++i)
            {
                ItemWrapper item = new ItemWrapper() { ItemID = itemIds[i], ItemNum = (int)itemNums[i] };
                TaskCompleteData.Add(item);

                m_taskCollectItemDict.Add(item.ItemID, item);
            }
        }

        public override void SetCompleteProgressData(object progressData)
        {
            if (progressData is IList<TaskPropProgress>)
            {
                IList<TaskPropProgress> itemCollectProgress = progressData as IList<TaskPropProgress>;
                float collectNum = 0;
                for (int i = 0; i < itemCollectProgress.Count; ++i)
                {
                    ItemWrapper collectionItem = this.m_taskCollectItemDict[itemCollectProgress[i].PropId];
                    collectionItem.CurrentItemNum = itemCollectProgress[i].Count;
                    if (collectionItem.ItemNum <= collectionItem.CurrentItemNum)
                        collectNum += 1;
                }
                this.m_taskCompletedConditionProgress = collectNum / TaskCompleteData.Count;
            }

            if (progressData is IList<TaskExhibitProgress>)
            {
                IList<TaskExhibitProgress> exhibitItemCollectProgress = progressData as IList<TaskExhibitProgress>;
                for (int i = 0; i < exhibitItemCollectProgress.Count; ++i)
                {
                    ItemWrapper collectionItem = this.m_taskCollectItemDict[exhibitItemCollectProgress[i].ExhibitId];
                    collectionItem.CurrentItemNum = exhibitItemCollectProgress[i].Count;
                }
            }

        }

        new public List<ItemWrapper> TaskCompleteData => this.m_taskCompleteData as List<ItemWrapper>;


    }
}
