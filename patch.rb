#!/usr/bin/env ruby

require 'fileutils'

PROJECT_NAME = "UnityPlayer"

%x{ln -shF ../Adwhirl #{PROJECT_NAME}/Adwhirl}
%x{ln -shF ../fbconnect #{PROJECT_NAME}/fbconnect}
%x{ln -shF ../Audio #{PROJECT_NAME}/Audio}
Dir["*.{m,mm,h}"].each {|f| FileUtils.ln_sf "../../#{f}", "#{PROJECT_NAME}/Classes/#{f}"}
FileUtils.ln_sf "../dummy-ad.png", "#{PROJECT_NAME}/dummy-ad.png"
FileUtils.ln_sf "../Game.plist", "#{PROJECT_NAME}/Game.plist"
FileUtils.ln_sf "../../project.pbxproj", "#{PROJECT_NAME}/Unity-iPhone.xcodeproj/project.pbxproj"