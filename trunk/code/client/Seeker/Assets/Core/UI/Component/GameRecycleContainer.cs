using System.Collections.Generic;

namespace EngineCore
{
    public class GameRecycleContainer : GameUIContainer
    {
        //每次自动分配的数量
        public static int AllocPertime = 2;

        //可用对象队列
        private Queue<GameUIComponent> m_avaliableElementQueue = new Queue<GameUIComponent>();

        public override void EnsureSize<T>(int count)
        {
            base.EnsureSize<T>(count);

            for (int i = 0; i < ChildCount; ++i)
            {
                GameUIComponent childElement = GetChild<GameUIComponent>(i);
                if (!childElement.Visible)
                    m_avaliableElementQueue.Enqueue(childElement);
            }
        }

        /// <summary>
        /// 获取可用容器对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetAvaliableContainerElement<T>() where T : GameUIComponent, new()
        {
            if (m_avaliableElementQueue.Count == 0)
            {
                for (int i = 0; i < AllocPertime; ++i)
                {
                    GameUIComponent childElement = AddChild<T>();
                    m_avaliableElementQueue.Enqueue(childElement);
                }
            }

            T avaliableElement = m_avaliableElementQueue.Dequeue() as T;

            return avaliableElement;
        }

        /// <summary>
        /// 获取可用容器对象
        /// </summary>
        /// <returns></returns>
        public GameUIComponent GetAvaliableContainerElement()
        {
            return GetAvaliableContainerElement<GameUIComponent>();
        }

        /// <summary>
        /// 回收容器对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="containerTemplateInstance"></param>
        public void RecycleElement<T>(T containerTemplateInstance) where T : GameUIComponent, new()
        {
            containerTemplateInstance.Visible = false;
            this.m_avaliableElementQueue.Enqueue(containerTemplateInstance);
        }

    }
}