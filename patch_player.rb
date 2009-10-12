#!/usr/bin/env ruby

require 'fileutils'

PROJECT_NAME = "UnityPlayer"

%x{cd #{PROJECT_NAME}; ln -s ../AdWhirl Adwhirl}

FileUtils.copy_file "AppController.mm", "#{PROJECT_NAME}/Classes/AppController.mm"
FileUtils.copy_file "project.pbxproj", "#{PROJECT_NAME}/Unity-iPhone.xcodeproj/project.pbxproj"