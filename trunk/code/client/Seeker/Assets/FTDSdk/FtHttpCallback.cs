using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface FtHttpCallback
{

    void onReponse(int code, string request, string mResponse);
}
