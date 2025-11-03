#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Misc
{
    public class SnapToTilemap : MonoBehaviour
    {
        [SerializeField]
        private Tilemap _tilemap;

        [ContextMenu("Snap to Tilemap")]
        public void SnapToTilemapBoundsCenter()
        {
            _tilemap.CompressBounds();
            var tilemapCenter = _tilemap.transform.position + _tilemap.localBounds.center;
            transform.position = new Vector3(tilemapCenter.x, tilemapCenter.y, transform.position.z);
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(transform);
                Undo.RecordObject(transform, "Snap to Tilemap");
            }
#endif
        }
    }
}