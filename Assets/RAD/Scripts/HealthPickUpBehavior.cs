using UnityEngine;

public class HealthPickUpBehavior : MonoBehaviour
{
    SoundMeterController soundMeterController;

    void Start()
    {
        soundMeterController = FindAnyObjectByType<SoundMeterController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Hand")
        {
            soundMeterController.OnHealthPickUp();
            Destroy(this.gameObject);
        }
    }
}

    
