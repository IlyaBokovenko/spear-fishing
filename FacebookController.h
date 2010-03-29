#import "FBConnect.h"
#import "FacebookPermissionChecker.h"

@class FBLoginButton;

@interface FacebookController : UIViewController<FBSessionDelegate, FBRequestDelegate, FacebookPermissionCheckerDelegate, FBDialogDelegate>{
	FBSession* session;
	FacebookPermissionChecker* permissionChecker;
	BOOL permissionsChecked;
	
	int fishes;
	float weight;
	NSString* userName;
	
	BOOL isExternalGuiShown;
	
	FBLoginButton* loginButton;
}

@property(readonly, nonatomic) BOOL isExternalGuiShown;

+(FacebookController*)createFacebookController;
-(void)uploadScoreFishes: (int)_fishes weight: (float)_weight;

@end