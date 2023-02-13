using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlighter : MonoBehaviour
{
    public GameObject Highlight;
    
    public GameObject HighlightLeftTarget { get; set; }
    public GameObject HighlightRightTarget { get; set; }
    public Color HighlightColorL
    {
        get => highlightColorL;
        set
        {
            highlightColorL = value;
            foreach (Renderer r in highlighterL.GetComponentsInChildren<Renderer>())
            {
                foreach (Material material in r.materials)
                {
                    material.SetColor("_BaseColor", highlightColorL);
                }
            }
        }
    }

    public Color HighlightColorR
    {
        get => highlightColorR;
        set
        {
            highlightColorR = value;
            foreach (Renderer r in highlighterR.GetComponentsInChildren<Renderer>())
            {
                foreach (Material material in r.materials)
                {
                    material.SetColor("_BaseColor", highlightColorR);
                }
            }
        }
    }
    
    private GameObject highlighterL;
    private GameObject highlighterR;
    private Color highlightColorL;
    private Color highlightColorR;
    private GameObject highlightLeftTarget;

    
    void OnEnable()
    {
        highlighterL = Instantiate(Highlight, Vector3.zero, Quaternion.identity);
        highlighterL.SetActive(false);
        highlighterR = Instantiate(Highlight, Vector3.zero, Quaternion.identity);
        highlighterR.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (HighlightLeftTarget != null)
        {
            highlighterL.SetActive(true);
            highlighterL.transform.position = HighlightLeftTarget.transform.position;
            highlighterL.transform.rotation = HighlightLeftTarget.transform.rotation;
            highlighterL.transform.localScale = new Vector3(0.005f, 0.012f, 0.012f);
            highlighterL.transform.parent = HighlightLeftTarget.transform;
        }
        else
        {
            highlighterL.SetActive(false);
        }

        if (HighlightRightTarget != null)
        {
            highlighterR.SetActive(true);
            highlighterR.transform.position = HighlightRightTarget.transform.position;
            highlighterR.transform.rotation = HighlightRightTarget.transform.rotation;
            highlighterR.transform.localScale = new Vector3(0.005f, 0.012f, 0.012f);
            highlighterR.transform.parent = HighlightRightTarget.transform;
        }
        else
        {
            highlighterR.SetActive(false);
        }
    }

    public void UnsetTargets()
    {
        HighlightLeftTarget = null;
        highlighterL.transform.parent = null;
        HighlightRightTarget = null;
        highlighterR.transform.parent = null;
    }

}
