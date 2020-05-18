using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PurchaseMenu : MonoBehaviour
{
    public Entity TargetEntity;
    public GameObject PurchasePanel;
    private float renderRange = 5f;
    public Text gunText;

    // Start is called before the first frame update
    void Start()
    {
        SetTargetNearestPlayer();
    }



    // Update is called once per frame
    void Update()
    {
        if (IsWithinRenderRange())
        {
            PurchasePanel.SetActive(true);
        }
        else
        {
            PurchasePanel.SetActive(false);
        }
    }

    public void SetText(string text)
    {
        gunText.text = text;
    }

    private void SetTargetNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        GameObject closest = SearchUtil.FindClosest(players, transform.position);

        if (closest != null)
        {
            Entity entityPlayer = closest.GetComponent<Entity>();

            if (entityPlayer != null)
            {
                SetTarget(entityPlayer);
            }
        }
    }

    public void SetTarget(Entity entity)
    {
        // Set target entity
        TargetEntity = entity;
    }

    private bool IsWithinRenderRange()
    {
        if (TargetEntity == null)
            return false;

        // Calculate square distance
        float sqrDistance = (TargetEntity.transform.position - transform.position).sqrMagnitude;

        return sqrDistance < renderRange * renderRange;
    }
}
