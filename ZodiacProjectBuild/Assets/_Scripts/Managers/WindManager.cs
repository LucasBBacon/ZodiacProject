using System.Collections;
using UnityEngine;

public class WindManager : MonoBehaviour
{
    [Header("Wind")]
    [SerializeField] AreaEffector2D[] _windEffectors;
    [SerializeField] float _windSpeed;
    [SerializeField] float _snowParticleSpeed;

    [Header("Effects")]
    [SerializeField] GameObject _snowOBJ;
    [SerializeField] GameObject _filterOBJ;

    [Header("Timers")]
    [SerializeField] float _windActiveTime;
    [SerializeField] float _windCooldownTime;
    [SerializeField] float _windStartupTime;

    public bool SetWindActive;
    ParticleSystem _snowParticleSystem;
    SpriteRenderer _filterImage;
    float _alphaValue;

    private void Awake()
    {
        SetWindActive = true;
        StartCoroutine(WindActive());
    }

    private void Start()
    {
        _snowOBJ.transform.SetParent(Camera.main.gameObject.transform);
        _filterOBJ.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, _filterOBJ.transform.position.z);
        _filterOBJ.transform.SetParent(Camera.main.gameObject.transform);

        _snowParticleSystem = _snowOBJ.GetComponent<ParticleSystem>();
        _filterImage = _filterOBJ.GetComponent<SpriteRenderer>();
        _alphaValue = _filterImage.color.a;
        _filterImage.color = new Color(1.0f, 1.0f, 1.0f, 0f);

        _snowOBJ.SetActive(false);
        _filterOBJ.SetActive(false);
    }

    void SetWindSpeed(float windSpeed)
    {
        for (int i = 0; i < _windEffectors.Length; i++)
        {
            _windEffectors[i].forceMagnitude = windSpeed;
        }
    }

    IEnumerator WindActive()
    {
        while (SetWindActive)
        {
            float cooldownTimer = 0f;
            while (cooldownTimer <= _windCooldownTime)
            {
                cooldownTimer += Time.deltaTime;

                yield return null;
            }

            _filterOBJ.SetActive(true);
            _snowOBJ.SetActive(true);

            StartCoroutine(WindStartup(_windSpeed));

            float windTimer = 0f;
            while (windTimer <= _windActiveTime)
            {
                windTimer += Time.deltaTime;

                yield return null;
            }


            StartCoroutine(WindStartup(0f));
            
        }
        
    }
    
    IEnumerator WindStartup(float windSpeed)
    {
        float startupTimer = 0f;
        while (startupTimer <= _windStartupTime)
        {
            startupTimer += Time.deltaTime;

            if (windSpeed == 0)
            {
                var emission = _snowParticleSystem.emission;
                emission.rateOverTime = (int)Mathf.Lerp(_snowParticleSystem.particleCount, 0f, startupTimer / _windStartupTime);

                _filterImage.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Lerp(_filterImage.color.a, 0f, startupTimer / _windStartupTime));
            }
            else
            {
                var emission = _snowParticleSystem.emission;
                emission.rateOverTime = 200;

                _filterImage.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Lerp(_filterImage.color.a, _alphaValue, startupTimer / _windStartupTime));
            }

            SetWindSpeed(Mathf.Lerp(_windEffectors[0].forceMagnitude, windSpeed, startupTimer/_windStartupTime));


            yield return null;
        }

        if (windSpeed == 0f)
        {
            _filterOBJ.SetActive(false);
            _snowOBJ.SetActive(false);
        }

    }
}
