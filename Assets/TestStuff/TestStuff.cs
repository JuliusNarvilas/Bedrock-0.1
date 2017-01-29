using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TestEnum
{
    TestEnum1,
    TestEnum2,
    TestEnum3
}

[System.Serializable]
public class TestListElement
{
    public string m_id;
    public TestEnum m_type;
    public int m_quantity;
}

[System.Serializable]
public class TestList
{
    public List<TestListElement> m_List = new List<TestListElement>();
}


public class TestStuff : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
