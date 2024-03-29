using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private float _speed = 13f;

    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        float topDown = 8f;

        if (transform.position.y > topDown)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
}
