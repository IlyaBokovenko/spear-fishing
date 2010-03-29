#import "ARRollerProtocol.h"
#import "GADAdViewController.h"
#import "GADAdSenseParameters.h"

@interface AdwhirlController : NSObject<ARRollerDelegate , GADAdViewControllerDelegate> {
	UIView* parent;
	UIView* view;
	GADAdViewController *adViewController;
}

@property(readonly) UIView* view;

+(AdwhirlController*) createAdwhirlControllerWith: (UIView*)parent;
-(void)adjustViewSize;
-(void)createDummy;
-(void)createAd;

@end
