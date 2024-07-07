#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <BitLabs/BitLabs-Swift.h>
#import "WidgetWrapper.h"

@implementation WidgetWrapper

WidgetView *widgetView;

extern "C" {
   void _initWidget(const char *token, const char *uid, const char *type) {
        NSString *tokenStr = [NSString stringWithUTF8String:token];
        NSString *uidStr = [NSString stringWithUTF8String:uid];
        NSString *typeStr = [NSString stringWithUTF8String:type];
        
        dispatch_async(dispatch_get_main_queue(), ^{
            UIViewController *rootViewController = [UIApplication sharedApplication].delegate.window.rootViewController;
            if (!rootViewController) {
                NSLog(@"[BitLabs] No root view controller found!");
                return;
            }

            widgetView = [[WidgetView alloc] initWithToken:tokenStr uid:uidStr type:typeStr];
            widgetView.center = rootViewController.view.center;
            [rootViewController.view addSubview:widgetView];
        });
    }

    void _setPosition(double x, double y) {
        dispatch_async(dispatch_get_main_queue(), ^{
            [widgetView setOriginWithX:x y:y];
        });
    }

    void _setSize(double width, double height) {
        dispatch_async(dispatch_get_main_queue(), ^{
            [widgetView setSizeWithWidth:width height:height];
        });
    }
}

@end