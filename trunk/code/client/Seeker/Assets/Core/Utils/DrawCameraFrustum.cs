/********************************************************************
	created:  2018-7-2 16:23:0
	filename: DrawCameraFrustum.cs
	author:	  songguangze@outlook.com
	
	purpose:  Gizmos绘制摄像机锥体
*********************************************************************/
using UnityEngine;

public class DrawCameraFrustum : MonoBehaviour
{
    public Camera TargetCamera;
    public Color DrawFrustumColor = new Color(0, 0, 255, 0.8f);

    private void OnEnable()
    {
        TargetCamera = TargetCamera ?? Camera.main;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = DrawFrustumColor;
        Gizmos.matrix = TargetCamera.transform.localToWorldMatrix;

        //Gizmos的TRS直接用一个Matrix4x4表示,所有绘制使用WorldSpace
        Gizmos.DrawFrustum(Vector3.zero, TargetCamera.fieldOfView, TargetCamera.farClipPlane, TargetCamera.nearClipPlane, TargetCamera.aspect);
    }





























    //void DrawFrustum(Camera cam)
    //{
    //    Vector3[] nearCorners = new Vector3[4]; //Approx'd nearplane corners
    //    Vector3[] farCorners = new Vector3[4]; //Approx'd farplane corners
    //    Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes(cam); //get planes from matrix
    //    Plane temp = camPlanes[1]; camPlanes[1] = camPlanes[2]; camPlanes[2] = temp; //swap [1] and [2] so the order is better for the loop

    //    for (int i = 0; i < 4; i++)
    //    {
    //        nearCorners[i] = Plane3Intersect(camPlanes[4], camPlanes[i], camPlanes[(i + 1) % 4]); //near corners on the created projection matrix
    //        farCorners[i] = Plane3Intersect(camPlanes[5], camPlanes[i], camPlanes[(i + 1) % 4]); //far corners on the created projection matrix
    //    }

    //    for (int i = 0; i < 4; i++)
    //    {
    //        Debug.DrawLine(nearCorners[i], nearCorners[(i + 1) % 4], Color.red, Time.deltaTime, true); //near corners on the created projection matrix
    //        Debug.DrawLine(farCorners[i], farCorners[(i + 1) % 4], Color.blue, Time.deltaTime, true); //far corners on the created projection matrix
    //        Debug.DrawLine(nearCorners[i], farCorners[i], Color.green, Time.deltaTime, true); //sides of the created projection matrix
    //    }
    //}

    //Vector3 Plane3Intersect(Plane p1, Plane p2, Plane p3)
    //{ //get the intersection point of 3 planes
    //    return ((-p1.distance * Vector3.Cross(p2.normal, p3.normal)) +
    //            (-p2.distance * Vector3.Cross(p3.normal, p1.normal)) +
    //            (-p3.distance * Vector3.Cross(p1.normal, p2.normal))) /
    //        (Vector3.Dot(p1.normal, Vector3.Cross(p2.normal, p3.normal)));
    //}
}
