using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CZoomCamera : MonoBehaviour
{
    #region ����
    public float orbitSpeed = 1.0f;
    public float zoomSpeed = 1.0f;
    public float panSpeed = 1.0f;
    public float autoOrbitSpeed = 0.0f;
    public float autoZoomSpeed = 0.0f;
    public float autoHorizontalPanSpeed = 0.0f;
    public float autoVerticalPanSpeed = 0.0f;
    public float originDistance = 1.0f;
    public Transform originReference;
    private CCameraTransView cameraTransView;

    private Vector3 lastMovement;
    private Vector2 deltaMovement;
    private Vector3 originPosition;
    #endregion

    private void OnDrawGizmos()
    {
        CalculateOrigin();
        Gizmos.DrawLine(transform.position, originPosition);
    }

    private void Awake()
    {
        CalculateOrigin();
        originReference = null;
        cameraTransView = FindObjectOfType<CCameraTransView>();
    }

    private void Update()
    {
        originPosition = GetTransformTranslatedPosition(transform, Vector3.forward * originDistance);
        deltaMovement = Input.mousePosition - lastMovement;
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            deltaMovement = Vector2.zero;
        }
        if (Input.GetMouseButton(0))
        {
            transform.RotateAround(originPosition, Vector3.up, deltaMovement.x * orbitSpeed * Time.deltaTime);
            transform.RotateAround(originPosition, transform.TransformDirection(Vector3.left), deltaMovement.y * orbitSpeed * Time.deltaTime);
        }
        else if (Input.GetMouseButton(1) && cameraTransView.isInTopView)
        {
            transform.Translate(Vector3.back * deltaMovement.y * zoomSpeed * Time.deltaTime);
            originDistance += deltaMovement.y * zoomSpeed * Time.deltaTime;
        }
        else if (Input.GetMouseButton(2))
        {
            transform.Translate(Vector3.left * deltaMovement.x * panSpeed * Time.deltaTime);
            transform.Translate(Vector3.down * deltaMovement.y * panSpeed * Time.deltaTime);
        }
        else
        {
            transform.RotateAround(originPosition, Vector3.up, autoOrbitSpeed * Time.deltaTime);
            transform.Translate(Vector3.back * autoZoomSpeed * Time.deltaTime);
            originDistance += autoZoomSpeed * Time.deltaTime;
            transform.Translate(Vector3.left * autoHorizontalPanSpeed * Time.deltaTime);
            transform.Translate(Vector3.down * autoVerticalPanSpeed * Time.deltaTime);
        }
        lastMovement = Input.mousePosition;
    }

    private Vector3 GetTransformTranslatedPosition(Transform r, Vector3 v)
    {
        Vector3 t = r.localPosition;
        r.Translate(v);
        Vector3 p = transform.position;
        r.localPosition = t;
        return p;
    }

    private void CalculateOrigin()
    {
        if (originReference)
        {
            originPosition = originReference.position;
            transform.LookAt(originPosition);
            originDistance = Vector3.Distance(transform.position, originPosition);
        }
        else
        {
            originPosition = GetTransformTranslatedPosition(transform, Vector3.forward * originDistance);
        }
    }
}
