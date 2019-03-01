using System;
using System.Collections.Generic;

namespace SeekerGame
{
    public class GameMainTipSystem
    {
    }

    public class GameMainArrowTipData
    {
        public long taskId;
        public long propId;

        public GameMainArrowTipData(long taskId, long propId)
        {
            this.taskId = taskId;
            this.propId = propId;
        }
    }
}
