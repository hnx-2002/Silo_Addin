using System;

namespace SiloModelingTaskClient
{
    public class ModelingPlacementResult
    {
        public Guid RfaResourceId { get; set; }
        public string FamilyName { get; set; }
        public string SymbolName { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double RotationAngle { get; set; }
        public double LocationXMeters { get; set; }
        public double LocationYMeters { get; set; }
        public double LocationZMeters { get; set; }
        public double RotationAngleDegrees { get; set; }
    }
}
