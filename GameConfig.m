//
//  GameConfig.m
//  Unity-iPhone
//
//  Created by Roman Spiryagin on 11/20/09.
//  Copyright 2009 __MyCompanyName__. All rights reserved.
//

#import "GameConfig.h"


@implementation GameConfig

+(bool)getBool:(NSString*)key {
	NSDictionary *gameConfig;
	NSString *mainConfigPath = [[NSBundle mainBundle] pathForResource:@"Game" ofType:@"plist"];
	gameConfig = [NSDictionary dictionaryWithContentsOfFile:mainConfigPath];
	return [[gameConfig objectForKey:key] boolValue]; 
}

+(NSString*)getString:(NSString*)key {
	NSDictionary *gameConfig;
	NSString *mainConfigPath = [[NSBundle mainBundle] pathForResource:@"Game" ofType:@"plist"];
	gameConfig = [NSDictionary dictionaryWithContentsOfFile:mainConfigPath];
	return [gameConfig objectForKey:key];
}

@end
