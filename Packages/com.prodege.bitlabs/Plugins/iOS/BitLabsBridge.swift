import Foundation
import BitLabs

/// Swift bridge for Unity integration with BitLabs SDK.
/// This class provides @objc-compatible methods that can be called from Objective-C++.
/// Unity will generate a bridging header for this file automatically.
@objc public class BitLabsBridge: NSObject {

    private static let bitlabs = BitLabs.shared

    // MARK: - Initialization

    @objc public static func initialize(
        token: String,
        uid: String,
        onSuccess: @escaping () -> Void,
        onError: @escaping (String) -> Void
    ) {
        bitlabs.configure(
            token: token,
            uid: uid,
            onSuccess: onSuccess,
            onError: onError
        )
    }

    // MARK: - Surveys

    @objc public static func checkSurveys(
        onSuccess: @escaping (Bool) -> Void,
        onError: @escaping (String) -> Void
    ) {
        bitlabs.checkSurveys(
            onSuccess: onSuccess,
            onError: onError
        )
    }

    @objc public static func getSurveys(
        onSuccess: @escaping (String) -> Void,
        onError: @escaping (String) -> Void
    ) {
        bitlabs.getSurveys(
            onSuccess: { surveys in
                let array = surveys.map { $0.asDictionary() }
                if let jsonData = try? JSONSerialization.data(withJSONObject: array, options: .prettyPrinted),
                   let jsonString = String(data: jsonData, encoding: .utf8) {
                    onSuccess(jsonString)
                } else {
                    onError("Failed to serialize surveys to JSON")
                }
            },
            onError: onError
        )
    }

    // MARK: - Reward Handler

    @objc public static func setRewardHandler(
        onReward: @escaping (Float) -> Void
    ) {
        bitlabs.setRewardCompletionHandler(onReward)
    }

    // MARK: - UI

    @objc public static func launchOfferWall(parent: UIViewController) {
        bitlabs.launchOfferWall(parent: parent)
    }

    // MARK: - Configuration

    @objc public static func setDebugMode(_ isDebug: Bool) {
        bitlabs.setIsDebugMode(isDebug)
    }

    @objc public static func setTags(_ tags: [String: String]) {
        bitlabs.setTags(tags)
    }

    @objc public static func addTag(key: String, value: String) {
        bitlabs.addTag(key: key, value: value)
    }
    
    // MARK: - Permissions

    @objc public static func requestTrackingAuthorization() {
        bitlabs.requestTrackingAuthorization()
    }
}
