// Copyright (c) Vismir LLC. All rights reserved.

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Vismir
{
    public class UIController_HandVis : MonoBehaviour
    {
        public bool isHandMeshVisible = false;
        public bool isHandJointVisible = false;

        private IMixedRealityInputSystem inputSystem = null;
        protected IMixedRealityInputSystem InputSystem
        {
            get
            {
                MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                return inputSystem;
            }
        }

        private void UpdateHandVisibility()
        {
            MixedRealityHandTrackingProfile handTrackingProfile = InputSystem?.InputSystemProfile?.HandTrackingProfile;
            if (handTrackingProfile != null)
            { 
                handTrackingProfile.EnableHandMeshVisualization = isHandMeshVisible;
                handTrackingProfile.EnableHandJointVisualization = isHandJointVisible;
            }
        }

        /// <summary>
        /// Initial setting of hand mesh visualization - default is disabled
        /// </summary>
        void Start()
        {
            // Initial setting of the hand visualization states
            UpdateHandVisibility();
        }

        #region Hand mesh
        /// <summary>
        /// Show hand mesh visualization
        /// </summary>
        public void Show_HandMesh()
        {
            Update_HandMesh(true);
        }


        /// <summary>
        /// Hide hand mesh visualization
        /// </summary>
        public void Hide_HandMesh()
        {
            Update_HandMesh(false);
        }

        /// <summary>
        /// Toggle hand mesh visualization
        /// </summary>
        public void Toggle_HandMesh()
        {
            Update_HandMesh(!isHandMeshVisible);
        }

        /// <summary>
        /// Updates the hand mesh visibility to the given boolean
        /// </summary>
        private void Update_HandMesh(bool show)
        {
            isHandMeshVisible = show;
            UpdateHandVisibility();
        }
        #endregion

        #region Hand joints
        /// <summary>
        /// Show hand mesh visualization
        /// </summary>
        public void Show_HandJoints()
        {
            Update_HandJoints(true);
        }


        /// <summary>
        /// Hide hand mesh visualization
        /// </summary>
        public void Hide_HandJoints()
        {
            Update_HandJoints(false);
        }

        /// <summary>
        /// Toggle hand mesh visualization
        /// </summary>
        public void Toggle_HandJoints()
        {
            Update_HandJoints(!isHandJointVisible);
        }

        /// <summary>
        /// Updates the hand mesh visibility to the given boolean
        /// </summary>
        private void Update_HandJoints(bool show)
        {
            isHandJointVisible = show;
            UpdateHandVisibility();
        }
        #endregion
    }
}
