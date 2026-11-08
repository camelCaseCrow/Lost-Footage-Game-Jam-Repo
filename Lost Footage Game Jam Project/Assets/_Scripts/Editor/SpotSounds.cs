using UnityEngine;

// Dylan picks the E-press sound for each kind of object here (Assets/Spot Sounds.asset).
// The spot tool copies them onto every spot it builds, so they survive Rebuild All Spots.
public class SpotSounds : ScriptableObject
{
    [SerializeField] private AK.Wwise.Event statueSound;
    [SerializeField] private AK.Wwise.Event pewSound;      // all 8 pews, marked or not
    [SerializeField] private AK.Wwise.Event organSound;
    [SerializeField] private AK.Wwise.Event keypadSound;
}
