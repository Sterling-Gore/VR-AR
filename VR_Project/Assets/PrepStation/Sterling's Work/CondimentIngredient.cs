using UnityEngine;

public class CondimentIngredient : MonoBehaviour
{
    [SerializeField] private string condimentName = "";
    [SerializeField] private Renderer condimentRenderer;
    
//---------------------------------------------------------------//
/*                      Unity Functions                        */
    private void Start()
    {
        if( condimentRenderer == null)
        {
            condimentRenderer.GetComponent<Renderer>();
        }
        _EraseCondiment();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Condiment condiment = other.GetComponent<Condiment>();
        // if( condiment != null)
        // {
        //     string recievedCondimentName = condiment.GetName();
        //     Color32 recievedCondimentColor = condiment.GetColor();
        //     _SetCondiment(recievedCondimentName, recievedCondimentColor);
        // } 
    }

//---------------------------------------------------------------//
/*                      Public Functions                        */
    

//---------------------------------------------------------------//
/*                      Private Functions                        */
    private void _SetCondiment(string recievedCondimentName, Color32 recievedCondimentColor)
    {
        condimentName = recievedCondimentName;
        condimentRenderer.material.SetColor("_BaseColor", recievedCondimentColor);
    }

    private void _EraseCondiment()
    {
        condimentName = "";
        condimentRenderer.material.SetColor("_BaseColor", new Color(0f, 0f, 0f, 0f));
    }

    [ContextMenu("Add Ketchup")]
    private void _TestAddKetchup()
    {
        _SetCondiment("ketchup", new Color32(207, 28, 28, 255));
    }

}
