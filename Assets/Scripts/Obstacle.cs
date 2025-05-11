using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // Random snelheid — vaste breedte en diepte
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        // Randomize snelheid tussen 3 en 5
        speed = Random.Range(3f, 5f);

        // Stel vaste schaal in: x = 1, y = 1, z = 5 bijvoorbeeld
        transform.localScale = new Vector3(1f, 1f, 5f); // z is nu vast op 5 (of een andere waarde die je wilt)

        // Zet de positie van het obstakel op een vaste beginpositie (x = -4, y = 0.5, z = 0)
        transform.position = new Vector3(-4f, 0.5f, 0f); // Nu altijd x = -4, y = 0.5, z = 0
    }

    // Update is called once per frame
    void Update()
    {
        // Beweeg het obstakel horizontaal (langs de x-as)
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Verwijder het obstakel als het buiten de zichtbare ruimte komt
        if (transform.position.x > 5f)
        {
            Destroy(gameObject);
        }
    }
}
