using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class Checklist : MonoBehaviour
{
    [SerializeField] public InputActionReference interactLButton;
    [SerializeField] public InputActionReference interactRButton;

    [SerializeField] Toggle checkboxPrefab;
    [SerializeField] Transform campSection;
    [SerializeField] Transform concertSection;
    [SerializeField] Transform foodSection;

    [SerializeField] GameObject[] campObjects;
    [SerializeField] GameObject[] concertObjects;
    [SerializeField] GameObject[] foodObjects;

    List<Toggle> toggleList = new List<Toggle>();
    GameObject[] checkObjects;
    bool[] checkArr;

    bool allCollected = false; // Win condition if true

    void Start()
    {
        checkObjects = new GameObject[campObjects.Length + concertObjects.Length + foodObjects.Length];
        campObjects.CopyTo(checkObjects, 0);
        concertObjects.CopyTo(checkObjects, campObjects.Length);
        foodObjects.CopyTo(checkObjects, concertObjects.Length + campObjects.Length);

        checkArr = new bool[checkObjects.Length];
        for (int i = 0; i < checkArr.Length; i++)
        {
            checkArr[i] = false;
        }

        CreateCheckboxes();
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

        if (allCollected)
        {
            // Display win Screen and other 
        }
    }

    void UpdateCheckboxVisuals()
    {
        for (int i = 0; i < checkObjects.Length; i++)
        {
            toggleList[i].isOn = !checkObjects[i].activeInHierarchy;
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

    public void ObjectInteraction(GameObject obj)
    {
        if (CheckForObject(obj, checkObjects))
        {
            obj.SetActive(false);
            UpdateChecklist(checkArr, checkObjects);
        }
    }
}
