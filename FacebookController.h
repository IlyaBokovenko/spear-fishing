#import "FBConnect.h"
#import "FacebookPermissionChecker.h"

@interface FacebookController : UIViewController<FBSessionDelegate, FBRequestDelegate, FacebookPermissionCheckerDelegate, FBDialogDelegate>{
	FBSession* session;
	FacebookPermissionChecker* permissionChecker;
	
	int fishes;
	int weight;
	
	BOOL isExternalGuiShown;
}

@property(readonly, nonatomic) BOOL isExternalGuiShown;

+(FacebookController*)createFacebookController;
-(void)uploadScoreFishes: (int)_fishes weight: (int)_weight;

@end