namespace DefaultNamespace
{
    public abstract class BaseState<T>
    {
        protected T Controller;

        protected BaseState(T controller)
        {
            this.Controller = controller;
        }

        public abstract void Update();
        public abstract void FixedUpdate();
    }
}