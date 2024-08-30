using UnityEngine;
using DG.Tweening;


public class SkyboxPropertyAnimator : MonoBehaviour
{
    public string propertyName;
    public float startValue;
    public float endValue;
    public float duration;

    void OnEnable()
    {
        // Check if the property name is valid
        if (string.IsNullOrEmpty(propertyName))
        {
            Debug.LogError("Property name is not specified!");
            return;
        }

        // Initialize DOTween
        DOTween.Init();

        // Get the current skybox material
        Material skyboxMaterial = RenderSettings.skybox;

        // Check if the material has the specified property
        if (!skyboxMaterial.HasProperty(propertyName))
        {
            Debug.LogError("Skybox material does not have property: " + propertyName);
            return;
        }
        else
        {
            // Animate skybox material property
            DOTween.To(() => startValue, x => skyboxMaterial.SetFloat(propertyName, x), endValue, duration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .OnComplete(() => {
                // Animation complete
                Debug.Log("Animation complete!");
                });
        }


    }
}
