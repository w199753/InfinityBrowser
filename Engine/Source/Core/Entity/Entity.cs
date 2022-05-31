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
        public Entity parent;
        public FTransform transform;

        private FTransform m_LastTransform;
        private CoroutineProcessor m_CoroutineProcessor;

        internal TArray<Entity> childs;
        internal TArray<Component> components;

        public Entity()
        {
            this.parent = null;
            this.childs = new TArray<Entity>(8);
            this.components = new TArray<Component>(8);
            this.m_CoroutineProcessor = new CoroutineProcessor();
        }

        public Entity(string name) : base(name)
        {
            this.parent = null;
            this.childs = new TArray<Entity>(8);
            this.components = new TArray<Component>(8);
            this.m_CoroutineProcessor = new CoroutineProcessor();
        }

        public Entity(string name, Entity parent) : base(name)
        {
            this.parent = parent;
            this.childs = new TArray<Entity>(8);
            this.components = new TArray<Component>(8);
            this.m_CoroutineProcessor = new CoroutineProcessor();
        }

        public override string ToString()
        {
            return name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object target)
        {
            return Equals((Entity)target);
        }

        public bool Equals(Entity target)
        {
            return name.Equals(target.name) && parent.Equals(target.parent) && childs.Equals(target.childs) && components.Equals(target.components) && transform.Equals(target.transform);
        }

        public int CompareTo(Entity target)
        {
            return 0;
        }
        
        public virtual void OnEnable()
        {
            for (int i = 0; i < components.length; ++i)
            {
                components[i].OnEnable();
                components[i].IsConstruct = false;
            }
        }

        public virtual void OnTransform()
        {
            for (int i = 0; i < components.length; ++i)
            {
                components[i].OnTransform();
            }
        }

        public virtual void OnUpdate(in float deltaTime)
        {
            if (!transform.Equals(m_LastTransform)) 
            {
                OnTransform();
                m_LastTransform = transform;
            }

            for (int i = 0; i < components.length; ++i)
            {
                if (components[i].IsConstruct)
                {
                    components[i].OnEnable();
                    components[i].IsConstruct = false;
                }

                components[i].OnUpdate(deltaTime);
            }

            m_CoroutineProcessor.OnUpdate(deltaTime);
        }

        public virtual void OnDisable() 
        {
            for (int i = 0; i < components.length; ++i)
            {
                components[i].OnDisable();
            }
        }

        public void SetParent(Entity parent)
        {
            this.parent = parent;
        }

        public void AddComponent<T>(T component) where T : Component
        {
            component.owner = this;
            components.Add(component);
        }

        public T FindComponent<T>() where T : Component
        {
            for (int i = 0; i < components.length; ++i)
            {
                if (components[i].GetType() == typeof(T))
                {
                    return (T)components[i];
                }
            }

            return null;
        }

        public void RemoveComponent<T>(T component) where T : Component
        {
            for (int i = 0; i < components.length; ++i)
            {
                if (components[i] == component)
                {
                    components.RemoveAtIndex(i);
                }
            }
        }

        public void AddChildActor<T>(T child) where T : Entity
        {
            child.parent = this;
            childs.Add(child);
        }

        public T FindChildActor<T>() where T : Entity
        {
            for (int i = 0; i < childs.length; ++i)
            {
                if (childs[i].GetType() == typeof(T))
                {
                    return (T)childs[i];
                }
            }

            return null;
        }

        public void RemoveChildActor<T>(T child) where T : Entity
        {
            for (int i = 0; i < childs.length; ++i)
            {
                if (childs[i] == child)
                {
                    childs.RemoveAtIndex(i);
                }
            }
        }

        public CoroutineRef StartCoroutine(IEnumerator routine)
        {
            return m_CoroutineProcessor.Start(routine);
        }

        public void StopCoroutine(in CoroutineRef routine)
        {
            m_CoroutineProcessor.Stop(routine.enumerator);
        }

        public void StopAllCoroutine()
        {
            m_CoroutineProcessor.StopAll();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetActorPosition(in float3 position)
        {
            transform.position = position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetActorRotation(in quaternion rotation)
        {
            transform.rotation = rotation;
        }
    }
}
