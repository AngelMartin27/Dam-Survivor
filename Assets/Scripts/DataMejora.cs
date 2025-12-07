using UnityEngine;

[CreateAssetMenu(fileName = "DataMejora", menuName = "Mejoras/DataMejora")]
public class DataMejora : ScriptableObject
{
    [Header("Info Visual")]
    public string titulo;
    [TextArea] public string descripcion;
    public Sprite icono;

    [Header("Lógica")]
    public GameObject prefabArma;
}
