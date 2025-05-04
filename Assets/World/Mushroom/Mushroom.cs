using System;
using System.Collections;
using SoundManager;
using TMPro;
using UnityEngine;
using World;

[RequireComponent(typeof(AudioSource))]
public class Mushroom : MonoBehaviour, IInteractable
{
    [SerializeField] private MushroomData data;
    [SerializeField] private GameObject popupUI;
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private GameObject pickupEffect;
    private Camera playerCamera;  
    private AudioSource audioSource;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        data ??= Resources.Load<MushroomData>("DefaultMushroom");
    }

    void Start()
    {
        playerCamera = Camera.main;
        if (popupUI != null)
        {
            popupUI.SetActive(false);  
        }
    }

    void Update()
    {
        if (popupUI.activeSelf && playerCamera)
        {

            Vector3 directionToCamera = playerCamera.transform.position - popupUI.transform.position;
            directionToCamera.y = 0; 

            popupUI.transform.rotation = Quaternion.LookRotation(directionToCamera);

        }
    }

    public string GetInteractionText()
    {
        return data.displayName + "\n" + data.description;
    }

    public void Interact()
    {
        if (gameObject != null)
        {
            
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            if (popupUI != null)
            {
                popupUI.SetActive(false); 
            }
            PlayPickupSound();
            StartCoroutine(ShrinkAndDestroy());
            
        }
    }
    
    private IEnumerator ShrinkAndDestroy()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 initialScale = transform.localScale;

        if (TryGetComponent<Collider>(out var col))
            col.enabled = false; 
        PlayPickUpEffect(pickupEffect, duration, true);
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    void PlayPickUpEffect(GameObject  effectPrefab,float duration, bool includeGlowingColor = false)
    {
        if (!effectPrefab) return;
        GameObject effect = Instantiate(pickupEffect, transform.position, Quaternion.identity);
        if (!effect) return;
        
        if (includeGlowingColor && effect.TryGetComponent<ParticleSystem>(out var ps))
        {
            // It doesnt work 
            var main = ps.main;
            main.startColor = data.glowColor;
        }
        Destroy(effect, duration*2); 
        
    }

    public void ShowPopUp()
    {
        if (!popupUI) return;
        popupUI.SetActive(true);
        textMesh.text = GetInteractionText();
    }

    public void HidePopUp()
    {
        if (popupUI)
        {
            popupUI.SetActive(false);
        }
    }

    public void SetPlayerCamera(Camera camera)
    {
        playerCamera = camera;
    }

    public bool isValid()
    {
        return this != null && gameObject != null; 
    }

    public void setActive(bool isActive)
    {
        if (isActive)
        {
            ShowPopUp();
        }
        else
        {
            HidePopUp();
        }
    }

    private void PlayPickupSound()
    {
        SoundManager.SoundManager.PlaySound(SoundType.Pickup, audioSource);
    }
}
