using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; 
using TMPro; 

public class PlayerStats : MonoBehaviour
{
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
    // Esta lista es vital para saber qué armas tienes ya
    public List<ArmaBase> armasEquipadas = new List<ArmaBase>(); 

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
        // Busca las armas que se le pusieron al Player como hijos en la escena
        ArmaBase[] armasEncontradas = GetComponentsInChildren<ArmaBase>();
        
        armasEquipadas.Clear(); 
        armasEquipadas.AddRange(armasEncontradas);

        // 3. Seguridad
        if (experienciaParaSubir <= 0) experienciaParaSubir = 100;
        if (incrementoPorNivel <= 0) incrementoPorNivel = 50;
    }

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


        // Ahora avisamos al Manager de Cartas para que elijas tú.
        
        if (LevelUpManager.instancia != null) 
        {
            LevelUpManager.instancia.MostrarOpciones(); 
        }
        else
        {
            Debug.LogWarning("No encuentro el LevelUpManager en la escena.");
        }
    }

    //  ESTA ES LA FUNCIÓN QUE USAN LAS CARTAS AL ELEGIR
    public void AplicarMejora(GameObject prefabRecompensa)
    {
        // Extraemos el script del prefab para ver cómo se llama el arma
        ArmaBase scriptDelPrefab = prefabRecompensa.GetComponent<ArmaBase>();

        if (scriptDelPrefab == null) 
        {
            Debug.LogError("El prefab no tiene script de arma.");
            return;
        }

        // Buscamos si ya tienes esa arma equipada
        // Usamos una búsqueda por NOMBRE para permitir armas distintas con mismo script
        ArmaBase armaYaInstalada = null;

        foreach (ArmaBase arma in armasEquipadas)
        {
            // Comparamos el STRING del nombre
            if (arma.nombreArma == scriptDelPrefab.nombreArma)
            {
                armaYaInstalada = arma;
                break; 
            }
        }

        // Ahora actuamos según si el arma ya la tiene o no
        if (armaYaInstalada != null)
        {
            // ya la tiene subida de nivel
            Debug.Log($"Mejorando arma existente: {armaYaInstalada.nombreArma}");
            armaYaInstalada.SubirNivel();
        }
        else
        {
            // No la tiene, se la damos nueva
            Debug.Log($"Nueva arma equipada: {scriptDelPrefab.nombreArma}");
            
            // Instanciar (Crear la caja)
            GameObject nuevaArmaObj = Instantiate(prefabRecompensa, transform.position, Quaternion.identity);
            
            // Hacerla hija del Player y resetear posición
            nuevaArmaObj.transform.SetParent(transform);
            nuevaArmaObj.transform.localPosition = Vector3.zero;

            // Añadir a la lista para la próxima vez
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