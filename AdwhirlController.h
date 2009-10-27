#import "ARRollerProtocol.h"

@interface AdwhirlController : NSObject<ARRollerDelegate> {
	UIView* parent;
	UIView* view;
}

@property(readonly) UIView* view;

+(AdwhirlController*) createAdwhirlControllerWith: (UIView*)parent;
-(void)adjustViewSize;
-(void)createDummy;
-(void)createAd;

@end
