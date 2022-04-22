using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField]
    private CharacterStats enemy_Stats;

    [SerializeField]
    private float updateSpeed;

    private GameObject healthBar;
    private float previousPCT;
    

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        previousPCT = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate () 
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
    }

    public IEnumerator takeDamageUI(float newPCT) 
    {
        float elapsed = 0f;

        while (elapsed < updateSpeed) {
            elapsed += Time.deltaTime;
            healthBar.transform.localScale = new Vector3 (Mathf.Lerp(40 * previousPCT, 40 * newPCT, elapsed / updateSpeed), 0.3f, 1f);
            yield return null;
        }

        previousPCT = newPCT;
    }
}
