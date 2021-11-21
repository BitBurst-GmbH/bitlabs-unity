#import <UIKit/UIKit.h>
#import <WebKit/WebKit.h>
#import <BitLabs/BitLabs-Swift.h>
#import "BitLabsWrapper.h"

extern void UnitySendMessage(const char * className,const char * methodName, const char * param);

@implementation BitLabsWrapper

BitLabs *bitlabs;

extern "C" {
    BitLabs* _init(const char *token, const char *uid) {
        bitlabs = [BitLabs InitWithToken:[[NSString alloc] initWithCString:token encoding:NSUTF8StringEncoding] uid:[[NSString alloc] initWithCString:uid encoding:NSUTF8StringEncoding]];
        
        return bitlabs;
    }
    
    void _show() {
        [bitlabs showWithParent:UnityGetGLViewController()];
    }
    
    void _appendTag(const char *key, const char *value) {
        [bitlabs appendTagWithKey: [[NSString alloc] initWithCString:key encoding:NSUTF8StringEncoding] value: [[NSString alloc] initWithCString:value encoding:NSUTF8StringEncoding]];
    }
}

@end
