/*
 
 AdWhirlDelegateProtocol.h
 
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

#import <CoreLocation/CoreLocation.h>
#import <UIKit/UIKit.h>

@class AdWhirlView;

@protocol AdWhirlDelegate<NSObject>

@required

- (NSString *)adWhirlApplicationKey;

@optional

/**
 * If you are running your own AdWhirl server instance, make sure you
 * implement the following to return the URL that points to the endpoints 
 * on your server.
 */
- (NSURL *)adWhirlConfigURL;
- (NSURL *)adWhirlImpMetricURL;
- (NSURL *)adWhirlClickMetricURL;
- (NSURL *)adWhirlCustomAdURL;

#pragma mark notifications

/**
 * You can listen to callbacks from AdWhirl via these methods.  When AdWhirl is
 * notified that an ad request is fulfilled, it will notify you immediately.
 * Thus, when notified that an ad request succeeded, you can choose to add the
 * AdWhirlView object as a subview to your view.  This view contains the ad.
 * When you are notified that an ad request failed, you are also informed if the
 * AdWhirlView is fetching a backup ad.  The backup fetching order is specified
 * by you in adwhirl.com or your own server instance.  When all backup sources
 * are attempted and the last ad request still fails, the usingBackup parameter
 * will be set to NO.  You can use this notification to try again and perhaps
 * request another AdWhirlView via [AdWhirlView requestAdWhirlViewWithDelegate:]
 */
- (void)adWhirlDidReceiveAd:(AdWhirlView *)adWhirlView;
- (void)adWhirlDidFailToReceiveAd:(AdWhirlView *)adWhirlView usingBackup:(BOOL)yesOrNo;

/**
 * This function is your integration point for Generic Notifications. You can
 * control when this notification occurs via the developers member section.  You
 * can allocate a percentage of your ad requests to initiate this callback.  When
 * you receive this notification, you can execute any code block that you own.
 * For example, you can replace the ad in AdWhirlView after getting this callback
 * by calling replaceBannerViewWith: . Note that the ad refresh cycle is still
 * alive, so your view could be replaced by other ads when it's time for an
 * ad refresh.
 */
- (void)adWhirlReceivedRequestForDeveloperToFufill:(AdWhirlView *)adWhirlView;

/**
 * In the event that ads are OFF, you can listen to this callback method to
 * determine that ads have been turned off.
 */
- (void)adWhirlReceivedNotificationAdsAreOff:(AdWhirlView *)adWhirlView;

/**
 * These notifications will let you know when a user is being shown a full screen
 * webview canvas with an ad because they tapped on an ad.  You should listen to
 * these notifications to determine when to pause/resume your game--if you're
 * building a game app.
 */
- (void)adWhirlWillPresentFullScreenModal;
- (void)adWhirlDidDismissFullScreenModal;

/**
 * An ad request is a two step process: first the SDK must go to the AdWhirl
 * server to retrieve configuration information. Then, based on the configuration
 * information, it chooses an ad network and fetch an ad. The following call
 * is for users to get notified when the first step is complete. The
 * adWhirlView passed could be null if you had called the AdWhirlView class
 * method +startPreFetchingConfigurationDataWithDelegate .
 */
- (void)adWhirlDidReceiveConfig:(AdWhirlView *)adWhirlView;


#pragma mark behavior configurations
- (BOOL)adWhirlTestMode; // request test ads for APIs that supports it


#pragma mark appearance configurations
- (UIColor *)adWhirlAdBackgroundColor;
- (UIColor *)adWhirlTextColor;
- (UIColor *)adWhirlSecondaryTextColor;
#pragma mark deprected
- (UIColor *)backgroundColor;
- (UIColor *)textColor;
#pragma mark deprected


#pragma mark hard-coded application keys
- (NSString *)admobPublisherID; // your Publisher ID from Admob.
- (NSDictionary *)quattroWirelessDictionary;  // key-value pairs for the keys "publisherID" and "siteID" provided by Quattro Wireless.  Set NSString values for these two keys.
- (NSString *)pinchApplicationKey; // your Application Code from Pinch Media.
- (NSDictionary *)videoEggConfigDictionary;  // key-value pairs for the keys "publisher" and "area" information from Video Egg.  Set NSString values for these two keys.
- (NSString *)millennialMediaApIDString;  // The ApID string from Millennial Media.


#pragma mark demographic information optional delegate methods
- (CLLocation *)locationInfo; // user's current location
- (NSString *)postalCode; // user's postal code, e.g. "94401"
- (NSString *)areaCode; // user's area code, e.g. "415"
- (NSDate *)dateOfBirth; // user's date of birth
- (NSString *)gender; // user's gender (e.g. @"m" or @"f")
- (NSString *)keywords; // keywords the user has provided or that are contextually relevant, e.g. @"twitter client iPhone"
- (NSString *)searchString; // a search string the user has provided, e.g. @"Jasmine Tea House San Francisco"
- (NSUInteger)incomeLevel; // return actual annual income


#pragma mark QuattroWireless-specific optional delegate methods
/**
 * Return a value for the education level if you have access to this info.  This
 * information will be relayed to Quattro Wireless if provided.
 * QWEducationNoCollege = 0
 * QWEducationCollegeGraduate = 1
 * QWEducationGraduateSchool = 2
 * QWEducationUnknown = 3
 */
- (NSUInteger)quattroWirelessEducationLevel;

/**
 * Return a value for the ethnicity if you have access to this info.  This
 * information will be relayed to Quattro Wireless if provided.
 * QWEthnicGroupAfrican_American = 0
 * QWEthnicGroupAsian = 1
 * QWEthnicGroupHispanic = 2
 * QWEthnicGroupWhite = 3
 * QWEthnicGroupOther = 4
 */
- (NSUInteger)quattroWirelessEthnicity;

#pragma mark MillennialMedia-specific optional delegate methods
/** 
 * Return a value for the education level if you have access to this info.  This
 * information will be relayed to Millennial Media if provided
 * MMEducationUnknown = 0,
 * MMEducationHishSchool = 1,
 * MMEducationSomeCollege = 2,
 * MMEducationInCollege = 3,
 * MMEducationBachelorsDegree = 4,
 * MMEducationMastersDegree = 5,
 * MMEducationPhD = 6
 */
- (NSUInteger)millennialMediaEducationLevel;

/** 
 * Return a value for ethnicity if you have access to this info.  This
 * information will be relayed to Millennial Media if provided.
 * MMEthnicityUnknown = 0,
 * MMEthnicityAfricanAmerican = 1,
 * MMEthnicityAsian = 2,
 * MMEthnicityCaucasian = 3,
 * MMEthnicityHispanic = 4,
 * MMEthnicityNativeAmerican = 5,
 * MMEthnicityMixed = 6
 */
- (NSUInteger)millennialMediaEthnicity;

#pragma mark deprecated (age is age...)
- (NSUInteger)millennialMediaAge;
#pragma mark

@end