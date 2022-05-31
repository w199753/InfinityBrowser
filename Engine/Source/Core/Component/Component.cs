using System;

namespace Infinity
{
    [Serializable]
    public class Component : Object
    {
        internal Entity Owner
        { 
            get 
            {
                return m_Owner;
            }
            set 
            {
                m_Owner = value;
            }
        }

        internal bool IsConstruct
        {
            get
            {
                return m_IsConstruct;
            }
            set
            {
                m_IsConstruct = value;
            }
        }

        private Entity m_Owner;
        private bool m_IsConstruct;

        public Component()
        {
            m_Owner = null;
            m_IsConstruct = true;
        }

        public Component(string name) : base(name)
        {
            m_Owner = null;
            m_IsConstruct = true;
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
