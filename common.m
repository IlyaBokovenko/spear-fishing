#import "common.h"

static NSDictionary* description(){
	static NSDictionary* gameDescription = nil;
	if(!gameDescription){
		NSString *mainConfigPath = [[NSBundle mainBundle] pathForResource:@"Game" ofType:@"plist"];
		gameDescription = [[NSDictionary dictionaryWithContentsOfFile:mainConfigPath] retain];		
	}
	
	return gameDescription;
}

BOOL g_isFreeVersion(){
	return [[description() objectForKey:@"IsFreeVersion"] boolValue];
}

NSString* g_adwhirlKey(){
	return [description() objectForKey:@"AdwhirlKey"];
}

NSString* g_productIdentifier() {
	return [description() objectForKey:@"ProductIdentifier"];
}
