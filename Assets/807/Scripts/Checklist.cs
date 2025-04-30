using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class Checklist : MonoBehaviour
{
    [SerializeField] InputActionReference interactButton;

    [SerializeField] Toggle checkboxPrefab;
    [SerializeField] Transform campSection;
    [SerializeField] Transform concertSection;
    [SerializeField] Transform foodSection;

    [SerializeField] GameObject[] campObjects;
    [SerializeField] GameObject[] concertObjects;
    [SerializeField] GameObject[] foodObjects;

    List<Toggle> toggleList = new List<Toggle>();
    GameObject[] listObjects;
    bool[] checkArr;

    bool allCollected = false; // Win condition if true

    void Start()
    {
        listObjects = new GameObject[campObjects.Length + concertObjects.Length + foodObjects.Length];
        campObjects.CopyTo(listObjects, 0);
        concertObjects.CopyTo(listObjects, campObjects.Length);
        foodObjects.CopyTo(listObjects, concertObjects.Length + campObjects.Length);

        checkArr = new bool[listObjects.Length];
        for (int i = 0; i < checkArr.Length; i++)
        {
            checkArr[i] = false;
        }

        CreateCheckboxes();
     }

    void OnTriggerStay(Collider other)
    {
        if (CheckForObject(other.gameObject, listObjects) && interactButton.action.IsPressed())
        {
            other.gameObject.SetActive(false);
            UpdateChecklist(checkArr, listObjects);
        }
    }

    bool CheckForObject(GameObject obj, GameObject[] objArr)
    {
        foreach (GameObject arrObj in objArr)
        {
            if (obj == arrObj)
            {
                return true;
            }
        }
        return false;
    }

    void CreateCheckboxes()
    {
        int index = 0;

        foreach (GameObject obj in campObjects)
        {
            AddCheckbox(obj.name, index, campSection);
            index++;
        }

        foreach (GameObject obj in concertObjects)
        {
            AddCheckbox(obj.name, index, concertSection);
            index++;
        }

        foreach (GameObject obj in foodObjects)
        {
            AddCheckbox(obj.name, index, foodSection);
            index++;
        }

        // Refresh UI for Vertical Layout Group spcaing corrections
        LayoutRebuilder.ForceRebuildLayoutImmediate(campSection as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(concertSection as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(foodSection as RectTransform);
    }

    void AddCheckbox(string labelText, int index, Transform parent)
    {
        Toggle toggle = Instantiate(checkboxPrefab, parent);
        toggle.isOn = false;
        toggle.interactable = false;

        TMP_Text label = toggle.GetComponentInChildren<TMP_Text>();
        if (label != null)
            label.text = labelText;

        toggleList.Add(toggle);
    }

    void UpdateChecklist(bool[] boolArr, GameObject[] objArr)
    {
        for (int i = 0; i < boolArr.Length; i++)
        {
            boolArr[i] = !objArr[i].activeInHierarchy;
        }

        UpdateCheckboxVisuals();
        allCollected = IsAllCollected();
    }

    void UpdateCheckboxVisuals()
    {
        for (int i = 0; i < listObjects.Length; i++)
        {
            toggleList[i].isOn = !listObjects[i].activeInHierarchy;
        }
    }

    bool IsAllCollected()
    {
        bool collected = true;

        foreach (Toggle toggle in toggleList)
        {
            if (!toggle.isOn)
            {
                collected = false;
                break;
            }
        }

        return collected;
    }
}
