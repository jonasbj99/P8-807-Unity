using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Checklist : MonoBehaviour
{
    [SerializeField] InputActionReference interactButton;

    [SerializeField] GameObject[] campObjects;
    [SerializeField] GameObject[] concertObjects;
    [SerializeField] GameObject[] foodObjects;

    GameObject[] listObjects;

    void Start()
    {
        listObjects = new GameObject[campObjects.Length + concertObjects.Length + foodObjects.Length];
        campObjects.CopyTo(listObjects, 0);
        concertObjects.CopyTo(listObjects, campObjects.Length);
        foodObjects.CopyTo(listObjects, concertObjects.Length + campObjects.Length);


    }

    void OnTriggerStay(Collider other)
    {
        if (CheckForObject(other.gameObject, listObjects) && interactButton.action.IsPressed())
        {
            other.gameObject.SetActive(false);
        }
    }

    bool CheckForObject(GameObject obj, GameObject[] objArr)
    {
        foreach (GameObject listObj in objArr)
        {
            if (obj == listObj)
            {
                return true;
            }
        }

        return false;
    }

    void CreateChecklist()
    {
        foreach(GameObject listObj in listObjects)
        {

        }
    }
}
