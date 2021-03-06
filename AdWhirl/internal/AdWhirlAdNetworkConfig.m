/*

 AdNetwork.m

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

#import "AdWhirlAdNetworkConfig.h"
#import "AdWhirlConfig.h"
#import "AdWhirlAdNetworkRegistry.h"
#import "AdWhirlLog.h"

#define kAdWhirlPubIdKey @"pubid"

@implementation AdWhirlAdNetworkConfig

@synthesize networkType;
@synthesize nid;
@synthesize networkName;
@synthesize trafficPercentage;
@synthesize priority;
@synthesize credentials;
@synthesize adapterClass;

- (id)initWithDictionary:(NSDictionary *)adNetConfigDict {
  self = [super init];

  if (self != nil) {
    NSInteger temp;
    id ntype = [adNetConfigDict objectForKey:AWAdNetworkConfigKeyType];
    id netId = [adNetConfigDict objectForKey:AWAdNetworkConfigKeyNID];
    id netName = [adNetConfigDict objectForKey:AWAdNetworkConfigKeyName];
    id weight = [adNetConfigDict objectForKey:AWAdNetworkConfigKeyWeight];
    id pri = [adNetConfigDict objectForKey:AWAdNetworkConfigKeyPriority];

    if (ntype == nil || netId == nil || netName == nil || pri == nil) {
      AWLogWarn(@"Ad network config has no network type, network id, network name, or priority");
      [self release];
      return nil;
    }
    
    if (awIntVal(&temp, ntype)) {
      networkType = temp;
      adapterClass = [[AdWhirlAdNetworkRegistry sharedRegistry] adapterClassFor:networkType];
      if (adapterClass == nil) {
        AWLogWarn(@"Ad network type %d not supported, no adapter found", networkType);
        [self release];
        return nil;
      }
    }
    if ([netId isKindOfClass:[NSString class]]) {
      nid = [[NSString alloc] initWithString:netId];
    }
    if ([netName isKindOfClass:[NSString class]]) {
      networkName = [[NSString alloc] initWithString:netName];
    }
    if (weight == nil) {
      trafficPercentage = 0;
    }
    else if (awIntVal(&temp, weight)) {
      trafficPercentage = temp;
    }
    if (awIntVal(&temp, pri)) {
      priority = temp;
    }
    
    if (networkType == 0 || nid == nil || networkName == nil || priority == 0) {
      AWLogWarn(@"Ad network config has invalid network type, network id, network name or priority");
      [self release];
      return nil;
    }

    id cred = [adNetConfigDict objectForKey:AWAdNetworkConfigKeyCred];
    if (cred == nil) {
      credentials = nil;
    }
    else {
      if ([cred isKindOfClass:[NSDictionary class]]) {
        credentials = [[NSDictionary alloc] initWithDictionary:cred copyItems:YES];
      }
      else if ([cred isKindOfClass:[NSString class]]) {
        credentials = [[NSDictionary alloc] initWithObjectsAndKeys:
                       [NSString stringWithString:cred], kAdWhirlPubIdKey,
                       nil];
      }
    }
  } 
  
  return self;
}

- (NSString *)pubId {
  if (credentials == nil) return nil;
  return [credentials objectForKey:kAdWhirlPubIdKey];
}

- (NSString *)description {
  NSString *creds = [self pubId];
  if (creds == nil) {
    creds = @"{";
    for (NSString *k in [credentials keyEnumerator]) {
      creds = [creds stringByAppendingFormat:@"%@:%@ ",
               k, [credentials objectForKey:k]];
    }
    creds = [creds stringByAppendingString:@"}"];
  }
  return [NSString stringWithFormat:
          @"name:%@ type:%d nid:%@ weight:%d priority:%d creds:%@",
          networkName, networkType, nid, trafficPercentage, priority, creds];
}

- (void)dealloc {
  [nid release], nid = nil;
  [networkName release], networkName = nil;
  [credentials release], credentials = nil;
  
  [super dealloc];
}

@end
