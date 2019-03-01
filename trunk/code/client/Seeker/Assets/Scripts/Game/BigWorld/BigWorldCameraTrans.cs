using DG.Tweening;
using EngineCore;
using HedgehogTeam.EasyTouch;
using System;
using UnityEngine;

namespace SeekerGame
{
    //用于摄像机变换  动作
    public class BigWorldCameraTrans : MonoBehaviour
    {
        private Camera GameSceneCamera
        {
            get
            {
                return CameraManager.Instance.MainCamera;
            }
        }

        public void SetTarget(Transform tran, Action onComplete)
        {
            GameSceneCamera.transform.DORotate(tran.eulerAngles, 5f);
            Tweener tweener = GameSceneCamera.transform.DOMove(tran.position, 3f);
            tweener.OnComplete(delegate
            {
                if (onComplete != null)
                {
                    onComplete();
                }
            });
        }
    }
}
