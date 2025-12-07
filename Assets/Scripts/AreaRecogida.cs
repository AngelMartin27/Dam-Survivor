using UnityEngine;

public class AreaRecogida : ArmaBase
{
    [Header("Configuración")]
    [SerializeField] private float radioBase = 3.0f;
    [SerializeField] private float multiplicadorRadioPorNivel = 1.2f;

    // Referencias
    private SphereCollider colisionadorIman;
    private PlayerStats misStats;

    private void Start()
    {
        // Inicialización de nombre si no se ha establecido
        if (string.IsNullOrEmpty(nombreArma)) nombreArma = "Imán de XP";

        ConfigurarFisicas();
        ObtenerReferenciasExternas();
        AplicarNuevosStats();
    }

    private void ConfigurarFisicas()
    {
        colisionadorIman = GetComponent<SphereCollider>();
        Rigidbody rb = GetComponent<Rigidbody>();
    }

    private void ObtenerReferenciasExternas()
    {
        misStats = GetComponentInParent<PlayerStats>();
        if (misStats == null) Debug.LogError("¡OJO! No encuentro PlayerStats en el padre.");
    }

    public override void SubirNivel()
    {
        base.SubirNivel();
        AplicarNuevosStats();
    }

    private void AplicarNuevosStats()
    {
        // --- CÁLCULO Y APLICACIÓN DE RADIO ---
        float nuevoRadio = radioBase * Mathf.Pow(multiplicadorRadioPorNivel, nivelActual - 1);
        
        colisionadorIman.radius = nuevoRadio;
        
        Debug.Log($"Imán Nivel {nivelActual} - Radio: {nuevoRadio:F2}");
    }

    private void OnTriggerEnter(Collider other)
    {

        OrbeExperiencia orbe = other.GetComponent<OrbeExperiencia>();

        if (orbe != null)
        {
            orbe.RecogerInstantaneo(misStats);
        }
    }
}