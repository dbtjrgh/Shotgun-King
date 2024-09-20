using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerShooting : MonoBehaviour
{
    // [Header("Fire rate")]
    // private int NProjectTile;
    private float fireCountdown = 0f;

    public GameObject firePoint;
    public Camera cam;

    public float maxLength;
    public GameObject projectTile;

    private Ray RayMouse;
    private Vector3 direction;
    private Quaternion rotation;

    private float buttonSaver = 0f;

    public Animation camAnim;
    private CCameraTransView cameraTransView;
    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();
    }
    private void Start()
    {
        //Counter(0);
    }
    private void Update()
    {
        if (cameraTransView.isInTopView)
        {
            return;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            camAnim.Play(camAnim.clip.name);
            Instantiate(projectTile, firePoint.transform.position, firePoint.transform.rotation);
        }

        fireCountdown -= Time.deltaTime;
        if (cam != null)
        {
            RaycastHit hit;
            var mousePos = Input.mousePosition;
            RayMouse = cam.ScreenPointToRay(mousePos);
            if (Physics.Raycast(RayMouse.origin, RayMouse.direction, out hit, maxLength))
            {
                // RotatateToMouseDirection(gameObject, hit.point);
            }
        }
    }

    private void RotatateToMouseDirection(GameObject obj, Vector3 destination)
    {
        direction = destination - obj.transform.position;
        rotation = Quaternion.LookRotation(direction);
        obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, rotation, 1);
    }

}
