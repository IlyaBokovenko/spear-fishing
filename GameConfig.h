//
//  GameConfig.h
//  Unity-iPhone
//
//  Created by Roman Spiryagin on 11/20/09.
//  Copyright 2009 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>


@interface GameConfig : NSObject {
}
+(bool)getBool:(NSString*)key;
+(NSString*)getString:(NSString*)key;
@end
