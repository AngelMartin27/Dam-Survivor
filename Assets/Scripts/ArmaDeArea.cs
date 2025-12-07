using UnityEngine;

public class ArmaDeZona : ArmaBase
{
    
    [Header("Visual Configuration")]
    [SerializeField] private GameObject zonaPrefab;

    [Header("Base Stats")]
    [SerializeField] private int danoBase = 10;
    [SerializeField] private int incrementoDanoPorNivel = 5;
    
    [Header("Area Tamaño")]
    [SerializeField] private float escalaBase = 6f; 
    [SerializeField] private float incrementoEscalaPorNivel = 0.5f;

    // --- REFERENCIAS INTERNAS ---
    private GameObject zonaInstanciada;
    private FrostZone scriptFrostZone;
    private float alturaOriginalZona;

    // MÉTODOS DE INICIALIZACIÓN

    private void Start()
    {

        if (string.IsNullOrEmpty(nombreArma)) 
        {
            nombreArma = "Zona de Hielo";
        }

        InvocarYConfigurarZona();
    }

    // LÓGICA DE NIVEL Y CÁLCULOS
    
    //  Se sobreescribe el método de la base para gestionar el nivel
    public override void SubirNivel()
    {
        //  Se llama al método base para incrementar la variable 'nivelActual'.
        base.SubirNivel(); 

        // Si la zona existe, se aplican los nuevos stats.
        if (zonaInstanciada != null)
        {
            AplicarNuevosStats();
        }
        // Si no existe, no se hace nada
    }

    /// Calcula el nuevo daño basado en el nivel actual.
    private int CalcularDano()
    {
        // El nivel 1 es el base, por lo que el incremento es (nivelActual - 1).
        return danoBase + ((nivelActual - 1) * incrementoDanoPorNivel);
    }
    

    /// Calcula la nueva escala del área basada en el nivel actual.
    private float CalcularEscala()
    {
        // El nivel 1 es el base, por lo que el incremento es (nivelActual - 1).
        return escalaBase + ((nivelActual - 1) * incrementoEscalaPorNivel);
    }

    /// Instancia y configura la zona de efecto.

    private void InvocarYConfigurarZona()
    {
        if (zonaPrefab == null)
        {
            Debug.LogError($"ERROR: No se ha asignado el prefab '{nameof(zonaPrefab)}' en {nameof(ArmaDeZona)}.");
            return;
        }

        // Instancia la zona en la posición del jugador (transform.position)
        zonaInstanciada = Instantiate(zonaPrefab, transform.position, Quaternion.identity);

        // Hace que la zona sea HIJA del jugador. Se mueve automáticamente.
        zonaInstanciada.transform.SetParent(transform);
        
        // Resetea su posición local para asegurar que esté centrada.
        // Usa Vector3.zero para evitar problemas de posición.
        zonaInstanciada.transform.localPosition = Vector3.zero; 

        // Obtiene referencias
        scriptFrostZone = zonaInstanciada.GetComponent<FrostZone>();
        
        // Guarda la altura Y original del prefab para no modificarla en las actualizaciones
        alturaOriginalZona = zonaInstanciada.transform.localScale.y;

        // Aplica los stats iniciales (Nivel 1)
        AplicarNuevosStats();
    }

    private void AplicarNuevosStats()
    {
        // --- CÁLCULO Y APLICACIÓN DE DAÑO ---
        int nuevoDano = CalcularDano();

        if (scriptFrostZone != null)
        {
            scriptFrostZone.damagePorTick = nuevoDano;
        }

        // --- CÁLCULO Y APLICACIÓN DE TAMAÑO (Escala) ---
        float nuevaEscala = CalcularEscala();
        
        // Aplica la nueva escala (X y Z) manteniendo la Y original.
        zonaInstanciada.transform.localScale = new Vector3(nuevaEscala, alturaOriginalZona, nuevaEscala);

        // --- LOG DE ACTUALIZACIÓN ---
        Debug.Log($"{nombreArma} actualizada a Nivel {nivelActual}. Daño: {nuevoDano} | Tamaño: {nuevaEscala:F2}");
        // Usamos :F2 para limitar los floats a dos decimales en el log.
    }
}