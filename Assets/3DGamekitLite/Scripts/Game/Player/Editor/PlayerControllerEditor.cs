using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Gamekit3D
{
    [CustomEditor(typeof(PlayerController))]
    public class PlayerControllerEditor : Editor
    {
        SerializedProperty m_ScriptProp;

        SerializedProperty m_MaxForwardSpeedProp;
        SerializedProperty m_GravityProp;
        SerializedProperty m_JumpSpeedProp;
        SerializedProperty m_MinTurnSpeedProp;
        SerializedProperty m_MaxTurnSpeedProp;
        SerializedProperty m_IdleTimeoutProp;

        SerializedProperty m_MeleeWeaponProp;
        SerializedProperty m_CameraSettingsProp;
        SerializedProperty m_FootstepPlayerProp;
        SerializedProperty m_HurtAudioPlayerProp;
        SerializedProperty m_LandingPlayerProp;
        SerializedProperty m_EmoteLandingPlayerProp;
        SerializedProperty m_EmoteDeathPlayerProp;
        SerializedProperty m_EmoteAttackPlayerProp;
        SerializedProperty m_EmoteJumpPlayerProp;

        private SerializedProperty m_NormalAttackDuringProp;
        
        private SerializedProperty m_CurrentStateProp;
        private SerializedProperty m_ShootConfigProp;

        private SerializedProperty m_GunHolderProp;
        private SerializedProperty m_Weapon02Prop;
        private SerializedProperty m_Weapon03Prop;

        GUIContent m_ScriptContent = new GUIContent("Script");

        GUIContent m_MaxForwardSpeedContent = new GUIContent("Max Forward Speed", "How fast Ellen can run.");
        GUIContent m_GravityContent = new GUIContent("Gravity", "How fast Ellen falls when in the air.");
        GUIContent m_JumpSpeedContent = new GUIContent("Jump Speed", "How fast Ellen takes off when jumping.");
        GUIContent m_TurnSpeedContent = new GUIContent("Turn Speed", "How fast Ellen turns.  This varies depending on how fast she is moving.  When stationary the maximum will be used and when running at Max Forward Speed the minimum will be used.");
        GUIContent m_IdleTimeoutContent = new GUIContent("Idle Timeout", "How many seconds before Ellen starts considering random Idle poses.");

        GUIContent m_MeleeWeaponContent = new GUIContent("Melee Weapon", "Used for damaging enemies when Ellen swings her staff.");
        GUIContent m_CameraSettingsContent = new GUIContent("Camera Settings", "Used to get the rotation of the current camera so that Ellen faces the correct direction.  Note: This is the only reference which is not part of the Ellen prefab.  It should automatically be set to the Camera Settings script of the CameraRig gameobject when the Prefab is instantiated.");
        GUIContent m_FootstepPlayerContent = new GUIContent("Footstep Random Audio Player", "Used to play a random sound when Ellen takes a step.");
        GUIContent m_HurtAudioPlayerContent = new GUIContent("Hurt Random Audio Player", "Used to play a random sound when Ellen gets hurt.");
        GUIContent m_LandingPlayerContent = new GUIContent("Landing Random Audio Player", "Used to play a random sound when Ellen lands.");
        GUIContent m_EmoteLandingPlayerContent = new GUIContent("Emote Landing Player", "Used to play a random vocal sound when Ellen lands.");
        GUIContent m_EmoteDeathPlayerContent = new GUIContent("Emote Death Player", "Used to play a random vocal sound when Ellen dies.");
        GUIContent m_EmoteAttackPlayerContent = new GUIContent("Emote Attack Player", "Used to play a random vocal sound when Ellen attacks.");
        GUIContent m_EmoteJumpPlayerContent = new GUIContent("Emote Jump Player", "Used to play a random vocal sound when Ellen jumps.");

        GUIContent m_NormalAttackDuringContent = new ("普通攻击间隔");
        
        GUIContent m_CurrentStateContent = new ("当前角色状态");
        GUIContent m_ShootConfigContent = new ("射击状态配置");
        
        GUIContent m_GunHolderContent = new ("枪械握持点");
        GUIContent m_Weapon02Content = new ("二号武器配置");
        GUIContent m_Weapon03Content = new ("三号武器配置");

        void OnEnable()
        {
            m_ScriptProp = serializedObject.FindProperty("m_Script");

            m_MaxForwardSpeedProp = serializedObject.FindProperty("maxForwardSpeed");
            m_GravityProp = serializedObject.FindProperty("gravity");
            m_JumpSpeedProp = serializedObject.FindProperty("jumpSpeed");
            m_MinTurnSpeedProp = serializedObject.FindProperty("minTurnSpeed");
            m_MaxTurnSpeedProp = serializedObject.FindProperty("maxTurnSpeed");
            m_IdleTimeoutProp = serializedObject.FindProperty("idleTimeout");

            m_MeleeWeaponProp = serializedObject.FindProperty("meleeWeapon");
            m_CameraSettingsProp = serializedObject.FindProperty("cameraSettings");
            m_FootstepPlayerProp = serializedObject.FindProperty("footstepPlayer");
            m_HurtAudioPlayerProp = serializedObject.FindProperty("hurtAudioPlayer");
            m_LandingPlayerProp = serializedObject.FindProperty("landingPlayer");
            m_EmoteLandingPlayerProp = serializedObject.FindProperty("emoteLandingPlayer");
            m_EmoteDeathPlayerProp = serializedObject.FindProperty("emoteDeathPlayer");
            m_EmoteAttackPlayerProp = serializedObject.FindProperty("emoteAttackPlayer");
            m_EmoteJumpPlayerProp = serializedObject.FindProperty("emoteJumpPlayer");

            m_NormalAttackDuringProp = serializedObject.FindProperty("normalAttackDuring");
            
            m_CurrentStateProp = serializedObject.FindProperty("currentState");
            m_ShootConfigProp = serializedObject.FindProperty("shootConfig");

            m_GunHolderProp = serializedObject.FindProperty("gunHolder");
            m_Weapon02Prop = serializedObject.FindProperty("weapon02");
            m_Weapon03Prop = serializedObject.FindProperty("weapon03");
        }

        private bool isOpenGunPanel = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_ScriptProp, m_ScriptContent);
            GUI.enabled = true;

            m_MaxForwardSpeedProp.floatValue = EditorGUILayout.Slider(m_MaxForwardSpeedContent, m_MaxForwardSpeedProp.floatValue, 4f, 12f);
            m_GravityProp.floatValue = EditorGUILayout.Slider(m_GravityContent, m_GravityProp.floatValue, 10f, 30f);
            m_JumpSpeedProp.floatValue = EditorGUILayout.Slider(m_JumpSpeedContent, m_JumpSpeedProp.floatValue, 5f, 20f);

            MinMaxTurnSpeed();

            EditorGUILayout.PropertyField(m_IdleTimeoutProp, m_IdleTimeoutContent);

            EditorGUILayout.Space();

            m_MeleeWeaponProp.isExpanded = EditorGUILayout.Foldout(m_MeleeWeaponProp.isExpanded, "References");

            if (m_MeleeWeaponProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_MeleeWeaponProp, m_MeleeWeaponContent);
                EditorGUILayout.PropertyField(m_CameraSettingsProp, m_CameraSettingsContent);
                EditorGUILayout.PropertyField(m_FootstepPlayerProp, m_FootstepPlayerContent);
                EditorGUILayout.PropertyField(m_HurtAudioPlayerProp, m_HurtAudioPlayerContent);
                EditorGUILayout.PropertyField(m_LandingPlayerProp, m_LandingPlayerContent);
                EditorGUILayout.PropertyField(m_EmoteLandingPlayerProp, m_EmoteLandingPlayerContent);
                EditorGUILayout.PropertyField(m_EmoteDeathPlayerProp, m_EmoteDeathPlayerContent);
                EditorGUILayout.PropertyField(m_EmoteAttackPlayerProp, m_EmoteAttackPlayerContent);
                EditorGUILayout.PropertyField(m_EmoteJumpPlayerProp, m_EmoteJumpPlayerContent);
                
                EditorGUILayout.PropertyField(m_NormalAttackDuringProp, m_NormalAttackDuringContent);
                EditorGUILayout.PropertyField(m_CurrentStateProp, m_CurrentStateContent);
                EditorGUILayout.PropertyField(m_ShootConfigProp, m_ShootConfigContent);
                
                EditorGUI.indentLevel--;
            }
            
            isOpenGunPanel = EditorGUILayout.Foldout(isOpenGunPanel, "WeaponControler");
            if (isOpenGunPanel)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.PropertyField(m_GunHolderProp, m_GunHolderContent);
                EditorGUILayout.PropertyField(m_Weapon02Prop, m_Weapon02Content);
                EditorGUILayout.PropertyField(m_Weapon03Prop, m_Weapon03Content);
                
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        void MinMaxTurnSpeed()
        {
            Rect position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

            const float spacing = 4f;
            const float intFieldWidth = 50f;

            position.width -= spacing * 3f + intFieldWidth * 2f;

            Rect labelRect = position;
            labelRect.width *= 0.48f;

            Rect minRect = position;
            minRect.width = 50f;
            minRect.x += labelRect.width + spacing;

            Rect sliderRect = position;
            sliderRect.width *= 0.52f;
            sliderRect.x += labelRect.width + minRect.width + spacing * 2f;

            Rect maxRect = position;
            maxRect.width = minRect.width;
            maxRect.x += labelRect.width + minRect.width + sliderRect.width + spacing * 3f;

            EditorGUI.LabelField(labelRect, m_TurnSpeedContent);
            m_MinTurnSpeedProp.floatValue = EditorGUI.IntField(minRect, (int)m_MinTurnSpeedProp.floatValue);

            float minTurnSpeed = m_MinTurnSpeedProp.floatValue;
            float maxTurnSpeed = m_MaxTurnSpeedProp.floatValue;
            EditorGUI.MinMaxSlider(sliderRect, GUIContent.none, ref minTurnSpeed, ref maxTurnSpeed, 100f, 1500f);
            m_MinTurnSpeedProp.floatValue = minTurnSpeed;
            m_MaxTurnSpeedProp.floatValue = maxTurnSpeed;

            m_MaxTurnSpeedProp.floatValue = EditorGUI.IntField(maxRect, (int)m_MaxTurnSpeedProp.floatValue);
        }
    } 
}
