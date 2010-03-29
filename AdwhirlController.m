#import "AdwhirlController.h"
#import "ARRollerProtocol.h"
#import "ARRollerView.h"
#import "common.h"

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
	adViewController.delegate = nil;
	[adViewController release];
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
	//static const int statusBarHeight = 26;
	//view.frame = CGRectMake(320-48-statusBarHeight-2, (480-320)/2, 48, 320);
	view.frame = CGRectMake(2, (480-320)/2, 48, 320);
}

#pragma mark ARRollerDelegate

- (NSString*)adWhirlApplicationKey{
	return g_adwhirlKey();
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

	adViewController.delegate = nil;
	[adViewController release];
	
	adViewController = [[GADAdViewController alloc] initWithDelegate:self];
	adViewController.adSize = kGADAdSize320x50;
	NSDictionary *attributes = [NSDictionary dictionaryWithObjectsAndKeys:
								@"ca-mb-app-pub-0240775936078148", kGADAdSenseClientID,
								@"Yossi Malki", kGADAdSenseCompanyName,
								@"Spearfishing 3D - Speargun Hunter (SF1)", kGADAdSenseAppName,
								@"iphone+games,iphone+applications,online+games,social+games,iphone+accessories,iphone+case,free+games,appstore+games,ipod+touch+accessories,mobile+games,action+games,sport+games", kGADAdSenseKeywords,
								@"http://bit.ly/spearfishing", kGADAdSenseAppWebContentURL,
								[NSNumber numberWithInt:0], kGADAdSenseIsTestAdRequest,
								[NSArray arrayWithObjects:@"8433703806", nil], kGADAdSenseChannelIDs,
								[NSNumber numberWithInt:1], kGADAdSenseIsTestAdRequest,
								nil];
	// Loading the ad will create the ad view.
	[adViewController loadGoogleAd:attributes];
	// Position ad at bottom of screen.
	UIView *adView = adViewController.view;
	CGFloat adViewHeight = [adView bounds].size.height;
	CGRect bounds = [adWhirlView bounds];
	CGPoint center = CGPointMake(CGRectGetMidX(bounds),CGRectGetMaxY(bounds) - adViewHeight / 2);
	[adView setCenter:center];
	[adWhirlView replaceBannerViewWith:adView];
	
}

#pragma mark GADAdViewControllerDelegate

- (GADAdClickAction)adControllerActionModelForAdClick:(GADAdViewController *)adController {
	return GAD_ACTION_DISPLAY_INTERNAL_WEBSITE_VIEW;
}

- (void)adController:(GADAdViewController *)adController failedWithError:(NSError *)error {
	adViewController.delegate = nil;
	[adViewController release];
	adViewController = nil;
	// Handle error here.
}
@end