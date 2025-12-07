using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Target Configuration")]
    [SerializeField] private Transform target;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomMin = 0.5f;
    [SerializeField] private float zoomMax = 2f;
    [SerializeField] private float sensibilidadZoom = 10f; // Antes 'suavizadoZoom'

    // --- ESTADO INTERNO ---
    private Vector3 offsetBase;   // La distancia original
    private float zoomActual = 1f; 
    private Controles inputActions; // Clase generada por el Input System

    // --- INICIALIZACIÓN DEL INPUT SYSTEM ---

    private void Awake()
    {
        inputActions = new Controles();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    // --- CONFIGURACIÓN INICIAL ---

    void Start()
    {
        // 1. Auto-detectar al Player si no está asignado
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
            else
            {
                Debug.LogError("ERROR: FollowCamera no tiene un objetivo asignado y no encuentra el tag 'Player'.");
                return;
            }
        }

        // 2. Calcular el Offset inicial (distancia relativa)
        // Esto asume que colocas la cámara donde quieres en la escena antes de dar Play.
        offsetBase = transform.position - target.position;
    }

    // --- BUCLE DE JUEGO ---

    // Usamos LateUpdate para mover la cámara DESPUÉS de que el jugador se haya movido.
    private void LateUpdate()
    {
        if (target == null) return;

        ProcesarZoom();
        MoverCamara();
    }

    private void ProcesarZoom()
    {
        // Leemos el valor del scroll (ratón o gamepad)
        float scrollValue = inputActions.Camara.Zoom.ReadValue<float>();

        // Si hay entrada, ajustamos el nivel de zoom
        if (scrollValue != 0)
        {
            // Restamos porque generalmente Scroll Arriba (+) queremos que acerque (Zoom menor)
            zoomActual -= scrollValue / sensibilidadZoom;
            
            // Limitamos el valor entre el mínimo y máximo
            zoomActual = Mathf.Clamp(zoomActual, zoomMin, zoomMax);
        }
    }

    private void MoverCamara()
    {
        // Calculamos el offset final basado en el nivel de zoom
        // Zoom 1 = Distancia original
        // Zoom 0.5 = Mitad de distancia (Más cerca)
        // Zoom 2 = Doble distancia (Más lejos)
        Vector3 offsetFinal = offsetBase * zoomActual;

        // Aplicamos la posición
        transform.position = target.position + offsetFinal;
    }
}