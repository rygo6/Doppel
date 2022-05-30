using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;

namespace GeoTetra.GTDoppel
{
    public class DoppelMesh : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler,
        IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, ISelectHandler,
        IDeselectHandler
    {
        [SerializeField] Transform m_Cursor;
        [SerializeField] Transform m_OutAnchorCursor;
        [SerializeField] Transform m_InAnchorCursor;

        [SerializeField] SplinePool m_SplinePool;

        [SerializeField] DoppelToolbar m_Toolbar;

        const float k_MeshOffset = .01f;
        const float k_TangentMultiplier = .1f;
        PointerEventData m_EnterPointerEventData;
        Ray m_PointerDownRay;
        Vector3 m_PointerDownPosition;
        Quaternion m_PointerDownRotation;

        void Update()
        {
            if (m_EnterPointerEventData is {pointerClick: null})
                m_Cursor.transform.position = m_EnterPointerEventData.pointerCurrentRaycast.worldPosition;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("OnPointerEnter");
            if (!eventData.dragging)
            {
                m_EnterPointerEventData = eventData;
                m_Cursor.transform.position = eventData.pointerCurrentRaycast.worldPosition;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("OnPointerExit");
            if (!eventData.dragging)
                m_EnterPointerEventData = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // var clickRay = eventData.pressEventCamera.ScreenPointToRay(eventData.pointerPressRaycast.screenPosition);
            // Vector3 position = clickRay.GetPoint(eventData.pointerPressRaycast.distance - 0.01f);
            // // var position = eventData.pointerCurrentRaycast.worldPosition;
            // var rotation = Quaternion.LookRotation(Vector3.up, eventData.pointerCurrentRaycast.worldNormal);
            // if (m_Toolbar.CurrentlySelectedItem == null)
            // {
            //     var spline = m_SplinePool.CreateSpline(position, rotation);
            //     m_Toolbar.CurrentlySelectedItem = spline;
            // }
            // else
            // {
            //     m_Toolbar.CurrentlySelectedItem.Spline.Add(new BezierKnot(position, 0, 0, rotation));
            // }
        }

        public void OnSelect(BaseEventData eventData)
        {
        }

        public void OnDeselect(BaseEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            m_OutAnchorCursor.gameObject.SetActive(true);
            m_InAnchorCursor.gameObject.SetActive(true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            (Ray ray, Vector3 position, Quaternion rotation) = MeshClickData(eventData);
            m_OutAnchorCursor.position = position;
            m_InAnchorCursor.localPosition = -m_OutAnchorCursor.localPosition;
            
            int knotCount = m_Toolbar.CurrentlySelectedItem.Spline.Count;
            var knot = m_Toolbar.CurrentlySelectedItem.Spline[knotCount - 1];
            knot.TangentOut = m_OutAnchorCursor.localPosition * k_TangentMultiplier;
            knot.TangentIn = m_InAnchorCursor.localPosition * k_TangentMultiplier;
            m_Toolbar.CurrentlySelectedItem.Spline[knotCount - 1] = knot;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            m_OutAnchorCursor.gameObject.SetActive(false);
            m_InAnchorCursor.gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            (m_PointerDownRay, m_PointerDownPosition, m_PointerDownRotation) = MeshClickData(eventData);

            if (m_Toolbar.CurrentlySelectedItem == null)
            {
                var spline = m_SplinePool.CreateSpline(m_PointerDownPosition, m_PointerDownRotation);
                m_Toolbar.CurrentlySelectedItem = spline;
            }
            else
            {
                m_Toolbar.CurrentlySelectedItem.Spline.Add(new BezierKnot(m_PointerDownPosition, 0, 0, m_PointerDownRotation));
            }

            m_Cursor.position = m_PointerDownPosition;
            m_Cursor.rotation = m_PointerDownRotation;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != gameObject)
                m_EnterPointerEventData = null;
        }

        (Ray, Vector3, Quaternion) MeshClickData(PointerEventData eventData)
        {
            var ray = eventData.enterEventCamera.ScreenPointToRay(eventData.pointerCurrentRaycast.screenPosition);
            var position = ray.GetPoint(eventData.pointerCurrentRaycast.distance - k_MeshOffset);
            var rotation = Quaternion.LookRotation(Vector3.up, eventData.pointerCurrentRaycast.worldNormal);
            return (ray, position, rotation);
        }
    }
}