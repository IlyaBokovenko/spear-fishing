/*

 AdWhirlConfig.h
 
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

#import "CJSONDeserializer.h"

@class AdWhirlConfig;
@protocol AdWhirlConfigDelegate<NSObject>

@optional
- (void)adWhirlConfigDidReceiveConfig:(AdWhirlConfig *)config;
- (void)adWhirlConfigDidFail:(NSError *)error;
- (NSURL *)adWhirlConfigURL;

@end

typedef enum {
  AWBannerAnimationTypeNone           = 0,
  AWBannerAnimationTypeFlipFromLeft   = 1,
  AWBannerAnimationTypeFlipFromRight  = 2,
  AWBannerAnimationTypeCurlUp         = 3,
  AWBannerAnimationTypeCurlDown       = 4,
  AWBannerAnimationTypeSlideFromLeft  = 5,
  AWBannerAnimationTypeSlideFromRight = 6,
  AWBannerAnimationTypeFadeIn         = 7,
  AWBannerAnimationTypeRandom         = 8,
} AWBannerAnimationType;

@class AdWhirlAdNetworkConfig;

@interface AdWhirlConfig : NSObject {
  NSString *appKey;
  BOOL legacy;

  BOOL adsAreOff;
  NSMutableArray *adNetworkConfigs;

  UIColor *backgroundColor;
  UIColor *textColor;
  NSTimeInterval refreshInterval;
  BOOL locationOn;
  AWBannerAnimationType bannerAnimationType;
  NSInteger fullscreenWaitInterval;
  NSInteger fullscreenMaxAds;
  
  NSMutableArray *delegates;
  NSURLConnection *connection;
  NSMutableData *receivedData;
  BOOL fetched;
}

+ (void)fetchConfig:(NSString *)appKey delegate:(id<AdWhirlConfigDelegate>)delegate;
+ (NSString *)uniqueId;

@property (nonatomic,readonly) NSString *appKey;
@property (nonatomic,readonly) BOOL adsAreOff;
@property (nonatomic,readonly) NSArray *adNetworkConfigs;
@property (nonatomic,readonly) UIColor *backgroundColor;
@property (nonatomic,readonly) UIColor *textColor;
@property (nonatomic,readonly) NSTimeInterval refreshInterval;
@property (nonatomic,readonly) BOOL locationOn;
@property (nonatomic,readonly) AWBannerAnimationType bannerAnimationType;
@property (nonatomic,readonly) NSInteger fullscreenWaitInterval;
@property (nonatomic,readonly) NSInteger fullscreenMaxAds;

@end

BOOL awIntVal(NSInteger *var, id val);

