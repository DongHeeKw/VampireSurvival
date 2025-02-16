using UnityEngine;
using System;
using System.Collections.Generic;

public class CubeEvolution : MonoBehaviour
{
    [System.Serializable]
    public class EvolutionStage
    {
        public string stageName;
        public int requiredLevel;
        public GameObject cubeModel;
        public ParticleSystem evolutionEffect;
        public float statMultiplier;
    }

    [System.Serializable]
    public class EvolutionPath
    {
        public string pathName;
        public CubeAttribute.AttributeType requiredAttribute;
        public List<EvolutionStage> stages;
    }

    [Header("Evolution Settings")]
    [SerializeField] private List<EvolutionPath> evolutionPaths;
    [SerializeField] private float evolutionDuration = 2f;
    
    [Header("Components")]
    [SerializeField] private CubeAttribute cubeAttribute;
    
    private int currentLevel = 1;
    private EvolutionPath currentPath;
    private EvolutionStage currentStage;
    private bool isEvolving;

    // Events
    public event Action<EvolutionStage> OnEvolutionComplete;
    public event Action<int> OnLevelUp;

    private void Start()
    {
        InitializeEvolution();
    }

    private void InitializeEvolution()
    {
        // Set initial evolution path based on cube's attribute
        if (cubeAttribute != null)
        {
            SetEvolutionPath(cubeAttribute.CurrentAttribute);
        }
    }

    public void SetEvolutionPath(CubeAttribute.AttributeType attribute)
    {
        currentPath = evolutionPaths.Find(p => p.requiredAttribute == attribute);
        if (currentPath != null)
        {
            // Set initial stage
            UpdateEvolutionStage();
        }
        else
        {
            Debug.LogWarning($"No evolution path found for attribute: {attribute}");
        }
    }

    public void AddExperience(int amount)
    {
        if (isEvolving) return;

        // Calculate required exp for next level (simple formula, can be adjusted)
        int requiredExp = currentLevel * 100;
        
        if (amount >= requiredExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        OnLevelUp?.Invoke(currentLevel);
        
        // Check if we can evolve
        if (CanEvolve())
        {
            StartEvolution();
        }
    }

    private bool CanEvolve()
    {
        if (currentPath == null) return false;

        // Find next evolution stage
        EvolutionStage nextStage = currentPath.stages.Find(
            s => s.requiredLevel <= currentLevel && s != currentStage
        );

        return nextStage != null;
    }

    private void StartEvolution()
    {
        if (isEvolving) return;
        
        isEvolving = true;
        
        // Store previous stage for reference
        EvolutionStage previousStage = currentStage;
        
        // Update to new stage
        UpdateEvolutionStage();
        
        if (currentStage != null && currentStage != previousStage)
        {
            // Start evolution sequence
            StartCoroutine(EvolutionSequence());
        }
        else
        {
            isEvolving = false;
        }
    }

    private System.Collections.IEnumerator EvolutionSequence()
    {
        // Disable player control during evolution
        CubeController controller = GetComponent<CubeController>();
        if (controller != null)
        {
            controller.StopMovement();
            controller.enabled = false;
        }

        // Play evolution effect
        if (currentStage.evolutionEffect != null)
        {
            currentStage.evolutionEffect.Play();
        }

        // Wait for evolution duration
        yield return new WaitForSeconds(evolutionDuration);

        // Update cube model
        UpdateCubeModel();

        // Re-enable player control
        if (controller != null)
        {
            controller.enabled = true;
        }

        // Apply stat changes
        ApplyEvolutionStats();

        isEvolving = false;
        OnEvolutionComplete?.Invoke(currentStage);
    }

    private void UpdateEvolutionStage()
    {
        if (currentPath == null) return;

        // Find highest available stage for current level
        currentStage = null;
        foreach (var stage in currentPath.stages)
        {
            if (stage.requiredLevel <= currentLevel &&
                (currentStage == null || stage.requiredLevel > currentStage.requiredLevel))
            {
                currentStage = stage;
            }
        }
    }

    private void UpdateCubeModel()
    {
        if (currentStage == null || currentStage.cubeModel == null) return;

        // Deactivate all other cube models
        foreach (var path in evolutionPaths)
        {
            foreach (var stage in path.stages)
            {
                if (stage.cubeModel != null)
                {
                    stage.cubeModel.SetActive(false);
                }
            }
        }

        // Activate current stage model
        currentStage.cubeModel.SetActive(true);
    }

    private void ApplyEvolutionStats()
    {
        if (currentStage == null) return;

        // Apply stat multiplier to relevant components
        // TODO: Implement stat modification system
    }

    public float GetCurrentStatMultiplier()
    {
        return currentStage?.statMultiplier ?? 1f;
    }

    public string GetCurrentStageName()
    {
        return currentStage?.stageName ?? "Basic Cube";
    }
}