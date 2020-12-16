using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;

//Dartti
public class Bullets : MonoBehaviour
{
    public weapon weapon_type;
    public float damage;
    public float speed;
    public float lifeDuration;
    public float thrust_force;
    public float knock_back;
    public int shells;
    public float blastRadius = 0f;

    //Reaycast spesific
    public float range;
    public float spread = 0;
    private bool raycastweapon = false;
    public bool lose_damage_over_distence = false;
    [SerializeField] LayerMask mask;
    List<Vector3> list_debug_rays; //DEBUG
    public GameObject player;
    public DecalHandler decal_handler;
    public GameObject trailParticle;
    public GameObject lightningParticle;
    public GameObject lightningSparkParticle;
    public Transform gunPosition;
    public GameObject[] hitEffects;
    public Material lightning_material;
    public Material granade_material;
    public Material rocket_material;
    //public MeshRenderer rocket_model;
    //public MeshRenderer granade_model;
    public Light rocket_light;
    public Light lightning_gunLight;
    public Mesh rocket_mesh;
    public Mesh granade_mesh;
    public GameObject obj_explosion;
    public GameObject explosion_small;
    public GameObject explosion_medium;
    public GameObject explosion_large;
    private GameObject particle_explosion;
    System.Random rnd = new System.Random();
    public GameObject particle_rocket_smoke;

    FMOD.Studio.EventInstance ProjectileSoundInstance;
    FMOD.Studio.EventInstance ProjectileExplosionSoundInstance;


    void Start()
    {
        var collider = GetComponent<CapsuleCollider>();
        var rigidbody = GetComponent<Rigidbody>();
        var transform = GetComponent<Transform>();
        var model = GetComponent<MeshRenderer>();
        var model_meshfilter = GetComponent<MeshFilter>().sharedMesh;
        var trailRender = GetComponent<TrailRenderer>();
        list_debug_rays = new List<Vector3>(); //DEBUG
        var weight = player.GetComponent<Rigidbody>().mass;

        if (blastRadius >= 4f)
        {
            particle_explosion = explosion_large;
        }
        else if (blastRadius > 2f)
        {
            particle_explosion = explosion_medium;
        }
        else if (blastRadius > 0f)
        {
            particle_explosion = explosion_small;
        }

        /*
        hitEffects = new GameObject[4];
        hitEffects[0] = Resources.Load<GameObject>("Assets/Prefabs/WallHitEffect1.prefab");
        hitEffects[1] = Resources.Load<GameObject>("Assets/Prefabs/WallHitEffect2.prefab");
        hitEffects[2] = Resources.Load<GameObject>("Assets/Prefabs/WallHitEffect3.prefab");
        hitEffects[3] = Resources.Load<GameObject>("Assets/Prefabs/WallHitEffect4.prefab");
        */

        //Special behaviors
        switch (weapon_type)
        {
            case weapon.revolver:
                raycastweapon = true;
                RayCastShot();
                break;

            case weapon.shotgun:
                raycastweapon = true;
                for (var i = 0; i < shells; i++)
                {
                    RayCastShot();
                }

                spread = 0;
                RayCastShot();

                player.GetComponent<Rigidbody>().AddForce(transform.up * -thrust_force * weight, ForceMode.Impulse);
                break;

            case weapon.chaingun:
                raycastweapon = true;
                RayCastShot();
                player.GetComponent<Rigidbody>().AddForce(transform.up * -thrust_force * weight, ForceMode.Impulse);
                break;

            case weapon.grenadeLauncher:
                rigidbody.AddForce(transform.up * thrust_force, ForceMode.Impulse);
                player.GetComponent<Rigidbody>().AddForce(transform.up * -thrust_force * weight / 2, ForceMode.Impulse);
                collider.radius = 0.2f;
                collider.height = 0.6f;
                transform.localScale += new Vector3(-0.8f, -0.8f, -0.8f);
                model.enabled = true;
                GetComponent<MeshFilter>().mesh = granade_mesh;
                GetComponent<MeshRenderer>().material = granade_material;
                break;

            case weapon.lightningGun:
                transform.localScale = new Vector3(0.2f, range, 0.2f);
                transform.position += transform.up * range - transform.up / 4;
                player.GetComponent<Rigidbody>().AddForce(transform.up * -thrust_force * weight, ForceMode.Impulse);
                /*
                model.enabled = true;
                
                var lightningTrail = Instantiate(lightningParticle, gunPosition.position + (transform.up * range), Quaternion.LookRotation(transform.up));
                Destroy(lightningTrail, lifeDuration);
                */
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                rigidbody.isKinematic = true;

                var lightningSprkTrail = Instantiate(lightningSparkParticle, gunPosition.position, Quaternion.LookRotation(transform.up)); ;
                Destroy(lightningSprkTrail, lifeDuration);

                GetComponent<MeshRenderer>().material = lightning_material;
                rocket_light.enabled = true;
                lightning_gunLight.enabled = true;

                Debug.DrawRay(gunPosition.position, transform.up * range * 2, Color.green, 5f);
                RaycastHit[] railWallHits = Physics.RaycastAll(gunPosition.position, transform.up, range * 2, LayerMask.GetMask("Geometry"));
                decal_handler.WallLightningHit(railWallHits, gunPosition.position, transform.up, range * 2);

                RaycastHit[] railEnemyHits = Physics.RaycastAll(gunPosition.position, transform.up, range * 2, LayerMask.GetMask("Enemy"));
                decal_handler.EnemyLightningHit(railEnemyHits);
                break;

            case weapon.rocketLauncher:
                transform.localScale += new Vector3(-0.8f, 0.5f, -0.8f);
                player.GetComponent<Rigidbody>().AddForce(transform.up * -thrust_force * weight, ForceMode.Impulse);
                model.enabled = true;
                collider.radius = 0.2f;
                collider.height = 0.6f;
                GetComponent<MeshFilter>().mesh = rocket_mesh;
                GetComponent<MeshRenderer>().material = rocket_material;
                trailRender.enabled = false;
                rocket_light.enabled = true;
                particle_rocket_smoke.SetActive(true);// = true;
                break;

            case weapon.rocketFist:
                //transform.position += transform.up;
                transform.localScale *= 4;
                model.enabled = true;
                break;

            case weapon.meleeFist:
                transform.localScale *= 7;
                model.enabled = false;
                trailRender.enabled = false;
                break;
            case weapon.meleeBlade:
                transform.localScale *= 7;
                model.enabled = false;
                break;
        }

        ProjectileSound(weapon_type);

    }

    void Update()
    {
        //DEBUG
        if (raycastweapon)
        {
            for (var i = 0; i < list_debug_rays.Count; i++)
            {
                var variance = 1f;
                var v3Offset = transform.up * Random.Range(0f, variance);
                v3Offset = Quaternion.AngleAxis(Random.Range(0f, 360f), transform.forward) * v3Offset;
                Debug.DrawRay(transform.position, list_debug_rays[i] * range, Color.green);
            }
        }

        transform.position += transform.up * speed * Time.deltaTime;
        lifeDuration -= Time.deltaTime;

        if (lifeDuration <= 0)
        {
            if (weapon_type == weapon.grenadeLauncher)
            {
                Explosion.ObstructedExplosionDamage(transform.position, blastRadius, (int)damage, 4, 1f, true);
                //var explosion = Instantiate(particle_explosion, gameObject.transform.position, Quaternion.identity);
                //Destroy(explosion, 1f);
                decal_handler.ExplosionWallHit(transform);
            }
            ProjectileExplosionSound(weapon_type);
            Destroy(gameObject);
        }

        if (weapon_type == weapon.lightningGun)
        {
            transform.Rotate(0, 10, 0);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        //var target = col.GetComponent<Target>();
        var target = col.transform.GetComponent<IHealth>();
        if (target != null)
        {
            if (weapon_type == weapon.grenadeLauncher || weapon_type == weapon.rocketLauncher)
            {
                Explosion.ObstructedExplosionDamage(transform.position, blastRadius, (int)damage, knock_back, 1, true);
                decal_handler.RocketEnemyHit(col.GetContact(0));

                //KostinKoodi
                ProjectileExplosionSound(weapon_type);

                Destroy(gameObject);
            }

            if (weapon_type == weapon.lightningGun)
            {
                target.TakeDamage((int)damage, gunPosition.position);
                Explosion.ObstructedExplosionDamage(col.transform.position, blastRadius, 0, knock_back, 1, false); //just knockback
            }

            //MELEE
            if (weapon_type == weapon.meleeFist)
            {
                Explosion.ObstructedExplosionDamage(col.transform.position, blastRadius, 0, knock_back, 1, false);
                target.TakeDamage((int)damage, gunPosition.position);
            }
        }
        else
        {
            if (weapon_type == weapon.lightningGun)
            {
                //decal_handler.WallLightningHit(col.GetContact(0));
            }
            else if (weapon_type == weapon.rocketLauncher)
            {
                decal_handler.RocketWallHit(col.GetContact(0));
            }
        }


        if (weapon_type == weapon.rocketLauncher)
        {
            //Explosion.UnobstructedExplosionDamage(transform.position, blastRadius, (int)damage, knock_back, 1, true);

            RaycastHit hit;
            if (Physics.Raycast(transform.position - transform.up / 6, -new Vector3(0, 1, 0), out hit, 1, mask))
            {
                Explosion.ObstructedExplosionDamage(transform.position, blastRadius, (int)damage, knock_back, 1, true);
                ProjectileExplosionSound(weapon_type);
            }
            else
            {
                Explosion.ObstructedExplosionDamage(transform.position - new Vector3(0, 1, 0), blastRadius, (int)damage, knock_back, 1, true);
                ProjectileExplosionSound(weapon_type);
            }

            //KostinKoodi
            ProjectileExplosionSound(weapon_type);

            Destroy(gameObject);
        }
        speed = 0;

    }

    void KnockBack(Transform col)
    {
        if (knock_back > 0)
        {
            col.transform.GetComponent<EnemyAi>().StunEnemy(1, col.position, knock_back);
            //var rigidbody_other = col.GetComponent<Rigidbody>();
            //rigidbody_other.AddForce(transform.up * knock_back, ForceMode.Impulse);
        }
    }

    void RayCastShot()
    {
        RaycastHit hit;
        int layerMask = 1 << 9;
        //var v3Offset = transform.up * Random.Range(0f, spread);
        //v3Offset = Quaternion.AngleAxis(Random.Range(0f, 360f), transform.forward) * v3Offset;
        //list_debug_rays.Add(transform.up + v3Offset);

        Vector3 randomPos = (Random.insideUnitSphere * spread);// + transform.position;
        list_debug_rays.Add(transform.up + randomPos);

        var parTrail = Instantiate(trailParticle, gunPosition.position + gunPosition.forward + gunPosition.right / 2 - gunPosition.up / 6/*(transform.position+hit.point)/2*/, Quaternion.LookRotation(transform.up + randomPos));
        Destroy(parTrail, 1f);

        if (Physics.Raycast(transform.position, transform.up + randomPos, out hit, range, mask))
        {
            var target = hit.transform.GetComponent<IHealth>();//hit.transform.GetComponent<Target>();
            if (target != null)
            {
                decal_handler.BulletEnemyHit(hit);
                damage = (lose_damage_over_distence) ? damage * (1 - (hit.distance / range)) : damage;
                hit.transform.GetComponent<IHealth>().TakeDamage((int)damage, gunPosition.position);
                knock_back = (lose_damage_over_distence) ? knock_back * (1 - (hit.distance / range)) : knock_back;
                //KnockBack(hit.transform);

                switch (weapon_type)
                {
                    case weapon.revolver: Explosion.ObstructedExplosionDamage(hit.point, 1, (int)(damage / 2), 30); break;

                    case weapon.shotgun:; break;
                    case weapon.chaingun:; break;
                        //case weapon.lightningGun:; break;
                }

            }
            else
            {
                Vector3 rayDir = transform.up + randomPos;
                decal_handler.BulletWallHit(hit, rayDir);
            }
        }
    }

    private bool ProjectileSound(weapon weapon_type)
    {
        string sound_path = "";

        switch (weapon_type)
        {
            case weapon.grenadeLauncher: sound_path = "event:/SX/Weapons/Sx_projectile"; break;
            case weapon.rocketLauncher: sound_path = "event:/SX/Weapons/Rocket Launcher/Sx_rocketlauncher_projectile"; break;
        }

        if (sound_path != "")
        {
            return false;
        }

        ProjectileSoundInstance = FMODUnity.RuntimeManager.CreateInstance(sound_path);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(ProjectileSoundInstance, transform, GetComponent<Rigidbody>());
        ProjectileSoundInstance.start();
        return true;
    }


    void ProjectileExplosionSound(weapon weapon_type)
    {
        string sound_path = "";

        switch (weapon_type)
        {

            case weapon.grenadeLauncher: sound_path = "event:/SX/Weapons/Hand Mortar/Sx_handMortar_explotion"; break;
            case weapon.rocketLauncher: sound_path = "event:/SX/Weapons/Rocket Launcher/Sx_rocketlauncher_explosion"; break;
        }

        if (sound_path != "")
        {
            ProjectileExplosionSoundInstance = FMODUnity.RuntimeManager.CreateInstance(sound_path);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(ProjectileExplosionSoundInstance, transform, GetComponent<Rigidbody>());
            ProjectileExplosionSoundInstance.start();
        }

        ProjectileSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

}
