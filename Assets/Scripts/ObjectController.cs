using UnityEngine;
using CustomMath; // Asegúrate de incluir el espacio de nombres de tu MyTransform

public class MyTransformController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float turnSpeed = 90.0f;

    [Header("Configuración de Espacio")]
    [SerializeField] private CustomMath.Space movementSpace = CustomMath.Space.Self;

    // Instancia de tu MyTransform personalizado
    private MyTransform myTransform;

    private void Awake()
    {
        // Inicializamos MyTransform pasando el Transform nativo de Unity
        myTransform = new MyTransform(transform);
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleScale();

        // Sincronizamos la posición y rotación resultantes de vuelta al Transform de Unity
        // para que se renderice en pantalla correctamente.
        SyncToUnityTransform();
    }

    private void HandleMovement()
    {
        // Obtener entradas de ejes (WASD / Flechas)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            // Creamos el vector de dirección
            Vec3 moveDirection = new Vec3(horizontal, 0f, vertical) * moveSpeed * Time.deltaTime;

            // Usamos el método Translate de tu clase MyTransform
            myTransform.Translate(moveDirection, movementSpace);
        }
    }

    private void HandleRotation()
    {
        // Rotar sobre el eje Y al presionar las teclas Q y E
        float yaw = 0f;
        if (Input.GetKey(KeyCode.E)) yaw += 1f;
        if (Input.GetKey(KeyCode.Q)) yaw -= 1f;

        if (yaw != 0f)
        {
            // Usamos el método Rotate de tu MyTransform
            myTransform.Rotate(0f, yaw * turnSpeed * Time.deltaTime, 0f, CustomMath.Space.Self);
        }
    }

    private void HandleScale()
    {
        // Cambiar escala progresivamente con R (agrandar) y F (achicar)
        if (Input.GetKey(KeyCode.R))
        {
            myTransform.localScale += Vec3.One * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.F))
        {
            Vec3 newScale = myTransform.localScale - Vec3.One * Time.deltaTime;
            // Evitamos escalas negativas o de cero
            if (newScale.x > 0.1f && newScale.y > 0.1f && newScale.z > 0.1f)
            {
                myTransform.localScale = newScale;
            }
        }
    }

    private void SyncToUnityTransform()
    {
        // Asignamos los valores calculados en MyTransform al Transform real de la escena
        transform.localPosition = new Vector3(myTransform.localPosition.x, myTransform.localPosition.y, myTransform.localPosition.z);
        transform.localRotation = myTransform.localRotation;
        transform.localScale = new Vector3(myTransform.localScale.x, myTransform.localScale.y, myTransform.localScale.z);
    }
}
