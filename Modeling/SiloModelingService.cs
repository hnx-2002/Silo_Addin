using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace SiloModelingTaskClient
{
    public class SiloModelingService
    {
        private readonly SiloTaskRepository _repository;
        private readonly SiloTypeResolver _siloTypeResolver;
        private readonly PlacementTemplateLoader _templateLoader;
        private readonly RfaResourceResolver _rfaResourceResolver;
        private readonly PlacementTransformCalculator _transformCalculator;
        private readonly RevitFamilyPlacementService _placementService;

        public SiloModelingService(SiloTaskRepository repository, string templateRootDir)
        {
            _repository = repository;
            _siloTypeResolver = new SiloTypeResolver();
            _templateLoader = new PlacementTemplateLoader(templateRootDir);
            _rfaResourceResolver = new RfaResourceResolver(repository);
            _transformCalculator = new PlacementTransformCalculator();
            _placementService = new RevitFamilyPlacementService(repository);
        }

        public List<ModelingPlacementResult> Execute(Document doc, ModelingTask task, Action<string> log)
        {
            Guid dictSiloId = _siloTypeResolver.ResolveDictSiloId(task.SiloType);
            DictSiloRecord dictSilo = _repository.GetDictSilo(dictSiloId);
            string finalSiloType = _siloTypeResolver.ResolveTemplateKey(dictSilo.SiloType);
            log("Template silo type: " + finalSiloType);

            List<PlacementTemplateRecord> templateRecords = _templateLoader.Load(finalSiloType);
            log("Template family instance count: " + templateRecords.Count);

            Dictionary<string, RfaResourceRecord> resources = _rfaResourceResolver.Resolve(templateRecords);
            log("Rfa resource count: " + resources.Count);

            List<ModelingPlacementResult> placements = _transformCalculator.Calculate(templateRecords, task, resources);
            _placementService.Place(doc, placements, resources);
            log("Revit family placement completed, instance count: " + placements.Count);

            return placements;
        }
    }
}
