using Autodesk.Revit.DB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SiloModelingTaskClient
{
    public class RfaInstanceCoordinateExporter
    {
        private readonly string _outputDir;

        public RfaInstanceCoordinateExporter(string outputDir)
        {
            _outputDir = outputDir;
        }

        public string Export(List<FamilyInstance> instances)
        {
            var records = new List<RfaInstanceCoordinateRecord>();
            foreach (FamilyInstance instance in instances)
            {
                if (!(instance.Location is LocationPoint locationPoint))
                {
                    throw new InvalidOperationException("族实例不是点定位实例，无法读取点坐标。ElementId：" + instance.Id.IntegerValue);
                }

                XYZ point = locationPoint.Point;
                records.Add(new RfaInstanceCoordinateRecord
                {
                    ElementId = instance.Id.IntegerValue,
                    FamilyName = instance.Symbol.Family.Name,
                    SymbolName = instance.Symbol.Name,
                    X = point.X,
                    Y = point.Y,
                    Z = point.Z
                });
            }

            Directory.CreateDirectory(_outputDir);

            string filePath = Path.Combine(_outputDir, "rfa_instance_coordinates.json");
            string json = JsonConvert.SerializeObject(records, Formatting.Indented);
            File.WriteAllText(filePath, json);
            return filePath;
        }
    }
}
