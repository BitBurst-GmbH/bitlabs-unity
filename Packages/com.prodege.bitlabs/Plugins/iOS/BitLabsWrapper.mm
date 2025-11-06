#import <UIKit/UIKit.h>
#import <WebKit/WebKit.h>
#import <UnityFramework/UnityFramework-Swift.h>
#import "BitLabsWrapper.h"

extern void UnitySendMessage(const char * gameObjectName, const char * methodName, const char * methodParam);

@implementation BitLabsWrapper

// Typedef for C# callback function pointers
typedef void (*InitSuccessCallback)();
typedef void (*ErrorCallback)(const char* error);
typedef void (*BooleanResponseCallback)(bool response);
typedef void (*StringResponseCallback)(const char* response);
typedef void (*RewardCallback)(double reward);

extern "C" {
    void _init(const char *token, const char *uid, InitSuccessCallback onSuccess, ErrorCallback onError) {
        NSString *tokenStr = [[NSString alloc] initWithCString:token encoding:NSUTF8StringEncoding];
        NSString *uidStr = [[NSString alloc] initWithCString:uid encoding:NSUTF8StringEncoding];

        [BitLabsBridge initializeWithToken:tokenStr
                                       uid:uidStr
                                 onSuccess:^{
                                     if (onSuccess != nil) {
                                         onSuccess();
                                     }
                                 }
                                   onError:^(NSString * error) {
                                     if (onError != nil) {
                                         onError([error UTF8String]);
                                     }
                                 }];
    }

    void _setIsDebugMode(bool isDebug) {
        [BitLabsBridge setDebugMode:isDebug];
    }

    void _setTags(NSDictionary *tags) {
        [BitLabsBridge setTags:tags];
    }

    void _addTag(const char *key, const char *value) {
        [BitLabsBridge addTagWithKey:[[NSString alloc] initWithCString:key encoding:NSUTF8StringEncoding]
                               value:[[NSString alloc] initWithCString:value encoding:NSUTF8StringEncoding]];
    }

    void _checkSurveys(BooleanResponseCallback onSuccess, ErrorCallback onError) {
        [BitLabsBridge checkSurveysOnSuccess:^(BOOL hasSurveys) {
            if (onSuccess != nil) {
                onSuccess(hasSurveys);
            }
        } onError:^(NSString * error) {
            if (onError != nil) {
                onError([error UTF8String]);
            }
        }];
    }

    void _getSurveys(StringResponseCallback onSuccess, ErrorCallback onError) {
        [BitLabsBridge getSurveysOnSuccess:^(NSString * surveysJson) {
            if (onSuccess != nil) {
                onSuccess([surveysJson UTF8String]);
            }
        } onError:^(NSString * error) {
            if (onError != nil) {
                onError([error UTF8String]);
            }
        }];
    }

    void _setRewardCompletionHandler(RewardCallback onReward) {
        [BitLabsBridge setRewardHandlerOnReward:^(float payout) {
            if (onReward != nil) {
                onReward((double)payout);
            }
        }];
    }

    void _launchOfferWall() {
        [BitLabsBridge launchOfferWallWithParent:UnityGetGLViewController()];
    }

    void _requestTrackingAuthorization() {
        [BitLabsBridge requestTrackingAuthorization];
    }
}

@end
