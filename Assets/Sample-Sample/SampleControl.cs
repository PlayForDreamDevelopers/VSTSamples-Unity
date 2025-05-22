using UnityEngine;
using YVR.Core;
using YVR.Enterprise.Camera;

namespace YVR.Samples
{
    public class SampleControl : MonoBehaviour
    {
        private void Start()
        {
            YVRManager.instance.hmdManager.SetPassthrough(true);
            Debug.Log("Open SetPassthrough");
            // YVRVSTCameraPlugin.SetVSTCameraFrequency(VSTCameraFrequencyType.VSTFrequency30Hz);
            // YVRVSTCameraPlugin.SetVSTCameraResolution(VSTCameraResolutionType.VSTResolution660_616);
            // YVRVSTCameraPlugin.SetVSTCameraFormat(VSTCameraFormatType.VSTCameraFmtNv21);
            // YVRVSTCameraPlugin.SetVSTCameraOutputSource(VSTCameraSourceType.VSTCameraBothEyes);
            // YVRVSTCameraPlugin.OpenVSTCamera();
        }
    }
}