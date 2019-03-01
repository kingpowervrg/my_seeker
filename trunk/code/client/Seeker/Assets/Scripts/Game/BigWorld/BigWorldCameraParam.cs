using UnityEngine;
using EngineCore.Utility;
namespace SeekerGame
{
    public class BigWorldCameraParam : MonoBehaviour
    {

        public GameObject[] boxCollider;

        public Vector3 m_zuoPoint;
        public Vector3 m_zuoNormal;

        public Vector3 m_youPoint;
        public Vector3 m_youNormal;

        void Start()
        {
            m_zuoPoint = boxCollider[3].transform.position + boxCollider[3].transform.right * 100;
            m_zuoNormal = -boxCollider[3].transform.forward;

            m_youPoint = boxCollider[2].transform.position + boxCollider[2].transform.right * 100;
            m_youNormal = -boxCollider[2].transform.forward;
        }

        public int IsOutBounds(Transform trans)
        {
            for (int i = 0; i < boxCollider.Length; i++)
            {
                Vector3 transDir = trans.position - boxCollider[i].transform.position;
                float dot = Vector3.Dot(transDir, boxCollider[i].transform.forward);
                if (dot <= 0)
                {
                    return i;
                }
            }
            return -1;
        }

    }
}
