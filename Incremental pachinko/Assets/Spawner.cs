using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject ballPrefab;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnBall();
        }
    }

    private void SpawnBall()
    {
        Instantiate(ballPrefab, new Vector3(0f, 35f, Random.Range(-1f, 1f)), Quaternion.identity);
    }
}
