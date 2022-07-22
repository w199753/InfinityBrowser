using System;
using System.Collections;
using Infinity.Container;
using Infinity.Threading;
using Infinity.Mathmatics;
using System.Runtime.CompilerServices;

namespace Infinity
{
    [Serializable]
    public class Entity : Object, IComparable<Entity>, IEquatable<Entity>
    {
        private Entity m_Parent;
        private int m_TransformHash;
        private FTransform m_Transform;
        private TArray<Entity> m_Childs;
        private TArray<Component> m_Components;
        private CoroutineProcessor m_CoroutineProcessor;

        public Entity()
        {
            m_Parent = null;
            m_Childs = new TArray<Entity>(8);
            m_Components = new TArray<Component>(8);
            m_CoroutineProcessor = new CoroutineProcessor();
        }

        public Entity(string name) : base(name)
        {
            m_Parent = null;
            m_Childs = new TArray<Entity>(8);
            m_Components = new TArray<Component>(8);
            m_CoroutineProcessor = new CoroutineProcessor();
        }

        public Entity(string name, Entity parent) : base(name)
        {
            m_Parent = parent;
            m_Childs = new TArray<Entity>(8);
            m_Components = new TArray<Component>(8);
            m_CoroutineProcessor = new CoroutineProcessor();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return name;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object target)
        {
            return Equals((Entity)target);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Entity target)
        {
            return this == target;
            //return name.Equals(target.name) && m_Parent.Equals(target.m_Parent) && m_Childs.Equals(target.m_Childs) && m_Components.Equals(target.m_Components) && m_Transform.Equals(target.m_Transform);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(Entity target)
        {
            return 0;
        }
        
        public virtual void OnEnable()
        {
            for (int i = 0; i < m_Components.length; ++i)
            {
                m_Components[i].OnEnable();
                m_Components[i].IsConstruct = false;
            }
        }

        protected virtual void OnTransform()
        {
            for (int i = 0; i < m_Components.length; ++i)
            {
                m_Components[i].OnTransform();
            }
        }

        public virtual void OnUpdate(in float deltaTime)
        {
            int transformHash = m_Transform.GetHashCode();
            if (!transformHash.Equals(m_TransformHash)) 
            {
                OnTransform();
                m_TransformHash = transformHash;
            }

            for (int i = 0; i < m_Components.length; ++i)
            {
                if (m_Components[i].IsConstruct)
                {
                    m_Components[i].OnEnable();
                    m_Components[i].IsConstruct = false;
                }

                m_Components[i].OnUpdate(deltaTime);
            }

            m_CoroutineProcessor.OnUpdate(deltaTime);
        }

        public virtual void OnDisable() 
        {
            for (int i = 0; i < m_Components.length; ++i)
            {
                m_Components[i].OnDisable();
            }
        }

        public void SetParent(Entity parent)
        {
            this.m_Parent = parent;
        }

        public void AddComponent<T>(T component) where T : Component
        {
            component.Owner = this;
            m_Components.Add(component);
        }

        public T FindComponent<T>() where T : Component
        {
            for (int i = 0; i < m_Components.length; ++i)
            {
                if (m_Components[i].GetType() == typeof(T))
                {
                    return (T)m_Components[i];
                }
            }

            return null;
        }

        public void RemoveComponent<T>(T component) where T : Component
        {
            for (int i = 0; i < m_Components.length; ++i)
            {
                if (m_Components[i] == component)
                {
                    m_Components.RemoveAtIndex(i);
                }
            }
        }

        public void AddChildActor<T>(T child) where T : Entity
        {
            child.m_Parent = this;
            m_Childs.Add(child);
        }

        public T FindChildActor<T>() where T : Entity
        {
            for (int i = 0; i < m_Childs.length; ++i)
            {
                if (m_Childs[i].GetType() == typeof(T))
                {
                    return (T)m_Childs[i];
                }
            }

            return null;
        }

        public void RemoveChildActor<T>(T child) where T : Entity
        {
            for (int i = 0; i < m_Childs.length; ++i)
            {
                if (m_Childs[i] == child)
                {
                    m_Childs.RemoveAtIndex(i);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CoroutineRef StartCoroutine(IEnumerator routine)
        {
            return m_CoroutineProcessor.Start(routine);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StopCoroutine(in CoroutineRef routine)
        {
            m_CoroutineProcessor.Stop(routine.enumerator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StopAllCoroutine()
        {
            m_CoroutineProcessor.StopAll();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetActorPosition(in float3 position)
        {
            m_Transform.position = position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetActorRotation(in quaternion rotation)
        {
            m_Transform.rotation = rotation;
        }
    }
}
