using System;
using System.Collections.Generic;
using UnityEngine;
using BreakInfinity;

// Example of how easy it is to extend the save system
// This demonstrates the Open-Closed Principle in action
public class SkillTreeSaveModule : ISaveModule
{
    public string ModuleName => "SkillTree";
    public int Version => 1;

    // Hypothetical skill tree data
    private Dictionary<string, int> skillLevels = new Dictionary<string, int>();
    private BigDouble availableSkillPoints = 0;

    public string Serialize()
    {
        var data = new SkillTreeData
        {
            availablePoints = availableSkillPoints.ToString(),
            skills = new List<SkillData>()
        };

        foreach (var skill in skillLevels)
        {
            data.skills.Add(new SkillData { id = skill.Key, level = skill.Value });
        }

        return JsonUtility.ToJson(data);
    }

    public void Deserialize(string data)
    {
        try
        {
            var skillData = JsonUtility.FromJson<SkillTreeData>(data);

            availableSkillPoints = BigDouble.SafeParseBigDouble(skillData.availablePoints);
            skillLevels.Clear();

            foreach (var skill in skillData.skills)
            {
                skillLevels[skill.id] = skill.level;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to deserialize skill tree data: {e.Message}");
            // Initialize with default values
            skillLevels.Clear();
            availableSkillPoints = 0;
        }
    }

    public void OnMigrate(int fromVersion, int toVersion)
    {
        Debug.Log($"Migrating SkillTree from version {fromVersion} to {toVersion}");

        // Handle migration logic here
        if (fromVersion < 1 && toVersion >= 1)
        {
            // Example migration: reset skill points if major version change
            availableSkillPoints = 10; // Give some free skill points
        }
    }

    // Public API for accessing skill data
    public int GetSkillLevel(string skillId)
    {
        return skillLevels.TryGetValue(skillId, out int level) ? level : 0;
    }

    public void SetSkillLevel(string skillId, int level)
    {
        skillLevels[skillId] = level;
    }

    public BigDouble GetAvailableSkillPoints()
    {
        return availableSkillPoints;
    }

    public void SetAvailableSkillPoints(BigDouble points)
    {
        availableSkillPoints = points;
    }
}

[Serializable]
public class SkillTreeData
{
    public string availablePoints;
    public List<SkillData> skills;
}

[Serializable]
public class SkillData
{
    public string id;
    public int level;
}