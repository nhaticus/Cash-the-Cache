using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInventory : MonoBehaviour
{
    List<ItemInfo> items = new List<ItemInfo>();

    public float delay = 0.6f;
    public bool canPick = false;

    private void Update()
    {
        if (!canPick)
        {
            delay -= Time.deltaTime;
            if (delay <= 0)
            {
                canPick = true;
                delay = 0.6f;
            }
        }
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PickUp o = Instantiate(items[0].Prefab.GetComponent<PickUp>(), transform.position, Quaternion.identity);
            o.SetItem(items[0]);
            items.Remove(items[0]);
            Debug.Log("create");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canPick)
        {
            PickUp item = other.GetComponent<PickUp>();
            if (item)
            {
                items.Add(item.itemToPickUp);
                Destroy(other.gameObject);
                Debug.Log("add");
                canPick = false;
            }
        }
        
    }
}
