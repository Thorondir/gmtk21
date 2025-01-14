using System.Collections; using System.Collections.Generic;
using UnityEngine;

public class chainmanager : MonoBehaviour
{
    public GameObject linkObject;
    public GameObject chainObject;
    public Health hp;

    public List<GameObject> chain = new List<GameObject>();
    int headindex = 0;
    List<GameObject> chains = new List<GameObject>();
    Rigidbody2D rb;
    List<link_movement> movementScripts = new List<link_movement>();
    List<ChainStretcher> chainScripts = new List<ChainStretcher>();
    public GameObject head;
    public List<Vector2> positions = new List<Vector2>();
    Vector2 pos;
    Vector2 lastPos;
    Vector2 direction;
    int nextTarget;

    public float maxspeed;
    public float speed;
    public float drag = 0.75f;

    public float dist = 2; 

    public GameObject target;

    public GameObject loseGame;
    // Start is called before the first frame update
    void Start()
    {
        summonDudes(5);
        rb = chain[headindex].GetComponent<Rigidbody2D>();
        hp = chain[headindex].GetComponent<Health>();
        //positions.Add(head.transform.position);
    }

    public GameObject GetLast()
    {
        GameObject lastGuy = head;
        for (int i = 0; i < chain.Count; i++)
        {
            if (chain[i].GetComponent<Health>().health > 0)
                lastGuy = chain[i];
        }
        return lastGuy;
    }

    // Update is called once per frame
    void Update()
    {
        speed = maxspeed;
        for (int i = 0; i < chain.Count; i++) {
            if (movementScripts[i].dead) {
                speed -= maxspeed/10;
            }
        }
        if (headindex < chain.Count) {
            while (hp.health <= 0) {
                headindex++;
                if (headindex < chain.Count) {
                    head = chain[headindex];
                    rb = head.GetComponent<Rigidbody2D>();
                    hp = head.GetComponent<Health>();
                } else {
                    break;
                }
            }

            // move the head
            direction = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            direction.Normalize();

            rb.velocity = direction * speed;
            if (direction.magnitude == 0) {
                rb.velocity = rb.velocity * drag;
            }

            // get the current position and potentially push the new position if it's far enough away from the last
            pos = head.transform.position;
            if (lastPos == null || (lastPos - pos).magnitude > dist) {
                positions.Add(pos);
                lastPos = pos;
            }

            nextTarget = positions.Count-1;
            for (int i = 0; i < chain.Count; i++) {
                if (!movementScripts[i].dead && Input.GetButtonDown("Jump"))
                    movementScripts[i].AttemptAttack(target);
                if (i > 0)
                    chainScripts[i-1].Join(chain[i],chain[i-1]);

                if (chain[i] != head) {
                    if (positions.Count - i >= 0) {
                        if (i > headindex)
                            movementScripts[i].moveTowards(positions[nextTarget--], (Vector2) chain[i-1].transform.position, dist);
                        else 
                            movementScripts[i].moveTowards(positions[nextTarget--], (Vector2) chain[i+1].transform.position, dist);

                        if (positions.Count > chain.Count) {
                            positions.RemoveAt(0);
                        }
                    }
                }
            }
        } else {
            loseGame.GetComponent<LoseGame>().Lose();
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
