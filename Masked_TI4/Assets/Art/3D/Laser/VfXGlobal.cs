using UnityEngine;

public class VfXGlobal : MonoBehaviour
{
    public AudioSource SoundEffect;

    public ParticleSystem Particles;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TocarAudio()
    {
        SoundEffect.Play();
    }

    public void PararAudio()
    {
        SoundEffect.Pause();
    }

    public void AtirarParticlas()
    {
        Particles.Play();
    }
}
