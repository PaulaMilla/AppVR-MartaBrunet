using UnityEngine;

public class CajaAmarilla : MonoBehaviour
{
    public RoomController roomController;

    void OnTriggerEnter(Collider other)
    {
        //Comprobar si el objeto que entró es el objeto clave
        if (other.CompareTag("KeyObject"))
        {

            if (roomController != null)
            {
                roomController.NotificarObjetoDestruido(other.gameObject);
            }

            Destroy(other.gameObject);

            //Avisar al "cerebro" (RoomController) que la tarea se completó
            if (roomController != null)
            {
                roomController.TareaCompletada();
            }
        }
    }
}
