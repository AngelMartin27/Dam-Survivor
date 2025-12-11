using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; 
using TMPro; 
using Random = UnityEngine.Random; 

public class PlayerStats : MonoBehaviour
{
    // --- TUS VARIABLES ORIGINALES ---
    
    [Header("UI - Barras y Textos")]
    public Slider barraDeVida; 
    public Slider barraDeXP; 
    public TMP_Text textoNivel; 

    [Header("Estado del Jugador")]
    public int currentHealth; 
    public int maxHealth = 100; 
    public bool estaVivo = true; 

    [Header("Sistema de Niveles")]
    public int nivel = 1; 
    public int experiencia = 0; 
    public int experienciaParaSubir = 100; 
    public int incrementoPorNivel = 50; 

    [Header("Inventario")]
    public List<ArmaBase> armasEquipadas = new List<ArmaBase>(); 
    
    // --- VARIABLES DE DEBUG (Mantenemos solo la lista de armas) ---

    [Header("DEBUG - Armas Disponibles")]
    // Lista de Prefabs de armas para el debug (Tecla 1)
    [SerializeField] private List<GameObject> armasDisponiblesDEBUG; 

    // Referencias internas de DEBUG
    private int armaIndexDEBUG = 0;

    // --- MÉTODOS DE INICIALIZACIÓN ---

    private void Awake() 
    {
        currentHealth = maxHealth;
        estaVivo = true;
    }

    private void Start()
    {
        // 1. Configurar Barras
        if (barraDeVida != null)
        {
            barraDeVida.maxValue = maxHealth;
            barraDeVida.value = currentHealth;
        }
        ActualizarUI_Experiencia();

        // 2. AUTO-DETECTAR ARMAS INICIALES
        ArmaBase[] armasEncontradas = GetComponentsInChildren<ArmaBase>();
        
        armasEquipadas.Clear(); 
        armasEquipadas.AddRange(armasEncontradas);

        // 3. Seguridad
        if (experienciaParaSubir <= 0) experienciaParaSubir = 100;
        if (incrementoPorNivel <= 0) incrementoPorNivel = 50;
    }

    // --- LÓGICA DE DEBUG (LLAMADA POR DebugLevel.cs) ---

    public void OtorgarNuevaArmaDEBUG()
    {
        if (armasDisponiblesDEBUG != null && armasDisponiblesDEBUG.Count > 0)
        {
            // Seleccionar el siguiente prefab de la lista
            GameObject weaponPrefab = armasDisponiblesDEBUG[armaIndexDEBUG];
            
            // Usar tu función existente para añadir/mejorar el arma
            AplicarMejora(weaponPrefab); 

            // Avanzar al siguiente índice (circular)
            armaIndexDEBUG = (armaIndexDEBUG + 1) % armasDisponiblesDEBUG.Count;
            Debug.Log($"DEBUG: Intentando otorgar/mejorar arma {weaponPrefab.name}.");
        }
        else
        {
            Debug.LogWarning("DEBUG: La lista de armas disponibles está vacía.");
        }
    }

    public void SubirNivelArmaExistenteDEBUG()
    {
        if (armasEquipadas.Count > 0)
        {
            // 1. Seleccionar un arma aleatoria de las que ya tienes
            int index = Random.Range(0, armasEquipadas.Count);
            ArmaBase armaAsubir = armasEquipadas[index];
            
            // 2. Llamar al método SubirNivel() del arma
            if (armaAsubir != null) 
            {
                armaAsubir.SubirNivel();
                Debug.Log($"DEBUG: Arma '{armaAsubir.nombreArma}' subida a Nivel {armaAsubir.nivelActual}!");
            }
        }
        else
        {
            Debug.LogWarning("DEBUG: No hay armas equipadas para subir de nivel.");
        }
    }
    
    // --- EL RESTO DE TUS FUNCIONES ORIGINALES ---

    public void GanarExperiencia(int cantidad)
    {
        if (!estaVivo) return;

        experiencia += cantidad;
        if (barraDeXP != null) barraDeXP.value = experiencia;

        while (experiencia >= experienciaParaSubir)
        {
            SubirNivel();
        }
    }

    private void SubirNivel()
    {
        experiencia -= experienciaParaSubir;
        nivel++;
        experienciaParaSubir += incrementoPorNivel;

        // Curar y subir vida máxima
        maxHealth += 10;
        currentHealth = maxHealth; 

        if (barraDeVida != null) 
        {
            barraDeVida.maxValue = maxHealth;
            barraDeVida.value = currentHealth;
        }

        ActualizarUI_Experiencia();

        Debug.Log($"¡NIVEL {nivel} ALCANZADO!");
        
        if (LevelUpManager.instancia != null) 
        {
            LevelUpManager.instancia.MostrarOpciones(); 
        }
        else
        {
            Debug.LogWarning("No encuentro el LevelUpManager en la escena.");
        }
    }

    public void AplicarMejora(GameObject prefabRecompensa)
    {
        ArmaBase scriptDelPrefab = prefabRecompensa.GetComponent<ArmaBase>();

        if (scriptDelPrefab == null) 
        {
            Debug.LogError("El prefab no tiene script de arma.");
            return;
        }

        ArmaBase armaYaInstalada = null;
        foreach (ArmaBase arma in armasEquipadas)
        {
            if (arma.nombreArma == scriptDelPrefab.nombreArma)
            {
                armaYaInstalada = arma;
                break; 
            }
        }

        if (armaYaInstalada != null)
        {
            Debug.Log($"Mejorando arma existente: {armaYaInstalada.nombreArma}");
            armaYaInstalada.SubirNivel();
        }
        else
        {
            Debug.Log($"Nueva arma equipada: {scriptDelPrefab.nombreArma}");
            
            GameObject nuevaArmaObj = Instantiate(prefabRecompensa, transform.position, Quaternion.identity);
            
            nuevaArmaObj.transform.SetParent(transform);
            nuevaArmaObj.transform.localPosition = Vector3.zero;

            ArmaBase scriptNueva = nuevaArmaObj.GetComponent<ArmaBase>();
            if(scriptNueva != null) 
            {
                armasEquipadas.Add(scriptNueva); 
            }
        }
    }

    private void ActualizarUI_Experiencia()
    {
        if (barraDeXP != null)
        {
            barraDeXP.maxValue = experienciaParaSubir;
            barraDeXP.value = experiencia;
        }
        if (textoNivel != null)
        {
            textoNivel.text = "Nivel " + nivel.ToString();
        }
    }

    public void RecibirDmg(int dmg)
    {
        if (!estaVivo) return;

        currentHealth -= dmg;
        if (barraDeVida != null) barraDeVida.value = currentHealth;
        
        if (currentHealth <= 0) 
        {
            currentHealth = 0;
            estaVivo = false;
            if (GameManager.instancia != null) GameManager.instancia.ActivarGameOver();
        }
    }
}