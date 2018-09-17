using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Holovis : MonoBehaviour, IInputClickHandler
{
    private string holovis = "holovisText";
    private Vector3[] orderedPositions;
    private string[] characters = new string[] { "H", "o", "l", "o", "v", "i", "s" };
    private CustomText[] customTexts;
    private TMP_FontAsset arialFont;
    private float speed = 0.5f;

    // Use this for initialization
    void Start()
    {
        InputManager.Instance.AddGlobalListener(gameObject);
        arialFont = Resources.Load("Fonts & Materials/Calibri SDF", typeof(TMP_FontAsset)) as TMP_FontAsset;
        orderedPositions = new Vector3[characters.Length];
        float startPos = -1.4f;

        customTexts = new CustomText[characters.Length];
        for (int i = 0; i < customTexts.Length; ++i)
        {
            Color tempColor = Color.white;
            bool isItalics = false;
            if (characters[i] == "i")
            {
                tempColor = Color.blue;
                isItalics = true;
            }
            var ct = new CustomText(gameObject, arialFont, characters[i], tempColor, isItalics);
            customTexts[i] = ct;

            startPos += ct.Text.rectTransform.sizeDelta.x;

            if (characters[i] == "l")
            {
                orderedPositions[i] = new Vector3(startPos + 0.11f, 0, 10);
            }
            else
            {
                orderedPositions[i] = new Vector3(startPos, 0, 10);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 headPos = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit info;
        if (Physics.Raycast(headPos, gazeDirection, out info))
        {
            if (characters.Contains(info.transform.gameObject.name))
            {
                float step = speed * Time.deltaTime;
                for (int i = 0; i < characters.Length; ++i)
                {
                    customTexts[i].TextGameObject.transform.position =
                        Vector3.MoveTowards(customTexts[i].TextGameObject.transform.position, orderedPositions[i], step);
                }
            }
        }
    }

    Vector3 GetRandomVec3InsideFOV()
    {
        return new Vector3(UnityEngine.Random.Range(-0.7f, 0.7f), UnityEngine.Random.Range(-0.7f, 0.7f), UnityEngine.Random.Range(5.0f, 10f));
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        RaycastHit rayCastHit;
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;
        if (Physics.Raycast(headPosition, gazeDirection, out rayCastHit))
        {
            if (characters.Contains(rayCastHit.transform.gameObject.name))
            {
                for (int i = 0; i < customTexts.Length; ++i)
                {
                    customTexts[i].TextGameObject.transform.position = GetRandomVec3InsideFOV();
                }
            }
        }
    }
}

public class CustomText
{
    public GameObject TextGameObject { get; set; }
    public TextMeshPro Text { get; private set; }
    public GameObject MyCube { get; set; }
    public CustomText(GameObject go, TMP_FontAsset font, string letter, Color color, bool isItalics)
    {
        TextGameObject = new GameObject();
        TextGameObject.name = letter;
        TextGameObject.transform.parent = go.transform;
        TextGameObject.transform.position =
            new Vector3(UnityEngine.Random.Range(-0.7f, 0.7f), UnityEngine.Random.Range(-0.7f, 0.7f), UnityEngine.Random.Range(5.0f, 10f));

        var collider = TextGameObject.AddComponent<BoxCollider>();
        collider.size = new Vector3(.5f, 0.5f, 0.5f);
        Text = TextGameObject.AddComponent<TextMeshPro>();
        Text.name = letter;
        Text.fontSize = 8;
        Text.font = font;

        if (!isItalics)
        {
            Text.text = letter;
        }
        else
        {
            string italicized = "<i>" + letter + "</i>";
            Text.text = italicized;
        }

        Text.color = color;
        Text.rectTransform.sizeDelta = Text.GetPreferredValues(letter);
        Text.transform.parent = TextGameObject.transform;
        Text.rectTransform.position = TextGameObject.transform.position;
    }
}