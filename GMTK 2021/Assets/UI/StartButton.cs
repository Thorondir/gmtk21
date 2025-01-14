using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{

    public GameObject sndManager;

    bool panning = false;

    public float MAX_PAN = 10;
    public int PAN_SPEED = 1;

    public Sprite ButtonUp;
    public Sprite ButtonDown;

    SpriteRenderer renderer;

    double OnButton;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = ButtonUp;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (panning)
        {
            Camera.main.transform.position += new Vector3(PAN_SPEED*Time.deltaTime, 0, 0);
            if (Camera.main.transform.position.y >= MAX_PAN)
            {
                Camera.main.transform.position = new Vector3(0, (float)MAX_PAN, Camera.main.transform.position.z);
                panning = false;

                
            }
            }*/

        if (Time.time - OnButton >= 1 && OnButton > 0) {
            SceneManager.LoadScene(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null)
            return;
        if (other == FindObjectOfType<chainmanager>().chain[0])
        {
            sndManager.GetComponent<SoundManager>().playSound("scrape");
            
            panUp();
        }
        renderer.sprite = ButtonDown;
        OnButton = Time.time;
    }

    private void OnTriggerExit2D(Collider2D other) {
        OnButton = 0;
        renderer.sprite = ButtonUp;
    }

    void panUp()
    {
        panning = true;
    }
}
