using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private float _speed = 13f;
    private enum LaserType
    {
        PlayerLaser,
        EnemyLaser
    }

    [SerializeField] private LaserType _laserType;

    private void Update()
    {
        Vector3 direction = IsPlayerLaser() ? Vector3.up : Vector3.down;

        transform.Translate(direction * _speed * Time.deltaTime);

        float topDown = 8f;

        if (transform.position.y > topDown || transform.position.y < -topDown)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayerLaser() && other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage(1);
            }
        }
    }

    private bool IsPlayerLaser()
    {
        return _laserType == LaserType.PlayerLaser;
    }
}
