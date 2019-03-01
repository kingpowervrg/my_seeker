namespace SeekerGame
{
    public class NormalTask : TaskBase
    {
        public NormalTask(long taskID) : base(taskID) { }

        new public ConfTask TaskData
        {
            get { return this.m_taskData as ConfTask; }
            set
            {
                m_taskData = value;
            }
        }
    }
}