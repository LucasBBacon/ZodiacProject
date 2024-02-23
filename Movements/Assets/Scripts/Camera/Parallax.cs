using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float _length, _startPos;
    public GameObject cam;
    public float parallaxEffect;

    private void Start() {
        _startPos = transform.position.x;
        _length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void FixedUpdate() {
        float dist = (cam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(_startPos + dist, transform.position.y, transform.position.z);
    }
}
