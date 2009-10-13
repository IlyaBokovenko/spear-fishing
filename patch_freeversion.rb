#!/usr/bin/env ruby

require 'fileutils'

PROJECT_NAME = "UnityPlayer"

%x{ln -shF ../Adwhirl #{PROJECT_NAME}/Adwhirl}
%x{ln -shF ../fbconnect #{PROJECT_NAME}/fbconnect}
Dir["*.{m,mm,h}"].each {|f| FileUtils.ln_sf "../../#{f}", "#{PROJECT_NAME}/Classes/#{f}"}
FileUtils.ln_sf "../../project.pbxproj", "#{PROJECT_NAME}/Unity-iPhone.xcodeproj/project.pbxproj"