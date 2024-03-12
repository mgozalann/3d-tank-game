﻿using System;
using UnityEngine;

namespace Complete
{
    public class CameraControl : MonoBehaviour
    {
        public float m_DampTime = 0.2f; // Approximate time for the camera to refocus.
        public float m_ScreenEdgeBuffer = 4f; // Space between the top/bottom most target and the screen edge.
        public float m_MinSize = 6.5f; // The smallest orthographic size the camera can be.
        [HideInInspector] public Transform[] m_Targets; // All the targets the camera needs to encompass.

        private Camera _camera; // Used for referencing the camera.
        private float _zoomSpeed; // Reference speed for the smooth damping of the orthographic size.
        private Vector3 _moveVelocity; // Reference velocity for the smooth damping of the position.
        private Vector3 _desiredPosition; // The position the camera is moving towards.


        private void Awake()
        {
            _camera = GetComponentInChildren<Camera>();
        }

        private void OnEnable()
        {
            GameManager.Current.OnTargetFocused += OnTargetFocused;
        }

        private void OnDisable()
        {
            GameManager.Current.OnTargetFocused -= OnTargetFocused;
        }

        private void OnTargetFocused(Vector3 target)
        {
          //  _desiredPosition = target;
        }

        private void FixedUpdate()
        {
            // if (GameManager.Current.IsFocused)
            // {
            //     transform.position =
            //         Vector3.SmoothDamp(transform.position, _desiredPosition, ref _moveVelocity, m_DampTime);
            // }
            // else
           // {
                Move();
            //}
            Zoom();
        }


        private void Move()
        {
            // Find the average position of the targets.
            FindAveragePosition();

            // Smoothly transition to that position.
            transform.position =
                Vector3.SmoothDamp(transform.position, _desiredPosition, ref _moveVelocity, m_DampTime);
            
        }


        private void FindAveragePosition()
        {
            Vector3 averagePos = new Vector3();
            int numTargets = 0;

            // Go through all the targets and add their positions together.
            for (int i = 0; i < m_Targets.Length; i++)
            {
                // If the target isn't active, go on to the next one.
                if (!m_Targets[i].gameObject.activeSelf)
                    continue;

                // Add to the average and increment the number of targets in the average.
                averagePos += m_Targets[i].position;
                numTargets++;
            }

            // If there are targets divide the sum of the positions by the number of them to find the average.
            if (numTargets > 0)
                averagePos /= numTargets;

            // Keep the same y value.
            averagePos.y = transform.position.y;
            //averagePos.x -= 10;

            // The desired position is the average position;
            _desiredPosition = averagePos;
        }


        private void Zoom()
        {
            // Find the required size based on the desired position and smoothly transition to that size.
            float requiredSize = FindRequiredSize() * .6f;
            _camera.orthographicSize =
                Mathf.SmoothDamp(_camera.orthographicSize, requiredSize, ref _zoomSpeed, m_DampTime);
        }


        private float FindRequiredSize()
        {
            // Find the position the camera rig is moving towards in its local space.
            Vector3 centerX = Vector3.left * 10;
            Vector3 desiredLocalPos = transform.InverseTransformPoint(_desiredPosition + centerX);

            // Start the camera's size calculation at zero.
            float size = 0f;

            // Go through all the targets...
            for (int i = 0; i < m_Targets.Length; i++)
            {
                // ... and if they aren't active continue on to the next target.
                if (!m_Targets[i].gameObject.activeSelf)
                    continue;

                // Otherwise, find the position of the target in the camera's local space.
                Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

                // Find the position of the target from the desired position of the camera's local space.
                Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

                // Choose the largest out of the current size and the distance of the tank 'up' or 'down' from the camera.
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

                // Choose the largest out of the current size and the calculated size based on the tank being to the left or right of the camera.
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / _camera.aspect);
            }

            // Add the edge buffer to the size.
            size += m_ScreenEdgeBuffer;

            // Make sure the camera's size isn't below the minimum.
            size = Mathf.Max(size, m_MinSize);

            return size;
        }


        public void SetStartPositionAndSize()
        {
            // Find the desired position.
            FindAveragePosition();

            // Set the camera's position to the desired position without damping.
            transform.position = _desiredPosition;

            // Find and set the required size of the camera.
            _camera.orthographicSize = FindRequiredSize();
        }
    }
}