using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SceneCameraParams_New : MonoBehaviour
{

    public float minAngleX_0;
    public float maxAngleX_0;
    public float minAngleY_0;
    public float maxAngleY_0;

    public float minAngleX_1;
    public float maxAngleX_1;
    public float minAngleY_1;
    public float maxAngleY_1;

    [HideInInspector]
    public float ZFar; //离地面最远的Z

    public float ZNear; //离地面最近的Z

    private float maxDistance;
    //public float ZFar { get { return zMax; } }

    void OnEnable()
    {
        ZFar = transform.position.z;
        this.CameraOriginPoint = transform.position;
        maxDistance = Mathf.Abs(ZFar - ZNear);
    }

    public Vector3 CameraOriginPoint
    {
        get;
        private set;
    }
    public float ZLerp { get { return Mathf.Clamp01(Mathf.Abs(transform.position.z - ZFar) / maxDistance); } }

    public float CurrentminAngleX { get { return Mathf.Lerp(minAngleX_0, minAngleX_1, ZLerp); } }
    public float CurrentmaxAngleX { get { return Mathf.Lerp(maxAngleX_0, maxAngleX_1, ZLerp); } }
    public float CurrentminAngleY { get { return Mathf.Lerp(minAngleY_0, minAngleY_1, ZLerp); } }
    public float CurrentmaxAngleY { get { return Mathf.Lerp(maxAngleY_0, maxAngleY_1, ZLerp); } }

}
