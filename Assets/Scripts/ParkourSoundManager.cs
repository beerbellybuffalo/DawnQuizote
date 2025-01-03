using UnityEngine;

public class ParkourSoundManager : MonoBehaviour
{
    public AudioManager audioManager = AudioManager.instance;
    public void playFlipSound()
    {
        audioManager.PlaySFX("Flip");
    }
    public void playStumbleSound()
    {
        audioManager.PlaySFX("Stumble");
    }
}
