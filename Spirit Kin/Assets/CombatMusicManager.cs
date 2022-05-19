using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMusicManager : MonoBehaviour
{
    [SerializeField] AudioSource combatBGMPlayer;

    public enum MusicState {Off, Begin, Loop, End};

    public MusicState combatBGM;

    public bool playerBeingChased;

    [Header("Parts of BGM")]
    [SerializeField] AudioClip beginCombatBGM;
    [SerializeField] AudioClip loopCombatBGM;
    [SerializeField] AudioClip endCombatBGM;



    // Start is called before the first frame update
    void Start()
    {
        combatBGM = MusicState.Off;
    }

    // Update is called once per frame
    void Update()
    {
        switch (combatBGM)
        {
            case MusicState.Off:
                if (playerBeingChased)
                {
                    changeMusicState(MusicState.Begin);
                    break;
                }
                if (combatBGMPlayer.isPlaying)
                    combatBGMPlayer.Stop(); //should fade out and then stop -> thats a polish step
                break;
            case MusicState.Begin:
                if (!combatBGMPlayer.isPlaying)
                {
                    changeMusicState(MusicState.Loop);
                    break;
                }
                    
                if (!playerBeingChased)
                {
                    changeMusicState(MusicState.End);
                    break;
                }
                break;
            case MusicState.Loop:
                if (!playerBeingChased)
                    changeMusicState(MusicState.End);
                break;
            case MusicState.End:
                if (!combatBGMPlayer.isPlaying)
                    changeMusicState(MusicState.Off);
                break;
            default:
                break;                                            
        }
    }

    public void changeMusicState(MusicState newState)
    {
        if (newState == combatBGM) return;

        if (newState == MusicState.Begin) { combatBGMPlayer.clip = beginCombatBGM; combatBGMPlayer.loop = false; combatBGMPlayer.Play();}
        else if (newState == MusicState.Loop) { combatBGMPlayer.clip = loopCombatBGM; combatBGMPlayer.loop = true; combatBGMPlayer.Play();}
        else if (newState == MusicState.End) { combatBGMPlayer.clip = endCombatBGM; combatBGMPlayer.loop = false; combatBGMPlayer.Play();}

        combatBGM = newState;
    }
}
