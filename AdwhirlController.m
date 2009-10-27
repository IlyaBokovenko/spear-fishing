#import "AdwhirlController.h"
#import "ARRollerProtocol.h"
#import "ARRollerView.h"

@implementation AdwhirlController
#pragma mark UIViewController

-(id)initWith: (UIView*)_parent{
	if(self = [super init]){
		parent = _parent;
	}
	
	return self;	
}

+(AdwhirlController*) createAdwhirlControllerWith: (UIView*)parent{
	AdwhirlController* instance = [[self alloc] initWith: parent];	
	return instance;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    return (interfaceOrientation == UIInterfaceOrientationLandscapeRight);
}

#pragma mark controlling

-(void)createAd{
	self.view = [[ARRollerView requestRollerViewWithDelegate:self] retain];	
	self.view.hidden = YES;	
	[parent addSubview:self.view];	
}

-(void)createDummy{
	self.view = [[UIImageView alloc] initWithImage: [UIImage imageNamed: @"dummy-ad.png"]];
	self.view.hidden = YES;	
	[parent addSubview:self.view];		
}


-(void)adjustViewSize{	
	self.view.frame = CGRectMake(320-48-2, (480-320)/2, 48, 320);
}

#pragma mark ARRollerDelegate

- (NSString*)adWhirlApplicationKey{
	return @"d3a1d37a928b102caf90c29cca1d33aa";
}

#pragma mark ARRollerDelegate optional delegate method implementations 

- (void)didReceiveAd:(ARRollerView*)adWhirlView 
{ 
	NSLog(@"Received ad from \n%@!", [adWhirlView mostRecentNetworkName]); 
    
} 
- (void)didFailToReceiveAd:(ARRollerView*)adWhirlView usingBackup:(BOOL)YesOrNo 
{ 
	NSLog(@"Failed to receive ad from \n%@.  \nUsing Backup: %@", [adWhirlView mostRecentNetworkName], YesOrNo ? @"YES" : @"NO"); 
}

- (void)rollerReceivedNotificationAdsAreOff:(ARRollerView*)adWhirlView{
	NSLog(@"Ads are off");
}

- (void)rollerReceivedRequestForDeveloperToFulfill:(ARRollerView*)adWhirlView{
	NSLog(@"Custom ads");
}

@end