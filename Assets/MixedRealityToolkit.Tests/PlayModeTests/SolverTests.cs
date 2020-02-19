﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class SolverTests : BasePlayModeTests
    {
        private const float DistanceThreshold = 1.5f;
        private const float HandDistanceThreshold = 0.5f;
        private const float SolverUpdateWaitTime = 1.0f; //seconds

        /// <summary>
        /// Internal class used to store data for setup
        /// </summary>
        protected class SetupData
        {
            public SolverHandler handler;
            public Solver solver;
            public GameObject target;
        }

        private List<SetupData> setupDataList = new List<SetupData>();

        [TearDown]
        public override void TearDown()
        {
            foreach (var setupData in setupDataList)
            {
                Object.Destroy(setupData?.target);
            }

            base.TearDown();
        }

        /// <summary>
        /// Test adding solver dynamically at runtime to GameObject
        /// </summary>
        [UnityTest]
        public IEnumerator TestRuntimeInstantiation()
        {
            InstantiateTestSolver<Orbital>();

            yield return null;
        }

        /// <summary>
        /// Test solver system's ability to change target types at runtime
        /// </summary>
        [UnityTest]
        public IEnumerator TestTargetTypes()
        {
            Vector3 rightHandPos = Vector3.right * 50.0f;
            Vector3 leftHandPos = -rightHandPos;
            Vector3 customTransformPos = Vector3.up * 50.0f;

            var transformOverride = new GameObject("Override");
            transformOverride.transform.position = customTransformPos;

            var testObjects = InstantiateTestSolver<Orbital>();
            InputSimulationService inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();

            // Test orbital around right hand
            {
                testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;
                yield return TestHandSolver(testObjects, inputSimulationService, rightHandPos, Handedness.Right);
            }

            // Test orbital around left hand line pointer
            {
                testObjects.handler.TrackedTargetType = TrackedObjectType.ControllerRay;
                testObjects.handler.TrackedHandness = Handedness.Left;

                yield return TestHandSolver(testObjects, inputSimulationService, leftHandPos, Handedness.Left);
            }

            // Test orbital around head
            {
                testObjects.handler.TrackedTargetType = TrackedObjectType.Head;

                yield return WaitForFrames(2);

                Assert.LessOrEqual(Vector3.Distance(testObjects.target.transform.position, CameraCache.Main.transform.position), DistanceThreshold);
            }

            // Test orbital around custom override
            {
                testObjects.handler.TrackedTargetType = TrackedObjectType.CustomOverride;
                testObjects.handler.TransformOverride = transformOverride.transform;

                yield return WaitForFrames(2);

                Assert.LessOrEqual(Vector3.Distance(testObjects.target.transform.position, customTransformPos), DistanceThreshold);

                yield return WaitForFrames(2);
            }
        }

        /// <summary>
        /// Tests solver handler's ability to switch hands
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandModality()
        {
            var testObjects = InstantiateTestSolver<Orbital>();

            // Set solver handler to track hands
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;

            // Set and save relevant positions
            Vector3 rightHandPos = Vector3.right * 20.0f;
            Vector3 leftHandPos = Vector3.right * -20.0f;

            yield return WaitForFrames(2);

            InputSimulationService inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();

            // Test orbital around right hand
            yield return TestHandSolver(testObjects, inputSimulationService, rightHandPos, Handedness.Right);

            // Test orbital around left hand
            yield return TestHandSolver(testObjects, inputSimulationService, leftHandPos, Handedness.Left);

            // Test orbital with both hands visible
            yield return PlayModeTestUtilities.ShowHand(Handedness.Left, inputSimulationService, Utilities.ArticulatedHandPose.GestureId.Open, leftHandPos);
            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService, Utilities.ArticulatedHandPose.GestureId.Open, rightHandPos);

            // Give time for cube to float to hand
            yield return WaitForFrames(2);

            Vector3 handOrbitalPos = testObjects.target.transform.position;
            Assert.LessOrEqual(Vector3.Distance(handOrbitalPos, leftHandPos), DistanceThreshold);
        }

        /// <summary>
        /// Test Surface Magnetism against "wall" and that attached object falls head direction
        /// </summary>
        [UnityTest]
        public IEnumerator TestSurfaceMagnetism()
        {
            // Reset view to origin
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.zero;
                p.LookAt(Vector3.forward);
            });

            // Build wall to collide against
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.localScale = new Vector3(25.0f, 25.0f, 0.2f);
            wall.transform.Rotate(Vector3.up, 180.0f); // Rotate wall so forward faces camera
            wall.transform.position = Vector3.forward * 10.0f;

            yield return WaitForFrames(2);

            // Instantiate our test GameObject with solver. 
            // Set layer to ignore raycast so solver doesn't raycast itself (i.e BoxCollider)
            var testObjects = InstantiateTestSolver<SurfaceMagnetism>();
            testObjects.target.layer = LayerMask.NameToLayer("Ignore Raycast");
            SurfaceMagnetism surfaceMag = testObjects.solver as SurfaceMagnetism;

            var targetTransform = testObjects.target.transform;
            var cameraTransform = CameraCache.Main.transform;

            yield return WaitForFrames(2);

            // Confirm that the surfacemagnetic cube is about on the wall straight ahead
            Assert.LessOrEqual(Vector3.Distance(targetTransform.position, wall.transform.position), DistanceThreshold);

            // Rotate the camera
            Vector3 cameraDir = Vector3.forward + Vector3.right;
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.zero;
                p.LookAt(cameraDir);
            });

            // Calculate where our camera hits the wall
            RaycastHit hitInfo;
            Assert.IsTrue(UnityEngine.Physics.Raycast(Vector3.zero, cameraDir, out hitInfo), "Raycast from camera did not hit wall");

            // Let SurfaceMagnetism update
            yield return WaitForFrames(2);

            // Confirm that the surfacemagnetic cube is on the wall with camera rotated
            Assert.LessOrEqual(Vector3.Distance(targetTransform.position, hitInfo.point), DistanceThreshold);

            // Default orientation mode is TrackedTarget, test object should be facing camera
            Assert.IsTrue(Mathf.Approximately(-1.0f, Vector3.Dot(targetTransform.forward.normalized, cameraTransform.forward.normalized)));

            // Change default orientation mode to surface normal
            surfaceMag.CurrentOrientationMode = SurfaceMagnetism.OrientationMode.SurfaceNormal;

            yield return WaitForFrames(2);

            // Test object should now be facing into the wall (i.e Z axis)
            Assert.IsTrue(Mathf.Approximately(1.0f, Vector3.Dot(targetTransform.forward.normalized, Vector3.forward)));
        }

        /// <summary>
        /// Test solver system's ability to change target types at runtime
        /// </summary>
        [UnityTest]
        public IEnumerator TestInBetween()
        {
            // Build "posts" to put solved object between
            var leftPost = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leftPost.transform.position = Vector3.forward * 10.0f - Vector3.right * 10.0f;

            var rightPost = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightPost.transform.position = Vector3.forward * 10.0f + Vector3.right * 10.0f;

            // Instantiate our test GameObject with solver. 
            var testObjects = InstantiateTestSolver<InBetween>();

            testObjects.handler.TrackedTargetType = TrackedObjectType.CustomOverride;
            testObjects.handler.TransformOverride = leftPost.transform;

            InBetween inBetween = testObjects.solver as InBetween;
            Assert.IsNotNull(inBetween, "Solver cast to InBetween is null");

            inBetween.SecondTrackedObjectType = TrackedObjectType.CustomOverride;
            inBetween.SecondTransformOverride = rightPost.transform;

            // Let InBetween update
            yield return WaitForFrames(2);

            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.forward * 10.0f, "InBetween solver did not place object in middle of posts");

            inBetween.PartwayOffset = 0.0f;

            // Let InBetween update
            yield return WaitForFrames(2);

            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, rightPost.transform.position, "InBetween solver did not move to the left post");
        }

        /// <summary>
        /// Test the HandConstraint to make sure it tracks hands correctly.
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandConstraint()
        {
            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<HandConstraint>();
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;
            testObjects.handler.TrackedHandness = Handedness.Both;

            yield return new WaitForSeconds(SolverUpdateWaitTime);

            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.zero, "HandConstraint solver did not start at the origin");

            // Add a right hand.
            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(Vector3.zero);

            // Move the hand to 0, 0, 1 and ensure that the hand constraint followed.
            var handPosition = Vector3.forward;
            yield return rightHand.MoveTo(handPosition);

            yield return new WaitForSeconds(SolverUpdateWaitTime);

            // Make sure the solver is not in the same location as the hand because the solver should move to a hand safe zone.
            TestUtilities.AssertNotAboutEqual(testObjects.target.transform.position, handPosition, "HandConstraint solver is in the same location of the hand when it should be slightly offset from the hand.");

            // Make sure the solver is near the hand.
            Assert.LessOrEqual(Vector3.Distance(testObjects.target.transform.position, handPosition), HandDistanceThreshold, "HandConstraint solver is not within {0} units of the hand", HandDistanceThreshold);

            // Hide the right hand and create a left hand.
            yield return rightHand.Hide();
            var leftHand = new TestHand(Handedness.Left);
            handPosition = Vector3.zero;
            yield return leftHand.Show(handPosition);

            yield return new WaitForSeconds(SolverUpdateWaitTime);

            // Make sure the solver is now near the other hand.
            Assert.LessOrEqual(Vector3.Distance(testObjects.target.transform.position, handPosition), HandDistanceThreshold, "HandConstraint solver is not within {0} units of the hand", HandDistanceThreshold);
        }

        /// <summary>
        /// Test the HandConstraintPalm up to make sure the FollowHandUntilFacingCamera behavior works as expected
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandConstraintPalmUpSolverPlacement()
        {
            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<HandConstraintPalmUp>();
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;
            testObjects.handler.TrackedHandness = Handedness.Both;

            var handConstraintSolver = (HandConstraintPalmUp) testObjects.solver;
            handConstraintSolver.FollowHandUntilFacingCamera = true;

            // Ensure that FacingCameraTrackingThreshold is greater than FollowHandCameraFacingThresholdAngle
            Assert.AreEqual(handConstraintSolver.FacingCameraTrackingThreshold - handConstraintSolver.FollowHandCameraFacingThresholdAngle > 0, true);

            yield return new WaitForSeconds(SolverUpdateWaitTime);

            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.zero, "HandConstraintPalmUp solver did not start at the origin");

            var cameraTransform = CameraCache.Main.transform;

            // Place hand 1 meter in front of user, 50 cm below eye level
            var handTestPos = cameraTransform.position + cameraTransform.forward - (Vector3.up * 0.5f);
            
            var cameraLookVector = (handTestPos - cameraTransform.position).normalized;

            // Generate hand rotation with hand palm facing camera
            var handRoation = Quaternion.LookRotation(cameraTransform.up, cameraLookVector);

            // Add a right hand.
            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(handTestPos);
            yield return rightHand.SetRotation(handRoation);

            yield return new WaitForSeconds(SolverUpdateWaitTime);

            // Ensure Rotation and offset behavior are following camera
            Assert.AreEqual(handConstraintSolver.RotationBehavior, HandConstraint.SolverRotationBehavior.LookAtMainCamera);
            Assert.AreEqual(handConstraintSolver.OffsetBehavior, HandConstraint.SolverOffsetBehavior.LookAtCameraRotation);

            // Rotate hand so palm is no longer within the FollowHandCameraFacingThresholdAngle
            var newHandRot = handRoation * Quaternion.Euler(-(handConstraintSolver.FollowHandCameraFacingThresholdAngle + 1), 0f, 0f);
            yield return rightHand.SetRotation(newHandRot);

            yield return new WaitForSeconds(SolverUpdateWaitTime);

            // Ensure Rotation and offset behavior are following camera
            Assert.AreEqual(handConstraintSolver.RotationBehavior, HandConstraint.SolverRotationBehavior.LookAtTrackedObject);
            Assert.AreEqual(handConstraintSolver.OffsetBehavior, HandConstraint.SolverOffsetBehavior.TrackedObjectRotation);

            yield return rightHand.Hide();

            yield return new WaitForSeconds(SolverUpdateWaitTime);
        }

        /// <summary>
        /// Test the Overlap solver and make sure it tracks the left simulated hand exactly
        /// </summary>
        [UnityTest]
        public IEnumerator TestOverlap()
        {
            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<Overlap>();
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;
            var targetTransform = testObjects.target.transform;

            TestUtilities.AssertAboutEqual(targetTransform.position, Vector3.zero, "Overlap not at original position");
            TestUtilities.AssertAboutEqual(targetTransform.rotation, Quaternion.identity, "Overlap not at original rotation");

            // Test that the solver flies to the position of the left hand
            var handPosition = Vector3.forward - Vector3.right;
            var handRotation = Quaternion.LookRotation(handPosition);
            var leftHand = new TestHand(Handedness.Left);
            yield return leftHand.Show(handPosition);
            yield return leftHand.SetRotation(handRotation);
            
            yield return WaitForFrames(2);
            var hand = PlayModeTestUtilities.GetInputSimulationService().GetHandDevice(Handedness.Left);
            Assert.IsNotNull(hand);
            Assert.IsTrue(hand.TryGetJoint(TrackedHandJoint.Palm, out MixedRealityPose pose));

            TestUtilities.AssertAboutEqual(targetTransform.position, pose.Position, "Overlap solver is not at the same position as the left hand.");
            Assert.IsTrue(Quaternion.Angle(targetTransform.rotation, pose.Rotation) < 2.0f);

            // Make sure the solver did not move when hand was hidden
            yield return leftHand.Hide();
            yield return WaitForFrames(2);
            TestUtilities.AssertAboutEqual(targetTransform.position, pose.Position, "Overlap solver moved when the hand was hidden.");
            Assert.IsTrue(Quaternion.Angle(targetTransform.rotation, pose.Rotation) < 2.0f);
        }

        /// <summary>
        /// Test solver system's ability to add multiple solvers at runtime and switch between them.
        /// </summary>
        [UnityTest]
        public IEnumerator TestSolverSwap()
        {
            // Reset view to origin
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.zero;
                p.LookAt(Vector3.forward);
            });

            // Instantiate and setup RadialView to place object in the view center.
            var testObjects = InstantiateTestSolver<RadialView>();
            RadialView radialViewSolver = (RadialView)testObjects.solver;
            radialViewSolver.MinDistance = 2.0f;
            radialViewSolver.MaxDistance = 2.0f;
            radialViewSolver.MinViewDegrees = 0.0f;
            radialViewSolver.MaxViewDegrees = 0.0f;

            // Let RadialView update the target object
            yield return WaitForFrames(2);

            // Make sure Radial View is placing object in center of View, so we can later check that a solver swap actually moved the target object.
            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.forward * 2.0f, "RadialView does not place object in center of view");

            // Disable the old solver
            radialViewSolver.enabled = false;

            // Add a another solver during runtime, give him a specific location to check whether the new solver updates the target object.
            Orbital orbitalSolver = AddSolverComponent<Orbital>(testObjects.target);
            orbitalSolver.WorldOffset = Vector3.zero;
            orbitalSolver.LocalOffset = Vector3.down * 2.0f;

            // Let Orbital update the target object
            yield return WaitForFrames(2);

            // Make sure Orbital is now updating the target object
            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.down * 2.0f, "Orbital solver did not place object below origin");

            // Swap solvers once again during runtime
            radialViewSolver.enabled = true;
            orbitalSolver.enabled = false;

            // Let RadialView update the target object
            yield return WaitForFrames(2);

            // Make sure Radial View is now updating the target object once again.
            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.forward * 2.0f, "RadialView solver did not place object in center of view");
        }


        #region Experimental

        /// <summary>
        /// Tests that the DirectionalIndicator can be instatiated through code.
        /// </summary>
        [UnityTest]
        public IEnumerator TestDirectionalIndicator()
        {
            // Reset view to origin
            TestUtilities.PlayspaceToOriginLookingForward();

            const float ANGLE_THRESHOLD = 30.0f;

            var directionTarget = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            directionTarget.transform.position = 10.0f * Vector3.right; 

            // Instantiate our test gameobject with solver.
            var testObjects = InstantiateTestSolver<DirectionalIndicator>();

            var indicatorSolver = testObjects.solver as DirectionalIndicator;
            indicatorSolver.DirectionalTarget = directionTarget.transform;

            var indicatorMesh = indicatorSolver.GetComponent<Renderer>();

            // Test that solver points to the right and is visible
            yield return WaitForFrames(2);
            Assert.LessOrEqual(Vector3.Angle(indicatorSolver.transform.up, directionTarget.transform.position.normalized), ANGLE_THRESHOLD);
            Assert.IsTrue(indicatorMesh.enabled);

            directionTarget.transform.position = -10.0f * Vector3.right;

            // Test that solver points to the left now and is visible
            yield return WaitForFrames(2);
            Assert.LessOrEqual(Vector3.Angle(indicatorSolver.transform.up, directionTarget.transform.position.normalized), ANGLE_THRESHOLD);
            Assert.IsTrue(indicatorMesh.enabled);

            // Test that the solver is invisible
            directionTarget.transform.position = 5.0f * Vector3.forward;

            yield return WaitForFrames(2);
            Assert.IsFalse(indicatorMesh.enabled);

            // Get back to a position where the directional indicator should be visible
            directionTarget.transform.position = -10.0f * Vector3.right;
            yield return WaitForFrames(2);
            Assert.LessOrEqual(Vector3.Angle(indicatorSolver.transform.up, directionTarget.transform.position.normalized), ANGLE_THRESHOLD);
            Assert.IsTrue(indicatorMesh.enabled);

            // Destroy the object and then validate that the mesh is no longer visible
            Object.Destroy(directionTarget);
            yield return null;
            Assert.IsFalse(indicatorMesh.enabled);
        }

        /// <summary>
        /// Test the Follow solver distance clamp options
        /// </summary>
        [UnityTest]
        public IEnumerator TestFollowDistance()
        {
            // Reset view to origin
            TestUtilities.PlayspaceToOriginLookingForward();

            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<Follow>();
            var followSolver = (Follow)testObjects.solver;
            followSolver.MoveToDefaultDistanceLerpTime = 0;
            testObjects.handler.TrackedTargetType = TrackedObjectType.Head;
            var targetTransform = testObjects.target.transform;

            yield return new WaitForFixedUpdate();
            yield return null;

            // Test distance remains within min/max bounds
            float distanceToHead = Vector3.Distance(targetTransform.position, CameraCache.Main.transform.position);
            Assert.LessOrEqual(distanceToHead, followSolver.MaxDistance, "Follow exceeded max distance");
            Assert.GreaterOrEqual(distanceToHead, followSolver.MinDistance, "Follow subceeded min distance");

            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.back * 2;
            });

            yield return new WaitForFixedUpdate();
            yield return null;

            distanceToHead = Vector3.Distance(targetTransform.position, CameraCache.Main.transform.position);
            Assert.LessOrEqual(distanceToHead, followSolver.MaxDistance, "Follow exceeded max distance");
            Assert.GreaterOrEqual(distanceToHead, followSolver.MinDistance, "Follow subceeded min distance");

            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.forward * 4;
            });

            yield return new WaitForFixedUpdate();
            yield return null;

            distanceToHead = Vector3.Distance(targetTransform.position, CameraCache.Main.transform.position);
            Assert.LessOrEqual(distanceToHead, followSolver.MaxDistance, "Follow exceeded max distance");
            Assert.GreaterOrEqual(distanceToHead, followSolver.MinDistance, "Follow subceeded min distance");

            // Test VerticalMaxDistance
            followSolver.VerticalMaxDistance = 0.1f;
            targetTransform.position = Vector3.forward;
            targetTransform.rotation = Quaternion.identity;
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.zero;
                p.LookAt(Vector3.forward);
            });

            yield return new WaitForFixedUpdate();
            yield return null;

            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.LookAt(Vector3.forward + Vector3.up);
            });

            yield return new WaitForFixedUpdate();
            yield return null;

            float yDistance = targetTransform.position.y - CameraCache.Main.transform.position.y;
            Assert.AreEqual(followSolver.VerticalMaxDistance, yDistance);

            followSolver.VerticalMaxDistance = 0f;
        }

        /// <summary>
        /// Test the Follow solver orientation options
        /// </summary>
        [UnityTest]
        public IEnumerator TestFollowOrientation()
        {
            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<Follow>();
            var followSolver = (Follow)testObjects.solver;
            followSolver.MoveToDefaultDistanceLerpTime = 0;
            testObjects.handler.TrackedTargetType = TrackedObjectType.Head;
            var targetTransform = testObjects.target.transform;

            // Test orientation deadzone
            followSolver.OrientToControllerDeadzoneDegrees = 70;
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.back;
                p.LookAt(Vector3.forward);
            });

            yield return new WaitForFixedUpdate();
            yield return null;
            
            Assert.AreEqual(targetTransform.rotation, Quaternion.identity, "Target rotated before we moved beyond the deadzone");

            MixedRealityPlayspace.PerformTransformation(p => p.RotateAround(Vector3.zero, Vector3.up, 45));
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.AreEqual(targetTransform.rotation, Quaternion.identity, "Target rotated before we moved beyond the deadzone");

            MixedRealityPlayspace.PerformTransformation(p => p.RotateAround(Vector3.zero, Vector3.up, 45));
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.AreNotEqual(targetTransform.rotation, Quaternion.identity, "Target did not rotate after we moved beyond the deadzone");

            // Test FaceUserDefinedTargetTransform
            var hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.forward + Vector3.right);
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;
            followSolver.FaceUserDefinedTargetTransform = true;
            followSolver.TargetToFace = CameraCache.Main.transform;

            Assert.AreEqual(Quaternion.LookRotation(targetTransform.position - CameraCache.Main.transform.position), targetTransform.rotation);

            yield return hand.MoveTo(Vector3.forward + Vector3.left, 1);
            yield return null;

            Assert.AreEqual(Quaternion.LookRotation(targetTransform.position - CameraCache.Main.transform.position), targetTransform.rotation);
        }

        /// <summary>
        /// Test the Follow solver angular clamp options
        /// </summary>
        [UnityTest]
        public IEnumerator TestFollowDirection()
        {
            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<Follow>();
            var followSolver = (Follow)testObjects.solver;
            followSolver.MoveToDefaultDistanceLerpTime = 0;
            testObjects.handler.TrackedTargetType = TrackedObjectType.Head;
            var targetTransform = testObjects.target.transform;

            // variables and lambdas to test direction remains within bounds
            var maxXAngle = followSolver.MaxViewHorizontalDegrees / 2;
            var maxYAngle = followSolver.MaxViewVerticalDegrees / 2;
            Vector3 directionToHead() => CameraCache.Main.transform.position - targetTransform.position;
            float xAngle() => (Mathf.Acos(Vector3.Dot(directionToHead(), targetTransform.right)) * Mathf.Rad2Deg) - 90;
            float yAngle() => 90 - (Mathf.Acos(Vector3.Dot(directionToHead(), targetTransform.up)) * Mathf.Rad2Deg);

            // Test without rotation
            TestUtilities.PlayspaceToOriginLookingForward();

            yield return new WaitForFixedUpdate();
            yield return null;
            
            Assert.LessOrEqual(Mathf.Abs(xAngle()), maxXAngle, "Follow exceeded the max horizontal angular bounds");
            Assert.LessOrEqual(Mathf.Abs(yAngle()), maxYAngle, "Follow exceeded the max vertical angular bounds");

            // Test y axis rotation
            MixedRealityPlayspace.PerformTransformation(p => p.Rotate(Vector3.up, 45));
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.LessOrEqual(Mathf.Abs(xAngle()), maxXAngle, "Follow exceeded the max horizontal angular bounds");
            Assert.LessOrEqual(Mathf.Abs(yAngle()), maxYAngle, "Follow exceeded the max vertical angular bounds");
            
            // Test x axis rotation
            MixedRealityPlayspace.PerformTransformation(p => p.Rotate(Vector3.right, 45));
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.LessOrEqual(Mathf.Abs(xAngle()), maxXAngle, "Follow exceeded the max horizontal angular bounds");
            Assert.LessOrEqual(Mathf.Abs(yAngle()), maxYAngle, "Follow exceeded the max vertical angular bounds");

            // Test translation
            MixedRealityPlayspace.PerformTransformation(p => p.Translate(Vector3.back, Space.World));
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.LessOrEqual(Mathf.Abs(xAngle()), maxXAngle, "Follow exceeded the max horizontal angular bounds");
            Assert.LessOrEqual(Mathf.Abs(yAngle()), maxYAngle, "Follow exceeded the max vertical angular bounds");
        }

        /// <summary>
        /// Test the Follow solver angular clamp options
        /// </summary>
        [UnityTest]
        public IEnumerator TestFollowStuckBehind()
        {
            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<Follow>();
            var followSolver = (Follow)testObjects.solver;
            followSolver.MoveToDefaultDistanceLerpTime = 0;
            testObjects.handler.TrackedTargetType = TrackedObjectType.Head;
            var targetTransform = testObjects.target.transform;

            // variables and lambdas to test direction remains within bounds
            Vector3 toTarget() => targetTransform.position - CameraCache.Main.transform.position;

            // Test without rotation
            TestUtilities.PlayspaceToOriginLookingForward();

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.Greater(Vector3.Dot(CameraCache.Main.transform.forward, toTarget()), 0, "Follow behind the player");

            // Test y axis rotation
            MixedRealityPlayspace.PerformTransformation(p => p.Rotate(Vector3.up, 180));
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.Greater(Vector3.Dot(CameraCache.Main.transform.forward, toTarget()), 0, "Follow behind the player");
        }

        #endregion

        #region Test Helpers

        private IEnumerator TestHandSolver(SetupData testData, InputSimulationService inputSimulationService, Vector3 handPos, Handedness hand)
        {
            Assert.IsTrue(testData.handler.TrackedTargetType == TrackedObjectType.ControllerRay 
                || testData.handler.TrackedTargetType == TrackedObjectType.HandJoint, "TestHandSolver supports on ControllerRay and HandJoint tracked target types");

            yield return PlayModeTestUtilities.ShowHand(hand, inputSimulationService, Utilities.ArticulatedHandPose.GestureId.Open, handPos);

            // Give time for cube to float to hand
            yield return WaitForFrames(2);

            Vector3 handOrbitalPos = testData.target.transform.position;
            Assert.LessOrEqual(Vector3.Distance(handOrbitalPos, handPos), DistanceThreshold);

            Transform expectedTransform = null;
            if (testData.handler.TrackedTargetType == TrackedObjectType.ControllerRay)
            {
                expectedTransform = PointerUtils.GetPointer<LinePointer>(hand)?.transform;
            }
            else
            {
                var handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
                expectedTransform = handJointService.RequestJointTransform(testData.handler.TrackedHandJoint, hand);
            }

            Assert.AreEqual(testData.handler.CurrentTrackedHandedness, hand);
            Assert.IsNotNull(expectedTransform);
            
            // SolverHandler creates a dummy GameObject to provide a transform for tracking so it can be managed (allocated/deleted)
            // Look at the parent to compare transform equality for what we should be tracking against
            Assert.AreEqual(testData.handler.TransformTarget.parent, expectedTransform);

            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);

            yield return WaitForFrames(2);
        }

        private SetupData InstantiateTestSolver<T>() where T: Solver
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = typeof(T).Name;
            cube.transform.localScale = new Vector3(0.1f, 0.2f, 0.1f);

            Solver solver = AddSolverComponent<T>(cube);

            SolverHandler handler = cube.GetComponent<SolverHandler>();
            Assert.IsNotNull(handler, "GetComponent<SolverHandler>() returned null");

           var setupData =  new SetupData()
            {
                handler = handler,
                solver = solver,
                target = cube
            };

            setupDataList.Add(setupData);

            return setupData;
        }

        private T AddSolverComponent<T>(GameObject target) where T : Solver
        {
            T solver = target.AddComponent<T>();
            Assert.IsNotNull(solver, "AddComponent<T>() returned null");

            // Set Solver lerp times to 0 so we can process tests faster instead of waiting for transforms to update/apply
            solver.MoveLerpTime = 0.0f;
            solver.RotateLerpTime = 0.0f;
            solver.ScaleLerpTime = 0.0f;

            return solver;
        }

        private IEnumerator WaitForFrames(int frames)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }
        }

#endregion
    }
}
#endif

