/*

 AdWhirlLog.h
 
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

typedef enum {
  AWLogLevelNone  = 0,
  AWLogLevelCrit  = 10,
  AWLogLevelError = 20,
  AWLogLevelWarn  = 30,
  AWLogLevelInfo  = 40,
  AWLogLevelDebug = 50
} AWLogLevel;

void AWLogSetLogLevel(AWLogLevel level);
void AWLogCrit(NSString *format, ...);
void AWLogError(NSString *format, ...);
void AWLogWarn(NSString *format, ...);
void AWLogInfo(NSString *format, ...);
void AWLogDebug(NSString *format, ...);