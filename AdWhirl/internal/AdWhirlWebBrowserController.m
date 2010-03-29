/*

 AdWhirlWebBrowserController.m
 
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

#import "AdWhirlWebBrowserController.h"
#import "AdWhirlLog.h"

#define kAWWebViewAnimDuration 1.0

@interface AdWhirlWebBrowserController ()
@property (nonatomic,retain) NSArray *loadingButtons;
@property (nonatomic,retain) NSArray *loadedButtons;
@end


@implementation AdWhirlWebBrowserController

@synthesize delegate;
@synthesize loadingButtons;
@synthesize loadedButtons;
@synthesize backButton;
@synthesize forwardButton;
@synthesize reloadButton;
@synthesize stopButton;
@synthesize linkOutButton;
@synthesize closeButton;


- (id)init {
    if (self = [super initWithNibName:@"AdWhirlWebBrowser" bundle:nil]) {
    }
    return self;
}

- (void)viewWillAppear:(BOOL)animated {
	[super viewWillAppear:animated];
	if (self.webView.request) {
    // has content from before, clear by creating another UIWebView
    CGRect frame = self.webView.frame;
    NSInteger tag = self.webView.tag;
    UIWebView *newView = [[UIWebView alloc] initWithFrame:frame];
    newView.tag = tag;
    UIWebView *oldView = self.webView;
    [oldView removeFromSuperview];
    [self.view addSubview:newView];
    newView.delegate = self;
    newView.scalesPageToFit = YES;
    [newView release];
	}
  self.toolBar.items = self.loadedButtons;
//  self.navigationItem.title = @"";
}

- (void)viewDidLoad {
  [super viewDidLoad];
  NSArray *items = self.toolBar.items;
  
  NSMutableArray *loadingItems = [[NSMutableArray alloc] init];
  [loadingItems addObjectsFromArray:items];
  [loadingItems removeObjectAtIndex:4];
  self.loadingButtons = loadingItems;
  [loadingItems release], loadingItems = nil;
  
  NSMutableArray *loadedItems = [[NSMutableArray alloc] init];
  [loadedItems addObjectsFromArray:items];
  [loadedItems removeObjectAtIndex:5];
  self.loadedButtons = loadedItems;
  [loadedItems release], loadedItems = nil;
}

/*
// Override to allow orientations other than the default portrait orientation.
- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}
*/

- (void)didReceiveMemoryWarning {
	// Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
	
	// Release any cached data, images, etc that aren't in use.
}

- (void)viewDidUnload {
	// Release any retained subviews of the main view.
	// e.g. self.myOutlet = nil;
}

- (UIWebView *)webView {
  return (UIWebView *)[self.view viewWithTag:2000];
}

- (UIToolbar *)toolBar {
  return (UIToolbar *)[self.view viewWithTag:1000];
}

static BOOL randSeeded = NO;

- (void)showInWindow:(UIWindow *)window transition:(AWCustomAdWebViewAnimType)animType {
  wasStatusBarHidden = [UIApplication sharedApplication].statusBarHidden;

  if (animType == AWCustomAdWebViewAnimTypeRandom) {
    if (!randSeeded) {
      srandom(CFAbsoluteTimeGetCurrent());
    }
    // range is 1 to 8 inclusive
    animType = (random() % 8) + 1;
    AWLogDebug(@"Webview animation type chosen by random is %d", animType);
  }
  transitionType = animType;
  
  // pre animation
  if (animType == AWCustomAdWebViewAnimTypeNone) {
    [window addSubview:self.view];
    if (wasStatusBarHidden) [[UIApplication sharedApplication] setStatusBarHidden:NO];
  }
  else {
    switch (animType) {
      case AWCustomAdWebViewAnimTypeSlideFromLeft:
      {
        CGRect f = self.view.frame;
        self.view.frame = CGRectOffset(f, -f.size.width, 0);
        break;
      }
      case AWCustomAdWebViewAnimTypeSlideFromRight:
      {
        CGRect f = self.view.frame;
        self.view.frame = CGRectOffset(f, f.size.width, 0);
        break;
      }
      case AWCustomAdWebViewAnimTypeFadeIn:
        self.view.alpha = 0;
        break;
      case AWCustomAdWebViewAnimTypeModal:
      {
        CGRect f = self.view.frame;
        self.view.frame = CGRectOffset(f, 0, f.size.height);
        break;
      }
    }
    BOOL statsBarAnim = NO;
    switch (transitionType) {
      case AWCustomAdWebViewAnimTypeFadeIn:
        statsBarAnim = YES;
      case AWCustomAdWebViewAnimTypeSlideFromLeft:
      case AWCustomAdWebViewAnimTypeSlideFromRight:
      case AWCustomAdWebViewAnimTypeCurlDown:
      case AWCustomAdWebViewAnimTypeModal:
        [window addSubview:self.view];
        if (wasStatusBarHidden)
          [[UIApplication sharedApplication] setStatusBarHidden:NO animated:statsBarAnim];
    }
    [UIView beginAnimations:@"AdWhirlWebBrowserTransitionIn" context:nil];
    [UIView setAnimationDelegate:self];
    [UIView setAnimationDidStopSelector:@selector(transitionInAnimDidStopWithAnimID:finished:context:)];
    [UIView setAnimationBeginsFromCurrentState:YES];
    [UIView setAnimationDuration:kAWWebViewAnimDuration];
    switch (animType) {
      case AWCustomAdWebViewAnimTypeSlideFromLeft:
      {
        CGRect f = self.view.frame;
        self.view.frame = CGRectOffset(f, f.size.width, 0);
        [window addSubview:self.view];
        break;
      }
      case AWCustomAdWebViewAnimTypeSlideFromRight:
      {
        CGRect f = self.view.frame;
        self.view.frame = CGRectOffset(f, -f.size.width, 0);
        [window addSubview:self.view];
        break;
      }
      case AWCustomAdWebViewAnimTypeFlipFromLeft:
        [UIView setAnimationTransition:UIViewAnimationTransitionFlipFromLeft forView:window cache:NO];
        [window addSubview:self.view];
        break;
      case AWCustomAdWebViewAnimTypeFlipFromRight:
        [UIView setAnimationTransition:UIViewAnimationTransitionFlipFromRight forView:window cache:NO];
        [window addSubview:self.view];
        break;
      case AWCustomAdWebViewAnimTypeCurlUp:
        [UIView setAnimationTransition:UIViewAnimationTransitionCurlUp forView:window cache:NO];
        [window addSubview:self.view];
        break;
      case AWCustomAdWebViewAnimTypeCurlDown:
        [UIView setAnimationTransition:UIViewAnimationTransitionCurlDown forView:window cache:NO];
        [window addSubview:self.view];
        break;
      case AWCustomAdWebViewAnimTypeFadeIn:
        self.view.alpha = 1;
        break;
      case AWCustomAdWebViewAnimTypeModal:
      {
        CGRect f = self.view.frame;
        self.view.frame = CGRectOffset(f, 0, -f.size.height);
        [window addSubview:self.view];
        break;
      }
    }
    [UIView commitAnimations];
  }
}

- (void)loadURL:(NSURL *)url {
	NSURLRequest *urlRequest = [NSURLRequest requestWithURL:url];
	[self.webView loadRequest:urlRequest];
}

- (void)transitionInAnimDidStopWithAnimID:(NSString *)aid finished:(BOOL)f context:(void *)c {
  if (wasStatusBarHidden) [[UIApplication sharedApplication] setStatusBarHidden:NO];
}

- (void)transitionOutAnimDidStopWithAnimID:(NSString *)aid finished:(BOOL)f context:(void *)c {
  switch (transitionType) {
    case AWCustomAdWebViewAnimTypeSlideFromLeft:
    case AWCustomAdWebViewAnimTypeSlideFromRight:
    case AWCustomAdWebViewAnimTypeModal:
      [self.view removeFromSuperview];
  }
  if (wasStatusBarHidden)
    [[UIApplication sharedApplication] setStatusBarHidden:wasStatusBarHidden];
  if (self.delegate) {
    [delegate webBrowserClosed:self];
  }
}

- (void)dealloc {
  [loadingButtons release], loadingButtons = nil;
  [loadedButtons release], loadedButtons = nil;
  [super dealloc];
}

#pragma mark -
#pragma mark UIWebViewDelegate methods

- (void)webView:(UIWebView *)webView didFailLoadWithError:(NSError *)error {
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
  self.toolBar.items = self.loadedButtons;
	if (self.webView.canGoForward) {
		self.forwardButton.enabled = YES;
	}
	if (self.webView.canGoBack) {
		self.backButton.enabled = YES;
	}
	self.reloadButton.enabled = YES;
	self.stopButton.enabled = NO;
	if (self.webView.request) {
    self.linkOutButton.enabled = YES;
  }
}

- (void)webViewDidFinishLoad:(UIWebView *)webView {
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
  self.toolBar.items = self.loadedButtons;
	if (self.webView.canGoForward) {
		self.forwardButton.enabled = YES;
	}
	if (self.webView.canGoBack) {
		self.backButton.enabled = YES;
	}
	self.reloadButton.enabled = YES;
	self.stopButton.enabled = NO;
	if (self.webView.request) {
    self.linkOutButton.enabled = YES;
  }
  
//  // extract title of page
//  NSString* title = [self.webView stringByEvaluatingJavaScriptFromString: @"document.title"];
//  self.navigationItem.title = title;
}

- (void)webViewDidStartLoad:(UIWebView *)webView {
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
  self.toolBar.items = self.loadingButtons;
	self.forwardButton.enabled = NO;
	self.backButton.enabled = NO;
	self.reloadButton.enabled = NO;
	self.stopButton.enabled = YES;
}

#pragma mark -
#pragma mark button targets

- (IBAction)forward:(id)sender {
	[self.webView goForward];
}

- (IBAction)back:(id)sender {
	[self.webView goBack];
}

- (IBAction)stop:(id)sender {
	[self.webView stopLoading];
}

- (IBAction)reload:(id)sender {
  [self.webView reload];
}

- (IBAction)linkOut:(id)sender {
  [[UIApplication sharedApplication] openURL:self.webView.request.URL];
}

- (IBAction)close:(id)sender {
  if (transitionType == AWCustomAdWebViewAnimTypeNone) {
    [self.view removeFromSuperview];
    if (wasStatusBarHidden) [[UIApplication sharedApplication] setStatusBarHidden:YES];
    if (self.delegate) [delegate webBrowserClosed:self];
    return;
  }
  // reverse the transition in
  [UIView beginAnimations:@"AdWhirlWebBrowserTransitionOut" context:nil];
  [UIView setAnimationDelegate:self];
  [UIView setAnimationDidStopSelector:@selector(transitionOutAnimDidStopWithAnimID:finished:context:)];
  [UIView setAnimationBeginsFromCurrentState:YES];
  [UIView setAnimationDuration:kAWWebViewAnimDuration];
  switch (transitionType) {
    case AWCustomAdWebViewAnimTypeFlipFromLeft:
      [UIView setAnimationTransition:UIViewAnimationTransitionFlipFromRight
                             forView:self.view.superview cache:NO];
      [self.view removeFromSuperview];
      break;
    case AWCustomAdWebViewAnimTypeFlipFromRight:
      [UIView setAnimationTransition:UIViewAnimationTransitionFlipFromLeft
                             forView:self.view.superview cache:NO];
      [self.view removeFromSuperview];
      break;
    case AWCustomAdWebViewAnimTypeCurlUp:
      [UIView setAnimationTransition:UIViewAnimationTransitionCurlDown
                             forView:self.view.superview cache:YES];
      [self.view removeFromSuperview];
      break;
    case AWCustomAdWebViewAnimTypeCurlDown:
      [UIView setAnimationTransition:UIViewAnimationTransitionCurlUp
                             forView:self.view.superview cache:YES];
      [self.view removeFromSuperview];
      break;
    case AWCustomAdWebViewAnimTypeSlideFromLeft:
    {
      CGRect f = self.view.frame;
      self.view.frame = CGRectOffset(f, -f.size.width, 0);
      break;
    }
    case AWCustomAdWebViewAnimTypeSlideFromRight:
    {
      CGRect f = self.view.frame;
      self.view.frame = CGRectOffset(f, f.size.width, 0);
      break;
    }
    case AWCustomAdWebViewAnimTypeFadeIn:
      self.view.alpha = 0.0;
      break;
    case AWCustomAdWebViewAnimTypeModal:
    {
      CGRect f = self.view.frame;
      self.view.frame = CGRectOffset(f, 0, f.size.height);
      break;
    }
  }
  [UIView commitAnimations];
  BOOL statsBarAnim = NO;
  switch (transitionType) {
    case AWCustomAdWebViewAnimTypeFadeIn:
    case AWCustomAdWebViewAnimTypeModal:
    case AWCustomAdWebViewAnimTypeCurlDown:
      statsBarAnim = YES;
    case AWCustomAdWebViewAnimTypeFlipFromLeft:
    case AWCustomAdWebViewAnimTypeFlipFromRight:
    case AWCustomAdWebViewAnimTypeCurlUp:
      [[UIApplication sharedApplication] setStatusBarHidden:wasStatusBarHidden
                                                   animated:statsBarAnim];
  }
}

@end


@implementation AdWhirlBackButton

- (id)initWithCoder:(NSCoder *)encoder {
  AdWhirlBackButton *saved = [(id<NSCoding>)super initWithCoder:encoder];
  if (saved == nil) return nil;

  // draw the back image
  CGColorSpaceRef colorspace = CGColorSpaceCreateDeviceRGB();
  CGContextRef ctx = CGBitmapContextCreate(nil, 25, 25, 8, 0, colorspace,
                                           kCGImageAlphaPremultipliedLast);
  CGColorSpaceRelease(colorspace);
  CGPoint bot = CGPointMake(19, 2);
  CGPoint top = CGPointMake(19, 20);
  CGPoint tip = CGPointMake(4, 11);
  CGContextSetFillColorWithColor(ctx, [UIColor whiteColor].CGColor);
  CGContextMoveToPoint(ctx, bot.x, bot.y);
  CGContextAddLineToPoint(ctx, tip.x, tip.y);
  CGContextAddLineToPoint(ctx, top.x, top.y);
  CGContextFillPath(ctx);
  
  // set the image
  CGImageRef backImgRef = CGBitmapContextCreateImage(ctx);
  CGContextRelease(ctx);
  UIImage* backImage = [[UIImage alloc] initWithCGImage:backImgRef];
  CGImageRelease(backImgRef);
  saved.image = backImage;
  [backImage release];
  
  return saved;
}

@end