#import "FacebookController.h"
#import "UIAlertView+Utils.h"

static NSString* API_KEY = @"1bb54f067729f2933a888d9f7e871e21";
static NSString* SHARE_PERMISSION = @"share_item";

@implementation FacebookController

@synthesize isExternalGuiShown;

#pragma mark private

-(NSString*)comment{
	return [NSString stringWithFormat:@"Yossi is playing Speargun Hunter 3D!\nCaught %d fish and total weight of %dLbs. Beat that!", fishes, weight];
}


- (void)postLink {
	NSMutableDictionary* params = [NSMutableDictionary dictionary];
	
	[params setObject: @"www.google.com" forKey:@"url"];
	[params setObject: [self comment] forKey:@"comment"];
	
	[[FBRequest requestWithDelegate:self] call:@"links.post" params:params];
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    return (interfaceOrientation == UIInterfaceOrientationLandscapeRight);
}

-(void)createSession{
	// singleton actually - no need to retain
	session = [FBSession sessionForApplication: API_KEY
										 secret:@"6b7bc15fa52e7531db6e796280c9e472" 
									   delegate: self];
}

#pragma mark NSObject

- (id) init{
	self = [super init];
	if (self != nil) {
		[self createSession];
		permissionChecker = [[FacebookPermissionChecker alloc] initWithPermission:SHARE_PERMISSION session:session];
		permissionChecker.delegate = self;		
	}
	return self;
}

+(FacebookController*)createFacebookController{
	return [[self class] new];	
}

- (void) dealloc{
	[permissionChecker release];
	[super dealloc];
}

#pragma mark controlling

-(void)uploadScoreFishes: (int)_fishes weight: (int)_weight{
	fishes = _fishes;
	weight = _weight;
	
	if([session isConnected]){
		[permissionChecker performSelectorOnMainThread:@selector(check) withObject:nil waitUntilDone:NO];
	}else{
		FBLoginDialog* dialog = [[[FBLoginDialog alloc] initWithSession:session] autorelease];	
		dialog.delegate = self;
		isExternalGuiShown = YES;
		[dialog performSelectorOnMainThread:@selector(show) withObject:nil waitUntilDone:NO];		
	}
}

#pragma mark FBSessionDelegate

- (void)session:(FBSession*)session didLogin:(FBUID)uid{
	isExternalGuiShown = NO;
	NSLog(@"facebook login");
	[permissionChecker performSelectorOnMainThread:@selector(check) withObject:nil waitUntilDone:NO];
}

- (void)sessionDidNotLogin:(FBSession*)session{
	isExternalGuiShown = NO;
	NSLog(@"facebook login failed");	
}

- (void)session:(FBSession*)session willLogout:(FBUID)uid{
		NSLog(@"facebook will login");
}

- (void)sessionDidLogout:(FBSession*)session{
		NSLog(@"facebook did login");
}

#pragma mark FBRequestDelegate

- (void)requestLoading:(FBRequest*)request{
	NSLog(@"request is loading");
}

- (void)request:(FBRequest*)request didReceiveResponse:(NSURLResponse*)response{
	NSLog(@"request did receive response: %@", response);
}

- (void)request:(FBRequest*)request didFailWithError:(NSError*)error{
	NSLog(@"request failed with error: %@", error);	
}

- (void)request:(FBRequest*)request didLoad:(id)result{
	NSLog(@"request did load with result: %@", result);
	[UIAlertView showAlertViewWithTitle:@"Score sucessfully posted!"];
}

- (void)requestWasCancelled:(FBRequest*)request{
	NSLog(@"request was canceled");	
}

#pragma mark FacebookPermissionCheckerDelegate

- (void)userHasPermission:(NSString*)permission{
	[self performSelectorOnMainThread:@selector(postLink) withObject:nil waitUntilDone:NO];
}

- (void)userDoesNotHavePermission:(NSString*)permission{
	FBPermissionDialog *dialog = [[FBPermissionDialog new] autorelease];
    dialog.delegate = self;
    dialog.permission = SHARE_PERMISSION;
	isExternalGuiShown = YES;
    [dialog performSelectorOnMainThread:@selector(show) withObject:nil waitUntilDone:NO];
}

- (void)checkOfPermission:(NSString*)permission failed:(NSError*)error{
	isExternalGuiShown = NO;
	[UIAlertView showAlertViewWithTitle:@"Permissions check error" message:[error localizedDescription]];
}

#pragma mark FBDialogDelegate 

- (void)dialogDidSucceed:(FBDialog*)dialog { // permission or login dialog
   isExternalGuiShown = NO;
	
	if([dialog isKindOfClass: [FBPermissionDialog class]]){
		[self performSelectorOnMainThread:@selector(postLink) withObject:nil waitUntilDone:NO];		
	} 
}

- (void)dialogDidCancel:(FBDialog*)dialog { 
	isExternalGuiShown = NO; 
}

- (void)dialog:(FBDialog*)dialog didFailWithError:(NSError*)error {
	isExternalGuiShown = NO;
    [UIAlertView showAlertViewWithTitle:@"Permissions dialog error" message:[error localizedDescription]];    
}


@end