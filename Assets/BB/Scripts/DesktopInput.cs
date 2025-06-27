using UnityEngine;

public class DesktopInput : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Bubble"))
                {
                    hit.collider.GetComponent<Bubble>().Pop();
                }
            }
        }
    }
}