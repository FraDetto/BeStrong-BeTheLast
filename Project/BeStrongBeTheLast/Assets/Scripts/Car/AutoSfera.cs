using UnityEngine;

public class AutoSfera : MonoBehaviour
{

    public Transform LookHere, Position;

    public Rigidbody sphere;
    public Vector3 diffSphere;
    public GameObject KartModel;

    public float steering, accellaration;


    private float currentSpeed, currentRotate, maxS;

    private GeneralCar generalCar;


    private void Start()
    {
        var MyCamera = Camera.main.GetComponent<CameraManager>();
        MyCamera.lookAtTarget = LookHere;
        MyCamera.positionTarget = Position;

        generalCar = GetComponent<GeneralCar>();
        maxS = generalCar.Speed / 15;
    }

    private void Update()
    {
        KartModel.transform.position = sphere.transform.position - diffSphere;
        //KartModel.transform.eulerAngles = new Vector3(KartModel.transform.eulerAngles.x, sphere.transform.eulerAngles.y, KartModel.transform.eulerAngles.z);

        var H = Input.GetAxis("Horizontal");

        currentRotate = Mathf.Lerp(currentRotate, steering * H, Time.deltaTime);
        currentSpeed = Mathf.SmoothStep(currentSpeed, accellaration, Time.deltaTime);

        if (currentSpeed > maxS)
            currentSpeed = maxS;

        // Debug.Log(currentRotate + " | " + currentSpeed);
    }

    private void FixedUpdate()
    {
        //sphere.transform.RotateAroundLocal(Vector3.up, currentRotate * Time.deltaTime * 5f);
        KartModel.transform.eulerAngles = Vector3.Lerp(KartModel.transform.eulerAngles, new Vector3(0, KartModel.transform.eulerAngles.y + currentRotate, 0), Time.deltaTime);
        sphere.AddForce(-1 * KartModel.transform.forward * currentSpeed, ForceMode.Acceleration);
    }


}