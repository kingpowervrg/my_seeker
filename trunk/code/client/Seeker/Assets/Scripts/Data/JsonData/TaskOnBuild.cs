using System.Collections.Generic;

public class TaskOnBuild
{
    public long TaskID { get; set; }
    public string Name { get; set; }

    public List<TaskOnBuild> TaskOnBuilds;

    public override string ToString()
    {
        return string.Format("TaskID:{0},Name:{1}", TaskID, Name);
    }
}