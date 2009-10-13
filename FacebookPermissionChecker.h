#import "FBConnect/FBConnect.h"

@protocol FacebookPermissionCheckerDelegate

- (void)userHasPermission:(NSString*)permission;
- (void)userDoesNotHavePermission:(NSString*)permission;
- (void)checkOfPermission:(NSString*)permission failed:(NSError*)error;

@end


@interface FacebookPermissionChecker : NSObject<FBRequestDelegate> {
    id<FacebookPermissionCheckerDelegate> delegate;
       
    NSString *permission;
    FBSession *session;
}
@property (assign) id<FacebookPermissionCheckerDelegate> delegate;
                      
- (id)initWithPermission:(NSString*)permission session:(FBSession*)session;

- (void)check;
@end
