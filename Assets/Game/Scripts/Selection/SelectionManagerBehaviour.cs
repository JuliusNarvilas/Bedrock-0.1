using UnityEngine;
using Common.Properties.Numerical.Specialisations;

public class SelectionManagerBehaviour : MonoBehaviour
{
    public enum SelectableState
    {
        None,
        Selecting,
        Selected
    }

    private GameObject m_LastSelectableGameObject = null;
    private ISelectable m_InspectedSelectableComponent = null;
    private SelectableState m_SelectableState = SelectableState.None;

    static private SelectionManagerBehaviour s_Instance = null;
    
    static public SelectionManagerBehaviour Get()
    {
        Common.Log.DebugAssert(s_Instance != null, "Requesting uninitialised SelectionManager.");
        return s_Instance;
    }


    public SelectableState GetSelectableState()
    {
        return m_SelectableState;
    }
    public GameObject GetFocused()
    {
        return m_LastSelectableGameObject;
    }

    private void Awake()
    {
        Common.Log.DebugAssert(s_Instance == null, "SelectionManager already initialised.");
        if (s_Instance == null)
        {
            s_Instance = this;
        }

        NumericalPropertyInt<int> numericalProperty = new NumericalPropertyInt<int>(-10);
        var modifier = new NumericalPropertyIntModifier<int>(5, 0);
        numericalProperty.AddModifier(modifier);
    }

    private bool DeselectOld(GameObject i_HitGameObject)
    {
        if (m_LastSelectableGameObject != null)
        {
            if (i_HitGameObject != null)
            {
                //disregard if the same instance is being selected
                if (m_LastSelectableGameObject.GetInstanceID() == i_HitGameObject.GetInstanceID())
                {
                    return false;
                }
            }
            //Deselect old
            Common.Log.DebugLog("{0} deselected.", m_LastSelectableGameObject.name);
            m_LastSelectableGameObject = null;
            m_InspectedSelectableComponent.OnDeselected();
            m_InspectedSelectableComponent = null;
            m_SelectableState = SelectableState.None;
        }
        return true;
    }

    private Vector3 GetSelectingLocation()
    {
        return Input.mousePosition;
    }

    private bool IsSelected()
    {
        return Input.GetMouseButtonUp(0);
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(GetSelectingLocation());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitGameObject = hit.collider.gameObject;
            bool canSelectNew = DeselectOld(hitGameObject);
            ISelectable inspectedSelectable = hitGameObject.GetComponentInParent<ISelectable>();
            
            if(inspectedSelectable != null)
            {
                m_InspectedSelectableComponent = inspectedSelectable;
                if (IsSelected())
                {
                    if (m_SelectableState != SelectableState.Selected)
                    {
                        Common.Log.DebugLog("{0} selected.", hitGameObject.name);
                        m_SelectableState = SelectableState.Selected;
                        m_InspectedSelectableComponent.OnSelected();
                    }
                }
                else if (canSelectNew)
                {
                    //activate new
                    m_LastSelectableGameObject = hitGameObject;
                    Common.Log.DebugLog("{0} selecting.", hitGameObject.name);
                    m_SelectableState = SelectableState.Selecting;
                    m_InspectedSelectableComponent.OnSelecting();
                }
            }
            else
            {
                DeselectOld(null);
            }
        }
        else
        {
            DeselectOld(null);
        }
    }

    void OnDestroy()
    {
        if(s_Instance == this)
        {
            s_Instance = null;
        }
    }
}