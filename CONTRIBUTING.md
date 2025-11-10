# Contributing to BitLabs Unity SDK

Internal development guide for BitLabs Unity SDK maintainers and contributors.

## Development Environment

### Required Tools

- Unity 6000.0.2f (Unity 6)
- Git
- VS Code with C# Dev Kit (or Rider/Visual Studio)
- Android Studio (for Android builds)
- Xcode (for iOS builds, macOS only)

### Initial Setup

1. **Clone and open project:**

   ```bash
   git clone https://github.com/BitBurst-GmbH/bitlabs-unity.git
   cd bitlabs-unity
   ```

   Open in Unity Hub with Unity 6000.0.2f

2. **Project structure:**

   ```
   Packages/com.prodege.bitlabs/    # The package (tracked in git)
   Assets/Samples/                   # Local development demo (gitignored)
   ```

3. **Install EDM4U dependency:**
   Since `Packages/manifest.json` is gitignored, you need to manually add EDM4U:

   Add to your local `Packages/manifest.json`:

   ```json
   {
     "dependencies": {
       "com.google.external-dependency-manager": "https://github.com/googlesamples/unity-jar-resolver.git?path=upm#v1.2.186"
     }
   }
   ```

   Unity will then resolve all dependencies automatically.

## Development Workflow

### Working on the SDK

1. **Make changes in the package:**

   - Edit code in `Packages/com.prodege.bitlabs/Runtime/`
   - Edit editor scripts in `Packages/com.prodege.bitlabs/Editor/`
   - Update iOS bridge in `Packages/com.prodege.bitlabs/Plugins/iOS/`

2. **Test locally:**

   - Use `Assets/Samples/` for quick testing (this folder is gitignored)
   - Import demo from `Packages/com.prodege.bitlabs/Samples~/DemoScene/`
   - Test on both Android and iOS devices

3. **Update documentation:**
   - Update `Packages/com.prodege.bitlabs/CHANGELOG.md`
   - Update README if API changes

### Updating Native SDKs

When BitLabs Android or iOS SDK releases a new version:

1. **Update EDM4U config:**

   ```xml
   <!-- Packages/com.prodege.bitlabs/Editor/BitLabsDependencies.xml -->
   <androidPackage spec="com.prodege.bitlabs:unity:NEW_VERSION" />
   <iosPod name="BitLabs/Unity" version="~> NEW_VERSION" />
   ```

2. **Update iOS bridge if needed:**

   - Check if iOS API changed
   - Update `BitLabsBridge.swift` and `BitLabsWrapper.mm` accordingly

3. **Test builds:**

   - Build Android and verify native SDK loads
   - Build iOS and verify native SDK loads

4. **Document in CHANGELOG:**

   ```markdown
   ### Changed

   - Updated Android SDK to vX.X.X
   - Updated iOS SDK to vX.X.X
   ```

## Building and Releasing

### Version Numbering

Follow Semantic Versioning (MAJOR.MINOR.PATCH):

- **MAJOR**: Breaking changes (e.g., API redesign)
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes only

### Release Checklist

1. **Update version:**

   - `Packages/com.prodege.bitlabs/package.json` - version field
   - `Packages/com.prodege.bitlabs/CHANGELOG.md` - add new version section

2. **Export .unitypackage:**

   - Open `Assets/BitLabsExportConfig` (Upload Config asset)
   - Click "Export for Local Testing"
   - Save as `BitLabs-vX.X.X.unitypackage`
   - Copy to `BitLabs-latest.unitypackage`

3. **Test the package:**

   - Import `.unitypackage` in a clean Unity project
   - Verify EDM4U resolves dependencies
   - Test Android and iOS builds

4. **Commit and push to master:**

   ```bash
   git add .
   git commit -m "vX.X.X"
   git push origin master
   ```

5. **Create and push UPM branch:**

   ```bash
   # Split package subfolder to upm branch
   git subtree split --prefix=Packages/com.prodege.bitlabs --branch upm

   # Push upm branch
   git push origin upm

   # Tag the release on upm branch
   git checkout upm
   git tag vX.X.X
   git push origin upm --tags

   # Return to master
   git checkout master
   ```

6. **Create GitHub release:**
   - Tag: `vX.X.X` (on upm branch)
   - Attach `.unitypackage` files
   - Copy CHANGELOG entries to release notes

## Branch Strategy

- **`master`**: Production-ready code
- **`feature/*`**: Feature branches
- **`hotfix/*`**: Urgent fixes

### Typical workflow:

```bash
# Create feature branch
git checkout -b feature/new-api-method

# Make changes and test
# ... development work ...

# Commit
git add .
git commit -m "Add GetUserRewards API method"

# Push and create PR
git push origin feature/new-api-method
```

## Git Subtree Workflow

We use `git subtree` to create a clean `upm` branch for distribution. This keeps the package at the root for cleaner UPM URLs.

**The workflow is integrated into the release checklist above.** Users install via:

```json
// Specific version
"com.prodege.bitlabs": "https://github.com/BitBurst-GmbH/bitlabs-unity.git#v4.0.0"

// Latest (upm branch HEAD)
"com.prodege.bitlabs": "https://github.com/BitBurst-GmbH/bitlabs-unity.git#upm"
```

**How it works:**

- `master` branch: Development with full Unity project structure
- `upm` branch: Auto-generated from `Packages/com.prodege.bitlabs/` subfolder
- On each release, run `git subtree split` to regenerate `upm` branch from master
- Tag the version on the `upm` branch, not master

## Testing Guidelines

### Before every commit:

1. **Compile check:** No errors in Unity console
2. **Android build:** Verify APK builds successfully
3. **iOS build:** Verify Xcode project generates

### Before every release:

1. **Clean project test:**

   - Import `.unitypackage` in fresh Unity project
   - Verify all features work
   - Build for Android and iOS

2. **Demo scene check:**
   - All buttons functional
   - No console errors
   - Surveys load correctly

## Common Issues

### EDM4U not resolving:

- **Fix:** Assets → External Dependency Manager → Android Resolver → Resolve

### iOS build fails:

- Check CocoaPods installed: `pod --version`
- Run iOS resolver: Assets → External Dependency Manager → iOS Resolver → Install Cocoapods

### Android SDK version conflicts:

- Check native SDK requirements in BitLabsDependencies.xml
- Update AGP/CompileSDK/TargetSDK in Unity Player Settings

## Internal Resources

- **Android SDK:** https://github.com/BitBurst-GmbH/bitlabs-android-library
- **iOS SDK:** https://github.com/BitBurst-GmbH/bitlabs-ios-sdk
- **Dashboard:** https://dashboard.bitlabs.ai
- **API Docs:** https://developer.bitlabs.ai
