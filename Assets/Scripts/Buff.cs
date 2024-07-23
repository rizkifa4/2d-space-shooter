using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    private float _speed = 3f;

    private enum PowerUpName
    {
        TripleShot,
        SpeedBoost,
        Shield,
        RestoreHealth,
        SpeedDecrease,
        RestoreAmmo
    }

    [SerializeField] private PowerUpName _powerUpName;
    private SoundManager _soundManager;

    private void Start()
    {
        _soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
    }

    private void Update()
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
            Player playerCol = other.GetComponent<Player>();

            if (playerCol != null)
            {
                PowerUpActive(playerCol);
            }
        }
    }

    private void PowerUpActive(Player player)
    {
        switch (_powerUpName)
        {
            case PowerUpName.TripleShot:
                if (!player.IsTripleShotEnabled)
                {
                    player.TripleShotActive();
                    Destroy(this.gameObject);
                }
                break;
            case PowerUpName.SpeedBoost:
                if (!player.IsSpeedBoostEnabled)
                {
                    player.SpeedBoostPowerupActive();
                    Destroy(this.gameObject);
                }
                break;
            case PowerUpName.Shield:
                if (!player.IsShieldEnabled)
                {
                    player.ShieldPowerupActive();
                    Destroy(this.gameObject);
                }
                break;
            case PowerUpName.RestoreHealth:
                if (player.Health < 3)
                {
                    player.RestoreHealth();
                    Destroy(this.gameObject);
                }
                break;
            case PowerUpName.SpeedDecrease:
                if (!player.IsSpeedDecreaseEnabled)
                {
                    player.SpeedDecreasePowerupActive();
                    Destroy(this.gameObject);
                }
                break;
            case PowerUpName.RestoreAmmo:
                if (player.CurrentAmmo < player.MaxAmmo)
                {
                    player.RestoreAmmo(player.MaxAmmo);
                    Destroy(this.gameObject);
                }
                break;
            default:
                Debug.Log("Default Value");
                break;
        }
    }
}