#import "FacebookController.h"
#import "UIAlertView+Utils.h"
#import "FBLoginButton.h"

static NSString* API_KEY = @"1bb54f067729f2933a888d9f7e871e21";
static NSString* SHARE_PERMISSION = @"share_item";

@interface FacebookController()

@property(readonly) FBLoginButton* loginButton;
@property(retain) NSString* userName;

@end

@implementation FacebookController

@synthesize isExternalGuiShown;
@synthesize userName;

#pragma mark private

-(void)postIfCan{
	if(userName && permissionsChecked)
		[self performSelectorOnMainThread:@selector(postLink) withObject:nil waitUntilDone:NO];
}

-(FBLoginButton*)createLoginButton{
	FBLoginButton* btn = [[FBLoginButton alloc] initWithFrame: CGRectMake(230, 300, 100, 100)];	
	btn.session = session;
	btn.style = FBLoginButtonStyleNormal;
	[[[UIApplication sharedApplication] keyWindow] addSubview: btn];
	return btn;
}

-(FBLoginButton*)loginButton{
	if(!loginButton)
		loginButton = [self createLoginButton];
	return loginButton;	
}

-(void)requestName{
	NSMutableDictionary* params = [NSMutableDictionary dictionary];
	[params setObject:@"first_name, last_name, name, locale" forKey:@"fields"];	
	NSString* strUid = [[NSNumber numberWithLongLong: session.uid] stringValue];	
	[params setObject: strUid forKey:@"uids"];
	[[FBRequest requestWithDelegate:self] call:@"facebook.users.getInfo" params: params];
}

-(NSString*)comment{
	return [NSString stringWithFormat:@"%@ is playing Speargun Hunter 3D!\nCaught %d %@ and total weight of %dLbs. Beat that!", userName, fishes, (fishes == 1 ? @"fish" : @"fishes"), weight];
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
	[loginButton release];
	[permissionChecker release];
	[super dealloc];
}

#pragma mark controlling

-(void)uploadScoreFishes: (int)_fishes weight: (int)_weight{
	fishes = _fishes;
	weight = _weight;

	permissionsChecked = NO;
	if([session isConnected]){
		[permissionChecker performSelectorOnMainThread:@selector(check) withObject:nil waitUntilDone:NO];
	}else{
		self.userName = nil;
		FBLoginDialog* dialog = [[[FBLoginDialog alloc] initWithSession:session] autorelease];	
		dialog.delegate = self;
		isExternalGuiShown = YES;
		[dialog performSelectorOnMainThread:@selector(show) withObject:nil waitUntilDone:NO];		
	}
}

#pragma mark FBSessionDelegate

- (void)session:(FBSession*)_session didLogin:(FBUID)uid{	
	isExternalGuiShown = NO;
	NSLog(@"facebook login, uid = %lld", session.uid);
	
	// 2 async calls
	[permissionChecker performSelectorOnMainThread:@selector(check) withObject:nil waitUntilDone:NO];
	[[FBRequest requestWithDelegate:self] call:@"facebook.users.getLoggedInUser" params:[NSDictionary	 dictionary]];
}

- (void)sessionDidNotLogin:(FBSession*)session{
	isExternalGuiShown = NO;
	NSLog(@"facebook login failed");	
}

- (void)session:(FBSession*)session willLogout:(FBUID)uid{
		NSLog(@"facebook will login");
}

- (void)sessionDidLogout:(FBSession*)session{
		self.userName = nil;
		NSLog(@"facebook did logout");
}

#pragma mark FBRequestDelegate

- (void)requestLoading:(FBRequest*)request{
	NSLog(@"request %@ is loading", request.method);
}

- (void)request:(FBRequest*)request didReceiveResponse:(NSURLResponse*)response{
	NSLog(@"request %@ did receive response: %@", request.method, response);
}

- (void)request:(FBRequest*)request didFailWithError:(NSError*)error{
	NSLog(@"request %@ failed with error: %@", request.method, error);	
}

- (void)request:(FBRequest*)request didLoad:(id)result{
	NSLog(@"request %@ did load with result: %@", request.method, result);
		
	if([request.method isEqual: @"facebook.users.getLoggedInUser"]){
		NSString* uid  = (NSString*)result;
		NSMutableDictionary* params = [NSMutableDictionary dictionary];
		[params setObject:@"first_name" forKey:@"fields"];		
		[params setObject: uid forKey:@"uids"];
		[[FBRequest requestWithDelegate:self] call:@"facebook.users.getInfo" params: params];
	}else if([request.method isEqual: @"facebook.users.getInfo"]){
		NSArray* results = (NSArray*)result;
		if(results.count){
			NSDictionary* info = [results objectAtIndex: 0];		
			self.userName = [info objectForKey: @"first_name"];		
			[self postIfCan];			
		}
	}else if([request.method isEqual: @"links.post"]){
		[UIAlertView showAlertViewWithTitle:@"Score sucessfully posted!"];
	}
}

- (void)requestWasCancelled:(FBRequest*)request{
	NSLog(@"request %@ was canceled", request.method);	
}

#pragma mark FacebookPermissionCheckerDelegate

- (void)userHasPermission:(NSString*)permission{
	permissionsChecked = YES;
	[self postIfCan];
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


-(void)showLoginButton:(BOOL)show{
	self.loginButton.hidden = !show;
}

@end
