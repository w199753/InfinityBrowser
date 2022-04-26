namespace InfinityEngine.Rendering
{
    public class PrimitiveRenderProxy : Disposal
    {
        protected override void Release()
        {

        }
    }

    public class PrimitiveComponent : Component
    {
        public PrimitiveComponent()
        {

        }

        public override void OnEnable()
        {
            OnRegister();
            CreateRender();
        }

        public override void OnUpdate(in float deltaTime)
        {

        }

        public override void OnDisable()
        {
            UnRegister();
            DestroyRender();
        }

        public virtual void OnRegister() { }

        public virtual void UnRegister() { }

        public virtual void CreateRender() { }

        public virtual void UpdateRender() { }

        public virtual void DestroyRender() { }
    }
}
