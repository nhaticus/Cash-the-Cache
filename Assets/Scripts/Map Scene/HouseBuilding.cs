using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Building Type for basic housing
 */

public class HouseBuilding : BaseMapBuilding
{
    [Header("Level Descriptors")]
    public int difficulty = 1;
    [SerializeField] bool difficultyFromSize = true;

    [Header("Map Dependencies")]
    [SerializeField] string levelScene = "Level Gen";

    private void Start()
    {
        if (difficultyFromSize)
        {
            BoxCollider collider = GetComponent<BoxCollider>();
            if (collider == null)
            {
                Debug.Log("box collider does not exist");
                return;
            }

            // get size of building and assign difficulty and floors
            Vector3 meshSize = collider.bounds.size;
            difficulty = (int) Mathf.Floor((meshSize.x + meshSize.y) / 10);
            if (difficulty < 1)
                difficulty = 1;
        }

        // change difficulty a little
        difficulty += (int) Mathf.Floor(difficulty + Random.Range(-0.6f, 1.5f));
    }

    public override void CreateCanvas(GameObject playerCam)
    {
        GameObject houseCanvas = CreateFollowingCanvas(playerCam);
        houseCanvas.GetComponent<HouseCanvas>().difficulty = difficulty;
    }

    public override void Interact()
    {
        PlayerPrefs.SetInt("Difficulty", difficulty);
        SceneManager.LoadScene(levelScene);
    }
}
