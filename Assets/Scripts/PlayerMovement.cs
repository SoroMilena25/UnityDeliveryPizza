using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [Tooltip("Vitesse de déplacement du livreur")]
    public float speed = 5.0f;
    [Tooltip("Vitesse de rotation du livreur (plus c'est élevé, plus c'est réactif)")]
    public float rotationSpeed = 10.0f;

    // Référence au gestionnaire pour vérifier si le jeu a démarré
    private GestionnaireLivraison gestionnaire;

    void Start()
    {
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
        transform.position += normalizedMovement * speed * Time.deltaTime;
        // 2. ROTATION (C'est ici que ça se passe !)
        // On calcule la rotation souhaitée vers la direction du mouvement
        Quaternion targetRotation = Quaternion.LookRotation(normalizedMovement);
        // On applique une rotation fluide vers cette cible
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
