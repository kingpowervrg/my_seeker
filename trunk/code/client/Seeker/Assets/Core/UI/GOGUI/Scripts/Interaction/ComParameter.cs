using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


namespace GOGUI
{

    public class ComParameter : MonoBehaviour
    {
        public object parameter;
        protected GameObject go;
        static public ComParameter Get(GameObject go)
        {
            ComParameter listener = go.GetComponent<ComParameter>();
            if (listener == null) listener = go.AddComponent<ComParameter>();
            listener.go = go;
            return listener;
        }
    }
}
