using UnityEngine;

public class EarplugBehavior : MonoBehaviour
{
    SoundMeterController soundMeterController;

    void Start()
    {
        soundMeterController = FindAnyObjectByType<SoundMeterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Hand")
        {
            soundMeterController.OnEarplugPickUp();
            Destroy(this.gameObject);
        }
    }
}
