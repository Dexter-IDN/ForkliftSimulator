using System;
using UnityEngine;

/// <summary>
/// Tipe penggerak kendaraan
/// </summary>
internal enum CarDriveType
{
    FrontWheelDrive,  // Penggerak roda depan
    RearWheelDrive,   // Penggerak roda belakang
    FourWheelDrive    // Penggerak empat roda
}

/// <summary>
/// Tipe kemudi kendaraan
/// </summary>
internal enum CarSteerType
{
    FrontSteerWheels, // Kemudi roda depan
    RearSteerWheels,  // Kemudi roda belakang
    FourSteerWheels   // Kemudi semua roda
}

/// <summary>
/// Satuan kecepatan
/// </summary>
internal enum SpeedType
{
    MPH, // Mil per jam
    KPH  // Kilometer per jam
}

/// <summary>
/// Kontroler untuk mengatur fisika dan perilaku kendaraan
/// </summary>
public class ForkliftController : MonoBehaviour
{
    // ===== PROPERTI KONFIGURASI =====
    [SerializeField] private int wheelCount;              // Jumlah roda yang bisa berputar (bisa digunakan untuk trailer)
    [SerializeField] private CarSteerType steerType = CarSteerType.FrontSteerWheels; // Tipe kemudi (depan/belakang/semua)
    [SerializeField] private CarDriveType driveType = CarDriveType.FourWheelDrive;   // Tipe penggerak (depan/belakang/4WD)
    
    // Referensi ke komponen fisika dan visual
    [SerializeField] private WheelCollider[] wheelColliders = new WheelCollider[4];  // Collider fisika roda
    [SerializeField] private GameObject[] wheelMeshes = new GameObject[4];           // Model 3D roda
    
    // Properti fisika kendaraan
    [SerializeField] private Vector3 centerOfMassOffset;       // Offset pusat massa untuk stabilitas
    [SerializeField] private float maximumSteerAngle;          // Sudut maksimal belok kemudi
    [Range(0, 1)] [SerializeField] private float steerHelper;  // Bantuan kemudi (0 = fisika mentah, 1 = kontrol penuh)
    [Range(0, 1)] [SerializeField] private float tractionControl; // Kontrol traksi (0 = tidak ada, 1 = maksimal)
    [SerializeField] private float fullTorqueOverAllWheels;    // Torsi maksimal pada semua roda
    [SerializeField] private float reverseTorque;              // Torsi untuk mundur
    [SerializeField] private float maxHandbrakeTorque;         // Torsi rem tangan maksimal
    [SerializeField] private float downforce = 100f;           // Gaya tekan ke bawah untuk grip
    [SerializeField] private SpeedType speedType;              // Satuan kecepatan (MPH/KPH)
    [SerializeField] private float topSpeed = 200;             // Kecepatan maksimal
    [SerializeField] private static int gearCount = 5;         // Jumlah gigi transmisi
    [SerializeField] private float revRangeBoundary = 1f;      // Batas rentang RPM
    [SerializeField] private float slipLimit;                  // Batas slip roda
    [SerializeField] private float brakeTorque;                // Torsi rem

    // ===== VARIABEL INTERNAL =====
    private Quaternion[] wheelMeshLocalRotations;  // Rotasi lokal mesh roda
    private Vector3 previousPosition, currentPosition; // Untuk kalkulasi pergerakan
    private float steerAngle;                      // Sudut kemudi saat ini
    private int currentGear;                       // Gigi transmisi saat ini
    private float gearFactor;                      // Faktor gigi untuk kalkulasi RPM
    private float oldRotation;                     // Rotasi sebelumnya untuk helper kemudi
    private float currentTorque;                   // Torsi saat ini
    private Rigidbody vehicleRigidbody;            // Referensi ke rigidbody kendaraan

    // Konstanta
    private const float reversingThreshold = 0.01f;

    // ===== PROPERTI PUBLIK =====
    public bool Skidding { get; private set; }             // Apakah kendaraan sedang tergelincir
    public float BrakeInput { get; private set; }          // Input rem
    public float CurrentSteerAngle { get { return steerAngle; } } // Sudut kemudi saat ini
    
    // Kecepatan saat ini dalam satuan yang sesuai
    public float CurrentSpeed { get { return vehicleRigidbody.velocity.magnitude * 2.23693629f; } }
    
    public float MaxSpeed { get { return topSpeed; } }     // Kecepatan maksimal
    public float Revs { get; private set; }                // RPM mesin
    public float AccelInput { get; private set; }          // Input akselerasi

    /// <summary>
    /// Inisialisasi saat permainan dimulai
    /// </summary>
    private void Start()
    {
        // Inisialisasi rotasi mesh roda
        wheelMeshLocalRotations = new Quaternion[wheelCount];
        for (int i = 0; i < wheelCount; i++)
        {
            wheelMeshLocalRotations[i] = wheelMeshes[i].transform.localRotation;
        }
        
        // Atur pusat massa
        wheelColliders[0].attachedRigidbody.centerOfMass = centerOfMassOffset;

        // Atur torsi rem tangan maksimal
        maxHandbrakeTorque = float.MaxValue;

        // Ambil referensi rigidbody
        vehicleRigidbody = GetComponent<Rigidbody>();
        
        // Hitung torsi awal dengan mempertimbangkan kontrol traksi
        currentTorque = fullTorqueOverAllWheels - (tractionControl * fullTorqueOverAllWheels);
    }

    /// <summary>
    /// Mengatur perpindahan gigi berdasarkan kecepatan
    /// </summary>
    private void GearChanging()
    {
        float speedRatio = Mathf.Abs(CurrentSpeed / MaxSpeed);
        float upGearLimit = 1 / (float)gearCount * (currentGear + 1);
        float downGearLimit = 1 / (float)gearCount * currentGear;

        // Turun gigi jika kecepatan di bawah batas bawah
        if (currentGear > 0 && speedRatio < downGearLimit)
        {
            currentGear--;
        }

        // Naik gigi jika kecepatan di atas batas atas
        if (speedRatio > upGearLimit && (currentGear < (gearCount - 1)))
        {
            currentGear++;
        }
    }

    /// <summary>
    /// Fungsi sederhana untuk memberikan bias kurva terhadap nilai dalam rentang 0-1
    /// </summary>
    private static float CurveFactor(float factor)
    {
        return 1 - (1 - factor) * (1 - factor);
    }

    /// <summary>
    /// Versi Lerp tanpa batas, memungkinkan nilai melebihi rentang from-to
    /// </summary>
    private static float ULerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }

    /// <summary>
    /// Menghitung faktor gigi untuk RPM
    /// </summary>
    private void CalculateGearFactor()
    {
        float gearWidth = 1.0f / gearCount;
        
        // Faktor gigi adalah representasi ternormalisasi dari kecepatan saat ini
        // dalam rentang kecepatan gigi saat ini
        var targetGearFactor = Mathf.InverseLerp(
            gearWidth * currentGear, 
            gearWidth * (currentGear + 1), 
            Mathf.Abs(CurrentSpeed / MaxSpeed)
        );
        
        // Transisi halus ke faktor gigi target
        gearFactor = Mathf.Lerp(gearFactor, targetGearFactor, Time.deltaTime * 5f);
    }

    /// <summary>
    /// Menghitung RPM mesin untuk tampilan/suara
    /// </summary>
    private void CalculateRevs()
    {
        // Hitung faktor gigi
        CalculateGearFactor();
        
        var gearNumFactor = currentGear / (float)gearCount;
        var revsRangeMin = ULerp(0f, revRangeBoundary, CurveFactor(gearNumFactor));
        var revsRangeMax = ULerp(revRangeBoundary, 1f, gearNumFactor);
        
        // Hitung RPM berdasarkan faktor gigi
        Revs = ULerp(revsRangeMin, revsRangeMax, gearFactor);
    }

    /// <summary>
    /// Method utama untuk menggerakkan kendaraan
    /// </summary>
    /// <param name="steering">Input kemudi (-1 sampai 1)</param>
    /// <param name="accel">Input akselerasi (0 sampai 1)</param>
    /// <param name="footbrake">Input rem kaki (-1 sampai 0)</param>
    /// <param name="handbrake">Input rem tangan (0 sampai 1)</param>
    public void Move(float steering, float accel, float footbrake, float handbrake)
    {
        // Perbarui posisi dan rotasi mesh roda sesuai dengan collider
        for (int i = 0; i < wheelCount; i++)
        {
            Quaternion rotation;
            Vector3 position;
            wheelColliders[i].GetWorldPose(out position, out rotation);
            wheelMeshes[i].transform.position = position;
            wheelMeshes[i].transform.rotation = rotation;
        }

        // Batasi nilai input
        steering = Mathf.Clamp(steering, -1, 1);
        AccelInput = accel = Mathf.Clamp(accel, 0, 1);
        BrakeInput = footbrake = -1 * Mathf.Clamp(footbrake, -1, 0);
        handbrake = Mathf.Clamp(handbrake, 0, 1);

        // Atur sudut kemudi berdasarkan tipe kemudi
        steerAngle = steering * maximumSteerAngle;
        
        switch (steerType)
        {
            case CarSteerType.FrontSteerWheels:
                // Kemudi roda depan
                wheelColliders[0].steerAngle = steerAngle;
                wheelColliders[1].steerAngle = steerAngle;
                break;
                
            case CarSteerType.RearSteerWheels:
                // Kemudi roda belakang
                wheelColliders[2].steerAngle = steerAngle;
                wheelColliders[3].steerAngle = steerAngle;
                break;
                
            case CarSteerType.FourSteerWheels:
                // Semua roda dapat berbelok
                wheelColliders[0].steerAngle = steerAngle;
                wheelColliders[1].steerAngle = steerAngle;
                wheelColliders[2].steerAngle = steerAngle;
                wheelColliders[3].steerAngle = steerAngle;
                break;
        }

        // Terapkan bantuan kemudi
        ApplySteerHelper();
        
        // Terapkan daya dorong dan rem
        ApplyDrive(accel, footbrake);
        
        // Batasi kecepatan maksimal
        LimitSpeed();

        // Terapkan rem tangan (di roda belakang)
        if (handbrake > 0f)
        {
            var handBrakeTorque = handbrake * maxHandbrakeTorque;
            wheelColliders[2].brakeTorque = handBrakeTorque;
            wheelColliders[3].brakeTorque = handBrakeTorque;
        }
        else
        {
            wheelColliders[2].brakeTorque = 0;
            wheelColliders[3].brakeTorque = 0;
        }

        // Hitung RPM dan ganti gigi
        CalculateRevs();
        GearChanging();

        // Tambahkan downforce dan kontrol traksi
        ApplyDownForce();
        ApplyTractionControl();
    }

    /// <summary>
    /// Membatasi kecepatan maksimal kendaraan
    /// </summary>
    private void LimitSpeed()
    {
        float speed = vehicleRigidbody.velocity.magnitude;
        
        switch (speedType)
        {
            case SpeedType.MPH:
                // Konversi ke MPH
                speed *= 2.23693629f;
                if (speed > topSpeed)
                    vehicleRigidbody.velocity = (topSpeed / 2.23693629f) * vehicleRigidbody.velocity.normalized;
                break;

            case SpeedType.KPH:
                // Konversi ke KPH
                speed *= 3.6f;
                if (speed > topSpeed)
                    vehicleRigidbody.velocity = (topSpeed / 3.6f) * vehicleRigidbody.velocity.normalized;
                break;
        }
    }

    /// <summary>
    /// Menerapkan gaya dorong dan pengereman
    /// </summary>
    private void ApplyDrive(float accel, float footbrake)
    {
        float thrustTorque;
        
        // Distribusi torsi berdasarkan tipe penggerak
        switch (driveType)
        {
            case CarDriveType.FourWheelDrive:
                // Distribusi ke semua roda
                thrustTorque = accel * (currentTorque / 4f);
                for (int i = 0; i < wheelCount; i++)
                {
                    wheelColliders[i].motorTorque = thrustTorque;
                }
                break;

            case CarDriveType.FrontWheelDrive:
                // Distribusi ke roda depan
                thrustTorque = accel * (currentTorque / 2f);
                wheelColliders[0].motorTorque = wheelColliders[1].motorTorque = thrustTorque;
                break;

            case CarDriveType.RearWheelDrive:
                // Distribusi ke roda belakang
                thrustTorque = accel * (currentTorque / 2f);
                wheelColliders[2].motorTorque = wheelColliders[3].motorTorque = thrustTorque;
                break;
        }

        // Terapkan pengereman atau mundur
        for (int i = 0; i < wheelCount; i++)
        {
            // Jika bergerak maju dan rem diinjak, terapkan rem
            if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, vehicleRigidbody.velocity) < 50f)
            {
                wheelColliders[i].brakeTorque = brakeTorque * footbrake;
            }
            // Jika diam atau bergerak mundur, terapkan torsi mundur
            else if (footbrake > 0)
            {
                wheelColliders[i].brakeTorque = 0f;
                wheelColliders[i].motorTorque = -reverseTorque * footbrake;
            }
        }
    }

    /// <summary>
    /// Bantuan kemudi untuk mencegah drift berlebihan
    /// </summary>
    private void ApplySteerHelper()
    {
        // Pastikan semua roda menyentuh tanah
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelHit;
            wheelColliders[i].GetGroundHit(out wheelHit);
            if (wheelHit.normal == Vector3.zero)
                return; // Roda tidak menyentuh tanah, jangan lakukan koreksi
        }

        // Hindari masalah gimbal lock yang bisa menyebabkan kendaraan tiba-tiba berubah arah
        if (Mathf.Abs(oldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnAdjust = (transform.eulerAngles.y - oldRotation) * steerHelper;
            Quaternion velocityRotation = Quaternion.AngleAxis(turnAdjust, Vector3.up);
            vehicleRigidbody.velocity = velocityRotation * vehicleRigidbody.velocity;
        }
        
        oldRotation = transform.eulerAngles.y;
    }

    /// <summary>
    /// Menambahkan gaya tekan ke bawah untuk meningkatkan grip
    /// </summary>
    private void ApplyDownForce()
    {
         if (vehicleRigidbody == null) return;

        // Tambahkan gaya ke bawah ke seluruh body kendaraan
        vehicleRigidbody.AddForce(
            -transform.up * downforce * vehicleRigidbody.velocity.magnitude,
            ForceMode.Force
        );
    }

    /// <summary>
    /// Kontrol traksi untuk mengurangi daya ke roda jika terjadi slip berlebihan
    /// </summary>
    private void ApplyTractionControl()
    {
        WheelHit wheelHit;
        
        switch (driveType)
        {
            case CarDriveType.FourWheelDrive:
                // Periksa semua roda
                for (int i = 0; i < 4; i++)
                {
                    wheelColliders[i].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                }
                break;

            case CarDriveType.RearWheelDrive:
                // Periksa roda belakang saja
                wheelColliders[2].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);

                wheelColliders[3].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);
                break;

            case CarDriveType.FrontWheelDrive:
                // Periksa roda depan saja
                wheelColliders[0].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);

                wheelColliders[1].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);
                break;
        }
    }

    /// <summary>
    /// Menyesuaikan torsi berdasarkan slip roda
    /// </summary>
    private void AdjustTorque(float forwardSlip)
    {
        // Jika terjadi slip berlebihan, kurangi torsi
        if (forwardSlip >= slipLimit && currentTorque >= 0)
        {
            currentTorque -= 10 * tractionControl;
        }
        else
        {
            // Jika tidak, kembalikan torsi secara bertahap
            currentTorque += 10 * tractionControl;
            
            // Batasi torsi maksimal
            if (currentTorque > fullTorqueOverAllWheels)
            {
                currentTorque = fullTorqueOverAllWheels;
            }
        }
    }
}