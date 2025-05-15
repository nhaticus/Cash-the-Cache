using UnityEngine;

public class BuildingInfo : MonoBehaviour
{
    public bool isShop = false;
    public int difficulty = 1; // range: 1-5

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
            difficulty = (int)Mathf.Floor((meshSize.x + meshSize.y) / 10);
            if (difficulty < 1)
                difficulty = 1;
        }
        difficulty += (int) Mathf.Floor(difficulty + Random.Range(-0.4f, 1.3f));  // difficulty is changed by a little
        transform.parent.tag = "Wall";
    }
}
