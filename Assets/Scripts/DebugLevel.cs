using UnityEngine;
using UnityEngine.InputSystem;

public class DebugLevel : MonoBehaviour
{
    [SerializeField]
    private PlayerStats playerStats; 

    [Header("Input Actions")]
    public InputActionReference AnadirArmaActionRef; 
    public InputActionReference SubirNivelActionRef; 

    private void OnEnable()
    {
        // Suscripción con Referencias a Métodos para una desuscripción segura.
        if (AnadirArmaActionRef != null)
        {
            AnadirArmaActionRef.action.performed += OnAnadirArmaPerformed;
        }
        if (SubirNivelActionRef != null)
        {
            SubirNivelActionRef.action.performed += OnSubirNivelPerformed;
        
        // Habilitar las acciones
        AnadirArmaActionRef?.action.Enable();
        SubirNivelActionRef?.action.Enable();
    }   
    }

    private void OnDisable()
    {
        // Cancelar la suscripción de forma segura.
        if (AnadirArmaActionRef != null)
        {
            AnadirArmaActionRef.action.performed -= OnAnadirArmaPerformed;
        }
        if (SubirNivelActionRef != null)
        {
            SubirNivelActionRef.action.performed -= OnSubirNivelPerformed;
        }
        
        AnadirArmaActionRef?.action.Disable();
        SubirNivelActionRef?.action.Disable();
    }
    
    // --- Métodos de Interfaz (Llaman a PlayerStats) ---
    
    // Nuevo método para la suscripción de Añadir Arma
    private void OnAnadirArmaPerformed(InputAction.CallbackContext context)
    {
        CallPlayerStats_GiveWeapon();
    }

    // Nuevo método para la suscripción de Subir Nivel
    private void OnSubirNivelPerformed(InputAction.CallbackContext context)
    {
        CallPlayerStats_LevelUpWeapon();
    }

    private void CallPlayerStats_GiveWeapon()
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats no está asignado en DebugLevel.cs.");
            return;
        }
        playerStats.OtorgarNuevaArmaDEBUG(); 
    }

    private void CallPlayerStats_LevelUpWeapon()
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats no está asignado en DebugLevel.cs.");
            return;
        }
        playerStats.SubirNivelArmaExistenteDEBUG();
    }
}