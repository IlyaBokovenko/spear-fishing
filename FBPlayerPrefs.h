#define DEFAULT_PREFS_FILE @"DefaultPlayerPrefs.plist"

@interface FBPlayerPrefs : NSObject
{
}

+ (void)setInt:(int)value withKey:(NSString *)key;
+ (void)setFloat:(float)value withKey:(NSString *)key;
+ (void)setString:(NSString *)value withKey:(NSString *)key;

+ (int)getInt:(NSString *)key orDefault:(int)value;
+ (float)getFloat:(NSString *)key orDefault:(float)value;
+ (NSString *)getString:(NSString *)key orDefault:(NSString *)value;

+ (BOOL)hasKey:(NSString *)key;
+ (void)deleteKey:(NSString *)key;

+ (void)readPrefsFile;
+ (void)saveAndUnload;
@end
