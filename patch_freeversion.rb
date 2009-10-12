#!/usr/bin/env ruby

require 'fileutils'

PROJECT_NAME = "UnityPlayer"

%x{ln -shF ../Adwhirl #{PROJECT_NAME}/Adwhirl}
FileUtils.ln_sf "../../AppController.mm", "#{PROJECT_NAME}/Classes/AppController.mm"
FileUtils.ln_sf "../../project.pbxproj", "#{PROJECT_NAME}/Unity-iPhone.xcodeproj/project.pbxproj"

