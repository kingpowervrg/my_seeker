//
//  UnityAppManager.m
//  fotoabl_test
//
//  Created by fotoabl on 2018/7/17.
//  Copyright © 2018年 fotoabl. All rights reserved.
//

#import "UnityAppManager.h"
#import "FAT_sta.h"


//固定代码
#if defined(__cplusplus)
extern "C"{
#endif
        extern void UnitySendMessage(const char *, const char *, const char *);
    extern NSString* _CreateNSString (const char* string);
#if defined(__cplusplus)
}
#endif

//static FATDelegate *mFATSDKdelegate;

static id object;

@interface UnityAppManager ()<FAT_staDelegate>


//+(FAT_SDKDelegate *) getFAT_SDKDelegate;

@end

@implementation UnityAppManager





- (void)fat_installationSuccessfullyCallback:(NSDictionary *)attribution channel:(NSString *)channel deviceId:(NSString *)deviceid{
    
    NSDictionary * dict = @{@"attribution" : attribution,@"channel": channel ,@"deviceid":deviceid};
    
    NSLog(@"获取归因数据 ---> unity 回调参数是  --- - 设备id --- %@ ",[self getJsonStringFromDictionary:dict]);
    
    UnitySendMessage(UNITY_SENDMESSAGE_CALLBACK, UNITY_SENDMESSAGE_CALLBACK_INSTALL, [[self getJsonStringFromDictionary:dict] UTF8String]);
    
}

- (void)fat_PayResultCallBack:(id)response{
    
     NSLog(@"内购验证结果 ---> unity 回调参数是  %@ ",response);
    
    UnitySendMessage(UNITY_SENDMESSAGE_CALLBACK, UNITY_PAY_CALLBACK, [response UTF8String]);
    
}


- (NSString *) getJsonStringFromDictionary:(NSDictionary *)dict {
    NSError *error;
    
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:NSJSONWritingPrettyPrinted error:&error];
    
    NSString *jsonString;
    
    if (!jsonData) {
        
        NSLog(@"%@",error);
        
    }else{
        
        jsonString = [[NSString alloc]initWithData:jsonData encoding:NSUTF8StringEncoding];
        
    }
    
    NSMutableString *mutStr = [NSMutableString stringWithString:jsonString];
    
    NSRange range = {0,jsonString.length};
    
    //去掉字符串中的空格
    
    [mutStr replaceOccurrencesOfString:@" " withString:@"" options:NSLiteralSearch range:range];
    
    NSRange range2 = {0,mutStr.length};
    
    //去掉字符串中的换行符
    
    [mutStr replaceOccurrencesOfString:@"\n" withString:@"" options:NSLiteralSearch range:range2];
    
    
    return mutStr;
}


- (void)abc{
    
    object = self;
}


#if defined(__cplusplus)
extern "C"{
#endif
    
    //字符串转化的工具函数
    
    NSString* _CreateNSString (const char* string)
    {
        if (string)
            return [NSString stringWithUTF8String: string];
        else
            return [NSString stringWithUTF8String: ""];
    }
    
    char* _MakeStringCopy( const char* string)
    {
        if (NULL == string) {
            return NULL;
        }
        char* res = (char*)malloc(strlen(string)+1);
        strcpy(res, string);
        return res;
    }
    
    
    static UnityAppManager *myManager;
    
    //供u3d调用的c函数 ( 因测试的sdk调用初始化为不传参方法，如果调用的sdk需要传参调用，此处应相应修改为含参数方法)    SDK初始化
    
    
    void fInitAppVestAndStartIt(const char *appid,  const char *appkey, const char *way ,const char *environment)
    {
        if(myManager==NULL)
        {
            myManager = [[UnityAppManager alloc]init];
        }
        [[UnityAppManager alloc] abc];
        [[FAT_sta sharedInstance] fat_startWithAppID:[NSString stringWithUTF8String:appid] secretKey:[NSString stringWithUTF8String:appkey] signWay:[NSString stringWithUTF8String:way]];
        [FAT_sta sharedInstance].delegate = object;
        
    }
    //注册成功
    void fRegisteredSuccese(const char *registChannel,const char *name,  const char *customParams){
        
        [[FAT_sta sharedInstance] fat_registeredWithChannel:[NSString stringWithUTF8String:registChannel] registName:[NSString stringWithUTF8String:name] params:[NSString stringWithUTF8String:customParams]];
    }
    
    //登录成功
    
    void fLoginedSuccese(const char *loginChannel,const char *name,const char *way,const char *intime,const char *outtime, const char *customParams){
        
    [[FAT_sta sharedInstance] fat_loginedWithChannel:[NSString stringWithUTF8String:loginChannel] loginName:[NSString stringWithUTF8String:name] loginWay:[NSString stringWithUTF8String:way] intime:[NSString stringWithUTF8String:intime] outtime:[NSString stringWithUTF8String:outtime] params:[NSString stringWithUTF8String:customParams]];    }
    
    
    //新手引导成功
    void fNewbieGuideSuccese(const char *isSuccess,  const char *customParams){
        
        [[FAT_sta sharedInstance] fat_newbieGuide:[NSString stringWithUTF8String:isSuccess] params:[NSString stringWithUTF8String:customParams]];
        
    }
    
    //支付成功
    void fTrackRevenueSuccese(const char *itemid,  const char *itemName ,const char *usdprice , const char *price,  const char *currency , const char *channel ,const char *customParams){
        
        [[FAT_sta sharedInstance] fat_TrackRevenueByItemid:[NSString stringWithUTF8String:itemid] itemname:[NSString stringWithUTF8String:itemName] usdPrice:[NSString stringWithUTF8String: usdprice] unitPrice:[NSString stringWithUTF8String:price] currency:[NSString stringWithUTF8String:currency] channel:[NSString stringWithUTF8String:channel] params:[NSString stringWithUTF8String:customParams]];
        
    }
    //用户自定义事件
    void fGetCustomEvent(const char *eventName ,const char *eventId,  const char *customParams){
        
        [[FAT_sta sharedInstance] fat_getCustomEvent:[NSString stringWithUTF8String:eventName] eventid:[NSString stringWithUTF8String:eventId] params:[NSString stringWithUTF8String:customParams]];
    }
    //上报归因数据
    void fGetAttributionReturnFromChannel(const char *jsonAttr ,const char *channel){
        [[FAT_sta sharedInstance] fat_getAttributionReturnFromChanneSuccess:[NSString stringWithUTF8String:jsonAttr] channel:[NSString stringWithUTF8String:channel]];
        
    }
    
    //在线时间的确定
    void fGetUserInAppWithWay(const char *way){
        
       [ [FAT_sta sharedInstance] fat_getUserInAppWithWay:[NSString stringWithUTF8String:way]];
    }
    
    //上报的标签
    void fGetUserWithTags(const char *tag){
        
        [[FAT_sta sharedInstance] fat_getUserWithTags:[NSString stringWithUTF8String:tag]];
    }
    
    
    //上报用户实时资料，金币，关卡等等
    void fGetUserWithUserAttributes(const char *attributes){
        
        [[FAT_sta sharedInstance] fat_getUserWithUserAttributes:[NSString stringWithUTF8String:attributes]];
    }
    
    // app adimpression 用户广告展示事件
    void fGetUserAdvertisingDisplay(const char *ad_app, const char *ad_media_source, const char *ad_campaign, const char *ad_channel, const char *ad,const char *params){
        
        [[FAT_sta sharedInstance] fat_getUserAdvertisingDisplay:[NSString stringWithUTF8String:ad_app] ad_media_source:[NSString stringWithUTF8String:ad_media_source] ad_campaign:[NSString stringWithUTF8String:ad_campaign] ad_channel:[NSString stringWithUTF8String:ad_channel] ad:[NSString stringWithUTF8String:ad] params:[NSString stringWithUTF8String:params]];
    }
    
    // app adclick 用户广告点击事件
    void fGetUserADClick(const char *ad_app, const char *ad_media_source, const char *ad_campaign, const char *ad_channel, const char *ad,const char *params){
        
        [[FAT_sta sharedInstance] fat_getUserADClick:[NSString stringWithUTF8String:ad_app] ad_media_source:[NSString stringWithUTF8String:ad_media_source] ad_campaign:[NSString stringWithUTF8String:ad_campaign] ad_channel:[NSString stringWithUTF8String:ad_channel] ad:[NSString stringWithUTF8String:ad] params:[NSString stringWithUTF8String:params]];
    }
    
    //内购验证
    void fValidateAndTrackInAppPurchase(const char *productIdentifier,const char *price,const char *currency,const char *tranactionId,const char *itemid,const char *itemname,const char *usdPrice ,const char *params){
        
        [[FAT_sta sharedInstance] fat_ValidateAndTrackInAppPurchase:[NSString stringWithUTF8String:productIdentifier] price:[NSString stringWithUTF8String:price] currency:[NSString stringWithUTF8String:currency] transactionId:[NSString stringWithUTF8String:tranactionId] Itemid:[NSString stringWithUTF8String:itemid] itemname:[NSString stringWithUTF8String:itemname] usdPrice:[NSString stringWithUTF8String:usdPrice] additionalParameters:[NSString stringWithUTF8String:params]];
    }
    
    
    //  登录关卡事件
    void fLoginLevels(const char *level,const char *way,const char *intime,const char *outtime, const char *params){
        
        [[FAT_sta sharedInstance] fat_LoginLevels:[NSString stringWithUTF8String:level] way:[NSString stringWithUTF8String:way] intime:[NSString stringWithUTF8String:intime] outtime:[NSString stringWithUTF8String:outtime]  params:[NSString stringWithUTF8String:params]];
    }
    
    
    // 完成关卡事件
    void fCompleteLevel(const char *level,const char *issuccess,const char *params){
        
        [[FAT_sta sharedInstance] fat_completeLevel:[NSString stringWithUTF8String:level] issuccess:[NSString stringWithUTF8String:issuccess] params:[NSString stringWithUTF8String:params]];
    }
    
    // 道具使用事件
    void fPropsToUse(const char *propid,const char *propname,const char *propnum,const char *params){
        
        [[FAT_sta sharedInstance] fat_propsToUse:[NSString stringWithUTF8String:propid] propname:[NSString stringWithUTF8String:propname] propnum:[NSString stringWithUTF8String:propnum] params:[NSString stringWithUTF8String:params]];
    }
    
    // 道具购买事件
    void fPropsToBuy(const char *propid,const char *propname,const char *propnum,const char *coinid,const char *coinname,const char *costcoin,const char *params){
        
        [[FAT_sta sharedInstance] fat_propsToBuy:[NSString stringWithUTF8String:propid] propname:[NSString stringWithUTF8String:propname] propnum:[NSString stringWithUTF8String:propnum] coinid:[NSString stringWithUTF8String:coinid] coinname:[NSString stringWithUTF8String:coinname] costcoin:[NSString stringWithUTF8String:costcoin] params:[NSString stringWithUTF8String:params]];
    }
    
    // 完成加载事件
    void fFinishedLoading(const char *issuccess,const char *params){
        
        [[FAT_sta sharedInstance] fat_finishedLoading:[NSString stringWithUTF8String:issuccess] params:[NSString stringWithUTF8String:params]];
    }
    
    
    
#if defined(__cplusplus)
}
#endif



@end
