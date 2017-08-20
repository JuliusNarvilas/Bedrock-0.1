using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum ETestEnum
{
    TestEnum1,
    TestEnum2,
    TestEnum3
}

[System.Serializable]
public class TestListElement
{
    public string m_id;
    public ETestEnum m_type;
    public int m_quantity;
}

[System.Serializable]
public class TestList
{
    public List<TestListElement> m_List = new List<TestListElement>();
}


public class TestStuff<Test> : MonoBehaviour {

    public Test font;
}

