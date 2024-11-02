# PR Description Generator 🚀

[![Build Status](https://github.com/nickmaluleke/PullRequestDescriptor/actions/workflows/build-and-release.yml/badge.svg)](https://github.com/nickmaluleke/PullRequestDescriptor/actions)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Why I Created This

As a developer, I found that writing comprehensive pull request descriptions was often time-consuming and sometimes overlooked. I wanted to create a tool that would:

1. Automatically analyze code changes and generate meaningful PR descriptions
2. Save developers time while maintaining high-quality documentation
3. Ensure consistent PR description format across teams
4. Make code review processes more efficient

## What It Does

PR Description Generator is a desktop application that:

1. **Analyzes Your Changes**: Connects to your local Git repository and analyzes differences between branches
2. **Generates Descriptions**: Uses OpenAI's GPT models to create detailed, structured PR descriptions
3. **Maintains Format**: Follows a consistent template with sections for:
   - Related Issues
   - Technical Changes
   - Release Notes
   - Testing Evidence
   - Quality Checklist

## Key Features

- **Smart Analysis**: Automatically detects and summarizes code changes
- **Customizable Templates**: Tailor the AI prompts to match your team's style
- **Cross-Platform**: Works seamlessly on Windows, macOS, and Linux
- **Git Integration**: Direct integration with local Git repositories
- **Modern UI**: Clean, intuitive interface with dark mode support
- **Quick Access**: Menu bar icon for easy access
- **Keyboard Shortcuts**: Efficient workflow (⌘ + , for settings)

## Screenshots

![Main Window](docs/images/app-screenshot.png)

## Installation

### Download

Get the latest version for your platform:
- [Windows (64-bit)](../../releases/latest/download/PR-Description-Generator-win-x64.zip)
- [macOS (64-bit)](../../releases/latest/download/PR-Description-Generator-osx-x64.zip)
- [Linux (64-bit)](../../releases/latest/download/PR-Description-Generator-linux-x64.zip)

### Setup

1. Extract the downloaded zip file
2. Run the application
3. Enter your OpenAI API key in Settings (⌘ + , or Ctrl + ,)
4. Start generating PR descriptions!

### For Developers

#### Prerequisites

- .NET 8 SDK
- Git
- OpenAI API key
