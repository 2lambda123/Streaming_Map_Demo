﻿//******************************************************************************
//
// Copyright (C) SAAB AB
//
// All rights, including the copyright, to the computer program(s)
// herein belong to SAAB AB. The program(s) may be used and/or
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
// File			: DistProperty.cs
// Module		: GizmoDistribution C#
// Description	: C# Bridge to gzDistAttribute class
// Author		: Anders Modén		
// Product		: GizmoDistribution 2.10.4
//		
//
//			
// NOTE:	GizmoBase is a platform abstraction utility layer for C++. It contains 
//			design patterns and C++ solutions for the advanced programmer.
//
//
// Revision History...							
//									
// Who	Date	Description						
//									
// AMO	180301	Created file 	
//
//******************************************************************************

using System;
using System.Runtime.InteropServices;
using GizmoSDK.GizmoBase;

 

namespace GizmoSDK
{
    namespace GizmoDistribution
    {
        [System.AttributeUsage(System.AttributeTargets.Property| System.AttributeTargets.Field, AllowMultiple = false)]
        public class DistProperty : System.Attribute
        {
        }

        [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
        public class DistPropertyAutoStore : System.Attribute
        {
        }

        [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
        public class DistPropertyAutoRestore : System.Attribute
        {
        }
    }
}
