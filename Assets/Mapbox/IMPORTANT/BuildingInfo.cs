using UnityEngine;

public class BuildingInfo : MonoBehaviour
{
    public bool isShop = false;
    public int difficulty = 1; // range: 1-5
    //public int floors = 1; // range: 1-3

    [SerializeField] bool difficultyFromSize = true;

    private void Start()
    {
        if (difficultyFromSize)
        {
            BoxCollider parentMesh = GetComponentInParent<BoxCollider>();
            if (parentMesh == null)
            {
                Debug.Log("box collider does not exist on parent");
                return;
            }

            // get size of building and assign difficulty and floors
            Vector3 meshSize = parentMesh.bounds.size;
            difficulty = (int)Mathf.Floor((meshSize.x + meshSize.y) / 10 + Random.Range(-0.5f, 1.6f)); // difficulty is changed by a little
            if (difficulty < 1)
                difficulty = 1;
            /*
            if (meshSize.y < 3)
                floors = 1;
            else if (meshSize.y < 6)
                floors = 2;
            else
                floors = 3;
            */
        }

        transform.parent.tag = "Wall";
    }
}
