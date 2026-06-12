using UnityEngine;

// Ce script se place sur le GameObject du Livreur.
// Il crée automatiquement un carré de couleur flottant au-dessus du véhicule,
// visible uniquement sur la minimap (layer MinimapOnly).
public class MarqueurMinimapLivreur : MonoBehaviour
{
    [Tooltip("Couleur du marqueur du livreur sur la minimap")]
    public Color couleurMarqueur = Color.blue; // Bleu pour distinguer des maisons (rouge)
    public float hauteur = 20f;  // Hauteur au-dessus du livreur
    public float taille = 6f;    // Taille du carré (un peu plus petit que les maisons)

    private GameObject marqueurObj;

    void Start()
    {
        CreerMarqueur();
    }

    void CreerMarqueur()
    {
        // 1. Créer un quad plat au-dessus du livreur
        marqueurObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
        marqueurObj.name = "MarqueurMinimapLivreur";

        DestroyImmediate(marqueurObj.GetComponent<MeshCollider>());

        // 2. Orienter le quad à plat (face vers la caméra minimap qui regarde vers le bas)
        marqueurObj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        marqueurObj.transform.localScale = new Vector3(taille, taille, taille);

        // 3. Rattacher le marqueur au livreur pour qu'il le suive automatiquement
        marqueurObj.transform.SetParent(transform);
        marqueurObj.transform.localPosition = new Vector3(0f, hauteur, 0f);

        // 4. Assigner le layer MinimapOnly
        int layer = LayerMask.NameToLayer("MinimapOnly");
        if (layer == -1)
        {
            Debug.LogError("Layer 'MinimapOnly' introuvable ! Crée-le dans Edit > Project Settings > Tags and Layers");
            return;
        }
        marqueurObj.layer = layer;

        // 5. Supprimer le collider (pas d'interaction physique)
        //Destroy(marqueurObj.GetComponent<MeshCollider>());

        // 6. Appliquer la couleur avec shader Unlit (visible sans lumière)
        Renderer r = marqueurObj.GetComponent<Renderer>();
        if (r != null)
        {
            r.material = new Material(Shader.Find("Unlit/Color"));
            r.material.color = couleurMarqueur;
        }
    }
}
