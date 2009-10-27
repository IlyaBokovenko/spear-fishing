#import "ARRollerProtocol.h"

@interface AdwhirlController : UIViewController<ARRollerDelegate> {
	UIView* parent;	
}

+(AdwhirlController*) createAdwhirlControllerWith: (UIView*)parent;
-(void)adjustViewSize;
-(void)createDummy;
-(void)createAd;

@end
