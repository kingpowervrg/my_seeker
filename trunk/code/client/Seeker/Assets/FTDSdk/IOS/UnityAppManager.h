//
//  UnityAppManager.h
//  fotoabl_test
//
//  Created by fotoabl on 2018/7/17.
//  Copyright © 2018年 fotoabl. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "FAT_sta.h"


static const char * UNITY_SENDMESSAGE_CALLBACK = "FTDSdk";


static const char * UNITY_SENDMESSAGE_CALLBACK_INSTALL = "onAttributeCallback";

static const char * UNITY_PAY_CALLBACK = "onPayResultCallback";

@interface UnityAppManager : NSObject<FAT_staDelegate>


@end
