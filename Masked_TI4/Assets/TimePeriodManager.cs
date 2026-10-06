using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class TimePeriodManager : MonoBehaviour
{
    [Header("Componente de Volume")]
    [SerializeField] private Volume _globalVolume;

    [Header("Perfis de Cada Época")]
    [SerializeField] private VolumeProfile _passadoProfile;
    [SerializeField] private VolumeProfile _futuroProfile;

    [Header("Configuração de Transição")]
    [SerializeField] private float _transitionDuration = 0.5f;

    public enum TimePeriod { Passado, Futuro }
    private TimePeriod _currentPeriod = TimePeriod.Passado;
    private bool _isTransitioning = false;

    public TimePeriod CurrentPeriod => _currentPeriod;

    private void Start()
    {
        SetPeriodImmediate(TimePeriod.Passado);
    }

    private void Update()
    {
        // Exemplo: Pressionar a tecla 'T' para alternar de época
        if (Input.GetKeyDown(KeyCode.T) && !_isTransitioning)
        {
            ToggleTimePeriod();
        }
    }

    public void ToggleTimePeriod()
    {
        TimePeriod nextPeriod = (_currentPeriod == TimePeriod.Passado) ? TimePeriod.Futuro : TimePeriod.Passado;
        StartCoroutine(TransitionToPeriodRoutine(nextPeriod));
    }

    public void SetPeriodImmediate(TimePeriod period)
    {
        _currentPeriod = period;
        if (_globalVolume != null)
        {
            _globalVolume.profile = (period == TimePeriod.Passado) ? _passadoProfile : _futuroProfile;
            _globalVolume.weight = 1f;
        }
    }

    private IEnumerator TransitionToPeriodRoutine(TimePeriod targetPeriod)
    {
        _isTransitioning = true;

        // 1. Reduz o peso do perfil atual (Fade Out do filtro)
        float elapsed = 0f;
        while (elapsed < _transitionDuration / 2f)
        {
            elapsed += Time.deltaTime;
            _globalVolume.weight = Mathf.Lerp(1f, 0f, elapsed / (_transitionDuration / 2f));
            yield return null;
        }

        // 2. Troca o perfil
        _currentPeriod = targetPeriod;
        _globalVolume.profile = (targetPeriod == TimePeriod.Passado) ? _passadoProfile : _futuroProfile;

        // 3. Aumenta o peso do novo perfil (Fade In do filtro)
        elapsed = 0f;
        while (elapsed < _transitionDuration / 2f)
        {
            elapsed += Time.deltaTime;
            _globalVolume.weight = Mathf.Lerp(0f, 1f, elapsed / (_transitionDuration / 2f));
            yield return null;
        }

        _globalVolume.weight = 1f;
        _isTransitioning = false;
    }
}