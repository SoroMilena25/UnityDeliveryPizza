using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Paramètres de vitesse")]
    [Tooltip("Vitesse de déplacement de base (hors de la route)")]
    public float baseSpeed = 5.0f;

    [Tooltip("Vitesse rapide quand le livreur roule SUR la route")]
    public float roadSpeed = 10.0f;

    [Tooltip("Vitesse de rotation du livreur")]
    public float rotationSpeed = 10.0f;

    // Cette variable stocke la vitesse appliquée à l'instant T
    private float currentSpeed;

    // --- NOUVEAU : Le compteur pour lisser la transition entre les morceaux de route ---
    private int routesTouchees = 0;

    // Référence au gestionnaire pour vérifier si le jeu a démarré
    private GestionnaireLivraison gestionnaire;

    void Start()
    {
        // On considère qu'il commence hors de la route
        currentSpeed = baseSpeed;

        // On récupère le gestionnaire pour pouvoir vérifier l'état du jeu
        gestionnaire = FindAnyObjectByType<GestionnaireLivraison>();
    }

    void Update()
    {
        // On bloque tous les inputs tant que le joueur n'a pas cliqué pour commencer
        if (gestionnaire == null || !gestionnaire.jeuDemarre || gestionnaire.jeuTermine) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        Vector3 movement = Vector3.zero;

        // Récupération des entrées
        if (keyboard.upArrowKey.isPressed) movement += Vector3.forward;
        if (keyboard.downArrowKey.isPressed) movement += Vector3.back;
        if (keyboard.leftArrowKey.isPressed) movement += Vector3.left;
        if (keyboard.rightArrowKey.isPressed) movement += Vector3.right;

        // Si aucun mouvement n'est détecté, on s'arrête là
        if (movement == Vector3.zero) return;

        // Normalisation pour éviter d'aller plus vite en diagonale
        Vector3 normalizedMovement = movement.normalized;

        // 1. DÉPLACEMENT
        transform.position += normalizedMovement * currentSpeed * Time.deltaTime;

        // 2. ROTATION
        Quaternion targetRotation = Quaternion.LookRotation(normalizedMovement);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // --- DETECTION DES COLLISIONS ---

    // Se déclenche quand le livreur entre en contact avec un autre collider
    private void OnCollisionEnter(Collision collision)
    {
        // On vérifie si l'objet touché a le tag "Road"
        if (collision.gameObject.CompareTag("Road"))
        {
            routesTouchees++; // On ajoute cette portion de route au compteur
            currentSpeed = roadSpeed; // BOOST ! On passe à la vitesse supérieure
        }
    }

    // Se déclenche quand le livreur quitte le contact avec le collider
    private void OnCollisionExit(Collision collision)
    {
        // On vérifie si l'objet qu'on vient de quitter a le tag "Road"
        if (collision.gameObject.CompareTag("Road"))
        {
            routesTouchees--; // On retire cette portion de route du compteur

            // Sécurité : on s'assure que le compteur ne passe jamais en dessous de 0
            if (routesTouchees < 0)
            {
                routesTouchees = 0;
            }

            // Si on ne touche PLUS AUCUNE portion de route, on ralentit
            if (routesTouchees == 0)
            {
                currentSpeed = baseSpeed; // On quitte définitivement la route
            }
        }
    }
}