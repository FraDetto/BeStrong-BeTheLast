using UnityEngine;

public class AutoSfera : MonoBehaviour
{

    public Transform LookHere, Position;

    public Rigidbody sphere;
    public Vector3 diffSphere;
    public GameObject KartModel, WheelFLModel, WheelFRModel;

    public float steering;
    private float steering_neg;

    private float currentSpeed, currentRotate;

    private GeneralCar generalCar;


    private void Start()
    {
        var MyCamera = Camera.main.GetComponent<CameraManager>();
        MyCamera.lookAtTarget = LookHere;
        MyCamera.positionTarget = Position;

        generalCar = GetComponent<GeneralCar>();

        steering_neg = steering * -1;
    }

    const float wheelAngleMultiplyer = 4;

    private void Update()
    {
        var H = Input.GetAxis("Horizontal");

        switch (H)
        {
            case 0:
                if (currentRotate > 0)
                    currentRotate -= wheelAngleMultiplyer;
                else if (currentRotate < 0)
                    currentRotate += wheelAngleMultiplyer;
                break;
            default:
                currentRotate += (H * wheelAngleMultiplyer);
                break;
        }

        if (currentRotate > steering)
            currentRotate = steering;
        else if (currentRotate < steering_neg)
            currentRotate = steering_neg;

        var kmh = GB.ms_to_kmh(sphere.velocity.magnitude);

        if (kmh < generalCar.Speed)
            currentSpeed++;

        WheelFLModel.transform.localEulerAngles = new Vector3(0, currentRotate, 0);
        WheelFRModel.transform.localEulerAngles = new Vector3(0, currentRotate, 0);

        KartModel.transform.position = sphere.transform.position - diffSphere;
        KartModel.transform.localEulerAngles = Vector3.Lerp(KartModel.transform.localEulerAngles, new Vector3(0, KartModel.transform.localEulerAngles.y + currentRotate, 0), Time.deltaTime * 3f);
    }

    private void FixedUpdate()
    {
        sphere.AddTorque(-1f * KartModel.transform.forward * generalCar.maxTorque);
    }


}