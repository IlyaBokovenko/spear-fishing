//
//  UpgradeController.h
//  Unity-iPhone
//
//  Created by Roman Spiryagin on 11/20/09.
//  Copyright 2009 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

typedef enum {
	UPGRADE_NONE, UPGRADE_FAIL, UPGRADE_SUCCESS, UPGRADE_IN_PROCESS
} UpgradeState;

@interface UpgradeController : NSObject <SKPaymentTransactionObserver, SKProductsRequestDelegate>{
	NSInteger upgradeState;
	NSString *error;
}

@property(nonatomic) NSInteger upgradeState;
-(void)TryUpgrade:(NSString *)productIdentifier;
-(NSString*)getError;
@end
