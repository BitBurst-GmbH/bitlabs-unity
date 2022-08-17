#import <UIKit/UIKit.h>
#import <WebKit/WebKit.h>
#import <BitLabs/BitLabs-Swift.h>
#import "BitLabsWrapper.h"

extern void UnitySendMessage(const char * gameObjectName, const char * methodName, const char * methodParam);

@implementation BitLabsWrapper

BitLabs *bitlabs;

extern "C" {
    BitLabs* _init(const char *token, const char *uid) {
        bitlabs = [BitLabs shared];
        [bitlabs configureWithToken:[[NSString alloc] initWithCString:token encoding:NSUTF8StringEncoding]
         uid:[[NSString alloc] initWithCString:uid encoding:NSUTF8StringEncoding]];
        
        return bitlabs;
    }

    void _setTags(NSDictionary *tags) {
        [bitlabs setTags:tags];
    }

    void _addTag(const char *key, const char *value) {
        [bitlabs addTagWithKey: [[NSString alloc] initWithCString:key encoding:NSUTF8StringEncoding] 
        value: [[NSString alloc] initWithCString:value encoding:NSUTF8StringEncoding]];
    }

    void _checkSurveys(const char *gameObject) {
        NSString *name = [NSString stringWithUTF8String:gameObject];
        [bitlabs checkSurveys:^(bool hasSurveys) {
            const char *hasSurveysStr = [@(hasSurveys).stringValue UTF8String];
            UnitySendMessage([name UTF8String], "CheckSurveysCallback", hasSurveysStr);
        }];
    }

    void _getSurveys(const char *gameObject) {
        NSString *name = [NSString stringWithUTF8String:gameObject];
        [bitlabs getSurveys:^(NSArray<Survey*> *surveys) {
            NSMutableArray *array = [NSMutableArray new];
            for(Survey* survey in surveys) {
                [array addObject:[survey asDictionary]];
            }
            NSData* data = [NSJSONSerialization dataWithJSONObject:array options:NSJSONWritingPrettyPrinted error:nil];
            NSString *surveysStr = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
            
            const char *jsonStr = [surveysStr UTF8String];
            UnitySendMessage([name UTF8String], "GetSurveysCallback", jsonStr);
        }];
    }

    void _setRewardCompletionHandler(const char *gameObject) {
        NSString *name = [NSString stringWithUTF8String:gameObject];
        [bitlabs setRewardCompletionHandler:^(float payout) {
            const char *payoutStr = [@(payout).stringValue UTF8String];
            UnitySendMessage([name UTF8String], "RewardCallback", payoutStr);
        }];
    }
    
    void _launchOfferWall() {
        [bitlabs launchOfferWallWithParent: UnityGetGLViewController()];
    }
    
    void _requestTrackingAuthorization() {
        [bitlabs requestTrackingAuthorization];
    }
}

@end
