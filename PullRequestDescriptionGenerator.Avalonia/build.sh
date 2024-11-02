#!/bin/bash

VERSION="1.0.0"
PROJECT="PullRequestDescriptionGenerator.Avalonia"

# Build for macOS
dotnet publish -c Release -r osx-x64 --self-contained true /p:PublishSingleFile=true

# Build for Windows
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true

# Build for Linux
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true

# Create zip files
cd bin/Release/net8.0/osx-x64/publish
zip -r ../../../../PR-Description-Generator-$VERSION-osx-x64.zip ./*

cd ../../win-x64/publish
zip -r ../../../../PR-Description-Generator-$VERSION-win-x64.zip ./*

cd ../../linux-x64/publish
zip -r ../../../../PR-Description-Generator-$VERSION-linux-x64.zip ./*
