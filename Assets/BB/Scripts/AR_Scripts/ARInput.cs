using UnityEngine;
public class ARInput : MonoBehaviour
{
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
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
