#import "ARRollerProtocol.h"

@interface AdwhirlController : UIViewController<ARRollerDelegate> {
	UIView* parent;
}

+(AdwhirlController*) createAdwhirlControllerWith: (UIView*)parent;
-(void)showAd;
-(void)showDummyBanner;
-(void)adjustViewSize;

@end
