#import "AdwhirlController.h"
#import "ARRollerProtocol.h"
#import "ARRollerView.h"

@implementation AdwhirlController

@synthesize view;

#pragma mark private

-(void) setupView{
	view.hidden = YES;	
	view.transform = CGAffineTransformMakeRotation(M_PI/2);
	[parent addSubview:view];			
}


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

- (void) dealloc{
	[view release];
	[super dealloc];
}


#pragma mark controlling

-(void)createAd{
	view = [[ARRollerView requestRollerViewWithDelegate:self] retain];	
	[self setupView];
}

-(void)createDummy{
	view = [[[UIImageView alloc] initWithImage: [UIImage imageNamed: @"dummy-ad.png"]] autorelease];
	[self setupView];
}

-(void)adjustViewSize{	
	view.frame = CGRectMake(320-48-2, (480-320)/2, 48, 320);
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