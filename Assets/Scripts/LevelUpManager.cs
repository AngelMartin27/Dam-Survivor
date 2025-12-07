using UnityEngine;
using System.Collections.Generic;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager instancia;

    [Header("UI - Referencias Visuales")]
    public GameObject panelLevelUp;       // El fondo negro que tapa la pantalla
    public Transform contenedorCartas;    // El objeto que ordenará las cartas (Layout Group)
    public GameObject prefabTarjeta;      // El prefab de la carta que diseñaste
    
    [Header("Base de Datos")]
    // Arrastra aquí TODOS tus archivos de mejora (Rayo, Hielo, Escudo...)
    public List<DataMejora> todasLasMejoras; 

    private PlayerStats player;

    void Awake()
    {
        // Configuración del Singleton
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Buscamos al jugador
        player = FindFirstObjectByType<PlayerStats>();
        
        // Aseguramos que el menú empiece oculto
        if(panelLevelUp != null) panelLevelUp.SetActive(false);
    }

    // Esta función la llama el PlayerStats cuando sube de nivel
    public void MostrarOpciones()
    {
        // 1. Pausar el tiempo del juego
        Time.timeScale = 0f; 
        panelLevelUp.SetActive(true);

        // 2. Limpiar cartas de la vez anterior (borrar hijos del contenedor)
        foreach (Transform child in contenedorCartas)
        {
            Destroy(child.gameObject);
        }

        // 3. Lógica de selección aleatoria (para no repetir cartas)
        List<DataMejora> seleccion = new List<DataMejora>();
        List<DataMejora> copiaLista = new List<DataMejora>(todasLasMejoras);

        // Mostramos 3 cartas, o menos si no tienes tantas creadas
        int cantidadAmostrar = Mathf.Min(3, copiaLista.Count); 

        for (int i = 0; i < cantidadAmostrar; i++)
        {
            int index = Random.Range(0, copiaLista.Count);
            seleccion.Add(copiaLista[index]);
            copiaLista.RemoveAt(index); // La quitamos de la copia para no elegirla dos veces
        }

        // 4. Crear las cartas visualmente en la pantalla
        foreach (DataMejora datos in seleccion)
        {
            GameObject nuevaCarta = Instantiate(prefabTarjeta, contenedorCartas);
            
            // Asegura que la carta tenga tamaño normal y posición correcta
            nuevaCarta.transform.localScale = Vector3.one; 
            nuevaCarta.transform.localPosition = new Vector3(nuevaCarta.transform.localPosition.x, nuevaCarta.transform.localPosition.y, 0);
            // -------------------------

            // Configuramos la carta pasándole los datos y ESTE manager
            if (nuevaCarta.TryGetComponent<TarjetaUI>(out var scriptCarta))
            {
                scriptCarta.Configurar(datos, this);
            }
        }
    }

    // Esta función se ejecuta cuando el jugador hace CLICK en una carta
    public void AlElegirCarta(DataMejora mejoraElegida)
    {
        if (player != null)
        {
            // Le damos el prefab del arma al PlayerStats
            player.AplicarMejora(mejoraElegida.prefabArma);
        }
        else
        {
            Debug.LogError("No se encontró al Player para darle la mejora.");
        }

        // Reanudar juego
        CerrarMenu();
    }

    void CerrarMenu()
    {
        if(panelLevelUp != null) panelLevelUp.SetActive(false);
        Time.timeScale = 1f; // El tiempo vuelve a correr
    }
}