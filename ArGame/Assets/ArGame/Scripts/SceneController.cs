using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneController : MonoBehaviour {
   public Camera FirstPersonCamera;
    private bool m_IsQuitting = false;
     

    void Update () {
        _UpdateApplicationLifecycle ();
    }

    private void _UpdateApplicationLifecycle () {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey (KeyCode.Escape)) {
            Application.Quit ();
        }

        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking) {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        } else {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting) {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to
        // appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted) {
            _ShowAndroidToastMessage ("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke ("_DoQuit", 0.5f);
        } else if (Session.Status.IsError ()) {
            _ShowAndroidToastMessage (
                "ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke ("_DoQuit", 0.5f);
        }
    }

    private void _ShowAndroidToastMessage (string message) {
        AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");

        if (unityActivity != null) {
            AndroidJavaClass toastClass = new AndroidJavaClass ("android.widget.Toast");
            unityActivity.Call ("runOnUiThread", new AndroidJavaRunnable (() => {
                AndroidJavaObject toastObject =
                    toastClass.CallStatic<AndroidJavaObject> (
                        "makeText", unityActivity, message, 0);
                toastObject.Call ("show");
            }));
        }
    }
}