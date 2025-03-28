using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterSort
{
    public class SurfaceIdleDatas
    {
        public List<SurfaceIdleData> surfaceIdleDatas;
    }

    [Serializable]
    public class SurfaceIdleData
    {
        public List<SurfaceIdlePosition> listFour;
        public List<SurfaceIdlePosition> listFive;
    }

    [Serializable]
    public class SurfaceIdlePosition
    {       
        public float yLocalPosition;
    }
}
