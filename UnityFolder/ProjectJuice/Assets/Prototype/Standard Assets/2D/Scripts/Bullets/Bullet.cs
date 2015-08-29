using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    public float m_baseSpeed;
	
    void Start()
    {
        Destroy(gameObject, 0.5f);
    }

	// Update is called once per frame
	void Update () {
        transform.Translate(Vector2.up * m_baseSpeed * Time.deltaTime);
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.layer == 8)
        {
            print("ground");
            Destroy(this);
        }
    }
}
