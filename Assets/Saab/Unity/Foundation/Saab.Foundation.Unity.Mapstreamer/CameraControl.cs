﻿//******************************************************************************
//
// Copyright (C) SAAB AB
//
// All rights, including the copyright, to the computer program(s) 
// herein belong to Saab AB. The program(s) may be used and/or
// copied only with the written permission of Saab AB, or in
// accordance with the terms and conditions stipulated in the
// agreement/contract under which the program(s) have been
// supplied. 
//
//
// Information Class:	COMPANY UNCLASSIFIED
// Defence Secrecy:		NOT CLASSIFIED
// Export Control:		NOT EXPORT CONTROLLED
//
//
// File			: CameraControl.cs
// Module		:
// Description	: Manages camera updates with large coordinates
// Author		: Anders Modén
// Product		: Gizmo3D 2.12.59
//
// NOTE:	Gizmo3D is a high performance 3D Scene Graph and effect visualisation 
//			C++ toolkit for Linux, Mac OS X, Windows, Android, iOS and HoloLens for  
//			usage in Game or VisSim development.
//
//
// Revision History...
//
// Who	Date	Description
//
// AMO	180607	Created file                                        (2.9.1)
//
//******************************************************************************

// ************************** NOTE *********************************************
//
//      Stand alone from BTA !!! No BTA code in this !!!
//
// *****************************************************************************

//#define TEST_ROTATION   // Just test some default rotation

using GizmoSDK.GizmoBase;
using Saab.Foundation.Map;
using Saab.Unity.Extensions;
using Saab.Utility.Unity.NodeUtils;
using System.Collections.Generic;
using UnityEngine;

using Quaternion = UnityEngine.Quaternion;



namespace Saab.Foundation.Unity.MapStreamer
{
    public class CameraControl : MonoBehaviour , ISceneManagerCamera
    {

        public float speed = 20f;
        // Use this for initialization

        public float rotspeed = 20f;

        public double X = 0;
        public double Y = 0;
        public double Z = 0;

        public float LodFactor => 1f;

        private double _lastRenderTime = 0;
        private double _currentRenderTime = 0;        
        
        public Camera Camera
        {
            get
            {
                return GetComponent<Camera>();
            }
        }

        public float GetDeltaTime()
        {
            if (_lastRenderTime == 0)
                return 0;
            else
                return (float)(_currentRenderTime - _lastRenderTime);
        }

        public Vec3D GlobalPosition
        {
            get { return new Vec3D(X, Y, Z); }

            set
            {
                X = value.x;
                Y = value.y;
                Z = value.z;
            }
        }

        public Vector3 Up
        {
            get { return MapControl.SystemMap.GetLocalOrientation(GlobalPosition).GetCol(2).ToVector3(); }
        }

        public Vector3 North
        {
            get { return MapControl.SystemMap.GetLocalOrientation(GlobalPosition).GetCol(1).ToVector3(); }
        }

        private void MoveForward(float moveSpeed)
        {
            X = X + moveSpeed * GetDeltaTime() * transform.forward.x;
            Y = Y + moveSpeed * GetDeltaTime() * transform.forward.y;

            // As we have a Right Handed ON system and unitys Z into the screen we apply a negative direction
            Z = Z - moveSpeed * GetDeltaTime() * transform.forward.z;
        }

        private void MoveRight(float moveSpeed)
        {
            X = X + moveSpeed * GetDeltaTime() * transform.right.x;
            Y = Y + moveSpeed * GetDeltaTime() * transform.right.y;

            // As we have a Right Handed ON system and unitys Z points into the screen we apply a negative direction
            Z = Z - moveSpeed * GetDeltaTime() * transform.right.z;
        }

        private void MoveUp(float moveSpeed)
        {
            X = X + moveSpeed * GetDeltaTime() * transform.up.x;
            Y = Y + moveSpeed * GetDeltaTime() * transform.up.y;

            // As we have a Right Handed ON system and unitys Z points into the screen we apply a negative direction
            Z = Z - moveSpeed * GetDeltaTime() * transform.up.z;
        }

        private Quaternion Tilt(float rotationSpeed)
        {
            System.Numerics.Quaternion.CreateFromYawPitchRoll(0, 0, 0);
            return Quaternion.Euler(rotationSpeed * GetDeltaTime(), 0, 0);
        }

        private Quaternion Pan(float rotationSpeed)
        {
            return Quaternion.Euler(0, rotationSpeed * GetDeltaTime(), 0);
        }

            // Update is called once per frame
        void Update()
        {
            try
            {
                Performance.Enter("CameraControl.Update");
                // Check mouse click

                if (Input.GetButtonDown("Fire1"))
                {
                    Map.MapPos mapPos;

                    var layerMask = GroundClampType.GROUND;

                    if (Map.MapControl.SystemMap.GetScreenGroundPosition((int)Input.mousePosition.x, (int)(Screen.height - Input.mousePosition.y), (uint)Screen.width, (uint)Screen.height, out mapPos, layerMask, Map.ClampFlags.DEFAULT))
                    {
                        List<GameObject> list;

                        if (NodeUtils.FindGameObjects(mapPos.node.GetNativeReference(), out list))
                        {
                            foreach (GameObject o in list)
                            {
                                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                                sphere.transform.parent = o.transform;
                                sphere.transform.transform.localPosition = new Vector3((float)mapPos.position.x, (float)mapPos.position.y, (float)mapPos.position.z);
                                sphere.transform.localScale = new Vector3(10, 10, 10);
                            }
                        }

                        // Just test some update
                        mapPos.position += new Vec3(1, 1, 1);

                        Map.MapControl.SystemMap.UpdatePosition(mapPos, GroundClampType.GROUND);

                        GlobalPosition = mapPos.GlobalPosition(new Vec3(0, 0, 10));
                    }


                }

                if (Input.GetButtonDown("Fire2"))
                {
                    GizmoSDK.Coordinate.LatPos latpos = new GizmoSDK.Coordinate.LatPos
                    {
                        Altitude = 245.52585220821,
                        Latitude = 1.00778345058085,
                        Longitude = 0.251106492463706

                    };

                    Map.MapPos mappos;

                    if (Map.MapControl.SystemMap.GetPosition(latpos, out mappos, Map.GroundClampType.GROUND, Map.ClampFlags.WAIT_FOR_DATA))
                    {
                        Debug.Log("Hit Ground ok");

                    
                    }

                    //Performance.DumpPerformanceInfo();
                }

                //transform.position;

                
                if(Input.GetKey("b"))
                {
                    GizmoSDK.Gizmo3D.DynamicLoaderManager.StopManager();
                }

                if (Input.GetKey("v"))
                {
                    GizmoSDK.Gizmo3D.DynamicLoaderManager.StartManager();
                }


                //transform.position = pos;

                
            }
            finally
            {
                Performance.Leave();
            }
        }

        public void PreTraverse(bool locked)
        {
            // Called before traverser runs
        }

        public void PostTraverse(bool locked)
        {
            // Called after all nodes have updated their transforms
        }

        public double UpdateCamera(double renderTime)
        {
            _lastRenderTime = _currentRenderTime;
            _currentRenderTime = renderTime;

            if (Input.GetKey("w"))
            {
                MoveForward(speed);
            }
            if (Input.GetKey("s"))
            {
                MoveForward(-speed);
            }

            if (Input.GetKey(KeyCode.Space))
            {
                MoveUp(speed / 2);
            }
            if (Input.GetKey(KeyCode.C))
            {
                MoveUp(-speed / 2);
            }

            if (Input.GetKey("d"))
            {
                MoveRight(speed);
            }
            if (Input.GetKey("a"))
            {
                MoveRight(-speed);
            }

            Quaternion rot = transform.rotation;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                rot = rot * Tilt(rotspeed);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                rot = rot * Tilt(-rotspeed);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                rot = Pan(-rotspeed) * rot;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                rot = Pan(rotspeed) * rot;
            }

            if (Input.GetKeyDown("p"))
            {
                rot = Quaternion.Euler(0, 180, 0) * rot;
            }

#if TEST_ROTATION
                rot = Pan(-rotspeed) * rot;
#endif

            transform.rotation = rot;


            return renderTime;
        }

        public void MapChanged()
        {
            // Called when global map has changed
        }
    }
}

