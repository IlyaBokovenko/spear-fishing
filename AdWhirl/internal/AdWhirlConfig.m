/*

 AdWhirlConfig.m
 
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

#import <CommonCrypto/CommonDigest.h>

#import "AdWhirlConfig.h"
#import "AdWhirlError.h"
#import "AdWhirlAdNetworkConfig.h"
#import "AdWhirlLog.h"
#import "AdWhirlView.h"
#import "AdWhirlAdNetworkAdapter.h"

@interface AdWhirlConfig ()

- (id)initWithAppKey:(NSString *)ak delegate:(id<AdWhirlConfigDelegate>)delegate;
- (void)addDelegate:(id<AdWhirlConfigDelegate>)delegate;
- (void)notifyDelegatesOfFailure:(NSError *)error;

@property BOOL fetched;

@end

BOOL awIntVal(NSInteger *var, id val) {
  if ([val isKindOfClass:[NSNumber class]] || [val isKindOfClass:[NSString class]]) {
    *var = [val integerValue];
    return YES;
  }
  return NO;
}

@interface UIColor (AdWhirlConfig)

- (id)initWithDict:(NSDictionary *)dict;

@end

@implementation UIColor (AdWhirlConfig)

- (id)initWithDict:(NSDictionary *)dict {
  id red, green, blue, alpha;
  CGFloat r, g, b, a;

  red   = [dict objectForKey:@"red"];
  if (red == nil)   { [self release]; return nil; }
  green = [dict objectForKey:@"green"];
  if (green == nil) { [self release]; return nil; }
  blue  = [dict objectForKey:@"blue"];
  if (blue == nil)  { [self release]; return nil; }

  NSInteger temp;
  if (!awIntVal(&temp, red))  { [self release]; return nil; }
  r = (CGFloat)temp/255.0;
  if (!awIntVal(&temp, green)){ [self release]; return nil; }
  g = (CGFloat)temp/255.0;
  if (!awIntVal(&temp, blue)) { [self release]; return nil; }
  b = (CGFloat)temp/255.0;

  alpha = [dict objectForKey:@"alpha"];
  if (alpha != nil && awIntVal(&temp, alpha)) a = (CGFloat)temp;
  else a = 1.0;

  return [self initWithRed:r green:g blue:b alpha:a];
}

@end


@implementation AdWhirlConfig

@synthesize appKey;
@synthesize adsAreOff;
@synthesize adNetworkConfigs;
@synthesize backgroundColor;
@synthesize textColor;
@synthesize refreshInterval;
@synthesize locationOn;
@synthesize bannerAnimationType;
@synthesize fullscreenWaitInterval;
@synthesize fullscreenMaxAds;
@synthesize fetched;

+ (void)fetchConfig:(NSString *)appKey delegate:(id<AdWhirlConfigDelegate>)delegate {
  static NSMutableDictionary *configs = nil;

  if (configs == nil) {
    configs = [[NSMutableDictionary alloc] init];
  }

  AdWhirlConfig *config = [configs objectForKey:appKey];
  if (config != nil) {
    if (config.fetched) {
      [delegate adWhirlConfigDidReceiveConfig:config];
      return;
    }
    [config addDelegate:delegate];
    return;
  }
  
  config = [[AdWhirlConfig alloc] initWithAppKey:appKey delegate:delegate];
  [configs setObject:config forKey:appKey];
  [config release];
}

+ (NSString *)uniqueId {
  static NSString *uid = nil;
  if (uid) return uid;

  // avoid sending UDID over the network in the clear
  NSString *udid = [[UIDevice currentDevice] uniqueIdentifier];
  NSData *udidData = [udid dataUsingEncoding:NSUTF8StringEncoding];
  unsigned char md[CC_MD5_DIGEST_LENGTH];
  CC_MD5_CTX ctx;
  CC_MD5_Init(&ctx);
  CC_MD5_Update(&ctx, [udidData bytes], [udidData length]);
  CC_MD5_Update(&ctx, "AdWhirl", 7);
  CC_MD5_Final(md, &ctx);
  uid = [NSString stringWithFormat:(@"%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X"),
         md[ 0],md[ 1],md[ 2],md[ 3],
         md[ 4],md[ 5],md[ 6],md[ 7],
         md[ 8],md[ 9],md[10],md[11],
         md[12],md[13],md[14],md[15]];
  [uid retain];
  return uid;
}

- (id)initWithAppKey:(NSString *)ak delegate:(id<AdWhirlConfigDelegate>)delegate {
  self = [super init];
  if (self != nil) {
    appKey = [[NSString alloc] initWithString:ak];
    legacy = NO;
    adNetworkConfigs = [[NSMutableArray alloc] init];
    delegates = [[NSMutableArray alloc] init];
    fetched = NO;
    [self addDelegate:delegate];
    NSURL *configBaseURL = nil;
    if ([delegate respondsToSelector:@selector(adWhirlConfigURL)]) {
      configBaseURL = [delegate adWhirlConfigURL];
    }
    if (configBaseURL == nil) {
      configBaseURL = [NSURL URLWithString:kAdWhirlDefaultConfigURL];
    }
    NSURL *configURL = [NSURL URLWithString:[NSString stringWithFormat:@"?appid=%@&uuid=%@&appver=%d",
                                             appKey,
                                             [AdWhirlConfig uniqueId],
                                             kAdWhirlAppVer]
                              relativeToURL:configBaseURL];
    AWLogDebug(@"Fetching config at %@", configURL);
    NSURLRequest *configRequest = [NSURLRequest requestWithURL:configURL];
    connection = [[NSURLConnection alloc] initWithRequest:configRequest
                                                 delegate:self];
    if (connection) {
      receivedData = [[NSMutableData alloc] init];
    }

    // default values
    backgroundColor = [[UIColor alloc] initWithRed:0.3 green:0.3 blue:0.3 alpha:1.0];
    textColor = [[UIColor whiteColor] retain];
    refreshInterval = 60;
    locationOn = YES;
    bannerAnimationType = AWBannerAnimationTypeRandom;
    fullscreenWaitInterval = 60;
    fullscreenMaxAds = 2;
  }
  return self;
}

- (void)dealloc {
  [appKey release], appKey = nil;
  [adNetworkConfigs release], adNetworkConfigs = nil;
  [backgroundColor release], backgroundColor = nil;
  [textColor release], textColor = nil;
  [delegates release], delegates = nil;
  if (connection != nil) {
    [connection cancel];
    [connection release], connection = nil;
  }
  [receivedData release], receivedData = nil;
  [super dealloc];
}

- (void)addDelegate:(id<AdWhirlConfigDelegate>)delegate {
  [delegates addObject:delegate];
}

- (NSString *)description {
  NSString *desc = [super description];
  NSString *configs = [NSString stringWithFormat:
                       @"location_access:%d fg_color:%@ bg_color:%@ cycle_time:%lf transition:%d",
                       locationOn, textColor, backgroundColor, refreshInterval, bannerAnimationType];
  return [NSString stringWithFormat:@"%@:\n%@ networks:%@",desc,configs,adNetworkConfigs];
}

- (BOOL)parseExtraConfig:(NSDictionary *)configDict error:(NSError **)error {
  id bgColor = [configDict objectForKey:@"background_color_rgb"];
  if (bgColor != nil && [bgColor isKindOfClass:[NSDictionary class]]) {
    [backgroundColor release];
    backgroundColor = [[UIColor alloc] initWithDict:(NSDictionary *)bgColor];
  }
  id txtColor = [configDict objectForKey:@"text_color_rgb"];
  if (txtColor != nil && [txtColor isKindOfClass:[NSDictionary class]]) {
    [textColor release];
    textColor = [[UIColor alloc] initWithDict:txtColor];
  }
  id tempVal;
  tempVal = [configDict objectForKey:@"refresh_interval"];
  if (tempVal == nil)
    tempVal = [configDict objectForKey:@"cycle_time"];
  NSInteger tempInt;
  if (tempVal && awIntVal(&tempInt, tempVal)) {
    refreshInterval = (NSTimeInterval)tempInt;
    if (refreshInterval >= 30000.0) {
      // effectively forever, set to 0
      refreshInterval = 0.0;
    }
  }
  if (awIntVal(&tempInt, [configDict objectForKey:@"location_on"])) {
    locationOn = (tempInt == 0)? NO : YES;
  }
  tempVal = [configDict objectForKey:@"transition"];
  if (tempVal == nil)
    tempVal = [configDict objectForKey:@"banner_animation_type"];
  if (tempVal && awIntVal(&tempInt, tempVal)) {
    switch (tempInt) {
      case 0: bannerAnimationType = AWBannerAnimationTypeNone; break;
      case 1: bannerAnimationType = AWBannerAnimationTypeFlipFromLeft; break;
      case 2: bannerAnimationType = AWBannerAnimationTypeFlipFromRight; break;
      case 3: bannerAnimationType = AWBannerAnimationTypeCurlUp; break;
      case 4: bannerAnimationType = AWBannerAnimationTypeCurlDown; break;
      case 5: bannerAnimationType = AWBannerAnimationTypeSlideFromLeft; break;
      case 6: bannerAnimationType = AWBannerAnimationTypeSlideFromRight; break;
      case 7: bannerAnimationType = AWBannerAnimationTypeFadeIn; break;
      case 8: bannerAnimationType = AWBannerAnimationTypeRandom; break;
    }
  }
  if (awIntVal(&tempInt, [configDict objectForKey:@"fullscreen_wait_interval"])) {
    fullscreenWaitInterval = tempInt;
  }
  if (awIntVal(&tempInt, [configDict objectForKey:@"fullscreen_max_ads"])) {
    fullscreenMaxAds = tempInt;
  }
  return YES;
}

- (BOOL)parseLegacyConfig:(NSArray *)configArray error:(NSError **)error {
  NSMutableDictionary *adNetConfigDicts = [[NSMutableDictionary alloc] init];
  for (int i = 0; i < [configArray count]; i++) {
    id configObj = [configArray objectAtIndex:i];
    if (![configObj isKindOfClass:[NSDictionary class]]) {
      if (error != nil)
        *error = [AdWhirlError errorWithCode:AdWhirlConfigDataError
                                 description:@"Expected dictionary in config data"];
      [adNetConfigDicts release];
      return NO;
    }
    NSDictionary *configDict = (NSDictionary *)configObj;
    switch (i) {
      case 0:
        // ration map
      case 1:
        // key map
      case 2:
        // priority map
        for (id key in [configDict keyEnumerator]) {
          // format: "<network name>_<value name>" e.g. "admob_ration"
          NSString *strKey = (NSString *)key;
          if ([strKey compare:@"empty_ration"] == NSOrderedSame) {
            NSInteger empty_ration;
            if (awIntVal(&empty_ration, [configDict objectForKey:key]) && empty_ration == 100) {
              adsAreOff = YES;
              [adNetConfigDicts release];
              return YES;
            }
          }
          adsAreOff = NO;
          NSRange underScorePos = [strKey rangeOfString:@"_" options:NSBackwardsSearch];
          if (underScorePos.location == NSNotFound) {
            if (error != nil)
              *error = [AdWhirlError errorWithCode:AdWhirlConfigDataError
                                       description:[NSString stringWithFormat:
                                                    @"Expected underscore delimiter in key '%@'", strKey]];
            [adNetConfigDicts release];
            return NO;
          }
          NSString *networkName = [strKey substringToIndex:underScorePos.location];
          NSString *valueName = [strKey substringFromIndex:(underScorePos.location+1)];
          if ([networkName length] == 0) {
            if (error != nil)
              *error = [AdWhirlError errorWithCode:AdWhirlConfigDataError
                                       description:[NSString stringWithFormat:
                                                    @"Empty ad network name in key '%@'", strKey]];
            [adNetConfigDicts release];
            return NO;
          }
          if ([valueName length] == 0) {
            if (error != nil)
              *error = [AdWhirlError errorWithCode:AdWhirlConfigDataError
                                       description:[NSString stringWithFormat:
                                                    @"Empty value name in key '%@'", strKey]];
            [adNetConfigDicts release];
            return NO;
          }
          if ([networkName compare:@"dontcare"] == NSOrderedSame) {
            continue;
          }
          NSMutableDictionary *adNetConfigDict = [adNetConfigDicts objectForKey:networkName];
          if (adNetConfigDict == nil) {
            adNetConfigDict = [[NSMutableDictionary alloc] init];
            [adNetConfigDicts setObject:adNetConfigDict forKey:networkName];
            [adNetConfigDict release];
            adNetConfigDict = [adNetConfigDicts objectForKey:networkName];
          }
          NSString *properValueName;
          if ([valueName compare:@"ration"] == NSOrderedSame) {
            properValueName = AWAdNetworkConfigKeyWeight;
          }
          else if ([valueName compare:@"key"] == NSOrderedSame) {
            properValueName = AWAdNetworkConfigKeyCred;
          }
          else if ([valueName compare:@"priority"] == NSOrderedSame) {
            properValueName = AWAdNetworkConfigKeyPriority;
          }
          else {
            properValueName = valueName;
          }
          [adNetConfigDict setObject:[configDict objectForKey:key]
                              forKey:properValueName];
        }
        break; // ad network config maps

      case 3:
        // general config map
        if (![self parseExtraConfig:configDict error:error]) {
          return NO;
        }
        break; // general config map
      default:
        AWLogWarn(@"Ignoring element at index %d in legacy config", i);
        break;
    } // switch (i)
  } // loop configArray
  
  // adwhirl_ special handling
  NSMutableDictionary *adRolloConfig = [adNetConfigDicts objectForKey:@"adrollo"];
  if (adRolloConfig != nil) {
    AWLogDebug(@"Processing AdRollo config %@", adRolloConfig);
    NSMutableArray *adWhirlNetworkConfigs = [[NSMutableArray alloc] init];;
    for (NSString *netname in [adNetConfigDicts keyEnumerator]) {
      if (![netname hasPrefix:@"adwhirl_"]) continue;
      [adWhirlNetworkConfigs addObject:[adNetConfigDicts objectForKey:netname]];
    }
    if ([adWhirlNetworkConfigs count] > 0) {
      // split the ration evenly, use same credentials
      NSInteger ration = [[adRolloConfig objectForKey:AWAdNetworkConfigKeyWeight] integerValue];
      ration = ration/[adWhirlNetworkConfigs count];
      for (NSMutableDictionary *cd in adWhirlNetworkConfigs) {
        [cd setObject:[NSNumber numberWithInteger:ration]
               forKey:AWAdNetworkConfigKeyWeight];
        [cd setObject:[adRolloConfig objectForKey:AWAdNetworkConfigKeyCred]
               forKey:AWAdNetworkConfigKeyCred];
      }
    }
    [adWhirlNetworkConfigs release];
  }

  NSInteger totalWeight = 0;
  for (id networkName in [adNetConfigDicts keyEnumerator]) {
    NSString *netname = (NSString *)networkName;
    if ([netname compare:@"adrollo"] == NSOrderedSame) {
      // skip adrollo, was used for "adwhirl_" networks
      continue;
    }
    NSMutableDictionary *adNetConfigDict = [adNetConfigDicts objectForKey:netname];
    
    // set network type for legacy
    NSInteger networkType = 0;
    if ([netname compare:@"admob"] == NSOrderedSame) {
      networkType = AdWhirlAdNetworkTypeAdMob;
    }
    else if ([netname compare:@"jumptap"] == NSOrderedSame) {
      networkType = AdWhirlAdNetworkTypeJumpTap;
    }
    else if ([netname compare:@"videoegg"] == NSOrderedSame) {
      networkType = AdWhirlAdNetworkTypeVideoEgg;
    }
    else if ([netname compare:@"medialets"] == NSOrderedSame) {
      networkType = AdWhirlAdNetworkTypeMedialets;
    }
    else if ([netname compare:@"liverail"] == NSOrderedSame) {
      networkType = AdWhirlAdNetworkTypeLiveRail;
    }
    else if ([netname compare:@"millennial"] == NSOrderedSame) {
      networkType = AdWhirlAdNetworkTypeMillennial;
    }
    else if ([netname compare:@"greystripe"] == NSOrderedSame) {
      networkType = AdWhirlAdNetworkTypeGreyStripe;
    }
    else if ([netname compare:@"quattro"] == NSOrderedSame) {
      networkType = AdWhirlAdNetworkTypeQuattro;
    }
    else if ([netname compare:@"custom"] == NSOrderedSame) {
      networkType = AdWhirlAdNetworkTypeCustom;
    }
    else if ([netname compare:@"adwhirl_10"] == NSOrderedSame) {
      networkType = AdWhirlAdNetworkTypeAdWhirl10;
    }
    else if ([netname compare:@"mobclix"] == NSOrderedSame) {
      networkType = AdWhirlAdNetworkTypeMobClix;
    }
    else if ([netname compare:@"adwhirl_12"] == NSOrderedSame) {
      networkType = AdWhirlAdNetworkTypeAdWhirl12;
    }
    else if ([netname compare:@"adwhirl_13"] == NSOrderedSame) {
      networkType = AdWhirlAdNetworkTypeAdWhirl13;
    }
    else if ([netname compare:@"google_adsense"] == NSOrderedSame) {
      networkType = AdWhirlAdNetworkTypeGoogleAdSense;
    }
    else if ([netname compare:@"google_doubleclick"] == NSOrderedSame) {
      networkType = AdWhirlAdNetworkTypeGoogleDoubleClick;
    }
    else if ([netname compare:@"generic"] == NSOrderedSame) {
      networkType = AdWhirlAdNetworkTypeGeneric;
    }
    else {
      AWLogWarn(@"Unrecognized ad network '%@' in legacy config, ignored", netname);
      continue;
    }
    
    [adNetConfigDict setObject:netname forKey:AWAdNetworkConfigKeyName];
    [adNetConfigDict setObject:[NSString stringWithFormat:@"%d", networkType]
                        forKey:AWAdNetworkConfigKeyNID];
    [adNetConfigDict setObject:[NSNumber numberWithInteger:networkType]
                        forKey:AWAdNetworkConfigKeyType];
    
    AdWhirlAdNetworkConfig *adNetConfig = [[AdWhirlAdNetworkConfig alloc] initWithDictionary:adNetConfigDict];
    if (adNetConfig != nil) {
      [adNetworkConfigs addObject:adNetConfig];
      totalWeight += adNetConfig.trafficPercentage;
      [adNetConfig release];
    }
    else {
      AWLogWarn(@"Cannot create ad network config from %@", adNetConfigDict);
    }
  } // for each ad network name

  if (totalWeight == 0) {
    adsAreOff = YES;
  }

  [adNetConfigDicts release];
  return YES;
}

- (BOOL)parseNewConfig:(NSDictionary *)configDict error:(NSError **)error {
  id extra = [configDict objectForKey:@"extra"];
  if (extra != nil && [extra isKindOfClass:[NSDictionary class]]) {
    NSDictionary *extraDict = extra;
    if (![self parseExtraConfig:extraDict error:error]) {
      return NO;
    }
  }
  else {
    AWLogWarn(@"No extra info dict in ad network config");
  }

  id rations = [configDict objectForKey:@"rations"];
  NSInteger totalWeight = 0;
  if (rations != nil && [rations isKindOfClass:[NSArray class]]) {
    if ([(NSArray *)rations count] == 0) {
      adsAreOff = YES;
      return YES;
    }
    adsAreOff = NO;
    for (id c in (NSArray *)rations) {
      if (![c isKindOfClass:[NSDictionary class]]) {
        AWLogWarn(@"Element in rations array is not a dictionary %@ in ad network config",c);
        continue;
      }
      AdWhirlAdNetworkConfig *adNetConfig =
        [[AdWhirlAdNetworkConfig alloc] initWithDictionary:(NSDictionary *)c];
      if (adNetConfig != nil) {
        [adNetworkConfigs addObject:adNetConfig];
        totalWeight += adNetConfig.trafficPercentage;
        [adNetConfig release];
      }
      else {
        AWLogWarn(@"Cannot create ad network config from %@", c);
      }
    }
  }
  else {
    AWLogError(@"No rations array in ad network config");
  }

  if (totalWeight == 0) {
    adsAreOff = YES;
  }
  
  return YES;
}

- (BOOL)parseConfig:(NSData *)data error:(NSError **)error {
  NSError *jsonError;
  id parsed = [[CJSONDeserializer deserializer] deserialize:data error:&jsonError];
  if (parsed == nil) {
    if (error != nil)
      *error = [AdWhirlError errorWithCode:AdWhirlConfigParseError
                               description:@"Error parsing config JSON from server"
                           underlyingError:jsonError];
    return NO;
  }
  if ([parsed isKindOfClass:[NSArray class]]) {
    // pre-open-source AdWhirl/AdRollo config
    legacy = YES;
    if (![self parseLegacyConfig:(NSArray *)parsed error:error]) {
      return NO;
    }
  }
  else if ([parsed isKindOfClass:[NSDictionary class]]) {
    // open-source AdWhirl config
    if (![self parseNewConfig:(NSDictionary *)parsed error:error]) {
      return NO;
    }
  }
  else {
    if (error != nil)
      *error = [AdWhirlError errorWithCode:AdWhirlConfigDataError
                               description:@"Expected top-level dictionary in config data"];
    return NO;
  }
  return YES;
}

- (void)notifyDelegatesOfFailure:(NSError *)error {
  for (id<AdWhirlConfigDelegate> delegate in delegates) {
    if ([delegate respondsToSelector:@selector(adWhirlConfigDidFail:)]) {
      [delegate adWhirlConfigDidFail:error];
    }
  }
}

#pragma mark NSURLConnection delegate methods.

- (void)connection:(NSURLConnection *)conn didReceiveResponse:(NSURLResponse *)response {
  if (conn != connection) return;
  [receivedData setLength:0];
}

- (void)connection:(NSURLConnection *)conn didFailWithError:(NSError *)error {
  if (conn != connection) return;
  [self notifyDelegatesOfFailure:[AdWhirlError errorWithCode:AdWhirlConfigConnectionError
                                                 description:@"Error connecting to config server"
                                             underlyingError:error]];
  [receivedData release], receivedData = nil;
}

- (void)connectionDidFinishLoading:(NSURLConnection *)conn {
  if (conn != connection) return;
  NSError *error;
  if ([self parseConfig:receivedData error:&error]) {
    self.fetched = YES;
    // notify delegates of success
    for (id<AdWhirlConfigDelegate> delegate in delegates) {
      if ([delegate respondsToSelector:@selector(adWhirlConfigDidReceiveConfig:)]) {
        [delegate adWhirlConfigDidReceiveConfig:self];
      }
    }
  }
  else {
    [self notifyDelegatesOfFailure:error];
  }
  [receivedData release], receivedData = nil;
}

- (void)connection:(NSURLConnection *)conn didReceiveData:(NSData *)data {
  if (conn != connection) return;
  [receivedData appendData:data];
}

@end
