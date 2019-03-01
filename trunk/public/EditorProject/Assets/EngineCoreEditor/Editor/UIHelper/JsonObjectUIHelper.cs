#if UNITY_EDITOR
using fastJSON;
using System.Collections.Generic;
using System.IO;

namespace EngineCore.Editor
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class JsonObjectTypeAttribute : System.Attribute
    {
        readonly JsonObjectHelperTypes type;

        public JsonObjectTypeAttribute(JsonObjectHelperTypes type)
        {
            this.type = type;
        }

        public JsonObjectHelperTypes Type
        {
            get { return type; }
        }
    }
    //辅助类，用于辅助编辑器实现//
    public abstract class JsonObjectUIHelper
    {
        //是否需要先启动游戏//
        public abstract bool DoesNeedStarted();
        //搜索目录还是文件//
        public abstract bool SearchForDir();
        //搜索目录//
        public abstract string GetSearchDir();
        //扩展名//
        public abstract string GetFileExt();

        //new//
        public abstract bool CanNew();
        public abstract object OnNew();
        //save//
        public abstract bool CanSave();
        public virtual void OnSave(object si, string strFullName)
        {
            string str = JSON.ToJSON(si);
            FileStream fileStream = new FileStream(strFullName, FileMode.Create);
            StreamWriter writer = new StreamWriter(fileStream);
            writer.Write(str);
            writer.Flush();
            writer.Close();
            fileStream.Close();
        }
        //delete//
        public abstract bool CanDelete();
        public virtual bool CanMultiple { get { return false; } }
        public virtual string MultipleActionName { get { return null; } }
        public virtual void OnDelete(string strFullName) { }
        //select//
        public abstract object OnSelect(string strFullName);

        //从SI中枚举选项//
        public abstract Dictionary<string, string> EnumOptions(object target, string paramName);
        //展现除选择区/属性区/之外的Edit界面//
        public abstract void MakeEditUI(object target);
        public virtual void MakeMultipleActionUI()
        {

        }
        public virtual bool MultipleAction(string fileName, out string error)
        {
            error = null;
            return true;
        }
        public virtual void OnLoadConfig(Dictionary<string, string> dic)
        {

        }
        public virtual void OnSaveConfig(Dictionary<string, string> dic)
        {

        }

        public virtual bool IsDirty { get { return false; } }
        //帧更新。返回值表示是否要更新属性区显示//
        public virtual bool OnUpdate(object target) { return false; }
        //选择改变。//
        public virtual void OnSelectionChange(object target) { }

        public virtual int SortFiles(string lhs, string rhs) { return string.Compare(Path.GetFileName(lhs), Path.GetFileName(rhs)); }
    }
}
#endif