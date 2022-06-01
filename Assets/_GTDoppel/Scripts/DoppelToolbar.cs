using GeoTetra.GTDoppel;
using GeoTetra.GTSplines;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;

public class DoppelToolbar : MonoBehaviour
{
    GTSplineContainer m_CurrentlySelectedItem;
    DoppelTool m_CurrentTool;

    public GTSplineContainer CurrentlySelectedItem
    {
        get => m_CurrentlySelectedItem;
        set => m_CurrentlySelectedItem = value;
    }

    public DoppelTool CurrentTool
    {
        get => m_CurrentTool;
        set => m_CurrentTool = value;
    }

    void Awake()
    {
        m_CurrentTool = new SplineTool();
    }
}

public abstract class DoppelTool
{
    public abstract void MeshClicked(PointerEventData data, DoppelMesh mesh);
}

public class SplineTool : DoppelTool
{
    DoppelToolbar m_Toolbar;

    public override void MeshClicked(PointerEventData data, DoppelMesh mesh)
    {

    }
}