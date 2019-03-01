using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTDHttpCallback : AndroidJavaProxy
{
    FtHttpCallback ftHttpCallback;
    public FTDHttpCallback(FtHttpCallback ftHttpCallback) : base("com.ftdsdk.www.http.base.FTHttpCallBack")
    {
        this.ftHttpCallback = ftHttpCallback;
    }
    void onResponse(int code, AndroidJavaObject request, AndroidJavaObject mResponse, AndroidJavaObject tag)
    {
        if (ftHttpCallback != null)
        {
            string mRequest = request.Call<string>("toString");
            string response = mResponse.Call<string>("toString");
            ftHttpCallback.onReponse(code, mRequest, response);
        }
    }

}
