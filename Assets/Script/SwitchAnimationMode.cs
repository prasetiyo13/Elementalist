using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SwitchAnimationMode : MonoBehaviour {
    Animator anim;
    CharacterController player;

    [SerializeField]
    Transform rightHand;
    [SerializeField]
    Transform swordEquipRoot;
    [SerializeField]
    Transform swordSheathRoot;
    void Awake () {
        anim = GetComponent<Animator>();
        player = GetComponent<CharacterController>();
    }

    public void DrawWeapon(string tolayerName) {
        
        int currentLayer = 0;
        int targetLayer = anim.GetLayerIndex(tolayerName);
        if(targetLayer < 0) {
            Debug.LogWarning("Layer Name : " + tolayerName + " not found.");
            return;
        }
        anim.SetLayerWeight(targetLayer, 1f);
        anim.SetLayerWeight(currentLayer, 0f);

        player.CharacterMode = (CharacterController.AnimationMode) targetLayer;
        swordEquipRoot.parent = rightHand;
    }
    public void SheathWeapon(string fromLayerName) {
        
        int currentLayer = anim.GetLayerIndex(fromLayerName);
        int targetLayer = 0;
        if (currentLayer < 0) {
            Debug.LogWarning("Layer Name : " + fromLayerName + " not found.");
            return;
        }
        anim.SetLayerWeight(targetLayer, 1f);
        anim.SetLayerWeight(currentLayer, 0f);

        player.CharacterMode = (CharacterController.AnimationMode) targetLayer;
        swordEquipRoot.parent = swordSheathRoot;
    }

}
