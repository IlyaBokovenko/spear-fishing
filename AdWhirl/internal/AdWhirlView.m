/*

 AdWhirlView.m

 Copyright 2009 AdMob, Inc.
 
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at
 
 http://www.apache.org/licenses/LICENSE-2.0
 
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
 
*/

#import "AdWhirlView.h"
#import "AdWhirlView+.h"
#import "AdWhirlConfig.h"
#import "AdWhirlAdNetworkConfig.h"
#import "CJSONDeserializer.h"
#import "AdWhirlLog.h"
#import "AdWhirlAdNetworkAdapter.h"
#import "AdWhirlError.h"

#define kAdWhirlViewAdSubViewTag   1000
#define kAWMinimumTimeBetweenFreshAdRequests 4.9f

NSInteger adNetworkPriorityComparer(id a, id b, void *ctx) {
  AdWhirlAdNetworkConfig *acfg = a, *bcfg = b;
  if(acfg.priority < bcfg.priority)
    return NSOrderedAscending;
  else if(acfg.priority > bcfg.priority)
    return NSOrderedDescending;
  else
    return NSOrderedSame;
}

@implementation AdWhirlView

@synthesize delegate;
@synthesize config;
@synthesize prioritizedAdNetworks;
@synthesize currAdapter;
@synthesize lastRequestTime;
@synthesize refreshTimer;
@synthesize lastError;

+ (AdWhirlView *)requestAdWhirlViewWithDelegate:(id<AdWhirlDelegate>)delegate {
  return [[[AdWhirlView alloc] initWithDelegate:delegate] autorelease];
}

static id<AdWhirlDelegate> classAdWhirlDelegateForConfig = nil;

+ (void)startPreFetchingConfigurationDataWithDelegate:(id<AdWhirlDelegate>)delegate {
  if (classAdWhirlDelegateForConfig != nil) return;
  classAdWhirlDelegateForConfig = delegate;
  [AdWhirlConfig fetchConfig:[delegate adWhirlApplicationKey] delegate:self];
}

- (id)initWithDelegate:(id<AdWhirlDelegate>)d {
  self = [super initWithFrame:kAdWhirlViewDefaultFrame];
  if (self != nil) {
    delegate = d;
    self.backgroundColor = [UIColor clearColor];
    [AdWhirlConfig fetchConfig:[delegate adWhirlApplicationKey] delegate:self];
    self.clipsToBounds = YES; // to prevent ugly artifacts if ad network banners are bigger than the default frame
  }
  return self;
}

- (void)prepAdNetworks {
  NSMutableArray *freshNets = [[NSMutableArray alloc] initWithArray:config.adNetworkConfigs];
  [freshNets sortUsingFunction:adNetworkPriorityComparer context:nil];
  totalPercent = 0;
  for (AdWhirlAdNetworkConfig *cfg in freshNets) {
    totalPercent += cfg.trafficPercentage;
  }
  self.prioritizedAdNetworks = freshNets;
  [freshNets release];
}

static BOOL randSeeded = NO;

- (AdWhirlAdNetworkConfig *)nextNetworkByPercent {
  // get random number based on the current totalPercent
  // walk down the prioritizedAdNetworks array subtracting the totalPercent
  // return AdNetwork.
  if (!randSeeded) {
    srandom(CFAbsoluteTimeGetCurrent());
  }
  long randNum = random();
  
  int dart = randNum % totalPercent;
  
  int tempTotal = 0;
  
  AdWhirlAdNetworkConfig *result = nil;
  for (AdWhirlAdNetworkConfig *network in prioritizedAdNetworks) {
    tempTotal += network.trafficPercentage;
    if (dart < tempTotal) {
      // this is the one to use.
      result = network;
      break;
    }
  }
  
  AWLogDebug(@"nextNetworkByPercent chosen %@ (%@), dart %d in %d",
        result.nid, result.networkName, dart, totalPercent);
  return result;
}

- (AdWhirlAdNetworkConfig *)nextNetworkByPriority {
  AdWhirlAdNetworkConfig *result = [prioritizedAdNetworks objectAtIndex:0];
  AWLogDebug(@"nextNetworkByPriority chosen %@ (%@)", result.nid, result.networkName);
  return result;
}

- (void)makeFirstAdRequest
{
  [self makeAdRequest:YES];
}

- (void)makeAdRequest:(BOOL)isFirstRequest
{
  // only make request in main thread
  if (![NSThread isMainThread]) {
    // respawn this call on the main thread.
    if (isFirstRequest) {
      [self performSelectorOnMainThread:@selector(makeFirstAdRequest) withObject:nil waitUntilDone:NO];      
    }
    else {
      [self performSelectorOnMainThread:@selector(rollOver) withObject:nil waitUntilDone:NO];            
    }
    return;
  }
  
  if ([prioritizedAdNetworks count] == 0) {
    // ran out of ad networks
    [lastError release];
    lastError = [[AdWhirlError alloc] initWithCode:AdWhirlAdRequestNoMoreAdNetworks
                                       description:@"No more ad networks for roll over"];
    if ([delegate respondsToSelector:@selector(adWhirlDidFailToReceiveAd:usingBackup:)]) {
      [[self retain] autorelease]; // to prevent self being freed before this returns
      [delegate adWhirlDidFailToReceiveAd:self usingBackup:NO];
    }
    return;
  }

  @synchronized(self) {
    if (requesting) {
      AWLogWarn(@"Already requesting ad");
      return;
    }
    requesting = YES;
  }
  
  AdWhirlAdNetworkConfig *nextAdNetwork = nil;
  
  if(isFirstRequest)
  {
    nextAdNetwork = [self nextNetworkByPercent];    
  }
  else
  {
    nextAdNetwork = [self nextNetworkByPriority];
  }

  AdWhirlAdNetworkAdapter *adapter = [nextAdNetwork.adapterClass alloc];
  self.currAdapter = [adapter initWithAdWhirlDelegate:delegate
                                                 view:self
                                               config:config
                                        networkConfig:nextAdNetwork];
  // take nextAdNetwork out so we don't request again when we roll over
  [prioritizedAdNetworks removeObject:nextAdNetwork];

  if(lastRequestTime)
    [lastRequestTime release];
  lastRequestTime = [[NSDate date] retain];

  [currAdapter getAd];
}

- (void)_requestFreshAdInternal {
  NSTimeInterval sinceLast = -[lastRequestTime timeIntervalSinceNow];
  if (lastRequestTime == nil || sinceLast > kAWMinimumTimeBetweenFreshAdRequests) {
    @synchronized(self) {
      if (!config) {
        [lastError release];
        lastError = [[AdWhirlError alloc] initWithCode:AdWhirlAdRequestNoConfigError
                                           description:@"No ad configuration"];
        // no config, can't do anything
        if ([delegate respondsToSelector:@selector(adWhirlDidFailToReceiveAd:usingBackup:)]) {
          [[self retain] autorelease]; // to prevent self being freed before this returns
          [delegate adWhirlDidFailToReceiveAd:self usingBackup:NO];
        }
        return;
      }
      [self prepAdNetworks];
    }
    [self makeFirstAdRequest];
  }
  else if (sinceLast <= kAWMinimumTimeBetweenFreshAdRequests) {
    [lastError release];
    NSString *desc = [NSString stringWithFormat:
                      @"Requesting fresh ad too soon! It has been only %lfs. Minimum %lfs",
                      sinceLast, kAWMinimumTimeBetweenFreshAdRequests];
    lastError = [[AdWhirlError alloc] initWithCode:AdWhirlAdRequestTooSoonError
                                       description:desc];
    if ([delegate respondsToSelector:@selector(adWhirlDidFailToReceiveAd:usingBackup:)]) {
      [[self retain] autorelease]; // to prevent self being freed before this returns
      [delegate adWhirlDidFailToReceiveAd:self usingBackup:NO];
    }
    return;
  }
}

- (void)requestFreshAd {
  // public method, hence more checks with delegate calls
  if (ignoreNewAdRequests) {
    // don't request new ad
    [lastError release];
    lastError = [[AdWhirlError alloc] initWithCode:AdWhirlAdRequestIgnoredError
                                       description:@"ignoreNewAdRequests flag set"];
    if ([delegate respondsToSelector:@selector(adWhirlDidFailToReceiveAd:usingBackup:)]) {
      [[self retain] autorelease]; // to prevent self being freed before this returns
      [delegate adWhirlDidFailToReceiveAd:self usingBackup:NO];
    }
  }
  if (requesting) {
    // don't request if there's a request outstanding
    [lastError release];
    lastError = [[AdWhirlError alloc] initWithCode:AdWhirlAdRequestInProgressError
                                       description:@"Ad request already in progress"];
    if ([delegate respondsToSelector:@selector(adWhirlDidFailToReceiveAd:usingBackup:)]) {
      [[self retain] autorelease]; // to prevent self being freed before this returns
      [delegate adWhirlDidFailToReceiveAd:self usingBackup:NO];
    }
  }
  [self _requestFreshAdInternal];
}

- (void)requestFreshAdTimer {
  self.refreshTimer = nil;
  if (ignoreAutoRefreshTimer) {
    // don't make ad request, but schedule the next one
    [self scheduleNextAdRefresh];
  }
  else {
    [self _requestFreshAdInternal];
  }
}

- (void)scheduleNextAdRefresh {
  if (config.refreshInterval > kAWMinimumTimeBetweenFreshAdRequests
      && !ignoreNewAdRequests) {
    AWLogDebug(@"Scheduling next ad refresh after %lfs", config.refreshInterval);
    if (refreshTimer) {
      [refreshTimer invalidate];
    }
    self.refreshTimer = [NSTimer scheduledTimerWithTimeInterval:config.refreshInterval
                                                         target:self
                                                       selector:@selector(requestFreshAdTimer)
                                                       userInfo:nil
                                                        repeats:NO];
  }
}

- (void)rollOver {
  if (ignoreNewAdRequests) return; // don't request ad
  [self makeAdRequest:NO];
}

- (BOOL)adExists {
  UIView *currAdView = [self viewWithTag:kAdWhirlViewAdSubViewTag];
  return currAdView != nil;
}

- (NSString *)mostRecentNetworkName {
  if (currAdapter == nil) return nil;
  return currAdapter.networkConfig.networkName;
}

- (void)ignoreAutoRefreshTimer {
  ignoreAutoRefreshTimer = YES;
}

- (void)doNotIgnoreAutoRefreshTimer {
  ignoreAutoRefreshTimer = NO;
}

- (void)ignoreNewAdRequests {
  ignoreNewAdRequests = YES;
}

- (void)doNotIgnoreNewAdRequests {
  ignoreNewAdRequests = NO;
}

- (void)transitionToView:(UIView *)view {
  UIView *currAdView = [self viewWithTag:kAdWhirlViewAdSubViewTag];
  view.tag = kAdWhirlViewAdSubViewTag;
  if (currAdView) {
    // swap
    currAdView.tag = 0;
    if (config.bannerAnimationType == AWBannerAnimationTypeNone) {
      [self addSubview:view];
      [currAdView removeFromSuperview];
    }
    else {
      AWBannerAnimationType animType;
      if (config.bannerAnimationType == AWBannerAnimationTypeRandom) {
        if (!randSeeded) {
          srandom(CFAbsoluteTimeGetCurrent());
        }
        // range is 1 to 7, inclusive
        animType = (random() % 7) + 1;
        AWLogDebug(@"Animation type chosen by random is %d", animType);
      }
      else {
        animType = config.bannerAnimationType;
      }
      
      switch (animType) {
        case AWBannerAnimationTypeSlideFromLeft:
        {
          CGRect f = view.frame;
          f.origin.x = -f.size.width;
          view.frame = f;
          [self addSubview:view];
          break;
        }
        case AWBannerAnimationTypeSlideFromRight:
        {
          CGRect f = view.frame;
          f.origin.x = self.frame.size.width;
          view.frame = f;
          [self addSubview:view];
          break;
        }
        case AWBannerAnimationTypeFadeIn:
          view.alpha = 0;
          [self addSubview:view];
          break;
      }
      
      [currAdView retain]; // will be released when animation is done
      [UIView beginAnimations:@"AdWhirlAdTransition" context:currAdView];
      [UIView setAnimationDelegate:self];
      [UIView setAnimationDidStopSelector:@selector(newAdAnimationDidStopWithAnimationID:finished:context:)];
      [UIView setAnimationBeginsFromCurrentState:YES];
      [UIView setAnimationDuration:1.0];
      // cache has to set to NO because of VideoEgg
      switch (animType) {
        case AWBannerAnimationTypeFlipFromLeft:
          [self addSubview:view];
          [currAdView removeFromSuperview];
          [UIView setAnimationTransition:UIViewAnimationTransitionFlipFromLeft
                                 forView:self cache:NO];
          break;
        case AWBannerAnimationTypeFlipFromRight:
          [self addSubview:view];
          [currAdView removeFromSuperview];
          [UIView setAnimationTransition:UIViewAnimationTransitionFlipFromRight
                                 forView:self cache:NO];
          break;
        case AWBannerAnimationTypeCurlUp:
          [self addSubview:view];
          [currAdView removeFromSuperview];
          [UIView setAnimationTransition:UIViewAnimationTransitionCurlUp
                                 forView:self cache:NO];
          break;
        case AWBannerAnimationTypeCurlDown:
          [self addSubview:view];
          [currAdView removeFromSuperview];
          [UIView setAnimationTransition:UIViewAnimationTransitionCurlDown
                                 forView:self cache:NO];
          break;
        case AWBannerAnimationTypeSlideFromLeft:
        case AWBannerAnimationTypeSlideFromRight:
        {
          CGRect f = view.frame;
          f.origin.x = 0;
          view.frame = f;
          break;
        }
        case AWBannerAnimationTypeFadeIn:
          view.alpha = 1.0;
          break;
        default:
          [self addSubview:view];
          AWLogWarn(@"Unrecognized Animation type: %d", animType);
          break;
      }
      [UIView commitAnimations];
    }
  }
  else {
    // new
    [self addSubview:view];
  }
}

- (void)replaceBannerViewWith:(UIView*)bannerView {
  [self transitionToView:bannerView];
}

// Called at the end of the new ad animation; we use this opportunity to do memory management cleanup.
// See the comment in adDidLoad:.
- (void)newAdAnimationDidStopWithAnimationID:(NSString *)animationID finished:(BOOL)finished context:(void *)context {
  UIView *adViewToRemove = (UIView *)context;
  [adViewToRemove removeFromSuperview];
  [adViewToRemove release];
}

- (void)adapter:(AdWhirlAdNetworkAdapter *)adapter didReceiveAdView:(UIView *)view {
  AWLogDebug(@"Received ad from adapter (nid %@)", adapter.networkConfig.nid);
  [self transitionToView:view];
  requesting = NO;
  if ([adapter shouldSendExMetric]) {
    [self notifyExImpression:adapter.networkConfig.nid netType:adapter.networkConfig.networkType];
  }
  if ([delegate respondsToSelector:@selector(adWhirlDidReceiveAd:)]) {
    [delegate adWhirlDidReceiveAd:self];
  }
  [self scheduleNextAdRefresh];
}

- (void)adapter:(AdWhirlAdNetworkAdapter *)adapter didFailAd:(NSError *)error {
  AWLogDebug(@"Failed to receive ad from adapter (nid %@): %@",
             adapter.networkConfig.nid, error);
  requesting = NO;
  BOOL doNotify = [delegate respondsToSelector:@selector(adWhirlDidFailToReceiveAd:usingBackup:)];
  if ([prioritizedAdNetworks count] == 0) {
    // we have run out of networks to try and need to error out.
    // just call the delegate's failed method.
    [lastError release];
    lastError = [[AdWhirlError alloc] initWithCode:AdWhirlAdRequestNoMoreAdNetworks
                                       description:@"No more ad networks for roll over"];
    if (doNotify) {
      [[self retain] autorelease]; // to prevent self being freed before this returns
      [delegate adWhirlDidFailToReceiveAd:self usingBackup:NO];
    }
    return;
  }
  [lastError release], lastError = nil; // no error really, just need to roll over
  if (doNotify) {
    [delegate adWhirlDidFailToReceiveAd:self usingBackup:YES];
  }
  [self makeAdRequest:NO]; // keep trying
}

- (void)adapterDidFinishAdRequest:(AdWhirlAdNetworkAdapter *)adapter {
  // view is supplied via other mechanism (e.g. Generic Notification)
  requesting = NO;
  if ([adapter shouldSendExMetric]) {
    [self notifyExImpression:adapter.networkConfig.nid netType:adapter.networkConfig.networkType];
  }
  [self scheduleNextAdRefresh];
}

- (void)metricPing:(NSURL *)endPointBaseURL nid:(NSString *)nid netType:(AdWhirlAdNetworkType)type {
  NSString *query = [NSString stringWithFormat:@"?appid=%@&nid=%@&type=%d&uuid=%@&country_code=%@&appver=%d",
                     [delegate adWhirlApplicationKey],
                     nid,
                     type,
                     [AdWhirlConfig uniqueId],
                     [[NSLocale currentLocale] localeIdentifier],
                     kAdWhirlAppVer];
  NSURL *metURL = [NSURL URLWithString:query
                         relativeToURL:endPointBaseURL];
  AWLogDebug(@"Sending metric ping to %@", metURL);
  NSURLRequest *metRequest = [NSURLRequest requestWithURL:metURL];
  [NSURLConnection connectionWithRequest:metRequest
                                delegate:nil]; // fire and forget
}

- (void)notifyExImpression:(NSString *)nid netType:(AdWhirlAdNetworkType)type {
  NSURL *baseURL = nil;
  if ([delegate respondsToSelector:@selector(adWhirlImpMetricURL)]) {
    baseURL = [delegate adWhirlImpMetricURL];
  }
  if (baseURL == nil) {
    baseURL = [NSURL URLWithString:kAdWhirlDefaultImpMetricURL];
  }
  [self metricPing:baseURL nid:nid netType:type];
}

- (void)notifyExClick:(NSString *)nid netType:(AdWhirlAdNetworkType)type {
  NSURL *baseURL = nil;
  if ([delegate respondsToSelector:@selector(adWhirlClickMetricURL)]) {
    baseURL = [delegate adWhirlClickMetricURL];
  }
  if (baseURL == nil) {
    baseURL = [NSURL URLWithString:kAdWhirlDefaultClickMetricURL];
  }
  [self metricPing:baseURL nid:nid netType:type];
}

- (void)dealloc {
  delegate = nil;
  [config release], config = nil;
  [prioritizedAdNetworks release], prioritizedAdNetworks = nil;
  totalPercent = 0;
  requesting = NO;
  [currAdapter release], currAdapter = nil;
  [lastRequestTime release], lastRequestTime = nil;
  [refreshTimer release], refreshTimer = nil;
  [lastError release], lastError = nil;

  [super dealloc];
}

#pragma mark UIView touch methods

- (BOOL)_isEventATouch30:(UIEvent *)event {
  if ([event respondsToSelector:@selector(type)]) {
    return event.type == UIEventTypeTouches;
  }
  return YES; // UIEvent in 2.2.1 has no type property, so assume yes.
}

- (BOOL)pointInside:(CGPoint)point withEvent:(UIEvent *)event {
  BOOL itsInside = [super pointInside:point withEvent:event];
  if (itsInside && currAdapter != nil && lastNotifyAdapter != currAdapter
      && [self _isEventATouch30:event]
      && [currAdapter shouldSendExMetric]) {
    [self ignoreAutoRefreshTimer]; // prevent reload
    lastNotifyAdapter = currAdapter;
    [self notifyExClick:currAdapter.networkConfig.nid netType:currAdapter.networkConfig.networkType];
  }
  return itsInside;
}

#pragma mark UIView methods

- (void)willMoveToSuperview:(UIView *)newSuperview {
  if (newSuperview == nil) {
    [refreshTimer invalidate];
    self.refreshTimer = nil;
  }
}

#pragma mark AdWhirlConfigDelegate methods

+ (NSURL *)adWhirlConfigURL {
  if (classAdWhirlDelegateForConfig != nil
      && [classAdWhirlDelegateForConfig respondsToSelector:@selector(adWhirlConfigURL)]) {
    return [classAdWhirlDelegateForConfig adWhirlConfigURL];
  }
  return nil;
}

+ (void)adWhirlConfigDidReceiveConfig:(AdWhirlConfig *)config {
  AWLogDebug(@"Fetched Ad network config: %@", config);
  if (classAdWhirlDelegateForConfig != nil
      && [classAdWhirlDelegateForConfig respondsToSelector:@selector(adWhirlDidReceiveConfig:)]) {
    [classAdWhirlDelegateForConfig adWhirlDidReceiveConfig:nil];
  }
}

+ (void)adWhirlConfigDidFail:(NSError *)error {
  AWLogError(@"Failed pre-fetching AdWhirl config: %@", error);
}

- (void)adWhirlConfigDidReceiveConfig:(AdWhirlConfig *)cfg {
  // now decide which ad network to use and fetch ad
  AWLogDebug(@"Fetched Ad network config: %@", cfg);
  if ([delegate respondsToSelector:@selector(adWhirlDidReceiveConfig:)]) {
    [delegate adWhirlDidReceiveConfig:self];
  }
  @synchronized(self) {
    self.config = cfg;
    if (cfg.adsAreOff) {
      if ([delegate respondsToSelector:@selector(adWhirlReceivedNotificationAdsAreOff:)]) {
        [[self retain] autorelease]; // to prevent self being freed before this returns
        [delegate adWhirlReceivedNotificationAdsAreOff:self];
      }
      return;
    }
    [self prepAdNetworks];
  }
  [self makeFirstAdRequest];
}

- (void)adWhirlConfigDidFail:(NSError *)error {
  AWLogError(@"Failed fetching AdWhirl config: %@", error);
  [lastError release];
  lastError = error;
  if ([delegate respondsToSelector:@selector(adWhirlDidFailToReceiveAd:usingBackup:)]) {
    [[self retain] autorelease]; // to prevent self being freed before this returns
    [delegate adWhirlDidFailToReceiveAd:self usingBackup:NO];
  }
}

- (NSURL *)adWhirlConfigURL {
  if ([delegate respondsToSelector:@selector(adWhirlConfigURL)]) {
    return [delegate adWhirlConfigURL];
  }
  return nil;
}


@end
