using System;

namespace ScriptableObjects
{
    public interface IConfigObject
    { 
        public event Action OnUpdated;
    }
}