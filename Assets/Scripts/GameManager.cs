using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Mis Paneles")]
    public GameObject panelInicio;    
    public GameObject panelGameOver;
    public GameObject panelVictoria;  
    public GameObject panelJuegoHUD;  
    public GameObject panelPausa;     

    // Variable para saber si estamos en pausa
    private bool juegoPausado = false;

    void Awake()
    {
        // Esto asegura que solo haya un GameManager
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
        // Al empezar, mostramos el menú principal
        PrepararInicio();
    }

    void Update()
    {
        // 1. Comprobamos si estamos jugando (ni en menú inicio, ni muertos ni en victoria)
        if (!panelInicio.activeSelf && !panelGameOver.activeSelf && !panelVictoria.activeSelf)
        {
            // 2. Detectar tecla ESC (Forma compatible con New Input System)
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (juegoPausado)
                {
                    ReanudarJuego();
                }
                else
                {
                    PausarJuego();
                }
            }
        }
    }

    // --- FUNCIONES PRINCIPALES ---

    void PrepararInicio()
    {
        Time.timeScale = 0f; // Juego parado
        
        panelInicio.SetActive(true);
        panelGameOver.SetActive(false);
        panelJuegoHUD.SetActive(false);
        panelPausa.SetActive(false);
        
        juegoPausado = false;
    }

    public void PausarJuego()
    {
        juegoPausado = true;
        Time.timeScale = 0f; // Congelar tiempo
        
        panelPausa.SetActive(true);       // Mostrar menú pausa
        panelJuegoHUD.SetActive(false);   // Ocultar vida/nivel
    }

    public void ReanudarJuego()
    {
        juegoPausado = false;
        Time.timeScale = 1f; // Tiempo normal
        
        panelPausa.SetActive(false);      // Ocultar menú pausa
        panelJuegoHUD.SetActive(true);    // Mostrar vida/nivel
    }

    public void ActivarGameOver()
    {
        Time.timeScale = 0f; // Congelar tiempo
        
        panelGameOver.SetActive(true);
        panelJuegoHUD.SetActive(false);
        panelPausa.SetActive(false); 
    }

    public void ActivarVictoria()
    {
        Debug.Log("¡Nivel Completado!");
        Time.timeScale = 0f; // Congelar tiempo
        
        panelVictoria.SetActive(true);
        panelJuegoHUD.SetActive(false);
        panelPausa.SetActive(false); 
        // Nota: No activamos panelGameOver, solo el de Victoria
    }

    // --- FUNCIONES PARA LOS BOTONES DE LA PANTALLA ---

    public void BotonJugar()
    {
        Time.timeScale = 1f; // Arrancar tiempo
        juegoPausado = false;

        panelInicio.SetActive(false);
        panelPausa.SetActive(false);
        panelJuegoHUD.SetActive(true);
    }

    public void BotonReiniciar()
    {
        Time.timeScale = 1f; // Importante poner el tiempo normal antes de recargar
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BotonSalir()
    {
        Debug.Log("Saliendo del juego...");

        #if UNITY_EDITOR
            // Si estamos en el editor de Unity, paramos la ejecución del juego.
            UnityEditor.EditorApplication.isPlaying = false;
        
        // Si estamos en una aplicación compilada, cerramos la aplicación.
        #else
            // Entonces cierra la aplicación por completo.
            Application.Quit();
        #endif
    }
}