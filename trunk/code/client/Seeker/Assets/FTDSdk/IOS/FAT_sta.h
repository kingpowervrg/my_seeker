//
//  FAT_sta.h
//  FAT_sta
//
//  Created by Tonyon 2018/8/15.
//  Copyright © 2018年 余超. All rights reserved.
//

#import <Foundation/Foundation.h>



NSString * const FAEnvironmentSandbox      = @"sandbox";
NSString * const FAEnvironmentProduction   = @"production";

/**
 * @protocol  FAT_staDelegate
 * @brief   SDK接口回调协议
 */
@protocol FAT_staDelegate <NSObject>
@required

/**
 * brief   渠道安装成功回调
 * attribution   回调数据
 * channel      具体渠道
 * deviceid     设备的id
 * return  无返回
 */
- (void)fat_installationSuccessfullyCallback:(NSDictionary *)attribution channel:(NSString *)channel deviceId:(NSString *)deviceid;

/**
 * brief   支付验证
 * return  无返回
 */
- (void)fat_PayResultCallBack:(id)response;


@end



/**
 SDK调用的方法
 */
@interface FAT_sta : NSObject

@property (nonatomic, assign) id <FAT_staDelegate>delegate;

@property (nonatomic, copy) NSString *AppID;

/**
 * @brief     初始化SDK信息
 * @return    FAT_SDK    生成的FAT_SDK对象实例
 */
+ (FAT_sta *)sharedInstance;

/**
 *  SDK初始化
 *
 *  @param   AppID      SDK后台赋予的appid
 *  @param    key       SDK后台赋予的key
 *  @param    way       SDK后台的加密方式
 */
- (void)fat_startWithAppID:(NSString *)AppID secretKey:(NSString *)key signWay:(NSString *)way;

/**
 * @brief     //用户自定义事件信息
 * @param    name    用户的name
 * @param    id      用户的id
 * @param    params       用户的params额外信息
 */
- (void)fat_getCustomEvent:(NSString *)name eventid:(NSString *)id params:(NSString *)params;


/**
 * @brief     //注册成功事件
 * @param      params    //同传参数
 */
- (void)fat_registeredWithChannel:(NSString *)channel registName:(NSString *)name params:(NSString *)params;

/**
 * @brief     //登录成功事件
 * @param      params    //同传参数
 */
- (void)fat_loginedWithChannel:(NSString *)channel loginName:(NSString *)name loginWay:(NSString *)way intime:(NSString *)intime outtime:(NSString *)outtime params:(NSString *)params;

/**
 * @brief     //是否完成新手引导
 * @param      params    //同传参数
 */
- (void)fat_newbieGuide:(NSString *)issuccess params:(NSString *)params;

/**
 *  SDK在线时长
 *
 *  @param   way      SDK进入或者出去的方式
 */
- (void)fat_getUserInAppWithWay:(NSString *)way;

/**
 *  //跟踪收入
 *
 *  @param    itemname         道具名称
 *  @param    itemid           收入定义的标识@“123456”
 *  @param    price            点击一个的单价  分制
 *  @param    currency         货币
 *  @param    usdPrice         usdPrice  apple后台注册的道具美分价格
 *  @param    channel          渠道 appsflyerSDK 还是adjustSDK
 *  @param    params           自定义参数
 */
- (void)fat_TrackRevenueByItemid:(NSString *)itemid itemname:(NSString *)itemname usdPrice:(NSString *)usdPrice unitPrice:(NSString *)price currency:(NSString *)currency channel:(NSString *)channel params:(NSString *)params;


/**
 *  //验证内购
 *
 *  @param    productIdentifier         product.productIdentifier
 *  @param    price                     product.price.stringValue 苹果返回的内购价格（苹果给多少就多少）
 *  @param    currency                  内购苹果返回的货币单位
 *  @param    tranactionId              trans.transactionIdentifier
 *  @param    itemname                  道具名称
 *  @param    itemid                    收入定义的标识@“123456”
 *  @param    usdPrice                  usdPrice  apple后台注册的道具美分价格
 *  @param    params                    额外参数
 */
- (void)fat_ValidateAndTrackInAppPurchase:(NSString *) productIdentifier
                                    price:(NSString *) price
                                 currency:(NSString *) currency
                            transactionId:(NSString *) tranactionId
                                   Itemid:(NSString *)itemid
                                 itemname:(NSString *)itemname
                                 usdPrice:(NSString *)usdPrice
                     additionalParameters:(NSString *) params;


/**
 *  //归因回传   appsflyer  Adjust  安装 事件会话 的回调 成功
 *
 *  @param    attribution                  安装归因回传的数据
 */

- (void)fat_getAttributionReturnFromChanneSuccess:(NSString *)attribution channel:(NSString *)channel;


/**
 *  app 上报的标签
 */
- (void)fat_getUserWithTags:(NSString *)tag;



/**
 *  app 上报用户实时资料，金币，关卡等等
 */
- (void)fat_getUserWithUserAttributes:(NSString *)attributes;

/**
 *  app adimpression 用户广告展示事件
 */
- (void)fat_getUserAdvertisingDisplay:(NSString *)ad_app ad_media_source:(NSString *)ad_media_source ad_campaign:(NSString *)ad_campaign ad_channel:(NSString *)ad_channel ad:(NSString *)ad params:(NSString *)params;


/**
 *  app adclick 用户广告点击事件
 */

- (void)fat_getUserADClick:(NSString *)ad_app ad_media_source:(NSString *)ad_media_source ad_campaign:(NSString *)ad_campaign ad_channel:(NSString *)ad_channel ad:(NSString *)ad params:(NSString *)params;


/**
 *  app tracklevelin  登录关卡事件
 */

- (void)fat_LoginLevels:(NSString *)level way:(NSString *)way intime:(NSString *)intime outtime:(NSString *)outtime  params:(NSString *)params;



/**
 *  app trackleveldone  完成关卡事件
 */

- (void)fat_completeLevel:(NSString *)level issuccess:(NSString *)issuccess params:(NSString *)params;


/**
 *  app trackpropuse 道具使用事件
 */

- (void)fat_propsToUse:(NSString *)propid propname:(NSString *)propname propnum:(NSString *)propnum params:(NSString *)params;

/**
 *  app trackpurchase 道具购买事件
 */

- (void)fat_propsToBuy:(NSString *)propid propname:(NSString *)propname propnum:(NSString *)propnum  coinid:(NSString *)coinid coinname:(NSString *)coinname costcoin:(NSString *)costcoin params:(NSString *)params;


/**
 *  app trackloading 完成加载事件
 */
- (void)fat_finishedLoading:(NSString *)issuccess params:(NSString *)params;

@end
