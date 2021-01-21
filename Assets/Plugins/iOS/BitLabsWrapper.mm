#import <UIKit/UIKit.h>
#import <WebKit/WebKit.h>
#import <BitLabs/BitLabs-Swift.h>
#import "BitLabsWrapper.h"

extern void UnitySendMessage(const char * className,const char * methodName, const char * param);

@implementation BitLabsWrapper

extern "C" {
    BitLabs* _init(const char *token, const char *uid) {
        BitLabs *bitlabs = [BitLabs InitWithToken:[[NSString alloc] initWithCString:token encoding:NSUTF8StringEncoding] uid:[[NSString alloc] initWithCString:uid encoding:NSUTF8StringEncoding]];
        
        return bitlabs;
    }
}

@end
