using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private float _speed = 3f;

    private enum PowerUpName
    {
        TripleShot,
        Speed,
        Shield
    }

    [SerializeField] private PowerUpName _powerUpName;
    private SoundManager _soundManager;

    private void Start()
    {
        _soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
    }

    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        float bottomDown = 6;

        if (transform.position.y < -bottomDown)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                // if (_powerUpName == PowerUpName.TripleShot)
                // {
                //     player.TripleShotActive();
                // }
                // else if (_powerUpName == PowerUpName.Speed)
                // {
                //     Debug.Log("Speed Powerup Collected");
                // }
                // else
                // {
                //     Debug.Log("Shield Powerup Collected");
                // }

                _soundManager.PlaySoundEffect(_soundManager.powerupSound);

                switch (_powerUpName)
                {
                    case PowerUpName.TripleShot:
                        player.TripleShotActive();
                        break;
                    case PowerUpName.Speed:
                        player.SpeedPowerupActive();
                        break;
                    case PowerUpName.Shield:
                        player.ShieldPowerupActive();
                        break;
                    default:
                        Debug.Log("Default Value");
                        break;
                }
            }

            Destroy(this.gameObject);
        }
    }
}