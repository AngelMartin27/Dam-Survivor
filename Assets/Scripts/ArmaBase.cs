using UnityEngine;

public class ArmaBase : MonoBehaviour
{
    // Solo esto sale en el Inspector para ponerle nombre
    public string nombreArma;

    // 'protected' permite que AreaRecogida y ArmaDeZona lo usen directamenente.
    public int nivelActual = 1;

    // Propiedad pública por si la UI necesita leer el nivel (solo lectura)
    public int NivelActual => nivelActual;

    public virtual void SubirNivel()
    {
        nivelActual++;
        Debug.Log($"[ArmaBase] '{nombreArma}' subió a Nivel {nivelActual}");
    }
}