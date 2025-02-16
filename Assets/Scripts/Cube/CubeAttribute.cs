using UnityEngine;
using System;
using System.Collections.Generic;

public class CubeAttribute : MonoBehaviour
{
    public enum AttributeType
    {
        None,
        Fire,   // 높은 데미지, 지속 데미지
        Ice,    // 이동속도 감소, 방어력 증가
        Lightning,  // 높은 이동속도, 연쇄 공격
        Earth      // 높은 체력, 넉백 효과
    }

    [System.Serializable]
    public class AttributeEffect
    {
        public AttributeType type;
        public Color color = Color.white;
        public ParticleSystem particleEffect;
        public float damageMultiplier = 1f;
        public float speedMultiplier = 1f;
        public float defenseMultiplier = 1f;
        [TextArea]
        public string description;
    }

    [Header("Attribute Settings")]
    [SerializeField] private List<AttributeEffect> attributeEffects;
    [SerializeField] private MeshRenderer cubeRenderer;
    [SerializeField] private float effectTriggerInterval = 0.5f;
    
    [Header("Effect Settings")]
    [SerializeField] private float effectRadius = 3f;
    [SerializeField] private LayerMask targetLayer;

    private AttributeType currentAttribute = AttributeType.None;
    private AttributeEffect currentEffect;
    private float effectTimer;
    private bool isEffectActive;

    // Components
    private CubeController cubeController;
    private CubeEvolution cubeEvolution;

    // Events
    public event Action<AttributeType> OnAttributeChanged;
    public event Action<AttributeType, Vector3> OnEffectTriggered;

    public AttributeType CurrentAttribute => currentAttribute;

    private void Awake()
    {
        cubeController = GetComponent<CubeController>();
        cubeEvolution = GetComponent<CubeEvolution>();
    }

    private void Start()
    {
        InitializeAttribute();
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.Playing) return;

        UpdateAttributeEffect();
    }

    private void InitializeAttribute()
    {
        // Load saved attribute if any
        string savedAttribute = PlayerPrefs.GetString("CubeAttribute", AttributeType.None.ToString());
        if (Enum.TryParse(savedAttribute, out AttributeType loadedAttribute))
        {
            SetAttribute(loadedAttribute);
        }
    }

    private void SubscribeToEvents()
    {
        if (cubeController != null)
        {
            cubeController.OnCubeMove += HandleCubeMove;
            cubeController.OnCubeRoll += HandleCubeRoll;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (cubeController != null)
        {
            cubeController.OnCubeMove -= HandleCubeMove;
            cubeController.OnCubeRoll -= HandleCubeRoll;
        }
    }

    public void SetAttribute(AttributeType newAttribute)
    {
        if (currentAttribute == newAttribute) return;

        currentAttribute = newAttribute;
        currentEffect = attributeEffects.Find(e => e.type == currentAttribute);

        if (currentEffect != null)
        {
            // Update visual appearance
            UpdateVisuals();
            
            // Save attribute
            PlayerPrefs.SetString("CubeAttribute", currentAttribute.ToString());
            PlayerPrefs.Save();

            // Notify listeners
            OnAttributeChanged?.Invoke(currentAttribute);
        }
    }

    private void UpdateVisuals()
    {
        // Update cube color
        if (cubeRenderer != null && currentEffect != null)
        {
            Material material = cubeRenderer.material;
            material.color = currentEffect.color;
            
            // Add emission for elemental effect
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", currentEffect.color * 0.5f);
        }

        // Update particle effects
        UpdateParticleEffects();
    }

    private void UpdateParticleEffects()
    {
        // Disable all particle effects first
        foreach (var effect in attributeEffects)
        {
            if (effect.particleEffect != null)
            {
                effect.particleEffect.Stop();
            }
        }

        // Enable current attribute's particle effect
        if (currentEffect?.particleEffect != null)
        {
            currentEffect.particleEffect.Play();
        }
    }

    private void UpdateAttributeEffect()
    {
        if (currentAttribute == AttributeType.None) return;

        effectTimer += Time.deltaTime;
        
        if (effectTimer >= effectTriggerInterval)
        {
            effectTimer = 0f;
            TriggerAttributeEffect();
        }
    }

    private void TriggerAttributeEffect()
    {
        switch (currentAttribute)
        {
            case AttributeType.Fire:
                ApplyFireEffect();
                break;
            case AttributeType.Ice:
                ApplyIceEffect();
                break;
            case AttributeType.Lightning:
                ApplyLightningEffect();
                break;
            case AttributeType.Earth:
                ApplyEarthEffect();
                break;
        }
    }

    private void ApplyFireEffect()
    {
        // Create burning area around cube
        Collider[] targets = Physics.OverlapSphere(transform.position, effectRadius, targetLayer);
        foreach (var target in targets)
        {
            EnemyBehavior enemy = target.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                float damage = 10f * currentEffect.damageMultiplier;
                enemy.TakeDamage(damage);
            }
        }
        
        OnEffectTriggered?.Invoke(AttributeType.Fire, transform.position);
    }

    private void ApplyIceEffect()
    {
        // Slow nearby enemies
        Collider[] targets = Physics.OverlapSphere(transform.position, effectRadius, targetLayer);
        foreach (var target in targets)
        {
            // TODO: Implement slow effect on enemies
        }
        
        OnEffectTriggered?.Invoke(AttributeType.Ice, transform.position);
    }

    private void ApplyLightningEffect()
    {
        // Chain lightning between nearby enemies
        Collider[] targets = Physics.OverlapSphere(transform.position, effectRadius, targetLayer);
        if (targets.Length > 0)
        {
            // TODO: Implement chain lightning effect
        }
        
        OnEffectTriggered?.Invoke(AttributeType.Lightning, transform.position);
    }

    private void ApplyEarthEffect()
    {
        // Create shockwave and knock back enemies
        Collider[] targets = Physics.OverlapSphere(transform.position, effectRadius, targetLayer);
        foreach (var target in targets)
        {
            Rigidbody targetRb = target.GetComponent<Rigidbody>();
            if (targetRb != null)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                targetRb.AddForce(direction * 10f, ForceMode.Impulse);
            }
        }
        
        OnEffectTriggered?.Invoke(AttributeType.Earth, transform.position);
    }

    private void HandleCubeMove(Vector3 direction)
    {
        // Add movement-based effects here
    }

    private void HandleCubeRoll()
    {
        // Add roll-based effects here
    }

    public float GetDamageMultiplier()
    {
        return currentEffect?.damageMultiplier ?? 1f;
    }

    public float GetSpeedMultiplier()
    {
        return currentEffect?.speedMultiplier ?? 1f;
    }

    public float GetDefenseMultiplier()
    {
        return currentEffect?.defenseMultiplier ?? 1f;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw effect radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
}