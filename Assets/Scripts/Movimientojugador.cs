using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    ///////////////////////////////////// VARIABLES /////////////////////////////////
    [Header("Configuración")]
    private bool puedeMoverse = true;
    public float velocidadMovimiento = 5f;

    private Vector2 direccionPlana;
    
    // Necesitamos esta variable para acceder a las físicas
    private Rigidbody rb; 

    public Controles control;

    [Header("Rotación")]
    public float velocidadRotacion = 10f; 

    ///////////////////////////////////// FUNCIONES UNITY /////////////////////////////////
    private void Awake()
    {
        control = new Controles();
        rb = GetComponent<Rigidbody>(); 
    }

    private void OnEnable()
    {
        control.Enable();
    }
    
    private void OnDisable()
    {
        control.Disable();
    }

    void Update()
    {
        if (puedeMoverse)
        {
            direccionPlana = control.Player.move.ReadValue<Vector2>();
        }
    }

    void FixedUpdate()
    {
        Vector3 direccionMovimiento = new Vector3(direccionPlana.x, 0, direccionPlana.y);
        
        rb.linearVelocity = new Vector3(direccionMovimiento.x * velocidadMovimiento, rb.linearVelocity.y, direccionMovimiento.z * velocidadMovimiento);

        // --- ROTACIÓN ---
        if (direccionMovimiento.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(direccionMovimiento);
        }
    }
}