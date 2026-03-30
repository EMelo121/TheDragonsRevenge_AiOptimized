using UnityEngine;
using UnityEngine.SceneManagement;

public class OWEnemyScript : MonoBehaviour
{
    [Tooltip("The combat scene that should load when this enemy is encountered.")]
    public string combatScene;

    [SerializeField]
    [Tooltip("Determines whether this enemy patrols between two points.")]
    private bool hasPatrol;

    [Tooltip("The first patrol point.")]
    public GameObject pointA;

    [Tooltip("The second patrol point.")]
    public GameObject pointB;

    [Tooltip("The movement speed used while patrolling.")]
    public float speed;

    // Reference to the enemy's sprite renderer, used to flip the sprite when changing direction.
    private SpriteRenderer sprite;

    // The patrol point the enemy is currently moving toward.
    private Transform destination;

    private void Start()
    {
        // Cache required component references when the script starts.
        sprite = GetComponent<SpriteRenderer>();

        // Initialize the first patrol destination.
        destination = pointB.transform;
    }

    private void Update()
    {
        // Only run patrol behavior if patrol is enabled for this enemy.
        if (hasPatrol == true)
        {
            // Move the enemy toward the current patrol destination.
            transform.position = Vector2.MoveTowards(
                transform.position,
                destination.position,
                speed * Time.deltaTime
            );

            // When the enemy reaches point B, flip direction and switch to point A.
            if (Vector2.Distance(transform.position, destination.position) < 0.5f &&
                destination == pointB.transform)
            {
                FlipSprite();
                destination = pointA.transform;
            }

            // When the enemy reaches point A, flip direction and switch to point B.
            if (Vector2.Distance(transform.position, destination.position) < 0.5f &&
                destination == pointA.transform)
            {
                FlipSprite();
                destination = pointB.transform;
            }
        }
    }

    /// Flips the enemy sprite horizontally to match a direction change.
    private void FlipSprite()
    {
        sprite.flipX = !sprite.flipX;
    }

    /// Loads the combat scene associated with this overworld enemy.
    public void LoadCombatScene()
    {
        Debug.Log("Loading " + combatScene);
        SceneManager.LoadScene(combatScene);
    }
}