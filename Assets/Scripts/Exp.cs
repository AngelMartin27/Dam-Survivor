using UnityEngine;

public class OrbeExperiencia : MonoBehaviour
{
    [Header("XP Configuration")]
    [Tooltip("Cantidad base de experiencia que otorga este orbe.")]
    [SerializeField] private int cantidadExperiencia = 10;

    // --- ESTADO INTERNO ---
    private bool recogido = false; // Flag de seguridad (evita que se recoja 2 veces en el mismo frame)

    // --- MÉTODOS DE CONFIGURACIÓN ---

    public void ConfigurarValor(int valor)
    {
        cantidadExperiencia = valor;
    }

    // --- LÓGICA DE RECOLECCIÓN ---

    public void RecogerInstantaneo(PlayerStats statsDelJugador)
    {
        // 1. Guard Check: Si ya está marcado como recogido, salimos.
        if (recogido) return;

        // 2. Marcar flag para evitar duplicados
        recogido = true;

        // 3. Aplicar lógica
        if (statsDelJugador != null)
        {
            statsDelJugador.GanarExperiencia(cantidadExperiencia);
        }
        else
        {
            Debug.LogWarning($"[OrbeExperiencia] Se intentó recoger un orbe pero la referencia 'statsDelJugador' es nula.");
        }

        // 4. Limpieza
        Destroy(gameObject);
    }
}