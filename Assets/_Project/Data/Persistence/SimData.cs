using System;
using UnityEngine;

namespace FestivalGrounds.Data
{
    [Serializable]
    public class SimData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public float Hunger;
        public float Bladder;
        public float Energy;
    }
}