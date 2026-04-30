using UnityEngine;

public class CondimentData : MonoBehaviour
{
    [SerializeField] private BurgerIngredients condimentName;
    [SerializeField] private Color32 condimentColor;
    [SerializeField] private ParticleSystem condimentSpray;

//---------------------------------------------------------------//
/*                      Unity Functions                          */
   void Awake()
    {
        _SetColorOverTime();
    }

//---------------------------------------------------------------//
/*                      Public Functions                        */
    public BurgerIngredients GetName()
    {
        return condimentName;
    }

    public Color32 GetColor()
    {
        return condimentColor;
    }

//---------------------------------------------------------------//
/*                      Private Functions                        */
    private void _SetColorOverTime()
    {
        var colorOverTime = condimentSpray.colorOverLifetime;
        colorOverTime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(condimentColor, 0.0f),
                new GradientColorKey(condimentColor, 1.0f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );
        colorOverTime.color = new ParticleSystem.MinMaxGradient(gradient);
    }

}