# BitLabs Unity SDK - AndroidJavaProxy Callback Implementation

## Overview
Successfully modernized the BitLabs Unity SDK Android integration by replacing the legacy UnitySendMessage pattern with type-safe AndroidJavaProxy callbacks. This document serves as a comprehensive reference for the implementation and future work.

---

## What We Accomplished

### ✅ Replaced Legacy UnitySendMessage with AndroidJavaProxy Callbacks

**Old Approach:**
- String-based communication: `UnitySendMessage(gameObject, method, data)`
- No type safety
- Required GameObject to be active in scene
- Hard to maintain and debug

**New Approach:**
- Type-safe AndroidJavaProxy implementing Kotlin interfaces
- C# Action delegates: `Action<bool>`, `Action<string>`, `Action<double>`
- Direct callback invocation without GameObject dependency
- Clean, maintainable, modern architecture

### ✅ Discovered and Solved the Kotlin Name Mangling Issue

**Problem:**
- Initial implementation failed with `NoSuchMethodError: no non-static method 'init' in class Ljava.lang.Object`
- Method appeared as `init-BWLJW6A` instead of `init` when inspected with `javap`

**Root Cause:**
- Kotlin's `runCatching` returns `Result<T>`, which is an inline value class
- JVM mangles method names that return inline value classes to prevent signature conflicts
- Example: `fun init(...): Result<Unit>` becomes `public final Object init-BWLJW6A(...)`

**Solution:**
- Removed `runCatching { ... }.onFailure { ... }` from Unity-facing methods
- Replaced with standard `try { ... } catch (e: Exception) { ... }` blocks
- Applied to: `init()`, `checkSurveys()`, `getSurveys()`

**Files Modified:**
- `/Users/omar.raad/StudioProjects/bitlabs-android-library/library/src/unity/java/ai/bitlabs/sdk/BitLabs.kt`

### ✅ Proved Generic Interfaces Don't Work with Unity's JNI

**Experiment Conducted:**
1. Updated `checkSurveys()` to use `OnResponseListener<Boolean>` instead of `OnBooleanResponseListener`
2. Updated C# `BooleanResponseListener` to implement `ai.bitlabs.sdk.util.OnResponseListener`
3. Tested with method signature: `public void onResponse(AndroidJavaObject response)`
4. **Result:** Failed with error `No such proxy method: onResponse(System.Boolean)`

**Key Discovery:**
- Unity's AndroidJavaProxy does **runtime type inspection**, not JVM type erasure
- When Kotlin passes `surveys.isNotEmpty()` (Boolean), Unity expects `onResponse(Boolean)`, not `onResponse(Object)`
- Unity's JNI bridge cannot match proxy methods against erased generic signatures
- The constructor parameter `Action<bool>` was NOT the issue - Unity looks at the public method signature

**Conclusion:**
- Generic interfaces (`OnResponseListener<T>`) **cannot work** with Unity's AndroidJavaProxy
- Concrete interfaces are **necessary** for Unity JNI compatibility
- This is a limitation of Unity's proxy system, not standard JNI

### ✅ Final Architecture

**Core Android SDK** (`/library/src/main/java/`):
- Uses clean generic `OnResponseListener<T>` for native Android apps
- Modern, flexible API for Kotlin/Java developers

**Unity Variant** (`/library/src/unity/java/`):
- Uses concrete interfaces in `UnityCallbacks.kt`:
  - `OnInitResponseListener { fun onResponse() }`
  - `OnBooleanResponseListener { fun onResponse(response: Boolean) }`
  - `OnStringResponseListener { fun onResponse(response: String) }`
- Specifically designed for Unity JNI compatibility

**Shared Interfaces** (no generics, work in both):
- `OnExceptionListener { fun onException(exception: Exception) }`
- `OnSurveyRewardListener { fun onSurveyReward(reward: Double) }`
- `OnOfferwallClosedListener { fun onOfferwallClosed(totalReward: Double) }`

**Benefits:**
- ✅ Core SDK maintains clean generic API
- ✅ Unity SDK has JNI-compatible concrete interfaces
- ✅ No code duplication (both in same library, different source sets)
- ✅ Clear separation of concerns

### ✅ Thread Safety Implementation

**UnityMainThreadDispatcher:**
- Singleton MonoBehaviour that marshals callbacks from background threads to Unity's main thread
- Required because Kotlin coroutines execute on IO dispatcher
- Unity APIs must be called from main thread

**Implementation:**
```csharp
public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    public void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    private void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue()?.Invoke();
            }
        }
    }
}
```

**Applied to all callbacks except init** (init tested without it and worked, but re-enabled for consistency)

---

## Files Modified

### Android SDK (`/Users/omar.raad/StudioProjects/bitlabs-android-library/`)

#### 1. `library/src/unity/java/ai/bitlabs/sdk/BitLabs.kt`
**Changes:**
- Removed `runCatching` from `init()`, `checkSurveys()`, `getSurveys()`
- Replaced with standard try-catch blocks
- Updated to use concrete callback interfaces: `OnBooleanResponseListener`, `OnStringResponseListener`
- Updated imports to remove `OnResponseListener<T>`, add `OnBooleanResponseListener`

**Key Methods:**
```kotlin
fun init(
    token: String, uid: String,
    onResponseListener: OnInitResponseListener,
    onExceptionListener: OnExceptionListener,
) {
    try {
        // ... initialization code ...
        onResponseListener.onResponse()
    } catch (e: Exception) {
        SentryManager.captureException(token, uid, e)
        onExceptionListener.onException(e)
    }
}

fun checkSurveys(
    onResponseListener: OnBooleanResponseListener,
    onExceptionListener: OnExceptionListener,
) = ifInitialised {
    coroutineScope.launch {
        try {
            val surveys = bitLabsRepo?.getSurveys("UNITY") ?: emptyList()
            onResponseListener.onResponse(surveys.isNotEmpty())
        } catch (e: Exception) {
            SentryManager.captureException(token, uid, e)
            onExceptionListener.onException(e)
        }
    }
}
```

#### 2. `library/src/unity/java/ai/bitlabs/sdk/util/UnityCallbacks.kt`
**Purpose:** Unity-specific callback interfaces without generics to avoid JNI type erasure issues

**Content:**
```kotlin
package ai.bitlabs.sdk.util

fun interface OnInitResponseListener {
    fun onResponse()
}

fun interface OnBooleanResponseListener {
    fun onResponse(response: Boolean)
}

fun interface OnStringResponseListener {
    fun onResponse(response: String)
}
```

### Unity SDK (`/Users/omar.raad/UnityProjects/bitlabs-unity/`)

#### 3. `Assets/BitLabs/Source/Runtime/BitLabsAndroidCallbacks.cs`
**Purpose:** AndroidJavaProxy implementations for Kotlin callback interfaces

**Key Classes:**
```csharp
namespace BitLabsCallbacks
{
#if UNITY_ANDROID
    [UnityEngine.Scripting.Preserve]
    public class ResponseListener : AndroidJavaProxy
    {
        private readonly Action _onResponse;

        public ResponseListener(Action onResponse)
            : base("ai.bitlabs.sdk.util.OnInitResponseListener")
        {
            _onResponse = onResponse;
        }

        public void onResponse()
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                try
                {
                    _onResponse?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[BitLabs] Error in ResponseListener: {e}");
                }
            });
        }
    }

    // Similar implementations for:
    // - BooleanResponseListener (OnBooleanResponseListener)
    // - StringResponseListener (OnStringResponseListener)
    // - ExceptionListener (OnExceptionListener)
    // - RewardListener (OnSurveyRewardListener)
#endif
}
```

**Important:**
- All methods must match exact Kotlin interface signatures
- All callbacks wrapped in `UnityMainThreadDispatcher.Instance.Enqueue()`
- All classes marked with `[UnityEngine.Scripting.Preserve]` to prevent IL2CPP stripping

#### 4. `Assets/BitLabs/Source/Runtime/UnityMainThreadDispatcher.cs`
**Purpose:** Thread-safe dispatcher for marshalling callbacks to Unity's main thread

**Implementation:**
- Singleton pattern with `DontDestroyOnLoad`
- Thread-safe queue with lock
- Executes queued actions in `Update()` on main thread

#### 5. `Assets/BitLabs/Source/Runtime/BitLabs.cs`
**Changes:**
- Updated `Init()` to accept `Action onSuccess` and `Action<string> onError` callbacks
- Updated `CheckSurveys()` to accept `Action<bool> onSuccess` callback
- Updated `GetSurveys()` to accept `Action<string> onSuccess` callback
- Updated `SetRewardCallback()` to accept `Action<double> onReward` callback
- Removed all UnitySendMessage calls

**Example:**
```csharp
public static void Init(string token, string uid, Action onSuccess = null, Action<string> onError = null)
{
#if UNITY_ANDROID
    unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

    bitlabsObject = new AndroidJavaObject("ai.bitlabs.sdk.BitLabs");
    bitlabs = bitlabsObject.GetStatic<AndroidJavaObject>("INSTANCE");

    var responseListener = new ResponseListener(onSuccess);
    var exceptionListener = new ExceptionListener(onError);

    bitlabs.Call("init", token, uid, responseListener, exceptionListener);
#endif
}
```

#### 6. `Assets/link.xml`
**Purpose:** Prevent IL2CPP from stripping callback classes used by Android native code

**Content:**
```xml
<linker>
  <assembly fullname="Assembly-CSharp">
    <type fullname="BitLabsCallbacks.ResponseListener" preserve="all"/>
    <type fullname="BitLabsCallbacks.BooleanResponseListener" preserve="all"/>
    <type fullname="BitLabsCallbacks.StringResponseListener" preserve="all"/>
    <type fullname="BitLabsCallbacks.ExceptionListener" preserve="all"/>
    <type fullname="BitLabsCallbacks.RewardListener" preserve="all"/>
    <type fullname="BitLabsCallbacks.UnityMainThreadDispatcher" preserve="all"/>
  </assembly>
</linker>
```

#### 7. `Assets/BitLabs/Samples/DemoScene/Demo.cs`
**Changes:**
- Updated to use new callback-based API
- Example usage:

```csharp
void Start()
{
    BitLabs.Init(Token, UserId,
        onSuccess: () => {
            Debug.Log("BitLabs SDK initialized successfully!");
            BitLabs.AddTag("userType", "new");
            BitLabs.SetRewardCallback(RewardCallback);
        },
        onError: (error) => {
            Debug.LogError($"BitLabs initialization failed: {error}");
        }
    );
}

private void RewardCallback(double payout)
{
    Debug.Log($"BitLabs Reward received: {payout}");
}
```

---

## Build Configuration

### Gradle Configuration for Local Testing

#### `Assets/Plugins/Android/settingsTemplate.gradle`
```gradle
dependencyResolutionManagement {
    repositoriesMode.set(RepositoriesMode.PREFER_SETTINGS)
    repositories {
        mavenLocal() // Check local Maven first for testing
        google()
        mavenCentral()
    }
}
```

#### `Assets/Plugins/Editor/BitLabsDependencies.xml`
```xml
<androidPackages>
  <repositories>
    <repository>https://jitpack.io</repository>
  </repositories>
  <androidPackage spec="com.prodege.bitlabs:unity:5.1.0">
  </androidPackage>
</androidPackages>
```

### Unity Version Requirements
- **Minimum:** Unity 6 (2022.3 has AGP/NDK version conflicts with modern Android dependencies)
- **AGP Version:** 8.6.1 (for compileSdk 35)
- **NDK Version:** 23.1.7779620 (Unity's bundled version)

---

## Build and Test Commands

### Android SDK Build
```bash
cd /Users/omar.raad/StudioProjects/bitlabs-android-library

# Clean build
./gradlew clean

# Build Unity variant AAR
./gradlew :library:assembleUnityRelease

# Publish to Maven Local for testing
./gradlew :library:publishToMavenLocal

# Verify AAR contents (optional)
jar tf library/build/outputs/aar/library-unity-release.aar | grep -E "(BitLabs|UnityCallbacks)"
```

### Unity Project Cache Clear
```bash
cd /Users/omar.raad/UnityProjects/bitlabs-unity

# Clear Unity's Gradle cache
rm -rf Library/Bee/Android/Prj/IL2CPP/Gradle/.gradle
rm -rf Library/Bee/Android/Prj/IL2CPP/Gradle/launcher/build
rm -rf Library/Bee/Android/Prj/IL2CPP/Gradle/unityLibrary/build

# Clear system Gradle cache for BitLabs artifact
rm -rf ~/.gradle/caches/modules-2/files-2.1/com.prodege.bitlabs/unity/5.1.0
```

### Unity Build
1. Open Unity Editor
2. Go to Build Settings → Android
3. Click Build (or Build and Run)

---

## Testing Results

### ✅ What Works
- `BitLabs.Init()` - Callbacks fire successfully
- `BitLabs.CheckSurveys()` - Returns boolean correctly
- `BitLabs.GetSurveys()` - Returns JSON string correctly
- `BitLabs.SetRewardCallback()` - Reward callback fires when offerwall closes
- `BitLabs.LaunchOfferWall()` - Opens offerwall activity
- Thread safety with `UnityMainThreadDispatcher`
- IL2CPP builds (with link.xml configuration)

### Debug Output Example
```
17:08:18.997 Unity I [BitLabs] ResponseListener.onResponse() called!
17:08:18.998 Unity I BitLabs SDK initialized successfully!
17:08:19.019 BitLabs D Advertising Id: 95bf8a4d-89f7-42b3-9abb-cbc724e5fcc0
```

---

## Lessons Learned

### 1. Kotlin Inline Value Classes Cause JVM Name Mangling
- `Result<T>` returned by `runCatching` is an inline value class
- JVM mangles method names to prevent signature conflicts
- Unity's JNI cannot find mangled methods
- **Solution:** Use standard try-catch instead of `runCatching` for Unity-facing methods

### 2. Unity's AndroidJavaProxy Doesn't Support Generic Interfaces
- Unity does runtime type inspection, not JVM type erasure
- Cannot write a proxy method that "matches whatever type gets passed"
- **Solution:** Create concrete interfaces for each type (Boolean, String, etc.)

### 3. Type Erasure vs Runtime Type Inspection
- **Standard JNI:** Respects type erasure, `OnResponseListener<T>` becomes `onResponse(Object)`
- **Unity's JNI:** Inspects runtime type, expects `onResponse(Boolean)` when Boolean is passed
- This is a Unity-specific limitation

### 4. Thread Safety is Critical
- Kotlin coroutines execute on background threads (Dispatchers.IO)
- Unity APIs must be called from main thread
- **Solution:** UnityMainThreadDispatcher marshals all callbacks to main thread

### 5. IL2CPP Code Stripping
- IL2CPP strips "unused" code at build time
- AndroidJavaProxy classes called from native code appear unused
- **Solution:** link.xml with `preserve="all"` for all callback classes

---

## TODO: Remaining Work

### 🔲 1. iOS Implementation - Apply Similar Callback Pattern

**Current iOS Implementation:**
- Located in `/Users/omar.raad/UnityProjects/bitlabs-unity/Assets/Plugins/iOS/`
- Uses `[DllImport("__Internal")]` for native C function calls
- Callbacks likely using UnitySendMessage or delegates

**Required Changes:**
- Review current iOS callback mechanism
- If using UnitySendMessage, replace with delegate-based callbacks
- Update iOS native code (`BitLabsSDK.mm` or similar) to use C# delegates
- Ensure thread safety (iOS callbacks may come from background threads)
- Test with real iOS builds

**Files to Review:**
- `Assets/Plugins/iOS/*.mm` - Objective-C++ native implementation
- `Assets/BitLabs/Source/Runtime/BitLabs.cs` - iOS-specific `#if UNITY_IOS` sections
- iOS native BitLabs SDK integration

**Tasks:**
- [ ] Review current iOS callback implementation
- [ ] Design iOS callback architecture (similar to Android)
- [ ] Implement iOS delegates in C#
- [ ] Update Objective-C++ bridge code
- [ ] Add iOS thread safety if needed
- [ ] Test on real iOS device
- [ ] Document iOS implementation

### 🔲 2. UPM Package Structure

**Goal:** Distribute as Unity Package Manager (UPM) package for easy installation

**Requirements:**
- Create `package.json` manifest
- Define proper directory structure:
  ```
  com.bitlabs.sdk/
  ├── package.json
  ├── README.md
  ├── CHANGELOG.md
  ├── LICENSE.md
  ├── Runtime/
  │   ├── BitLabs.cs
  │   ├── BitLabsAndroidCallbacks.cs
  │   ├── UnityMainThreadDispatcher.cs
  │   └── BitLabs.Runtime.asmdef
  ├── Editor/
  │   ├── BitLabsDependencies.xml
  │   └── BitLabs.Editor.asmdef
  ├── Plugins/
  │   ├── Android/
  │   │   ├── settingsTemplate.gradle
  │   │   ├── mainTemplate.gradle
  │   │   └── baseProjectTemplate.gradle
  │   └── iOS/
  │       └── [iOS native files]
  ├── Samples~/
  │   └── DemoScene/
  └── Documentation~/
  ```

**Tasks:**
- [ ] Create package.json with proper metadata
- [ ] Restructure project to UPM format
- [ ] Create assembly definitions (.asmdef)
- [ ] Move samples to `Samples~` directory
- [ ] Create comprehensive README.md
- [ ] Add CHANGELOG.md
- [ ] Test installation from:
  - [ ] Git URL
  - [ ] Local tarball
  - [ ] npm registry (if publishing)
- [ ] Document installation instructions

**Package.json Example:**
```json
{
  "name": "com.bitlabs.sdk",
  "version": "5.1.0",
  "displayName": "BitLabs SDK",
  "description": "BitLabs survey integration for Unity",
  "unity": "2021.3",
  "keywords": ["bitlabs", "surveys", "monetization"],
  "author": {
    "name": "BitLabs",
    "url": "https://www.bitlabs.ai"
  },
  "dependencies": {}
}
```

### 🔲 3. Code Cleanup and Refactoring

**Android Side:**
- [ ] Remove any remaining debug logs
- [ ] Ensure consistent error handling across all methods
- [ ] Add KDoc documentation to all public methods
- [ ] Review and update method signatures for consistency
- [ ] Consider adding suspend function variants for Kotlin users

**Unity C# Side:**
- [ ] Remove debug logs from callbacks
- [ ] Add XML documentation comments to all public APIs
- [ ] Consider adding async/await Task-based API alongside callbacks
- [ ] Validate all method parameters (null checks, empty strings)
- [ ] Add proper error messages for common mistakes

**Example Improvements:**
```csharp
/// <summary>
/// Initializes the BitLabs SDK with your app token and user ID.
/// </summary>
/// <param name="token">Your BitLabs API token from the dashboard</param>
/// <param name="uid">Unique identifier for the current user</param>
/// <param name="onSuccess">Called when initialization succeeds</param>
/// <param name="onError">Called when initialization fails with error message</param>
public static void Init(string token, string uid, Action onSuccess = null, Action<string> onError = null)
{
    if (string.IsNullOrEmpty(token))
        throw new ArgumentException("Token cannot be null or empty", nameof(token));
    if (string.IsNullOrEmpty(uid))
        throw new ArgumentException("User ID cannot be null or empty", nameof(uid));

    // ... implementation
}
```

**Testing:**
- [ ] Create comprehensive test scene with all API methods
- [ ] Test error cases (invalid token, network failures, etc.)
- [ ] Test thread safety under stress
- [ ] Test IL2CPP builds on multiple devices
- [ ] Performance testing (callback latency, memory usage)

### 🔲 4. Documentation

**Technical Documentation:**
- [ ] API reference for all public methods
- [ ] Architecture overview diagram
- [ ] Callback flow diagrams
- [ ] Threading model explanation
- [ ] Common issues and troubleshooting

**Integration Guide:**
- [ ] Step-by-step setup instructions
- [ ] Android-specific setup (Gradle, permissions)
- [ ] iOS-specific setup (Podfile, Info.plist)
- [ ] Example implementations
- [ ] Migration guide from old UnitySendMessage version

**Code Examples:**
- [ ] Basic integration example
- [ ] Advanced usage (tags, custom parameters)
- [ ] Error handling patterns
- [ ] Testing callbacks in editor

### 🔲 5. CI/CD Pipeline

**Automated Building:**
- [ ] Set up GitHub Actions or similar CI
- [ ] Automate Android AAR building
- [ ] Automate iOS framework building
- [ ] Automate Unity package creation
- [ ] Version synchronization between Android/iOS/Unity

**Automated Testing:**
- [ ] Unit tests for callback logic
- [ ] Integration tests with mock Android SDK
- [ ] IL2CPP build tests
- [ ] Multi-platform build verification

### 🔲 6. Versioning and Release Strategy

**Version Management:**
- [ ] Decide on versioning scheme (semantic versioning)
- [ ] Synchronize versions across:
  - Android SDK
  - iOS SDK
  - Unity package
- [ ] Document breaking changes

**Release Process:**
- [ ] Create release checklist
- [ ] Automate changelog generation
- [ ] Tag releases in git
- [ ] Publish to package registry
- [ ] Announce releases

---

## Known Issues

### None Currently
All major issues have been resolved. The callback implementation is stable and tested.

---

## References

### Unity Documentation
- [AndroidJavaProxy](https://docs.unity3d.com/6000.2/Documentation/ScriptReference/AndroidJavaProxy.html)
- [AndroidJavaObject](https://docs.unity3d.com/6000.2/Documentation/ScriptReference/AndroidJavaObject.html)
- [Unity Package Manager](https://docs.unity3d.com/Manual/Packages.html)

### Kotlin/JVM
- [Inline Value Classes](https://kotlinlang.org/docs/inline-classes.html)
- [JVM Name Mangling](https://github.com/Kotlin/KEEP/blob/master/proposals/inline-classes.md)
- [Coroutines Dispatchers](https://kotlinlang.org/docs/coroutine-context-and-dispatchers.html)

### Project Locations
- **Android SDK:** `/Users/omar.raad/StudioProjects/bitlabs-android-library/`
- **Unity SDK:** `/Users/omar.raad/UnityProjects/bitlabs-unity/`
- **This Document:** `/Users/omar.raad/UnityProjects/bitlabs-unity/IMPLEMENTATION_SUMMARY.md`

---

## Contact and Maintenance

**For Future AI Context:**
- This implementation was completed and tested on 2025-10-29
- All callbacks are working correctly with concrete interfaces
- Generic interfaces were tested and confirmed incompatible with Unity's JNI
- Kotlin name mangling from `runCatching` was identified and resolved

**When resuming this project:**
1. Read this document first for full context
2. Review the TODO section for remaining work
3. Check git history for any changes since this document was created
4. Test the current implementation before making changes

---

*Last Updated: 2025-10-29*
*Status: Android implementation complete and tested ✅*
