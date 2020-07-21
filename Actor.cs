using System;
using System.Collections;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.UI;
using UnityEditor;

public class Actor : Hurtable
{
    #region Actor
    public float AcorLife = 100;
    private float AcorSpeed = 20;
    private float AcorJumpSpeed = 5;
    private float AcorSwimSpeed = 10;
    private int AcorHashCode;
    public bool respondstoInput = true;
    public Transform mycamera;
    private Transform reference;

    public float jumpHeight = 2.0f;
    public float jumpinterval = 1.5f;
    private float nextjump = 1.2f;
    private float maxhitpoints = 1000f;
    private float hitpoints = 1000f;
    public float regen = 100f;
    public Text healthtext;
    public AudioClip[] hurtsounds;
    public RawImage painflashtexture;
    private float alpha;
    public Transform recoilCamera;

    public float gravity = 20.0f;
    public float rotatespeed = 4.0f;
    private float speed;
    public float normalspeed = 4.0f;
    public float runspeed = 8.0f;
    public float crouchspeed = 1.0f;
    public float crouchHeight = 1;
    private bool crouching = false;
    public float normalHeight = 2.0f;
    public float camerahighposition = 1.75f;
    public float cameralowposition = 0.9f;
    private float cameranewpositionY;
    private Vector3 cameranewposition;
    private float cameranextposition;
    public float dampTime = 2.0f;



    private float moveAmount;
    public float smoothSpeed = 2.0f;

    private Vector3 forward = Vector3.forward;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 right;

    private float movespeed;
    public Vector3 localvelocity;


    public bool climbladder = false;
    public Quaternion ladderRotation;
    public Vector3 ladderposition;
    public Vector3 ladderforward;
    public Vector3 climbdirection;

    public float climbspeed = 2.0f;


    public bool canclimb = true;
    private Vector3 addVector = Vector3.zero;


    public bool running = false;
    public bool canrun = true;

    public AudioSource myAudioSource;
    public AudioSource myAudioSource2;
    public AudioClip walkloop;
    public AudioClip runloop;
    public AudioClip crouchloop;
    public AudioClip climbloop;
    public AudioClip jumpclip;
    public AudioClip landclip;
    Vector3 targetDirection = Vector3.zero;
    private bool canrun2 = true;
    public bool hideselectedweapon = false;
    Vector3 targetVelocity;
    public float falldamage;
    private float airTime;
    public float falltreshold = 2f;
    private bool prevGrounded;
    public Transform Deadplayer;
    public Transform weaponroot;

    Animator weaponanimator;
    public Transform head;
    Animator headanimator;
    public LayerMask mask;
    private float nextcheck;

    public Vector3 normalposition;
    public Vector3 aimposition;
    public Vector3 retractPos;

    public float aimFOV = 45f;
    public float normalFOV = 65f;
    public float weaponnormalFOV = 32f;
    public float weaponaimFOV = 20f;


    public AudioSource fireAudioSource1;
    public AudioSource fireAudioSource2;
    public AudioClip emptySound;
    public AudioClip fireSound;

    public AudioClip readySound;
    public AudioClip reloadSound;


    public int projectilecount = 1;
    public float spreadNormal = 0.08f;
    public float spreadAim = 0.02f;
    public float force = 500f;
    public float damage = 50f;
    public float range = 100f;
    public float smoothdamping = 2f;
    public float recoil = 5f;


    public AnimationClip fireAnim1;
    public AnimationClip fireAnim2;
    public float fireAnimSpeed = 1.1f;

    public AnimationClip reloadAnim;
    public AnimationClip readyAnim;

    public AnimationClip hideAnim;
    public GameObject shell;

    public Transform shellPos1;
    public Transform shellPos2;
    public float shellejectdelay = 0;
    public int ammo = 200;
    public int currentammo = 20;
    public int clipSize = 20;

    public Transform muzzle1;
    public Transform muzzlesmoke1;
    public Transform muzzle2;
    public Transform muzzlesmoke2;
    public Camera weaponcamera;
    public float runXrotation = 20f;
    public float runYrotation = 0f;
    public Vector3 runposition = Vector3.zero;
    private float nextField;
    private float weaponnextfield;


    private Vector3 wantedrotation;
    private bool canaim = true;

    private bool canfire = true;
    private bool retract = false;
    private bool isreloading = false;
    public Transform grenadethrower;
    public Transform meleeweapon;
    public Transform rayfirer;
    public Transform player;
    Animation myanimation;

    private void InitActor()
    {
        AcorLife = UnityEngine.Random.Range(80, 120);
        AcorSpeed = UnityEngine.Random.Range(18, 20);
        AcorJumpSpeed = UnityEngine.Random.Range(4, 8);
        AcorSwimSpeed = UnityEngine.Random.Range(8, 15);
        AcorHashCode = gameObject.GetHashCode();
        StartCoroutine(checkActor());
        reference = new GameObject().transform;
        weaponanimator = weaponroot.GetComponent<Animator>();
        headanimator = head.GetComponent<Animator>();
        painflashtexture.CrossFadeAlpha(0f, 0f, true);
        cameranextposition = camerahighposition;
    }

    private IEnumerator checkActor()
    {
        yield return new WaitForSeconds(5);
        InitActorWeapon();

    }
    private void InitActorWeapon()
    {
        clipSize = currentammo;

        nextField = normalFOV;
        weaponnextfield = weaponnormalFOV;
        myanimation.Stop();
        onstart();
    }
    void onstart()
    {
        myAudioSource.Stop();
        fireAudioSource1.Stop();

        fireAudioSource2.Stop();

        myanimation.Stop();
        if (!isreloading)
        {
            myAudioSource.clip = readySound;
            myAudioSource.loop = false;
            myAudioSource.volume = 1;
            myAudioSource.Play();

            myanimation.Play(readyAnim.name);
            canaim = true;
            canfire = true;
        }

    }

    public void UpdateByUpdateManager()
    {
        if (Input.GetButton("Reload"))
        {
            //Debug.Log("RELOAD");
            if (currentammo != clipSize && ammo > 0)
            {

                activeWeapon.Reload();
            }
        }

        float step = speed * Time.deltaTime;

        float newField = Mathf.Lerp(Camera.main.fieldOfView, nextField, Time.deltaTime * 2);
        float newfieldweapon = Mathf.Lerp(weaponcamera.fieldOfView, weaponnextfield, Time.deltaTime * 2);
        Camera.main.fieldOfView = newField;
        weaponcamera.fieldOfView = newfieldweapon;


        if (Input.GetButton("ThrowGrenade") && !myanimation.isPlaying && canfire)
        {
            if (Time.timeSinceLevelLoad > 1)
            {
                StartCoroutine(setThrowGrenade());
            }
        }
        if (Input.GetButton("Melee") && !myanimation.isPlaying && canfire)
        {
            StartCoroutine(setMelee());
        }
        if (retract)
        {
            canfire = false;
            canaim = false;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, retractPos, step);
            weaponnextfield = weaponnormalFOV;
            nextField = normalFOV;
        }
        else if (!retract)
        {
            canfire = false;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, runposition, step);
            wantedrotation = new Vector3(runXrotation, runYrotation, 0f);
            weaponnextfield = weaponnormalFOV;
            nextField = normalFOV;

        }
        else
        {
            canfire = true;
            wantedrotation = Vector3.zero;
            if (((Input.GetButton("Aim") || Input.GetAxis("Aim") > 0.1)) && canaim)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, aimposition, step);
                weaponnextfield = weaponaimFOV;
                nextField = aimFOV;
            }
            else
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);
                weaponnextfield = weaponnormalFOV;
                nextField = normalFOV;
            }

        }

        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(wantedrotation), step * 3f);

        if (currentammo == 0 || currentammo <= 0)
        {
            if (ammo <= 0)
            {
                canfire = false;
                if ((Input.GetButton("Fire1") || Input.GetAxis("Fire1") > 0.1) && !myAudioSource.isPlaying)
                {
                    myAudioSource.PlayOneShot(emptySound);
                }
            }
            else
            {
                activeWeapon.Reload();
            }
        }


        if (!isreloading && canfire)
        {
            if ((Input.GetButton("Fire1") || Input.GetAxis("Fire1") > 0.1))
            {
                activeWeapon.Fire(forward, false);
            }
        }


    }

    IEnumerator setMelee()
    {
        if (!meleeweapon.gameObject.activeInHierarchy)
        {

            retract = true;
            canfire = false;
            meleeweapon.gameObject.SetActive(true);
            meleeweapon.gameObject.BroadcastMessage("melee");
            Animation meleeAnimation = meleeweapon.GetComponent<Animation>();
            yield return new WaitForSeconds(meleeAnimation.clip.length);
            retract = false;
            canaim = true;
            canfire = true;
            meleeweapon.gameObject.SetActive(false);
        }
    }
    IEnumerator setThrowGrenade()
    {
        canfire = false;
        retract = true;
        grenadethrower.gameObject.SetActive(true);
        grenadethrower.gameObject.BroadcastMessage("throwstuff");
        Animation throwerAnimation = grenadethrower.GetComponent<Animation>();

        yield return new WaitForSeconds(throwerAnimation.clip.length);
        retract = false;
        canaim = true;
        canfire = true;
        grenadethrower.gameObject.SetActive(false);
    }
    #endregion
    public enum TargetType
    {
        Infantry,
        InfantryGroup,
        Unarmored,
        Armored,
        Air
    }

    private const float RUNPEARSPEED = 5f;

    private const float CHANGERUNBEFORELARE = 2f;

    private const float CLAMPTOWERAGLE = 50f;

    private const float OFFESBACKSEPD = 2f;

    private const float UPSTANDPADEING = 0.5f;

    private const float GAMEPROJECTGROUNDTHERS = 1.5f;

    private const float UNRESMEOPER = 0.3f;

    private const float ZIDONGRUNDROPRANGE = 4f;

    private const float ROGDALLDANCEINGSPRING = 700f;

    private const float RAGDOLL_DRIVE_DAMPING = 3f;

    private const float RAGDOLL_DEAD_DRIVE_SPRING = 50f;

    private const float RAGDOLL_DEAD_DRIVE_DAMPING = 1f;

    private const float BALANCE_GAIN_PER_SECOND = 10f;

    public const float CULL_ANIMATOR_DISTANCE = 300f;

    public const float LQ_CAMERA_DISTANCE = 10000f;

    public const float LQ_UPDATE_RATE = 0.2f;

    private const int AIM_ANIMATION_LAYER = 1;

    private const int CROUCH_ANIMATION_LAYER = 2;

    private const int RAGDOLL_ANIMATION_LAYER = 3;

    private const float SWIM_SPEED = 30f;

    public ActorController controller;

    public StartActor ragdoll;

    public Animator animator;

    public bool autoMoveActor;

    public Rigidbody hipRigidbody;

    public Rigidbody headRigidbody;

    [NonSerialized]
    public float deathTimestamp;

    [NonSerialized]
    public float health = 100f;

    [NonSerialized]
    public float balance = 100f;

    [NonSerialized]
    public bool dead;

    [NonSerialized]
    public bool fallenOver;

    private Collider[] hitboxColliders;

    private CharacterAction ik;

    private Vector2 movement;

    private Vector3 parentOffset = Vector3.zero;

    private Transform originalParent;

    private Action fallAction = new Action(3f);

    private Action stopFallAction = new Action(0.5f);

    private Action getupAction = new Action(2f);

    private Action highlightAction = new Action(4f);

    private Action hurtAction = new Action(0.6f);

    private bool wasCrouching;

    [NonSerialized]
    public bool inWater;

    [NonSerialized]
    public Weapon activeWeapon;

    [NonSerialized]
    public Weapon[] weapons = new Weapon[5];

    private int activeWeaponSlot;

    public int[] spareAmmo = new int[5];

    private bool FireStatus;

    private bool aiming;

    private Action aimingAction = new Action(0.2f);

    [NonSerialized]
    public bool hasAmmoBox;

    [NonSerialized]
    public int ammoBoxSlot;

    [NonSerialized]
    public bool hasMedipack;

    [NonSerialized]
    public int medipackSlot;

    [NonSerialized]
    public bool aiControlled;

    [NonSerialized]
    public bool needsResupply;

    [NonSerialized]
    public Seat seat;

    private Action cannotEnterVehicleAction = new Action(1f);

    [NonSerialized]
    public SkinnedMeshRenderer skinnedRenderer;

    [NonSerialized]
    public SkinnedMeshRenderer skinnedRendererRagdoll;

    private bool lqUpdate;

    private float lastUpdate;

    private float nextLqUpdateTime;

    [NonSerialized]
    public float lqUpdatePhase;

    private Rigidbody rigidbody;

    private static Vector3 removePitchEuler = new Vector3(0f, 1f, 1f);

    private static Vector3 removeY = new Vector3(1f, 0f, 1f);

    private Transform RedPosition;
    private Transform BluePosition;

    private readonly int moving = Animator.StringToHash("moving");
    private readonly int sprinting = Animator.StringToHash("sprinting");
    private readonly int crouched = Animator.StringToHash("crouched");
    private readonly int movementx = Animator.StringToHash("movement x");
    private readonly int movementy = Animator.StringToHash("movement y");
    private readonly int ragdolled = Animator.StringToHash("ragdolled");
    private readonly int deadA = Animator.StringToHash("dead");
    private readonly int hail = Animator.StringToHash("hail");
    private readonly int regroup = Animator.StringToHash("regroup");
    private readonly int move = Animator.StringToHash("move");
    private readonly int halt = Animator.StringToHash("halt");
    private readonly int seated = Animator.StringToHash("seated");
    private readonly int lean = Animator.StringToHash("lean");
    private readonly int hurt = Animator.StringToHash("hurt");
    private readonly int hurtx = Animator.StringToHash("hurt x");
    private readonly int reset = Animator.StringToHash("reset");
    private readonly int seatedtype = Animator.StringToHash("seated type");
    private readonly int falling = Animator.StringToHash("falling");
    private readonly int onBack = Animator.StringToHash("onBack");
    private readonly int swim = Animator.StringToHash("swim");
    private readonly int swimforward = Animator.StringToHash("swim forward");

    public float Score = 0;
    public string nickName = "";

    public virtual void Awake()
    {
        animator.enabled = false;
        ragdoll.ragdollObject.SetActive(value: true);
        skinnedRenderer = animator.GetComponentInChildren<SkinnedMeshRenderer>();
        skinnedRendererRagdoll = ragdoll.ragdollObject.GetComponentInChildren<SkinnedMeshRenderer>();
        ragdoll.ragdollObject.SetActive(value: false);
        ik = animator.GetComponent<CharacterAction>();
        parentOffset = base.transform.localPosition;
        originalParent = base.transform.parent;
        aiControlled = (controller.GetType() == typeof(RoleSystems));
        dead = true;
        //rigidbody = GetComponent<Rigidbody>();
        hitboxColliders = ragdoll.animatedObject.GetComponentsInChildren<Collider>();
    }

    public void Start()
    {
        ActorManager.Enrolment(this);
        lastUpdate = Time.time;
        if (ActorManager.instance.AllActorForRank.Length > 0)
        {
            ActorManager.instance.AllActorForRank[ActorManager.instance.actors.Count - 1] = this;
        }

    }

    public bool IsAiming()
    {
        return aiming;
    }

    public void SpawnAt(Vector3 position)
    {
        if (autoMoveActor)
        {
            base.transform.position = position;
        }
        SpawnLoadoutWeapons();
        if (seat != null)
        {
            seat.OccupantLeft();
            seat = null;
        }
        ik.turnBody = true;
        ik.weight = 1f;
        fallenOver = false;
        animator.enabled = true;
        animator.SetLayerWeight(3, 0f);
        animator.SetTrigger(reset);
        ragdoll.SetDrive(700f, 3f);
        balance = 100f;
        health = 100f;
        dead = false;
        ragdoll.InstantAnimate();
        controller.EnableInput();
        controller.SpawnAt(position);
        needsResupply = false;
        animator.SetBool(deadA, value: false);
        animator.SetBool(seated, value: false);
        if (!aiControlled)
        {
            IngameUi.instance.oShowMutib();
            IngameUi.instance.zSetLifeValue(Mathf.Max(0f, health));
        }
        ActorManager.SetAlive(this);
    }
    //初始化武器
    private void SpawnLoadoutWeapons()
    {

        hasAmmoBox = false;
        hasMedipack = false;
        WeaponManager.LoadoutSet loadout = controller.GetLoadout();
        SpawnWeapon(loadout.primary, 0);
        SpawnWeapon(loadout.secondary, 1);
        SpawnWeapon(loadout.gear1, 2);
        SpawnWeapon(loadout.gear2, 3);
        SpawnWeapon(loadout.gear3, 4);
        SwitchToFirstAvailableWeapon();

    }

    private void SwitchToFirstAvailableWeapon()
    {
        int num = 0;
        while (true)
        {
            if (num < 5)
            {
                if (HasWeaponInSlot(num))
                {
                    break;
                }
                num++;
                continue;
            }
            return;
        }
        activeWeaponSlot = num;
        LeatherCase(weapons[num]);
    }

    private void SpawnWeapon(WeaponManager.WeaponEntry entry, int slotNumber)
    {
        if (entry == null)
        {

            weapons[slotNumber] = null;
            if (!autoMoveActor)
            {
                IngameUi.instance.WeaponsImg[slotNumber].enabled = false;
            }
            return;
        }
        if (!autoMoveActor)
        {
            IngameUi.instance.WeaponsImg[slotNumber].enabled = true;
        }
        Weapon component;
        if (aiControlled && entry.TpPrefab)
        {
            component = UnityEngine.Object.Instantiate(entry.TpPrefab).GetComponent<Weapon>();
        }
        else
        {
            component = UnityEngine.Object.Instantiate(entry.prefab).GetComponent<Weapon>();
        }


        component.gameObject.name = entry.name;

        component.FindRenderers(aiControlled);
        component.Equip(this);
        component.transform.parent = controller.WeaponParent();

        if (aiControlled)
        {
            UnityEngine.Object.Destroy(component.animator);
            if (!component.tpOnly)
            {
                component.CullFpsObjects();
                component.thirdPersonTransform.localEulerAngles = new Vector3(180, 90f, 0f);
                component.thirdPersonTransform.localPosition = component.thirdPersonOffset;
                component.thirdPersonTransform.localScale = new Vector3(component.thirdPersonScale, component.thirdPersonScale, component.thirdPersonScale);
            }
            else
            {
                //component.thirdPersonTransform.localEulerAngles = new Vector3(-90, 0, 90);
                component.thirdPersonTransform.localEulerAngles = new Vector3(-72.454f, 30.448f, 134.283f);
                component.thirdPersonTransform.localPosition = component.thirdPersonOffset;
                component.thirdPersonTransform.localScale = new Vector3(component.thirdPersonScale, component.thirdPersonScale, component.thirdPersonScale);
            }



        }
        else
        {
            component.AssignFpAudioMix();
        }

        //component.transform.localPosition = Vector3.zero;
        //适配新武器的修改
        if (!aiControlled)
        {
            //if (component.lowPos)
            //{
            //    component.transform.localPosition = new Vector3(0, -0.137f, 0.18f);
            //}
            //else
            //{
            //component.transform.localPosition = new Vector3(0, -0.09f, 0.18f);
            component.transform.localPosition = new Vector3(0, -0.086f, 0.18f);
            //}
            component.transform.localRotation = Quaternion.identity;

        }
        else
        {
            if (!component.tpOnly)
            {
                component.transform.localPosition = Vector3.zero;
                component.transform.localRotation = Quaternion.identity;
            }

        }


        component.slot = slotNumber;
        component.ammo = component.configuration.ammo;
        weapons[slotNumber] = component;
        spareAmmo[slotNumber] = component.configuration.spareAmmo;
        if (!component.IsToggleable())
        {
            component.gameObject.SetActive(value: false);
            if (entry.name == "AMMO BAG")
            {
                hasAmmoBox = true;
                ammoBoxSlot = slotNumber;
            }
            if (entry.name == "MEDIPACK")
            {
                hasMedipack = true;
                medipackSlot = slotNumber;
            }
        }
        if (!autoMoveActor)
        {
            if (slotNumber < IngameUi.instance.WeaponsImg.Length)
            {
                IngameUi.instance.WeaponsImg[slotNumber].sprite = component.uiSprite;
                IngameUi.instance.WeaponsInfo[slotNumber].text = ((component.configuration.ammo == -1) ? string.Empty : component.configuration.ammo.ToString());
                int spare = component.GetSpareAmmo();
                if (component.GetSpareAmmo() >= 0)
                {
                    IngameUi.instance.WeaponsInfo[slotNumber].text += "/" + spare;
                    return;
                }
                switch (spare)
                {
                    case -1:
                        IngameUi.instance.WeaponsInfo[slotNumber].text += string.Empty;
                        break;
                    case -2:
                        IngameUi.instance.WeaponsInfo[slotNumber].text += "/∞";
                        break;
                }



            }

        }
    }

    public void FaceBog()
    {
        if (!dead && !ragdoll.IsRagdoll() && animator.enabled)
        {
            animator.SetTrigger(hail);
        }
    }

    public void FaceNoodle()
    {
        if (!dead && !ragdoll.IsRagdoll() && animator.enabled)
        {
            animator.SetTrigger(regroup);
        }
    }

    public void BookRun()
    {
        if (!dead && !ragdoll.IsRagdoll() && animator.enabled)
        {
            animator.SetTrigger(move);
        }
    }

    public void BookHalt()
    {
        if (!dead && !ragdoll.IsRagdoll() && animator.enabled)
        {
            animator.SetTrigger(halt);
        }
    }

    public void FixedUpdateMe()
    {
        if (!ragdoll.ragdollObject.activeInHierarchy)
        {
            return;
        }
        if (inWater)
        {
            hipRigidbody.AddForce(-Physics.gravity * 3f, ForceMode.Acceleration);
            hipRigidbody.drag = 8f;
            hipRigidbody.angularDrag = 8f;
            if (!dead)
            {
                headRigidbody.AddForce(-Physics.gravity * 3.5f, ForceMode.Acceleration);
                headRigidbody.angularDrag = 3f;
                headRigidbody.drag = 10f;
            }
        }
        else
        {
            hipRigidbody.drag = 0f;
            headRigidbody.drag = 0f;
            hipRigidbody.angularDrag = 0.05f;
            headRigidbody.angularDrag = 0.05f;
        }
        if (!dead && WaterLevel.InWater(CenterPosition()))
        {
            Vector3 a = controller.SwimInput() * 60f;
            hipRigidbody.AddForce(-Physics.gravity * 2f + a * 0.2f, ForceMode.Acceleration);
            headRigidbody.AddForce(-Physics.gravity * 2f + a * 0.8f, ForceMode.Acceleration);
        }
    }

    public void UpdateMe()
    {
        Vector3 position = CenterPosition();
        position.y += 0.5f;
        inWater = WaterLevel.InWater(position);
        if (dead)
        {
            return;
        }
        if (inWater && !fallenOver)
        {
            if (IsSeated())
            {
                LeftPew();
            }
            DropOver();
        }
        if (!hurtAction.Done() && !fallenOver && !dead)
        {
            float num = hurtAction.Ratio();
            if (num < 0.2f)
            {
                ik.weight = 0.5f - 2.5f * num + 0.5f;
            }
            else
            {
                ik.weight = 0.625f * (num - 0.2f) + 0.5f;
            }
        }
        if (!getupAction.Done())
        {
            UpdateGetup();
        }
        bool flag = lqUpdate;
        lqUpdate = QualityBelow();
        float dt = Time.time - lastUpdate;
        if (!lqUpdate || Time.time >= nextLqUpdateTime)
        {
            if (!fallenOver)
            {
                ModifyEmote();
                if (!IsSeated())
                {
                    ModifySpeed(dt);
                }
            }
            else
            {
                ReUpgradeDallSus();
            }
            nextLqUpdateTime = Mathf.Ceil(Time.time / LQ_UPDATE_RATE) * LQ_UPDATE_RATE + lqUpdatePhase;
            lastUpdate = Time.time;
        }
        if (activeWeapon != null)
        {
            if (lqUpdate && !flag)
            {
                activeWeapon.Hide();
            }
            else if (!lqUpdate && flag)
            {
                activeWeapon.Show();
            }
        }
        if (activeWeapon != null)
        {
            UpdateWeapon();
        }
        animator.SetBool(seated, IsSeated());
        balance = Mathf.Min(balance + Time.deltaTime * 10f, 100f);
    }

    private void UpdateWeapon()
    {
        bool flag = !fallenOver && controller.Fire() && (!IsSeated() || seat.CanUseWeapon() || seat.HasMountedWeapon());
        if (flag)
        {
            activeWeapon.Fire(controller.FacingDirection(), controller.UseMuzzleDirection());
        }
        else if (FireStatus)
        {
            activeWeapon.StopFire();
        }
        FireStatus = flag;
        aiming = ((controller.Aiming() || !aimingAction.TrueDone()) && !fallenOver && activeWeapon != null && activeWeapon.CanBeAimed());
        if (!activeWeapon.aiming && aiming)
        {
            activeWeapon.SetAiming(aiming: true);
            aimingAction.Start();
        }
        else if (activeWeapon.aiming && !aiming)
        {
            activeWeapon.SetAiming(aiming: false);
        }
        if (controller.Reload() && !activeWeapon.AmmoFull() && activeWeapon.HasSpareAmmo())
        {
            activeWeapon.Reload();
        }
    }

    private void UpdateGetup()
    {
        float num = getupAction.Ratio();
        animator.SetLayerWeight(3, Mathf.Clamp(2f * (1f - num), 0, 1));
        ik.weight = num;
        if (num > 0.5f)
        {
            controller.EnableInput();
        }
        if (getupAction.TrueDone())
        {
            fallenOver = false;
            controller.EndRagdoll();
            if (HasUnholsteredWeapon())
            {
                activeWeapon.gameObject.SetActive(value: true);
            }
        }
    }

    public float StandPass()
    {
        if (ragdoll.IsRagdoll())
        {
            return 0f;
        }
        return getupAction.Ratio();
    }

    private void ModifyEmote()
    {
        Vector3 vector = controller.FacingDirection();
        Quaternion to = Quaternion.Euler(Vector3.Scale(Quaternion.LookRotation(vector, Vector3.up).eulerAngles, removePitchEuler));
        if (!IsSeated())
        {
            Vector3 eulerAngles = base.transform.eulerAngles;
            float y = eulerAngles.y;
            Vector3 eulerAngles2 = to.eulerAngles;
            float f = Mathf.DeltaAngle(y, eulerAngles2.y);
            if (Mathf.Abs(f) > 50f)
            {
                base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, Mathf.Abs(f) - 50f);
                base.transform.eulerAngles.Scale(new Vector3(0f, 1f, 1f));
            }
        }
        Vector3 normalized = Vector3.Cross(Vector3.up, vector).normalized;

        if (ControllingVehicle())
        {
            ik.aimPoint = base.transform.position + vector * 100f;
        }
        else
        {
            ik.aimPoint = base.transform.position + vector * 100f + normalized * 55f;
        }
        animator.SetFloat(lean, controller.Lean());
    }

    private void ModifySpeed(float dt)
    {
        if (!controller.OnGround())
        {
            return;
        }
        Vector3 vector = controller.Velocity();
        Vector3 vector2 = Vector3.Scale(vector, removeY);
        //Vector3 vector2 = (float3)vector * (float3)removeY;
        bool flag = vector2.magnitude > 0.1f;
        animator.SetBool(moving, flag);
        animator.SetBool(sprinting, controller.IsSprinting());
        if (!IngameMenuUi.IsOpen())
        {
            bool flag2 = controller.Crouch();
            if (flag2 && !wasCrouching)
            {
                animator.SetLayerWeight(2, 1f);
                controller.StartCrouch();
            }
            else if (!flag2 && wasCrouching)
            {
                if (!controller.EndCrouch())
                {
                    flag2 = true;
                }
                else
                {
                    animator.SetLayerWeight(2, 0f);
                }
            }
            animator.SetBool(crouched, flag2);
            wasCrouching = flag2;
        }
        if (flag)
        {
            bool flag3 = Vector3.Dot(vector, base.transform.forward) < 0f;
            Vector3 vector3 = transform.worldToLocalMatrix.MultiplyVector(vector2);
            Vector2 b = new Vector2(vector3.x, vector3.z);
            Quaternion b2;
            if (!flag3)
            {
                b2 = Quaternion.LookRotation(vector2);
            }
            else
            {
                b.x = 0f - b.x;
                b2 = Quaternion.LookRotation(-vector2);
            }
            movement = Vector2.Lerp(movement, b, 5f * dt);

            animator.SetFloat(movementx, movement.x);
            animator.SetFloat(movementy, movement.y);
            //rigidbody.MoveRotation(Quaternion.Slerp(base.transform.rotation, b2, dt * 2f));
            transform.rotation = Quaternion.Slerp(base.transform.rotation, b2, dt * 2f);
        }
        //Vector3 vector4 = rigidbody.position + vector * dt;
        Vector3 vector4 = transform.position + vector * dt;

        if (controller.ProjectToGround() && Time.frameCount % 3 == 0)
        {
            Ray ray = new Ray(vector4 + Vector3.up * 1.5f, Vector3.down);
            //射线地面检测
            //if (Physics.SphereCast(ray, 0.3f, out RaycastHit hitInfo, 15f, 1))
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 15f, 1))
            {
                if (hitInfo.distance > 4f)
                {
                    DropOver();
                }
                else
                {
                    Vector3 point = hitInfo.point;
                    vector4.y = point.y;
                }
            }
            else
            {
                DropOver();
            }
        }

        if (autoMoveActor)
        {
            //rigidbody.position = vector4;
            transform.position = vector4;
        }
        ModifyOffset(dt);
    }

    private void ModifyOffset(float dt)
    {
        if (base.transform.parent != null)
        {
            base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, parentOffset, 2f * dt);

        }
    }

    private void ReUpgradeDallSus()
    {
        bool flag = Mathf.Abs(ragdoll.Velocity().magnitude) > 0.6f;
        animator.SetBool(falling, flag);
        animator.SetBool(onBack, ragdoll.OnBack());
        animator.SetBool(swim, inWater);
        animator.SetBool(swimforward, inWater && controller.SwimInput() != Vector3.zero);
        if (fallAction.TrueDone() && !flag && ragdoll.IsRagdoll() && !inWater)
        {
            if (stopFallAction.TrueDone())
            {
                GetUp();
            }
        }
        else
        {
            stopFallAction.Start();
        }
    }

    public void OverThrow(Vector3 force)
    {
        if (!ragdoll.IsRagdoll())
        {
            DropOver();
            RigidBodyPower(force);
        }
    }

    public void RigidBodyPower(Vector3 force)
    {
        ragdoll.MainRigidbody().AddForce(force, ForceMode.Impulse);
    }

    public void DropOver()
    {
        if (IsSeated())
        {
            LeftPew();
        }
        animator.SetBool(ragdolled, value: true);
        fallenOver = true;
        ragdoll.Ragdoll(controller.Velocity());

        controller.DisableInput();

        ik.weight = 0f;
        animator.SetLayerWeight(3, 1f);

        controller.StartRagdoll();

        fallAction.Start();
        getupAction.Stop();

        if (HasUnholsteredWeapon())
        {
            activeWeapon.SetAiming(aiming: false);
            activeWeapon.gameObject.SetActive(value: false);
        }
        //animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    }

    private void GetUp()
    {
        //if (autoMoveActor)
        //{
        //    animator.cullingMode = AnimatorCullingMode.CullCompletely;
        //}
        ragdoll.Animate();
        controller.GettingUp();
        getupAction.Start();
        animator.SetBool(ragdolled, value: false);
    }

    private void ImmediateUp()
    {
        ragdoll.InstantAnimate();
        controller.GettingUp();
        controller.EnableInput();
        animator.SetLayerWeight(3, 0f);
        ik.weight = 1f;
        fallenOver = false;
        controller.EndRagdoll();
        if (HasUnholsteredWeapon())
        {
            activeWeapon.gameObject.SetActive(value: true);
        }
    }

    public void LoneWolfModePlayerDead()
    {
        if (!aiControlled)
        {
            GameOverShow.Win(blue: false);
        }
    }

    public void LoneWolfModePlayerWin()
    {
        if (!ActorManager.instance.actors[0].aiControlled)
        {
            GameOverShow.Win(blue: true);
        }
        else
        {
            GameOverShow.Win(blue: false);
        }
    }

    private void Die(Vector3 impactForce)
    {
        if (GameManager.instance.LoneWolfMode)
        {
            ActorManager.Drop(this);
            if (ActorManager.instance.actors.Count == 1)
            {

                Invoke("LoneWolfModePlayerWin", 2);
            }
            Invoke("LoneWolfModePlayerDead",2);
        }

        Vector3 point = Position();
        animator.SetBool(deadA, value: true);
        for (int i = 0; i < 5; i++)
        {
            if (HasWeaponInSlot(i))
            {
                DropWeapon(weapons[i]);
            }
        }
        if (IsSeated())
        {
            LeftPew();
        }
        deathTimestamp = Time.time;
        fallAction.Stop();
        getupAction.Stop();
        controller.Die();
        controller.DisableInput();
        animator.enabled = false;
        ragdoll.SetDrive(50f, 1f);
        ragdoll.Ragdoll(controller.Velocity());
        RigidBodyPower(impactForce);
        dead = true;
        if (!aiControlled)
        {
            IngameUi.instance.iHideBtn();
        }
        ActorManager.PlaceDie(this);
        PathfindingManager.RegisterDeath(point);
        GameOverShow.AddScore((team == 1) ? 1 : 0, (team == 0) ? 1 : 0);
    }

    public virtual Vector3 Position()
    {
        if (ragdoll.IsRagdoll())
        {
            return ragdoll.Position();
        }
        return base.transform.position;
    }

    public virtual Vector3 CenterPosition()
    {
        if (ragdoll.IsRagdoll())
        {
            if (!RedPosition)
            {
                RedPosition = ragdoll.HumanBoneTransform(HumanBodyBones.Spine);
            }
            //return ragdoll.HumanBoneTransform(HumanBodyBones.Spine).position;
            return RedPosition.position;
        }
        if (!BluePosition)
        {
            BluePosition = ragdoll.HumanBoneTransformAnimated(HumanBodyBones.Spine);
        }
        //return ragdoll.HumanBoneTransformAnimated(HumanBodyBones.Spine).position;
        return BluePosition.position;
    }

    public virtual Vector3 Velocity()
    {
        if (IsSeated())
        {
            return seat.vehicle.Velocity();
        }
        if (ragdoll.IsRagdoll())
        {
            return ragdoll.Velocity();
        }
        return controller.Velocity();
    }

    public bool UpStraight()
    {
        return Velocity().sqrMagnitude < 0.1f;
    }

    public override Tuple<bool, bool> Damage(float healthDamage, float balanceDamage, bool piercing, Vector3 point, Vector3 direction, Vector3 impactForce)
    {
        bool flag = IsSeated() && seat.enclosed;
        if (!piercing && flag)
        {
            return Tuple.Create(false, false);
        }
        if (dead)
        {
            return Tuple.Create(false, false);
        }
        controller.ReceivedDamage(healthDamage, balanceDamage, point, direction, impactForce);
        health -= healthDamage;
        if (!flag)
        {
            balance = Mathf.Max(balance - balanceDamage, -100f);
        }
        int num = (int)Mathf.Ceil(healthDamage / 10f);
        for (int i = 0; i < num; i++)
        {
            Temsystem.CreateBloodDrop(point, Vector3.ClampMagnitude(impactForce * 0.1f, 5f), team);
        }
        if (!aiControlled)
        {
            IngameUi.instance.zSetLifeValue(Mathf.Max(0f, health));
            float intensity = Mathf.Clamp(0.3f + (1f - health / 100f), 0, 1);
            IngameUi.instance.ShowVignette(intensity, 6f);
        }
        if (health <= 0f)
        {
            Die(impactForce);
            return Tuple.Create(true, true);
        }
        else if (ragdoll.IsRagdoll())
        {
            RigidBodyPower(impactForce);
        }
        else if (balance < 0f)
        {
            OverThrow(Vector3.up * 100f + impactForce);
        }
        else
        {
            Hurt(UnityEngine.Random.Range(-2f, 2f));
        }

        return Tuple.Create(true, false);
    }

    public virtual void ForRecoil(Vector3 impulse)
    {
        controller.ApplyRecoil(impulse);
    }

    public Vector3 WeaponMuzzlePosition()
    {
        return activeWeapon.MuzzlePosition();
    }

    private void LeatherCase(Weapon weapon)
    {
        weapon.gameObject.SetActive(value: true);
        activeWeapon = weapon;
        activeWeapon.Unholster();
        controller.SwitchedToWeapon(activeWeapon);
        if (lqUpdate)
        {
            activeWeapon.Hide();
        }
        if (!aiControlled)
        {
            IngameUi.instance.aSeletGun(weapon);
            UpdateAmmoUi();
        }
    }

    private void LeatherGun()
    {
        if (!(activeWeapon == null))
        {
            activeWeapon.Holster();
            activeWeapon = null;
        }
    }

    private void DropWeapon(Weapon weapon)
    {
        weapon.transform.parent = null;
        weapon.Drop();
        if (weapon == activeWeapon)
        {
            activeWeapon = null;
        }
    }

    public bool HasWeaponInSlot(int i)
    {
        return weapons[i] != null;
    }

    public bool HasUnholsteredWeapon()
    {
        return activeWeapon != null;
    }

    public void Highlight()
    {
        highlightAction.Start();
    }

    public bool IsHighlighted()
    {
        return !highlightAction.TrueDone();
    }

    public bool IsSeated()
    {
        return seat != null;
    }

    public bool IsPassenger()
    {
        return IsSeated() && seat.type != 0 && seat.type != Seat.Type.Pilot;
    }

    public bool IsDriver()
    {
        return IsSeated() && (seat.type == Seat.Type.Driver || seat.type == Seat.Type.Pilot);
    }

    public bool PewStutas()
    {
        return !IsSeated() && cannotEnterVehicleAction.TrueDone();
    }

    public bool EnterSeat(Seat seat)
    {
        if (fallenOver)
        {
            ImmediateUp();
        }
        if (seat.vehicle.dead || seat.IsOccupied())
        {
            return false;
        }
        animator.SetInteger(seatedtype, (int)seat.animation);
        seat.SetOccupant(this);
        base.transform.parent = seat.transform;
        base.transform.localPosition = Vector3.zero;
        base.transform.localRotation = Quaternion.identity;
        controller.StartSeated(seat);
        this.seat = seat;
        animator.SetLayerWeight(2, 0f);
        if (!seat.CanUseWeapon())
        {
            animator.SetLayerWeight(1, 0f);
            LeatherGun();
            ik.turnBody = false;
        }
        if (seat.HasMountedWeapon())
        {
            LeatherCase(seat.weapon);
        }
        if (!autoMoveActor)
        {
            Collider[] array = hitboxColliders;

            foreach (Collider collider in array)
            {
                collider.gameObject.layer = 16;
            }
            if (seat.type == Seat.Type.Passenger)
            {
                foreach (var item in IngameUi.instance.CarMode)
                {
                    item.ui.SetActive(item.toggle);
                }
            }
            else if (seat.type == Seat.Type.Driver)
            {
                foreach (var item in IngameUi.instance.DriverMode)
                {
                    item.ui.SetActive(item.toggle);
                }
            }
            else if (seat.type == Seat.Type.Pilot)
            {
                foreach (var item in IngameUi.instance.TankMode)
                {
                    item.ui.SetActive(item.toggle);
                }
            }
            else if (seat.type == Seat.Type.Gunner)
            {
                foreach (var item in IngameUi.instance.GunnerMode)
                {
                    item.ui.SetActive(item.toggle);
                }
            }
        }
        else
        {
            gameObject.layer = 16;
        }





        return true;
    }

    public void LeftPew()
    {
        Vector3 vector = seat.transform.position + seat.transform.localToWorldMatrix.MultiplyVector(seat.exitOffset);
        Vector3 forward = seat.transform.forward;
        Vehicle vehicle = seat.vehicle;
        if (seat.HasMountedWeapon() && activeWeapon == seat.weapon)
        {
            LeatherGun();
        }
        seat.OccupantLeft();
        seat = null;
        Quaternion quaternion = Quaternion.LookRotation(Vector3.Scale(forward, removeY), Vector3.up);
        controller.EndSeated(vector, quaternion);
        base.transform.parent = originalParent;
        if (originalParent != null)
        {
            base.transform.localRotation = Quaternion.identity;
        }
        //rigidbody.position = vector;
        //rigidbody.rotation = quaternion;
        transform.position = vector;
        transform.rotation = quaternion;
        Physics.SyncTransforms();
        animator.SetLayerWeight(1, 1f);
        ik.turnBody = true;
        if (activeWeapon == null)
        {
            SwitchToFirstAvailableWeapon();
        }
        cannotEnterVehicleAction.Start();
        if (!autoMoveActor)
        {
            foreach (var item in IngameUi.instance.NormalMode)
            {
                item.ui.SetActive(item.toggle);
            }
        }
        StartCoroutine(ReactivateCollisionsWith(vehicle));
    }

    private IEnumerator ReactivateCollisionsWith(Vehicle vehicle)
    {
        yield return new WaitForSeconds(0.5f);
        bool reenteredThatVehicle = IsSeated() && seat.vehicle == vehicle;
        if (vehicle != null && !reenteredThatVehicle)
        {
            //Collider[] array = hitboxColliders;
            //foreach (Collider collider in array)
            //{
            //    collider.gameObject.layer = 8;
            //}
            gameObject.layer = 8;
        }
    }

    public void NextGun()
    {
        if (dead || fallenOver || (IsSeated() && !seat.CanUseWeapon()))
        {
            return;
        }
        int num = 1;
        int num2;
        while (true)
        {
            if (num < 4)
            {
                num2 = (activeWeaponSlot + num) % 5;
                if (weapons[num2] != null && !weapons[num2].IsToggleable())
                {
                    break;
                }
                num++;
                continue;
            }
            return;
        }
        SwitchWeapon(num2);
    }

    public void PreviousWeapon()
    {
        if (dead || fallenOver || (IsSeated() && !seat.CanUseWeapon()))
        {
            return;
        }
        int num = 1;
        int num2;
        while (true)
        {
            if (num < 4)
            {
                num2 = (activeWeaponSlot - num + 5) % 5;
                if (weapons[num2] != null)
                {
                    break;
                }
                num++;
                continue;
            }
            return;
        }
        SwitchWeapon(num2);
    }

    public void SwitchWeapon(int slot)
    {
        if (dead || fallenOver || (IsSeated() && !seat.CanUseWeapon()))
        {
            return;
        }
        Weapon weapon = weapons[slot];
        if (!(weapon != null))
        {
            return;
        }
        if (weapon.IsToggleable())
        {
            ((ToggleableItem)weapon).Toggle();
        }
        else if (weapon != activeWeapon)
        {
            if (activeWeapon != null)
            {
                LeatherGun();
            }
            activeWeaponSlot = slot;
            LeatherCase(weapon);
        }
    }

    public void SwitchSeat(int targetSeat)
    {
        if (IsSeated())
        {
            Vehicle vehicle = seat.vehicle;
            if (targetSeat >= 0 && targetSeat < vehicle.seats.Length && !vehicle.seats[targetSeat].IsOccupied())
            {
                LeftPew();
                EnterSeat(vehicle.seats[targetSeat]);
            }
        }
    }

    public void Hurt(float x)
    {
        if (!fallenOver && !dead)
        {
            animator.SetFloat(hurtx, x);
            animator.SetTrigger(hurt);
            hurtAction.Start();
        }
    }

    public void SetTeam(int team)
    {
        base.team = team;

        //Color color = ColorScheme.TeamColor(base.team);
        //skinnedRenderer.material.color = color;
        //skinnedRendererRagdoll.material.color = color;
        //skinnedRenderer.material.SetColor("_BaseColor", color);
        //skinnedRendererRagdoll.material.SetColor("_BaseColor", color);

        skinnedRenderer.material = ActorManager.instance.ActorMaterials[team];
        skinnedRendererRagdoll.material = ActorManager.instance.ActorMaterials[team];
    }

    private bool ControllingVehicle()
    {
        return IsSeated() && !seat.CanUseWeapon();
    }

    public int LeftSpareAmmoBox(Weapon weapon)
    {
        for (int i = 0; i < 5; i++)
        {
            if (weapon == weapons[i])
            {
                return spareAmmo[i];
            }
        }
        return 0;
    }
    //更新子弹UI
    public void UpdateAmmoUi()
    {
        //IngameUi.instance.SetAmmoText(activeWeapon.ammo, activeWeapon.GetSpareAmmo());
        IngameUi.instance.SetTheAmmoText(activeWeapon);
    }

    public void UpdateHealthUi()
    {
        IngameUi.instance.zSetLifeValue(health);
    }

    public int RemoveSpareAmmo(int howmuch, int slot)
    {
        if (slot != -1)
        {
            int num = spareAmmo[slot];
            int num2 = Mathf.Max(0, num - howmuch);
            spareAmmo[slot] = num2;
            if (!aiControlled)
            {
                UpdateAmmoUi();
            }
            return num - num2;
        }
        return 0;
    }

    public void AmmoChanged()
    {
        if (activeWeapon != null && activeWeapon.AllowsResupply() && LeftSpareAmmoBox(activeWeapon) <= activeWeapon.configuration.spareAmmo / 2)
        {
            needsResupply = true;
        }
        if (!aiControlled)
        {
            UpdateAmmoUi();
        }
    }

    public void AgainAmmo()
    {
        bool flag = false;
        needsResupply = false;
        for (int i = 0; i < 5; i++)
        {
            Weapon weapon = weapons[i];
            if (!(weapon == null) && weapon.AllowsResupply())
            {
                int num = spareAmmo[i];
                spareAmmo[i] = Mathf.Min(weapon.configuration.spareAmmo, spareAmmo[i] + weapon.configuration.resupplyNumber);
                flag |= (num != spareAmmo[i]);
                if (spareAmmo[i] <= weapon.configuration.spareAmmo / 2)
                {
                    needsResupply = true;
                }
            }
        }
        if (flag)
        {
            AmmoChanged();
            if (!aiControlled)
            {
                IngameUi.instance.Resupply();
            }
        }
    }

    public bool AddLife()
    {
        if (dead)
        {
            return false;
        }
        float num = health;
        health = Mathf.Min(health + 30f, 100f);
        if (!aiControlled && num != health)
        {
            UpdateHealthUi();
            IngameUi.instance.Ahealor();
        }
        return num != health;
    }

    public bool QualityBelow()
    {
        if (!aiControlled)
        {
            return false;
        }
        if (fallenOver)
        {
            //return !skinnedRendererRagdoll.isVisible || Vector3.Distance(base.transform.position, Camera.main.transform.position) > LQ_CAMERA_DISTANCE / Camera.main.fieldOfView;
            if (MainSystem.instance)
            {
                return skinnedRendererRagdoll.isVisible || (transform.position - MainSystem.instance.actor.transform.position).sqrMagnitude > 22500;
            }
            else
            {
                return skinnedRendererRagdoll.isVisible;
            }
        }

        //return !skinnedRenderer.isVisible || Vector3.Distance(base.transform.position, Camera.main.transform.position) > LQ_CAMERA_DISTANCE / Camera.main.fieldOfView;
        if (MainSystem.instance)
        {
            return skinnedRendererRagdoll.isVisible || (transform.position - MainSystem.instance.transform.position).sqrMagnitude > 22500;
        }
        else
        {
            return skinnedRendererRagdoll.isVisible;
        }
    }

    public virtual TargetType GetTargetType()
    {
        if (IsSeated())
        {
            return seat.vehicle.targetType;
        }
        if (controller.IsGroupedUp())
        {
            return TargetType.InfantryGroup;
        }
        return TargetType.Infantry;
    }
}
