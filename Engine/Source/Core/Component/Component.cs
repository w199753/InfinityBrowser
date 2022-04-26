using System;

namespace InfinityEngine
{
    [Serializable]
    public class Component : Object
    {
        public Actor owner;
        internal bool IsConstruct;

        public Component()
        {
            owner = null;
            IsConstruct = true;
        }

        public Component(string name) : base(name)
        {
            owner = null;
            IsConstruct = true;
        }

        public virtual void OnEnable() { }

        public virtual void OnTransform() { }

        public virtual void OnUpdate(in float deltaTime) { }

        public virtual void OnDisable() { }

        protected override void Release()
        {

        }
    }
}
