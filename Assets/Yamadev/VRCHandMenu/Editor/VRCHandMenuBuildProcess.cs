using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using VRC.SDKBase;
using Yamadev.VRCHandMenu.Script;

namespace Yamadev.VRCHandMenu.Editor
{
    public class VRCHandMenuBuildProcess : IProcessSceneWithReport
    {
        public int callbackOrder => -1;
        public void OnProcessScene(Scene scene, BuildReport report)
        {
            SetMainCamera();
        }

        void SetMainCamera()
        {
            GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            if (!mainCamera) return;

            Camera targetCamera = mainCamera.GetComponent<Camera>();

            MenuHandle[] handles = Resources.FindObjectsOfTypeAll<MenuHandle>();
            if (handles.Length == 0) return;
            handles[0].SetVariable("targetCamera", targetCamera);

            VRC_SceneDescriptor desc = GameObject.Find("VRCWorld").GetComponent<VRC_SceneDescriptor>();
            if (!desc || desc.ReferenceCamera && desc.ReferenceCamera.GetComponent<PostProcessLayer>()) return;

            if (mainCamera.GetComponent<PostProcessLayer>() == null)
            {
                PostProcessLayer postProcessLayer = mainCamera.AddComponent<PostProcessLayer>();
                postProcessLayer.volumeTrigger = mainCamera.transform;
                postProcessLayer.volumeLayer = LayerMask.NameToLayer("PostProcessing");
            }
            
            desc.ReferenceCamera = mainCamera;
        }
    }
}