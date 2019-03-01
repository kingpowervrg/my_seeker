using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTDAttributeCallback : AndroidJavaProxy
{
    FtOnattributeChangedListener changedListener;
    public FTDAttributeCallback(FtOnattributeChangedListener changedListener) : base("com.ftdsdk.www.api.FTAttributionListener")
    {
        this.changedListener = changedListener;
    }
    void onAttributionChanged(string channel, string attribution)
    {
        if (this.changedListener != null)
        {
            this.changedListener.onAttributionChanged(attribution);
        }
    }

}
