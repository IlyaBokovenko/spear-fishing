/*

 AdWhirlAdapterJumpTap.m
 
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

#import "AdWhirlAdapterJumpTap.h"
#import "AdWhirlView.h"
#import "AdWhirlConfig.h"
#import "AdWhirlAdNetworkConfig.h"
#import "AdWhirlAdNetworkAdapter+Helpers.h"
#import "AdWhirlAdNetworkRegistry.h"


@implementation AdWhirlAdapterJumpTap

+ (AdWhirlAdNetworkType)networkType {
  return AdWhirlAdNetworkTypeJumpTap;
}

+ (void)load {
  [[AdWhirlAdNetworkRegistry sharedRegistry] registerClass:self];
}

- (void)getAd {
  JTAdWidget *widget = [[JTAdWidget alloc] initWithDelegate:self
                                         shouldStartLoading:YES];
  widget.frame = kAdWhirlViewDefaultFrame;
  widget.refreshInterval = 0; // do not self-refresh
  self.adNetworkView = widget;
}

- (void)dealloc {
  [super dealloc];
}

#pragma mark JTAdWidgetDelegate methods

- (NSString *)publisherId:(id)theWidget {
  return networkConfig.pubId;
}

- (BOOL)shouldRenderAd:(id)theWidget {
  [self helperFitAdNetworkView];
  [adWhirlView adapter:self didReceiveAdView:theWidget];
  return YES;
}

- (void)beginAdInteraction:(id)theWidget {
  [self helperNotifyDelegateOfFullScreenModal];
}

- (void)endAdInteraction:(id)theWidget {
  [self helperNotifyDelegateOfFullScreenModalDismissal];
}

- (void)adWidget:(id)theWidget didFailToShowAd:(NSError *)error {
  [adWhirlView adapter:self didFailAd:error];
}

- (void)adWidget:(id)theWidget didFailToRequestAd:(NSError *)error {
  [adWhirlView adapter:self didFailAd:error];
}

#pragma mark JTAdWidgetDelegate methods -Targeting
//- (NSString *)site:(id)theWidget;
//- (NSString *)adSpot:(id)theWidget;
//- (NSString *)query:(id)theWidget;
//- (NSString *)category:(id)theWidget;
//- (AdultContent)adultContent:(id)theWidget;

#pragma mark JTAdWidgetDelegate methods -General Configuration

- (NSDictionary*)extraParameters:(id)theWidget {
  NSMutableDictionary *dict = [NSMutableDictionary dictionaryWithCapacity:10];
  if ([adWhirlDelegate respondsToSelector:@selector(dateOfBirth)]) {
    NSInteger age = [self helperCalculateAge];
    if (age >= 0)
      [dict setObject:[NSString stringWithFormat:@"%d",age] forKey:@"mt-age"];
  }
  if ([adWhirlDelegate respondsToSelector:@selector(gender)]) {
    NSString *gender = [adWhirlDelegate gender];
    if (gender != nil)
      [dict setObject:gender forKey:@"mt-gender"];
  }
  if ([adWhirlDelegate respondsToSelector:@selector(incomeLevel)]) {
    NSUInteger income = [adWhirlDelegate incomeLevel];
    NSString *level = nil;
    if (income < 15000) {
      level = @"000_015";
    }
    else if (income < 20000) {
      level = @"015_020";
    }
    else if (income < 30000) {
      level = @"020_030";
    }
    else if (income < 40000) {
      level = @"030_040";
    }
    else if (income < 50000) {
      level = @"040_050";
    }
    else if (income < 75000) {
      level = @"050_075";
    }
    else if (income < 100000) {
      level = @"075_100";
    }
    else if (income < 125000) {
      level = @"100_125";
    }
    else if (income < 150000) {
      level = @"125_150";
    }
    else {
      level = @"150_OVER";
    }
    [dict setObject:level forKey:@"mt-hhi"];
  }
  return dict;
}

- (UIColor *)adBackgroundColor:(id)theWidget {
  return [self helperBackgroundColorToUse];
}

- (UIColor *)adForegroundColor:(id)theWidget {
  return [self helperTextColorToUse];
}

#pragma mark JTAdWidgetDelegate methods -Location Configuration

- (BOOL)allowLocationUse:(id)theWidget {
  return adWhirlConfig.locationOn;
}

- (CLLocation*)location:(id)theWidget {
  if (![adWhirlDelegate respondsToSelector:@selector(locationInfo)]) {
    return nil;
  }
  return [adWhirlDelegate locationInfo];
}

#pragma mark JTAdWidgetDelegate methods -Ad Display and User Interaction
// The ad orientation changed
//- (void)adWidget:(id)theWidget orientationHasChangedTo:(UIInterfaceOrientation)interfaceOrientation;

// Language methods
//- (NSString*)getPlayVideoPrompt:(id)theWidget;
//- (NSString*)getBackButtonPrompt:(id)theWidget isInterstitial:(BOOL)isInterstitial;
//- (NSString*)getSafariButtonPrompt:(id)theWidget;

@end
