using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    public float m_baseSpeed;
	
    void Start()
    {
        Destroy(gameObject, 5.0f);
    }

	// Update is called once per frame
	void Update () {
        transform.Translate(Vector2.up * m_baseSpeed * Time.deltaTime);
	}
}
