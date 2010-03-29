//
//  UpgradeController.m
//  Unity-iPhone
//
//  Created by Roman Spiryagin on 11/20/09.
//  Copyright 2009 __MyCompanyName__. All rights reserved.
//

#import "UpgradeController.h"

@implementation UpgradeController

@synthesize upgradeState;

- (id) init {
	self = [super init];
	if(self != nil) {
		upgradeState = UPGRADE_NONE;
		[[SKPaymentQueue defaultQueue] addTransactionObserver:self];
	}
	return self;
}

- (void) TryUpgrade:(NSString *)productIdentifier {
	SKPayment *payment = [SKPayment paymentWithProductIdentifier:productIdentifier];
	[[SKPaymentQueue defaultQueue] addPayment:payment];
	upgradeState = UPGRADE_IN_PROCESS;
}

- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response {}
- (void)recordTransaction:(SKPaymentTransaction *)transaction {}
- (void)provideContent:(SKPaymentTransaction *)productIdentifier{}

- (void) failedTransaction:(SKPaymentTransaction *)transaction {
    if (transaction.error.code != SKErrorPaymentCancelled) {
		[error release];
		//error = [[NSString alloc] initWithString: [transaction.error localizedDescription]];
		error = [[transaction.error localizedDescription] retain]; 
		NSLog(@"Error: %@", error);
	}
	NSLog(@"Transaction failed!");
	self.upgradeState = UPGRADE_FAIL;
	[[SKPaymentQueue defaultQueue] finishTransaction: transaction];
}

- (void) restoreTransaction:(SKPaymentTransaction *)transaction {
    NSLog(@"Transaction restore!");
	self.upgradeState = UPGRADE_SUCCESS;
	[[SKPaymentQueue defaultQueue] finishTransaction: transaction];
}

- (void) completeTransaction:(SKPaymentTransaction *)transaction {
	// Your application should implement these two methods.
    [self recordTransaction: transaction];
    [self provideContent: transaction];
	NSLog(@"Transaction completed!");
	self.upgradeState = UPGRADE_SUCCESS;
	// Remove the transaction from the payment queue.
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
}

- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions {
	for (SKPaymentTransaction *transaction in transactions) {
		switch (transaction.transactionState) {
            case SKPaymentTransactionStatePurchased:
                [self completeTransaction:transaction];
                break;
            case SKPaymentTransactionStateFailed:
                [self failedTransaction:transaction];
                break;
            case SKPaymentTransactionStateRestored:
                [self restoreTransaction:transaction];
            default:
                break;
        }
    }
}

-(NSString*)getError {
	return error != nil ? error : @"";
}

- (void) dealloc
{
	[error release];
	[super dealloc];
}
@end
