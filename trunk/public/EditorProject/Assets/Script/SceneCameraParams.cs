/********************************************************************
	created:  2018-4-23 17:21:6
	filename: SceneCameraParams.cs
	author:	  songguangze@outlook.com
	
	purpose:  场景摄像机参数
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
//[RequireComponent(typeof(Camera))]
public class SceneCameraParams : MonoBehaviour
{
    public float ZNear = 0;

    public GameObject SceneLeftEdgeObject;
    public GameObject SceneRightEdgeObject;
    public GameObject SceneUpEdgeObject;
    public GameObject SceneBottonEdgeObject;
    public Transform SceneCenterTransform;

    private BoxCollider m_cachedSceneLeftCollider = null;
    private BoxCollider m_cachedSceneRightCollider = null;
    private BoxCollider m_cachedSceneUpCollider = null;
    private BoxCollider m_cachedSceneDownCollider = null;


    public Bounds SceneLeftBounds
    {
        get
        {
            if (!this.m_cachedSceneLeftCollider && SceneLeftEdgeObject)
                this.m_cachedSceneLeftCollider = SceneLeftEdgeObject.GetComponent<BoxCollider>();

            return this.m_cachedSceneLeftCollider.bounds;
        }
    }

    public Bounds SceneRightBounds
    {
        get
        {
            if (!this.m_cachedSceneRightCollider && SceneRightEdgeObject)
                this.m_cachedSceneRightCollider = SceneRightEdgeObject.GetComponent<BoxCollider>();

            return this.m_cachedSceneRightCollider.bounds;
        }
    }

    public Bounds SceneUpBounds
    {
        get
        {
            if (!this.m_cachedSceneUpCollider && SceneUpEdgeObject)
                this.m_cachedSceneUpCollider = SceneUpEdgeObject.GetComponent<BoxCollider>();

            return this.m_cachedSceneUpCollider.bounds;
        }
    }

    public Bounds SceneDownBounds
    {
        get
        {
            if (!this.m_cachedSceneDownCollider && SceneBottonEdgeObject)
                this.m_cachedSceneDownCollider = SceneBottonEdgeObject.GetComponent<BoxCollider>();

            return this.m_cachedSceneDownCollider.bounds;
        }

    }

    private void OnEnable()
    {
        CameraOriginPoint = transform.position;

        if (SceneCenterTransform != null)
        {
            transform.LookAt(SceneCenterTransform);
            CameraOriginDirection = transform.forward;
            CameraOriginRotation = transform.rotation;
        }


        float factor = (ZNear - ZFar) / transform.forward.z;
        CameraNearsetPosition = CameraOriginPoint + transform.forward * factor;
    }

    public float ZFar
    {
        get { return CameraOriginPoint.z; }
    }

    public Vector3 CameraOriginPoint
    {
        get;
        private set;
    }

    public Vector3 CameraNearsetPosition
    {
        get;
        private set;
    }

    public Vector3 CameraOriginDirection
    {
        get;
        private set;
    }

    public Quaternion CameraOriginRotation
    {
        get;
        private set;
    }
}