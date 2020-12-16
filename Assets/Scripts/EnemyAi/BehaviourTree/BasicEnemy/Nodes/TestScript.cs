using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour, IBehaviourTreeNode
{
    public void EnterState()
    {
        Debug.Log("THIS SHIT WORKS");
    }

    public void OnCollisionEnter(Collision collision)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TestPrint()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
