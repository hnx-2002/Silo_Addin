using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SiloModelingTaskClient
{
    public class PlacementTransformCalculator
    {
        private const string BaseFamilyName = "结构库底板示意";

        public List<ModelingPlacementResult> Calculate(
            List<PlacementTemplateRecord> templateRecords,
            ModelingTask task,
            Dictionary<string, RfaResourceRecord> rfaResources)
        {
            PlacementTemplateRecord baseRecord = templateRecords.FirstOrDefault(x => x.FamilyName == BaseFamilyName);
            if (baseRecord == null)
            {
                throw new InvalidOperationException("模板中缺少基准族：" + BaseFamilyName);
            }

            if (!task.TaskX.HasValue || !task.TaskY.HasValue || !task.TaskZ.HasValue)
            {
                throw new InvalidOperationException("建模任务缺少项目基点坐标。");
            }

            if (!task.RotationAngle.HasValue)
            {
                throw new InvalidOperationException("Modeling task is missing rotation_angle.");
            }

            double angleDegrees = Decimal.ToDouble(task.RotationAngle.Value);
            double angleRadians = ModelingUnitConverter.DegreesToRadians(angleDegrees);
            double cos = Math.Cos(angleRadians);
            double sin = Math.Sin(angleRadians);

            var targetBase = new XYZ(
                ModelingUnitConverter.MetersToFeet(Decimal.ToDouble(task.TaskX.Value)),
                ModelingUnitConverter.MetersToFeet(Decimal.ToDouble(task.TaskY.Value)),
                ModelingUnitConverter.MetersToFeet(Decimal.ToDouble(task.TaskZ.Value)));

            var results = new List<ModelingPlacementResult>();
            foreach (PlacementTemplateRecord record in templateRecords)
            {
                if (!rfaResources.ContainsKey(record.FamilyName))
                {
                    throw new InvalidOperationException("未找到族资源：" + record.FamilyName);
                }

                double dx = record.X - baseRecord.X;
                double dy = record.Y - baseRecord.Y;
                double dz = record.Z - baseRecord.Z;

                double rotatedX = dx * cos - dy * sin;
                double rotatedY = dx * sin + dy * cos;

                RfaResourceRecord resource = rfaResources[record.FamilyName];
                double x = targetBase.X + rotatedX;
                double y = targetBase.Y + rotatedY;
                double z = targetBase.Z + dz;

                results.Add(new ModelingPlacementResult
                {
                    RfaResourceId = resource.Id,
                    FamilyName = record.FamilyName,
                    SymbolName = record.SymbolName,
                    X = x,
                    Y = y,
                    Z = z,
                    RotationAngle = angleRadians,
                    LocationXMeters = ModelingUnitConverter.FeetToMeters(x),
                    LocationYMeters = ModelingUnitConverter.FeetToMeters(y),
                    LocationZMeters = ModelingUnitConverter.FeetToMeters(z),
                    RotationAngleDegrees = angleDegrees
                });
            }

            return results;
        }
    }
}
