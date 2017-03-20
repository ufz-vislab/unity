#if UNITY_STANDALONE_WIN
﻿using System;
using UnityEngine;
using System.Collections;
using Kitware.VTK;

namespace UFZ.VTK
{
	[Serializable]
	public class VtkActor : vtkActor
	{
		public new static VtkActor New()
		{
			var ret = vtkObjectFactory.CreateInstance("VtkActor");
			if (ret != null)
				return (VtkActor)ret;
			return new VtkActor();
		}

	}
}
#endif
