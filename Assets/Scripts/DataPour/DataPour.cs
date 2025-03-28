using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WaterSort
{
    public class DataPour
    {
        public List<HolderDataPour> holderDataPours;
    }
    [Serializable]
    public class HolderDataPour
    {
        public Vector3 lineRightPoin;
        public Vector3 lineLeftPoin;
        public float contentPoin;
        public List<DataPositionAndAngle> listFour;
        public List<DataPositionAndAngle> listFive;
    }
    [Serializable]
    public class DataPositionAndAngle
    {
        public Vector3 deltaStartPoint;        
        public float startAngle;
        public float endAngle;
    }
}
