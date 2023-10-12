using UnityEngine;

namespace Piece
{
    public class ObjectID : MonoBehaviour
    {
        public string ID { get; private set; }
        public string OwnerID { get; private set; }
        
        public void Initialize(string objectID, string ownerID)
        {
            ID = objectID;
            OwnerID = ownerID;
        }
    }
}