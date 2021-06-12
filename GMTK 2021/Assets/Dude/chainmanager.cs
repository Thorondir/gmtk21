using System.Collections; using System.Collections.Generic;
using UnityEngine;

public class chainmanager : MonoBehaviour
{
    public GameObject linkObject;
    public GameObject chainObject;
    public List<GameObject> chain = new List<GameObject>();
    List<GameObject> chains = new List<GameObject>();
    Rigidbody2D rb;
    List<link_movement> movementScripts = new List<link_movement>();
    List<ChainStretcher> chainScripts = new List<ChainStretcher>();
    GameObject head;
    public List<Vector2> positions = new List<Vector2>();
    Vector2 lastPos;
    Vector2 direction;

    public float speed;
    public float drag = 0.75f;

    public float dist = 2; 
    // Start is called before the first frame update
    void Start()
    {
        summonDudes(5);
        rb = chain[0].GetComponent<Rigidbody2D>();
        //positions.Add(head.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        // move the head
        direction = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        direction.Normalize();
        rb.velocity = direction * speed;
        if (direction.magnitude == 0) {
            rb.velocity = rb.velocity * drag;
        }

        // get the current position and potentially push the new position if it's far enough away from the last
        Vector2 pos = head.transform.position;
        if (lastPos == null || (lastPos - pos).magnitude > dist) {
            positions.Add(pos);
            lastPos = pos;
        }

        for (int i = 1; i < chain.Count; i++) {
            chainScripts[i-1].Join(chain[i],chain[i-1]);
            if (positions.Count - i >= 0) {
                movementScripts[i].moveTowards(positions[positions.Count - i], (Vector2) chain[i-1].transform.position, dist);

                if (i == chain.Count-1 && positions.Count > i) {
                    positions.RemoveAt(0);
                }
            }
        }
    }

    void summonDude() {
        chain.Add(Instantiate(linkObject, gameObject.transform));
        movementScripts.Add(chain[chain.Count - 1].GetComponent<link_movement>());
        if (chain.Count > 1) {
            chains.Add(Instantiate(chainObject, gameObject.transform));
            chainScripts.Add(chains[chains.Count - 1].GetComponent<ChainStretcher>());
            chain[chain.Count - 1].transform.position = chain[chain.Count - 2].transform.position - new Vector3(0, dist, 0);
        }
        if (chain.Count == 1) {
            head = chain[0];
        }
    }

    void summonDudes(int dudes) {
        for (int i = 0; i < dudes; i++) {
            summonDude();
        }
    }
}
