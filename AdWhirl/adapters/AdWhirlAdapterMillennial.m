/*

 AdWhirlAdapterMillennial.m
 
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

#import "AdWhirlAdapterMillennial.h"
#import "AdWhirlView.h"
#import "AdWhirlConfig.h"
#import "AdWhirlAdNetworkConfig.h"
#import "AdWhirlDelegateProtocol.h"
#import "AdWhirlLog.h"
#import "AdWhirlAdNetworkAdapter+Helpers.h"
#import "AdWhirlAdNetworkRegistry.h"

@implementation AdWhirlAdapterMillennial

+ (AdWhirlAdNetworkType)networkType {
  return AdWhirlAdNetworkTypeMillennial;
}

+ (void)load {
  [[AdWhirlAdNetworkRegistry sharedRegistry] registerClass:self];
}

- (id)initWithAdWhirlDelegate:(id<AdWhirlDelegate>)delegate
                         view:(AdWhirlView *)view
                       config:(AdWhirlConfig *)config
                networkConfig:(AdWhirlAdNetworkConfig *)netConf {
  self = [super initWithAdWhirlDelegate:delegate
                                   view:view
                                 config:config
                          networkConfig:netConf];
  if (self != nil) {
    requestingAd = NO;
  }
  return self;
}

- (void)getAd {
  requestingAd = YES;
  MMAdView *adView = [[MMAdView alloc] initWithFrame:kAdWhirlViewDefaultFrame];
  if ([adWhirlDelegate respondsToSelector:@selector(millennialMediaApIDString)]) {
    adView.apID = [adWhirlDelegate millennialMediaApIDString];
  }
  else {
    adView.apID = networkConfig.pubId;
  }
  adView.delegate = self;
  self.adNetworkView = adView;
  [adView release];
}

- (void)dealloc {
  [super dealloc];
}

#pragma mark MMAdViewDelegate methods

- (void)adRequestSucceeded {
  requestingAd = NO;
  // millennial ads are slightly taller than default frame, at 53 pixels.
  [self helperFitAdNetworkView];
  [adWhirlView adapter:self didReceiveAdView:adNetworkView];
}

- (void)adRequestFailed {
  if (!requestingAd) return;
  requestingAd = NO;
  [adWhirlView adapter:self didFailAd:nil];
}

- (void)adViewWillTakeOver:(MMAdView *)adView {
  [self helperNotifyDelegateOfFullScreenModal];
}

- (void)adViewWillReleaseControl:(MMAdView *)adView {
  [self helperNotifyDelegateOfFullScreenModalDismissal];
}

#pragma mark MMAdViewDelegate optional methods

- (BOOL)respondsToSelector:(SEL)selector {
  if (selector == @selector(latitude)
      && ![adWhirlDelegate respondsToSelector:@selector(locationInfo)]) {
    return NO;
  }
  else if (selector == @selector(longitude)
           && ![adWhirlDelegate respondsToSelector:@selector(locationInfo)]) {
    return NO;
  }
  else if (selector == @selector(age)
           && (!([adWhirlDelegate respondsToSelector:@selector(millennialMediaAge)]
                 || [adWhirlDelegate respondsToSelector:@selector(dateOfBirth)])
               || [self age] < 0)) {
    return NO;
  }
  else if (selector == @selector(zipCode)
           && ![adWhirlDelegate respondsToSelector:@selector(postalCode)]) {
    return NO;
  }
  else if (selector == @selector(sex)
           && ![adWhirlDelegate respondsToSelector:@selector(gender)]) {
    return NO;
  }
  else if (selector == @selector(householdIncome)
           && ![adWhirlDelegate respondsToSelector:@selector(incomeLevel)]) {
    return NO;
  }
  else if (selector == @selector(educationLevel)
           && ![adWhirlDelegate respondsToSelector:@selector(millennialMediaEducationLevel)]) {
    return NO;
  }
  else if (selector == @selector(ethnicity)
           && ![adWhirlDelegate respondsToSelector:@selector(millennialMediaEthnicity)]) {
    return NO;
  }
  return [super respondsToSelector:selector];
}

- (float)latitude {
  CLLocation *loc = [adWhirlDelegate locationInfo];
  if (loc == nil) return 0.0;
  return loc.coordinate.latitude;
}

- (float)longitude {
  CLLocation *loc = [adWhirlDelegate locationInfo];
  if (loc == nil) return 0.0;
  return loc.coordinate.longitude;
}

- (NSInteger)age {
  if ([adWhirlDelegate respondsToSelector:@selector(millennialMediaAge)]) {
    return [adWhirlDelegate millennialMediaAge];
  }
  return [self helperCalculateAge];
}

- (NSString*)zipCode {
  return [adWhirlDelegate postalCode];
}

- (MMSex)sex {
  NSString *gender = [adWhirlDelegate gender];
  MMSex sex = MMSexUnknown;
  if (gender == nil)
    return sex;
  if ([gender compare:@"m"] == NSOrderedSame) {
    sex = MMSexMale;
  }
  else if ([gender compare:@"f"] == NSOrderedSame) {
    sex = MMSexFemale;
  }
  return sex;
}

- (NSInteger)householdIncome {
  return (NSInteger)[adWhirlDelegate incomeLevel];
}

- (MMEducation)educationLevel {
  return [adWhirlDelegate millennialMediaEducationLevel];
}

- (MMEthnicity)ethnicity {
  return [adWhirlDelegate millennialMediaEthnicity];
}

@end
