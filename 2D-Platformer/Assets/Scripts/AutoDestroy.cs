using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
	[SerializeField] float lifeTime = 3;
    // Start is called before the first frame update
    void Start()
    {
		Destroy(this.gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
