using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Administra el comportamiento de los proyectiles, tanto en su movimiento, como efecto sobre las entidades con las que impacta.

public class BulletProperties : MonoBehaviour {

	private float m_degree_local = 0f;                                  // Angulo local
	private float m_degree_parent = 0f;                                 // Angulo global del proyectil padre

	private float m_localSpeedMultiplier = 1;                           // Multiplicador de velocidad local, para efectos de movimiento.
    private float m_inheritedStrenght = 1;                              // Fuerza heredada del proyectil, afecta a su efectividad y daño.
    private int m_bouncesRemaining = 0;                                 // Rebotes restantes de este proyectil
    private float m_localTimescale = 1;                                 // Escala de tiempo local
	private bool m_canSplit = true;                       		        // Se le permite fragmentacion? (desactivado si el proyectil ya es un fragmento de otro proyectil)

	private float tempTrajectory_targetDegree = 0f;						// (TEMP) Valor objetivo auxiliar para trayectoria
	private float tempTrajectory_degreeMultiplier = 1f;					// (TEMP) Multiplicador auxiliar para trayectoria
	private float tempTrajectory_timer = 0f;							// (TEMP) Temporizador auxiliar para trayectoria
	private float tempFindNewTargetCooldown = 0f;						// (TEMP) Temporizador auxiliar para busqueda de nuevo objetivo

	private float m_lifetimeRemaining;                                  // Tiempo de vida restante del proyectil.
	private Transform m_target;                                         // Objetivo del proyectil, inicialmente, null, solo se gasta si el proyectil tiene algun tipo de tracking.
	private WeaponData m_weaponReferenced;                              // Almacena el arma de la que este proyectil sale.
    private EntityBase m_entityUser;									// Entidad que ha lanzado el ataque
    private UpgradeData.Trajectory m_currentTrajectory;                 // Modificador de trayectoria aplicado a este proyectil.
	private TrailRenderer m_TR;                                         // Referencia al componente TrailRenderer de este GameObject.

    private EntityBase m_lastEntityDamagedByParent = null;              // Ultima entidad rival con la que impacto el proyectil padre.
    private EntityBase m_lastEntityDamaged = null;                      // Ultima entidad rival con la que ha impactado.

	private const float SPEED_BASE = 25f;                               // (CONST) Velocidad base.
	private const float SPEED_TO_HELIX_MULTIPLIER = 100f;               // (CONST) Velocidad de giro del modificador Helix.
	private const float SPEED_TO_WAVE_MULTIPLIER = 100f;                // (CONST) Velocidad de giro del modificador Wave.
	private const float SPEED_TO_DLYSRG_MULTIPLIER = 2f;                // (CONST) Velocidad de deceleracion del modificador DelayedSurge.
	private const float SPEED_TO_TRACKING_MULTIPLIER = 100f;            // (CONST) Velocidad de giro del modificador Tracking.
	private const float SPEED_TO_COIL_MULTIPLIER = 50f;                 // (CONST) Velocidad de giro del modificador FixedTurn.
    private const float SPEED_TO_ARC_MULTIPLIER = 100f;                 // (CONST) Velocidad de giro del modificador Arc.

    // Inicializa los parametros del proyectil, similar a un constructor, se llama cuando alguna entidad intenta disparar.
	public void InitializeBullet(Vector3 _position, float _localRotation, float _parentRotation, WeaponData _weaponRef, EntityBase _entityUser, 
		float _inheritedStr = 1f, bool _canSplit = true, bool _hasSplit = false, EntityBase _lastEntityDamagedByParent = null)
	{
        ResetAuxiliarValues();
        m_weaponReferenced = _weaponRef;
        m_entityUser = _entityUser;
        m_bouncesRemaining = _weaponRef.weapon_bounces;
        m_localTimescale = m_entityUser.m_projectileSpeedScale * m_weaponReferenced.weapon_speedScale;

        gameObject.SetActive(true);
        _entityUser.m_referencesToThisEntityInUse++;

		m_target = null;
        m_lastEntityDamaged = null;
        m_lastEntityDamagedByParent = _lastEntityDamagedByParent;




        m_canSplit = _canSplit;
		m_localSpeedMultiplier = 1;
		transform.position = _position;
        tempFindNewTargetCooldown = 0;

		m_degree_local = _localRotation;
		m_degree_parent = _parentRotation;

        m_inheritedStrenght = _inheritedStr;
        transform.localScale = Vector3.one * _inheritedStr;

        if (m_TR == null)
            m_TR = this.GetComponent<TrailRenderer>();
        m_TR.Clear();
        VisualSetup();

        // Comprobamos si el proyectil padre se ha fragmentado ya
        if (_hasSplit)
        {
            m_currentTrajectory = _weaponRef.weapon_trajectoryAfterSplit;
            m_lifetimeRemaining = _weaponRef.weapon_lifetime_afterSplit;
        }
        else
        {
            m_currentTrajectory = _weaponRef.weapon_trajectory;
            m_lifetimeRemaining = _entityUser.m_attachToPlayerEntity == null && _weaponRef.weapon_split > 0 ? 0.75f : _weaponRef.weapon_lifetime_beforeSplit;
        }

        // Ajustes iniciales necesarios para algunos tipos de trayectoria
        switch (m_currentTrajectory)
        {
            case UpgradeData.Trajectory.helix:
                {
					m_localSpeedMultiplier = 1f + Mathf.Abs (m_degree_local * Mathf.PI * 0.001f);
                    tempTrajectory_targetDegree = m_degree_local * -1f;
                    break;
                }
            case UpgradeData.Trajectory.arc:
                {
                    tempTrajectory_targetDegree = m_degree_local * -1;
                    tempTrajectory_degreeMultiplier = 0.1f + Mathf.Abs(m_degree_local * 0.01f);
                    break;
                }
            case UpgradeData.Trajectory.coil:
                {
                    tempTrajectory_degreeMultiplier = m_degree_local > 0 ? -8 : 8;
                    m_degree_local -= 180;
                    break;
                }
			case UpgradeData.Trajectory.tracking:
				{
					m_degree_local += m_degree_parent;
					m_degree_parent = 0;
					break;
				}
        }
    }

    // UPDATE
    // ================================================================================================

    void FixedUpdate()
    {
        switch (m_currentTrajectory)
        {
            // Modificador: [Normal]
            // Movimiento normal simple, linea recta apartir del angulo desde el que se dispara
            case UpgradeData.Trajectory.normal:
                {
                    m_localSpeedMultiplier = Mathf.MoveTowards(m_localSpeedMultiplier, 1, GetLocalTimeScale() * 5f);
                    MoveForwardAndUpdateLifetime();
                    break;
                }
            // Comportamiento para el modificador [Helix] 
            // Los proyectiles viajaran en una oleada cruzandose en helice
            case UpgradeData.Trajectory.helix:
                {
				if (tempTrajectory_targetDegree != m_degree_local)
					m_degree_local = Mathf.MoveTowardsAngle (m_degree_local, tempTrajectory_targetDegree, GetLocalTimeScale() * SPEED_TO_HELIX_MULTIPLIER);
				else {
					tempTrajectory_targetDegree *= -1;
				}

                    MoveForwardAndUpdateLifetime();
                    break;
                }
            // Comportamiento para el modificador [Wave] 
            // Los proyectiles viajaran formando una oleada
            case UpgradeData.Trajectory.wave:
                {
                    m_degree_local = Mathf.MoveTowardsAngle(m_degree_local, 0, GetLocalTimeScale() * SPEED_TO_WAVE_MULTIPLIER);

                    MoveForwardAndUpdateLifetime();
                    break;
                }
            // Comportamiento para el modificador [DelayedSurge] 
            // Tras un breve retraso, los proyectiles se lanzaran a altas velocidades en linea recta
            case UpgradeData.Trajectory.delayedSurge:
                {
                    if (m_localSpeedMultiplier < 2)
                    {
                        m_localSpeedMultiplier = Mathf.MoveTowards(m_localSpeedMultiplier, 0, GetLocalTimeScale() * SPEED_TO_DLYSRG_MULTIPLIER);
                        if (m_localSpeedMultiplier == 0)
                        {
                            m_localSpeedMultiplier = 3f;
                            m_degree_local = 0;
                        }
                    }

                    MoveForwardAndUpdateLifetime();
                    break;
                }
            // Comportamiento para el modificador [DelayedTrack] 
            // Tras un breve retraso, los proyectiles se lanzaran sobre su objetivo a altas velocidades.
            case UpgradeData.Trajectory.delayedTrack:
                {
                    if (m_localSpeedMultiplier < 2)
                    {
                        m_localSpeedMultiplier = Mathf.MoveTowards(m_localSpeedMultiplier, 0, GetLocalTimeScale() * SPEED_TO_DLYSRG_MULTIPLIER);
                        if (m_localSpeedMultiplier == 0)
                        {
							FindNewTarget ();
                            m_localSpeedMultiplier = 3f;
							if (m_target != null) {
								m_degree_parent = 0;
								m_degree_local = -TrackTo(m_target);
							}
							
                        }
                    }

                    MoveForwardAndUpdateLifetime();
                    break;
                }
            // Comportamiento para el modificador [Tracking] 
            // Los proyectiles buscaran a los enemigos, girando lentamente hacia ellos.
            case UpgradeData.Trajectory.tracking:
                {
                    if (tempTrajectory_timer < 0.15f)
                    {
                        tempTrajectory_timer += GetLocalTimeScale();
                    }
                    else
                    {
						if (m_target == null) 
							FindNewTarget ();
                    	else
                    	{
                            tempTrajectory_targetDegree = -TrackTo(m_target);
                            m_degree_local = Mathf.MoveTowardsAngle(m_degree_local, tempTrajectory_targetDegree, GetLocalTimeScale() * SPEED_TO_TRACKING_MULTIPLIER);

                        }
                    }
                    MoveForwardAndUpdateLifetime();
                    break;
                }
            // Comportamiento para el modificador [Arc] 
            // Los proyectiles hacen trayectoria de arco.
            case UpgradeData.Trajectory.arc:
                {
                    m_degree_local = Mathf.MoveTowardsAngle(m_degree_local, tempTrajectory_targetDegree, GetLocalTimeScale() * SPEED_TO_ARC_MULTIPLIER);
                    MoveForwardAndUpdateLifetime();
                    break;
                }
            // Comportamiento para el modificador [Coil] [EXCLUSIVO DE SPLIT]
            // Gira en espiral, perdiendo velocidad de giro cada segundo.
            case UpgradeData.Trajectory.coil:
                {
                    tempTrajectory_degreeMultiplier = Mathf.MoveTowards(tempTrajectory_degreeMultiplier, 0, GetLocalTimeScale() * 7);
                    m_degree_local += tempTrajectory_degreeMultiplier * GetLocalTimeScale() * SPEED_TO_COIL_MULTIPLIER;
                    MoveForwardAndUpdateLifetime();
                    break;
                }
            // Comportamiento para [Deflected] NO ES UN MODIFICADOR ACCESIBLE
            // Al reflejar un proyectil, se cambia la trayectoria a esta.
            case UpgradeData.Trajectory.deflected:
                {
                    m_localSpeedMultiplier = Mathf.MoveTowards(m_localSpeedMultiplier, 1, GetLocalTimeScale() * 2f);
                    m_degree_local = Mathf.MoveTowardsAngle(m_degree_local, tempTrajectory_targetDegree, GetLocalTimeScale() * 800f);
                    MoveForwardAndUpdateLifetime();
                    break;
                }
        }
        //transform.position += Vector3.right * StageManager.currentInstance.scrollSpeed * Time.fixedDeltaTime;
		// Eliminamos el proyectil una vez lleva demasiado tiempo en pantalla (por si se atasca de alguna forma)
        if (m_lifetimeRemaining < 0)
        {
            RemoveBullet();
        }
    }

    #region Procesamiento de colisiones
    // PROCESAMIENTO DE COLISIONES
    // ================================================================================================

    private void OnTriggerEnter(Collider other)
    {
        // Si colisiona con una entidad que puede recibir daño, registramos colision.
        if (other.tag == "DamageableEntity")
        {
            m_lastEntityDamaged = other.GetComponent<EntityBase>();
            if (m_lastEntityDamaged == null)
                return;
            if (m_lastEntityDamagedByParent != null && m_lastEntityDamagedByParent == m_lastEntityDamaged)
                return;
            if (m_entityUser.m_isEnemy == m_lastEntityDamaged.m_isEnemy)
                return;
            if (m_weaponReferenced.weapon_split > 0)
            {
                Split();
                m_lifetimeRemaining = 0;
            }
            else if (m_bouncesRemaining > 0)
            {
                m_bouncesRemaining--;
                CancelTrajectoryIfNotCompatibleWithBounce();
                m_degree_local = Random.Range(1, 361);
                SpawnParticleBurst();
            }
            else
            {
                m_lifetimeRemaining = 0;
            }
            Explode();
            DamageEntity(m_lastEntityDamaged);
            // Si por el contrario colisiona con una superficie de reflejo, reflejamos el proyectil.
        }
        else if (other.tag == "Deflect")
        {
            Deflect(other.gameObject);
        }
    }

	// Para evitar que un proyectil pueda golpear mas de una vez a un objetivo, o que las balas que salen de fragmentar este dañen al mismo objetivo
	// que el proyectil padre, el proyectil ignorara las colisiones con el ultimo objetivo con el que impacto su proyectil padre, en el momento que
	// el proyectil abandone el collider de este, podra volver a dañarlo, ya que marcaremos como ultimo objetivo golpeado por el parent a Null.
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<EntityBase>() == m_lastEntityDamagedByParent)
            m_lastEntityDamagedByParent = null;
    }

    // Procesamos la colision con muros y calculamos la refraccion del proyectil con la superficie de impacto, lo hacemos de la siguiente forma.
    // 1 - Comprobamos la normal de colision, teniendo este vector, lo transformamos en un angulo euler con Mathf.Atan(hit.x/hit.y)*Mathf.Deg2Rad
    // 2 - Suponiendo que la refraccion de cualquier angulo sobre una superficie de normal 0,1(90ºeuler) es el angulo en negativo...
    // 3 - Le restamos al angulo el angulo de la normal, multiplicamos el resultado por -1, y sumamos el angulo de la normal otra vez. 
    // 4 - El resultado de la operacion es el angulo que tendra el proyectil despues de colisionar con la superficie.
    private void OnCollisionEnter(Collision collision)
    {
        if (!(collision.gameObject.tag == "Wall"))
            return;

        Explode();
        // Si la bala no tiene rebotes restantes, ni puede hacer split, no es necesario hacer ningun calculo.
        if (!(m_bouncesRemaining > 0) && !(m_weaponReferenced.weapon_split > 0))
        {
            m_lifetimeRemaining = 0;
            return;
        }
        Vector3 hit = collision.contacts[0].normal;

        SpawnParticleBurst();

        float normalAngle = Mathf.Atan(hit.x/hit.y) * Mathf.Rad2Deg;

        CancelTrajectoryIfNotCompatibleWithBounce();

        m_degree_local = (-(m_degree_local - normalAngle))+normalAngle;
        

        if (m_bouncesRemaining > 0)
        {
            m_bouncesRemaining--;
        }
        else
        {
            // Aunque no sea necesario calcular el angulo de rebote para split, lo calculamos de todas formas y simulamos el rebote, si un proyectil con split agota sus rebotes con un muro,
            // split se malgastaria, ya que los proyectiles impactarian instantaneamente, si justo antes de hacer split giramos el proyectil como si fuera a rebotar, el split se hara hacia
            // la direccion contraria al muro, asi no se desaprovechara la mejora.
            Split();
            m_lifetimeRemaining = 0;
        }
            
    }
    #endregion

    #region Funciones principales
    // FUNCIONES PRINCIPALES
    // ================================================================================================

    // Devuelve la escala de tiempo local del proyectil, multiplicada por FixedDeltaTime.
    float GetLocalTimeScale()
    {
        return m_localTimescale * Time.fixedDeltaTime;
    }
    void DamageEntity(EntityBase targetEntity)
	{
		if (targetEntity.m_health_current <= 0)
			return;

        // Separamos por partes cada efecto posible del arma

        // Calculo de daño directo.
        if (m_weaponReferenced.baseEffect_normal > 0)
        {
            float dmgDone = m_inheritedStrenght * m_entityUser.GetDamageMultiplier() * m_weaponReferenced.baseEffect_normal * m_weaponReferenced.weapon_effectivenessMultiplier;
            if (Random.Range(1, 101) < m_entityUser.GetCriticalChance())
            {
                dmgDone *= m_weaponReferenced.GetCritDamage() + m_entityUser.GetCriticialDamageMultiplier();
                targetEntity.DealDamage(dmgDone, WeaponData.DamageType.critical);
            }
            else
            {
                targetEntity.DealDamage(dmgDone, WeaponData.DamageType.normal);
            }
        }
		

		// Calculo de daño de fuego.
		if (m_weaponReferenced.baseEffect_photon > 0) {
			m_lastEntityDamaged.DealFireDamage (m_inheritedStrenght * m_entityUser.GetDamageMultiplier() * m_weaponReferenced.baseEffect_photon * m_weaponReferenced.weapon_effectivenessMultiplier);
		}

		// Calculo de robo de energia.
		if (m_weaponReferenced.baseEffect_energySteal > 0) {
			if (m_entityUser != null)
				m_entityUser.HealWithEnergySteal (m_inheritedStrenght * m_weaponReferenced.baseEffect_energySteal * m_weaponReferenced.weapon_effectivenessMultiplier);
		}
        // Calculo de daño nuclear.
        if (m_weaponReferenced.baseEffect_nuclear > 0)
        {
            m_lastEntityDamaged.DealNuclearDamage(m_inheritedStrenght * m_weaponReferenced.baseEffect_nuclear * m_weaponReferenced.weapon_effectivenessMultiplier * m_entityUser.GetDamageMultiplier());
        }
	}
    // Elimina la bala de la partida moviendola al pool de objetos inactivos.
    // NOTA: La bala no se destruye, se desactiva para que pueda reciclarse, cuidado con hacer cambios permanentes a este objeto.
    void RemoveBullet()
	{
        SpawnParticleBurst();
		StopAllCoroutines ();
		m_entityUser.m_referencesToThisEntityInUse--;
        gameObject.SetActive(false);
    }

	// Busca un nuevo objetivo, la lista de objetivos posibles esta en StageData, si esta entidad es enemiga, buscaremos un objetivo "bueno", si la
	// entidad es amistosa, entonces buscaremos a un objetivo "malo", hay un temporizador de cooldown de busqueda de nuevo objetivo, por razones de
	// optimización, solo se podra buscar un objetivo nuevo cada 0.25s por poryectil.
	void FindNewTarget()
	{
		if (tempFindNewTargetCooldown > 0)
			return;
		tempFindNewTargetCooldown = 0.25f;
        EntityBase tar;
		if (m_entityUser.m_isEnemy) {
			tar = StageManager.currentInstance.GetRandomAlly ();
		} else {
			tar = StageManager.currentInstance.GetRandomEnemy ();
		}
        if (tar != null)
            m_target = tar.transform;
	}

    // Cambia la parte visual (particulas, trails...) dependiendo del tipo de arma
    void VisualSetup()
    {
        switch (m_weaponReferenced.weapon_type)
        {
            case WeaponData.WeaponType.bullet:
                {
                    m_TR.startWidth = 0.5f;
                    m_TR.endWidth = 0.2f;
                    m_TR.time = 0.17f;
                    break;
                }
            case WeaponData.WeaponType.laser:
                {
                    m_TR.startWidth = 0.6f;
                    m_TR.endWidth = 0.6f;
                    m_TR.time = 0.175f;
                    break;
                }
            case WeaponData.WeaponType.missile:
                {
                    m_TR.startWidth = 1f;
                    m_TR.endWidth = 0.5f;
                    m_TR.time = 0.1f;
                    break;
                }
            case WeaponData.WeaponType.photon:
                {
                    m_TR.startWidth = 0.8f;
                    m_TR.endWidth = 0.1f;
                    m_TR.time = 0.16f;
                    break;
                }
            case WeaponData.WeaponType.plasma:
                {
                    m_TR.startWidth = 0.9f;
                    m_TR.endWidth = 0.1f;
                    m_TR.time = 0.18f;
                    break;
                }

            default:
                {
                    m_TR.startWidth = 0.25f;
                    m_TR.endWidth = 0f;
                    m_TR.time = 0.25f;
                    break;
                }
        }
        m_TR.startWidth *= m_inheritedStrenght;
        m_TR.endWidth *= m_inheritedStrenght;
        m_TR.time /= m_localTimescale;
        m_TR.startColor = m_TR.endColor = m_entityUser.m_isEnemy ? EntityBase.ENEMY_COLOR_TINT : EntityBase.ALLY_COLOR_TINT;
    }

	// Reinicia los valores auxiliares del calculo de trayectoria, esta funcion debe llamarse al empezar, o al cambiar el tipo de trayectoria
	// del proyectil
    void ResetAuxiliarValues()
    {
        tempTrajectory_targetDegree = 0;
        tempTrajectory_timer = 0;
        tempTrajectory_degreeMultiplier = 1;
		tempFindNewTargetCooldown = 0f;
    }
    // Mueve el proyectil hacia delante, el angulo de movimiento es igual a la suma del angulo local mas el angulo global del proyectil padre
	void MoveForwardAndUpdateLifetime()
	{
		transform.rotation = Quaternion.Euler (m_degree_local + m_degree_parent, 90, 0);
		transform.Translate (Vector3.forward * GetLocalTimeScale() * m_localSpeedMultiplier * SPEED_BASE);
        m_lifetimeRemaining -= GetLocalTimeScale() * m_localSpeedMultiplier;
		tempFindNewTargetCooldown -= Time.fixedDeltaTime * m_localSpeedMultiplier;
    }
    // Algunas trayectorias no son compatibles con rebote, asi que al rebotar, su trayectoria pasa a ser "normal"
    void CancelTrajectoryIfNotCompatibleWithBounce()
    {
        if (m_currentTrajectory == UpgradeData.Trajectory.wave || m_currentTrajectory == UpgradeData.Trajectory.helix 
            || m_currentTrajectory == UpgradeData.Trajectory.arc || m_currentTrajectory == UpgradeData.Trajectory.coil)
        {
            m_currentTrajectory = UpgradeData.Trajectory.normal;
            m_degree_local += m_degree_parent;
            m_degree_parent = 0;
        }
    }

    // Fragmenta el proyectil en otros proyectiles mas pequeños. Funciona similar al disparo de EntityShoot
    void Split()
    {
        if (!m_canSplit || m_weaponReferenced.weapon_split <= 0)
            return;
        float tempSplitAngle = 45;
        
        GameObject lastSplitCreated;
        for (int i = 0; i < m_weaponReferenced.weapon_split; i++)
        {
            lastSplitCreated = ObjectPool.currentInstance.GetFromPool_bullet();
			lastSplitCreated.GetComponent<BulletProperties> ().InitializeBullet (transform.position, tempSplitAngle, m_degree_parent + m_degree_local,
				m_weaponReferenced, m_entityUser, m_inheritedStrenght * 0.33f, false, true, m_lastEntityDamaged);
			tempSplitAngle -= 90f/(m_weaponReferenced.weapon_split-1);
        }
    }

    // Hace estallar el proyectil, en el caso que el arma que lo lanze tenga algo de componente explosivo.
    void Explode()
    {
        if (m_weaponReferenced.baseEffect_explosive > 0)
        {
            ObjectPool.currentInstance.GetFromPool_explosion().SetAs(transform.position, m_inheritedStrenght, m_entityUser, m_weaponReferenced);
        }
    }
    #endregion

    #region Funciones publicas
    // FUNCIONES PUBLICAS
    // ================================================================================================

    // Desvia el proyectil, parando todas las co-rutinas de movimiento y forzando la co-rutina de desvio
    public void Deflect(GameObject deflecter)
    {
        ResetAuxiliarValues();
        m_degree_local += m_degree_parent;
        m_degree_parent = 0;
        m_target = deflecter.transform;
		m_currentTrajectory = UpgradeData.Trajectory.deflected;
        tempTrajectory_targetDegree = -TrackTo(deflecter.transform) + 180;
    }
    // Crea una explosion de particulas al impactar con un enemigo o muro
    public void SpawnParticleBurst()
    {
        GameObject particleBurst = ObjectPool.currentInstance.GetFromPool_partBurst();
        particleBurst.transform.position = transform.position;
        particleBurst.GetComponent<ParticleBurstBehaviour>().SetBurstColor(m_entityUser.m_isEnemy? EntityBase.ENEMY_COLOR_TINT : EntityBase.ALLY_COLOR_TINT);
        particleBurst.gameObject.SetActive(true);
    }
    #endregion

    #region Funciones auxiliares
    // FUNCIONES AUXILIARES
    // ================================================================================================

    // Funcion auxiliar que devuelve el angulo hacia el que debe mirar este objeto para estar enfocado a un transform objetivo
    // NOTA: por razones de diferencia de ejes, el angulo que esta funcion devuelve debe multiplicarse por -1 para que funcione correctamente.
    float TrackTo(Transform targTransform)
    {
        if (targTransform == null)
            return transform.rotation.eulerAngles.x;

        float difX = targTransform.position.x - transform.position.x;
        float difY = targTransform.position.y - transform.position.y;
        if (difX == 0)
        {
            if (difY > 0) { return 90; }
            else { return 270; }
        }

        if (difX > 0) { return Mathf.Atan(difY / difX) * Mathf.Rad2Deg; }
        else { return (Mathf.Atan(difY / difX) * Mathf.Rad2Deg) + 180; }

    }
    #endregion
}
