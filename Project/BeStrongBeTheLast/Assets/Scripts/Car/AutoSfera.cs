using UnityEngine;

public class AutoSfera : MonoBehaviour
{

    public Rigidbody sphere;
    public Vector3 diffSphere;
    public GameObject KartModel;

    public float steering, accellaration;
    private float currentSpeed, currentRotate;

    public Transform LookHere, Position;

    private GeneralCar generalCar;


    private void Start()
    {
        var MyCamera = Camera.main.GetComponent<CameraManager>();
        MyCamera.lookAtTarget = LookHere;
        MyCamera.positionTarget = Position;

        generalCar = GetComponent<GeneralCar>();
    }

    private void Update()
    {
        KartModel.transform.position = sphere.transform.position - diffSphere;

        var H = Input.GetAxis("Horizontal");

        currentRotate = Mathf.Lerp(currentRotate, steering * H, Time.deltaTime * 4f);
        currentSpeed = Mathf.SmoothStep(currentSpeed, accellaration, Time.deltaTime * 12f);

        if (currentSpeed > generalCar.Speed / 20)
            currentSpeed = generalCar.Speed / 20;
    }

    private void FixedUpdate()
    {
        sphere.AddForce(-1 * KartModel.transform.forward * currentSpeed, ForceMode.Acceleration);

        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);
    }


}