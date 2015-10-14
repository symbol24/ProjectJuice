using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(shield))]
public class ShieldEditor : Editor {
    string[] _listOfClips;

    int[] _choice = new int[7];

    string[] _listOFParticles;

    int[] _particle = new int[2];

    public override void OnInspectorGUI()
    {
        _listOfClips = EditorUtilities.GetListOfAudioClips();
        _listOFParticles = EditorUtilities.GetListOfParticles();
        DrawDefaultInspector();

        var leShield = target as shield;

        _choice[0] = EditorUtilities.GetSelectedClip(_listOfClips, leShield.Activate);
        _choice[1] = EditorUtilities.GetSelectedClip(_listOfClips, leShield.AbsorbBullet);
        _choice[2] = EditorUtilities.GetSelectedClip(_listOfClips, leShield.ShootingBullets);
        _choice[3] = EditorUtilities.GetSelectedClip(_listOfClips, leShield.FullCharge);
        _choice[4] = EditorUtilities.GetSelectedClip(_listOfClips, leShield.Ricochet);
        _choice[5] = EditorUtilities.GetSelectedClip(_listOfClips, leShield.CoolDown);
        _choice[6] = EditorUtilities.GetSelectedClip(_listOfClips, leShield.AbrosbExplosion);

        _particle[0] = EditorUtilities.GetSelectedParticle(leShield.m_MuzzleFlash);
        _particle[1] = EditorUtilities.GetSelectedParticle(leShield.m_MuzzleSmoke);

        _choice[0] = EditorGUILayout.Popup("Activate Shield SFX", _choice[0], _listOfClips);
        _choice[1] = EditorGUILayout.Popup("Absorb Bullet SFX", _choice[1], _listOfClips);
        _choice[2] = EditorGUILayout.Popup("Shoot Bullets SFX", _choice[2], _listOfClips);
        _choice[3] = EditorGUILayout.Popup("Full Charge SFX", _choice[3], _listOfClips);
        _choice[4] = EditorGUILayout.Popup("Ricochet? SFX", _choice[4], _listOfClips);
        _choice[5] = EditorGUILayout.Popup("Cooldown SFX", _choice[5], _listOfClips);
        _choice[6] = EditorGUILayout.Popup("Abesorb Explosion SFX", _choice[6], _listOfClips);

        _particle[0] = EditorGUILayout.Popup("Muzzle Flash", _particle[0], _listOFParticles);
        _particle[1] = EditorGUILayout.Popup("Muzzle SMoke", _particle[1], _listOFParticles);

        leShield.Activate = _listOfClips[_choice[0]];
        leShield.AbsorbBullet = _listOfClips[_choice[1]];
        leShield.ShootingBullets = _listOfClips[_choice[2]];
        leShield.FullCharge = _listOfClips[_choice[3]];
        leShield.Ricochet = _listOfClips[_choice[4]];
        leShield.CoolDown = _listOfClips[_choice[5]];
        leShield.AbrosbExplosion = _listOfClips[_choice[6]];

        leShield.m_MuzzleFlash = Database.instance.Particles[_particle[0]];
        leShield.m_MuzzleSmoke = Database.instance.Particles[_particle[1]];

        EditorUtility.SetDirty(target);
    }

}
