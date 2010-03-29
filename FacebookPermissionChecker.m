#import "FacebookPermissionChecker.h"

@implementation FacebookPermissionChecker

@synthesize delegate;

- (void)check {
    NSDictionary *args = [NSDictionary dictionaryWithObject:permission forKey:@"ext_perm"];
    FBRequest *request = [FBRequest requestWithSession:session delegate:self];
    [request call:@"users.hasAppPermission" params:args];
}


#pragma mark FBRequestDelegate

- (void)request:(FBRequest*)request didLoad:(id)result {
    if([result boolValue]) {
        [delegate userHasPermission:permission];
    } else {
        [delegate userDoesNotHavePermission:permission];
    }
}


- (void)request:(FBRequest*)request didFailWithError:(NSError*)error {
    [delegate checkOfPermission:permission failed:error];
}


#pragma mark init/deinit

- (id)initWithPermission:(NSString*)p session:(FBSession*)s {
    if(self = [super init]) {
        permission = [p retain];
        session = [s retain];
    }
    
    return self;
}


- (void)dealloc {
    [permission release];
    [session release];
    
    [super dealloc];
}


@end
