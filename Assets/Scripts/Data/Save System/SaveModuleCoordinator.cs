using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveModuleCoordinator
{
    private const int CurrentVersion = 3;
    private readonly List<ISaveModule> _saveModules = new();

    public void RegisterModule(ISaveModule module)
    {
        if (!_saveModules.Any(m => m.ModuleName == module.ModuleName))
        {
            _saveModules.Add(module);
        }
    }

    public SaveContainer SerializeModules()
    {
        var moduleDataList = new List<ModuleSaveData>();

        foreach (var module in _saveModules)
        {
            try
            {
                moduleDataList.Add(new ModuleSaveData
                {
                    moduleName = module.ModuleName,
                    version = module.Version,
                    data = module.Serialize()
                });
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to serialize {module.ModuleName}: {e.Message}");
            }
        }

        return new SaveContainer
        {
            version = CurrentVersion,
            modules = moduleDataList.ToArray()
        };
    }

    public void DeserializeModules(SaveContainer saveContainer)
    {
        foreach (var moduleData in saveContainer.modules)
        {
            var module = _saveModules.FirstOrDefault(m => m.ModuleName == moduleData.moduleName);

            if (module == null)
            {
                Debug.LogWarning($"Unknown save module: {moduleData.moduleName}");
                continue;
            }

            try
            {
                if (moduleData.version != module.Version)
                {
                    module.OnMigrate(moduleData.version, module.Version);
                }

                module.Deserialize(moduleData.data);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to deserialize {moduleData.moduleName}: {e.Message}");
            }
        }
    }
}
