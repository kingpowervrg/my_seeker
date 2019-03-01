//
//  FATDelegate
//  
//
//  Created by Golan on 6/21/15.
//  Edited by Shachar on 5/18/17.
//
//

#import "FATDelegate.h"
#import "UnityAppController.h"


//固定代码
#if defined(__cplusplus)
extern "C"{
#endif
//    extern void UnitySendMessage(const char *, const char *, const char *, const char *);
    extern NSString* _CreateNSString (const char* string);
#if defined(__cplusplus)
}
#endif

@interface FATDelegate() {
    BOOL didEnteredBackGround;
}

@end

@implementation FATDelegate

- (instancetype)init
{
    NSLog(@"initializing FATDelegate");
    self = [super init];
    if (self) {
    }
    return self;
}




- (NSString *) getJsonStringFromDictionary:(NSDictionary *)dict {
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict
                                                       options:0
                                                         error:&error];
    
    if (!jsonData) {
        NSLog(@"JSON error: %@", error);
        return nil;
    } else {
        
        NSString *JSONString = [[NSString alloc] initWithBytes:[jsonData bytes] length:[jsonData length] encoding:NSUTF8StringEncoding];
        NSLog(@"JSON OUTPUT: %@", JSONString);
        return JSONString;
    }
}


@end
