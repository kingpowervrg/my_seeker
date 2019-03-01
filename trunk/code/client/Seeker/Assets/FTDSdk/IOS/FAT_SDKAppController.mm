//
//  FAT_SDKAppController
//
//
//

#import "UnityAppController.h"
#import "FATDelegate.h"
#import "FAT_sta.h"


@interface FAT_SDKAppController : UnityAppController //<AppDelegateListener>
{
    BOOL didEnteredBackGround;
}
@end

@implementation FAT_SDKAppController

- (instancetype)init
{
    NSLog(@"initializing FAT_SDKAppController");
    self = [super init];
	if (self) {
//        UnityRegisterAppDelegateListener(self);
	}
    return self;
}

//- (void)applicationWillResignActive:(UIApplication *)application {
//    NSLog(@"应用程序将要进入非活动状态，即将进入后台");
//}
//
//
//- (void)applicationDidEnterBackground:(UIApplication *)application {
//    NSLog(@"如果应用程序支持后台运行，则应用程序已经进入后台运行");
//    [[FAT_SDK sharedInstance] fat_getUserInAppWithWay:@"out"];
//}
//
//
//- (void)applicationWillEnterForeground:(UIApplication *)application {
//    NSLog(@"应用程序将要进入活动状态，即将进入前台运行");
//
//}
//
//- (void)applicationDidBecomeActive:(UIApplication *)application {
//    [[AppsFlyerTracker sharedTracker] trackAppLaunch];
//
//    NSLog(@"应用程序已进入前台，处于活动状态");
//    [[FAT_SDK sharedInstance] fat_getUserInAppWithWay:@"in"];
//}
//
//// Reports app open from a Universal Link for iOS 9 or above
//- (BOOL) application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void (^)(NSArray *_Nullable))restorationHandler {
//    //    [[AppsFlyerTracker sharedTracker] continueUserActivity:userActivity restorationHandler:restorationHandler];
//    return YES;
//}
//
//// Reports app open from deep link from apps which do not support Universal Links (Twitter) and for iOS8 and below
//- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation {
//    //    [[AppsFlyerTracker sharedTracker] handleOpenURL:url sourceApplication:sourceApplication withAnnotation:annotation];
//    return YES;
//}
//// Reports app open from deep link for iOS 10
//- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url
//            options:(NSDictionary *) options {
//    //    [[AppsFlyerTracker sharedTracker] handleOpenUrl:url options:options];
//    return YES;
//}
//
//- (void)applicationWillTerminate:(UIApplication *)application {
//
//    NSLog(@"应用程序将要退出，通常用于保存数据和一些退出前的清理工作");
//    [[FAT_SDK sharedInstance] fat_getUserInAppWithWay:@"out"];
//}





@end

IMPL_APP_CONTROLLER_SUBCLASS(FAT_SDKAppController)
