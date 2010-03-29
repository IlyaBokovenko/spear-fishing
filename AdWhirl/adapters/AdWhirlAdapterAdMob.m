/*
 
 AdWhirlAdapterAdmob.m
 
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

#import "AdWhirlAdapterAdMob.h"
#import "AdWhirlAdNetworkConfig.h"
#import "AdWhirlView.h"
#import "AdMobView.h"
#import "AdWhirlLog.h"
#import "AdWhirlAdNetworkAdapter+Helpers.h"
#import "AdWhirlAdNetworkRegistry.h"

@implementation AdWhirlAdapterAdMob

+ (AdWhirlAdNetworkType)networkType {
  return AdWhirlAdNetworkTypeAdMob;
}

+ (void)load {
  [[AdWhirlAdNetworkRegistry sharedRegistry] registerClass:self];
}

- (void)getAd {
  AdMobView *adMobView = [AdMobView requestAdWithDelegate:self];
  self.adNetworkView = adMobView;
}

- (void)dealloc {
  [super dealloc];
}

#pragma mark AdMobDelegate notification methods

- (NSString *)publisherId {
  if ([adWhirlDelegate respondsToSelector:@selector(admobPublisherID)]) {
    return [adWhirlDelegate admobPublisherID];
  }
  return networkConfig.pubId;
}

- (void)didReceiveAd:(AdMobView *)adView {
  [self helperFitAdNetworkView];
  [adWhirlView adapter:self didReceiveAdView:adView];
}

- (void)didFailToReceiveAd:(AdMobView *)adView {
  [adWhirlView adapter:self didFailAd:nil];
}

- (void)willPresentFullScreenModal {
  [self helperNotifyDelegateOfFullScreenModal];
}

- (void)didDismissFullScreenModal {
  [self helperNotifyDelegateOfFullScreenModalDismissal];
}

#pragma mark AdMobDelegate config methods
- (UIColor *)adBackgroundColor {
  return [self helperBackgroundColorToUse];
}

- (UIColor *)primaryTextColor {
  return [self helperTextColorToUse];
}

- (UIColor *)secondaryTextColor {
  return [self helperSecondaryTextColorToUse];
}

- (BOOL)mayAskForLocation {
  return adWhirlConfig.locationOn;
}

- (BOOL)useTestAd {
  if ([adWhirlDelegate respondsToSelector:@selector(adWhirlTestMode)])
    return [adWhirlDelegate adWhirlTestMode];
  return NO;
}

#pragma mark AdMobDelegate optional config methods

- (BOOL)respondsToSelector:(SEL)selector {
  if (selector == @selector(location)
      && ![adWhirlDelegate respondsToSelector:@selector(locationInfo)]) {
    return NO;
  }
  else if (selector == @selector(postalCode)
           && ![adWhirlDelegate respondsToSelector:@selector(postalCode)]) {
    return NO;
  }
  else if (selector == @selector(areaCode)
           && ![adWhirlDelegate respondsToSelector:@selector(areaCode)]) {
    return NO;
  }
  else if (selector == @selector(dateOfBirth)
           && ![adWhirlDelegate respondsToSelector:@selector(dateOfBirth)]) {
    return NO;
  }
  else if (selector == @selector(gender)
           && ![adWhirlDelegate respondsToSelector:@selector(gender)]) {
    return NO;
  }
  else if (selector == @selector(keywords)
           && ![adWhirlDelegate respondsToSelector:@selector(keywords)]) {
    return NO;
  }
  return [super respondsToSelector:selector];
}

- (CLLocation *)location {
  return [adWhirlDelegate locationInfo];
}

- (NSString *)postalCode {
  return [adWhirlDelegate postalCode];
}

- (NSString *)areaCode {
  return [adWhirlDelegate areaCode];
}

- (NSDate *)dateOfBirth {
  return [adWhirlDelegate dateOfBirth];
}

- (NSString *)gender {
  return [adWhirlDelegate gender];
}

- (NSString *)keywords {
  return [adWhirlDelegate keywords];
}

@end
